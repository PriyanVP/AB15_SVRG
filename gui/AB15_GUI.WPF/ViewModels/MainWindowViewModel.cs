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
    /// TODO define proper place
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
        private UIConnectionStatus _PCconnectionStatus;
        public UIConnectionStatus PCCurrentStatus
        {
            get => _PCconnectionStatus;
            set
            {
                if (_PCconnectionStatus != value)
                {
                    _PCconnectionStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// MCU status for view
        /// </summary>
        private UIConnectionStatus _MCUconnectionStatus;
        public UIConnectionStatus MCUCurrentStatus
        {
            get => _MCUconnectionStatus;
            set
            {
                if (_MCUconnectionStatus != value)
                {
                    _MCUconnectionStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// AB status for view
        /// </summary>
        private UIConnectionStatus _ABconnectionStatus;
        public UIConnectionStatus ABCurrentStatus
        {
            get => _ABconnectionStatus;
            set
            {
                if (_ABconnectionStatus != value)
                {
                    _ABconnectionStatus = value;
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
