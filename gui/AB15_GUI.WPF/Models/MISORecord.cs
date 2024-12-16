using System;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds SPI transaction MISO frame
    /// </summary>
    public class MISORecord
    {       
        /// <summary>
        /// General status 5
        /// </summary>
        public bool GS5
        { 
            get
            {
                // Check if bit is set
                return (RawMISO & 0x8000_0000) != 0;
            }
        }

        /// <summary>
        /// General status 4
        /// </summary>
        public bool GS4
        { 
            get
            {
                // Check if bit is set
                return (RawMISO & 0x4000_0000) != 0;
            }
        }

        /// <summary>
        /// General status 3
        /// </summary>
        public bool GS3
        { 
            get
            {
                // Check if bit is set
                return (RawMISO & 0x2000_0000) != 0;
            }
        }

        /// <summary>
        /// General status 2
        /// </summary>
        public bool GS2
        { 
            get
            {
                // Check if bit is set
                return (RawMISO & 0x1000_0000) != 0;
            }
        }

        /// <summary>
        /// General status 1
        /// </summary>
        public bool GS1
        { 
            get
            {
                // Check if bit is set
                return (RawMISO & 0x800_0000) != 0;
            }
        }

        /// <summary>
        /// General status 0
        /// </summary>
        public bool GS0
        { 
            get
            {
                // Check if bit is set
                return (RawMISO & 0x400_0000) != 0;
            }
        }

        /// <summary>
        /// Sensor frame
        /// </summary>
        public bool SF
        { 
            get
            {
                // Check if bit is set
                return (RawMISO & 0x200_0000) != 0;
            }
        }

        /// <summary>
        /// Additional status flags
        /// </summary>
        public uint AdditionalStatus
        { 
            get
            {
                return (RawMISO & 0x1F0_0000) >> 20;
            }
        }

        /// <summary>
        /// Data
        /// </summary>
        public uint Data
        { 
            get
            {
                return (RawMISO & 0xF_FFF0) >> 4;
            }
        }

        /// <summary>
        /// ASIC error flag
        /// 0=ASIC error not present, 1=Oscillator Monitor Error (Oscillator failure) or
        /// Current APB Read Access Time Error (ATE) or 
        /// Current APB Read Bus Transaction Error(APB).
        /// </summary>
        public bool S0
        { 
            get
            {
                // Check if bit is set
                return (RawMISO & 0x8) != 0;
            }
        }

        /// <summary>
        /// CRC
        /// </summary>
        public uint CRC
        { 
            get
            {
                return (RawMISO & 0x7) >> 0;
            }
        }

        /// <summary>
        /// MOSI frame raw
        /// Can be set from outside for raw communication
        /// </summary>
        public uint RawMISO { get; set; }
    }
}
