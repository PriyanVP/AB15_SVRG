using AB15_GUI.WPF.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AB15_GUI.WPF.Views.Converters
{
    /// <summary>
    /// Scale input value converter
    /// </summary>
    public class ScalingConverter : IValueConverter
    {       
        /// <summary>
        /// Multiply input value by scale factor
        /// </summary>
        /// <param name="value">input value</param>
        /// <param name="parameter">scale factor</param>
        /// <returns>Scaled input value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is double inputValue)
            {
                return (inputValue * double.Parse(parameter.ToString())).ToString();
            }
            return "0";
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
