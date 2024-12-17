using System;
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

            ReadingCheckBox.IsChecked = true;
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
    }
}
