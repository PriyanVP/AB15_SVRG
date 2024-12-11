using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
