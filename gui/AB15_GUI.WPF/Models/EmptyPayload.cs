using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using System.ComponentModel;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class that used for commands without payload
    /// </summary>
    public class EmptyPayload : IByteListSerializable
    {
        /// <summary>
        /// Property to report errors
        /// </summary>
        public string? Error { get; set; } = null;

        /// <summary>
        /// Convert byte list to field values
        /// </summary>
        /// <param name="status">status field of package</param>
        /// <param name="rawData">Data that should be converted to parameters values</param>
        public void Deserialize(MCUStatus status, List<byte> rawData)
        {
            byte asicID;
            List<byte> payload;

            // Apply different handling based on status
            switch (status)
            {
                case MCUStatus.ERROR:
                    // Error handling -> store data to property
                    asicID = rawData.ElementAt(SerialPackageConstants.ASICIDPosition);
                    payload = rawData.Slice(SerialPackageConstants.PayloadPosition, rawData.ElementAt(SerialPackageConstants.PayloadLengthPosition));
                    Error = $"Message with error status received: ASIC ID {asicID} Payload {string.Join(" ", payload)}";
                    break;
                case MCUStatus.RESPONCE_ABSENT:
                    // Error handling -> store data to property
                    Error = $"Message with responce from MCU wasn't received in expected timeframe.";
                    break;
                case MCUStatus.ACK:
                    // empty
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
