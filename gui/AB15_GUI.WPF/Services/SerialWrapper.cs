using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Services.Interfaces;
using System.Threading.Tasks;

namespace AB15_GUI.WPF.Services;

/// <summary>
/// Class to wrap work with low level Serial port
/// </summary>
public class SerialWrapper : IDisposable, ISerialWrapper
{
    /// <summary>
    /// Length of queue at initialization. Required for runtime optimization
    /// </summary>
    private const int queueDefaultLength = 48;

    /// <summary>
    /// Queue to store packages from MCU. Length of each array should match length of package
    /// </summary>
    private Queue<List<byte>> _receivedPackages = new Queue<List<byte>>(queueDefaultLength);

    /// <summary>
    /// List of IDs for routing responses from MCU
    /// </summary>
    private IWaitlist _responseWaitlist;

    /// <summary>
    /// Timer to periodically parse input buffer on COM port
    /// </summary>
    private Timer _serialInputDispatchTimer;

    /// <summary>
    /// Timer to periodically parse waitlist to remove outdated messages
    /// </summary>
    private Timer _waitlistTimeOfLifeTimer;

    /// <summary>
    /// Queue to store full packages from MCU
    /// </summary>
    private ConcurrentQueue<byte> _inputSerialBuffer;

    /// <summary>
    /// Local reference to class for working with COM port
    /// </summary>
    private ISerialComm _serialPort;

    /// <summary>
    /// Local reference to logger
    /// </summary>
    private ILoggingService logger;

    /// <summary>
    /// Lock object for handling multithreading
    /// </summary>
    private readonly object _lock = new object();

    /// <summary>
    /// List of COM ports with Shieldbuddy connected to them
    /// </summary>
    public List<string> AvailableCOMPorts => _serialPort.GetCOMPorts();

    /// <summary>
    /// Checks if MCU COM port present in COM ports list
    /// </summary>
    public bool IsCOMPortPresent => AvailableCOMPorts.Any();

    /// <summary>
    /// COM port name when selected manually
    /// Default value null corresponds to automatic port detection
    /// </summary>
    public string? ManualComPortName
    {
        get
        {
            return _serialPort.ManualCOMPortName;
        }
        set
        {
            _serialPort.ManualCOMPortName = value;
        }
    }

    /// <summary>
    /// Create wrapper class for handling serial communication input
    /// </summary>
    /// <param name="logger">logger reference</param>
    /// <param name="serialPort">reference to low level object for managing</param>
    /// <param name="responseWaitlist">waitlist that stores data for handling received packages</param>
    public SerialWrapper(ILoggingService logger, ISerialComm serialPort, IWaitlist responseWaitlist)
    {
        // Init reference to logger
        this.logger = logger;

        // Create serial comm object (low level operations with COM port)
        _serialPort = serialPort;

        // Get reference to serial input buffer
        _inputSerialBuffer = _serialPort.ReceiveBuffer;

        // Configure timer for packages handling
        _serialInputDispatchTimer = new Timer();
        _serialInputDispatchTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        _serialInputDispatchTimer.Interval = 50; // time delay is experimentally defined for smooth operation
        _serialInputDispatchTimer.Enabled = true;

        // Configure timer for outdated messages
        _waitlistTimeOfLifeTimer = new Timer();
        _waitlistTimeOfLifeTimer.Elapsed += new ElapsedEventHandler(OnWaitlistTimedEvent);
        _waitlistTimeOfLifeTimer.Interval = 5000; // time delay is experimentally defined for smooth operation
        _waitlistTimeOfLifeTimer.Enabled = true;

        // Initialize waitlist
        _responseWaitlist = responseWaitlist;

        logger.Info("Initialized Serial Wrapper");
    }

    /// <summary>
    /// Serial wrapper destruction method. Required for testability and correct timer handling
    /// </summary>
    public void Dispose()
    {
        _serialInputDispatchTimer.Elapsed -= new ElapsedEventHandler(OnTimedEvent);
        _waitlistTimeOfLifeTimer.Elapsed -= new ElapsedEventHandler(OnWaitlistTimedEvent);
        _serialInputDispatchTimer.Dispose();
        _waitlistTimeOfLifeTimer.Dispose();
    }

    /// <summary>
    /// Check if any of the waitlist items are outdated
    /// </summary>
    /// <param name="source">caller</param>
    /// <param name="e">event arguments</param>
    private void OnWaitlistTimedEvent(object? sender, ElapsedEventArgs e)
    {
        // Stop timer
        _waitlistTimeOfLifeTimer.Stop();

        // Run removal of outdated packages
        _responseWaitlist.RemoveOutdatedItems();

        // Start timer
        _waitlistTimeOfLifeTimer.Start();
    }

    /// <summary>
    /// Function that is called by timer elapsed event. Used for handling serial communicaton from MCU
    /// </summary>
    /// <param name="source">caller</param>
    /// <param name="e">event arguments</param>
    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        // Stop timer
        _serialInputDispatchTimer.Stop();

        // Temporary variables
        List<byte> package;

        // Check SerialComm for full messages, store all of them to queue
        SelectFullPackages();

        // Call corresponding event handlers (delegates) for messages in queue
        for (int idx = 0; idx < _receivedPackages.Count; idx++)
        {
            package = _receivedPackages.Dequeue();

            // Here task will be completed and data will be execution of command will continue
            _responseWaitlist.HandleResponse(package);
        }

