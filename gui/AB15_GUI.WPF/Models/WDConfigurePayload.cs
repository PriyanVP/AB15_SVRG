using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Generated.Registers;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class used for sending WD Configuration to AB15
    /// </summary>
    public class WDConfigurePayload : IByteListSerializable
    {
        /// <summary>
        /// Register with WD1 main configuration
        /// </summary>
        public Reg_spi_config_wd1 spi_config_wd1 = new Reg_spi_config_wd1();

        /// <summary>
        /// Register with WD2 main configuration
        /// </summary>
        public Reg_spi_config_wd2 spi_config_wd2 = new Reg_spi_config_wd2();

        /// <summary>
        /// ENx decouple configuration
        /// </summary>
        public Reg_spi_config_wd_decouple spi_config_wd_decouple = new Reg_spi_config_wd_decouple();

        /// <summary>
        /// EN0 threshold configuration
        /// </summary>
        public Reg_spi_config_wd_thres0 spi_config_wd_thres0 = new Reg_spi_config_wd_thres0();

        /// <summary>
        /// Property to report errors
        /// </summary>
        public string? Error { get; set; } = null;

        /// <summary>
        /// Convert byte list to field values
        /// </summary>
        /// <param name="status">WatchdogStatus field of package</param>
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
        /// <returns>Byte list of PC-MCU payload data</returns>
        public List<byte> Serialize()
        {
            List<byte> serializedPackage = new List<byte>();

            // spi_config_wd1
            serializedPackage.Add(spi_config_wd1.Address.GetLSB());
            serializedPackage.Add(spi_config_wd1.Address.GetMSB());
            serializedPackage.Add(spi_config_wd1.Data.GetLSB());
            serializedPackage.Add(spi_config_wd1.Data.GetMSB());

            // spi_config_wd2
            serializedPackage.Add(spi_config_wd2.Address.GetLSB());
            serializedPackage.Add(spi_config_wd2.Address.GetMSB());
            serializedPackage.Add(spi_config_wd2.Data.GetLSB());
            serializedPackage.Add(spi_config_wd2.Data.GetMSB());

            // spi_config_wd_decouple
            serializedPackage.Add(spi_config_wd_decouple.Address.GetLSB());
            serializedPackage.Add(spi_config_wd_decouple.Address.GetMSB());
            serializedPackage.Add(spi_config_wd_decouple.Data.GetLSB());
            serializedPackage.Add(spi_config_wd_decouple.Data.GetMSB());

            // spi_config_wd_thres0
            serializedPackage.Add(spi_config_wd_thres0.Address.GetLSB());
            serializedPackage.Add(spi_config_wd_thres0.Address.GetMSB());
            serializedPackage.Add(spi_config_wd_thres0.Data.GetLSB());
            serializedPackage.Add(spi_config_wd_thres0.Data.GetMSB());

            // SPI_CONFIG_WD1 - 0x21
            // 0..5  spi_set_responsetime_wd1 (6 bits)
            // 6..11 spi_set_locktime_wd1 (6 bits)

            // SPI_CONFIG_WD2 - 0x22
            // 0..5  spi_set_responsetime_wd2 (6 bits)
            // 6..11 spi_set_locktime_wd2 (6 bits)

            // SPI_CONFIG_WD_DECOUPLE - 0x23
            // 0     spi_decouple_wd_en0 (1 bit)

            // SPI_CONFIG_WD_THRES0 - 0x24
            // 0..5  spi_set_en0_thre_wd1 (6 bits)
            // 8..13 spi_set_en0_thre_wd2 (6 bits)

            // SPI_SET_WDSETTINGS - 0x2E
            // 0     spi_init_asic ... for ASIC initialization
            // 1     spi_res_wd    ... for resetting the watchdog logic
            // 6     spi_on_sl     ... to activate watchdog logic
            // 7     spi_set_slff  ... to set the overall watchdog fault directly
            // 8     spi_set_wd1_clock (slow wd) ... 1MHz clock for testing only
            // 9     spi_set_wd2_clock (fast wd) ... 1MHz clock for testing only
            // 10    spi_lock_wd_clock ... to lock wd clock frequency

            return serializedPackage;
        }

    }
}
