using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using AB15_GUI.WPF.ViewModels;

namespace AB15_GUI.WPF.Views.Converters
{
    /// <summary>
    /// 
    /// </summary>
    class StatusColorConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            UIConnectionStatus status = (UIConnectionStatus)value;

            switch (status)
            {
                case UIConnectionStatus.NotConnected:
                    return Brushes.Transparent;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
