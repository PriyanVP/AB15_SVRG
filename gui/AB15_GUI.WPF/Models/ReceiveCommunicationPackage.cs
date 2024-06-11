using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models;

/// <summary>
/// Class that represents receive data for serial port
/// </summary>
public class ReceiveCommunicationPackage<T> : IReceiveCommunicationPackage where T : IByteListSerializable 
{
    private bool _packageValid = false;
    private bool _isCRCCorrect = false;
    private int _msgID;
    private int _ASICID;
    private T _payload;
    private MCUStatus _status = MCUStatus._STATUS_MIN;

    /// <summary>
    /// Message ID
    /// </summary>
    public int MsgID
    {
        get { return _msgID; }
        private set { _msgID = value; }
    }

    /// <summary>
    /// ASIC ID
    /// </summary>
    public int ASICID
    {
        get { return _ASICID; }
        private set { _ASICID = value; }
    }

    /// <summary>
    /// Status received in package
    /// </summary>
    public MCUStatus Status
    {
        get { return _status; }
        private set
        {
            // Set only valid values of statuses
            if ((value > MCUStatus._STATUS_MIN) && (value < MCUStatus._STATUS_MAX))
            {
                _status = value;
            }
        }
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
    /// Flag to check if package has valid value TODO: validation is different for send and receive packages
    /// </summary>
    public bool IsPackageValid
    {
        get
        {
            // Return flag that package is valid only if required fields are set correctly
            _packageValid = (_isCRCCorrect) && (_status != MCUStatus._STATUS_MIN);
            return _packageValid;
        }
    }

    /// <summary>
    /// Init serial receive package class
    /// </summary>
    public ReceiveCommunicationPackage()
    {
    }

    /// <summary>
    /// Unpack data from received byte array to format suitable for further use in application
    /// <param name="receivedPackage">array with one full package received by serial communication</param>
    /// <returns>Flag signalizing if unpacked data is valid</returns>
    public bool UnpackPackage(List<byte> receivedPackage)
    {
        // Return false if input parameters are incorrect
        if (receivedPackage.Count < SerialPackageConstants.MinPackageLength) return false;

        // Check if CRC is correct
        int crcDataLength = receivedPackage.Count - 3; // 3 is number of bytes unused in CRC calculation: start byte, end byte and crc itself
        _isCRCCorrect = receivedPackage[receivedPackage.Count - Math.Abs(SerialPackageConstants.CRCPosition)] == receivedPackage.GetCRC8(SerialPackageConstants.MsgIDPosition, crcDataLength);

        // Exit processing if CRC is not correct
        if (_isCRCCorrect == false) return false;

        // Get value of payload length in bytes
        int payloadLength = (int)receivedPackage[SerialPackageConstants.PayloadLengthPosition];

        // Unpack data
        MsgID = (int)receivedPackage[SerialPackageConstants.MsgIDPosition];                                 // Message ID
        ASICID = (int)receivedPackage[SerialPackageConstants.ASICIDPosition];                               // ASIC ID
        Status = (MCUStatus)receivedPackage[SerialPackageConstants.CmdStatusPosition];                      // Status
        Payload.Deserealize(receivedPackage.Slice(SerialPackageConstants.PayloadPosition, payloadLength));  // Payload

        // Return flag signalizing if data was unpacked without errors
        return IsPackageValid;
    }
}