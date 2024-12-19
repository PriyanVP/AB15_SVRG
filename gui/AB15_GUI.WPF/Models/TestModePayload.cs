using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class that is used for unpacking Test Mode data
    /// </summary>
    public class TestModePayload : IByteListSerializable
    {
        /// <summary>
        /// Property to report errors
        /// </summary>
        public string? Error { get; set; } = null;

        // TODO: implement unpacking - use different storages

        ///// <summary>
        ///// Property to report errors
        ///// </summary>
        //public string? Error { get; set; } = null;
      
        /// <summary>
        /// List with register values
        /// </summary>
        public List<UInt16> Data { get; set; } = new List<ushort>();

        /// <summary>
        /// Convert byte list to field values
        /// </summary>
        /// <param name="status">status field of package</param>
        /// <param name="rawData">Data that should be converted to parameters values (payload only)</param>
        public void Deserialize(MCUStatus status, List<byte> rawData)
        {
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
                    // Data was received
                    // Check if data in payload is expected
                    if (rawData.Count != 20)
                    {
                        Error = $"Received package with unexpected number of bytes in payload. Expected 86, but got {rawData.Count}";
                        break;
                    }

                    // TODO: analysis logic for incoming data

                    // Store payload as registers data
                    for (int i = 0; i < rawData.Count; i++)
                    {
                        Data.Add(rawData[i]);

                    }
 
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
