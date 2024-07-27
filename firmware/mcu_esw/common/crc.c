/**********************************************************************************************************************
 * \file crc.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

uint8 CRC8(uint8 * const buffer, uint16 length)
{
    uint8 crc = 0x00; // initial value
    for (uint16 i = 0; i < length; i++)
    {
        crc ^= buffer[i];
        for (uint8 j = 0; j < 8; j++)
        {
            if ((crc & 0x80) != 0)
            {
                crc = (uint8)((crc << 1) ^ 0x7); // polynomial (MSB not included)
            }
            else
            {
                crc <<= 1;
            }
        }
    }
    crc ^= 0x55; // final XOR
    return crc;
}

uint8 CRC3(const uint32 buffer, const uint8 startBitPosition, const uint8 endBitPosition)
{
    const uint8 num_trailing_zeros = 3;
    uint32 extended_buffer;
    uint8 bit0, bit1, bit2;
    uint8 tmp_bit0, tmp_bit1, tmp_bit2;
    uint8 input_data_msb_index = endBitPosition - startBitPosition + num_trailing_zeros; // index of first MSB relevant for CRC

    // Get variable with only relevant data for CRC calculation (starting from index 0)
    // Clear MSB higher than endBitPosition -> clear LSB lower than startBitPosition -> shift relevant for CRC bits to 0 index -> add trailing 0
    extended_buffer = ((buffer << (31 - endBitPosition)) >> (31 - endBitPosition + startBitPosition)) << num_trailing_zeros;

    // Store binary start value "111" into CRC register according to spec
    bit0 = 0x1;
    bit1 = 0x1;
    bit2 = 0x1;

    // calculate CRC as in specification
    // CRC polynomial: x^3 + x + 1 --> 1*x^3 + 0* x^2 + 1*x^1 + 1*x^0 --> poly = 1011
    // - process data from MSB to LSB
    // - XOR bits according to polynomial
    for (sint8 i = input_data_msb_index; i >= 0; i--)
    {
        // Process bits
        tmp_bit0 = bit2 ^ ((extended_buffer >> i) & 0x01);
        tmp_bit1 = bit0 ^ bit2;
        tmp_bit2 = bit1;

        // Update with new values
        bit0 = tmp_bit0;
        bit1 = tmp_bit1;
        bit2 = tmp_bit2;
    }

    // CRC result
    return ((bit2 << 2) | (bit1 << 1) | bit0);
}