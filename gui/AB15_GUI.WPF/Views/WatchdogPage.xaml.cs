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
    /// Interaction logic for WatchDog.xaml
    /// </summary>
    public partial class WatchDog : Page
    {
        public WatchDog()
        {
            InitializeComponent();
        }

        private void WD1LockTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WD1ResponseTimeSlider == null || WD1LockTimeSlider == null) { return; }


            if ((WD1ResponseTimeSlider.Value + WD1LockTimeSlider.Value) > 63)
            {
                WD1ResponseTimeSlider.Value = 63 - WD1LockTimeSlider.Value;
            }

            WD1ResponseTimeSlider.Minimum = -1 * WD1LockTimeSlider.Value;
            WD1ResponseTimeSlider.Maximum = 63 - WD1LockTimeSlider.Value;
        }

        private void WD1ResponseTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WD1ResponseTimeSlider == null || WD1LockTimeSlider == null) { return; }
            
            if (WD1ResponseTimeSlider.Value < 0 ) 
            {
                WD1ResponseTimeSlider.Value = 0;
            }

            if ((WD1ResponseTimeSlider.Value + WD1LockTimeSlider.Value) > 63)
            {
                WD1ResponseTimeSlider.Value = 63 - WD1LockTimeSlider.Value;
            }
        }






        private void WD2LockTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WD2ResponseTimeSlider == null || WD2LockTimeSlider == null) { return; }


            if ((WD2ResponseTimeSlider.Value + WD2LockTimeSlider.Value) > 63)
            {
                WD2ResponseTimeSlider.Value = 63 - WD2LockTimeSlider.Value;
            }

            WD2ResponseTimeSlider.Minimum = -1 * WD2LockTimeSlider.Value;
            WD2ResponseTimeSlider.Maximum = 63 - WD2LockTimeSlider.Value;
        }

        private void WD2ResponseTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WD2ResponseTimeSlider == null || WD2LockTimeSlider == null) { return; }

            if (WD2ResponseTimeSlider.Value < 0)
            {
                WD2ResponseTimeSlider.Value = 0;
            }

            if ((WD2ResponseTimeSlider.Value + WD2LockTimeSlider.Value) > 63)
            {
                WD2ResponseTimeSlider.Value = 63 - WD2LockTimeSlider.Value;
            }
        }

        private void WD1EN0DisableTreshholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WD2ResponseTimeSlider == null || WD2LockTimeSlider == null) { return; }


            if ((WD2ResponseTimeSlider.Value + WD2LockTimeSlider.Value) > 63)
            {
                WD2ResponseTimeSlider.Value = 63 - WD2LockTimeSlider.Value;
            }

            WD2ResponseTimeSlider.Minimum = -1 * WD2LockTimeSlider.Value;
            WD2ResponseTimeSlider.Maximum = 63 - WD2LockTimeSlider.Value;
        }



        private void WD2EN0DisableTreshholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WD2ResponseTimeSlider == null || WD2LockTimeSlider == null) { return; }

            if (WD2ResponseTimeSlider.Value < 0)
            {
                WD2ResponseTimeSlider.Value = 0;
            }

            if ((WD2ResponseTimeSlider.Value + WD2LockTimeSlider.Value) > 63)
            {
                WD2ResponseTimeSlider.Value = 63 - WD2LockTimeSlider.Value;
            }
        }
    }
}
