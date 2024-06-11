using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using SerialDataTypesNamespace;
using CommonNamespace;
using ModernUI.Back;

namespace SerialWrapperNamespace;

/// <summary>
/// Class implementing waitlist for routing responses from MCU
/// </summary>
public class Waitlist
{
    /// <summary>
    /// Construct waitlist and fill with default items
    /// </summary>
    public Waitlist(Logger log)
    {
        // Init logger
        _log = log;

        _waitlist = new List<WaitlistItem>();
        errorCounter = 0;

        InitWaitlist();
    }

    private List<WaitlistItem> _waitlist;
    public int errorCounter;

    // Local reference to logger. Used for logging
    private Logger _log;

    /// <summary>
    /// Add predefined items to waitlist.
    /// These items handle error messages, error telemetry and status responses
    /// </summary>
    private void InitWaitlist()
    {
        // TODO: uncomment after creation of delegates
        // _waitlist.Add(new WaitlistItem(deleg_err, isContinuous:true));
        // _waitlist.Add(new WaitlistItem(deleg_err_telemetry, isContinuous:true));
        // _waitlist.Add(new WaitlistItem(deleg_status, isContinuous:true));
    }

    /// <summary>
    /// Generate unique MSG ID (not present in current waitlist) for new message to MCU
    /// </summary>
    /// <returns>msg ID if unoccupied IDs present, 0xFFFF - otherwise</returns>
    public int GenerateUniqueMsgID()
    {
        // Init loacal variables
        int msgID = 0x0;
        int maxValueForID = (int) MsgIDMasks.GeneralPurposeBits;
        int tmpMsgID;

        // Loop through available Ids in waitlist till found minimal not occupied
        for (tmpMsgID = 0; tmpMsgID <= maxValueForID; tmpMsgID++)
        {
            // Exit loop if found unoccupied msg ID
            if (_waitlist.Exists(itm => itm.msgID == tmpMsgID) == false) break;
        }

        // No available IDs - error situation, communication not possible
        if (tmpMsgID > maxValueForID)
        {
            CustomAssertions.Assert(false, "All msgIDs occupied!");
            return 0xFFFF;
        }

        // Construct msgID
        msgID = tmpMsgID & ((int)MsgIDMasks.GeneralPurposeBits);

        return msgID;
    }

    /// <summary>
    /// Add item to waitlist
    /// </summary>
    /// <param name="deleg">delegate that will be called for received msg</param>
    /// <param name="msgID">message ID of send (!) transaction. Used to define which consumer will handle response from MCU</param>
    /// <param name="isContinuous">flag to define if MCU response can be received few times</param>
    /// <returns>true if adding was successful, false - otherwise</returns>
    public bool AddItemToWaitlist(CallbackSerialDelegate deleg, int? msgID, bool isContinuous = false)
    {
        // Check if item with same msgID already exist
        if ((_waitlist.Count != 0) && (msgID != null))
        {
            // Item with same ID already in the list
            if (_waitlist.Exists(itm => itm.msgID == msgID)) return false;
        }

        // Add item to list
        _waitlist.Add(new WaitlistItem(deleg, msgID:msgID, isContinuous:isContinuous));

        return true;
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
            CustomAssertions.Assert(false, "Tried to remove item from empty waitlist or item ID wasn't provided!");
            return false;
        }

