using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models.Generated.Registers
{
    /// <summary>
    /// Class for working with FLM_Read_Lea_Diode_ch20_17 register
    /// WARNING: this class is generated, do not modify manually!
    /// </summary>
    public class Reg_FLM_Read_Lea_Diode_ch20_17 : IRegister
    {
        /// <summary>
        /// Reset value for register
        /// </summary>
        public UInt16 ResetValue { get; private set; } = 0x0000;

        /// <summary>
        /// Name of the register
        /// </summary>
        public string Name { get; private set; } = "FLM_Read_Lea_Diode_ch20_17";

        /// <summary>
        /// Absolute address of the register
        /// </summary>
        public UInt16 Address { get; private set; } = 0x01d5;

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; } = "FLM result of lea diode polarity test Ch20 to Ch17";

        /// <summary>
        /// Access level of this register
        /// </summary>
        public string Access { get; private set; } = "read-only";

        #region Fields instances
        /// <summary>
        /// lea diode polarity test error
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_lea_diode_pol_err_ch20 flm_lea_diode_pol_err_ch20 { get; set; } = new Field_flm_lea_diode_pol_err_ch20();
        
        /// <summary>
        /// lea diode polarity test error
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_lea_diode_pol_err_ch19 flm_lea_diode_pol_err_ch19 { get; set; } = new Field_flm_lea_diode_pol_err_ch19();
        
        /// <summary>
        /// lea diode polarity test error
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_lea_diode_pol_err_ch18 flm_lea_diode_pol_err_ch18 { get; set; } = new Field_flm_lea_diode_pol_err_ch18();
        
        /// <summary>
        /// lea diode polarity test error
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_lea_diode_pol_err_ch17 flm_lea_diode_pol_err_ch17 { get; set; } = new Field_flm_lea_diode_pol_err_ch17();
        #endregion // Fields instances

        /// <summary>
        /// Data property. Constructs register value from fields on get. Deconstruct register value by fields on set
        /// </summary>
        public UInt16 Data
        {
            get
            {
                UInt16 data = 0x0;

                data |= flm_lea_diode_pol_err_ch20.GetPositionalValue();
                data |= flm_lea_diode_pol_err_ch19.GetPositionalValue();
                data |= flm_lea_diode_pol_err_ch18.GetPositionalValue();
                data |= flm_lea_diode_pol_err_ch17.GetPositionalValue();
                
                return data;
            }
            set
            {
                flm_lea_diode_pol_err_ch20.UpdateValue(value);
                flm_lea_diode_pol_err_ch19.UpdateValue(value);
                flm_lea_diode_pol_err_ch18.UpdateValue(value);
                flm_lea_diode_pol_err_ch17.UpdateValue(value);
                
            }
        }

        #region Field classes declarations
        /// <summary>
        /// Class for working with flm_lea_diode_pol_err_ch20 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_lea_diode_pol_err_ch20
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
            private const UInt16 bitOffset = 3;

            /// <summary>
            /// Bit width of the field in register
            /// </summary>
            private const UInt16 bitWidth = 1;

            /// <summary>
            /// Data stored in register
            /// </summary>
            private UInt16 data;

            /// <summary>
            /// Name of the field
            /// </summary>
            public string Name { get; private set; } = "flm_lea_diode_pol_err_ch20";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "lea diode polarity test error";

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
        
        /// <summary>
        /// Class for working with flm_lea_diode_pol_err_ch19 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_lea_diode_pol_err_ch19
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
            private const UInt16 bitOffset = 2;

            /// <summary>
            /// Bit width of the field in register
            /// </summary>
            private const UInt16 bitWidth = 1;

            /// <summary>
            /// Data stored in register
            /// </summary>
            private UInt16 data;

            /// <summary>
            /// Name of the field
            /// </summary>
            public string Name { get; private set; } = "flm_lea_diode_pol_err_ch19";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "lea diode polarity test error";

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
        
        /// <summary>
        /// Class for working with flm_lea_diode_pol_err_ch18 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_lea_diode_pol_err_ch18
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
            private const UInt16 bitOffset = 1;

            /// <summary>
            /// Bit width of the field in register
            /// </summary>
            private const UInt16 bitWidth = 1;

            /// <summary>
            /// Data stored in register
            /// </summary>
            private UInt16 data;

            /// <summary>
            /// Name of the field
            /// </summary>
            public string Name { get; private set; } = "flm_lea_diode_pol_err_ch18";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "lea diode polarity test error";

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
        
        /// <summary>
        /// Class for working with flm_lea_diode_pol_err_ch17 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_lea_diode_pol_err_ch17
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
            private const UInt16 bitWidth = 1;

            /// <summary>
            /// Data stored in register
            /// </summary>
            private UInt16 data;

            /// <summary>
            /// Name of the field
            /// </summary>
            public string Name { get; private set; } = "flm_lea_diode_pol_err_ch17";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "lea diode polarity test error";

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