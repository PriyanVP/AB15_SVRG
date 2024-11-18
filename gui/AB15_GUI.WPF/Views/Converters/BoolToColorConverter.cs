using AB15_GUI.WPF.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AB15_GUI.WPF.Views.Converters
{
    /// <summary>
    /// Convert bool value to color
    /// </summary>
    public class BoolToColorConverter : IMultiValueConverter
    {
        /// <summary>
        /// Based on input values convert bool value to color.
        /// Multivalue converter is used to also update colors in case theme changed (resources will change underneath)
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            bool value = (bool)values[0];

            switch (value)
            {
                case true:
                    return (Brush)App.Current.Resources["Foreground.Error"];
                case false:
                    return (Brush)App.Current.Resources["Foreground.Info"];
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
