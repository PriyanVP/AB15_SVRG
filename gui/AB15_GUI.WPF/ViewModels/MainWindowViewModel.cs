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

    public class MainViewModel : ViewModelBase
    {

        private bool _isLightTheme = true;

        public bool IsLightTheme
        {
            get => _isLightTheme;
            set
            {
                _isLightTheme = value;
                OnPropertyChanged();
            }
        }

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

        private readonly Logger logger;

        public readonly LoggerViewModel LoggerViewModel;

        public MainViewModel(Logger logger, LoggerViewModel loggerViewModel, LoggerView loggerWindowView)
        {
            this.logger = logger;
            LoggerViewModel = loggerViewModel;

            logger.Trace("In MainViewModel");
            loggerWindowView.Show();

            PCCurrentStatus = UIConnectionStatus.Warning;
            ABCurrentStatus = UIConnectionStatus.Error;
            MCUCurrentStatus = UIConnectionStatus.Connected;
        }
    }
}
