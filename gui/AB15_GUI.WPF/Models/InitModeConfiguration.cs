using System.Collections.Generic;
using AB15_GUI.WPF.Models.Generated.Registers;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class for storing init mode configuration
    /// </summary>
    public class InitModeConfiguration
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InitModeConfiguration(IASICWrapper asicWrapper)
        {
            // Populate list
            InitListWithDefaultValues();

            // Append configuration to ASIC centralized config
            asicWrapper.ASICs[0].AppendConfigRegisters(ConfigData);
        }

        // TODO: move configuration related functionality from ASIC class here

        /// <summary>
        /// Configuration data
        /// </summary>
        public List<IRegister> ConfigData { get; set; } = new List<IRegister>();

        /// <summary>
        /// Fill list with dummy values
        /// </summary>
        private void InitListWithDefaultValues()
        {
            // Write configuration for common registers
            ConfigData.Add(new Reg_Common_Config1() { Data = 0x0007 });
            ConfigData.Add(new Reg_SysStates_Reset_Locked_Config() { Data = 0x0019 }); // Note: due to ASIC specifics this register should be written AFTER CRC data

            // Write configuration for AIO registers
            ConfigData.Add(new Reg_AIO_Config_Ch1() { Data = 0x8006 });
            ConfigData.Add(new Reg_AIO_Config_Ch2() { Data = 0x8006 });
            ConfigData.Add(new Reg_AIO_Config_Ch3() { Data = 0x8006 });
            ConfigData.Add(new Reg_AIO_Config_Ch4() { Data = 0x8006 });
            ConfigData.Add(new Reg_AIO_COMMON_Config() { Data = 0x00F0 });

            // Write configuration for KACL registers
            ConfigData.Add(new Reg_K_ACL_Config() { Data = 0x0000 });

            // Write configuration for AIN registers
            ConfigData.Add(new Reg_AIN_Config_Auto_Ch1() { Data = 0x0003 });
            ConfigData.Add(new Reg_AIN_Config_Auto_Ch2() { Data = 0x0003 });
            ConfigData.Add(new Reg_AIN_Config_Auto_Ch3() { Data = 0x0003 });
            ConfigData.Add(new Reg_AIN_Config_Auto_Ch4() { Data = 0x0003 });
            ConfigData.Add(new Reg_AIN_Config_Auto_Ch5() { Data = 0x0003 });
            ConfigData.Add(new Reg_AIN_Config_Auto_Ch6() { Data = 0x0003 });
            ConfigData.Add(new Reg_AIN_Config_Auto_Ch7() { Data = 0x0003 });
            ConfigData.Add(new Reg_AIN_Config_Auto_Ch8() { Data = 0x0003 });
            ConfigData.Add(new Reg_AIN_Config_Auto_Ch9() { Data = 0x0003 });
            ConfigData.Add(new Reg_AIN_Config_Auto_Ch10() { Data = 0x0003 });
            ConfigData.Add(new Reg_AIN_CONFIG_UNLOCK() { Data = 0x0000 });

            // Write configuration for UART registers
            ConfigData.Add(new Reg_Uart_Sp_Config() { Data = 0x0007 });

            // Write configuration for PSI registers
            ConfigData.Add(new Reg_PSI_SID_Slot1_Ch1() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot2_Ch1() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot3_Ch1() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot4_Ch1() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot1_Ch2() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot2_Ch2() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot3_Ch2() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot4_Ch2() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot1_Ch3() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot2_Ch3() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot3_Ch3() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot4_Ch3() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot1_Ch4() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot2_Ch4() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot3_Ch4() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot4_Ch4() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot1_Ch5() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot2_Ch5() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot3_Ch5() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot4_Ch5() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot1_Ch6() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot2_Ch6() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot3_Ch6() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot4_Ch6() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot1_Ch7() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot2_Ch7() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot3_Ch7() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot4_Ch7() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot1_Ch8() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot2_Ch8() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot3_Ch8() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_SID_Slot4_Ch8() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_Config_Common() { Data = 0x0005 });
            ConfigData.Add(new Reg_PSI_Config_Ch1() { Data = 0x001A });
            ConfigData.Add(new Reg_PSI_Config_Ch2() { Data = 0x001A });
            ConfigData.Add(new Reg_PSI_Config_Ch3() { Data = 0x001A });
            ConfigData.Add(new Reg_PSI_Config_Ch4() { Data = 0x001A });
            ConfigData.Add(new Reg_PSI_Config_Ch5() { Data = 0x001A });
            ConfigData.Add(new Reg_PSI_Config_Ch6() { Data = 0x001A });
            ConfigData.Add(new Reg_PSI_Config_Ch7() { Data = 0x001A });
            ConfigData.Add(new Reg_PSI_Config_Ch8() { Data = 0x001A });

            // Write configuration for POM registers
            ConfigData.Add(new Reg_POM_Config() { Data = 0x0000 });

            // Write configuration for Safing registers
            ConfigData.Add(new Reg_SMON_Config_Ch0() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch1() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch2() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch3() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch4() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch5() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch6() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch7() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch8() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch9() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch10() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch11() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch12() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch13() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch14() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch15() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch16() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch17() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch18() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch19() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch20() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch21() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch22() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch23() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch24() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch25() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch26() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch27() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch28() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch29() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch30() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch31() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Filter_On_Ch15_0() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Filter_On_Ch31_16() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Shift_On_Ch15_0() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Shift_On_Ch31_16() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Long() { Data = 0x0001 });
            ConfigData.Add(new Reg_SWMON1_Config() { Data = 0x0000 });
            ConfigData.Add(new Reg_SWMON1_Config_Lower_Limit_B1() { Data = 0x0000 });
            ConfigData.Add(new Reg_SWMON1_Config_Upper_Limit_B1() { Data = 0x0000 });
            ConfigData.Add(new Reg_SWMON1_Config_Lower_Limit_B2() { Data = 0x0000 });
            ConfigData.Add(new Reg_SWMON1_Config_Upper_Limit_B2() { Data = 0x0000 });
            ConfigData.Add(new Reg_SWMON2_Config() { Data = 0x0000 });
            ConfigData.Add(new Reg_SWMON2_Config_Lower_Limit_B1() { Data = 0x0000 });
            ConfigData.Add(new Reg_SWMON2_Config_Upper_Limit_B1() { Data = 0x0000 });
            ConfigData.Add(new Reg_SWMON2_Config_Lower_Limit_B2() { Data = 0x0000 });
            ConfigData.Add(new Reg_SWMON2_Config_Upper_Limit_B2() { Data = 0x0000 });
            ConfigData.Add(new Reg_Disable_Logic_Config() { Data = 0x0FFF });
            ConfigData.Add(new Reg_Disable_Monoflops_Config() { Data = 0x0000 });
            ConfigData.Add(new Reg_Mapping_Config_Ch2_Ch1() { Data = 0x0101 });
            ConfigData.Add(new Reg_Mapping_Config_Ch4_Ch3() { Data = 0x0101 });
            ConfigData.Add(new Reg_Mapping_Config_Ch6_Ch5() { Data = 0x0101 });
            ConfigData.Add(new Reg_Mapping_Config_Ch8_Ch7() { Data = 0x0101 });
            ConfigData.Add(new Reg_Mapping_Config_Ch10_Ch9() { Data = 0x0101 });
            ConfigData.Add(new Reg_Mapping_Config_Ch12_Ch11() { Data = 0x0101 });
            ConfigData.Add(new Reg_Mapping_Config_Ch14_Ch13() { Data = 0x0101 });
            ConfigData.Add(new Reg_Mapping_Config_Ch16_Ch15() { Data = 0x0101 });
            ConfigData.Add(new Reg_Mapping_Config_Ch18_Ch17() { Data = 0x0101 });
            ConfigData.Add(new Reg_Mapping_Config_Ch20_Ch19() { Data = 0x0101 });

            // Write configuration for FLM registers
            ConfigData.Add(new Reg_FLM_Config_ch2_1() { Data = 0x0101 });
            ConfigData.Add(new Reg_FLM_Config_ch4_3() { Data = 0x0101 });
            ConfigData.Add(new Reg_FLM_Config_ch6_5() { Data = 0x0101 });
            ConfigData.Add(new Reg_FLM_Config_ch8_7() { Data = 0x0101 });
            ConfigData.Add(new Reg_FLM_Config_ch10_9() { Data = 0x01101 });
            ConfigData.Add(new Reg_FLM_Config_ch12_11() { Data = 0x0101 });
            ConfigData.Add(new Reg_FLM_Config_ch14_13() { Data = 0x0101 });
            ConfigData.Add(new Reg_FLM_Config_ch16_15() { Data = 0x0101 });
            ConfigData.Add(new Reg_FLM_Config_ch18_17() { Data = 0x0101 });
            ConfigData.Add(new Reg_FLM_Config_ch20_19() { Data = 0x0101 });

            // Write configuration for SVRG registers
            ConfigData.Add(new Reg_SVRG_Config() { Data = 0x00DA });
        }
    }
}
