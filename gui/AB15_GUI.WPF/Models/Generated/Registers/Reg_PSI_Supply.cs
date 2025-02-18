using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models.Generated.Registers
{
    /// <summary>
    /// Class for working with PSI_Supply register
    /// WARNING: this class is generated, do not modify manually!
    /// </summary>
    public class Reg_PSI_Supply : IRegister
    {
        /// <summary>
        /// Reset value for register
        /// </summary>
        public UInt16 ResetValue { get; private set; } = 0x0;

        /// <summary>
        /// Name of the register
        /// </summary>
        public string Name { get; private set; } = "PSI_Supply";

        /// <summary>
        /// Absolute address of the register
        /// </summary>
        public UInt16 Address { get; private set; } = 0x00b0;

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; } = "switch PSI Supply";

        /// <summary>
        /// Access level of this register
        /// </summary>
        public string Access { get; private set; } = "read-write";

        #region Fields instances
        /// <summary>
        /// Disable data threshold regulation (Debug Option for EMC)
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_disable_data_threshold_regulation_ch8 psi_disable_data_threshold_regulation_ch8 { get; set; } = new Field_psi_disable_data_threshold_regulation_ch8();
        
        /// <summary>
        /// Disable data threshold regulation (Debug Option for EMC)
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_disable_data_threshold_regulation_ch7 psi_disable_data_threshold_regulation_ch7 { get; set; } = new Field_psi_disable_data_threshold_regulation_ch7();
        
        /// <summary>
        /// Disable data threshold regulation (Debug Option for EMC)
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_disable_data_threshold_regulation_ch6 psi_disable_data_threshold_regulation_ch6 { get; set; } = new Field_psi_disable_data_threshold_regulation_ch6();
        
        /// <summary>
        /// Disable data threshold regulation (Debug Option for EMC)
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_disable_data_threshold_regulation_ch5 psi_disable_data_threshold_regulation_ch5 { get; set; } = new Field_psi_disable_data_threshold_regulation_ch5();
        
        /// <summary>
        /// Disable data threshold regulation (Debug Option for EMC)
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_disable_data_threshold_regulation_ch4 psi_disable_data_threshold_regulation_ch4 { get; set; } = new Field_psi_disable_data_threshold_regulation_ch4();
        
        /// <summary>
        /// Disable data threshold regulation (Debug Option for EMC)
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_disable_data_threshold_regulation_ch3 psi_disable_data_threshold_regulation_ch3 { get; set; } = new Field_psi_disable_data_threshold_regulation_ch3();
        
        /// <summary>
        /// Disable data threshold regulation (Debug Option for EMC)
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_disable_data_threshold_regulation_ch2 psi_disable_data_threshold_regulation_ch2 { get; set; } = new Field_psi_disable_data_threshold_regulation_ch2();
        
        /// <summary>
        /// Disable data threshold regulation (Debug Option for EMC)
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_disable_data_threshold_regulation_ch1 psi_disable_data_threshold_regulation_ch1 { get; set; } = new Field_psi_disable_data_threshold_regulation_ch1();
        
        /// <summary>
        /// switch psi supply for selected channels, Bit 0 is mapped to channel 1
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_supply_on_ch8 psi_supply_on_ch8 { get; set; } = new Field_psi_supply_on_ch8();
        
        /// <summary>
        /// switch psi supply for selected channels, Bit 0 is mapped to channel 1
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_supply_on_ch7 psi_supply_on_ch7 { get; set; } = new Field_psi_supply_on_ch7();
        
        /// <summary>
        /// switch psi supply for selected channels, Bit 0 is mapped to channel 1
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_supply_on_ch6 psi_supply_on_ch6 { get; set; } = new Field_psi_supply_on_ch6();
        
        /// <summary>
        /// switch psi supply for selected channels, Bit 0 is mapped to channel 1
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_supply_on_ch5 psi_supply_on_ch5 { get; set; } = new Field_psi_supply_on_ch5();
        
        /// <summary>
        /// switch psi supply for selected channels, Bit 0 is mapped to channel 1
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_supply_on_ch4 psi_supply_on_ch4 { get; set; } = new Field_psi_supply_on_ch4();
        
        /// <summary>
        /// switch psi supply for selected channels, Bit 0 is mapped to channel 1
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_supply_on_ch3 psi_supply_on_ch3 { get; set; } = new Field_psi_supply_on_ch3();
        
        /// <summary>
        /// switch psi supply for selected channels, Bit 0 is mapped to channel 1
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_supply_on_ch2 psi_supply_on_ch2 { get; set; } = new Field_psi_supply_on_ch2();
        
        /// <summary>
        /// switch psi supply for selected channels, Bit 0 is mapped to channel 1
        /// Bitwidth: 1
        /// </summary>
        public Field_psi_supply_on_ch1 psi_supply_on_ch1 { get; set; } = new Field_psi_supply_on_ch1();
        #endregion // Fields instances

        /// <summary>
        /// Data property. Constructs register value from fields on get. Deconstruct register value by fields on set
        /// </summary>
        public UInt16 Data
        {
            get
            {
                UInt16 data = 0x0;

                data |= psi_disable_data_threshold_regulation_ch8.GetPositionalValue();
                data |= psi_disable_data_threshold_regulation_ch7.GetPositionalValue();
                data |= psi_disable_data_threshold_regulation_ch6.GetPositionalValue();
                data |= psi_disable_data_threshold_regulation_ch5.GetPositionalValue();
                data |= psi_disable_data_threshold_regulation_ch4.GetPositionalValue();
                data |= psi_disable_data_threshold_regulation_ch3.GetPositionalValue();
                data |= psi_disable_data_threshold_regulation_ch2.GetPositionalValue();
                data |= psi_disable_data_threshold_regulation_ch1.GetPositionalValue();
                data |= psi_supply_on_ch8.GetPositionalValue();
                data |= psi_supply_on_ch7.GetPositionalValue();
                data |= psi_supply_on_ch6.GetPositionalValue();
                data |= psi_supply_on_ch5.GetPositionalValue();
                data |= psi_supply_on_ch4.GetPositionalValue();
                data |= psi_supply_on_ch3.GetPositionalValue();
                data |= psi_supply_on_ch2.GetPositionalValue();
                data |= psi_supply_on_ch1.GetPositionalValue();
                
                return data;
            }
            set
            {
                psi_disable_data_threshold_regulation_ch8.UpdateValue(value);
                psi_disable_data_threshold_regulation_ch7.UpdateValue(value);
                psi_disable_data_threshold_regulation_ch6.UpdateValue(value);
                psi_disable_data_threshold_regulation_ch5.UpdateValue(value);
                psi_disable_data_threshold_regulation_ch4.UpdateValue(value);
                psi_disable_data_threshold_regulation_ch3.UpdateValue(value);
                psi_disable_data_threshold_regulation_ch2.UpdateValue(value);
                psi_disable_data_threshold_regulation_ch1.UpdateValue(value);
                psi_supply_on_ch8.UpdateValue(value);
                psi_supply_on_ch7.UpdateValue(value);
                psi_supply_on_ch6.UpdateValue(value);
                psi_supply_on_ch5.UpdateValue(value);
                psi_supply_on_ch4.UpdateValue(value);
                psi_supply_on_ch3.UpdateValue(value);
                psi_supply_on_ch2.UpdateValue(value);
                psi_supply_on_ch1.UpdateValue(value);
                
            }
        }

        #region Field classes declarations
        /// <summary>
        /// Class for working with psi_disable_data_threshold_regulation_ch8 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_disable_data_threshold_regulation_ch8
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
            private const UInt16 bitOffset = 15;

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
            public string Name { get; private set; } = "psi_disable_data_threshold_regulation_ch8";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "Disable data threshold regulation (Debug Option for EMC)";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_disable_data_threshold_regulation_ch7 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_disable_data_threshold_regulation_ch7
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
            private const UInt16 bitOffset = 14;

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
            public string Name { get; private set; } = "psi_disable_data_threshold_regulation_ch7";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "Disable data threshold regulation (Debug Option for EMC)";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_disable_data_threshold_regulation_ch6 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_disable_data_threshold_regulation_ch6
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
            private const UInt16 bitOffset = 13;

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
            public string Name { get; private set; } = "psi_disable_data_threshold_regulation_ch6";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "Disable data threshold regulation (Debug Option for EMC)";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_disable_data_threshold_regulation_ch5 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_disable_data_threshold_regulation_ch5
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
            private const UInt16 bitOffset = 12;

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
            public string Name { get; private set; } = "psi_disable_data_threshold_regulation_ch5";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "Disable data threshold regulation (Debug Option for EMC)";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_disable_data_threshold_regulation_ch4 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_disable_data_threshold_regulation_ch4
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
            private const UInt16 bitOffset = 11;

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
            public string Name { get; private set; } = "psi_disable_data_threshold_regulation_ch4";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "Disable data threshold regulation (Debug Option for EMC)";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_disable_data_threshold_regulation_ch3 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_disable_data_threshold_regulation_ch3
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
            private const UInt16 bitOffset = 10;

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
            public string Name { get; private set; } = "psi_disable_data_threshold_regulation_ch3";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "Disable data threshold regulation (Debug Option for EMC)";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_disable_data_threshold_regulation_ch2 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_disable_data_threshold_regulation_ch2
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
            private const UInt16 bitOffset = 9;

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
            public string Name { get; private set; } = "psi_disable_data_threshold_regulation_ch2";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "Disable data threshold regulation (Debug Option for EMC)";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_disable_data_threshold_regulation_ch1 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_disable_data_threshold_regulation_ch1
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
            private const UInt16 bitOffset = 8;

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
            public string Name { get; private set; } = "psi_disable_data_threshold_regulation_ch1";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "Disable data threshold regulation (Debug Option for EMC)";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_supply_on_ch8 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_supply_on_ch8
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
            private const UInt16 bitOffset = 7;

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
            public string Name { get; private set; } = "psi_supply_on_ch8";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "switch psi supply for selected channels, Bit 0 is mapped to channel 1";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_supply_on_ch7 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_supply_on_ch7
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
            private const UInt16 bitOffset = 6;

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
            public string Name { get; private set; } = "psi_supply_on_ch7";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "switch psi supply for selected channels, Bit 0 is mapped to channel 1";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_supply_on_ch6 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_supply_on_ch6
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
            private const UInt16 bitOffset = 5;

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
            public string Name { get; private set; } = "psi_supply_on_ch6";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "switch psi supply for selected channels, Bit 0 is mapped to channel 1";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_supply_on_ch5 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_supply_on_ch5
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
            private const UInt16 bitOffset = 4;

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
            public string Name { get; private set; } = "psi_supply_on_ch5";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "switch psi supply for selected channels, Bit 0 is mapped to channel 1";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_supply_on_ch4 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_supply_on_ch4
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
            public string Name { get; private set; } = "psi_supply_on_ch4";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "switch psi supply for selected channels, Bit 0 is mapped to channel 1";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_supply_on_ch3 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_supply_on_ch3
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
            public string Name { get; private set; } = "psi_supply_on_ch3";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "switch psi supply for selected channels, Bit 0 is mapped to channel 1";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_supply_on_ch2 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_supply_on_ch2
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
            public string Name { get; private set; } = "psi_supply_on_ch2";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "switch psi supply for selected channels, Bit 0 is mapped to channel 1";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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
        /// Class for working with psi_supply_on_ch1 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_psi_supply_on_ch1
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
            public string Name { get; private set; } = "psi_supply_on_ch1";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "switch psi supply for selected channels, Bit 0 is mapped to channel 1";

            /// <summary>
            /// Access level of this field
            /// </summary>
            public string Access { get; private set; } = "read-write";

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