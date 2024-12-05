using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Generated.Registers;
using System.Diagnostics;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class used for reading WD Status
    /// Receiving Register content / SPI command result for reading WD WatchdogStatus flags
    /// 
    /// Assumptions:
    /// AB12: Response Payload of "Read WD Status" command (16bits) + WDF from GS bits (1 byte)
    /// AB15: Several bytes of register contents (tbd)
    /// </summary>
    public class WDStatusPayload : IByteListSerializable
    {
        /// <summary>
        /// WD1 status register
        /// </summary>
        public Reg_spi_read_wdstatus1 spi_read_wdstatus1 = new Reg_spi_read_wdstatus1();
        
        /// <summary>
        /// WD2 status register
        /// </summary>
        public Reg_spi_read_wdstatus2 spi_read_wdstatus2 = new Reg_spi_read_wdstatus2();
        
        /// <summary>
        /// ENx status register
        /// </summary>
        public Reg_spi_read_enx spi_read_enx = new Reg_spi_read_enx();

        /// <summary>
        /// QA config register
        /// </summary>
        public Reg_spi_read_wdqa spi_read_wdqa = new Reg_spi_read_wdqa();

        /// <summary>
        /// Property to report errors
        /// </summary>
        public string? Error { get; set; } = null;

        /// <summary>
        /// Convert byte list to field values
        /// </summary>
        /// <param name="status">MCU Status field of package</param>
        /// <param name="rawData">Data that should be converted to parameters values (payload only)</param>
        public void Deserialize(MCUStatus status, List<byte> rawData)
        {
            // Apply different handling based on WatchdogStatus
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
                    // WD Status was received
                    // Check if correct amount of data in payload present
                    if (rawData.Count < 8)
                    {
                        Error = $"Received package without correct data length in payload";
                        break;
                    }

                    // Unpack data to fields
                    spi_read_wdstatus1.Data = BitOperationsExtensions.ConstructWordFromBytes(rawData[1], rawData[0]);
                    spi_read_wdstatus2.Data = BitOperationsExtensions.ConstructWordFromBytes(rawData[3], rawData[2]);
                    spi_read_enx.Data = BitOperationsExtensions.ConstructWordFromBytes(rawData[5], rawData[4]);
                    spi_read_wdqa.Data = BitOperationsExtensions.ConstructWordFromBytes(rawData[7], rawData[6]);

                    break;
                default:
                    throw new ArgumentException($"Unexpected status received: {status}");
            }
        }

        /// <summary>
        /// Converts payload data to byte list
        /// - UNUSED -
        /// </summary>
        /// <returns>Empty list</returns>
        public List<byte> Serialize()
        {
            return new List<byte>();
        }
    }
}
