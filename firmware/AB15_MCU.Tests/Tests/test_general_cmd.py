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
    DEVICE_ID_AB15 = (0x0241 << 16) | 0x0096

    # Delay for recieving MCU response
    DELAY = 0.1 # TODO: verify that duration is sufficient for all testcases
    
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
        received_value = (result.payload[0] & 0xFF ) | ((result.payload[1] << 8) & 0xFF00 ) | ((result.payload[2] << 16) & 0xFF0000)  | ((result.payload[3] << 24) & 0xFF000000)

        # Assert
        assert is_response_received, "No response from MCU received"
        assert (received_value == self.DEVICE_ID_AB15), f"Unexpected device ID. Expected {hex(self.DEVICE_ID_AB15)}, but received {received_value}"

        # Output to be captured if test passes
        print(f'MCU response with IC device ID: ', end='')
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
        payload = [*address_converted.bytes, *data_converted.bytes]

        packageToSend = pkg.TransmitPackage(0x00, 0x01, pkg.Command.WRITE_REG, payload)

        # Act
        self.serial.com_port.write(packageToSend.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
        
        # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.ACK, f"Incorrect status in payload. Expected ACK, but received {result.status}"
        assert (result.payload_len == 0), f"Unexpected data. Expected empty payload, but received {result.payload}"

    # @pytest.mark.parametrize("device_id,data", [(0x02, 0xF055BB0F)])
    # def test_WriteRaw(self, device_id, data, ):
    def test_WriteRaw(self):
        '''Checks if writing single register works as expected
        tests:
        - WRITE_REG'''

        # Arrange
        msg_id = 0x00
        device_id = 0x07 # CS MON2 
        # address_converted = pkg.Int2BytesConverter(address)
        # data_converted = pkg.Int2BytesConverter(data)
        # payload = [0x02, 0x00, 0x08, 0x06] correct  order 
        payload = [0x06, 0x08, 0x00, 0x02] # wrong order, needs to be changed in MCU code 

        packageToSend = pkg.TransmitPackage(0x00, device_id, pkg.Command.WRITE_RAW_DATA_SPI, payload)

        # Act
        self.serial.com_port.write(packageToSend.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
        
        # Arrange
        msg_id = 0x00
        device_id = 0x01
        address = 0x144
        address_converted = pkg.Int2BytesConverter(address)
        packageToSend = pkg.TransmitPackage(msg_id, device_id, pkg.Command.READ_REG, address_converted.bytes)

        # Act
        self.serial.com_port.write(packageToSend.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
        print("\nFull Package received: ",[hex(itm) for itm in result.package])
        
        received_value = pkg.Bytes2IntConverter(result.payload)
        print("raw data",received_value)
        # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.DATA, f"Incorrect status in payload. Expected DATA, but received {result.status}"
        assert (received_value.int_value == 0x080), f"Unexpected data. Expected 0x80 , but received {received_value.int_value}"


    def test_uart_valid(self):
        """Checks writing data via UART.
        tests:
        - WRITE_DATA_UART"""

        # Arrange
        msg_id = 0x00
        device_id = 0x01
        command = pkg.Command.WRITE_DATA_UART
        payload = [0xAA, 0xFF, 0x00]
        package_to_send = pkg.TransmitPackage(msg_id, device_id, command, payload)

        # Act
        self.serial.com_port.write(package_to_send.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))

        # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.ACK, f"Incorrect status in payload. Expected ACK, but received {result.status}"
        assert (result.payload_len == 0), f"Unexpected data. Expected empty payload, but received {result.payload}"

        # Validate that data is received by ASIC

        # Arrange
        msg_id = 0x00
        device_id = 0x01
        address_uart_ext_data = 0x8F        # Address of UART external data register
        address_converted = pkg.Int2BytesConverter(address_uart_ext_data)
        package_to_send = pkg.TransmitPackage(msg_id, device_id, pkg.Command.READ_REG, address_converted.bytes)

        # Act
        self.serial.com_port.write(package_to_send.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
        received_value = pkg.Bytes2IntConverter(result.payload)

        # Assert
        exptected_data = 0x00FF             # Expected data, lowest 7 bits are received data, next bit is received flag, higher bits are error flags
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.DATA, f"Incorrect status in payload. Expected DATA, but received {result.status}"
        assert (received_value.int_value == exptected_data), f"Unexpected data. Expected {hex(exptected_data)}, but received {received_value.int_value}"

    def test_uart_invalid_too_short(self):
        """Checks writing data via UART.
        tests:
        - WRITE_DATA_UART"""

        # Arrange
        msg_id = 0x00
        device_id = 0x01
        command = pkg.Command.WRITE_DATA_UART
        payload = [0xAA, 0xFF]
        package_to_send = pkg.TransmitPackage(msg_id, device_id, command, payload)

        # Act
        self.serial.com_port.write(package_to_send.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))

        # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.ERROR, f"Incorrect status in payload. Expected ERROR, but received {result.status}"
        assert (result.payload_len == 0), f"Unexpected data. Expected empty payload, but received {result.payload}"

    def test_uart_invalid_too_long(self):
        """Checks writing data via UART.
        tests:
        - WRITE_DATA_UART"""

        # Arrange
        msg_id = 0x00
        device_id = 0x01
        command = pkg.Command.WRITE_DATA_UART
        payload = [0xAA, 0xFF, 0x00, 0xBB]
        package_to_send = pkg.TransmitPackage(msg_id, device_id, command, payload)

        # Act
        self.serial.com_port.write(package_to_send.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))

        # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.ERROR, f"Incorrect status in payload. Expected ERROR, but received {result.status}"
        assert (result.payload_len == 0), f"Unexpected data. Expected empty payload, but received {result.payload}"
