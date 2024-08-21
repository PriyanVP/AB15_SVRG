# group basic tests
# tests TODO mcu commands

import os
import pytest
import serial
import time
from fixtures.serial_fixtures import *

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

@pytest.fixture() # COM port handling; connects to COM14 by default
def serial():
    serial = SerialWrapper()
    return serial


def test_COMport(serial):
    '''group basic tests
    tests:
    - ShieldBuddy connection'''
    
    serial.OpenSerialPort()
    assert serial.com_port.is_open == True
    serial.CloseSerialPort()


def test_MCUVersion(serial):
    '''group basic tests
    verifies ShieldBuddy's firmware version
    tests:
    - firmware version;
    - MCU command:
        * USB_CMD_GET_MCU_BUILD_VERSION'''

    serial.OpenSerialPort()

    user_cmd = USB_CMD_GET_MCU_VERSION
    serial.com_port.write(int(user_cmd).to_bytes(7, 'big'))
    time.sleep(1)

    result = serial.com_port.read(10)
    print(f'Firmware version: ', end='') # expected 0xab 0x80 0x0 0x83 0x3 0x0 0x2 0x0 0xfe 0xba
    for itm in result:
        print(f'{itm:#03x} ', end='')
    assert (result[5] == VERSION_MAJOR) and (result[6] == VERSION_MINOR) and (result[7] == VERSION_PATCH)

    serial.CloseSerialPort()


def test_DeviceID(serial):
    #Add documentation comments for functions of test files
    '''group basic tests
    verifies AB15(12) DeviceID
    tests:
    - MCU command:
        * USB_CMD_READ_DEV_ID'''
    
    serial.OpenSerialPort()

    user_cmd = USB_CMD_READ_DEV_ID
    serial.com_port.write(int(user_cmd).to_bytes(7, 'big'))
    time.sleep(1)

    result = serial.com_port.read(8)
    print(f'MCU response with IC device ID: ', end='') # expected 0xAB 0x8F 0x00 0x80 0x01 0xC4 0xBE 0xBA
    for itm in result:
        print(f'{itm:#03x} ', end='')
    assert (result[5] == DEVICE_ID_AB12)

    serial.CloseSerialPort()