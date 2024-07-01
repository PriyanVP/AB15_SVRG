using AB15_GUI.WPF.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AB15_GUI.WPF.Views.Converters
{
    /// <summary>
    /// Convert status of connection to color
    /// </summary>
    public class StatusToColorConverter : IMultiValueConverter
    {
        /// <summary>
        /// Based on input values convert status to color.
        /// Multivalue converter is used to also update colors in case theme changed (resources will change underneath)
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            UIConnectionStatus status = (UIConnectionStatus)values[0];

            switch (status)
            {
                case UIConnectionStatus.NotConnected:
                    return (Brush)App.Current.Resources["StatusBox.NotConnected"];
                case UIConnectionStatus.Connected:
                    return (Brush)App.Current.Resources["StatusBox.Connected"];
                case UIConnectionStatus.Warning:
                    return (Brush)App.Current.Resources["StatusBox.Warning"];
                case UIConnectionStatus.Error:
                    return (Brush)App.Current.Resources["StatusBox.Error"];
                default:
                    return Brushes.Transparent;
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
