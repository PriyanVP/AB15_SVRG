# tests CRC16 calculation for AB15 config

import os
import pytest
import serial
from time import sleep
from serial_fixtures import SerialWrapper
import package_helper as pkg
import crc_helper as crc

class TestCRC16:
    '''CRC16 tests'''

    # Delay for recieving MCU response
    DELAY = 0.1 # TODO: verify that duration is sufficient for all testcases

    # Test values
    registers2 = [0x0000, 0x1234, 0x2345, 0x89ff, 0xabfc, 0x0000]

    # Config values array from GUI files
    cfgValuesGUI = [0x8006, 0x8006, 0x8006, 0x8006, 0x00F0, 0x0000, 0x0003, 0x0003, 0x0003, 0x0003, 0x0003, 0x0003, 0x0003, 0x0003, 0x0003, 0x0003, 0x0007, 0x0001, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0002, 0x2102, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x7001, 0x7001, 0x7001, 0x7001, 0x7001, 0x7001, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0001, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0FFF, 0x0000, 0x0101, 0x0101, 0x0101, 0x0101, 0x0101, 0x0101, 0x0101, 0x0101, 0x0101, 0x0101, 0x0808, 0x0404, 0x0404, 0x0404, 0x0101, 0x0404, 0x0404, 0x0404, 0x0404, 0x0404]
    
    # Config values array from APbuttler example
    cfgValuesAPbuttler = [0x7, 0x8006, 0x8006, 0x8006, 0x8006, 0x00f0, 0x0, 0x3, 0x3, 0x3, 0x3, 0x3, 0x3, 0x3, 0x3, 0x3, 0x3, 0x0, 0x7, 0x1, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x2, 0x2102, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x7001, 0x7001, 0x7001, 0x7001, 0x7001, 0x7001, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x1, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0xFFF , 0x0, 0x101 , 0x101 , 0x101 , 0x101 , 0x101 , 0x101 , 0x101 , 0x101 , 0x101 , 0x101 , 0x808 , 0x404 , 0x404 , 0x404 , 0x1010, 0x404 , 0x404 , 0x404 , 0x404 , 0x404 , 0x00da]

    @classmethod
    def setup_class(cls):
        # COM port handling; connects to COM port with SW TC375 by default once before executing tests in class
        #cls.serial = SerialWrapper()
        #cls.serial.open_port()
        pass

    @classmethod 
    def teardown_class(cls):
        # disconnects after test's completion
        #cls.serial.close_port()
        pass

    def test_crc16Test(self):
        '''Tests CRC16 calculation for test value'''
        # Arrange

        # Act
        crcValueTest = crc.crc16(self.registers2, 0, len(self.registers2))
        # Output to be captured if test passes
        print(f"CRC16 test Result: Actual 0x{crcValueTest:04X}, Expected 0xA113")

        # Assert
        assert crcValueTest == 0xA113, "CRC16 calculation of test values gives different value than expected from example"


    def test_crc16Cfg(self):
        '''Tests CRC16 calculation for AB15 config'''
        # Arrange

        # Act
        crcValue = crc.crc16(self.cfgValuesGUI, 0, len(self.cfgValuesGUI))
        # Output to be captured if test passes
        print(f"CRC16 config Result: Actual 0x{crcValue:04X}, Expected 0x2F0A")
        print(f"Number of cfg values fed into crc16: {len(self.cfgValuesGUI)}")

        # Assert
        assert crcValue == 0x2F0A, "CRC16 calculation of cfg values gives different value than expected from APbuttler log"

    def test_crc16APbuttler(self):
        '''Tests CRC16 calculation for AB15 config'''
        # Arrange

        # Act
        crcValue = crc.crc16(self.cfgValuesAPbuttler, 0, len(self.cfgValuesAPbuttler))
        # Output to be captured if test passes
        print(f"CRC16 config Result: Actual 0x{crcValue:04X}, Expected 0x2F0A")
        print(f"Number of cfg values fed into crc16: {len(self.cfgValuesAPbuttler)}")

        # Assert
        assert crcValue == 0x2F0A, "CRC16 calculation of cfg values gives different value than expected from APbuttler log"




