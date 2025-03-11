using AB15_GUI.WPF.ViewModels.Commands;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Services.Interfaces;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
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
        #region Bindable_Properties

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
        private ObservableCollection<string> availableCOMPorts;
        public ObservableCollection<string> AvailableCOMPorts
        {
            get => availableCOMPorts;
            set
            {
                availableCOMPorts = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The selected comm port from the drop down menu
        /// </summary>
        private string selectedCOMPort;
        public string SelectedCOMPort
        {
            get => selectedCOMPort;
            set
            {
                selectedCOMPort = value;
                OnPropertyChanged();

                if (SelectedCOMPort == this.serialWrapper.ManualComPortName)
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
        private bool comPortAvaiable;
        public bool COMPortAvaiable
        {
            get => comPortAvaiable;
            set
            {
                comPortAvaiable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Connected to a comm port
        /// </summary>
        private bool isCOMPortConnected;
        public bool IsCOMPortConnected
        {
            get => isCOMPortConnected;
            set
            {
                isCOMPortConnected = value;
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

        #endregion // Bindable_Properties

        /// <summary>
        /// Local logger instance
        /// </summary>
        private readonly ILoggingService logger;

        /// <summary>
        /// ASIC wrapper holding references to every available ASIC
        /// </summary>
        private readonly IASICWrapper asicWrapper;

        /// <summary>
        /// Serial Wrapper instance
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
        public MainViewModel(ILoggingService logger, LoggerViewModel loggerViewModel, WatchdogViewModel watchdogViewModel, IASICWrapper asicWrapper, ISerialWrapper serialWrapper) :
                base(logger)
        {
            // Command initialization
            comPortConnectCommand = new RelayCommand(COMPortConnect);
            rescanCommPortsCommand = new RelayCommand(RescanCOMPorts);
            
            // Init Logger and logger view model
            this.logger = logger;
            this.asicWrapper = asicWrapper;

            // Store references to child view models
            LoggerViewModel = loggerViewModel;
            WatchdogViewModel = watchdogViewModel;

            // Dummy configuration TODO: refactor to final implementation when available
            var asicConfig = new DummyConfiguration(asicWrapper);

            logger.Trace("In MainViewModel");

            this.serialWrapper = serialWrapper;
            
            // Chech the available COM ports at start
            RescanCOMPorts(null);
        }

        #region Commands

        /// <summary>
        /// Used to connect/reconnect to the selected COM port
        /// </summary>
        private RelayCommand comPortConnectCommand;
        public ICommand COMPortConnectCommand => comPortConnectCommand;

        private void COMPortConnect(object commandParameter)
        {
            if (SelectedCOMPort == null) return;

            // If try to connect to another port, first close the currently connected one
            if (SelectedCOMPort != this.serialWrapper.ManualComPortName)
            {
                this.serialWrapper.DicsonnectCOMPort();
                serialWrapper.ManualComPortName = SelectedCOMPort;
            }
            else
            {
                // Before the reconnect reset the MCU
                if (IsCOMPortConnected)
                {
                    ResetMcu();

                    Task.WaitAll([Task.Delay(100)]);
                }
            }

            // Connect to the selected comm port
            IsCOMPortConnected = serialWrapper.ConnectCOMPort();

            if (IsCOMPortConnected)
            {
                // Update the flag to change the button text from Connect to Reconnect
                IsSelectedPortDisplayed = true;

                // Trigger ASIC reset + start ASIC state reading
                this.asicWrapper.EstablishConnectionAsync();
                this.asicWrapper.StartInitModeTimeoutResetting();
            }
        }

        /// <summary>
        /// Used to rescan the list of the avaiable COM ports
        /// </summary>
        private RelayCommand rescanCOMPortsCommand;
        public ICommand RescanCOMPortsCommand => rescanCOMPortsCommand ??= new RelayCommand(RescanCOMPorts);

        private RelayCommand rescanCommPortsCommand;
        public ICommand RescanCommPortsCommand => rescanCommPortsCommand;
        private void RescanCOMPorts(object commandParameter)
        {
            AvailableCOMPorts = new ObservableCollection<string>(this.serialWrapper.AvailableCOMPorts);

            COMPortAvaiable = (availableCOMPorts != null) && (availableCOMPorts.Count > 0);

            if (string.IsNullOrEmpty(SelectedCOMPort))
            {
                SelectedCOMPort = availableCOMPorts.FirstOrDefault();
            }
        }

        #endregion // Commands

        #region Helper methods

        /// <summary>
        /// Execute reset mcu command
        /// </summary>
        private async void ResetMcu()
        {
            logger.Debug($"Reseting MCU");

            // Create package to MCU
            TransmitCommunicationPackage<EmptyPayload> packageToSend = new TransmitCommunicationPackage<EmptyPayload>();
            packageToSend.ASICID = (int)DeviceIDs.SPI1_CS1MASTER; //TODO
            packageToSend.Cmd = MCUCommand.RESET_MCU;
            packageToSend.PayloadType = typeof(EmptyPayload);

            // Send command to MCU
            ReceiveCommunicationPackage<EmptyPayload>? mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>?)await this.serialWrapper.SerialWriteAsync(packageToSend);

            // Validate response
            IsResponseValid(mcuResponse, nameof(ResetMcu));
        }

        #endregion Helper methods
    }
}