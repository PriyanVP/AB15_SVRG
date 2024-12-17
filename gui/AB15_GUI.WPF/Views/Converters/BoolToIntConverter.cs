using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AB15_GUI.WPF.Views.Converters
{    
    /// <summary>
    /// Convert bool value to color
    /// </summary>
    public class BoolToIntConverter : IValueConverter
    {
        /// <summary>
        /// Based on input values convert bool value to color.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool) value;

            return (boolValue ? 1 : 0);
        }

        /// <summary>
        /// Unused method
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = (int) value;
            
            return (intValue != 0);
        }
    }
}
