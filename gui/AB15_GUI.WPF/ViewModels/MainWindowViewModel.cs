using AB15_GUI.WPF.Views;
using NLog;

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
        public readonly LoggerViewModel LoggerViewModel;

        public readonly WatchdogPageViewModel WatchdogPageViewModel;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel(Logger logger, LoggerViewModel loggerViewModel, LoggerView loggerWindowView, WatchdogPageViewModel watchdogPageViewModel)
        {
            // Init Logger and logger view model
            this.logger = logger;
            LoggerViewModel = loggerViewModel;

            logger.Trace("In MainViewModel");
            loggerWindowView.Show();

            WatchdogPageViewModel = watchdogPageViewModel;
        }
    }
}
