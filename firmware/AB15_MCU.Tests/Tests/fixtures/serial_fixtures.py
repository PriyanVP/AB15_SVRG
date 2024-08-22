import serial
import serial.tools.list_ports
from typing import List
from typeguard import typechecked
from package_constants import pkg_const

@typechecked
class SerialWrapper():
    '''Class to manage serial communication'''

    # MCU characteristics
    VENDOR_ID = 0x058B  # Infineon DAS JDS - vendor ID
    PRODUCT_ID = 0x0043 # Infineon DAS JDS - product_ID

    def __init__(self, port_name:str=None):
        # Protocol characteristics
        self.com_port = serial.Serial()
        self.com_port.baudrate = 115200
        self.com_port.port = port_name  #TODO: provide manual name option and autodetect
        self.com_port.bytesize = 8
        self.com_port.parity = 'N'
        self.com_port.stopbits = 1
        self.com_port.timeout = 1 # in seconds

        self.packages = []

    def open_port(self):
        self.com_port.open()
        if (not self.com_port.is_open):
            raise Exception("Can't open COM port!")

    def close_port(self):
        self.com_port.close()
        if (self.com_port.is_open):
            raise Exception("Can't close COM port!")

    def find_connected_boards(self) -> List[str]:
        '''Get list of COM ports where ShieldBuddy TC375 are connected'''
        ports = serial.tools.list_ports.comports()
        # List all COM ports with human readable data
        for port, desc, hwid in sorted(ports):
            print(f"{port}: {desc} [{hwid}]")
            # TODO: use port data to determine where SB TC375 is connected
        pass

    def extract_packages(self) -> bool:
        '''Extract MCU responses from input com port buffer and store them to attribute
            Warning: won't report if invalid data was received - will just skip it
            Returns true if at least one package was extracted'''
        is_package_found = false
        tmp_package = []

        while (self.com_port.in_waiting > pkg_const.MIN_PACKAGE_LENGTH):
            # Read first byte
            tmp = self.com_port.read(pkg_const.START_BYTE_LENGTH) 
            tmp_package.append(*[byte for byte in tmp]) # append bytes from buffer as individual int values

            # If first byte is incorrect, remove it and try next byte
            if (tmp_package[0] != pkg_const.START_BYTE_VALUE):
                tmp_package.clear()
                continue

            # Read bytes till payload length
            tmp = self.com_port.read(pkg_const.MSG_ID_LENGTH + pkg_const.ASIC_ID_LENGTH + pkg_const.CMD_STATUS_LENGTH + pkg_const.PAYLOAD_LENGTH_LENGTH) 
            tmp_package.append(*[byte for byte in tmp]) # append bytes from buffer as individual int values

            # Check if enough items in buffer
            remaining_package_length = tmp_package[pkg_const.PAYLOAD_POSITION] + pkg_const.CRC_LENGTH + pkg_const.END_BYTE_LENGTH
            if (self.com_port.in_waiting < remaining_package_length):
                tmp_package.clear()
                continue

            # Read remaining bytes in package
            tmp = self.com_port.read(remaining_package_length) 
            tmp_package.append(*[byte for byte in tmp]) # append bytes from buffer as individual int values

            # Report that package was found
            is_package_found = True

            # Add package to list
            self.packages.append(tmp_package)

            # Clear tmp variable for the next iteration
            tmp = None
            tmp_package = []

        return is_package_found

if __name__ == "__main__":
    serial_tst = SerialWrapper('COM14')
    # boards = serial_tst.find_connected_boards()