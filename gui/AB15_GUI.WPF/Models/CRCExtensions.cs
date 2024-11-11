using System;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Models
{
    public static class CRCExtensions
    {
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