        // Check if item with msgID exists
        WaitlistItem? itmToDelete = _waitlist.Find(itm => itm.msgID == msgID);
        if (itmToDelete != null)
        {
            // Deletes item and returns true if successful
            return (_waitlist.Remove(itmToDelete));
        }
        else
        {
            // Item with msgID wasn't found
            CustomAssertions.Assert(false, "Tried to remove unexisting item from waitlist!");
            return false;
        }
    }

    /// <summary>
    /// Looks for delegates that will handle package and removes relevant items from waitlist
    /// </summary>
    /// <param name="receivedPackage">package from MCU</param>
    /// <returns>List with delegates that should be called for this package (max 2)</returns>
    public List<CallbackSerialDelegate> GetDelegate(SerialDataReceive receivedPackage)
    {
        // Return empty array if package not valid
        if (receivedPackage.IsPackageValid != true)
        {
            // TODO
            // Bug with receivedPackage
            // in Debug mode receivedPackage have only zeros in it
            // occurs randomly in GUI runtime
            // CustomAssertions is commented because it close GUI
            //CustomAssertions.Assert(false, $"Invalid package received! Package: {receivedPackage.ToString()}");

            _log.Critical($"Invalid package received! Package: {receivedPackage}");
            return (new List<CallbackSerialDelegate>(0));
        }

        // Create temporary variables
        int msgID = receivedPackage.MsgID & (~(int) MsgIDMasks.ResponseBit); // clear response bit; can't be null!
        MCUStatus status = receivedPackage.Status;
        WaitlistItem?[] tmpWaitlistItems = new WaitlistItem?[2];
        List<CallbackSerialDelegate> outputDelegates = new List<CallbackSerialDelegate>(2);

        // Find items that match msgID or status
        tmpWaitlistItems[0] = _waitlist.Find(itm => itm.msgID == msgID);
        tmpWaitlistItems[1] = _waitlist.Find(itm => (itm.msgID == null) && (itm.status == status));

        // Fill output variable and remove items from waitlist
        foreach (var itm in tmpWaitlistItems)
        {
            if (itm != null)
            {
                // Fill output variable
                outputDelegates.Add(new CallbackSerialDelegate(itm.deleg));

                // Remove item from waitlist, if not continuous
                // All checks for removal are passed at this point
                if (itm.isContinuous == false)
                {
                    _waitlist.Remove(itm);
                }
            }
        }

        // Increment error counter if delegate wasn't found
        errorCounter++;

        return outputDelegates;
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
        /// <param name="status">status from MCU transaction. Used primarly for error, status transactions</param>
        /// <param name="isContinuous">flag to define if MCU response can be received few times</param>
        public WaitlistItem(CallbackSerialDelegate deleg, int? msgID = null, MCUStatus status = MCUStatus._STATUS_MIN, bool isContinuous = false)
        {
            this.deleg = deleg;
            this.msgID = msgID;
            this.status = status;
            this.isContinuous = isContinuous;
        }
        /// <summary>
        /// Message ID of send (!) transaction. Used to define which consumer will handle response from MCU
        /// Reserved value null is used for transactions that are always handled, even without previous transaction from GUI (ack, error, ...)
        /// </summary>
        public int? msgID;

        /// <summary>
        /// Status from MCU transaction. Used to define if transaction should be handled in special manner
        /// </summary>
        public MCUStatus status;

        /// <summary>
        /// Delegate that will be called for received msg
        /// </summary>
        public CallbackSerialDelegate deleg;

        /// <summary>
        /// Flag to define if MCU response can be received few times
        /// </summary>
        public bool isContinuous;
    }
}

// TODO: maybe use singleton pattern?
/// <summary>
/// Class to wrap work with low level Serial port
/// </summary>
public class SerialWrapper : IDisposable
{
    /// <summary>
    /// Length of queue at initialization. Required for runtime optimization
    /// </summary>
    public const int queueDefaultLength = 48;

    /// <summary>
    /// Queue to store packages from MCU. Length of each array should match length of package
    /// </summary>
    private Queue<byte[]> _receivedPackages;

    /// <summary>
    /// List of IDs for routing responses from MCU
    /// </summary>
    private Waitlist _responseWaitlist;

    /// <summary>
    /// Timer to periodically parse input buffer on COM port
    /// </summary>
    private System.Timers.Timer _serialInputDispatchTimer;

    /// <summary>
    /// Queue to store full packages from MCU
    /// </summary>
    private ConcurrentQueue<byte> _inputSerialBuffer;

    /// <summary>
    /// Local reference to class for working with COM port
    /// </summary>
    private SerialComm _serialPort;

    /// <summary>
    /// Local reference to logger
    /// </summary>
    private Logger _log;

    /// <summary>
    /// Lock object for handling multithreading
    /// </summary>
    private readonly object _lock;

