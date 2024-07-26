using AB15_GUI.WPF.Views;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainView : Window
    {
        /// <summary>
        /// Constructor for main window
        /// </summary>
        public MainView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sitch betwen tabs from menu buttons
        /// </summary>
        /// <param name="sender">Button name</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name) 
            {
                case "HomeButton":
                    PagesControl.SelectedIndex = 0;
                    break;
                case "LearningButton":
                    PagesControl.SelectedIndex = 1;
                    break;
                case "WatchdogButton":
                    PagesControl.SelectedIndex = 2;
                    break;
                case "FiringButton":
                    PagesControl.SelectedIndex = 3;
                    break;
                case "AB15DataButton":
                    PagesControl.SelectedIndex = 4;
                    break;
                case "SensorsButton":
                    PagesControl.SelectedIndex = 5;
                    break;
                case "DiagnosticsButton":
                    PagesControl.SelectedIndex = 6;
                    break;
                default:
                    PagesControl.SelectedIndex = 0;
                    break;
            }

        }

        // TODO find better way
        /// <summary>
        /// Set data context for watchdog page
        /// </summary>
        private void WatchdogFrame_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (WatchdogFrame.Content is FrameworkElement content)
            {
                content.DataContext = WatchdogFrame.DataContext;
            }
        }
    }
}