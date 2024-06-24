using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models;

/// <summary>
/// Class that represents transmit data for serial port
/// </summary>
public class TransmitCommunicationPackage<T> : ITransmitCommunicationPackage where T : IByteListSerializable 
{
    /// <summary>
    /// Message ID
    /// </summary>
    public int MsgID { get; set; }

    /// <summary>
    /// ASIC ID
    /// </summary>
    public int ASICID { get; set; }

    /// <summary>
    /// Command to send in package
    /// </summary>
    public MCUCommand Cmd { get; set; } = MCUCommand._CMD_MIN;

    /// <summary>
    /// Flag to indicate if few responses can be received for this package
    /// </summary>
    public bool IsContinuous { get; set; } = false;

    /// <summary>
    /// Package payload
    /// </summary>
    public T Payload { get; set; }

    /// <summary>
    /// Receive package payload type
    /// </summary>
    public Type? PayloadType { get; set; } = null;

    /// <summary>
    /// Delegate that will be called for received msg
    /// </summary>
    public Action<IReceiveCommunicationPackage>? Deleg { get; set; } = null;

    /// <summary>
    /// Flag to check if package has valid value
    /// </summary>
    public bool IsPackageValid
    {
        get
        {
            // Return flag that package is valid only if required fields are set correctly
            return (Cmd > MCUCommand._CMD_MIN) && (Cmd < MCUCommand._EXT_CMD_MAX)
                            && (ASICID >= 0) && (ASICID < 256)
                            && ((PayloadType is not null) || (Deleg is null));
        }
    }

    /// <summary>
    /// Init serial transmit package class
    /// </summary>
    public TransmitCommunicationPackage()
    {
        Payload = Activator.CreateInstance<T>();
    }

    /// <summary>
    /// Init serial transmit package class
    /// </summary>
    /// <param name="payload">payload instance</param>
    public TransmitCommunicationPackage(T payload)
    {
        Payload = payload;
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
        packageBytes.Add((byte)(MsgID & 0x00FF));          // Message ID
        packageBytes.Add((byte)(ASICID & 0x00FF));         // ASIC ID
        packageBytes.Add((byte)((int)Cmd & 0x00FF));       // Command position

        // Fill payload
        List<byte> payload = Payload.Serialize();
        packageBytes.Add((byte)(payload.Count & 0x00FF));   // Payload length
        packageBytes.AddRange(payload);                     // Payload

        // Fill CRC
        byte crcVal = packageBytes.GetCRC8(SerialPackageConstants.MsgIDPosition, (packageBytes.Count - SerialPackageConstants.StartByteLength)); 
        packageBytes.Add(crcVal);                           // CRC

        // Add end byte
        packageBytes.Add(0xBA);                             // End byte

        return packageBytes;
    }
}