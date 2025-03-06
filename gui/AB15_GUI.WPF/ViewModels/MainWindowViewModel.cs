using AB15_GUI.WPF.ViewModels.Commands;
using AB15_GUI.WPF.Views;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;
using System.Collections.Generic;
using AB15_GUI.WPF.Services.Interfaces;
using System;
using System.Windows.Input;
using AB15_GUI.WPF.Services;
using System.Linq;

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
        /// The list of available comm ports
        /// </summary>
        private List<string> availableCommPorts;
        public List<string> AvailableCommPorts
        {
            get => availableCommPorts;
            set
            {
                availableCommPorts = value;
                OnPropertyChanged();

                if ( (availableCommPorts != null)
                  && (availableCommPorts.Count > 0))
                {
                    CommPortAvaiable = true;

                    if (string.IsNullOrEmpty(SelectedCommPort))
                    {
                        SelectedCommPort = availableCommPorts.FirstOrDefault();
                    }
                }
                else
                {
                    CommPortAvaiable = false;
                }
            }
        }

        /// <summary>
        /// The selected comm port
        /// </summary>
        private string selectedCommPort;
        public string SelectedCommPort
        {
            get => selectedCommPort;
            set
            {
                selectedCommPort = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// There is at least one comm port avaiable
        /// </summary>
        private bool commPortAvaiable;
        public bool CommPortAvaiable
        {
            get => commPortAvaiable;
            set
            {
                commPortAvaiable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Connected to a comm port
        /// </summary>
        private bool isCommPortConnected;
        public bool IsCommPortConnected
        {
            get => isCommPortConnected;
            set
            {
                isCommPortConnected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Local logger instance
        /// </summary>
        private readonly ILoggingService logger;

        /// <summary>
        /// ASIC wrapper holding references to every available ASIC
        /// </summary>
        private readonly IASICWrapper asicWrapper;

        /// <summary>
        /// Logger window instance
        /// </summary>
        public LoggerViewModel LoggerViewModel { get; private set; }

        /// <summary>
        /// Watchdog page instance
        /// </summary>
        public WatchdogViewModel WatchdogViewModel { get; private set; }

        /// <summary>
        /// Watchdog page instance
        /// </summary>
        public ISerialWrapper SerialWrapper { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel(ILoggingService logger, LoggerViewModel loggerViewModel, LoggerView loggerWindowView, WatchdogViewModel watchdogViewModel, IASICWrapper asicWrapper, ISerialWrapper serialWrapper) :
                base(logger)
        {
            // Init Logger and logger view model
            this.logger = logger;
            this.asicWrapper = asicWrapper;

            // Store references to child view models
            LoggerViewModel = loggerViewModel;
            WatchdogViewModel = watchdogViewModel;

            // Dummy configuration TODO: refactor to final implementation when available
            var asicConfig = new DummyConfiguration(asicWrapper);

            logger.Trace("In MainViewModel");
            loggerWindowView.Show();

            SerialWrapper = serialWrapper;
            AvailableCommPorts = SerialWrapper.AvailableCOMPorts;

            // TODO: remove temporary code - should be on other page
            // Trigger ASIC reset + start ASIC state reading
            this.asicWrapper.EstablishConnectionAsync();              // TODO: uncomment for testing
            this.asicWrapper.StartInitModeTimeoutResetting();
        }

        // Commands
        private RelayCommand commPortConnectCommand;
        public ICommand CommPortConnectCommand => commPortConnectCommand ??= new RelayCommand(CommPortConnect);

        private void CommPortConnect(object commandParameter)
        {
            if (SelectedCommPort != null)
            {
                if (SelectedCommPort != SerialWrapper.ManualComPortName)
                {
                    SerialWrapper.DicsonnectCOMPort();
                    SerialWrapper.ManualComPortName = SelectedCommPort;
                }
                IsCommPortConnected = SerialWrapper.ReconnectCOMPort();
            }
        }

        private RelayCommand rescanCOmmPortsCommand;
        public ICommand RescanCOmmPortsCommand => rescanCOmmPortsCommand ??= new RelayCommand(RescanCOmmPorts);

        private void RescanCOmmPorts(object commandParameter)
        {
            AvailableCommPorts = SerialWrapper.AvailableCOMPorts;

            // TEST
            AvailableCommPorts = new List<string>() { "Comm1", "Comm2", "Comm3" };
        }
    }
}