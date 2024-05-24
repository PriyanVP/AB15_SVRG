using AB15_GUI.WPF.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;
using NLog;
using NLog.Targets;
using System;
using System.Reflection;
using NLog.Config;
using System.Xml.Linq;
using AB15_GUI.WPF.NLog;

namespace AB15_GUI.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Getting Logger reference with custom configuration
        /// </summary>
        private static Logger Logger = LogManager.Setup()
                                                 .SetupExtensions(ext => ext.RegisterLayoutRenderer<BuildConfigurationLayoutRenderer>("build-configuration"))
                                                 .SetupExtensions(ext => ext.RegisterTarget<LogMemoryRecordtTarget>("MemoryRecord"))
                                                 .GetCurrentClassLogger();

        public App()
        {
            Logger.Info("Starting application.");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Logger.Info("App startup event");
            MainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel()
            };
            MainWindow.Show();
            
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Logger.Info("App exit event");
            LogManager.Shutdown();
            base.OnExit(e);
        }
    }
}
