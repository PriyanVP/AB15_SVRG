# group basic tests
# tests GET_MCU_VERSION, READ_DEV_ID
#TODO: add comprehensive `test failed` description messages into test's asserts

import os
import pytest
import serial
from time import sleep
from serial_fixtures import SerialWrapper
import package_helper as pkg

class TestDeviceID:
    '''DeviceId command tests'''
    
    # MCU firmware version TODO: find a vay to keep up-to-date automatically
    VERSION_MAJOR = 0x00
    VERSION_MINOR = 0x02
    VERSION_PATCH = 0x00
    # AB12 IC device ID
    DEVICE_ID_AB12 = 0xC4

    @classmethod
    def setup_class(cls):
        # COM port handling; connects to COM port with SW TC375 by default once before executing tests in class
        cls.serial = SerialWrapper()
        cls.serial.open_port()
        print ('\nsetup_class()')

    @classmethod 
    def teardown_class(cls):
        # disconnects after test's completion
        cls.serial.close_port()
        print ('\nteardown_class()')


    @pytest.mark.basic
    @pytest.mark.serial
    def test_COMport(self):
        '''group `basic` tests
        tests:
        - ShieldBuddy's serial connection to PC'''

        # TODO: check if approach works

        # Arrange

        # Act

        # Assert
        assert self.serial.com_port.is_open == True, "COM port is closed, when expected to be open"

        # Output to be captured if test passes
        print("ShieldBuddy is connected sucesfully at port", self.serial.com_port.name)

    @pytest.mark.serial
    @pytest.mark.basic
    def test_MCUVersion(self):
        '''group basic tests
        verifies ShieldBuddy's firmware version
        tests:
        - firmware version;
        - MCU command:
            * GET_MCU_BUILD_VERSION'''

        # Arrange
        packageToSend = pkg.TransmitPackage(0x00, 0x00, pkg.Command.GET_MCU_VERSION)

        # Act
        self.serial.com_port.write(packageToSend.serialize())
        sleep(0.1)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))

        # Assert
        assert is_response_received, "No response from MCU received"
        assert (result.payload[0] == self.VERSION_MAJOR) and (result.payload[1] == self.VERSION_MINOR) and (result.payload[2] == self.VERSION_PATCH), f"MCU version is not expected. Expected {hex(self.VERSION_MAJOR)}.{hex(self.VERSION_MINOR)}.{hex(self.VERSION_PATCH)}, but received {hex(result.payload[0])}.{hex(result.payload[1])}.{hex(result.payload[2])}"

        # Output to be captured if test passes
        print(f'Firmware version: {hex(result.payload[0])}.{hex(result.payload[1])}.{hex(result.payload[2])}') # expected 0x0 0x2 0x0

    @pytest.mark.serial
    @pytest.mark.basic
    @pytest.mark.skip(reason="Skipped due no HW connected (AB12/15 ASIC board)")
    def test_DeviceID(self):
        #Add documentation comments for functions of test files
        '''group basic tests
        verifies AB15(12) DeviceID
        tests:
        - MCU command:
            * READ_DEV_ID'''

        # Arrange
        packageToSend = pkg.TransmitPackage(0x0F, 0x01, pkg.Command.READ_DEV_ID)

        # Act
        self.serial.com_port.write(packageToSend.serialize())

        sleep(0.1)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))

        # Assert
        assert is_response_received, "No response from MCU received"
        assert (result.payload[0] == DEVICE_ID_AB12), f"Unexpected device ID. Expected {hex(DEVICE_ID_AB12)}, but received {result.payload[0]}"

        # Output to be captured if test passes
        print(f'MCU response with IC device ID: ', end='') # expected 0xAB 0x8F 0x00 0x80 0x01 0xC4 0xBE 0xBA
        for itm in result.package:
            print(f'{itm:#03x} ', end='')