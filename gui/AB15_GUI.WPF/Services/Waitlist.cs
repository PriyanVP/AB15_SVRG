using System;
using System.Collections.Generic;
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
    /// Add item to waitlist. Will return message id
    /// </summary>
    /// <param name="msgID">message ID</param>
    /// <param name="isAddedSuccessfully">flag to tell if item was added to waitlist</param>
    /// <param name="deleg">delegate that will be called for received msg</param>
    /// <param name="payloadType">payload type</param>
    /// <param name="isContinuous">flag to define if MCU response can be received few times</param>
    /// <returns>pair of msgID and isAddedSuccessfully</returns>
    public (int msgID, bool isAddedSuccessfully) AddItemToWaitlist(Action<IReceiveCommunicationPackage> deleg, Type payloadType, bool isContinuous = false)
    {
        // Check that payloadType is implementing expected interface
        // TODO: check why Contract usage breaks application if not in debug mode
        // Contract.Requires<ArgumentException>(payloadType.GetInterface(nameof(IByteListSerializable)) != null, "Incorrect payloadType. Should implement IByteListSerializable.");

        // Generate unique message ID
        int msgID = GenerateUniqueMsgID();

        // Check if item with same msgID already exist or if payload type is not correct
        if (_waitlist.Exists(itm => itm.msgID == msgID) || (payloadType.GetInterface(nameof(IByteListSerializable)) is null))
        {
            return (msgID: msgID, isAddedSuccessfully: false);
        }

        // Add item to list
        _waitlist.Add(new WaitlistItem(deleg, payloadType, msgID: msgID, isContinuous: isContinuous));

        return (msgID: msgID, isAddedSuccessfully: true);
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
            // Call was incorrect (no valid msgID supplied)
            // or list is empty
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
        return _waitlist.Remove(itmToDelete);
    }

    /// <summary>
    /// Removs all items from waitlist
    /// </summary>
    /// <param name="msgID">message ID</param>
    /// <returns>true if removal was successful, false - otherwise</returns>
    public void ClearWaitlist()
    {
        _waitlist.Clear();
    }

    /// <summary>
    /// Gets payload type stored in waitlist item
    /// </summary>
    /// <param name="msgID">message ID in package from MCU</param>
    /// <returns>Type of package payload or null if no waitlist item was found</returns>
    public Type? GetPayloadType(int msgID)
    {
        // Create temporary variables
        int tmpMsgID = msgID & (~(int)MsgIDMasks.ResponseBit); // clear response bit; can't be null!
        WaitlistItem? waitlistItem = _waitlist.Find(itm => itm.msgID == tmpMsgID);

        // Assign delegate and remove item from waitlist if conditions are met
        if (waitlistItem is null)
        {
            return null;
        }

        return waitlistItem.payloadType;
    }

    /// <summary>
    /// Looks for delegate that will handle package and removes relevant item from waitlist
    /// </summary>
    /// <param name="receivedPackage">package from MCU</param>
    /// <returns>Delegate that should be called for this package or null if delegate is not found</returns>
    public Action<IReceiveCommunicationPackage>? GetDelegate(IReceiveCommunicationPackage receivedPackage)
    {
        // Return empty array if package not valid
        if (receivedPackage.IsPackageValid != true)
        {
            logger.Error($"Invalid package received! Package: {receivedPackage}");
            return null;
        }

        // Create temporary variables
        Action<IReceiveCommunicationPackage>? returnDelegate = null;
        int msgID = receivedPackage.MsgID & (~(int)MsgIDMasks.ResponseBit); // clear response bit; can't be null!
        WaitlistItem? waitlistItem = _waitlist.Find(itm => itm.msgID == msgID);

        // Assign delegate and remove item from waitlist if conditions are met
        if (waitlistItem is not null)
        {
            // Fill output variable
            returnDelegate = waitlistItem.deleg;

            // Remove item from waitlist, if not continuous
            // All checks for removal are passed at this point
            if (waitlistItem.isContinuous == false)
            {
                _waitlist.Remove(waitlistItem);
            }
        }

        return returnDelegate;
    }

    /// <summary>
    /// Generate unique MSG ID (not present in current waitlist) for new message to MCU
    /// </summary>
    /// <returns>msg ID if unoccupied IDs present, 0xFFFF - otherwise</returns>
    private int GenerateUniqueMsgID()
    {
        // Init loacal variables
        int msgID = 0x0;
        int maxValueForID = ((int)MsgIDMasks.GeneralPurposeBits);

        // Loop through available Ids in waitlist till found minimal not occupied
        for (msgID = 0; msgID <= maxValueForID; msgID++)
        {
            // Exit loop if found unoccupied msg ID
            if (_waitlist.Exists(itm => itm.msgID == msgID) == false) break;
        }

        // No available IDs - error situation, communication not possible
        if (msgID > maxValueForID)
        {
            throw new InvalidOperationException("All msgIDs in waitlist are occupied!");
        }

        return msgID;
    }
   
    /// <summary>
    /// Removes outdated items from waitlist and returns list of their delegates
    /// </summary>
    /// <returns>List with outdated commands delegates and payload types</returns>
    public List<(Action<IReceiveCommunicationPackage> deleg, Type? payloadType)> RemoveOutdatedItems()
    {
        DateTime timeNow = DateTime.Now;
        List<(Action<IReceiveCommunicationPackage> deleg, Type? payloadType)> outdatedItems = new List<(Action<IReceiveCommunicationPackage> deleg, Type? payloadType)>();
        List<WaitlistItem> itemsForRemove = new List<WaitlistItem>();

        lock(_waitlist)
        {
            // Loop through waitlist to check waht items are outdated
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

                        // Add package to list
                        outdatedItems.Add((item.deleg, item.payloadType));
                    }
                }
            }

            // Remove outdated items
            foreach(WaitlistItem itm in itemsForRemove)
            {
                _waitlist.Remove(itm);
            }
        }

        return outdatedItems;
    }
    
    /// <summary>
    /// Class representing item in MCU response waitlist
    /// </summary>
    internal class WaitlistItem
    {
        /// <summary>
        /// Assign values to attributes
        /// </summary>
        /// <param name="deleg">delegate that will be called for received msg</param>
        /// <param name="msgID">message ID of send (!) transaction. Used to define which consumer will handle response from MCU</param>
        /// <param name="isContinuous">flag to define if MCU response can be received few times</param>
        public WaitlistItem(Action<IReceiveCommunicationPackage> deleg, Type payloadType, int? msgID = null, bool isContinuous = false)
        {
            this.deleg = deleg;
            this.msgID = msgID;
            this.isContinuous = isContinuous;
            this.payloadType = payloadType;
            creationTime = DateTime.Now;
        }

        /// <summary>
        /// Type of payload used in package
        /// </summary>
        public Type payloadType;

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
        /// Delegate that will be called for received msg
        /// </summary>
        public Action<IReceiveCommunicationPackage> deleg;

        /// <summary>
        /// Flag to define if MCU response can be received few times
        /// </summary>
        public bool isContinuous;
    }
}