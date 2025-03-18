using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models.Generated.Registers
{
    /// <summary>
    /// Class for working with PSI_Read_Data_Slot2_Ch6 register
    /// WARNING: this class is generated, do not modify manually!
    /// </summary>
    public class Reg_PSI_Read_Data_Slot2_Ch6 : IRegister
    {
        /// <summary>
        /// Reset value for register
        /// </summary>
        public UInt16 ResetValue { get; private set; } = 0x0009;

        /// <summary>
        /// Name of the register
        /// </summary>
        public string Name { get; private set; } = "PSI_Read_Data_Slot2_Ch6";

        /// <summary>
        /// Absolute address of the register
        /// </summary>
        public UInt16 Address { get; private set; } = 0x0215;

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; } = "Read PSI data of selected channel and Slot      ";

        /// <summary>
        /// Access level of this register
        /// </summary>
        public string Access { get; private set; } = "read-only";

        #region Fields instances
        /// <summary>
        /// psi_read_data_slot2_ch6
        /// Bitwidth: 16
        /// </summary>
        public Field_psi_read_data_slot2_ch6 psi_read_data_slot2_ch6 { get; set; } = new Field_psi_read_data_slot2_ch6();
        #endregion // Fields instances

        /// <summary>
        /// Data property. Constructs register value from fields on get. Deconstruct register value by fields on set
        /// </summary>
        public UInt16 Data
        {
            get
            {
                UInt16 data = 0x0;

                data |= psi_read_data_slot2_ch6.GetPositionalValue();
                
                return data;
            }
            set
            {
                psi_read_data_slot2_ch6.UpdateValue(value);
                
            }
        }

        #region Field classes declarations
        /// <summary>
        /// Class for working with psi_read_data_slot2_ch6 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_read_data_slot2_ch6
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
            public string Name { get; private set; } = "psi_read_data_slot2_ch6";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "psi_read_data_slot2_ch6";

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