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
    public partial class WatchDogView : Page
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WatchDogView()
        {
            InitializeComponent();

            // TODO debug code
            MoreInfoButton.IsChecked = true;
        }

        /// <summary>
        /// Backend of WD1 response - lock time Slider dependency 
        /// </summary>
        private void WD1LockTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // check for null, can occure durind start up
            if (WD1ResponseTimeSlider == null || WD1LockTimeSlider == null) { return; }

            // check if sliders sum is greater then 63
            if ((WD1ResponseTimeSlider.Value + WD1LockTimeSlider.Value) > 63)
            {
                // set new value for response time slider
                WD1ResponseTimeSlider.Value = 63 - WD1LockTimeSlider.Value;
            }

            // change min and max value for responce time slider
            WD1ResponseTimeSlider.Minimum = -1 * WD1LockTimeSlider.Value;
            WD1ResponseTimeSlider.Maximum = 63 - WD1LockTimeSlider.Value;
        }

        /// <summary>
        /// Backend of WD1 response - lock time Slider dependency 
        /// </summary>
        private void WD1ResponseTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // check for null, can occure durind start up
            if (WD1ResponseTimeSlider == null || WD1LockTimeSlider == null) { return; }
            
            // safty check for values less then 0
            if (WD1ResponseTimeSlider.Value < 0 ) 
            {
                WD1ResponseTimeSlider.Value = 0;
            }

            // check if sliders sum is greater then 63
            if ((WD1ResponseTimeSlider.Value + WD1LockTimeSlider.Value) > 63)
            {
                // set new value for response time slider
                WD1ResponseTimeSlider.Value = 63 - WD1LockTimeSlider.Value;
            }
        }

        /// <summary>
        /// Backend of WD2 response - lock time Slider dependency 
        /// </summary>
        private void WD2LockTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // check for null, can occure durind start up
            if (WD2ResponseTimeSlider == null || WD2LockTimeSlider == null) { return; }

            // check if sliders sum is greater then 63
            if ((WD2ResponseTimeSlider.Value + WD2LockTimeSlider.Value) > 63)
            {
                // set new value for response time slider
                WD2ResponseTimeSlider.Value = 63 - WD2LockTimeSlider.Value;
            }

            // change min and max value for responce time slider
            WD2ResponseTimeSlider.Minimum = -1 * WD2LockTimeSlider.Value;
            WD2ResponseTimeSlider.Maximum = 63 - WD2LockTimeSlider.Value;
        }

        /// <summary>
        /// Backend of WD2 response - lock time Slider dependency 
        /// </summary>
        private void WD2ResponseTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // check for null, can occure durind start up
            if (WD2ResponseTimeSlider == null || WD2LockTimeSlider == null) { return; }

            // safty check for values less then 0
            if (WD2ResponseTimeSlider.Value < 0)
            {
                WD2ResponseTimeSlider.Value = 0;
            }

            // check if sliders sum is greater then 63
            if ((WD2ResponseTimeSlider.Value + WD2LockTimeSlider.Value) > 63)
            {
                // set new value for response time slider
                WD2ResponseTimeSlider.Value = 63 - WD2LockTimeSlider.Value;
            }
        }

        /// <summary>
        /// clear focuse from slider
        /// </summary>
        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        /// <summary>
        /// clear focuse from slider
        /// </summary>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }
    }
}
