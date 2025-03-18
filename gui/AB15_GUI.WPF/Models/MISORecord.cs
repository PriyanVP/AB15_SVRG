using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds SPI transaction MISO frame
    /// </summary>
    public class MISORecord : INotifyPropertyChanged
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
        /// <inheritdoc cref="RawMISO" path='/summary'/>
        /// </summary>
        private uint rawMISO;

        /// <summary>
        /// MISO frame raw
        /// Can be set from outside for raw communication
        /// </summary>
        public uint RawMISO 
        { 
            get => rawMISO;
            set
            {
                rawMISO = value;
                OnPropertyChanged();

                // Also raise event on all other properties
                OnPropertyChanged(nameof(GS5));
                OnPropertyChanged(nameof(GS4));
                OnPropertyChanged(nameof(GS3));
                OnPropertyChanged(nameof(GS2));
                OnPropertyChanged(nameof(GS1));
                OnPropertyChanged(nameof(GS0));
                OnPropertyChanged(nameof(SF));
                OnPropertyChanged(nameof(AdditionalStatus));
                OnPropertyChanged(nameof(Data));
                OnPropertyChanged(nameof(S0));
                OnPropertyChanged(nameof(CRC));
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
