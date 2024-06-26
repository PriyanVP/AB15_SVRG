using System;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.Views;
using AB15_GUI.WPF.ViewModels;
using AB15_GUI.WPF.Services;
using AB15_GUI.WPF.Services.Interfaces;
using System.Collections.Generic;
using NLog.Config;
using System.Configuration;

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
        /// Command line parameters
        /// </summary>
        private Dictionary<string, string> arguments; 

        /// <summary>
        /// Application constructor. First method that will run at GUI startup
        /// </summary>
        public App()
        {
            // Get and parse command line arguments
            arguments = GetCommandLineArguments();

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

                            #region Services

                            services.AddTransient<Logger>(sp => LogManager.Setup()
                                                 .SetupExtensions(ext => ext.RegisterLayoutRenderer<BuildConfigurationLayoutRenderer>("build-configuration"))
                                                 .SetupExtensions(ext => ext.RegisterTarget<LogMemoryRecordTarget>("MemoryRecord"))
                                                 .GetCurrentClassLogger()); // Same logger will be used across all classes - instance always created in App

                            services.AddSingleton<IWaitlist, Waitlist>();
                            services.AddSingleton<ISerialComm, SerialComm>();
                            services.AddSingleton<ISerialWrapper, SerialWrapper>();

                            #endregion // Services

                            #region Other

                            services.AddSingleton<LogMemoryRecordTarget>(sp => (LogMemoryRecordTarget)LogManager.Configuration.FindTargetByName("memory"));

                            #endregion // Other
                        })
                        .Build();

            this.logger = AppHost.Services.GetRequiredService<Logger>();

            // Overwrite logging levels for Logger regression testing
            if (arguments.ContainsKey("loggerTests"))
            {
                EnableAllLogLevels();
            }

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

        /// <summary>
        /// Get and parse command line arguments for application
        /// Command line arguments expected format: -key value
        /// </summary>
        /// <returns>Dictionary with command line arguments</returns>
        private Dictionary<string, string> GetCommandLineArguments()
        {
            arguments = new Dictionary<string, string>();
            string[] args = Environment.GetCommandLineArgs();

            // Store arguments to dictionary (element at 0 - name of the program)
            try
            {
                for (int index = 1; index < args.Length; index += 2)
                {
                    string arg = args[index].Replace("-", "");
                    arguments.Add(arg, args[index + 1]);
                }
            }
            catch (Exception ex)
            {
                // Do nothing as logger is not yet initialized
            }

            return arguments;
        }

        /// <summary>
        /// Overwrite all logging rules to support all levels
        /// Warning: at least one logger should be created before calling this method
        /// </summary>
        private void EnableAllLogLevels()
        {
            // Get reference to configuration (loaded from config file)
            LoggingConfiguration logConfig = LogManager.Configuration;

            // Modify all rules to log all levels
            foreach (LoggingRule rule in logConfig.LoggingRules)
            {
                rule.SetLoggingLevels(LogLevel.Trace, LogLevel.Fatal);
            }

            // Apply new configuration to all loggers
            LogManager.Configuration = logConfig;
        }
    }
}
