using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class that used for ReadRegister command
    /// </summary>
    public class ReadRegisterPayload : IByteListSerializable
    {
        /// <summary>
        /// Property to report errors
        /// </summary>
        public string? Error { get; set; } = null;

        /// <summary>
        /// Property to store register data
        /// </summary>
        public UInt16 RegisterData { get; set; }

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
                    // Check if data in payload present and size is expected
                    if (rawData.Count <= 0)
                    {
                        Error = $"Received package with incorrect data in payload. Expected 2 bytes, but got {rawData.Count}";
                        break;
                    }

                    // Reconstruct register content from bytes in payload
                    RegisterData = BitOperationsExtensions.ConstructWordFromBytes(rawData[1], rawData[0]);

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