    /// <summary>
    /// Create wrapper class for handling serial communication input
    /// </summary>
    /// <param name="disableTimer">flag to disable timer for testing/specific modes</param>
    public SerialWrapper(Logger log, GuiStatusTop guiReference, bool disableTimer = false)
    {
        // Init reference to logger
        _log = log;

        // Initialize lock object
        _lock = new object();

        // Init queue (FIFO) to store packages
        _receivedPackages = new Queue<byte[]>(queueDefaultLength);

        // If timer disable finish init
        if (disableTimer == true) return;

        // Create serial comm object (low level operations with COM port)
        _serialPort = new SerialComm(guiReference);

        // Get reference to serial input buffer
        _inputSerialBuffer = _serialPort.GetReadBufferRef();

        // TODO: timer for unpacking data, should be disabable for testability
        _serialInputDispatchTimer = new System.Timers.Timer();
        _serialInputDispatchTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        _serialInputDispatchTimer.Interval = 50; // time delay is experimentally defined for smooth operation during transfer config
        _serialInputDispatchTimer.Enabled = true;

        // TODO: add link to input serial buffer or init serialComm here

        // Initialize waitlist
        _responseWaitlist = new Waitlist(_log);

        _log.Info("Init Serial Wrapper");
    }

    /// <summary>
    /// Serial wrapper destruction method. Required for testability and correct timer handling
    /// </summary>
    public void Dispose()
    {
        _serialInputDispatchTimer.Dispose();
    }

    /// <summary>
    /// Funcion that is called by timer elapsed event. Used for handling serial communicaton from MCU
    /// </summary>
    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        // Stop timer
        _serialInputDispatchTimer.Stop();

        // Temporary variables
        List<CallbackSerialDelegate> msgCallbacks;
        byte[] package;
        bool isPackageValid = false;

        // TODO: use lock (avoid deadlock)? on data from MCU readout data

        // Check SerialComm for full messages, store all of them to queue
        SelectFullPackages(_inputSerialBuffer);

        // Call corresponding event handlers (delegates) for messages in queue
        for (int idx = 0; idx < _receivedPackages.Count; idx++)
        {
            SerialDataReceive tmpReceivedPackage = new SerialDataReceive();
            package = _receivedPackages.Dequeue();
            isPackageValid = tmpReceivedPackage.UnpackPackage(package, package.Length);

            // If package is not valid - skip
            if (isPackageValid == false)
            {
                _log.Info("Received invalid message");
                continue;
            }

            // Get delegates for
            msgCallbacks = _responseWaitlist.GetDelegate(tmpReceivedPackage);

            if (msgCallbacks.Count == 0)
            {
                _log.Info("Received msg without delegate");
            }

            // Call delegates functions
            for (int i = 0; i < msgCallbacks.Count; i++) msgCallbacks[i](tmpReceivedPackage);

            // Clear delegates variable
            msgCallbacks.Clear();
        }

