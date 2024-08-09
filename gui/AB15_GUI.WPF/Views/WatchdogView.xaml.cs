using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AB15_GUI.WPF.Views
{
    /// <summary>
    /// Interaction logic for WatchdogView.xaml
    /// </summary>
    public partial class WatchdogView : Page
    {
        /// <summary>
        /// initial size of status box
        /// </summary>
        private double initialSize = 0;

        /// <summary>
        /// initial fontsize of status box
        /// </summary>
        private double fontsizeOld = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public WatchdogView()
        {
            InitializeComponent();

            // inital value TODO: why here, can be done in XAML
            MoreInfoButton.IsChecked = true;
        }

        // TODO: check if can be moved to Slider back itself (all 4 slider events)
        /// <summary>
        /// Backend of WD1 response - lock time Slider dependency 
        /// </summary>
        private void WD1LockTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // check for null, can occurs during start up
            if (WD1ResponseTimeSlider == null || WD1LockTimeSlider == null) { return; }

            // check if sliders sum is greater then 63
            if ((WD1ResponseTimeSlider.Value + WD1LockTimeSlider.Value) > 63)
            {
                // set new value for response time slider
                WD1ResponseTimeSlider.Value = 63 - WD1LockTimeSlider.Value;
            }

            // change min and max value for response time slider
            WD1ResponseTimeSlider.Minimum = -1 * WD1LockTimeSlider.Value;
            WD1ResponseTimeSlider.Maximum = 63 - WD1LockTimeSlider.Value;
        }

        /// <summary>
        /// Backend of WD1 response - lock time Slider dependency 
        /// </summary>
        private void WD1ResponseTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // check for null, can occurs during start up
            if (WD1ResponseTimeSlider == null || WD1LockTimeSlider == null) { return; }
            
            // safety check for values less then 0
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
            // check for null, can occurs during start up
            if (WD2ResponseTimeSlider == null || WD2LockTimeSlider == null) { return; }

            // check if sliders sum is greater then 63
            if ((WD2ResponseTimeSlider.Value + WD2LockTimeSlider.Value) > 63)
            {
                // set new value for response time slider
                WD2ResponseTimeSlider.Value = 63 - WD2LockTimeSlider.Value;
            }

            // change min and max value for response time slider
            WD2ResponseTimeSlider.Minimum = -1 * WD2LockTimeSlider.Value;
            WD2ResponseTimeSlider.Maximum = 63 - WD2LockTimeSlider.Value;
        }

        /// <summary>
        /// Backend of WD2 response - lock time Slider dependency 
        /// </summary>
        private void WD2ResponseTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // check for null, can occurs during start up
            if (WD2ResponseTimeSlider == null || WD2LockTimeSlider == null) { return; }

            // safety check for values less then 0
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
        /// clear focuses from slider
        /// </summary>
        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        /// <summary>
        /// clear focuses from slider
        /// </summary>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        /// <summary>
        /// Calculate new font size for status box
        /// </summary>
        private void TextScale_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // check for first time function call
            if (initialSize == 0)
            {
                // set initial values
                initialSize = TextScaleBorder.ActualWidth;
                fontsizeOld = TextScale.FontSize;
                return;
            }

            // calculate fontsize
            double newFontSize = (fontsizeOld * TextScaleBorder.ActualWidth) / initialSize;

            // check for max fontsize, if fontsize too big it will look funny
            if (newFontSize >= 30)
            {
                TextScale.FontSize = 30;
            }
            else
            {
                TextScale.FontSize = newFontSize;
            }

        }
    }
}
