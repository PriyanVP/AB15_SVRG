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
                address = value > MAX_ADDRESS ? MAX_ADDRESS : value;
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

                // Update fields
                address = (RawMOSI & ADDRESS_MASK) >> ADDRESS_OFFSET;
                rwFlag  = (RawMOSI & RW_FLAG_MASK) != 0;
                data    = (ushort) ((RawMOSI & DATA_MASK) >> DATA_OFFSET);
                crc     = (RawMOSI & CRC_MASK) >> CRC_OFFSET;

                OnPropertyChanged(nameof(Address));
                OnPropertyChanged(nameof(RwFlag));
                OnPropertyChanged(nameof(Data));
                OnPropertyChanged(nameof(CRC));
            } 
        }

        /// <summary>
        /// Create copy of current object (new reference)
        /// </summary>
        /// <returns>New MOSIRecord object</returns>
        public MOSIRecord Copy()
        {
            return new MOSIRecord() { RawMOSI = this.RawMOSI };
        }

        /// <summary>
        /// Method to construct frame data from fields. Also implements logic of SPI frame
        /// </summary>
        private void UpdateFrame()
        {
            // Sets int value of frame from other fields
            rawMOSI  = 0x0;
            rawMOSI |= Address << ADDRESS_OFFSET;
            rawMOSI |= Convert.ToUInt32(RwFlag) << RW_FLAG_OFFSET;
            rawMOSI |= Convert.ToUInt32(Data) << DATA_OFFSET;
            CRC      = RawMOSI.GetCRC3(CRC_START_IDX, CRC_END_IDX);
            rawMOSI |= CRC << CRC_OFFSET;

            OnPropertyChanged(nameof(RawMOSI));
        }

        #region Constants

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
        /// Address field offset in SPI frame
        /// </summary>
        private const int ADDRESS_OFFSET = 22;
        
        /// <summary>
        /// Address field read mask in SPI frame
        /// </summary>
        private const uint ADDRESS_MASK  = 0xFFC0_0000;

        /// <summary>
        /// RW flag offset in SPI frame
        /// </summary>
        private const int RW_FLAG_OFFSET = 21;

        /// <summary>
        /// RW flag read mask in SPI frame
        /// </summary>
        private const uint RW_FLAG_MASK  = 0x0020_0000;

        /// <summary>
        /// Data field offset in SPI frame
        /// </summary>
        private const int DATA_OFFSET = 5;
        
        /// <summary>
        /// Data field read mask in SPI frame
        /// </summary>
        private const uint DATA_MASK  = 0x001F_FFE0;

        /// <summary>
        /// CRC3 field offset in SPI frame
        /// </summary>
        private const int CRC_OFFSET = 2;
        
        /// <summary>
        /// CRC3 field read mask in SPI frame
        /// </summary>
        private const uint CRC_MASK  = 0x0000_001C;

        #endregion // Constants

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
