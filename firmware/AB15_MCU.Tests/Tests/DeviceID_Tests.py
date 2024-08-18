# group basic tests
# tests TODO mcu commands

import serial
import time

#TestMCUFWVersion TODO implement for test example to run without ASIC (CmdGetMcuVersion() from general_cmd.c)

# MCU characteristics
VENDOR_ID = 0x058B # Infineon DAS JDS - vendor ID
PRODUCT_ID = 0x0043 # Infineon DAS JDS - product_ID

# MCU firmware version
VERSION_MAJOR = 0x00
VERSION_MINOR = 0x02
VERSION_PATCH = 0x00
# AB12 IC device ID
DEVICE_ID_AB12 = 0xC4

# USB commands tested, from AB_protocol_description.xlsx, corresponds to CmdGetMcuVersion()
USB_CMD_GET_MCU_VERSION = 0xAB0000120028BA
USB_CMD_READ_DEV_ID = 0xAB0F010300D3BA

# Protocol characteristics
baud_rate = 115200
data_size = 8
parity = None
stop_bit = 1
port_name = 'COM14' # hardcoded, can be different
com_port = serial.Serial()
com_port.baudrate = baud_rate
com_port.port = port_name
com_port.bytesize = 8
com_port.parity = 'N'
com_port.stopbits = 1
com_port.timeout = 3 # in seconds


def test_COMport():
    '''group basic tests
    tests:
    - ShieldBuddy connection'''

    # Open COM port
    com_port.open()
    assert com_port.is_open == True

    # Close com port
    com_port.close()


def test_MCUVersion():
    '''group basic tests
    verifies ShieldBuddy's firmware version
    tests:
    - firmware version;
    - MCU command:
        * USB_CMD_GET_MCU_BUILD_VERSION'''

    com_port.open()

    user_cmd = USB_CMD_GET_MCU_VERSION
    com_port.write(int(user_cmd).to_bytes(7, 'big'))
    time.sleep(1)

    result = com_port.read(10)
    print(f'Firmware version: ', end='') # expected 0xab 0x80 0x0 0x83 0x3 0x30 0x31 0x35 0x52 0xba TODO: update comment, version is 0.2.0.
    for itm in result:
        print(f'{itm:#03x} ', end='')
    assert (result[5] == VERSION_MAJOR) and (result[6] == VERSION_MINOR) and (result[7] == VERSION_PATCH)

    # Close com port
    com_port.close()


def test_DeviceID():
    #Add documentation comments for functions of test files
    '''group basic tests
    verifies AB15(12) DeviceID
    tests:
    - MCU command:
        * USB_CMD_READ_DEV_ID'''
    
    com_port.open()

    user_cmd = USB_CMD_READ_DEV_ID
    com_port.write(int(user_cmd).to_bytes(7, 'big'))
    time.sleep(1)

    result = com_port.read(8)
    print(f'MCU response with IC device ID: ', end='') # expected 0xAB 0x8F 0x00 0x80 0x01 0xC4 0xBE 0xBA
    for itm in result:
        print(f'{itm:#03x} ', end='')
    assert (result[5] == DEVICE_ID_AB12)

    com_port.close()