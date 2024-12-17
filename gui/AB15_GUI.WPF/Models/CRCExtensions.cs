using System;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Models
{
    public static class CRCExtensions
    {
        /// <summary>
        /// Calculate CRC3. Code identical to function used in MCU
        /// Polynom: x^3+x+1, Initial value: 0x7
        /// </summary>
        /// <param name="data">data to calculate CRC</param>
        /// <param name="startBitPosition">index of first LSB relevant for CRC bit (0 based)</param>
        /// <param name="endBitPosition">index of last MSB relevant for CRC bit (0 based)</param>
        /// <returns>CRC value</returns>
        public static uint GetCRC3(this uint data, int startBitPosition, int endBitPosition)
        {
            const int NUM_TRAILING_ZEROS = 3;
            uint extended_buffer;
            uint bit0, bit1, bit2;
            uint tmp_bit0, tmp_bit1, tmp_bit2;
            int input_data_msb_index = endBitPosition - startBitPosition + NUM_TRAILING_ZEROS; // index of first MSB relevant for CRC

            // Get variable with only relevant data for CRC calculation (starting from index 0)
            // Clear MSB higher than endBitPosition -> clear LSB lower than startBitPosition -> shift relevant for CRC bits to 0 index -> add trailing 0
            extended_buffer = ((data << (31 - endBitPosition)) >> (31 - endBitPosition + startBitPosition)) << NUM_TRAILING_ZEROS;

            // Store binary start value "111" into CRC register according to spec
            bit0 = 0x1;
            bit1 = 0x1;
            bit2 = 0x1;

            // calculate CRC as in specification
            // CRC polynomial: x^3 + x + 1 --> 1*x^3 + 0* x^2 + 1*x^1 + 1*x^0 --> poly = 1011
            // - process data from MSB to LSB
            // - XOR bits according to polynomial
            for (int i = input_data_msb_index; i >= 0; i--)
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

        /// <summary>
        /// Calculate CRC (CRC-8-CCITT, -ITU). Code identical to function used in MCU
        /// Polynom: x^8+x^2+x+1, Initial value: 0x00
        /// </summary>
        /// <param name="dataArray">link to buffer to operate on</param>
        /// <param name="startIdx">index of first byte for CRC calculation</param>
        /// <param name="length">number of bytes for CRC calculation</param>
        /// <returns>CRC value</returns>
        public static byte GetCRC8(this IList<byte> dataArray, int startIdx, int length)
        {
            byte crc = 0x00; // initial value
            for (int i = startIdx; i < (startIdx + length); i++)
            {
                crc ^= dataArray[i];
                for (byte j = 0; j < 8; j++)
                {
                    if ((crc & 0x80) != 0)
                    {
                        crc = (byte)((crc << 1) ^ 0x7); // polynomial (MSB not included)
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

        /// <summary>
        /// Calculate CRC (CRC16)
        /// Polynom: x14+x13+x12+x10+x8+x6+x4+x3+x1+1, Initial value: 0xFFFF
        /// </summary>
        /// <param name="dataArray">link to buffer to operate on</param>
        /// <param name="startIdx">index of first byte for CRC calculation</param>
        /// <param name="length">number of bytes for CRC calculation</param>
        /// <returns>CRC value</returns>
        public static UInt16 GetCRC16(this IList<UInt16> dataArray, int startIdx, int length)
        {
            const int MSB_POS = 15;             // Index of most significant bit
            const int MSB_READMASK = 0x8000;    // Readmask for most significant bit
            const UInt16 POLINOMIAL = 0x755B;   // Polynomial for CRC (MSB is not included)
            UInt16 crc = 0xFFFF;                // initial value
            UInt16 data;                        // tmp data storage variable
            for (int i = startIdx; i < (startIdx + length); i++)
            {
                data = dataArray[i];
                for (int j = MSB_POS; j >= 0; j--)
                {
                    if ((crc & MSB_READMASK) != 0)
                    {
                        crc = (UInt16) ((UInt16)((crc << 1) | ((data >> MSB_POS) & 1)) ^ POLINOMIAL);
                    }
                    else
                    {
                        crc = (UInt16)((crc << 1) | ((data >> MSB_POS) & 1));
                    }
                    data <<= 1;
                }
            }
            return crc;
        }
    }
}
