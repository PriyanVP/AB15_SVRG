using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds channel specific configuration data
    /// </summary>
    public class FiringChannelConfigurationRecord : INotifyPropertyChanged
    {
        /// <summary>
        /// <inheritdoc cref="ASICID" path='/summary'/>
        /// </summary>
        public int asicID;

        /// <summary>
        /// ASIC ID
        /// </summary>
        public int ASICID 
        { 
            get
            {
                return asicID;
            } 
            set
            {
                asicID = value;
                OnPropertyChanged();
            } 
        }

        /// <summary>
        /// <inheritdoc cref="ChannelID" path='/summary'/>
        /// </summary>
        public int channelID;
        
        /// <summary>
        /// Channel ID
        /// </summary>
        public int ChannelID 
        { 
            get
            {
                return channelID;
            } 
            set
            {
                if ((value < 1) || (value > 20))
                {
                    throw new ArgumentOutOfRangeException($"Unexpected value for channel ID");
                }
                channelID = value;
                OnPropertyChanged();
            } 
        }
        
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
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// List with applicable modes for firing channels 1-8
        /// </summary>
        private readonly List<int> _applicableModesCh1_8_17_20  = new List<int>() { 0x0, 0x2, 0x3, 0x4, 0x5, 0x6, 0xC, 0xD, 0x10 };

        /// <summary>
        /// List with applicable modes for firing channels 9-20
        /// </summary>
        private readonly List<int> _applicableModesCh9_16 = new List<int>() { 0x0, 0x2, 0x3, 0x4, 0x5, 0x6, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF, 0x10, 0x11 };
        
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
                switch (ChannelID)
                {
                    // TODO: verify approach - incorrect mode can be set by user which will lead to exception
                    case >= 0 and < 9:
                    case > 16 and <= 20:
                        if (_applicableModesCh1_8_17_20.Contains(value) == false)
                        {
                            throw new ArgumentOutOfRangeException($"Unexpected value for mode");
                        }
                        break;
                    case >= 9 and <= 16:
                        if (_applicableModesCh9_16.Contains(value) == false)
                        {
                            throw new ArgumentOutOfRangeException($"Unexpected value for mode");
                        }
                        break;   
                }

                mode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ModeDescription));
            } 
        }

        /// <summary>
        /// Dictionary for retrieving firing mode description
        /// </summary>
        /// <typeparam name="int">register field value for mode</typeparam>
        /// <typeparam name="string">mode description</typeparam>
        private readonly Dictionary<int, string> _modeDescriptions = new Dictionary<int, string>()
        {
            {0x0,  "off, no function"},
            {0x2,  "Fire Mode 1: 1.75A 0.7ms"},
            {0x3,  "Fire Mode 1a: 1.75A 0.5ms"},
            {0x4,  "Fire Mode 2: 1.2A 2.2ms"},
            {0x5,  "Fire Mode 2a: 1.2A 2.0ms"},
            {0x6,  "Fire Mode 3: 1.75A 0.7ms (+opt 1.2A 1.2ms)"},
            {0x8,  "Fire Mode 4 LEA: 1.75A 0.7ms"},
            {0x9,  "Fire Mode 4a LEA: 1.75A 0.5ms"},
            {0xA,  "Fire Mode 5 LEA: 1.2A 2.2ms"},
            {0xB,  "Fire Mode 5a LEA: 1.2A 2.0ms"},
            {0xC,  "Fire Mode 6: 1.75A 0.7ms, can be switched to 1.2A 2.2ms by flm_fire_mode_sel=1"},
            {0xD,  "Fire Mode 6a: 1.75A 0.5ms, can be switched to 1.2A 2.0ms by flm_fire_mode_sel=1"},
            {0xE,  "Fire Mode 7 LEA: 1.75A 0.7ms, can be switched to 1.2A 2.2ms by flm_fire_mode_sel=1"},
            {0xF,  "Fire Mode 7a LEA: 1.75A 0.5ms, can be switched to 1.2A 2.0ms by flm_fire_mode_sel=1"},
            {0x10, "Fire Mode 8: 1.75A 0.7ms, can be switched to 1.2A 2.0ms by flm_fire_mode_sel=1"},
            {0x11, "Fire Mode 9: LEA: 1.75A 0.7ms, can be switched to 1.2A 2.0ms by flm_fire_mode_sel=1"}
        };

        /// <summary>
        /// Channel firing mode description
        /// </summary>
        public string ModeDescription
        {
            get
            {
                return _modeDescriptions[Mode];
            } 
        }

        #region Services

        /// <summary>
        /// Event for notification if property has changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise event to notify about property change
        /// </summary>
        /// <param name="propertyName">name of property</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Sanity check
            if (propertyName == null) throw new ArgumentException("Property name can't be null!");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // Services
    }
}