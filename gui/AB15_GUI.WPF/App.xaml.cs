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
using AB15_GUI.WPF.Views;
using AB15_GUI.WPF.ViewModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog.Fluent;

namespace AB15_GUI.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml. Application is hosted here
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Host for application. Handles object initializations, passing required parameters to constructors
        /// </summary>
        public static IHost? AppHost { get; private set; }

        /// <summary>
        /// Logger reference with custom configuration
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// Application constructor. First method that will run at GUI startup
        /// </summary>
        public App()
        {
            // Application container initialization. All ViewModels. services should be present here
            // Please add them to corresponding regions for maintainability
            AppHost = Host.CreateDefaultBuilder()
                        .ConfigureServices((host, services) => 
                        {
                            #region ViewModels

                            services.AddSingleton<MainViewModel>();
                            services.AddSingleton<LoggerViewModel>();

                            #endregion // ViewModels

                            #region Views

                            services.AddSingleton<MainView>(sp =>
                            {
                                MainView mainWindow = new MainView();
                                mainWindow.DataContext = sp.GetRequiredService<MainViewModel>();
                                return mainWindow;
                            });

                            services.AddSingleton<LoggerView>(sp =>
                            {
                                LoggerView tmpWindow = new LoggerView();
                                tmpWindow.DataContext = sp.GetRequiredService<LoggerViewModel>();
                                return tmpWindow;
                            });

                            #endregion // Views

                            #region Other

                            services.AddTransient<Logger>(sp => LogManager.Setup()
                                                 .SetupExtensions(ext => ext.RegisterLayoutRenderer<BuildConfigurationLayoutRenderer>("build-configuration"))
                                                 .SetupExtensions(ext => ext.RegisterTarget<LogMemoryRecordTarget>("MemoryRecord"))
                                                 .GetCurrentClassLogger()); // Same logger will be used across all classes - instance always created in App
                            services.AddSingleton<LogMemoryRecordTarget>(sp => (LogMemoryRecordTarget)LogManager.Configuration.FindTargetByName("memory"));

                            #endregion // Other
                        })
                        .Build();
            
            this.logger = AppHost.Services.GetRequiredService<Logger>();
            logger.Trace("Starting application.");

            // Subscribe to the UnhandledException event
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// Application startup event handler. Ocuurs after constructor
        /// </summary>
        /// <param name="e">startup events</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            MainView startupForm = AppHost.Services.GetRequiredService<MainView>();
            startupForm.Show();

            base.OnStartup(e);
        }

        /// <summary>
        /// Application exit event handler
        /// </summary>
        /// <param name="e">exit event arguments</param>
        protected override async void OnExit(ExitEventArgs e)
        {
            logger.Trace("App exit event");

            await AppHost!.StopAsync();
            AppHost?.Dispose();

            LogManager.Shutdown();
            base.OnExit(e);
        }

        /// <summary>
        /// Event for that processes unhandled exceptions
        /// Mainly used for logging purposes
        /// </summary>
        /// <param name="sender">object that called event</param>
        /// <param name="ex">exception event arguments</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs ex)
        {
            logger.Fatal((Exception)ex.ExceptionObject, "Unhandled exception!");
            LogManager.Shutdown();
        }
    }
}
