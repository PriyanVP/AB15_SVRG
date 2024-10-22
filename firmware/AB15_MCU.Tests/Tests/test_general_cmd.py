# group basic tests
# tests all commands in general_cmd.c

import os
import pytest
import serial
from time import sleep
from serial_fixtures import SerialWrapper
import package_helper as pkg

class TestGeneralCommands:
    '''General commands tests'''

    # MCU firmware version TODO: find a vay to keep up-to-date automatically
    VERSION_MAJOR = 0x00
    VERSION_MINOR = 0x02
    VERSION_PATCH = 0x00

    # AB12/15 IC device ID
    DEVICE_ID_AB12 = 0xC4
    DEVICE_ID_AB15 = 0x04 # TODO: what is correct value?

    # Delay for recieving MCU response
    DELAY = 0.1
    
    @classmethod
    def setup_class(cls):
        # COM port handling; connects to COM port with SW TC375 by default once before executing tests in class
        cls.serial = SerialWrapper()
        cls.serial.open_port()

    @classmethod 
    def teardown_class(cls):
        # disconnects after test's completion
        cls.serial.close_port()

    @pytest.mark.basic
    @pytest.mark.serial
    def test_COMport(self):
        '''group `basic` tests
        tests:
        - ShieldBuddy's serial connection to PC'''

        # Arrange

        # Act

        # Assert
        assert self.serial.com_port.is_open == True, "COM port is closed, when expected to be open"

        # Output to be captured if test passes
        print("ShieldBuddy is connected successfully at port", self.serial.com_port.name)

    def test_IsAlive(self):
        ''' Check if "is alive" command is working
        tests:
        - USB_CMD_IS_ALIVE'''

        # Arrange
        packageToSend = pkg.TransmitPackage(0x00, 0x00, pkg.Command.IS_ALIVE)

        # Act
        self.serial.com_port.write(packageToSend.serialize())
        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))

        # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.ACK

    @pytest.mark.serial
    @pytest.mark.basic
    def test_GetMcuVersion(self):
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
        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))

        # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.STATUS, f"Incorrect status in payload. Expected STATUS, but received {result.status}"
        assert (result.payload[0] == self.VERSION_MAJOR) and (result.payload[1] == self.VERSION_MINOR) and (result.payload[2] == self.VERSION_PATCH), f"MCU version is not expected. Expected {hex(self.VERSION_MAJOR)}.{hex(self.VERSION_MINOR)}.{hex(self.VERSION_PATCH)}, but received {hex(result.payload[0])}.{hex(result.payload[1])}.{hex(result.payload[2])}"

    @pytest.mark.serial
    @pytest.mark.basic
    def test_GetDeviceId(self):
        '''group basic tests
        verifies AB15 DeviceID
        tests:
        - MCU command:
            * READ_DEV_ID'''

        # Arrange
        packageToSend = pkg.TransmitPackage(0x0F, 0x01, pkg.Command.READ_DEV_ID)

        # Act
        self.serial.com_port.write(packageToSend.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))

        # Assert
        assert is_response_received, "No response from MCU received"
        assert (result.payload[0] == self.DEVICE_ID_AB15), f"Unexpected device ID. Expected {hex(DEVICE_ID_AB15)}, but received {result.payload[0]}"

        # Output to be captured if test passes
        print(f'MCU response with IC device ID: ', end='') # expected 0xAB 0x8F 0x00 0x80 0x01 0xC4 0xBE 0xBA
        for itm in result.package:
            print(f'{itm:#03x} ', end='')

    @pytest.mark.parametrize("address,data", [(0x000, 0x0241), (0x001, 0x0096)])
    def test_ReadReg(self, address, data):
        '''Checks if reading single register works as expected
        tests:
        - READ_REG'''

        # Arrange
        msg_id = 0x00
        device_id = 0x01
        address_converted = pkg.Int2BytesConverter(address)
        packageToSend = pkg.TransmitPackage(msg_id, device_id, pkg.Command.READ_REG, address_converted.bytes)

        # Act
        self.serial.com_port.write(packageToSend.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
        received_value = pkg.Bytes2IntConverter(result.payload)

        # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.DATA, f"Incorrect status in payload. Expected DATA, but received {result.status}"
        assert (received_value.int_value == data), f"Unexpected data. Expected {hex(data)}, but received {received_value.int_value}"

    @pytest.mark.parametrize("address,data,expected", [(0x030, 0x006, 0x006)])
    def test_WriteReg(self, address, data, expected):
        '''Checks if writing single register works as expected
        tests:
        - WRITE_REG'''

        # Arrange
        msg_id = 0x00
        device_id = 0x01
        address_converted = pkg.Int2BytesConverter(address)
        data_converted = pkg.Int2BytesConverter(data)
        # extract element from address_converted.bytes 
        payload = [*address_converted.bytes, *data_converted.bytes]
            # print some values
        print("SOME STUFF")
        print(payload)

        packageToSend = pkg.TransmitPackage(0x00, 0x01, pkg.Command.WRITE_REG, payload)

        # Act
        self.serial.com_port.write(packageToSend.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
    
        
        # received_value = pkg.Bytes2IntConverter(result.payload)
        
        # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.ACK, f"Incorrect status in payload. Expected DATA, but received {result.status}"
        assert (result.payload_len == 0), f"Unexpected data. Expected {hex(data)}, but received {received_value.int_value}"