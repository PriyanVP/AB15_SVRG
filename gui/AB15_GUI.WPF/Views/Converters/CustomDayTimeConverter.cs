using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AB15_GUI.WPF.Views.Converters
{
    public class CustomDayTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime;
            bool IsChecked;

            dateTime = (DateTime)values[0];
            IsChecked = (bool)values[1];

            if (IsChecked == false)
            {
                return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            }
            else
            {
                return dateTime.ToString("HH:mm:ss");
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
