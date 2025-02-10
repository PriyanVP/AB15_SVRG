using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AB15_GUI.WPF.Views
{
    /// <summary>
    /// Interaction logic for SPILearning.xaml
    /// </summary>
    public partial class SPILearning : Page
    {
        public SPILearning()
        {
            InitializeComponent();

            ReadingCheckBox.IsChecked = true;

            AddressTextBox.Text = "0";
            DataTextBox.Text = "0";
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Name == ReadingCheckBox.Name)
            {
                WritingCheckBox.IsChecked = !checkBox.IsChecked;
            }
            else
            {
                ReadingCheckBox.IsChecked = !checkBox.IsChecked;
            }
        }

        private void MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Skip if not double click
            if (e.LeftButton != MouseButtonState.Pressed || e.ClickCount != 2) return;
            
            try
            {
                Clipboard.SetText(((TextBox)sender).Text);
            }
            catch (Exception ex)
            {
                // Do something w/ exception 
            }
        }

        private void isRawFormat_Checked(object sender, RoutedEventArgs e)
        {
            ReadWriteChooseField.Visibility = Visibility.Hidden;
            RawFormatTextBox.Visibility = Visibility.Visible;
            RawHexText.Visibility = Visibility.Visible;
        }

        private void isRawFormat_Unchecked(object sender, RoutedEventArgs e)
        {
            ReadWriteChooseField.Visibility = Visibility.Visible;
            RawFormatTextBox.Visibility = Visibility.Hidden;
            RawHexText.Visibility = Visibility.Hidden;
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

        private void HexAddressTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
            if (TryParseHex(newText, out uint parsedValue) && parsedValue <= 0x3f0)
            {
                textBox.Text = newText;
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (parsedValue > 0x3f0)
            {
                ShowPopupMessage(textBox, "Invalid value! Max 0x3F0!");
                e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private void HexDataTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
            if (TryParseHex(newText, out uint parsedValue) && parsedValue <= 0xffff)
            {
                textBox.Text = newText;
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (parsedValue > 0xffff)
            {
                ShowPopupMessage(textBox, "Invalid value! Max 0x3F0!");
                e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private void HexFullTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

            if (newText.Length > 8)
            {
                ShowPopupMessage(textBox, "Invalid length! Max 8 symbols!");
                e.Handled = true;
                return;
            }


            // Ensure new text does not exceed MAXVALUE
            if (TryParseHex(newText, out uint parsedValue) && parsedValue <= uint.MaxValue)
            {
                textBox.Text = newText;
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (parsedValue > uint.MaxValue)
            {
                ShowPopupMessage(textBox, "Invalid value! Max UINT!");
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
