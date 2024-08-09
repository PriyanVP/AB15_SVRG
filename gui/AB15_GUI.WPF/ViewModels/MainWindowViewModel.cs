using AB15_GUI.WPF.Views;
using NLog;
using System;

namespace AB15_GUI.WPF.ViewModels
{
    /// <summary>
    /// Connection status value definition 
    /// </summary>
    public enum UIConnectionStatus
    {
        NotConnected,
        Connected,
        Warning,
        Error
    }

    /// <summary>
    /// View Model for Main Window
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// PC status for view
        /// </summary>
        private UIConnectionStatus connectionStatusPC;
        public UIConnectionStatus ConnectionStatusPC
        {
            get => connectionStatusPC;
            set
            {
                if (connectionStatusPC != value)
                {
                    connectionStatusPC = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// MCU status for view
        /// </summary>
        private UIConnectionStatus connectionStatusMCU;
        public UIConnectionStatus ConnectionStatusMCU
        {
            get => connectionStatusMCU;
            set
            {
                if (connectionStatusMCU != value)
                {
                    connectionStatusMCU = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// AB status for view
        /// </summary>
        private UIConnectionStatus connectionStatusAB;
        public UIConnectionStatus ConnectionStatusAB
        {
            get => connectionStatusAB;
            set
            {
                if (connectionStatusAB != value)
                {
                    connectionStatusAB = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Local logger instance
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// Logger window instance
        /// </summary>
        public LoggerViewModel LoggerViewModel { get; private set; }

        /// <summary>
        /// Watchdog page instance
        /// </summary>
        public WatchdogViewModel WatchdogViewModel { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel(Logger logger, LoggerViewModel loggerViewModel, LoggerView loggerWindowView, WatchdogViewModel watchdogViewModel)
        {
            // Init Logger and logger view model
            this.logger = logger;

            // Store references to child view models
            LoggerViewModel = loggerViewModel;
            WatchdogViewModel = watchdogViewModel;

            logger.Trace("In MainViewModel");
            loggerWindowView.Show();       
        }
    }
}
