using AB15_GUI.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Windows.Input;
using System.Windows;
using AB15_GUI.WPF.ViewModels.Commands;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Services;
using AB15_GUI.WPF.Services.Interfaces;

namespace AB15_GUI.WPF.ViewModels
{
    /// <summary>
    /// Faults status value definition
    /// </summary>
    public enum FaultStatus
    {
        NoStatus,
        Good,
        Fault
    }

    /// <summary>
    /// State values for WD feature, required for centralized flags handling for UI
    /// </summary>
    public enum WDBackendState
    {
        Idle,                  /* Default state, entered after startup */
        InConfiguration,       /* State after at least one reading of WD config in ASIC */
        Configured,            /* State when current WD config is loaded to ASIC */
        Running,               /* WD is running on MCU */
        Stopped,               /* WD is stopped on MCU */
    }

    public class WatchdogViewModel : ViewModelBase
    {
        /// <summary>
        /// Local logger instance
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// SerialWrapper reference to perform communication with MCU
        /// </summary>
        private readonly ISerialWrapper serialWrapper;

        /// <summary>
        /// Constructor
        /// </summary>
        public WatchdogViewModel(Logger logger, ISerialWrapper serialWrapper)
        {
            this.logger = logger;
            this.serialWrapper = serialWrapper;
            logger.Trace("In WatchdogViewModel");

            // Initial state transition
            ExecuteStateTransition(WDBackendState.Idle);

            // Init commands for buttons
            ReadConfigFromASIC  = new RelayCommand(ReadConfigFromASICExecute, ((x) => _isReadWDConfigButtonEnabled));
            WriteConfigToASIC   = new RelayCommand(WriteConfigToASICExecute, ((x) => _isWriteWDConfigButtonEnabled));

            StartWatchdog   = new RelayCommand(StartWatchdogExecute, ((x) => _isStartWDButtonEnabled));
            StopWatchdog    = new RelayCommand(StartWatchdogExecute, ((x) => _isStopWDButtonEnabled));
        }

        #region State_Machine

        /// <summary>
        /// Variable to hold state of WD backend
        /// </summary>
        private WDBackendState _wdBackendState;

