using System;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds SPI transaction
    /// </summary>
    public class SPITransactionRecord
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="miso">optional MISO frame object</param>
        /// <param name="mosi">optional MOSI frame object</param>
        public SPITransactionRecord(MISORecord? miso = null, MOSIRecord? mosi = null)
        {
            // Set MOSI and MISO frame if provided for constructor
            MISO = miso ?? (new MISORecord() { RawMISO = 0xFFFF_FFFF });
            MOSI = mosi ?? (new MOSIRecord() { RawMOSI = 0x0000_0000 });
        }

        /// <summary>
        /// Time of transaction
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Flag indicating if raw transaction used (for example emulating sensor data)
        /// </summary>
        public bool IsRawSPIFrame { get; set; }

        /// <summary>
        /// MISO frame
        /// </summary>
        public MISORecord MISO { get; set; }

        /// <summary>
        /// MOSI frame
        /// </summary>
        public MOSIRecord MOSI { get; set; }
    }
}
