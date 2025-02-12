using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Services.Interfaces;

namespace AB15_GUI.WPF.Services;

/// <summary>
/// Class implementing waitlist for routing responses from MCU
/// </summary>
public class Waitlist : IWaitlist
{
    /// <summary>
    /// Time span during which response can be received
    /// </summary>
    private const int ItemTimeOfLife = 5;

    /// <summary>
    /// Local reference to logger
    /// </summary>
    private ILoggingService logger;

    /// <summary>
    /// Lock object for handling multithreading
    /// </summary>
    private readonly object _lock = new object();

    /// <summary>
    /// Waitlist for storing and handling records for received messages from MCU
    /// </summary>
    /// <typeparam name="WaitlistItem">record in waitlist</typeparam>
    private List<WaitlistItem> _waitlist = new List<WaitlistItem>();

    /// <summary>
    /// Construct waitlist and fill with default items
    /// </summary>
    public Waitlist(ILoggingService logger)
    {
        // Init logger
        this.logger = logger;
    }

    /// <summary>
    /// Add item to waitlist. Will return message id and awaitable task
    /// </summary>
    /// <param name="msgID">message ID</param>
    /// <param name="task">task to be awaited by sender</param>
    /// <param name="payloadType">payload type</param>
    /// <param name="isContinuous">flag to define if MCU response can be received few times</param>
    /// <returns>pair of msgID and task; will be nulls if unsuccessful</returns>
    public (int? msgID, Task<IReceiveCommunicationPackage>? task) AddItemToWaitlist(Type payloadType, bool isContinuous = false)
    {
        // Validate input
        if (payloadType.GetInterface(nameof(IByteListSerializable)) is null)
        {
            logger.Error("Tried to add item to waitlist with invalid payload type!");
            return (msgID: null, task: null);
        }

        // Local variables
        int? msgID = null;
        IReceiveCommunicationPackage receivedPackageObject;
        TaskCompletionSource<IReceiveCommunicationPackage> taskCompletionSource;

        // Avoid possibility of threading issues
        lock (_lock)
        {
            msgID = GenerateUniqueMsgID();

            try
            {
                // Create received package instance dynamically based on type
                Type packageType = typeof(ReceiveCommunicationPackage<>).MakeGenericType(payloadType);
                receivedPackageObject = (IReceiveCommunicationPackage) Activator.CreateInstance(packageType);

                // Create received package instance dynamically based on type
                Type taskType =  typeof(TaskCompletionSource<>).MakeGenericType(typeof(IReceiveCommunicationPackage));
                taskCompletionSource = (TaskCompletionSource<IReceiveCommunicationPackage>) Activator.CreateInstance(taskType);

                // Skip if any of the required parameters is null
                if ((msgID is not null) && (receivedPackageObject is not null) && (taskCompletionSource is not null))
                {
                    _waitlist.Add(new WaitlistItem(receivedPackageObject, taskCompletionSource, msgID: msgID, isContinuous: isContinuous));
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error while creating task or received package object: {ex.Message}");
                msgID = null;
                taskCompletionSource = null;
            }
        }

        return (msgID: msgID, task: taskCompletionSource?.Task);
    }

    /// <summary>
    /// Handle responses from MCU
    /// </summary>
    /// <param name="package">raw package from MCU</param>
    /// <returns>Nothing</returns>
    public void HandleResponse(List<byte> package) 
    {
        // Create temporary variables
        int msgID = package[SerialPackageConstants.MsgIDPosition] & (~(int)MsgIDMasks.ResponseBit); // clear response bit; can't be null!

        lock (_lock)
        {
            WaitlistItem? waitlistItem = _waitlist.Find(itm => itm.msgID == msgID);

            // No item found in waitlist
            if (waitlistItem is null)
            { 
                return;
            }

            // Get response object
            IReceiveCommunicationPackage tmpReceivedPackage = waitlistItem.receivedPackage;

            // Convert raw package received from MCU to field values
            bool isPackageValid = tmpReceivedPackage!.UnpackPackage(package);

            // If package is not valid - skip
            if (isPackageValid == false)
            {
                logger.Info("Received invalid message");
                return;
            }

            // Report response (await finished) and remove item from waitlist if conditions are met
            waitlistItem.taskCompletionSource.SetResult(tmpReceivedPackage);

            // Remove item from waitlist, if not continuous
            // All checks for removal are passed at this point
            if (waitlistItem.isContinuous == false)
            {
                _waitlist.Remove(waitlistItem);
            }
            else 
            {
                // Continuous item - create new instances of received package and task
                waitlistItem.receivedPackage = (IReceiveCommunicationPackage) Activator.CreateInstance(tmpReceivedPackage.GetType());
                waitlistItem.taskCompletionSource = (TaskCompletionSource<IReceiveCommunicationPackage>) Activator.CreateInstance(waitlistItem.taskCompletionSource.GetType());
            }
        }
    }

    /// <summary>
    /// Get task instance from waitlist by ID
    /// To be used for continuous communication (few answers on one request)
    /// </summary>
    /// <param name="msgID">message ID</param>
    /// <returns>Task with awaitable response or Task with null response if item not found</returns>
    public Task<IReceiveCommunicationPackage?> GetContinuousTaskInstance(int? msgID)
    {
        // Handling null input scenario (valid option)
        if (msgID is null) return Task.FromResult<IReceiveCommunicationPackage?>(null);

        // Avoid possibility of threading issues
        lock (_lock)
        {
            // Find item in waitlist
            WaitlistItem? waitlistItem = _waitlist.Find(itm => itm.msgID == msgID);

            // If no item found in waitlist will return null as task result
            return waitlistItem?.taskCompletionSource.Task ?? Task.FromResult<IReceiveCommunicationPackage?>(null);
        }
    }

    /// <summary>
    /// Remove item from waitlist by ID. Intended for removing continuous items
    /// </summary>
    /// <param name="msgID">message ID</param>
    /// <returns>true if removal was successful, false - otherwise</returns>
    public bool RemoveItemFromWaitlist(int? msgID)
    {
        // Check if deletion can be performed
        if ((_waitlist.Count == 0) || (msgID == null))
        {
            // Call was incorrect (no valid msgID supplied) or list is empty
            logger.Error("Tried to remove item from empty waitlist or item ID wasn't provided!");
            return false;
        }

        // Check if item with msgID exists
        WaitlistItem? itmToDelete = _waitlist.Find(itm => itm.msgID == msgID);
        if (itmToDelete is null)
        {
            // Item with msgID wasn't found
            logger.Error("Tried to remove unexisting item from waitlist!");
            return false;
        }

        // Deletes item and returns true if successful
        bool isRemovedSuccesfully;
        lock (_lock)
        {
            // Report status // TODO: check
            itmToDelete.receivedPackage!.UnpackPackage(new List<byte>() { (byte)MCUStatus.RESPONSE_ABSENT });

            // Report response (await finished) and remove item from waitlist if conditions are met
            itmToDelete.taskCompletionSource.SetResult(itmToDelete.receivedPackage);

            // Perform removal
            isRemovedSuccesfully = _waitlist.Remove(itmToDelete);
        }

        return isRemovedSuccesfully;
    }
 
    /// <summary>
    /// Removes outdated items from waitlist
    /// </summary>
    /// <returns>Nothing</returns>
    public void RemoveOutdatedItems()
    {
        DateTime timeNow = DateTime.Now;
        List<WaitlistItem> itemsForRemove = new List<WaitlistItem>();

        lock(_waitlist)
        {
            // Loop through waitlist to check what items are outdated
            foreach(WaitlistItem item in _waitlist)
            {
                if ((timeNow - item.creationTime).TotalSeconds > ItemTimeOfLife)
                {
                    // Continuous packages
                    if (item.isContinuous)
                    {
                        // Refresh creation time
                        item.creationTime = DateTime.Now;
                    }
                    // Regular packages
                    else
                    {
                        // Add item to remove list
                        itemsForRemove.Add(item);

                        // Report status
                        item.receivedPackage!.UnpackPackage(new List<byte>() { (byte)MCUStatus.RESPONSE_ABSENT });

                        // Report response (await finished) and remove item from waitlist if conditions are met
                        item.taskCompletionSource.SetResult(item.receivedPackage);
                    }
                }
            }

            // Remove outdated items
            foreach(WaitlistItem itm in itemsForRemove)
            {
                _waitlist.Remove(itm);
            }
        }
    }

    /// <summary>
    /// Remove all items from waitlist
    /// WARNING: may cause stuck of await operations
    /// </summary>
    /// <param name="msgID">message ID</param>
    /// <returns>true if removal was successful, false - otherwise</returns>
    public void ClearWaitlist()
    {
        lock (_lock)
        {
            _waitlist.Clear();
        }
    }

    /// <summary>
    /// Generate unique MSG ID (not present in current waitlist) for new message to MCU
    /// </summary>
    /// <returns>msg ID if unoccupied IDs present, null - otherwise</returns>
    private int? GenerateUniqueMsgID()
    {
        // Init local variables
        int msgID = 0x0;
        int maxValueForID = ((int)MsgIDMasks.GeneralPurposeBits);

        // Loop through available Ids in waitlist till found minimal not occupied
        for (msgID = 0; msgID <= maxValueForID; msgID++)
        {
            // Exit loop if found unoccupied msg ID
            if (_waitlist.Exists(itm => itm.msgID == msgID) == false) break;
        }

        // No available IDs - error situation, communication not possible
        if (msgID >= maxValueForID)
        {
            return null;
        }

        return msgID;
    }
    
    /// <summary>
    /// Class representing item in MCU response waitlist
    /// </summary>
    internal class WaitlistItem
    {
        /// <summary>
        /// Assign values to attributes
        /// </summary>
        /// <param name="receivedPackage">object to use for reporting received package</param>
        /// <param name="taskCompletionSource">task that will be used for reporting received msg</param>
        /// <param name="msgID">message ID of send (!) transaction. Used to define which consumer will handle response from MCU</param>
        /// <param name="isContinuous">flag to define if MCU response can be received few times</param>
        public WaitlistItem(IReceiveCommunicationPackage receivedPackage, TaskCompletionSource<IReceiveCommunicationPackage> taskCompletionSource, int? msgID = null, bool isContinuous = false)
        {
            this.receivedPackage = receivedPackage;
            this.taskCompletionSource = taskCompletionSource;
            this.msgID = msgID;
            this.isContinuous = isContinuous;
            creationTime = DateTime.Now;
        }

        /// <summary>
        /// Time when item was created or updated (for continuous)
        /// </summary>
        public DateTime creationTime;

        /// <summary>
        /// Message ID of send (!) transaction. Used to define which consumer will handle response from MCU
        /// Reserved value null is used for transactions that are always handled, even without previous transaction from GUI (ack, error, ...)
        /// </summary>
        public int? msgID;

        /// <summary>
        /// Task for handling received package
        /// </summary>
        public TaskCompletionSource<IReceiveCommunicationPackage> taskCompletionSource;

        /// <summary>
        /// Received package object
        /// </summary>
        public IReceiveCommunicationPackage receivedPackage;

        /// <summary>
        /// Flag to define if MCU response can be received few times
        /// </summary>
        public bool isContinuous;
    }
}