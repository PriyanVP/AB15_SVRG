using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models.Generated.Registers
{
    /// <summary>
    /// Class for working with FLM_Unlock register
    /// WARNING: this class is generated, do not modify manually!
    /// </summary>
    public class Reg_FLM_Unlock : IRegister
    {
        /// <summary>
        /// Reset value for register
        /// </summary>
        public UInt16 ResetValue { get; private set; } = 0x0;

        /// <summary>
        /// Name of the register
        /// </summary>
        public string Name { get; private set; } = "FLM_Unlock";

        /// <summary>
        /// Absolute address of the register
        /// </summary>
        public UInt16 Address { get; private set; } = 0x018a;

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; } = "Unlock";

        /// <summary>
        /// Access level of this register
        /// </summary>
        public string Access { get; private set; } = "read-write";

        #region Fields instances
        /// <summary>
        /// Safety Code for Unlock on. Invalid Code will prevent Write Action 
        /// Bitwidth: 2
        /// </summary>
        public Field_flm_code_unlock flm_code_unlock { get; set; } = new Field_flm_code_unlock();
        
        /// <summary>
        /// Clear firing current counter. Only effective if all loops are locked (flm_unlock_ls_module(n) = 0 and flm_unlock_hs_module(n) = 0)
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_clear_fire_cnt flm_clear_fire_cnt { get; set; } = new Field_flm_clear_fire_cnt();
        
        /// <summary>
        /// activates current sink at VH2. Needs to be enabled before activation of external FET supplying VH2
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_vh2_pd_en flm_vh2_pd_en { get; set; } = new Field_flm_vh2_pd_en();
        
        /// <summary>
        /// activates current sink at VH1a. Needs to be enabled before activation of external FET supplying VH1a
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_vh1a_pd_en flm_vh1a_pd_en { get; set; } = new Field_flm_vh1a_pd_en();
        
        /// <summary>
        /// for Fire Modes 6, 6a, 7 or 7a: enable alternative Mode
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_fire_mode_sel flm_fire_mode_sel { get; set; } = new Field_flm_fire_mode_sel();
        
        /// <summary>
        /// unlock highsides 20 19 18 17
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_unlock_hs_module5 flm_unlock_hs_module5 { get; set; } = new Field_flm_unlock_hs_module5();
        
        /// <summary>
        /// unlock lowsides 20 19 18 17
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_unlock_ls_module5 flm_unlock_ls_module5 { get; set; } = new Field_flm_unlock_ls_module5();
        
        /// <summary>
        /// unlock highsides 16 15 14 13
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_unlock_hs_module4 flm_unlock_hs_module4 { get; set; } = new Field_flm_unlock_hs_module4();
        
        /// <summary>
        /// unlock lowsides 16 15 14 13
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_unlock_ls_module4 flm_unlock_ls_module4 { get; set; } = new Field_flm_unlock_ls_module4();
        
        /// <summary>
        /// unlock highsides 12 11 10 9
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_unlock_hs_module3 flm_unlock_hs_module3 { get; set; } = new Field_flm_unlock_hs_module3();
        
        /// <summary>
        /// unlock lowsides 12 11 10 9
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_unlock_ls_module3 flm_unlock_ls_module3 { get; set; } = new Field_flm_unlock_ls_module3();
        
        /// <summary>
        /// unlock highsides 8 7 6 5
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_unlock_hs_module2 flm_unlock_hs_module2 { get; set; } = new Field_flm_unlock_hs_module2();
        
        /// <summary>
        /// unlock lowsides 8 7 6 5
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_unlock_ls_module2 flm_unlock_ls_module2 { get; set; } = new Field_flm_unlock_ls_module2();
        
        /// <summary>
        /// unlock highsides 4 3 2 1
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_unlock_hs_module1 flm_unlock_hs_module1 { get; set; } = new Field_flm_unlock_hs_module1();
        
        /// <summary>
        /// unlock lowsides 4 3 2 1
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_unlock_ls_module1 flm_unlock_ls_module1 { get; set; } = new Field_flm_unlock_ls_module1();
        #endregion // Fields instances

        /// <summary>
        /// Data property. Constructs register value from fields on get. Deconstruct register value by fields on set
        /// </summary>
        public UInt16 Data
        {
            get
            {
                UInt16 data = 0x0;

                data |= flm_code_unlock.GetPositionalValue();
                data |= flm_clear_fire_cnt.GetPositionalValue();
                data |= flm_vh2_pd_en.GetPositionalValue();
                data |= flm_vh1a_pd_en.GetPositionalValue();
                data |= flm_fire_mode_sel.GetPositionalValue();
                data |= flm_unlock_hs_module5.GetPositionalValue();
                data |= flm_unlock_ls_module5.GetPositionalValue();
                data |= flm_unlock_hs_module4.GetPositionalValue();
                data |= flm_unlock_ls_module4.GetPositionalValue();
                data |= flm_unlock_hs_module3.GetPositionalValue();
                data |= flm_unlock_ls_module3.GetPositionalValue();
                data |= flm_unlock_hs_module2.GetPositionalValue();
                data |= flm_unlock_ls_module2.GetPositionalValue();
                data |= flm_unlock_hs_module1.GetPositionalValue();
                data |= flm_unlock_ls_module1.GetPositionalValue();
                
                return data;
            }
            set
            {
                flm_code_unlock.UpdateValue(value);
                flm_clear_fire_cnt.UpdateValue(value);
                flm_vh2_pd_en.UpdateValue(value);
                flm_vh1a_pd_en.UpdateValue(value);
                flm_fire_mode_sel.UpdateValue(value);
                flm_unlock_hs_module5.UpdateValue(value);
                flm_unlock_ls_module5.UpdateValue(value);
                flm_unlock_hs_module4.UpdateValue(value);
                flm_unlock_ls_module4.UpdateValue(value);
                flm_unlock_hs_module3.UpdateValue(value);
                flm_unlock_ls_module3.UpdateValue(value);
                flm_unlock_hs_module2.UpdateValue(value);
                flm_unlock_ls_module2.UpdateValue(value);
                flm_unlock_hs_module1.UpdateValue(value);
                flm_unlock_ls_module1.UpdateValue(value);
                
            }
        }

        #region Field classes declarations
        /// <summary>
        /// Class for working with flm_code_unlock field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_code_unlock
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
            private const UInt16 bitWidth = 2;

            /// <summary>
            /// Data stored in register
            /// </summary>
            private UInt16 data;

            /// <summary>
            /// Name of the field
            /// </summary>
            public string Name { get; private set; } = "flm_code_unlock";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "Safety Code for Unlock on. Invalid Code will prevent Write Action ";

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
                
                { "val0", 0} 
                
            };
            

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
        /// Class for working with flm_clear_fire_cnt field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_clear_fire_cnt
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
            public string Name { get; private set; } = "flm_clear_fire_cnt";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "Clear firing current counter. Only effective if all loops are locked (flm_unlock_ls_module(n) = 0 and flm_unlock_hs_module(n) = 0)";

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
        /// Class for working with flm_vh2_pd_en field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_vh2_pd_en
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
            public string Name { get; private set; } = "flm_vh2_pd_en";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "activates current sink at VH2. Needs to be enabled before activation of external FET supplying VH2";

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
        /// Class for working with flm_vh1a_pd_en field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_vh1a_pd_en
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
            public string Name { get; private set; } = "flm_vh1a_pd_en";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "activates current sink at VH1a. Needs to be enabled before activation of external FET supplying VH1a";

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
        /// Class for working with flm_fire_mode_sel field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_fire_mode_sel
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
            public string Name { get; private set; } = "flm_fire_mode_sel";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "for Fire Modes 6, 6a, 7 or 7a: enable alternative Mode";

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
        /// Class for working with flm_unlock_hs_module5 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_unlock_hs_module5
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
            public string Name { get; private set; } = "flm_unlock_hs_module5";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "unlock highsides 20 19 18 17";

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
        /// Class for working with flm_unlock_ls_module5 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_unlock_ls_module5
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
            public string Name { get; private set; } = "flm_unlock_ls_module5";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "unlock lowsides 20 19 18 17";

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
        /// Class for working with flm_unlock_hs_module4 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_unlock_hs_module4
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
            public string Name { get; private set; } = "flm_unlock_hs_module4";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "unlock highsides 16 15 14 13";

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
        /// Class for working with flm_unlock_ls_module4 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_unlock_ls_module4
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
            public string Name { get; private set; } = "flm_unlock_ls_module4";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "unlock lowsides 16 15 14 13";

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
        /// Class for working with flm_unlock_hs_module3 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_unlock_hs_module3
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
            public string Name { get; private set; } = "flm_unlock_hs_module3";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "unlock highsides 12 11 10 9";

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
        /// Class for working with flm_unlock_ls_module3 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_unlock_ls_module3
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
            public string Name { get; private set; } = "flm_unlock_ls_module3";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "unlock lowsides 12 11 10 9";

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
        /// Class for working with flm_unlock_hs_module2 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_unlock_hs_module2
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
            public string Name { get; private set; } = "flm_unlock_hs_module2";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "unlock highsides 8 7 6 5";

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
        /// Class for working with flm_unlock_ls_module2 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_unlock_ls_module2
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
            public string Name { get; private set; } = "flm_unlock_ls_module2";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "unlock lowsides 8 7 6 5";

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
        /// Class for working with flm_unlock_hs_module1 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_unlock_hs_module1
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
            public string Name { get; private set; } = "flm_unlock_hs_module1";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "unlock highsides 4 3 2 1";

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
        /// Class for working with flm_unlock_ls_module1 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_unlock_ls_module1
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
            public string Name { get; private set; } = "flm_unlock_ls_module1";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "unlock lowsides 4 3 2 1";

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