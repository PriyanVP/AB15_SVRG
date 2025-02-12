using System;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Models
{
    public static class BitOperationsExtensions
    {
        /// <summary>
        /// Get MSB byte from 16 bit unsigned integer
        /// </summary>
        /// <param name="data">16 bit input data</param>
        /// <returns></returns>
        public static byte GetMSB(this UInt16 data)
        {
            return (byte)((data & 0xFF00) >> 8);
        }

        /// <summary>
        /// Get LSB byte from 16 bit unsigned integer
        /// </summary>
        /// <param name="data">16 bit input data</param>
        /// <returns></returns>
        public static byte GetLSB(this UInt16 data)
        {
            return (byte)(data & 0x00FF);
        }

        /// <summary>
        /// Create 16 bit word from it's bytes
        /// </summary>
        /// <param name="msb">8 MSB of word</param>
        /// <param name="lsb">8 LSB of word</param>
        /// <returns>16 bit unsigned int constructed from bytes</returns>
        public static UInt16 ConstructWordFromBytes(byte msb, byte lsb)
        {
            return (UInt16)((msb << 8) | lsb);
        }

        /// <summary>
        /// Create 32 bit word from a variable number of bytes (3-4)
        /// </summary>
        /// <param name="bytes">input values; lsb to msb, 3-4 values</param>
        /// <returns>32 bit unsigned int constructed from bytes</returns>
        public static uint ConstructWordFromBytes(params byte[] bytes)
        {
            if (bytes.Length < 3 || bytes.Length > 4)
            {
                throw new ArgumentException("ConstructWordFromBytes requires 3 to 4 bytes.");
            }

            uint result = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                result |= (uint)(bytes[i] << (8 * i));
            }

            return result;
        }
    }
}
