using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Generic class that used for commands requiring address and data payload
    /// Note: read/write sequence commands, read/write commands, etc.
    /// </summary>
    public class AddressDataPayload : IByteListSerializable
    {
        /// <summary>
        /// Property to report errors
        /// </summary>
        public string? Error { get; set; } = null;

        /// <summary>
        /// List with absolute register addresses
        /// </summary>
        public List<UInt16> Address { get; set; } = new List<ushort>();
        
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
                    // Check if data in payload present and number of bytes is odd
                    if ((rawData.Count <= 0) || (rawData.Count % 2 == 1))
                    {
                        Error = $"Received package without data in payload";
                        break;
                    }

                    // Store payload as registers data
                    // Layout Data_MSB - Data_LSB
                    for (int i = 0; i < rawData.Count; i += 2)
                    {
                        Data.Add(BitOperationsExtensions.ConstructWordFromBytes(rawData[i+1], rawData[i]));
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
            List<byte> payloadToSend = new List<byte>();

            // Handle only 3 Address and Data sizes combinations
            if (Address.Count == Data.Count)
            {
                for (int i = 0; i < Address.Count; i++)
                {
                    // Layout: Addr_MSB - Addr_LSB - Data_MSB - Data_LSB
                    payloadToSend.Add(Address[i].GetLSB());
                    payloadToSend.Add(Address[i].GetMSB());
                    payloadToSend.Add(Data[i].GetLSB());
                    payloadToSend.Add(Data[i].GetMSB());
                }
            }
            else if ((Address.Count != 0) && (Data.Count == 0))
            {
                for (int i = 0; i < Address.Count; i++)
                {
                    // Layout: Addr_MSB - Addr_LSB
                    payloadToSend.Add(Address[i].GetLSB());
                    payloadToSend.Add(Address[i].GetMSB());
                }  
            }
            else if ((Address.Count == 0) && (Data.Count != 0))
            {
                for (int i = 0; i < Address.Count; i++)
                {
                    // Layout: Addr_MSB - Addr_LSB
                    payloadToSend.Add(Data[i].GetLSB());
                    payloadToSend.Add(Data[i].GetMSB());
                }  
            }
            else
            {
                throw new ArgumentException($"Unexpected Address-Data combination. Address length: {Address.Count}. Data length: {Data.Count}.");
            }

            return payloadToSend;
        }
    }
}
