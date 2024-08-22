def crc8(data_array: list, start_idx: int, length: int) -> int:
    '''CRC8 algorithm'''
    polynomial = 0x7 # polynomial (MSB not included)
    crc = 0x00
    for i in range(start_idx, start_idx+length):
        crc ^= data_array[i]
        for j in range(8):
            if ((crc & 0x80) != 0):
                crc = ((crc << 1) ^ polynomial)
            else:
                crc <<= 1
            crc &= 0xff # Leave only 8 LSB
    crc ^= 0x55 # final XOR
    return crc


# Self test + usage example
if __name__ == "__main__":
    data_array  = [0xAB, 0x0F, 0x01, 0x03, 0x00, 0xD3, 0xBA] # (CRC should be: 0xD3)
    start_idx = 1
    length = len(data_array)-3 # use 4 elements starting from index 1 (due to package format)
    print(f"Result: {hex(crc8(data_array, start_idx, length))}")