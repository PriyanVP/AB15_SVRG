using AB15_GUI.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AB15_GUI.WPF.Views.Converters
{
    /// <summary>
    /// Firing mode index to field value mapping
    /// </summary>
    public class FiringModeConverter : IValueConverter
    {
        /// <summary>
        /// Dictionary to map firing mode index in UI dropdown to corresponding code from register field
        /// Key - index in dropdown, Value - code for register
        /// Note: should match regmap and mode in UI
        /// </summary>
        private readonly Dictionary<int, UInt16> _modeToIntMapping = new Dictionary<int, UInt16>
        {               
            { 0,    0},     /*    OFF       */
            { 1,    2},     /*    Mode 1    */
            { 2,    3},     /*    Mode 1a   */
            { 3,    4},     /*    Mode 2    */
            { 4,    5},     /*    Mode 2a   */
            { 5,    6},     /*    Mode 3    */
            { 6,    8},     /*    Mode 4    */
            { 7,    9},     /*    Mode 4a   */
            { 8,    10},    /*    Mode 5    */
            { 9,    11},    /*    Mode 5a   */
            { 10,   12},    /*    Mode 6    */
            { 11,   13},    /*    Mode 6a   */
            { 12,   14},    /*    Mode 7    */
            { 13,   15},    /*    Mode 7a   */
            { 14,   16},    /*    Mode 8    */
            { 15,   17}     /*    Mode 9    */
        };

        /// <summary>
        /// Convert firing mode index in dropdown list to corresponding field value in regmap
        /// </summary>
        /// <param name="value">input value - text of dropdown list item</param>
        /// <returns>Scaled input value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _modeToIntMapping.GetValueOrDefault(int value, 0);
        }

        /// <summary>
        /// Convert back field value of firing mode in regmap to firing mode index in dropdown list
        /// </summary>
        /// <param name="value">input value from back - int corresponding to mode</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UInt16 modeFieldValue = (UInt16) value;
            return _modeToIntMapping.FirstOrDefault(x => x.Value == modeFieldValue).Key;
        }
    }
}
