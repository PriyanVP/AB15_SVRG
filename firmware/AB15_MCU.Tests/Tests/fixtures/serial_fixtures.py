import serial

# Protocol characteristics
baud_rate = 115200
data_size = 8
parity = None
stop_bit = 1
port_name = 'COM14' # hardcoded, can be different
com_port = serial.Serial()
com_port.baudrate = baud_rate
com_port.port = port_name
com_port.bytesize = 8
com_port.parity = 'N'
com_port.stopbits = 1
com_port.timeout = 3 # in seconds

def OpenSerialPort():
    com_port.open()
    if (not com_port.is_open):
        raise Exception("Can't open COM port!")

def CloseSerialPort():
    com_port.close()
    if (com_port.is_open):
        raise Exception("Can't close COM port!")