        // Restart timer
        _serialInputDispatchTimer.Start();
    }

    /// <summary>
    /// Selects packages from input buffer of serial communication and stores them into queue
    /// </summary>
    private void SelectFullPackages()
    {
        // Local variables
        byte tmpBufferItm = 0x00;
        int lastBytePosition;
        int packageLength;

        // Process data till no packages left
        while (_inputSerialBuffer.Count >= SerialPackageConstants.MinPackageLength)
        {
            // Go through buffer to find beginning of package
            int itmsInBuffer = _inputSerialBuffer.Count;
            for (int idx = 0; idx < itmsInBuffer; idx++)
            {
                // Peek item
                _inputSerialBuffer.TryPeek(out tmpBufferItm);

                // Check that first byte is expected
                if (tmpBufferItm == SerialPackageConstants.StartByteValue)
                {
                    // If package start found - exit loop
                    break;
                }
                else
                {
                    // Remove item if not expected
                    logger.Warn($"Unexpected start byte in received package: {tmpBufferItm}");
                    _inputSerialBuffer.TryDequeue(out tmpBufferItm);
                }
            }

            // If start byte is not found or input buffer size is too small - skip iteration
            if ((tmpBufferItm != 0xAB) || (_inputSerialBuffer.Count < SerialPackageConstants.MinPackageLength)) continue;

            // Check that last byte is expected
            packageLength = SerialPackageConstants.StartByteLength + SerialPackageConstants.MsgIDLength + SerialPackageConstants.ASICIDLength
                            + SerialPackageConstants.CmdStatusLength + SerialPackageConstants.PayloadLengthLength 
                            + _inputSerialBuffer.ElementAt(SerialPackageConstants.PayloadLengthPosition) + SerialPackageConstants.CRCLength
                            + SerialPackageConstants.EndByteLength;
            lastBytePosition = packageLength - 1;

            // Check if all package is received
            if (_inputSerialBuffer.Count < packageLength) continue;

            // Check if last byte is correct
            if (_inputSerialBuffer.ElementAt(lastBytePosition) != SerialPackageConstants.EndByteValue)
            {
                // package incorrect - dismiss start byte - will initiate looking for start of next package
                _inputSerialBuffer.TryDequeue(out tmpBufferItm);
                logger.Warn($"Unexpected end byte in received package: {tmpBufferItm}");

                // skip to next iteration
                continue;
            }

            // Start and end of the package are expected - extract package
            List<byte> package = new List<byte>(packageLength);
            for (int idx = 0; idx < packageLength; idx++)
            {
                _inputSerialBuffer.TryDequeue(out tmpBufferItm);
                package.Add(tmpBufferItm);
            }

            // Command tracer
            LogPackage(false, package);

            // Save package to queue
            _receivedPackages.Enqueue(package);
        }
    }

    /// <summary>
    /// Looks for delegates that will handle package and removes relevant items from waitlist
    /// </summary>
    /// <param name="packageToSend">package that will be send via serial port</param>
    /// <returns>Task which returns response if all operations were performed successfully, null - otherwise (message wasn't send)</returns>
    public Task<IReceiveCommunicationPackage?> SerialWriteAsync(ITransmitCommunicationPackage packageToSend)
    {
        // Stop processing if package is not valid
        if (packageToSend.IsPackageValid == false)
        {
            logger.Warn("Tried to send invalid package!");
            return Task.FromResult<IReceiveCommunicationPackage?>(null);
        }

        Task<IReceiveCommunicationPackage>? responseTask;

        // Critical section - block multithread access
        lock (_lock)
        {
            (packageToSend.MsgID, responseTask) = _responseWaitlist.AddItemToWaitlist(packageToSend.PayloadType, packageToSend.IsContinuous);

            // If adding item to list wasn't successful, stop processing and return
            if ((packageToSend.MsgID is null) || (responseTask is null))
            {
                logger.Error("Adding item to waitlist failed!");
                return Task.FromResult<IReceiveCommunicationPackage?>(null);
            }

            // Generate package and call serial write function
            List<byte> package = packageToSend.GetPackage();

            // Command tracer
            LogPackage(true, package);

            // Call low level write function
            _serialPort.Write(package.ToArray<byte>(), package.Count);
        }

        return responseTask;
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
    /// Get task instance from waitlist by ID
    /// To be used for continuous communication (few answers on one request)
    /// </summary>
    /// <param name="msgID">message ID</param>
    /// <returns></returns>
    public Task<IReceiveCommunicationPackage?> GetContinuousTaskInstance(int msgID)
    {
        return _responseWaitlist.GetContinuousTaskInstance(msgID);
    }

    /// <summary>
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

    /// <summary>
    /// Package logger. Intended usage: search log file by [CT IN] or [CT OUT] keywords
    /// Works only if logger in debug mode. Logs both in and out packages
    /// </summary>
    /// <param name="isTransmitPackage">flag indicating if package is transmitted from PC</param>
    /// <param name="package">full package</param>
    private void LogPackage(bool isTransmitPackage, List<byte> package)
    {
        if (logger.IsDebugEnabled)
        {
            string inOut = isTransmitPackage ? "OUT" : "IN";
            string packageAsString = $"[CT {inOut}] Package:";
            package.ForEach((x) => packageAsString = $"{packageAsString} 0x{x:X2}");
            logger.Debug(packageAsString);
        }
    }
}