using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace AB15_GUI.WPF.Views.Converters
{
    // TODO: connect to XAML in WatchdogPage
    /// <summary>
    /// Provides help messages for bindable properties and UI elements from ViewModel storage
    /// </summary>
    class HelpMessageConverter : IValueConverter
    {
        /// <summary>
        /// Provide help message from ViewModel storage based on parameters
        /// </summary>
        /// <param name="value">dictionary with help messages</param>
        /// <param name="targetType">not used</param>
        /// <param name="parameter">string with comma delimited list of dictionary keys. Ex.: key1, key2</param>
        /// <param name="culture">not used</param>
        /// <returns>Help message string</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<string, string> helpMsgDictionary = (Dictionary<string, string>) value;
            string rawKeysString = (string)parameter;
            string helpMessage = string.Empty;
            string[] keys = rawKeysString.Split(", ");

            // Concatenate help messages
            foreach (string key in keys)
            {
                helpMessage += $"{helpMsgDictionary.GetValueOrDefault(key.Trim())}{Environment.NewLine}";
            }

            // Remove last newline
            helpMessage = helpMessage.Substring(0, helpMessage.LastIndexOf(Environment.NewLine));

            return helpMessage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
