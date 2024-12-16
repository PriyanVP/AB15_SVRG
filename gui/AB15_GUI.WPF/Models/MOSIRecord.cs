using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds SPI transaction MOSI frame
    /// </summary>
    public class MOSIRecord : INotifyPropertyChanged
    {      
        /// <summary>
        /// Maximum address in ASIC
        /// </summary>
        private const int MAX_ADDRESS = 0x3F0;

        /// <summary>
        /// Maximum CRC3 value in ASIC
        /// </summary>
        private const int MAX_CRC = 0x7;

        /// <summary>
        /// Start index for CRC calculation
        /// </summary>
        private const int CRC_START_IDX = 5;

        /// <summary>
        /// End index for CRC calculation
        /// </summary>
        private const int CRC_END_IDX = 31;

        /// <summary>
        /// <inheritdoc cref="Address" path='/summary'/>
        /// </summary>
        public uint address;

        /// <summary>
        /// Address of register
        /// </summary>
        public uint Address 
        { 
            get => address;
            set
            {
                // Validation
                if (value > MAX_ADDRESS)
                {
                    throw new ArgumentOutOfRangeException("Incorrect value for address!");
                }

                address = value;
                OnPropertyChanged();

                // Trigger frame update
                UpdateFrame();
            }
        }

        /// <summary>
        /// <inheritdoc cref="RwFlag" path='/summary'/>
        /// </summary>
        public bool rwFlag;

        /// <summary>
        /// Read/write flag
        /// False - read, true - write
        /// </summary>
        public bool RwFlag
        { 
            get => rwFlag;
            set
            {
                rwFlag = value;
                OnPropertyChanged();

                // Trigger frame update
                UpdateFrame();
            }
        }

        /// <summary>
        /// <inheritdoc cref="Data" path='/summary'/>
        /// </summary>
        public UInt16 data;

        /// <summary>
        /// Register data
        /// Should be 0 for write
        /// </summary>
        public UInt16 Data 
        { 
            get => data;
            set
            {
                data = value;
                OnPropertyChanged();

                // Trigger frame update
                UpdateFrame();
            }
        }

        /// <summary>
        /// <inheritdoc cref="CRC" path='/summary'/>
        /// </summary>
        public uint crc;

        /// <summary>
        /// CRC3 value. Can only be set internally
        /// </summary>
        public uint CRC 
        { 
            get => crc;
            private set
            {
                // Validation
                if (value > MAX_CRC)
                {
                    throw new ArgumentOutOfRangeException("Incorrect value for crc3!");
                }

                crc = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="RawMOSI" path='/summary'/>
        /// </summary>
        public uint rawMOSI;

        /// <summary>
        /// MOSI frame raw
        /// Can be set from outside for raw communication
        /// </summary>
        public uint RawMOSI 
        { 
            get => rawMOSI;
            set
            {
                rawMOSI = value;
                OnPropertyChanged();
            } 
        }

        /// <summary>
        /// Method to construct frame data from fields. Also implements logic of SPI frame
        /// </summary>
        private void UpdateFrame()
        {
            // Update Data in case write option selected
            Data = (RwFlag) ? ((UInt16) 0x0) : (Data);

            // Sets int value of frame from other fields
            RawMOSI  = 0x0;
            RawMOSI |= Address << 22;
            RawMOSI |= Convert.ToUInt32(RwFlag) << 21;
            RawMOSI |= Convert.ToUInt32(Data) << 5;
            CRC      = RawMOSI.GetCRC3(CRC_START_IDX, CRC_END_IDX);
            RawMOSI |= CRC << 2;
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
