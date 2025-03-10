using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

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

        private Expander expander;

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

        private void Button_Undock_Click(object sender, RoutedEventArgs e)
        {
            if ((expander = FindName("LoggerExpander") as Expander) != null)
            {
                try
                {
                    var loggerWindowName = "Logger Window";
                    var loggerWindowIcon = new BitmapImage(new Uri("pack://application:,,,/AB15_GUI.WPF;component/Views/Resources/bosch.ico"));

                    expander.Visibility = Visibility.Collapsed;
                    FrameworkElement dockedWindowContent = (FrameworkElement)expander.Content;
                    expander.Content = null;

                    Window window = new()
                    {
                        Content = dockedWindowContent,
                        Icon = loggerWindowIcon,
                        Tag = expander,
                        Name = expander.Name,
                        Title = loggerWindowName,
                        Width = 800,
                        Height = 450,
                        MinWidth = 700,
                        MinHeight = 300
                    };

                    window.Closing += (s, e) =>
                    {
                        Window windowToClose = (Window)s;
                        Expander expanderItem = (Expander)windowToClose.Tag;

                        expanderItem.Content = windowToClose.Content;
                        expanderItem.Visibility = Visibility.Visible;
                    };

                    window.Show();
                }
                catch (Exception)
                {
                    // Protect the app of crashing by just not undocking the window
                }
            }
        }
    }
}