        // Restart timer
        _serialInputDispatchTimer.Start();
    }

    /// <summary>
    /// Selects packages from input buffer of serial communication and stores them into queue
    /// </summary>
    /// <param name="inputSerialBuffer">buffer with input bytes from serial port</param>
    private void SelectFullPackages(ConcurrentQueue<byte> inputSerialBuffer)
    {
        // Loacal variables
        byte tmpBufferItm = 0x00;
        int lastBytePosition;
        int packageLength;
        // Process data till no packages left
        while (inputSerialBuffer.Count >= (SerialPackageConstants.MinPackageLength))
        {
            // Go through buffer to find beginning of package
            int itmsInBuffer = inputSerialBuffer.Count;
            for (int idx = 0; idx < itmsInBuffer; idx++)
            {
                // Peek item
                inputSerialBuffer.TryPeek(out tmpBufferItm);

                // Check that first byte is expected
                if (tmpBufferItm == 0xAB)
                {
                    // If package start found - exit loop
                    break;
                }
                else
                {
                    // Remove item if not expected
                    inputSerialBuffer.TryDequeue(out tmpBufferItm);
                }
            }

            // If start byte is not found or input buffer size is too small - skip iteration
            if ((tmpBufferItm != 0xAB) || (inputSerialBuffer.Count < 3)) continue;

            // Check that last byte is expected
            lastBytePosition = SerialPackageConstants.PayloadPosition + inputSerialBuffer.ElementAt(3) + SerialPackageConstants.CRCLength;

            // Check if all package received and if last byte is correct
            if (inputSerialBuffer.Count > lastBytePosition)
            {
                if (inputSerialBuffer.ElementAt(lastBytePosition) != 0xBA)
                {
                    // package incorrect - dismiss start byte - will initiate looking for start of next package
                    inputSerialBuffer.TryDequeue(out tmpBufferItm);
                    // skip to next iteration
                    continue;
                }
            }
            else
            {
                // skip to next iteration
                continue;
            }

            // Start and end of package are expected - extract package
            packageLength = lastBytePosition + SerialPackageConstants.EndByteLength;
            byte[] tmpPackage = new byte[packageLength];
            for (int idx = 0; idx < packageLength; idx++)
            {
                inputSerialBuffer.TryDequeue(out tmpPackage[idx]);
            }

            // Save package to queue
            _receivedPackages.Enqueue(tmpPackage);
        }
    }

    /// <summary>
    /// Looks for delegates that will handle package and removes relevant items from waitlist
    /// </summary>
    /// <param name="packageToSend">package that will be send via serial port</param>
    /// <param name="deleg">delegate that will be called after receiving response from MCU</param>
    /// <param name="isContinuous">flag to define if MCU response can be received few time</param>
    /// <returns>true if all operations were performed succesfully, false - otherwise (message wasn't send)</returns>
    public bool SerialWrite(SerialDataSend packageToSend, CallbackSerialDelegate? deleg = null, bool isContinuous = false)
    {
        // Stop processing if package is not valid
        if (packageToSend.IsPackageValid == false)
        {
            CustomAssertions.Assert(false, "Tried to send invalid package!");
            return false;
        }

        // Critical section - block multithread access
        lock(_lock)
        {
            // Add unique message ID to package
            packageToSend.MsgID = _responseWaitlist.GenerateUniqueMsgID();

            // Create item in waitlist waiting for response if delegate is present
            if (deleg != null)
            {
                bool addingStatus;
                addingStatus = _responseWaitlist.AddItemToWaitlist(deleg, packageToSend.MsgID, isContinuous);
                // If adding item to list wasn't successfull, stop processing and return
                if (addingStatus == false)
                {
                    CustomAssertions.Assert(false, "Adding item to waitlist failed!");
                    return false;
                }
            }

            // Generate package and call serial write function
            (int length, byte[] byteArray) = packageToSend.GetPackage();
            if (length > 0)
            {
                // Call low level write function
                _serialPort.Write(byteArray, length); // TODO: invoke on thread
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Reconnect to USB COM port
    /// </summary>
    /// <returns>true if connected, false if failed</returns>
    public bool ReconnectCOMPort()
    {
        return _serialPort.ConnectCOMPort();
    }

    /// <summary>
    /// Checks if MCU COM port present in COM pors list
    /// </summary>
    /// <returns>true if MCU COM port present, false - otherwise</returns>
    public bool IsCOMPortPresent()
    {
        return (_serialPort.GetCOMPorts().Count != 0);
    }

    /// <summary>
    /// Gets list of COM ports with Shieldbuddy connected to them
    /// </summary>
    /// <returns>Port numbers list in format COMX (X number); empty list if port wasn't found</returns>
    public List<string> GetListOfCOMPorts()
    {
        return _serialPort.GetCOMPorts();
    }

    /// <summary>
    /// Set new value of COM port manual selction
    /// </summary>
    /// <returns>Nothing</returns>
    public void SetCOMPortManually(string comPortName)
    {
        _serialPort.manualCOMPort = comPortName;
    }

    /// Removes item from waitlist by msg_id
    /// </summary>
    /// <param name="msgID">message ID of command that should be removed</param>
    /// <returns>true if removal was successful, false - otherwise</returns>
    public bool RemoveWaitlistItem(int? msgID)
    {
        // Removal was called without msgID - valid operation scenario
        if (msgID == null) return true;

        bool isRemovalSuccessful = _responseWaitlist.RemoveItemFromWaitlist(msgID);
        return isRemovalSuccessful;
    }
}