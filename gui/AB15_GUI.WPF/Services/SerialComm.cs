using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using System.IO.Ports;
using System.Management;
using AB15_GUI.WPF.Services.Interfaces;
using AB15_GUI.WPF.NLog;

namespace AB15_GUI.WPF.Services;

/// <summary>
/// Low-level class to work with COM port
/// </summary>
public class SerialComm : ISerialComm
{
    /// <summary>
    /// Serial communication baud rate (should match to baud rate of MCU target)
    /// </summary>
    private const int BAUD_RATE = 115200;

    /// <summary>
    /// Logger reference with custom configuration
    /// </summary>
    private readonly ILoggingService logger;

    /// <summary>
    /// COM port handle
    /// </summary>
    private SerialPort _port;

    /// <summary>
    /// Tread-safe input buffer (data from MCU)
    /// </summary>
    public ConcurrentQueue<byte> ReceiveBuffer { get; set; } = new ConcurrentQueue<byte>();

    /// <summary>
    /// Property used for overwriting automatic port detection
    /// Default value corresponds to automatic COM port selection
    /// </summary>
    public string? ManualCOMPortName { get; set; } = null;

    /// <summary>
    /// Lock object to avoid mixing messages
    /// </summary>
    private readonly object _lock = new object();

    /// <summary>
    /// Initialization of virtual COM port
    /// </summary>
    /// <param name="guiReference">Ensures operation of StatusPanel updates</param>
    public SerialComm(ILoggingService logger)
    {
        // Init logger reference
        this.logger = logger;

        // Init and configure port
        _port = new SerialPort()
        {
            BaudRate = BAUD_RATE,
            DataBits = 8,
            Parity = Parity.None,
            StopBits = StopBits.One,
        };

        // Event handler for DataReceived assignment
        _port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

        // Connect COM port
        ConnectCOMPort();

        logger.Trace("Finished SerialComm initialization");
    }

    /// <summary>
    /// Connect/reconnect to USB COM port
    /// </summary>
    /// <returns>true if connected, false if failed</returns>
    public bool ConnectCOMPort()
    {
        logger.Trace($"Trying to connect MCU...");

        // Attempt automatic port detection
        string? detectedPortNumber = (ManualCOMPortName is null) ? (GetCOMPorts().LastOrDefault()) : (ManualCOMPortName);

        // Check if required port present
        if (detectedPortNumber is null)
        {
            // Automatic connection was unsuccesfull, report in StatusPanel and log window
            logger.Warn($"Automatic port detect hasn't found MCU");
            return false;
        }

        // Seting up port for opening
        _port.Close();
        _port.PortName = detectedPortNumber;
        logger.Debug($"Found needed MCU port at {detectedPortNumber}");

        // Try to open port
        try
        {
            _port.Open();
            logger.Debug("Connected to MCU at " + detectedPortNumber);
            return true;
        }
        catch (Exception serEx)
        {
            logger.Warn(serEx, "Couldn't open port! Check if port is not busy!");
            logger.Warn($"Connection to MCU failed");
            return false;
        }
    }

    /// <summary>
    /// Automatically detect all COM ports with ShieldBuddy's connected to them
    /// </summary>
    /// <returns>Port numbers list in format COMX (X number); empty list if port wasn't found</returns>
    public List<string> GetCOMPorts()
    {
        // List of currently available com ports where ShieldBuddy TC375 are connected
        List<string> availableCOMPorts = new List<string>();

        // Look for COM port connected device with specific description in Plug&Play devices list
        ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%Infineon DAS JDS COM  (COM%'");

        // Extract COM port short name with number from device's description
        foreach (ManagementObject currentObject in managementObjectSearcher.Get())
        {
            try
            {
                availableCOMPorts.Add(currentObject["Name"].ToString().Split('(', ')')[1]);
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Unexpected data from COM ports list.");
            }

        }
        return availableCOMPorts;
    }

    /// <summary>
    /// Event handler for data received from MCU
    /// </summary>
    /// <param name="sender">object that called this method</param>
    /// <param name="e">event arguments</param>
    private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        // Stop execution if port is closed
        if (_port.IsOpen == false)
        {
            _port.Close(); // TODO: check if works without port name
            return;
        }

        // Read bytes from COM port input buffer (will also remove them)
        int bytes = _port.BytesToRead;
        byte[] buffer = new byte[bytes];
        _port.Read(buffer, 0, bytes);

        // Enqueue each byte in internal FIFO buffer
        foreach (byte b in buffer)
        {
            ReceiveBuffer.Enqueue(b);
        }
    }

    /// <summary>
    /// Send array of bytes via virtualCOM port (must be already open)
    /// </summary>
    /// <param name="dataToSend">array with bytes to send to MCU</param>
    /// <param name="length">length of valid bytes in array</param>
    public void Write(byte[] dataToSend, int length)
    {
        // Sanity checks 
        // TODO: check why Contract usage breaks application if not in debug mode
        // Contract.Requires<ArgumentOutOfRangeException>(length <= 0, "Length of buffer must be positive integer");

        // Stop execution if port is closed TODO: add error reporting
        if (_port.IsOpen == false)
        {
            logger.Warn($"Attempt to write package when port in closed!\nPackage: {string.Join(" ", dataToSend)}");
            _port.Close(); // TODO: check if works without port name
            return;
        }

        // Send data to MCU with chuncks that can be handled
        int offset = 0;
        int dataChunk = 16; // Number 16 is defined by MCU HW (ASCLIN HW buffer size)

        lock (_lock)
        {
            while (length / dataChunk > 0)
            {
                // Write data by chunks
                _port.Write(dataToSend, offset, dataChunk);
    
                // Handle offset and length variables
                offset += dataChunk;
                length -= dataChunk;
    
                // Pause is required for MCU to empty HW buffer
                Thread.Sleep(1);
            }
    
            // Send remaining data if present
            if (length > 0) _port.Write(dataToSend, offset, length);
    
            // Pause is required for MCU to empty HW buffer
            Thread.Sleep(1);
        }
    }
}