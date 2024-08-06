using AB15_GUI.WPF.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AB15_GUI.WPF.Views.Converters
{
    /// <summary>
    /// Convert input register value to real time value
    /// </summary>
    public class WD1SliderToTextBoxValueConverter : IValueConverter
    {
        /// <summary>
        /// Based on input values convert value for wd1
        /// Multivalue converter is used to also update colors in case theme changed (resources will change underneath)
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double sliderValue)
            {
                return (sliderValue * 50).ToString();
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
