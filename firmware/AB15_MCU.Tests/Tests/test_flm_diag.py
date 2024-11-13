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
    
    @classmethod
    def setup_class(cls):
        # COM port handling; connects to COM port with SW TC375 by default once before executing tests in class
        cls.serial = SerialWrapper()
        cls.serial.open_port()

    @classmethod 
    def teardown_class(cls):
        # disconnects after test's completion
        cls.serial.close_port()

    @pytest.mark.firing
    @pytest.mark.serial
    def test_FLMDiagReadResults(self):
        '''group `firing` tests
        tests:
        - size of USB_CMD_FLM_DIAG_READ_RESULTS payload (should be 86 bytes)'''

        # Arrange
        packageToSend = pkg.TransmitPackage(0x0F, 0, Command.FLM_DIAG_READ_RESULTS)
        
        # Act
        self.serial.com_port.write(packageToSend.serialize())
        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))

        # Assert
        assert result.payload_len == 86, f"Length of payload is not 86 bytes! Length received: {result.payload_len}"

    @pytest.mark.firing
    @pytest.mark.serial
    def test_EnableFLMDiag(self):
        '''group `firing` tests
        tests:
        - enabling AB15 FLM Diagnostics and manually getting the results of TODO: diagnostic'''

        # Arrange
        packageToSend = pkg.TransmitPackage(0x0F, 0, Command.FLM_DIAG_ENABLE)

        # Act
        self.serial.com_port.write(packageToSend.serialize())
        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
        # TODO: read LM_READ_SQUIB_RES_SQREF

        # Assert
        # TODO: add analysis of TODO: diagnostic results (LM_READ_SQUIB_RES_SQREF?)
        assert result.status == pkg.Status.ACK