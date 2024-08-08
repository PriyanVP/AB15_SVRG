# group basic tests
# tests TODO mcu commands

import serial
import time

#TestMCUFWVersion TODO implement for test example to run without ASIC (CmdGetMcuVersion() from general_cmd.c)

# MCU characteristics
VENDOR_ID = 0x058B # Infineon DAS JDS - vendor ID
PRODUCT_ID = 0x0043 # Infineon DAS JDS - product_ID
USB_CMD_GET_MCU_VERSION = 0xAB00001A004FBA # from AB_protocol_description.xlsx, corresponds to CmdGetMcuVersion()
# AB 0F 00 19 00 CC BA
# +  -  +  -  +  ?  +
# AB 00 00 1A 00 4F BA

# Protocol characteristics
baud_rate = 115200
data_size = 8
parity = None
stop_bit = 1

port_name = 'COM14' # hardcoded, can be different

# Opening COM port for read/write
com_port = serial.Serial()
com_port.baudrate = baud_rate
com_port.port = port_name
com_port.bytesize = 8
com_port.parity = 'N'
com_port.stopbits = 1
com_port.timeout = 3 # in seconds

# Open com port
com_port.open()
assert com_port.is_open, "Can't open com port!"
if com_port.is_open:
    print ("COM14 is open")

user_cmd = USB_CMD_GET_MCU_VERSION
print(int(user_cmd).to_bytes(7, 'big'))
print(user_cmd)
com_port.write(int(user_cmd).to_bytes(7, 'big'))
time.sleep(1)
result = com_port.read(10)
print(f'Firmware version: ', end='') # expected 0xab 0x80 0x0 0x83 0x3 0x30 0x31 0x35 0x52 0xba
for itm in result:
    print(f'{itm:#03x} ', end='')

# Close com port
com_port.close()
assert (not com_port.is_open), "Can't close com port!"




def TestMCUVersion():
    '''group basic tests
    verifies ShieldBuddy's firmware version
    tests:
    - firmware version;
    - MCU command:
        * mcu_command_name'''
    #assert
    


def TestDeviceID():
    #Add documentation comments for functions of test files
    '''group basic tests
    verifies AB15(12) DeviceID
    tests:
    - MCU command:
        * mcu_command_name'''
    #assert 
