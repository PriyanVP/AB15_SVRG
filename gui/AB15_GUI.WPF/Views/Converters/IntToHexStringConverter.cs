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
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Typecast to actual types. Incorrect typecast is handled by try-catch
                uint inputNumber = (uint) value;
                int bitPosition = System.Convert.ToInt32(parameter);

                // Validate bit position range
                if (bitPosition < 0 || bitPosition > 31)
                    return "0";

                return (inputNumber & (1 << bitPosition)) != 0 ? "1" : "0";
            }
            catch
            {
                return "0";
            }
        }

        /// <summary>
        /// Unused method
        /// </summary>
        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
