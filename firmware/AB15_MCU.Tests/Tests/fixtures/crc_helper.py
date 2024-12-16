from typeguard import typechecked
from typing import List

@typechecked
def crc3(buffer: int, start_bit_position: int, end_bit_position: int) -> int:
    '''CRC8 algorithm
    Polynom: x3+x1+1, Initial value: 0x7'''

    NUM_TRAILING_ZEROS = 3
    input_data_msb_index = end_bit_position - start_bit_position + NUM_TRAILING_ZEROS # index of first MSB relevant for CRC

    # Get variable with only relevant data for CRC calculation (starting from index 0)
    # Clear MSB higher than end_bit_position -> clear LSB lower than start_bit_position -> shift relevant for CRC bits to 0 index -> add trailing 0
    extended_buffer = (((buffer << (31 - end_bit_position)) & 0xFFFF_FFFF) >> (31 - end_bit_position + start_bit_position)) << NUM_TRAILING_ZEROS

    # Store binary start value "111" into CRC register according to spec
    bit0 = 0x1
    bit1 = 0x1
    bit2 = 0x1

    # calculate CRC as in specification
    # CRC polynomial: x^3 + x + 1 --> 1*x^3 + 0* x^2 + 1*x^1 + 1*x^0 --> poly = 1011
    # - process data from MSB to LSB
    # - XOR bits according to polynomial
    for i in range(input_data_msb_index, -1, -1):
        # Process bits
        tmp_bit0 = bit2 ^ ((extended_buffer >> i) & 0x01)
        tmp_bit1 = bit0 ^ bit2
        tmp_bit2 = bit1

        # Update with new values
        bit0 = tmp_bit0
        bit1 = tmp_bit1
        bit2 = tmp_bit2

    # CRC result
    return ((bit2 << 2) | (bit1 << 1) | bit0)

@typechecked
def crc8(data_array: list, start_idx: int, length: int) -> int:
    '''CRC8 algorithm
    Polynom: x8+x2+x1+1, Initial value: 0x00'''
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

@typechecked
def crc16(data_array: List[int], start_idx: int, length: int) -> int:
    '''CRC16 algorithm
    Polynom: x14+x13+x12+x10+x8+x6+x4+x3+x1+1, Initial value: 0xFFFF'''
    poly = 0x755B
    crc = 0xFFFF

    for i in range(start_idx, start_idx+length):
        data = data_array[i]
        for i in range(15, -1, -1):
            if (crc & 0x8000) != 0:
                crc = ((crc << 1) | ((data >> 15) & 1)) ^ poly
            else:
                crc = (crc << 1) | ((data >> 15) & 1)
            data = (data << 1) & 0xFFFF  # Ensure data remains 16-bit
        crc &= 0xFFFF  # Ensure crc remains 16-bit

    return crc


# Self test + usage example
if __name__ == "__main__":
    # CRC3 tests
    frame = 0x0040_0010
    start_bit_position = 5
    end_bit_position = 31
    crc = crc3(frame, start_bit_position, end_bit_position)
    print(f"CRC3 Result 1: Actual 0x{crc:01X}, Expected 0x4")

    frame = 0x3000_0C46
    start_bit_position = 3
    end_bit_position = 26
    crc = crc3(frame, start_bit_position, end_bit_position)
    print(f"CRC3 Result 2: Actual 0x{crc:01X}, Expected 0x6")

    # CRC8 tests
    data_array  = [0xAB, 0x0F, 0x01, 0x03, 0x00, 0xD3, 0xBA] # (CRC should be: 0xD3)
    start_idx = 1
    length = len(data_array)-3 # use 4 elements starting from index 1 (due to package format)
    print(f"Result: {hex(crc8(data_array, start_idx, length))}")

    # CRC16 tests
    registers2 = [0x0000, 0x1234, 0x2345, 0x89ff, 0xabfc, 0x0000]
    registers3 = [0x0110, 0x1224, 0x2335, 0xffff, 0x0000]

    crc = crc16(registers2, 0, len(registers2))
    print(f"CRC Result 2: Actual 0x{crc:04X}, Expected 0xA113")

    crc = crc16(registers3, 0, len(registers3))
    print(f"CRC Result 3: Actual 0x{crc:04X}, Expected 0xE8CB")