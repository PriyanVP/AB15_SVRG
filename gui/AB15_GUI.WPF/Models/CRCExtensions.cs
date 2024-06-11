using System;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Models
{
    public static class CRCExtensions
    {
        /// <summary>
        /// Calculate CRC (CRC-8-CCITT, -ITU). Code identical to function used in MCU
        /// Polynom: x^8+x^2+x+1
        /// </summary>
        /// <param name="dataArray">link to buffer to operate on</param>
        /// <param name="startIdx">index of first byte for CRRC calculation</param>
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
    }
}
