using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Services.Interfaces;
using AB15_GUI.WPF.Views;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

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
        private UIConnectionStatus pcConnectionStatus;
        public UIConnectionStatus pcStatus
        {
            get => pcConnectionStatus;
            set
            {
                if (pcConnectionStatus != value)
                {
                    pcConnectionStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// MCU status for view
        /// </summary>
        private UIConnectionStatus mcuCnnectionStatus;
        public UIConnectionStatus mcuStatus
        {
            get => mcuCnnectionStatus;
            set
            {
                if (mcuCnnectionStatus != value)
                {
                    mcuCnnectionStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// AB status for view
        /// </summary>
        private UIConnectionStatus abConnectionStatus;
        public UIConnectionStatus abStatus
        {
            get => abConnectionStatus;
            set
            {
                if (abConnectionStatus != value)
                {
                    abConnectionStatus = value;
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

        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel(Logger logger, LoggerViewModel loggerViewModel, LoggerView loggerWindowView)
        {
            // Init Logger and logger view model
            this.logger = logger;
            LoggerViewModel = loggerViewModel;

            logger.Trace("In MainViewModel");
            loggerWindowView.Show();
        }
    }
}
