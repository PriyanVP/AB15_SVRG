using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;

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
        /// Class to hold decoded WD WatchdogStatus information
        /// </summary>
        public class WDStatus
        {
            /// <summary>
            /// Indicates locked safety logic configuration
            /// AB12: null, AB15: spi_on_sl
            /// </summary>
            // public bool? slLocked;

            /// <summary>
            /// Indicates active watchdog safety logic
            /// AB12: null, AB15: wd_on_spi
            /// </summary>
            // public bool? wdActive;

            /// <summary>
            /// Indicates overall watchdog fault.
            /// AB12: WDF in GS bits, AB15: slff_set_spi
            /// </summary>
            public bool WatchdogFault { get; set; }

            /// <summary>
            /// Indicates a watchdog timing fault in fast or slow WD
            /// AB12: wd2 OR wd3, AB15: wd_set_slff_spi
            /// </summary>
            // public bool watchdogTimingFault;

            /// <summary>
            /// Indicates slow watchdog fault
            /// AB12: WD3, AB15: wd1_set_slff_spi
            /// </summary>
            public bool SlowWatchdogFault { get; set; }

            /// <summary>
            /// Indicates fast watchdog fault
            /// AB12: WD2, AB15: wd2_set_slff_spi
            /// </summary>
            public bool FastWatchdogFault { get; set; }

            /// <summary>
            /// Indicates oscillator timing fault
            /// AB12: WD1, AB15: oscfail_set_slff_spi
            /// </summary>
            public bool OscillatorFault { get; set; }

            /// <summary>
            /// Indicates oscillator timing fault Underflow
            /// AB12: u1, AB15: null
            /// </summary>
            public bool OscillatorUnderflow { get; set; }

            /// <summary>
            /// Indicates oscillator timing fault Underflow
            /// AB12: o1, AB15: null
            /// </summary>
            public bool OscillatorOverflow { get; set; }

            /// <summary>
            /// Indicates slow watchdog overflow (too slow or missing)
            /// AB12: o3 in WD_STATUS, AB15: null
            /// </summary>
            public bool SlowWatchdogOverflow { get; set; }

            /// <summary>
            /// AB12: r3 in WD_STATUS, AB15: qa1_set_slff_spi
            /// </summary>
            public bool SlowWatchdogQAFault { get; set; }

            /// <summary>
            /// Indicates a fast watchdog timing underflow (too fast)
            /// AB12: u2, AB15: null
            /// </summary>
            public bool FastWatchdogUnderflow { get; set; }

            /// <summary>
            /// Indicates a fast watchdog timing overflow (too slow or missing)
            /// AB12: o2, AB15: null
            /// </summary>
            public bool FastWatchdogOverflow { get; set; }

            /// <summary>
            /// AB12: r2 in WD_STATUS, AB15: qa2_set_slff_spi
            /// </summary>
            public bool FastWatchdogQAFault { get; set; }
        }

        /// <summary>
        /// Property to report errors
        /// </summary>
        public string? Error { get; private set; } = null;

        /// <summary>
        /// Property to report WD WatchdogStatus information
        /// Null corresponds to no WatchdogStatus information.
        /// Error property should be set in such case
        /// </summary>
        public WDStatus? WatchdogStatus{ get; private set; } = null;

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
                    if (rawData.Count < 3)
                    {
                        Error = $"Received package without correct datalength in payload";
                        break;
                    }

                    // Based on WD WatchdogStatus code - decode the bitfields of the payload
                    // TODO Get the rawData bytes and extract the boolean flags as the bitfield is defined
                    decodeStatus(rawData);

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

        /// <summary>
        /// Decode the bitfield(s) into the WD WatchdogStatus struct elements
        /// If this method is called, dataBytes is not null and the message did not present an error
        /// AB12 Payload: WD Status Payload 16 bits (MSB, LSB) + WDF flag as 1 byte
        /// </summary>
        /// <param name="dataBytes">Payload bytes from the response to Read WD WatchdogStatus</param>
        /// <returns></returns>
        private void decodeStatus(List<byte> dataBytes)
        {
            // AB12: 3 bytes
            WatchdogStatus = new WDStatus();

            byte wdsMSB = dataBytes[0];
            byte wdsLSB = dataBytes[1];
            byte wdf    = dataBytes[2];

            // WDF bit from GS bits
            WatchdogStatus.WatchdogFault = (wdf != 0) ; // true if WDF byte is != 0

            // MSB bits
            WatchdogStatus.SlowWatchdogFault = (wdsMSB & 0b00010000) != 0;
            WatchdogStatus.FastWatchdogFault = (wdsMSB & 0b00001000) != 0;
            WatchdogStatus.OscillatorFault   = (wdsMSB & 0b00000100) != 0;

            // LSB bits
            WatchdogStatus.OscillatorUnderflow   = (wdsLSB & 0b00000001) != 0;
            WatchdogStatus.OscillatorOverflow    = (wdsLSB & 0b00000010) != 0;

            WatchdogStatus.FastWatchdogUnderflow = (wdsLSB & 0b00000100) != 0;
            WatchdogStatus.FastWatchdogOverflow  = (wdsLSB & 0b00001000) != 0;
            WatchdogStatus.FastWatchdogQAFault   = (wdsLSB & 0b00010000) != 0;

            WatchdogStatus.SlowWatchdogOverflow  = (wdsLSB & 0b00100000) != 0;
            WatchdogStatus.SlowWatchdogQAFault   = (wdsLSB & 0b01000000) != 0;

            return;
        }
    }
}
