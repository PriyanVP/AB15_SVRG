using System;
using System.Globalization;
using System.Windows.Data;

namespace AB15_GUI.WPF.Views.Converters
{
    class ValueToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double sliderValue = (double)value;
            double sliderWidth = (double)parameter;

            return (sliderValue / 100) * sliderWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
