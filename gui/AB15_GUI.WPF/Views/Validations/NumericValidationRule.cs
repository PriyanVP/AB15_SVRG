using System;
using System.Globalization;
using System.Windows.Controls;

namespace AB15_GUI.WPF.Views.Validations
{
    /// <summary>
    /// Check if input value is numeric and in expected range
    /// </summary>
    public class NumericValidationRule : ValidationRule
    {
        /// <summary>
        /// Minimum value
        /// </summary>
        public int MinValue { get; set; } = int.MinValue;

        /// <summary>
        /// Maximum value
        /// </summary>
        public int MaxValue { get; set; } = int.MaxValue;

        /// <summary>
        /// Check if input is valid and in expected range
        /// </summary>
        /// <param name="value">input data</param>
        /// <param name="cultureInfo">information about current culture</param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool isConvertedSuccessfully = false;
            int intVal;
            string? strValue = Convert.ToString(value);
            isConvertedSuccessfully = int.TryParse(strValue, out intVal);

            if (!isConvertedSuccessfully)
            {
                return new ValidationResult(false, $"Value cannot be coverted to int.");
            }

            if ((intVal >= MinValue) && (intVal <= MaxValue))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, $"Input is out of expected range ([{MinValue}; {MaxValue}])");
            }
        }
    }
}
