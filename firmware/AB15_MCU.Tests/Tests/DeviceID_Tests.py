# group basic tests
# tests TODO mcu commands

import os
import pytest
import serial
import time
from fixtures.serial_fixtures import SerialWrapper

# Change working directory to script directory TODO: investigate why it doesn't work as expected
#os.chdir(os.path.dirname(__file__))

# MCU characteristics
VENDOR_ID = 0x058B # Infineon DAS JDS - vendor ID
PRODUCT_ID = 0x0043 # Infineon DAS JDS - product_ID

# MCU firmware version TODO: find a vay to keep up-to-date automatically
VERSION_MAJOR = 0x00
VERSION_MINOR = 0x02
VERSION_PATCH = 0x00
# AB12 IC device ID
DEVICE_ID_AB12 = 0xC4

# USB commands tested, from AB_protocol_description.xlsx, corresponds to CmdGetMcuVersion()
USB_CMD_GET_MCU_VERSION = 0xAB0000120028BA
USB_CMD_READ_DEV_ID = 0xAB0F010300D3BA

@pytest.fixture() 
# COM port handling; connects to COM14 by default before each test, 
# disconnects after test's completion
def serial():
    # setup
    serial = SerialWrapper()
    serial.OpenSerialPort()
    yield serial
    # teardown
    serial.CloseSerialPort()

@pytest.mark.basic
@pytest.mark.serial
def test_COMport(serial):
    '''group `basic` tests
    tests:
    - ShieldBuddy's serial connection to PC'''
    
    assert serial.com_port.is_open == True
    
    # Output to be captured if test passes
    print("ShieldBuddy is connected sucesfully at port", serial.com_port.name)

@pytest.mark.serial
#@pytest.mark.basic
def test_MCUVersion(serial):
    '''group basic tests
    verifies ShieldBuddy's firmware version
    tests:
    - firmware version;
    - MCU command:
        * USB_CMD_GET_MCU_BUILD_VERSION'''

    user_cmd = USB_CMD_GET_MCU_VERSION
    serial.com_port.write(int(user_cmd).to_bytes(7, 'big'))
    time.sleep(1)
    result = serial.com_port.read(10)
    
    assert (result[5] == VERSION_MAJOR) and (result[6] == VERSION_MINOR) and (result[7] == VERSION_PATCH)
    
    # Output to be captured if test passes
    print('Firmware version: ', hex(result[5]), hex(result[6]), hex(result[7])) # expected 0x0 0x2 0x0

@pytest.mark.serial
@pytest.mark.basic
@pytest.mark.skip(reason="Skipped due no HW connected (AB12/15 ASIC board)")
def test_DeviceID(serial):
    #Add documentation comments for functions of test files
    '''group basic tests
    verifies AB15(12) DeviceID
    tests:
    - MCU command:
        * USB_CMD_READ_DEV_ID'''

    user_cmd = USB_CMD_READ_DEV_ID
    serial.com_port.write(int(user_cmd).to_bytes(7, 'big'))
    time.sleep(1)
    result = serial.com_port.read(8)

    assert (result[5] == DEVICE_ID_AB12)
    
    # Output to be captured if test passes
    print(f'MCU response with IC device ID: ', end='') # expected 0xAB 0x8F 0x00 0x80 0x01 0xC4 0xBE 0xBA
    for itm in result:
        print(f'{itm:#03x} ', end='')