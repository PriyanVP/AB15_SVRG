using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation.Peers;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class used for sending WD Configuration to AB15
    /// </summary>
    public class WDConfigurePayload : IByteListSerializable
    {
        /// <summary>
        /// Class to hold decoded WD WatchdogStatus information
        /// </summary>
        public class WDConfiguration
        {
            /// <summary>
            /// Slow watchdog lock time (valid timing > lock time)
            /// </summary>
            public int SlowWatchdogLockTime { get; set; }

            /// <summary>
            /// Slow watchdog response time (valid timing <= response time)
            /// </summary>
            public int SlowWatchdogResponseTime { get; set; }

            /// <summary>
            /// Fast watchdog lock time (valid timing > lock time)
            /// </summary>
            public int FastWatchdogLockTime { get; set; }

            /// <summary>
            /// Fast watchdog response time (valid timing <= response time)
            /// </summary>
            public int FastWatchdogResponseTime { get; set; }

            /// <summary>
            /// EN0 disable threshold for WD1
            /// </summary>
            public int SlowWatchdogEn0DisThreshold { get; set; }

            /// <summary>
            /// EN0 disable threshold for WD2
            /// </summary>
            public int FastWatchdogEn0DisThreshold { get; set; }

        }

        /// <summary>
        /// Property to report errors
        /// </summary>
        public string? Error { get; private set; } = null;

        /// <summary>
        /// Property to send WD configuration information
        /// Error property should be set in such case
        /// </summary>
        /// TODO Create WD Status struct
        public WDConfiguration? WatchdogConfiguration{ get; set; } = null;

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
            List<byte> bytes = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00]; // 6 bytes for now

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

            return bytes;

        }

    }
}