        /// <summary>
        /// Method to update flags in centralized way. All flags updates should be done there
        /// </summary>
        /// <param name="newState">new state of WD backend</param>
        private void ExecuteStateTransition(WDBackendState newState)
        {
            // TODO: simple implementation, if state machines will be used widely refactor implementation to advanced state machine
            // TODO: add check on allowed transitions? check triggers?
            // TODO: pass trigger?
            _wdBackendState = newState;

            switch (_wdBackendState)
            {
                case WDBackendState.Idle:
                    // Buttons enable handling
                    _isReadWDConfigButtonEnabled = true;
                    _isWriteWDConfigButtonEnabled = false;
                    _isStartWDButtonEnabled = false;
                    _isStopWDButtonEnabled = false;

                    // Configuration enable handling
                    IsConfigEnable = false;
                    break;
                case WDBackendState.InConfiguration:
                    // Buttons enable handling
                    _isReadWDConfigButtonEnabled = true;
                    _isWriteWDConfigButtonEnabled = true;
                    _isStartWDButtonEnabled = false;
                    _isStopWDButtonEnabled = false;

                    // Configuration enable handling
                    IsConfigEnable = true;
                    break;
                case WDBackendState.Configured:
                    // Buttons enable handling
                    _isReadWDConfigButtonEnabled = true;
                    _isWriteWDConfigButtonEnabled = true;
                    _isStartWDButtonEnabled = true;
                    _isStopWDButtonEnabled = true;

                    // Configuration enable handling
                    IsConfigEnable = true;
                    break;
                case WDBackendState.Running:
                    // Buttons enable handling
                    _isReadWDConfigButtonEnabled = true;
                    _isWriteWDConfigButtonEnabled = false;
                    _isStartWDButtonEnabled = false;
                    _isStopWDButtonEnabled = true;

                    // Configuration enable handling
                    IsConfigEnable = false;
                    break;
                case WDBackendState.Stopped:
                    // Buttons enable handling
                    _isReadWDConfigButtonEnabled = true;
                    _isWriteWDConfigButtonEnabled = true;
                    _isStartWDButtonEnabled = true;
                    _isStopWDButtonEnabled = true;

                    // Configuration enable handling
                    IsConfigEnable = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), "Unexpected state received");
            }
        }

        #endregion //State_Machine

        #region Bindable_Properties

        /// <summary>
        /// Enable EN0 thresholds configuration
        /// </summary>
        private bool isEN0Enabled;
        public bool IsEN0Enabled
        {
            get => isEN0Enabled;
            set 
            { 
                isEN0Enabled = value;
                OnPropertyChanged();
                ExecuteStateTransition(WDBackendState.InConfiguration);
            }
        }

        /// <summary>
        /// WD1 Response time value
        /// </summary>
        private int wd1ResponseTime;
        public int WD1ResponseTime
        {
            get => wd1ResponseTime;
            set
            {
                wd1ResponseTime = value;
                OnPropertyChanged();
                ExecuteStateTransition(WDBackendState.InConfiguration);
            }
        }

        /// <summary>
        /// WD1 Lock time value
        /// </summary>
        private int wd1LockTime;
        public int WD1LockTime
        {
            get => wd1LockTime;
            set
            {
                wd1LockTime = value;
                OnPropertyChanged();
                ExecuteStateTransition(WDBackendState.InConfiguration);
            }
        }

        /// <summary>
        /// WD2 Response time value
        /// </summary>
        private int wd2ResponseTime;
        public int WD2ResponseTime
        {
            get => wd2ResponseTime;
            set
            {
                wd2ResponseTime = value;
                OnPropertyChanged();
                ExecuteStateTransition(WDBackendState.InConfiguration);
            }
        }

        /// <summary>
        /// WD1 Lock time value
        /// </summary>
        private int wd1EN0DisableThreshold;
        public int WD1EN0DisableThreshold
        {
            get => wd1EN0DisableThreshold;
            set
            {
                wd1EN0DisableThreshold = value;
                OnPropertyChanged();
                ExecuteStateTransition(WDBackendState.InConfiguration);
            }
        }

        /// <summary>
        /// WD2 Response time value
        /// </summary>
        private int wd2EN0DisableThreshold;
        public int WD2EN0DisableThreshold
        {
            get => wd2EN0DisableThreshold;
            set
            {
                wd2EN0DisableThreshold = value;
                OnPropertyChanged();
                ExecuteStateTransition(WDBackendState.InConfiguration);
            }
        }

        /// <summary>
        /// WD2 Lock time value
        /// </summary>
        private int wd2LockTime;
        public int WD2LockTime
        {
            get => wd2LockTime;
            set
            {
                wd2LockTime = value;
                OnPropertyChanged();
                ExecuteStateTransition(WDBackendState.InConfiguration);
            }
        }

        /// <summary>
        /// toggle enable for configuration fields 
        /// </summary>
        private bool isConfigEnable;
        public bool IsConfigEnable
        {
            get => isConfigEnable;
            set
            {
                isConfigEnable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Global wd fault flag
        /// </summary>
        private FaultStatus wdFaultStatus;
        public FaultStatus WDFaultStatus
        {
            get => wdFaultStatus;
            set
            {
                wdFaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD1 fault flag
        /// </summary>
        private FaultStatus wd1FaultStatus;
        public FaultStatus WD1FaultStatus
        {
            get => wd1FaultStatus;
            set
            {
                wd1FaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD2 fault flag
        /// </summary>
        private FaultStatus wd2FaultStatus;
        public FaultStatus WD2FaultStatus
        {
            get => wd2FaultStatus;
            set
            {
                wd2FaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Error pin fault flag
        /// </summary>
        private FaultStatus errorPinFaultStatus;
        public FaultStatus ErrorPinFaultStatus
        {
            get => errorPinFaultStatus;
            set
            {
                errorPinFaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD1 timer fault flag
        /// </summary>
        private FaultStatus wd1TimerFaultStatus;
        public FaultStatus WD1TimerFaultStatus
        {
            get => wd1TimerFaultStatus;
            set
            {
                wd1TimerFaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD2 timer fault flag
        /// </summary>
        private FaultStatus wd2TimerFaultStatus;
        public FaultStatus WD2TimerFaultStatus
        {
            get => wd2TimerFaultStatus;
            set
            {
                wd2TimerFaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// OSCMON fault flag
        /// </summary>
        private FaultStatus oscmonFaultStatus;
        public FaultStatus OSCMONFaultStatus
        {
            get => oscmonFaultStatus;
            set
            {
                oscmonFaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD1 QA fault flag
        /// </summary>
        private FaultStatus wd1QAFaultStatus;
        public FaultStatus WD1QAFaultStatus
        {
            get => wd1QAFaultStatus;
            set
            {
                wd1QAFaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD2 QA Fault flag
        /// </summary>
        private FaultStatus wd2QAFaultStatus;
        public FaultStatus WD2QAFaultStatus
        {
            get => wd2QAFaultStatus;
            set
            {
                wd2QAFaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Oscillator fault flag
        /// </summary>
        private FaultStatus oscillatorFaultStatus;
        public FaultStatus OscillatorFaultStatus
        {
            get => oscillatorFaultStatus;
            set
            {
                oscillatorFaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD reset flag
        /// </summary>
        private FaultStatus wdResetFaultStatus;
        public FaultStatus WDResetFaultStatus
        {
            get => wdResetFaultStatus;
            set
            {
                wdResetFaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD1 counter fault flag
        /// </summary>
        private FaultStatus wd1CounterFaultStatus;
        public FaultStatus WD1CounterFaultStatus
        {
            get => wd1CounterFaultStatus;
            set
            {
                wd1CounterFaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// QA1 fault flag
        /// </summary>
        private FaultStatus qa1FaultStatus;
        public FaultStatus QA1FaultStatus
        {
            get => qa1FaultStatus;
            set
            {
                qa1FaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD2 counter fault flag
        /// </summary>
        private FaultStatus wd2CounterFaultStatus;
        public FaultStatus WD2CounterFaultStatus
        {
            get => wd2CounterFaultStatus;
            set
            {
                wd2CounterFaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// QA2 fault flag
        /// </summary>
        private FaultStatus qa2FaultStatus;
        public FaultStatus QA2FaultStatus
        {
            get => qa2FaultStatus;
            set
            {
                qa2FaultStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// EN0 Hight status flag
        /// </summary>
        private FaultStatus en0HightStatus;
        public FaultStatus EN0HightStatus
        {
            get => en0HightStatus;
            set
            {
                en0HightStatus = value;
                OnPropertyChanged();
            }
        }

        #endregion // Bindable_Properties

        #region Commands

        /// <summary>
        /// Start WD button enable state
        /// </summary>
        private bool _isStartWDButtonEnabled;

        /// <summary>
        /// Stop WD button enable state
        /// </summary>
        private bool _isStopWDButtonEnabled;

        /// <summary>
        /// Read WD config button enable state
        /// </summary>
        private bool _isReadWDConfigButtonEnabled;

        /// <summary>
        /// Write WD config button enable state
        /// </summary>
        private bool _isWriteWDConfigButtonEnabled;

        /// <summary>
        /// Command handler for Read config from ASIC button
        /// </summary>
        public ICommand ReadConfigFromASIC { get; }

        /// <summary>
        /// Command handler for Write config to ASIC button
        /// </summary>
        public ICommand WriteConfigToASIC { get; }

        /// <summary>
        /// Command handler for Start watchdog button
        /// </summary>
        public ICommand StartWatchdog { get; }

        /// <summary>
        /// Command handler for Stop watchdog button
        /// </summary>
        public ICommand StopWatchdog { get; }

        /// <summary>
        /// Execute Read config from ASIC command
        /// </summary>
        private void ReadConfigFromASICExecute(object obj)
        {
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 1;
            packageToSend.Cmd = MCUCommand.READ_REG;
        }

        /// <summary>
        /// Execute Write config to ASIC command
        /// </summary>
        private void WriteConfigToASICExecute(object obj)
        {
        }

        /// <summary>
        /// Execute Start watchdog command
        /// </summary>
        private void StartWatchdogExecute(object obj)
        {
        }

        /// <summary>
        /// Execute Stop watchdog command
        /// </summary>
        private void StopWatchdogExecute(object obj)
        {
        }

        private void ReadConfigDelegate(IReceiveCommunicationPackage response)
        {
            // Typecast response to actual type
            ReceiveCommunicationPackage<WDStatusPayload> mcuResponse = (ReceiveCommunicationPackage<WDStatusPayload>) response;

            // Change state if response received
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(WriteConfigToASIC));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
            }
            else if (_wdBackendState == WDBackendState.Idle)
            {
                ExecuteStateTransition(WDBackendState.InConfiguration);
            }
        }

        private void WriteConfigDelagate(IReceiveCommunicationPackage response)
        {
            // Typecast response to actual type
            ReceiveCommunicationPackage<EmptyPayload> mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>) response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(WriteConfigToASIC));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
            }
            else
            {
                ExecuteStateTransition(WDBackendState.Configured);
            }
        }

        private void StartConfigDelagate(IReceiveCommunicationPackage response)
        {
            // Typecast response to actual type
            ReceiveCommunicationPackage<EmptyPayload> mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>) response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(WriteConfigToASIC));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
            }
            else
            {
                ExecuteStateTransition(WDBackendState.Running);
            }
        }

        private void StopConfigDelagate(IReceiveCommunicationPackage response)
        {
            // Typecast response to actual type
            ReceiveCommunicationPackage<EmptyPayload> mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>) response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(WriteConfigToASIC));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
            }
            else
            {
                ExecuteStateTransition(WDBackendState.Stopped);
            }
        }

        #endregion // Commands
    }
}