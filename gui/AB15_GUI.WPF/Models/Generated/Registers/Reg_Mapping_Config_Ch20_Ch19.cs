using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models.Generated.Registers
{
    /// <summary>
    /// Class for working with Mapping_Config_Ch20_Ch19 register
    /// WARNING: this class is generated, do not modify manually!
    /// </summary>
    public class Reg_Mapping_Config_Ch20_Ch19 : IRegister
    {
        /// <summary>
        /// Reset value for register
        /// </summary>
        public UInt16 ResetValue { get; private set; } = 0x0;

        /// <summary>
        /// Name of the register
        /// </summary>
        public string Name { get; private set; } = "Mapping_Config_Ch20_Ch19";

        /// <summary>
        /// Absolute address of the register
        /// </summary>
        public UInt16 Address { get; private set; } = 0x0141;

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
        /// influence of dis_dbt0_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_ahp_relevant_loop_ch20 dismap_dis_ahp_relevant_loop_ch20 { get; set; } = new Field_dismap_dis_ahp_relevant_loop_ch20();
        
        /// <summary>
        /// influence of dis_dbt0_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_dbt0_relevant_loop_ch20 dismap_dis_dbt0_relevant_loop_ch20 { get; set; } = new Field_dismap_dis_dbt0_relevant_loop_ch20();
        
        /// <summary>
        /// influence of dis_s2_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_s2_relevant_loop_ch20 dismap_dis_s2_relevant_loop_ch20 { get; set; } = new Field_dismap_dis_s2_relevant_loop_ch20();
        
        /// <summary>
        /// influence of dis_s1_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_s1_relevant_loop_ch20 dismap_dis_s1_relevant_loop_ch20 { get; set; } = new Field_dismap_dis_s1_relevant_loop_ch20();
        
        /// <summary>
        /// influence of dis_w_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_w_relevant_loop_ch20 dismap_dis_w_relevant_loop_ch20 { get; set; } = new Field_dismap_dis_w_relevant_loop_ch20();
        
        /// <summary>
        /// influence of dis_y_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_y_relevant_loop_ch20 dismap_dis_y_relevant_loop_ch20 { get; set; } = new Field_dismap_dis_y_relevant_loop_ch20();
        
        /// <summary>
        /// influence of dis_x_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_x_relevant_loop_ch20 dismap_dis_x_relevant_loop_ch20 { get; set; } = new Field_dismap_dis_x_relevant_loop_ch20();
        
        /// <summary>
        /// parity bit covering dismap_dis_x_y_w_s_dbt0_relevant_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_parity_loop_ch20 dismap_dis_parity_loop_ch20 { get; set; } = new Field_dismap_dis_parity_loop_ch20();
        
        /// <summary>
        /// influence of dis_dbt0_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_ahp_relevant_loop_ch19 dismap_dis_ahp_relevant_loop_ch19 { get; set; } = new Field_dismap_dis_ahp_relevant_loop_ch19();
        
        /// <summary>
        /// influence of dis_dbt0_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_dbt0_relevant_loop_ch19 dismap_dis_dbt0_relevant_loop_ch19 { get; set; } = new Field_dismap_dis_dbt0_relevant_loop_ch19();
        
        /// <summary>
        /// influence of dis_s2_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_s2_relevant_loop_ch19 dismap_dis_s2_relevant_loop_ch19 { get; set; } = new Field_dismap_dis_s2_relevant_loop_ch19();
        
        /// <summary>
        /// influence of dis_s1_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_s1_relevant_loop_ch19 dismap_dis_s1_relevant_loop_ch19 { get; set; } = new Field_dismap_dis_s1_relevant_loop_ch19();
        
        /// <summary>
        /// influence of dis_w_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_w_relevant_loop_ch19 dismap_dis_w_relevant_loop_ch19 { get; set; } = new Field_dismap_dis_w_relevant_loop_ch19();
        
        /// <summary>
        /// influence of dis_y_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_y_relevant_loop_ch19 dismap_dis_y_relevant_loop_ch19 { get; set; } = new Field_dismap_dis_y_relevant_loop_ch19();
        
        /// <summary>
        /// influence of dis_x_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_x_relevant_loop_ch19 dismap_dis_x_relevant_loop_ch19 { get; set; } = new Field_dismap_dis_x_relevant_loop_ch19();
        
        /// <summary>
        /// parity bit covering dismap_dis_x_y_w_s_dbt0_relevant_loop on selected firing loop
        /// Bitwidth: 1
        /// </summary>
        public Field_dismap_dis_parity_loop_ch19 dismap_dis_parity_loop_ch19 { get; set; } = new Field_dismap_dis_parity_loop_ch19();
        #endregion // Fields instances

        /// <summary>
        /// Data property. Constructs register value from fields on get. Deconstruct register value by fields on set
        /// </summary>
        public UInt16 Data
        {
            get
            {
                UInt16 data = 0x0;

                data |= dismap_dis_ahp_relevant_loop_ch20.GetPositionalValue();
                data |= dismap_dis_dbt0_relevant_loop_ch20.GetPositionalValue();
                data |= dismap_dis_s2_relevant_loop_ch20.GetPositionalValue();
                data |= dismap_dis_s1_relevant_loop_ch20.GetPositionalValue();
                data |= dismap_dis_w_relevant_loop_ch20.GetPositionalValue();
                data |= dismap_dis_y_relevant_loop_ch20.GetPositionalValue();
                data |= dismap_dis_x_relevant_loop_ch20.GetPositionalValue();
                data |= dismap_dis_parity_loop_ch20.GetPositionalValue();
                data |= dismap_dis_ahp_relevant_loop_ch19.GetPositionalValue();
                data |= dismap_dis_dbt0_relevant_loop_ch19.GetPositionalValue();
                data |= dismap_dis_s2_relevant_loop_ch19.GetPositionalValue();
                data |= dismap_dis_s1_relevant_loop_ch19.GetPositionalValue();
                data |= dismap_dis_w_relevant_loop_ch19.GetPositionalValue();
                data |= dismap_dis_y_relevant_loop_ch19.GetPositionalValue();
                data |= dismap_dis_x_relevant_loop_ch19.GetPositionalValue();
                data |= dismap_dis_parity_loop_ch19.GetPositionalValue();
                
                return data;
            }
            set
            {
                dismap_dis_ahp_relevant_loop_ch20.UpdateValue(value);
                dismap_dis_dbt0_relevant_loop_ch20.UpdateValue(value);
                dismap_dis_s2_relevant_loop_ch20.UpdateValue(value);
                dismap_dis_s1_relevant_loop_ch20.UpdateValue(value);
                dismap_dis_w_relevant_loop_ch20.UpdateValue(value);
                dismap_dis_y_relevant_loop_ch20.UpdateValue(value);
                dismap_dis_x_relevant_loop_ch20.UpdateValue(value);
                dismap_dis_parity_loop_ch20.UpdateValue(value);
                dismap_dis_ahp_relevant_loop_ch19.UpdateValue(value);
                dismap_dis_dbt0_relevant_loop_ch19.UpdateValue(value);
                dismap_dis_s2_relevant_loop_ch19.UpdateValue(value);
                dismap_dis_s1_relevant_loop_ch19.UpdateValue(value);
                dismap_dis_w_relevant_loop_ch19.UpdateValue(value);
                dismap_dis_y_relevant_loop_ch19.UpdateValue(value);
                dismap_dis_x_relevant_loop_ch19.UpdateValue(value);
                dismap_dis_parity_loop_ch19.UpdateValue(value);
                
            }
        }

        #region Field classes declarations
        /// <summary>
        /// Class for working with dismap_dis_ahp_relevant_loop_ch20 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_ahp_relevant_loop_ch20
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
            public string Name { get; private set; } = "dismap_dis_ahp_relevant_loop_ch20";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_dbt0_loop on selected firing loop";

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
        /// Class for working with dismap_dis_dbt0_relevant_loop_ch20 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_dbt0_relevant_loop_ch20
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
            public string Name { get; private set; } = "dismap_dis_dbt0_relevant_loop_ch20";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_dbt0_loop on selected firing loop";

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
        /// Class for working with dismap_dis_s2_relevant_loop_ch20 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_s2_relevant_loop_ch20
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
            public string Name { get; private set; } = "dismap_dis_s2_relevant_loop_ch20";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_s2_loop on selected firing loop";

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
        /// Class for working with dismap_dis_s1_relevant_loop_ch20 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_s1_relevant_loop_ch20
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
            public string Name { get; private set; } = "dismap_dis_s1_relevant_loop_ch20";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_s1_loop on selected firing loop";

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
        /// Class for working with dismap_dis_w_relevant_loop_ch20 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_w_relevant_loop_ch20
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
            public string Name { get; private set; } = "dismap_dis_w_relevant_loop_ch20";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_w_loop on selected firing loop";

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
        /// Class for working with dismap_dis_y_relevant_loop_ch20 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_y_relevant_loop_ch20
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
            public string Name { get; private set; } = "dismap_dis_y_relevant_loop_ch20";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_y_loop on selected firing loop";

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
        /// Class for working with dismap_dis_x_relevant_loop_ch20 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_x_relevant_loop_ch20
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
            public string Name { get; private set; } = "dismap_dis_x_relevant_loop_ch20";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_x_loop on selected firing loop";

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
        /// Class for working with dismap_dis_parity_loop_ch20 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_parity_loop_ch20
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
            public string Name { get; private set; } = "dismap_dis_parity_loop_ch20";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "parity bit covering dismap_dis_x_y_w_s_dbt0_relevant_loop on selected firing loop";

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
        /// Class for working with dismap_dis_ahp_relevant_loop_ch19 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_ahp_relevant_loop_ch19
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
            public string Name { get; private set; } = "dismap_dis_ahp_relevant_loop_ch19";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_dbt0_loop on selected firing loop";

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
        /// Class for working with dismap_dis_dbt0_relevant_loop_ch19 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_dbt0_relevant_loop_ch19
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
            public string Name { get; private set; } = "dismap_dis_dbt0_relevant_loop_ch19";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_dbt0_loop on selected firing loop";

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
        /// Class for working with dismap_dis_s2_relevant_loop_ch19 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_s2_relevant_loop_ch19
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
            public string Name { get; private set; } = "dismap_dis_s2_relevant_loop_ch19";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_s2_loop on selected firing loop";

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
        /// Class for working with dismap_dis_s1_relevant_loop_ch19 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_s1_relevant_loop_ch19
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
            public string Name { get; private set; } = "dismap_dis_s1_relevant_loop_ch19";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_s1_loop on selected firing loop";

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
        /// Class for working with dismap_dis_w_relevant_loop_ch19 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_w_relevant_loop_ch19
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
            public string Name { get; private set; } = "dismap_dis_w_relevant_loop_ch19";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_w_loop on selected firing loop";

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
        /// Class for working with dismap_dis_y_relevant_loop_ch19 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_y_relevant_loop_ch19
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
            public string Name { get; private set; } = "dismap_dis_y_relevant_loop_ch19";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_y_loop on selected firing loop";

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
        /// Class for working with dismap_dis_x_relevant_loop_ch19 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_x_relevant_loop_ch19
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
            public string Name { get; private set; } = "dismap_dis_x_relevant_loop_ch19";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "influence of dis_x_loop on selected firing loop";

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
        /// Class for working with dismap_dis_parity_loop_ch19 field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_dismap_dis_parity_loop_ch19
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
            public string Name { get; private set; } = "dismap_dis_parity_loop_ch19";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "parity bit covering dismap_dis_x_y_w_s_dbt0_relevant_loop on selected firing loop";

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