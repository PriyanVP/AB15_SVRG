# group firing tests
# tests FLM module cyclic Diagnostics functionality

import os
import pytest
import serial
from time import sleep
from serial_fixtures import SerialWrapper
import package_helper as pkg

class TestFLMDiagCommands:
    '''FLM diagnostics tests'''

    # Delay for recieving MCU response
    DELAY = 0.1 # TODO: verify that duration is sufficient for all testcases
    # Delay for diagnostics to run on ASIC
    DELAY_DIAG = 2
    
    @classmethod
    def setup_class(cls):
        # COM port handling; connects to COM port with SW TC375 by default once before executing tests in class
        cls.serial = SerialWrapper()
        cls.serial.open_port()

    @classmethod 
    def teardown_class(cls):
        # disconnects after test's completion
        cls.serial.close_port()

    # TODO: test_EnableFLMDiag not operational, update pending
    # - enable flags for separate diags;
    # - FLM_DIAG_ENABLE payload must have value for SVRG_DIAG configuration
    @pytest.mark.firing
    @pytest.mark.serial
    def test_EnableFLMDiag(self):
        '''group `firing` tests
        tests:
        - enabling AB15 FLM Diagnostics and manually getting the results of diagnostic'''

        # Arrange
        # TODO: separate ASIC cold start into it's own method
        address = 0x01E # SysStates_Reset_Config
        data = 0x001 # Bit 0 spi_coldstart1
        address_converted = pkg.Int2BytesConverter(address)
        data_converted = pkg.Int2BytesConverter(data)
        payload = [*address_converted.bytes, *data_converted.bytes]
        packageToSend = pkg.TransmitPackage(0x00, 0x01, pkg.Command.WRITE_REG, payload)
        
        # Act
        self.serial.com_port.write(packageToSend.serialize()) # Issue ASIC cold start 
        sleep(self.DELAY)
        packageToSend = pkg.TransmitPackage(0x0F, 0, pkg.Command.FLM_DIAG_ENABLE)
        self.serial.com_port.write(packageToSend.serialize()) # Turn on diagnostics
        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
        print("Status:" + str(result.status))

        # Assert
        # TODO: add analysis of TODO: diagnostic results
        assert result.status == pkg.Status.ACK

    @pytest.mark.firing
    @pytest.mark.serial
    def test_FLMDiagReadResults(self):
        '''group `firing` tests
        tests:
        - size of USB_CMD_FLM_DIAG_READ_RESULTS payload (should be 86 bytes)'''

        # Arrange
        packageToSend = pkg.TransmitPackage(0x0F, 0, pkg.Command.FLM_DIAG_READ_RESULTS)
        sleep(self.DELAY_DIAG)
        
        # Act
        self.serial.com_port.write(packageToSend.serialize())
        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
        print("Length of payload, bytes:" + str(result.payload_len))
        # Assert
        assert result.payload_len == 92, f"Length of payload is not 86 bytes! Length received: {result.payload_len}"
        # TODO: analyse and check values of diagnostics results