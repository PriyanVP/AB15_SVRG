using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class that used for ReadRegister command
    /// </summary>
    public class SvrPayload : IByteListSerializable
    {
        /// <summary>
        /// Property to report errors
        /// </summary>
        public string? Error { get; set; } = null;

        /// <summary>
        /// Property to store SVR1 command
        /// </summary>
        public SvrCommandValues Svr1Command { get; set; } = SvrCommandValues.SVR_OFF;
        
        /// <summary>
        /// Property to store SVR1 command
        /// </summary>
        public SvrCommandValues Svr2Command { get; set; } = SvrCommandValues.SVR_OFF;

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
                case MCUStatus.ACK:
                    // Command executed successfully
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
            List<byte> serializedPackage = new List<byte>();
            serializedPackage.Add((byte)Svr1Command);
            serializedPackage.Add((byte)Svr2Command);
            return serializedPackage;
        }
    }
}
