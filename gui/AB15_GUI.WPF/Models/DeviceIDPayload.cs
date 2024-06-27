using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class that used for commands without payload
    /// </summary>
    public class DeviceIDPayload : IByteListSerializable
    {
        /// <summary>
        /// Property to report errors
        /// </summary>
        public string? Error { get; private set; } = null;

        /// <summary>
        /// Property to report device ID
        /// Null corresponds to no device ID. Error property should be set in such case
        /// </summary>
        public string? DeviceID { get; private set; } = null;

        /// <summary>
        /// Convert byte list to field values
        /// </summary>
        /// <param name="status">status field of package</param>
        /// <param name="rawData">Data that should be converted to parameters values (payload only)</param>
        public void Deserialize(MCUStatus status, List<byte> rawData)
        {
            int deviceID;

            // Apply different handling based on status
            switch (status)
            {
                case MCUStatus.ERROR:
                    // Error handling -> store data to property
                    Error = $"Message with error status received: Payload {string.Join(" ", rawData)}";
                    break;
                case MCUStatus.RESPONSE_ABSENT:
                    // Error handling -> store data to property
                    Error = $"Message with response from MCU wasn't received in expected timeframe.";
                    break;
                case MCUStatus.DATA:
                    // Device ID was received
                    // Check if data in payload present
                    if (rawData.Count <= 0)
                    {
                        Error = $"Received package without data in payload";
                        break;
                    }

                    // Based on Device ID code define device ID string
                    deviceID = rawData[0];
                    DeviceID = (deviceID == 0xC4) ? ("CG904") :
                               (deviceID == 0xC3) ? ("CG903") :
                               (deviceID == 0xC2) ? ("CG902") : 
                                                    (null);
                    break;
                default:
                    throw new ArgumentException($"Unexpected status received: {status}");
            }
        }

        /// <summary>
        /// Converts payload data to byte list
        /// </summary>
        /// <returns>Empty list</returns>
        public List<byte> Serialize()
        {
            return new List<byte>();
        }
    }
}
