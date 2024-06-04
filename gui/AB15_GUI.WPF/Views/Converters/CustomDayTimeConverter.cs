using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AB15_GUI.WPF.Views.Converters
{
    /// <summary>
    /// Convert date time style format
    /// </summary>
    public class CustomDayTimeConverter : IMultiValueConverter
    {
        /// <summary>
        /// Based on input values change output string format of day and time
        /// </summary>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime;
            bool IsChecked;

            // Raw DateTime value
            dateTime = (DateTime)values[0];

            // Flag to chnage format
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
        /// ToDo
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
