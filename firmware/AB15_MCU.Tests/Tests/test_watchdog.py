# group watchdog tests
# expects Asic startup, does basic watchdog config and tests if Watchdog trigger was sucessful
# shall be started as soon as ASIC enters init mode 

    # steps to progress: 
    # 1. issue reset
    # 2. init timer creaet--> clear IMT      
    # 3. configure Watchdog  by 
    # void CmdConfigureWatchdog(USBReceiveData const * const commandPackage)
    #  - configures ASIC and 
    #  - configures MCU WDT triggers 

    # 4. start watchdog 
    # 5. get watchdog status
    # 6 enter EOP

import os
import pytest
import serial
from time import sleep
from serial_fixtures import SerialWrapper
import package_helper as pkg

class TestWatchdog:
    '''Watchdog tests'''
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

    @pytest.mark.watchdog
    def test_ReadSystemState(self):
        '''Checks system state  
        tests:
        - COMMON_SYSTEM_STATE'''
        # Arrange
        address = 0x021
        data = 0x000
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
        # assert (received_value.int_value == data), f"Unexpected data. Expected {hex(data)}, but received {received_value.int_value}"
        # Output to be captured if test passes
        print(f'MCU response with Status: ', end='')
        for itm in result.package:
            print(f'{itm:#03x} ', end='')

    @pytest.mark.watchdog
    def test_ReadGeneralStatus2(self):
        '''Checks general status 
        tests:
        - GENERAL STAUS 2'''
        # Arrange
        address = 0x01D
        data = 0x000
        msg_id = 0x00
        device_id = 0x01
        address_converted = pkg.Int2BytesConverter(address)
        packageToSend = pkg.TransmitPackage(msg_id, device_id, pkg.Command.READ_REG, address_converted.bytes)

        # Act
        self.serial.com_port.write(packageToSend.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
        
        # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.DATA, f"Incorrect status in payload. Expected DATA, but received {result.status}"
        # Output to be captured if test passes
        print(f'MCU response with Status: ', end='')
        for itm in result.package:
            print(f'{itm:#03x} ', end='')
    
    @pytest.mark.watchdog
    def test_IssueColdStart(self):
        '''issues ASIC cold start 
        tests:
        - correct ACK only' '''
        # Arrange
        msg_id = 0x00
        device_id = 0x01
        address = 0x01E # SysStates_Reset_Config
        data = 0x001 # Bit 0 spi_coldstart1
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
    
    @pytest.mark.watchdog
    def test_configureWatchdog(self):
        '''config Wd in MCU and ASIC  
        tests:
        - correct ACK only'''
        # Arrange
        msg_id = 0x00
        device_id = 0x01
        address = [0x00, 0x30, 0x00, 0x31] # adresse for both registers, spit in byte 
        # data for relaxed WD: locktime = 1 and Response time = 3E see chapter 1.6.7.3.3.2 --> 0x07e split  in 4 bytes 
        data = [0x00, 0x7E, 0x00, 0x7E] 
        payload = [*address[0:2], *data[0:2], *address[2:4], *data[2:4]] # recombine dress and data for the transmit function
        print("Address: 0x{address:02X}")
        print("Data: 0x{data:02X}")
        print("payload: 0x{payload:02X}")
        packageToSend = pkg.TransmitPackage(0x00, 0x01, pkg.Command.CONFIGURE_WATCHDOG, payload)

        # Act
        print('MCU config WDT with:  ')
        self.serial.com_port.write(packageToSend.serialize())

        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))
          # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.ACK, f"Incorrect status in payload. Expected ACK, but received {result.status}"
        
    @pytest.mark.watchdog
    def test_startWatchdog(self):
        '''starts watchdog in MCU  
        tests:
        - correct ACK only' '''
        # Arrange
        packageToSend = pkg.TransmitPackage(0x00, 0x00, pkg.Command.START_WATCHDOG)

        # Act
        self.serial.com_port.write(packageToSend.serialize())
        sleep(self.DELAY)
        is_response_received = self.serial.extract_packages()
        result = pkg.ReceivePackage(self.serial.packages.pop(0))

        # Assert
        assert is_response_received, "No response from MCU received"
        assert result.status == pkg.Status.ACK, f"Incorrect status returned. Expected ACK, but received {result.status}"
        
    @pytest.mark.watchdog
    def test_ReadWatchdogState(self):
        '''Checks WD state  
        tests:
        - spi_read_wdstatus1'''
        # Arrange
        address = 0x03E
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
        # assert (received_value.int_value == data), f"Unexpected data. Expected {hex(data)}, but received {received_value.int_value}"
        # Output to be captured if test passes
        print(f'MCU response with Status: ', end='')
        for itm in result.package:
            print(f'{itm:#03x} ', end='')
 
    @pytest.mark.watchdog
    def test_resetInitTimer(self):
        '''issues a reset of the init mode timer 
        tests:
        - correct ACK only' '''
        # Arrange
        msg_id = 0x00
        device_id = 0x01
        address = 0x01E # SysStates_Reset_Config
        data = 0x080 # Bit 7  spi_clear_imt 
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
