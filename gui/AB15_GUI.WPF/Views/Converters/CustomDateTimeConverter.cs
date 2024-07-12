using System;
using System.Globalization;
using System.Windows.Data;

namespace AB15_GUI.WPF.Views.Converters
{
    /// <summary>
    /// Convert date+time to short or long format
    /// </summary>
    public class CustomDateTimeConverter : IMultiValueConverter
    {
        /// <summary>
        /// Based on input values convert date+time to long/short string
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime;
            bool IsChecked;

            // Raw DateTime value
            dateTime = (DateTime)values[0];

            // Flag to change format
            IsChecked = (bool)values[1];

            // If flag true - format not changed
            if (IsChecked == true)
            {
                // Long format
                return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            }
            else
            {
                // Short format
                return dateTime.ToString("HH:mm:ss");
            }
        }

        /// <summary>
        /// Unused method
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
