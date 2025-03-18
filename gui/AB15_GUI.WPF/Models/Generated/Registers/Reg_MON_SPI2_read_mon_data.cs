using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models.Generated.Registers
{
    /// <summary>
    /// Class for working with MON_SPI2_read_mon_data register
    /// WARNING: this class is generated, do not modify manually!
    /// </summary>
    public class Reg_MON_SPI2_read_mon_data : IRegister
    {
        /// <summary>
        /// Reset value for register
        /// </summary>
        public UInt16 ResetValue { get; private set; } = 0x0000;

        /// <summary>
        /// Name of the register
        /// </summary>
        public string Name { get; private set; } = "MON_SPI2_read_mon_data";

        /// <summary>
        /// Absolute address of the register
        /// </summary>
        public UInt16 Address { get; private set; } = 0x0144;

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; } = "SPI MON2 read monitored data";

        /// <summary>
        /// Access level of this register
        /// </summary>
        public string Access { get; private set; } = "read-only";

        #region Fields instances
        /// <summary>
        /// monitored data on SPI MON2
        /// Bitwidth: 16
        /// </summary>
        public Field_MON_SPI2_Data MON_SPI2_Data { get; set; } = new Field_MON_SPI2_Data();
        #endregion // Fields instances

        /// <summary>
        /// Data property. Constructs register value from fields on get. Deconstruct register value by fields on set
        /// </summary>
        public UInt16 Data
        {
            get
            {
                UInt16 data = 0x0;

                data |= MON_SPI2_Data.GetPositionalValue();
                
                return data;
            }
            set
            {
                MON_SPI2_Data.UpdateValue(value);
                
            }
        }

        #region Field classes declarations
        /// <summary>
        /// Class for working with MON_SPI2_Data field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_MON_SPI2_Data
        {
            /// <summary>
            /// Max value that can be stored in field
            /// </summary>
            private readonly UInt16 _maxValue = ((1 << bitWidth) - 1);

            /// <summary>
            /// Read mask for field in register
            /// </summary>
            private readonly UInt16 _readMask = (UInt16) (((1 << bitWidth) - 1) << bitOffset);

            /// <summary>
            /// Bit offset of the field in register
            /// </summary>
            private const UInt16 bitOffset = 0;

            /// <summary>
            /// Bit width of the field in register
            /// </summary>
            private const UInt16 bitWidth = 16;

            /// <summary>
            /// Data stored in register
            /// </summary>
            private UInt16 data;

            /// <summary>
            /// Name of the field
            /// </summary>
            public string Name { get; private set; } = "MON_SPI2_Data";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "monitored data on SPI MON2";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-only";

            /// <summary>
            /// Bit offset of the field in register
            /// </summary>
            public UInt16 BitOffset 
            { 
                get
                {
                    return bitOffset;
                }
            }

            /// <summary>
            /// Bit width of the field in register
            /// </summary>
            public UInt16 BitWidth 
            { 
                get
                {
                    return bitWidth;
                }
            }

            /// <summary>
            /// Data property. Constructs register value from fields on get. Deconstruct register value by fields on set
            /// </summary>
            public UInt16 Data
            {
                get { return data; }
                set
                {
                    if (value > _maxValue)
                    {
                        throw new ArgumentOutOfRangeException("", $"Expected max value of {_maxValue}, but received {value}");
                    }

                    data = value;
                }
            }

            /// <summary>
            /// Gets field value from register
            /// </summary>
            /// <param name="registerValue">value of register</param>
            public void UpdateValue(UInt16 registerValue)
            {
                Data = (UInt16) ((UInt16)(registerValue & _readMask) >> bitOffset);
            }

            /// <summary>
            /// Return field value in correct bit position in register (other bits set to 0)
            /// </summary>
            /// <returns>field value in correct bit position in register</returns>
            public UInt16 GetPositionalValue()
            {
                return (UInt16) (Data << BitOffset);
            }
        }
        
        #endregion // Field classes declarations
    }
}