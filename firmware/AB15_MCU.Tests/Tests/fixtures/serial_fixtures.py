import serial

class SerialWrapper():
    
    # MCU characteristics
    VENDOR_ID = 0x058B # Infineon DAS JDS - vendor ID
    PRODUCT_ID = 0x0043 # Infineon DAS JDS - product_ID

    def __init__(self, port_name:str="COM14"):
        # Protocol characteristics
        self.com_port = serial.Serial()
        self.com_port.baudrate = 115200
        self.com_port.port = port_name # hardcoded, can be different
        self.com_port.bytesize = 8
        self.com_port.parity = 'N'
        self.com_port.stopbits = 1
        self.com_port.timeout = 3 # in seconds

    def OpenSerialPort(self):
        self.com_port.open()
        if (not self.com_port.is_open):
            raise Exception("Can't open COM port!")

    def CloseSerialPort(self):
        self.com_port.close()
        if (self.com_port.is_open):
            raise Exception("Can't close COM port!")