using System;
using System.Globalization;
using System.Windows.Data;

namespace AB15_GUI.WPF.Views.Converters
{    
    /// <summary>
    /// Convert hex value to int
    /// </summary>
    public class HexToIntConverter : IValueConverter
    {
        /// <summary>
        /// Based on input values convert int value to hex string.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToUInt32(value).ToString("X");
            }
            catch (Exception)
            {
                return value;
            }
        }

        /// <summary>
        /// Convert back hex string to int value.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return UInt32.Parse((string) value, System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}
