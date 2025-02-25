using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AB15_GUI.WPF.Views.Converters
{    
    /// <summary>
    /// Convert bool value to R/W
    /// </summary>
    public class BoolToRWConverter : IValueConverter
    {
        /// <summary>
        /// Based on input values convert bool value to color.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool) value;

            return (boolValue ? "W" : "R");
        }

        /// <summary>
        /// Unused method
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
