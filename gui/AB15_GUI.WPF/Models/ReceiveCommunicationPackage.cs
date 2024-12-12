using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models;

/// <summary>
/// Class that represents receive data for serial port
/// </summary>
public class ReceiveCommunicationPackage<T> : IReceiveCommunicationPackage where T : IByteListSerializable 
{
    private bool _isCRCCorrect = false;

    /// <summary>
    /// Message ID
    /// </summary>
    public int MsgID { get; set; }

    /// <summary>
    /// ASIC ID
    /// </summary>
    public int ASICID { get; set; }

    /// <summary>
    /// Status received in package
    /// </summary>
    public MCUStatus Status { get; set; } = MCUStatus._STATUS_MIN;

    /// <summary>
    /// Package payload
    /// </summary>
    public T Payload { get; set; }

    /// <summary>
    /// Flag to check if package has valid value
    /// </summary>
    public bool IsPackageValid
    {
        get
        {
            // Return flag that package is valid only if required fields are set correctly
            return (_isCRCCorrect) && (Status > MCUStatus._STATUS_MIN) && (Status < MCUStatus._STATUS_MAX);
        }
    }

    /// <summary>
    /// Init serial receive package class
    /// </summary>
    public ReceiveCommunicationPackage()
    {
        Payload = Activator.CreateInstance<T>();
    }

    /// <summary>
    /// Init serial receive package class
    /// </summary>
    /// <param name="payload">payload instance</param>
    public ReceiveCommunicationPackage(T payload)
    {
        Payload = payload;
    }

    /// <summary>
    /// Unpack data from received byte array to format suitable for further use in application
    /// <param name="receivedPackage">array with one full package received by serial communication. Specific case: 1 item in list - response absent scenario</param>
    /// <returns>Flag signalizing if unpacked data is valid</returns>
    public bool UnpackPackage(List<byte> receivedPackage)
    {
        // Handle package with response absent status
        if (receivedPackage.Count == 1)
        {
            Status = (MCUStatus)receivedPackage[0];
            Payload.Deserialize(Status, new List<byte>());
            return true;
        }

        // Return false if input parameters are incorrect
        if (receivedPackage.Count < SerialPackageConstants.MinPackageLength) return false;

        // Check if CRC is correct
        int crcDataLength = receivedPackage.Count -  SerialPackageConstants.StartByteLength - SerialPackageConstants.CRCLength - SerialPackageConstants.EndByteLength ; // substract number of bytes unused in CRC calculation
        _isCRCCorrect = receivedPackage[receivedPackage.Count - Math.Abs(SerialPackageConstants.CRCPosition)] == receivedPackage.GetCRC8(SerialPackageConstants.MsgIDPosition, crcDataLength);

        // Exit processing if CRC is not correct
        if (_isCRCCorrect == false) return false;

        // Get value of payload length in bytes
        int payloadLength = (int)receivedPackage[SerialPackageConstants.PayloadLengthPosition];

        // Unpack data
        MsgID = (int)receivedPackage[SerialPackageConstants.MsgIDPosition];                                         // Message ID
        ASICID = (int)receivedPackage[SerialPackageConstants.ASICIDPosition];                                       // ASIC ID -> TODO: outdated naming
        Status = (MCUStatus)receivedPackage[SerialPackageConstants.CmdStatusPosition];                              // Status

        // If package is not valid - revert Properties to their default state and exit
        if (IsPackageValid == false)
        {
            MsgID = default(int);
            ASICID = default(int);
            Status = MCUStatus._STATUS_MIN;
            return false;
        }

        Payload.Deserialize(Status, receivedPackage.Slice(SerialPackageConstants.PayloadPosition, payloadLength));  // Payload

        // Return flag signalizing if data was unpacked without errors
        return IsPackageValid;
    }
}