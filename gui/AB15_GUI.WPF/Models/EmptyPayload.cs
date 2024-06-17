using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class that used for commands without payload
    /// </summary>
    public class EmptyPayload : IByteListSerializable
    {
        /// <summary>
        /// Logger reference with custom configuration
        /// </summary>
        private readonly Logger logger;

        public EmptyPayload(Logger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Convert byte list to field values
        /// </summary>
        /// <param name="status">status field of package</param>
        /// <param name="rawData">Data that should be converted to parameters values</param>
        public void Deserialize(MCUStatus status, List<byte> rawData)
        {
            // Apply different handling based on status
            switch (status)
            {
                case MCUStatus.ERROR:
                    // Error handling -> report via logger
                    byte asicID = rawData.ElementAt(SerialPackageConstants.ASICIDPosition);
                    List<byte> payload = rawData.Slice(SerialPackageConstants.PayloadPosition, rawData.ElementAt(SerialPackageConstants.PayloadLengthPosition));
                    logger.Warn($"Message with error status received: ASIC ID {asicID} Payload {string.Join(" ", payload)}");
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
