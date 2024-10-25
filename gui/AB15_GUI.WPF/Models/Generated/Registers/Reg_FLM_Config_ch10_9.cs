using System;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models.Generated.Registers
{
    /// <summary>
    /// Class for working with FLM_Config_ch10_9 register
    /// WARNING: this class is generated, do not modify manually!
    /// </summary>
    public class Reg_FLM_Config_ch10_9 : IRegister
    {
        /// <summary>
        /// Reset value for register
        /// </summary>
        public UInt16 ResetValue { get; private set; } = 0x101;

        /// <summary>
        /// Name of the register
        /// </summary>
        public string Name { get; private set; } = "FLM_Config_ch10_9";

        /// <summary>
        /// Absolute address of the register
        /// </summary>
        public UInt16 Address { get; private set; } = 0x0184;

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; } = "configuration for selected firing loop";

        /// <summary>
        /// Access level of this register
        /// </summary>
        public string Access { get; private set; } = "read-write";

        #region Fields instances
        /// <summary>
        /// firing mode for selected firing loop
        /// Bitwidth: 5
        /// </summary>
        public Field_flm_mode_ch10 flm_mode_ch10 { get; set; } = new Field_flm_mode_ch10();
        
        /// <summary>
        /// parity bit covering firing mode on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_mode_parity_ch10 flm_mode_parity_ch10 { get; set; } = new Field_flm_mode_parity_ch10();
        
        /// <summary>
        /// firing mode for selected firing loop
        /// Bitwidth: 5
        /// </summary>
        public Field_flm_mode_ch9 flm_mode_ch9 { get; set; } = new Field_flm_mode_ch9();
        
        /// <summary>
        /// parity bit covering firing mode on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_mode_parity_ch9 flm_mode_parity_ch9 { get; set; } = new Field_flm_mode_parity_ch9();
        #endregion // Fields instances

        /// <summary>
        /// Data property. Constructs register value from fields on get. Deconstruct register value by fields on set
        /// </summary>
        public UInt16 Data
        {
            get
            {
                UInt16 data = 0x0;

                data |= flm_mode_ch10.GetPositionalValue();
                data |= flm_mode_parity_ch10.GetPositionalValue();
                data |= flm_mode_ch9.GetPositionalValue();
                data |= flm_mode_parity_ch9.GetPositionalValue();
                
                return data;
            }
            set
            {
                flm_mode_ch10.UpdateValue(value);
                flm_mode_parity_ch10.UpdateValue(value);
                flm_mode_ch9.UpdateValue(value);
                flm_mode_parity_ch9.UpdateValue(value);
                
            }
        }

        #region Field classes declarations
        /// <summary>
        /// Class for working with flm_mode_ch10 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_mode_ch10
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
            private const UInt16 bitWidth = 5;

            /// <summary>
            /// Data stored in register
            /// </summary>
            private UInt16 data;

            /// <summary>
            /// Name of the field
            /// </summary>
            public string Name { get; private set; } = "flm_mode_ch10";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "firing mode for selected firing loop";

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
            }/// <summary>
            /// Enumerated values for this field
            /// </summary>
            public Dictionary<string, UInt16> EnumeratedValues { get; private set; } = new Dictionary<string, UInt16>
            {
                // TODO: last item without comma; check if {} work fine
                
                { "off", 0} ,
                
                { "Mode1", 2} ,
                
                { "Mode1a", 3} ,
                
                { "Mode2", 4} ,
                
                { "Mode2a", 5} ,
                
                { "Mode3", 6} ,
                
                { "Mode4", 8} ,
                
                { "Mode4a", 9} ,
                
                { "Mode5", 10} ,
                
                { "Mode5a", 11} ,
                
                { "Mode6", 12} ,
                
                { "Mode6a", 13} ,
                
                { "Mode7", 14} ,
                
                { "Mode7a", 15} ,
                
                { "Mode8", 16} ,
                
                { "Mode9", 17} 
                
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
        /// Class for working with flm_mode_parity_ch10 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_mode_parity_ch10
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
            public string Name { get; private set; } = "flm_mode_parity_ch10";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "parity bit covering firing mode on selected firing loop";

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
        /// Class for working with flm_mode_ch9 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_mode_ch9
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
            private const UInt16 bitWidth = 5;

            /// <summary>
            /// Data stored in register
            /// </summary>
            private UInt16 data;

            /// <summary>
            /// Name of the field
            /// </summary>
            public string Name { get; private set; } = "flm_mode_ch9";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "firing mode for selected firing loop";

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
            }/// <summary>
            /// Enumerated values for this field
            /// </summary>
            public Dictionary<string, UInt16> EnumeratedValues { get; private set; } = new Dictionary<string, UInt16>
            {
                // TODO: last item without comma; check if {} work fine
                
                { "off", 0} ,
                
                { "Mode1", 2} ,
                
                { "Mode1a", 3} ,
                
                { "Mode2", 4} ,
                
                { "Mode2a", 5} ,
                
                { "Mode3", 6} ,
                
                { "Mode4", 8} ,
                
                { "Mode4a", 9} ,
                
                { "Mode5", 10} ,
                
                { "Mode5a", 11} ,
                
                { "Mode6", 12} ,
                
                { "Mode6a", 13} ,
                
                { "Mode7", 14} ,
                
                { "Mode7a", 15} ,
                
                { "Mode8", 16} ,
                
                { "Mode9", 17} 
                
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
        /// Class for working with flm_mode_parity_ch9 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_mode_parity_ch9
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
            public string Name { get; private set; } = "flm_mode_parity_ch9";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "parity bit covering firing mode on selected firing loop";

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