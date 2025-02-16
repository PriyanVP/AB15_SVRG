# group basic tests
# tests all commands in general_cmd.c

import os
import pytest
import serial
from time import sleep
from serial_fixtures import SerialWrapper
import package_helper as pkg

class TestUART:
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

        # Assert
        exptected_data = 0x00FF             # Expected data, lowest 7 bits are received data, next bit is received flag, higher bits are error flags
        is_response_received = self.serial.extract_packages()
        assert is_response_received, "No response from MCU received"
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
        assert result.status == pkg.Status.DATA, f"Incorrect status in payload. Expected DATA, but received {result.status}"
        received_value = pkg.Bytes2IntConverter(result.payload)
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
