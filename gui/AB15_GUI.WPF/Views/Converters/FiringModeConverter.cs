using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

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
            { 0,    0x0},     /*    OFF       */
            { 1,    0x2},     /*    Mode 1    */
            { 2,    0x3},     /*    Mode 1a   */
            { 3,    0x4},     /*    Mode 2    */
            { 4,    0x5},     /*    Mode 2a   */
            { 5,    0x6},     /*    Mode 3    */
            { 6,    0x8},     /*    Mode 4    */
            { 7,    0x9},     /*    Mode 4a   */
            { 8,    0xA},     /*    Mode 5    */
            { 9,    0xB},     /*    Mode 5a   */
            { 10,   0xC},     /*    Mode 6    */
            { 11,   0xD},     /*    Mode 6a   */
            { 12,   0xE},     /*    Mode 7    */
            { 13,   0xF},     /*    Mode 7a   */
            { 14,   0x10},    /*    Mode 8    */
            { 15,   0x11}     /*    Mode 9    */
        };

        /// <summary>
        /// Convert back field value of firing mode in regmap to firing mode index in dropdown list
        /// </summary>
        /// <param name="value">input value from back - int corresponding to mode</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UInt16 modeFieldValue = System.Convert.ToUInt16(value);
            return _modeToIntMapping.FirstOrDefault(x => x.Value == modeFieldValue).Key;
        }

        /// <summary>
        /// Convert firing mode index in dropdown list to corresponding field value in regmap
        /// </summary>
        /// <param name="value">input value - text of dropdown list item</param>
        /// <returns>Scaled input value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _modeToIntMapping.GetValueOrDefault((int) value); // Not converted correctly
        }
    }
}
