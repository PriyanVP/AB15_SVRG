using System.Windows;
using System.Windows.Controls.Primitives;

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
            // open page based on clicked button name
            switch (((ToggleButton)sender).Name) 
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

            // go for each menu checkbox and uncheck old one, check clicked one
            for (int i = 0; i < MenuPanel.Children.Count; i++)
            {
                // check if element ToggleButton
                if (MenuPanel.Children[i] is ToggleButton)
                {
                    ToggleButton toggleButton = (ToggleButton)MenuPanel.Children[i];

                    // check for clicked ToggleButton name
                    if (toggleButton.Name == ((ToggleButton)sender).Name)
                    {
                        toggleButton.IsChecked = true;
                    }
                    else if (toggleButton.Name != "") // check for menu ToggleButton name
                    {
                        toggleButton.IsChecked = false;
                    }
                }
            }

        }
    }
}