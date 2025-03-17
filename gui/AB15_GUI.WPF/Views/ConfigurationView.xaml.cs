using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AB15_GUI.WPF.Views
{
    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    public partial class ConfigurationView : Page
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }
        private async void ShowPopupMessage(TextBox textBox, string message)
        {
            ToolTip toolTip = new ToolTip
            {
                Content = message,
                PlacementTarget = textBox,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Right,
                IsOpen = true,
                Style = (Style)FindResource("ToolTipError")
            };

            textBox.ToolTip = toolTip;
            await Task.Delay(1000); // Show message for 1 second
            toolTip.IsOpen = false;
        }

        private void HexPsiConfigTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string input = e.Text.ToUpper();

            if (!Regex.IsMatch(input, "[0-9A-F]"))
            {
                ShowPopupMessage(textBox, "Invalid character! Use 0-9, A-F");
                e.Handled = true;
                return;
            }

            // Predict the new text if input is accepted
            string newText = textBox.Text.Insert(textBox.SelectionStart, input);

            if (newText.Length > 4)
            {
                ShowPopupMessage(textBox, "Invalid length! Max 4 symbols!");
                e.Handled = true;
                return;
            }

            // Ensure new text does not exceed MAXVALUE
            if (TryParseHex(newText, out uint parsedValue) && parsedValue <= ushort.MaxValue)
            {
                textBox.Text = newText;
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (parsedValue > ushort.MaxValue)
            {
                ShowPopupMessage(textBox, "Invalid value! Max 0xFFFF!");
                e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private void HexUartFrameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string input = e.Text.ToUpper();

            if (!Regex.IsMatch(input, "[0-9A-F]"))
            {
                ShowPopupMessage(textBox, "Invalid character! Use 0-9, A-F");
                e.Handled = true;
                return;
            }

            // Predict the new text if input is accepted
            string newText = textBox.Text.Insert(textBox.SelectionStart, input);

            if (newText.Length > 3)
            {
                ShowPopupMessage(textBox, "Invalid length! Max 3 symbols!");
                e.Handled = true;
                return;
            }

            // Ensure new text does not exceed MAXVALUE
            if (TryParseHex(newText, out uint parsedValue) && parsedValue <= 0x7FF)
            {
                textBox.Text = newText;
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (parsedValue > 0x7FF)
            {
                ShowPopupMessage(textBox, "Invalid value! Max 0x7FF!");
                e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private bool TryParseHex(string hexString, out uint result)
        {
            return uint.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
        }
    }
}
