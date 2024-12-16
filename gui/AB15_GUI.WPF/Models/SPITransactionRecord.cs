using System;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds SPI transaction
    /// </summary>
    public class SPITransactionRecord
    {       
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