# from DummyConfiguration.cs
'''
        private void InitListWithDummyValues()
        {
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

            // Write configuration for UART registers
            ConfigData.Add(new Reg_Uart_Sp_Config() { Data = 0x0007 });

            // Write configuration for PSI registers
            ConfigData.Add(new Reg_PSI_SID_Slot1_Ch1() { Data = 0x0001 });
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
            ConfigData.Add(new Reg_PSI_Config_Common() { Data = 0x0002 });
            ConfigData.Add(new Reg_PSI_Config_Ch1() { Data = 0x2102 });
            ConfigData.Add(new Reg_PSI_Config_Ch2() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_Config_Ch3() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_Config_Ch4() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_Config_Ch5() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_Config_Ch6() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_Config_Ch7() { Data = 0x0000 });
            ConfigData.Add(new Reg_PSI_Config_Ch8() { Data = 0x0000 });

            // Write configuration for POM registers
            ConfigData.Add(new Reg_POM_Config() { Data = 0x0000 });

            // Write configuration for Safing registers
            ConfigData.Add(new Reg_SMON_Config_Ch0() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch1() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch2() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch3() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch4() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch5() { Data = 0x7001 });
            ConfigData.Add(new Reg_SMON_Config_Ch6() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch7() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch8() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch9() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch10() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch11() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch12() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch13() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch14() { Data = 0x0000 });
            ConfigData.Add(new Reg_SMON_Config_Ch15() { Data = 0x0000 });
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
            ConfigData.Add(new Reg_FLM_Config_ch2_1() { Data = 0x0808 });
            ConfigData.Add(new Reg_FLM_Config_ch4_3() { Data = 0x0404 });
            ConfigData.Add(new Reg_FLM_Config_ch6_5() { Data = 0x0404 });
            ConfigData.Add(new Reg_FLM_Config_ch8_7() { Data = 0x0404 });
            ConfigData.Add(new Reg_FLM_Config_ch10_9() { Data = 0x01010 });
            ConfigData.Add(new Reg_FLM_Config_ch12_11() { Data = 0x0404 });
            ConfigData.Add(new Reg_FLM_Config_ch14_13() { Data = 0x0404 });
            ConfigData.Add(new Reg_FLM_Config_ch16_15() { Data = 0x0404 });
            ConfigData.Add(new Reg_FLM_Config_ch18_17() { Data = 0x0404 });
            ConfigData.Add(new Reg_FLM_Config_ch20_19() { Data = 0x0404 });
'''

# from DummyConfiguration.cs, formatted
'''
    Data = 0x8006
    Data = 0x8006
    Data = 0x8006
    Data = 0x8006
    Data = 0x00F0
    Data = 0x0000
    Data = 0x0003
    Data = 0x0003
    Data = 0x0003
    Data = 0x0003
    Data = 0x0003
    Data = 0x0003
    Data = 0x0003
    Data = 0x0003
    Data = 0x0003
    Data = 0x0003
    Data = 0x0007
    Data = 0x0001
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0002
    Data = 0x2102
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x7001
    Data = 0x7001
    Data = 0x7001
    Data = 0x7001
    Data = 0x7001
    Data = 0x7001
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0001
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0000
    Data = 0x0FFF
    Data = 0x0000
    Data = 0x0101
    Data = 0x0101
    Data = 0x0101
    Data = 0x0101
    Data = 0x0101
    Data = 0x0101
    Data = 0x0101
    Data = 0x0101
    Data = 0x0101
    Data = 0x0101
    Data = 0x0808
    Data = 0x0404
    Data = 0x0404
    Data = 0x0404
    Data = 0x0101
    Data = 0x0404
    Data = 0x0404
    Data = 0x0404
    Data = 0x0404
    Data = 0x0404
'''


# from APbuttler, formatted
'''
    Data: 0x7
    Data: 0x8006
    Data: 0x8006
    Data: 0x8006
    Data: 0x8006
    Data: 0x00f0
    Data: 0x0
    Data: 0x3
    Data: 0x3
    Data: 0x3
    Data: 0x3
    Data: 0x3
    Data: 0x3
    Data: 0x3
    Data: 0x3
    Data: 0x3
    Data: 0x3
    Data: 0x0
    Data: 0x7
    Data: 0x1
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x2
    Data: 0x2102
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x7001
    Data: 0x7001
    Data: 0x7001
    Data: 0x7001
    Data: 0x7001
    Data: 0x7001
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x1
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0x0
    Data: 0xFFF 
    Data: 0x0
    Data: 0x101 
    Data: 0x101 
    Data: 0x101 
    Data: 0x101 
    Data: 0x101 
    Data: 0x101 
    Data: 0x101 
    Data: 0x101 
    Data: 0x101 
    Data: 0x101 
    Data: 0x808 
    Data: 0x404 
    Data: 0x404 
    Data: 0x404 
    Data: 0x1010
    Data: 0x404 
    Data: 0x404 
    Data: 0x404 
    Data: 0x404 
    Data: 0x404 
    Data: 0x00da
'''