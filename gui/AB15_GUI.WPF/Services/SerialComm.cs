using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Management;

namespace ModernUI.Back;

/// <summary>
/// Low-level class to work with COM port
/// </summary>
public class SerialComm
{
    /// <summary>
    /// Serial communication baud rate of ShieldBuddy
    /// </summary>
    private const int BAUD_RATE = 115200;

    /// <summary>
    /// Create Attribute for virtualCOM instance
    /// </summary>
    private SerialPort _port;

    /// <summary>
    /// Local reference to gui backend instance
    /// </summary>
    private GuiStatusTop _guiReferenceVar;

    /// <summary>
    /// Create tread-safe input buffer
    /// </summary>
    public ConcurrentQueue<byte> inputBuffer;

    /// <summary>
    /// Attribute used for overwriting automatic port detection
    /// </summary>
    public string manualCOMPort;

    /// <summary>
    /// Initialization of virtual COM port
    /// </summary>
    /// <param name="guiReference">Ensures operation of StatusPanel updates</param>
    public SerialComm(GuiStatusTop guiReference)
    {
        // TODO: add error checking on opening (attemp to open opened port results in error)
        //
        _guiReferenceVar = guiReference;

        // Init COM port instance selected manually
        manualCOMPort = "auto";

        // Thread-safe FIFO collection
        inputBuffer = new ConcurrentQueue<byte>();

        // Init port
        _port = new SerialPort();

        // Set default configs for port
        _port.BaudRate = BAUD_RATE;
        _port.DataBits = 8;
        _port.Parity = Parity.None;
        _port.StopBits = StopBits.One;

        // Event handler for DataReceived assignment
        _port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

        // Connect COM port
        ConnectCOMPort();
    }

    /// <summary>
    /// Connect/reconnect to USB COM port
    /// </summary>
    /// <returns>true if connected, false if failed</returns>
    public bool ConnectCOMPort()
    {
        Global.log.Info($"Try MCU connection...");

        // Attempt automatic port detection
        string? detectedPortNumber = (manualCOMPort == "auto") ? (GetCOMPorts().LastOrDefault()) : (manualCOMPort);

        // Check if required port present
        if (detectedPortNumber == null)
        {
            // Automatic connection was unsuccesfull, report in StatusPanel and log window
            _guiReferenceVar.SetStatus("USB", StatusTypes.Disconnected);
            Global.log.Info($"Automatic port detect do not found MCU");
            return false;
        }
        else
        {
            // Detection attempt was successfully
            _port.Close();
            _port.PortName = detectedPortNumber;
            Global.log.Debug($"Found needed MCU port at - {detectedPortNumber}");
        }

        // Try to open port
        try
        {
            _port.Open();
            _guiReferenceVar.SetStatus("USB", StatusTypes.Connected);
            Global.log.Info("Connected to MCU at " + detectedPortNumber);
            return true;
        }
        catch (Exception serEx)
        {
            Global.log.Debug(serEx.Message);
            _guiReferenceVar.SetStatus("USB", StatusTypes.Disconnected);
            Global.log.Info($"Connection to MCU failed");
            return false;
        }
    }

    /// <summary>
    /// Automatically detect all COM ports with ShieldBuddy's connected to them
    /// </summary>
    /// <returns>Port numbers list in format COMX (X number); empty list if port wasn't found</returns>
    public List<string> GetCOMPorts()
    {
        List<string> availableCOMPorts = new List<string>(4);

        // Look for COM port connected device with specific description in Plug&Play devices list
        ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%Infineon DAS JDS COM  (COM%'");

        // Extract COM port number from device's description
        string tmpCOMName;
        foreach (ManagementObject currentObject in managementObjectSearcher.Get())
        {
            tmpCOMName = (currentObject["Name"].ToString()).Split('(', ')')[1];
            availableCOMPorts.Add(tmpCOMName);
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
            _guiReferenceVar.SetStatus("USB", StatusTypes.Disconnected);
            return;
        }

        // Read bytes from COM port input buffer (will also remove them)
        int bytes = _port.BytesToRead;
        byte[] buffer = new byte[bytes];
        _port.Read(buffer, 0, bytes);

        // Enqueue each byte in internal FIFO buffer
        foreach (byte b in buffer)
        {
            inputBuffer.Enqueue(b);
        }
    }

    // TODO: remove or redo -> currently error prone
    /// <summary>
    /// Read specified number of bytes from virtualCOM port's input buffer
    /// If number of requested bytes is not present in queue - will return all that are present
    /// </summary>
    /// <param name="numberOfBytes">number of bytes to acquire from virtualCOM port's input buffer</param>
    public byte[] Read(int numberOfBytes)
    {
        // Create byte array with len == required number of bytes to read from input buffer (will be function's return value)
        byte[] readBuffer = new byte[numberOfBytes];

        // Copy required number of bytes into byte array to-be-returned
        for (int i = 0; i < readBuffer.Length; i++)
        {
            byte outBufferElement = 0;
            inputBuffer.TryDequeue(out outBufferElement);
            readBuffer[i] = outBufferElement;
        }

        return readBuffer;
    }

    /// <summary>
    /// Send array of bytes via virtualCOM port (must be already open)
    /// </summary>
    /// <param name="sendData">array with bytes to send to MCU</param>
    /// <param name="length">length of valid bytes in array</param>
    public void Write(byte[] sendData, int length)
    {
        // Stop execution if port is closed TODO: add error reporting
        if (_port.IsOpen == false)
        {
            _port.Close(); // TODO: check if works without port name
            _guiReferenceVar.SetStatus("USB", StatusTypes.Disconnected);
            return;
        }

        int offset = 0;
        int dataChunk = 16; // Number 16 is defined by MCU HW (ASCLIN HW buffer size)

        while (length / dataChunk > 0) // incorrect handling
        {
            // Write data by chunks
            _port.Write(sendData, offset, dataChunk);

            // Handle offset and length variables
            offset += dataChunk;
            length -= dataChunk;

            // Pause is required for MCU to empty HW buffer
            Thread.Sleep(1);
        }

        // Send remaining data if present
        if (length > 0) _port.Write(sendData, offset, length); // TODO: use separate thread

        // Pause is required for MCU to empty HW buffer
        Thread.Sleep(1);
    }

    /// <summary>
    /// Returns reference to internal input buffer
    /// </summary>
    /// <returns>Reference to internal input buffer</returns>
    public ConcurrentQueue<byte> GetReadBufferRef()
    {
        return inputBuffer;
    }
}