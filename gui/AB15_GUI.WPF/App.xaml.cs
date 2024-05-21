using System.Configuration;
using System.Data;
using System.Windows;

namespace AB15_GUI.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow();

            // TODO: databinding to MainWindowView

            MainWindow.Show();
            
            base.OnStartup(e);
        }
    }

}
