using AB15_GUI.WPF.ViewModels.Commands;
using AB15_GUI.WPF.Views;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Services.Interfaces;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;

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
        private ObservableCollection<string> availableCommPorts;
        public ObservableCollection<string> AvailableCommPorts
        {
            get => availableCommPorts;
            set
            {
                availableCommPorts = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The selected comm port from the drop down menu
        /// </summary>
        private string selectedCommPort;
        public string SelectedCommPort
        {
            get => selectedCommPort;
            set
            {
                selectedCommPort = value;
                OnPropertyChanged();

                if (SelectedCommPort == this.serialWrapper.ManualComPortName)
                {
                    IsSelectedPortDisplayed = true;
                }
                else
                {
                    IsSelectedPortDisplayed = false;
                }
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
        /// True if the currently displayed comm port in the drop down menu is the 
        /// same as the connected one, used to show "Reconnect" instead of "Connect"
        /// </summary>
        private bool isSelectedPortDisplayed;
        public bool IsSelectedPortDisplayed
        {
            get => isSelectedPortDisplayed;
            set
            {
                isSelectedPortDisplayed = value;
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
        /// Watchdog page instance
        /// </summary>
        public ISerialWrapper serialWrapper { get; private set; }

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

            this.serialWrapper = serialWrapper;
            AvailableCommPorts = new ObservableCollection<string>(this.serialWrapper.AvailableCOMPorts);
            UpdateAvailableAndSelectedCommPort();
        }

        // Commands
        private RelayCommand commPortConnectCommand;
        public ICommand CommPortConnectCommand => commPortConnectCommand ??= new RelayCommand(CommPortConnect);

        private void CommPortConnect(object commandParameter)
        {
            if (SelectedCommPort != null)
            {
                // If try to connect to another port, first close the currently connected one
                if (SelectedCommPort != this.serialWrapper.ManualComPortName)
                {
                    this.serialWrapper.DicsonnectCOMPort();
                    serialWrapper.ManualComPortName = SelectedCommPort;
                }
                else
                {
                    // Before the reconnect reset the MCU
                    if (IsCommPortConnected)
                    {
                        ResetMcu();

                        //TODO add wait
                    }
                }

                // Connect to the selected comm port
                IsCommPortConnected = serialWrapper.ConnectCOMPort();

                // TEST START
                IsCommPortConnected = true;
                // TEST END

                if (IsCommPortConnected)
                {
                    // Update the flag to change the button text from Connect to Reconnect
                    IsSelectedPortDisplayed = true;

                    // TODO: remove temporary code - should be on other page
                    // Trigger ASIC reset + start ASIC state reading
                    this.asicWrapper.EstablishConnectionAsync();              // TODO: uncomment for testing
                    this.asicWrapper.StartInitModeTimeoutResetting();
                }
            }
        }

        /// <summary>
        /// Used to re-scan the list of the avaiable comm ports
        /// </summary>
        private RelayCommand rescanCommPortsCommand;
        public ICommand RescanCommPortsCommand => rescanCommPortsCommand ??= new RelayCommand(RescanCommPorts);
        private void RescanCommPorts(object commandParameter)
        {
            //AvailableCommPorts = SerialWrapper.AvailableCOMPorts;
            // TEST START
            if (AvailableCommPorts.Count == 0)
            {
                AvailableCommPorts = new ObservableCollection<string>() { "Comm1", "Comm2", "Comm3", "Comm4" };
            }
            else
            {
                AvailableCommPorts.Remove(AvailableCommPorts[AvailableCommPorts.Count-1]);
            }
            // TEST END
            UpdateAvailableAndSelectedCommPort();
        }

        // Helper methods

        /// <summary>
        /// Used to update CommPortAvaiable flag if the list of commports is not empty
        /// and SelectedCommPort if it is not initialized or is removed from the list
        /// </summary>
        private void UpdateAvailableAndSelectedCommPort()
        {
            if ((availableCommPorts != null)
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

        /// <summary>
        /// Execute reset mcu command
        /// </summary>
        private async void ResetMcu()
        {
            logger.Debug($"Reseting MCU");

            // Create package to MCU
            TransmitCommunicationPackage<EmptyPayload> packageToSend = new TransmitCommunicationPackage<EmptyPayload>();
            packageToSend.ASICID = 1; //TODO
            packageToSend.Cmd = MCUCommand.RESET_MCU;
            packageToSend.PayloadType = typeof(EmptyPayload);

            // Send command to MCU
            ReceiveCommunicationPackage<EmptyPayload>? mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>?)await this.serialWrapper.SerialWriteAsync(packageToSend);

            // Validate response
            IsResponseValid(mcuResponse, nameof(ResetMcu));
        }

    }
}