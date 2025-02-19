using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        }

        private void MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Skip if not double click
            if (e.LeftButton != MouseButtonState.Pressed || e.ClickCount != 2) return;
            
            try
            {
                Clipboard.SetDataObject(((TextBlock)sender).Text);
            }
            catch (Exception)
            {
                // In case of exception do not copy to clipboard
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
        
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9a-fA-F]+$");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
