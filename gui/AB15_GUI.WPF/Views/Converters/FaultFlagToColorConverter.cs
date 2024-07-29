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
    public class FaultFlagToColorConverter : IMultiValueConverter
    {
        /// <summary>
        /// Based on input values convert status to color.
        /// Multivalue converter is used to also update colors in case theme changed (resources will change underneath)
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            UIFaultStatus status = (UIFaultStatus)values[0];

            switch (status)
            {
                case UIFaultStatus.NoStatus:
                    return (Brush)App.Current.Resources["StatusBox.NoStatus"];
                case UIFaultStatus.Good:
                    return (Brush)App.Current.Resources["StatusBox.Good"];
                case UIFaultStatus.Fault:
                    return (Brush)App.Current.Resources["StatusBox.Fault"];
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
