using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models.Generated.Registers
{
    /// <summary>
    /// Class for working with FLM_Diag_Start register
    /// WARNING: this class is generated, do not modify manually!
    /// </summary>
    public class Reg_FLM_Diag_Start : IRegister
    {
        /// <summary>
        /// Reset value for register
        /// </summary>
        public UInt16 ResetValue { get; private set; } = 0x0;

        /// <summary>
        /// Name of the register
        /// </summary>
        public string Name { get; private set; } = "FLM_Diag_Start";

        /// <summary>
        /// Absolute address of the register
        /// </summary>
        public UInt16 Address { get; private set; } = 0x01ac;

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; } = "FLM Diagnose start";

        /// <summary>
        /// Access level of this register
        /// </summary>
        public string Access { get; private set; } = "read-write";

        #region Fields instances
        /// <summary>
        /// Defines the channel the diagnosis is executed. Only used for modes flagged by 'single channel'
        /// Bitwidth: 5
        /// </summary>
        public Field_flm_diag_channel flm_diag_channel { get; set; } = new Field_flm_diag_channel();
        
        /// <summary>
        /// FLM diag mode. Defines the diagnosis mode to start
        /// Bitwidth: 5
        /// </summary>
        public Field_flm_diag_mode flm_diag_mode { get; set; } = new Field_flm_diag_mode();
        
        /// <summary>
        /// FLM diag start bit. Has to be set 1 to start selected Diagnosis. If flm_diag_mode = off, then diagnosis is stopped. Bit is automatically cleared.
        /// Bitwidth: 1
        /// </summary>
        public Field_flm_diag_start flm_diag_start { get; set; } = new Field_flm_diag_start();
        #endregion // Fields instances

        /// <summary>
        /// Data property. Constructs register value from fields on get. Deconstruct register value by fields on set
        /// </summary>
        public UInt16 Data
        {
            get
            {
                UInt16 data = 0x0;

                data |= flm_diag_channel.GetPositionalValue();
                data |= flm_diag_mode.GetPositionalValue();
                data |= flm_diag_start.GetPositionalValue();
                
                return data;
            }
            set
            {
                flm_diag_channel.UpdateValue(value);
                flm_diag_mode.UpdateValue(value);
                flm_diag_start.UpdateValue(value);
                
            }
        }

        #region Field classes declarations
        /// <summary>
        /// Class for working with flm_diag_channel field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_diag_channel
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
            private const UInt16 bitWidth = 5;

            /// <summary>
            /// Data stored in register
            /// </summary>
            private UInt16 data;

            /// <summary>
            /// Name of the field
            /// </summary>
            public string Name { get; private set; } = "flm_diag_channel";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "Defines the channel the diagnosis is executed. Only used for modes flagged by 'single channel'";

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
                
                { "channel_off", 0} ,
                
                { "channel1", 1} ,
                
                { "channel2", 2} ,
                
                { "channel3", 3} ,
                
                { "channel4", 4} ,
                
                { "channel5", 5} ,
                
                { "channel6", 6} ,
                
                { "channel7", 7} ,
                
                { "channel8", 8} ,
                
                { "channel9", 9} ,
                
                { "channel10", 10} ,
                
                { "channel11", 11} ,
                
                { "channel12", 12} ,
                
                { "channel13", 13} ,
                
                { "channel14", 14} ,
                
                { "channel15", 15} ,
                
                { "channel16", 16} ,
                
                { "channel17", 17} ,
                
                { "channel18", 18} ,
                
                { "channel19", 19} ,
                
                { "channel20", 20} 
                
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
        /// Class for working with flm_diag_mode field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_diag_mode
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
            public string Name { get; private set; } = "flm_diag_mode";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "FLM diag mode. Defines the diagnosis mode to start";

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
                
                { "stop", 0} ,
                
                { "igx_cap_all", 1} ,
                
                { "squib_res_single", 2} ,
                
                { "squib_res_all", 3} ,
                
                { "squib_pres_single", 4} ,
                
                { "squib_pres_all", 5} ,
                
                { "lea_diode_single", 6} ,
                
                { "lea_diode_all", 7} ,
                
                { "vh_mease_single", 8} ,
                
                { "vh_mease_all", 9} ,
                
                { "cross_couple_master", 10} ,
                
                { "cross_couple_slave", 11} ,
                
                { "highside_powerstage", 12} ,
                
                { "Lowside_powerstage", 13} ,
                
                { "svrg_test", 14} 
                
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
        /// Class for working with flm_diag_start field
        /// WARNING: this class is generated, do not modify manually!
        /// </summary>
        public sealed class Field_flm_diag_start
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
            public string Name { get; private set; } = "flm_diag_start";

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; private set; } = "FLM diag start bit. Has to be set 1 to start selected Diagnosis. If flm_diag_mode = off, then diagnosis is stopped. Bit is automatically cleared.";

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