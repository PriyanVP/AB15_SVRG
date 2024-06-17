using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models;

/// <summary>
/// Class that represents transmit data for serial port
/// </summary>
public class TransmitCommunicationPackage<T> : ITransmitCommunicationPackage where T : IByteListSerializable 
{
    private bool _packageValid = false;
    private int _msgID;
    private int _ASICID;
    private T _payload;
    private MCUCommand _cmd = MCUCommand._CMD_MIN;

    /// <summary>
    /// Message ID
    /// </summary>
    public int MsgID
    {
        get { return _msgID; }
        set { _msgID = value; }
    }

    /// <summary>
    /// ASIC ID
    /// </summary>
    public int ASICID
    {
        get { return _ASICID; }
        set { _ASICID = value; }
    }

    /// <summary>
    /// Command to send in package
    /// </summary>
    public MCUCommand Cmd
    {
        get { return _cmd; }
        set { _cmd = value; }
    }

    /// <summary>
    /// Package payload
    /// </summary>
    public T Payload
    {
        get { return _payload; }
        set { _payload = value; }
    }

    /// <summary>
    /// Package payload type
    /// </summary>
    public Type PayloadType
    {
        get { return typeof(T); }
    }

    /// <summary>
    /// Flag to check if package has valid value
    /// </summary>
    public bool IsPackageValid
    {
        get
        {
            // Return flag that package is valid only if required fields are set correctly
            _packageValid = (_cmd > MCUCommand._CMD_MIN) && (_cmd < MCUCommand._EXT_CMD_MAX)
                            && (_ASICID >= 0) && (_ASICID < 256);
            return _packageValid;
        }
    }

    /// <summary>
    /// Init serial transmit package class
    /// </summary>
    public TransmitCommunicationPackage()
    {
    }

    /// <summary>
    /// Format data to send in byte list suitable for use in communication
    /// </summary>
    /// <returns>List with bytes to be send to MCU</returns>
    public List<byte> GetPackage()
    {
        List<byte> packageBytes = new List<byte>();

        // Return empty list if no data is stored in class instance
        if (IsPackageValid == false) return packageBytes;

        // Fill first elements
        packageBytes.Add(0xAB);                             // Start byte
        packageBytes.Add((byte)(_msgID & 0x00FF));          // Message ID
        packageBytes.Add((byte)(_ASICID & 0x00FF));         // ASIC ID
        packageBytes.Add((byte)((int)_cmd & 0x00FF));       // Command position

        // Fill payload
        List<byte> payload = _payload.Serialize();
        packageBytes.Add((byte)(payload.Count & 0x00FF));   // Payload length
        packageBytes.AddRange(payload);                     // Payload

        // Fill CRC
        // Start from 1 to skip strat byte
        // -1 accounts on start byte which is not used for CRC
        byte crcVal = packageBytes.GetCRC8(1, (packageBytes.Count - 1)); 
        packageBytes.Add(crcVal);                           // CRC

        // Add end byte
        packageBytes.Add(0xBA);                             // End byte

        return packageBytes;
    }
}