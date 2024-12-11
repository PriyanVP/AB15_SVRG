using AB15_GUI.WPF.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AB15_GUI.WPF.Views.Converters
{
    /// <summary>
    /// Convert bool value to color
    /// </summary>
    public class TextToBitConverter : IValueConverter
    {
        /// <summary>
        /// Based on input values convert bool value to color.
        /// Multivalue converter is used to also update colors in case theme changed (resources will change underneath)
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string inputString || string.IsNullOrWhiteSpace(inputString))
                return "0";

            try
            {
                // Parse the input as a hexadecimal value
                int number = int.Parse(inputString, System.Globalization.NumberStyles.HexNumber);

                if (parameter is not string bitParameter || !int.TryParse(bitParameter, out int bitPosition) || bitPosition < 0)
                    return "0";

                return (number & (1 << bitPosition)) != 0 ? "1" : "0";
            }
            catch
            {
                return "0";
            }
        }

        /// <summary>
        /// Unused method
        /// </summary>
        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
