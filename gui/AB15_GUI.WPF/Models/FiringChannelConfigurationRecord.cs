using System;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds channel specific configuration data
    /// </summary>
    public class FiringChannelConfigurationRecord
    {
        /// <summary>
        /// ASIC ID
        /// </summary>
        public int ASICID { get; set; }
        
        /// <summary>
        /// Channel ID
        /// </summary>
        public int ChannelID { get; set; }
        
        /// <summary>
        /// <inheritdoc cref="Identifier" path='/summary'/>
        /// </summary>
        private string identifier = "None";
        
        /// <summary>
        /// Custom channel identifier
        /// Note: identifier is not guaranteed to be unique
        /// </summary>
        public string Identifier
        { 
            get
            {
                return identifier;
            }
            set
            {
                // Length limitation
                if (value.Length > 15) return;
                identifier = value;
            }
        }
        
        /// <summary>
        /// <inheritdoc cref="Mode" path='/summary'/>
        /// </summary>
        private int mode = 0;
        
        /// <summary>
        /// Channel firing mode
        /// </summary>
        public int Mode 
        { 
            get
            {
                return mode;
            } 
            set
            {
                // Verify range
                if ((value < 0) || (value > 9))
                {
                    throw new ArgumentOutOfRangeException($"Unexpected value");
                }
                mode = value;
            } 
        }
    }
}
