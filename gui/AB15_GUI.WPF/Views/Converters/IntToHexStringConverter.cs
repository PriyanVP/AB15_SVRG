using System;
using System.Globalization;
using System.Windows.Data;

namespace AB15_GUI.WPF.Views.Converters
{
    /// <summary>
    /// Convert int number to hex string of specified length
    /// </summary>
    public class IntToHexStringConverter : IValueConverter
    {
        /// <summary>
        /// Based on input values convert int number to string in hex format (0x0)
        /// </summary>
        /// <param name="value">unsigned value of input</param>
        /// <param name="parameter">length of output string</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Typecast to actual types. Incorrect typecast is handled by try-catch
                ushort inputNumber = (ushort)value;
                int outputLength = (parameter != null) ? System.Convert.ToInt32(parameter) : 4; // defaults to 4

                // Validate bit position range
                if (outputLength < 1 || outputLength > 16)
                    return "0";

                return $"0x{inputNumber.ToString("X").PadLeft(outputLength, '0')}";
            }
            catch
            {
                return "Err";
            }
        }
       
        /// <summary>
        /// Convert hex string to int number
        /// </summary>
        /// <param name="value">string input as hex (0xXX or XX)</param>
        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            try
            {
                // Typecast to actual types. Incorrect typecast is handled by try-catch
                string hexString = value.ToString();
                if (string.IsNullOrEmpty(hexString))
                {
                    return 0;
                }

                // Remove "0x" prefix if present
                if (hexString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    hexString = hexString.Substring(2);
                }

                // Convert hex string to integer
                uint inputNumber = uint.Parse(hexString, System.Globalization.NumberStyles.HexNumber);

                return inputNumber;
            }
            catch
            {
                return 0;
            }
        }
    }
}
