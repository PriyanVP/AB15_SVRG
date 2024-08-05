using System;
using System.Collections.Generic;
using NLog;
using Stateless;
using Stateless.Graph;
using System.Windows.Input;
using AB15_GUI.WPF.ViewModels.Commands;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Genereted.Registers;
using AB15_GUI.WPF.Services.Interfaces;
using System.Net;

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

            // Configure state machine
            _stateMachine = new StateMachine<State, Triggers>(State.InitialState);

            // Emulate initial transition after POR
            _stateMachine.Configure(State.InitialState)
                         .Permit(Triggers.POR, State.Idle);

            _stateMachine.Configure(State.Idle)
                         .Permit(Triggers.GotConfiguration, State.InConfiguration);

            _stateMachine.Configure(State.InConfiguration)
                         .Permit(Triggers.ConfigurationLoaded, State.Configured)
                         .Ignore(Triggers.GotConfiguration)
                         .Ignore(Triggers.ConfigurationChanged);

            _stateMachine.Configure(State.Configured)
                         .Permit(Triggers.ConfigurationChanged, State.InConfiguration)
                         .Permit(Triggers.StartedWD, State.Running)
                         .Ignore(Triggers.GotConfiguration);

            _stateMachine.Configure(State.Running)
                         .Permit(Triggers.StoppedWD, State.InConfiguration);

            // Action that will be executed on every state change
            _stateMachine.OnTransitionCompleted((transition) => ExecuteStateTransition());

            // Fire transition to Idle state
            _stateMachine.Fire(Triggers.POR);

            // DOT graph of state machine
            // Can be used with debugger to plot state machine visualization
            string graph = UmlDotGraph.Format(_stateMachine.GetInfo());

            // Init commands for buttons
            ReadConfigFromASIC  = new RelayCommand(ReadConfigFromASICExecute, ((x) => _isReadWDConfigButtonEnabled));
            WriteConfigToASIC   = new RelayCommand(WriteConfigToASICExecute, ((x) => _isWriteWDConfigButtonEnabled));

            StartWatchdog       = new RelayCommand(StartWatchdogExecute, ((x) => _isStartWDButtonEnabled));
            StopWatchdog        = new RelayCommand(StopWatchdogExecute, ((x) => _isStopWDButtonEnabled));
        }

        #region State_Machine

        /// <summary>
        /// State values for WD feature, required for centralized flags handling for UI
        /// </summary>
        private enum State
        {
            InitialState,          /* Intial dummy state, require to handle initial transition */
            Idle,                  /* Default state, entered after startup */
            InConfiguration,       /* State after at least one reading of WD config in ASIC */
            Configured,            /* State when current WD config is loaded to ASIC */
            Running                /* WD is running on MCU */
        }

        /// <summary>
        /// State values for WD feature, required for centralized flags handling for UI
        /// </summary>
        private enum Triggers
        {
            POR,                   /* Transition after startup */
            GotConfiguration,      /* Got configuration from ASIC */
            ConfigurationChanged,  /* Configuration in GUI changed */
            ConfigurationLoaded,   /* Configuration in from GUI loaded to ASIC */
            StartedWD,             /* WD is started */
            StoppedWD              /* WD is stopped */
        }

        /// <summary>
        /// State machine to hold state of WD backend and handle transitions
        /// </summary>
        private readonly StateMachine<State, Triggers> _stateMachine;

        /// <summary>
        /// Property for current state of state machine observation
        /// Used for testing
        /// </summary>
        public State StateObservation => _stateMachine.State;

        /// <summary>
        /// Method to update flags in centralized way. All flags updates should be done there
        /// </summary>
        private void ExecuteStateTransition()
        {
            switch (_stateMachine.State)
            {
                case State.Idle:
                    // Buttons enable handling
                    _isReadWDConfigButtonEnabled = true;
                    _isWriteWDConfigButtonEnabled = false;
                    _isStartWDButtonEnabled = false;
                    _isStopWDButtonEnabled = false;

                    // Configuration enable handling
                    IsConfigEnable = false;
                    break;
                case State.InConfiguration:
                    // Buttons enable handling
                    _isReadWDConfigButtonEnabled = true;
                    _isWriteWDConfigButtonEnabled = true;
                    _isStartWDButtonEnabled = false;
                    _isStopWDButtonEnabled = false;

                    // Configuration enable handling
                    IsConfigEnable = true;
                    break;
                case State.Configured:
                    // Buttons enable handling
                    _isReadWDConfigButtonEnabled = true;
                    _isWriteWDConfigButtonEnabled = true;
                    _isStartWDButtonEnabled = true;
                    _isStopWDButtonEnabled = true;

                    // Configuration enable handling
                    IsConfigEnable = true;
                    break;
                case State.Running:
                    // Buttons enable handling
                    _isReadWDConfigButtonEnabled = true;
                    _isWriteWDConfigButtonEnabled = false;
                    _isStartWDButtonEnabled = false;
                    _isStopWDButtonEnabled = true;

                    // Configuration enable handling
                    IsConfigEnable = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_stateMachine.State), "Unexpected state received");
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
                // Do nothing if value is not changed
                if (isEN0Enabled == value) return;

                isEN0Enabled = value;
                OnPropertyChanged();
                _stateMachine.Fire(Triggers.ConfigurationChanged);
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
                // Do nothing if value is not changed
                if (wd1ResponseTime == value) return;

                wd1ResponseTime = value;
                OnPropertyChanged();
                _stateMachine.Fire(Triggers.ConfigurationChanged);
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
                // Do nothing if value is not changed
                if (wd1LockTime == value) return;

                wd1LockTime = value;
                OnPropertyChanged();
                _stateMachine.Fire(Triggers.ConfigurationChanged);
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
                // Do nothing if value is not changed
                if (wd2ResponseTime == value) return;

                wd2ResponseTime = value;
                OnPropertyChanged();
                _stateMachine.Fire(Triggers.ConfigurationChanged);
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
                // Do nothing if value is not changed
                if (wd2LockTime == value) return;

                wd2LockTime = value;
                OnPropertyChanged();
                _stateMachine.Fire(Triggers.ConfigurationChanged);
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
                // Do nothing if value is not changed
                if (wd1EN0DisableThreshold == value) return;
                
                wd1EN0DisableThreshold = value;
                OnPropertyChanged();
                _stateMachine.Fire(Triggers.ConfigurationChanged);
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
                // Do nothing if value is not changed
                if (wd2EN0DisableThreshold == value) return;

                wd2EN0DisableThreshold = value;
                OnPropertyChanged();
                _stateMachine.Fire(Triggers.ConfigurationChanged);
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

        #region Internal_configuration

        private Reg_spi_config_wd1 _spi_config_wd1 = new Reg_spi_config_wd1();
        private Reg_spi_config_wd2 _spi_config_wd2 = new Reg_spi_config_wd2();
        private Reg_spi_config_wd_decouple _spi_config_wd_decouple = new Reg_spi_config_wd_decouple();
        private Reg_spi_config_wd_thres0 _spi_config_wd_thres0 = new Reg_spi_config_wd_thres0();
        private Reg_spi_set_wdsettings _spi_set_wdsettings = new Reg_spi_set_wdsettings();

        #endregion // Internal_configuration
        
        #region Commands

        /// <summary>
        /// Monitoring message ID
        /// </summary>
        private int? _msgIdForMonitoring = null;

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
            // TODO: uncomment for AB15
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 1;
            // packageToSend.Cmd = MCUCommand. // TODO: add
            packageToSend.PayloadType = typeof(AddressDataPayload);
            packageToSend.Payload.Address.Add(_spi_config_wd1.Address);
            packageToSend.Payload.Address.Add(_spi_config_wd2.Address);
            packageToSend.Payload.Address.Add(_spi_config_wd_decouple.Address);
            packageToSend.Payload.Address.Add(_spi_config_wd_thres0.Address);
            packageToSend.Payload.Address.Add(_spi_set_wdsettings.Address);

            // TODO: temporary implementation for AB12, replace by actual on AB15
            ReceiveCommunicationPackage<AddressDataPayload> placeholderPackage = new ReceiveCommunicationPackage<AddressDataPayload>();
            placeholderPackage.ASICID = 1;
            placeholderPackage.Status = MCUStatus.DATA;
            placeholderPackage.Payload.Data.AddRange(new List<UInt16>() { 0, 0, 0, 0, 0 });
            ReadConfigDelegate(placeholderPackage);
        }

        /// <summary>
        /// Execute Write config to ASIC command
        /// </summary>
        private void WriteConfigToASICExecute(object obj)
        {
            TransmitCommunicationPackage<WDConfigurePayload> packageToSend = new TransmitCommunicationPackage<WDConfigurePayload>();

            packageToSend.ASICID = 1;
            packageToSend.Cmd = MCUCommand.CONFIGURE_WATCHDOG;
            packageToSend.Deleg = WriteConfigDelagate;
            packageToSend.PayloadType = typeof(EmptyPayload);

            // Unpack data from UI to fields of registers
            // Load configs with data from ASIC
            packageToSend.Payload.spi_config_wd1.Data = _spi_config_wd1.Data;
            packageToSend.Payload.spi_config_wd2.Data = _spi_config_wd2.Data;
            packageToSend.Payload.spi_config_wd_decouple.Data = _spi_config_wd_decouple.Data;
            packageToSend.Payload.spi_config_wd_thres0.Data = _spi_config_wd_thres0.Data;

            // Modify some fields based on UI input
            packageToSend.Payload.spi_config_wd1.spi_set_locktime_wd1.Data = (UInt16) WD1LockTime;
            packageToSend.Payload.spi_config_wd1.spi_set_responsetime_wd1.Data = (UInt16) WD1ResponseTime;

            packageToSend.Payload.spi_config_wd2.spi_set_locktime_wd2.Data = (UInt16) WD2LockTime;
            packageToSend.Payload.spi_config_wd2.spi_set_responsetime_wd2.Data = (UInt16) WD2ResponseTime;

            packageToSend.Payload.spi_config_wd_decouple.spi_decouple_wd_en0.Data = (UInt16) ((IsEN0Enabled) ? (1) : (0));

            packageToSend.Payload.spi_config_wd_thres0.spi_set_en0_thre_wd1.Data = (UInt16) WD1EN0DisableThreshold;
            packageToSend.Payload.spi_config_wd_thres0.spi_set_en0_thre_wd2.Data = (UInt16) WD2EN0DisableThreshold;

            // Send command to MCU
            serialWrapper.SerialWrite(packageToSend);
        }

        /// <summary>
        /// Execute Start watchdog command
        /// </summary>
        private void StartWatchdogExecute(object obj)
        {
            TransmitCommunicationPackage<EmptyPayload> packageToSendStartWD = new TransmitCommunicationPackage<EmptyPayload>();
            TransmitCommunicationPackage<EmptyPayload> packageToSendStartMonitoringWD = new TransmitCommunicationPackage<EmptyPayload>();

            // Configure start watchdog command
            packageToSendStartWD.ASICID = 1;
            packageToSendStartWD.Cmd = MCUCommand.START_WATCHDOG;
            packageToSendStartWD.Deleg = StartWatchdogDelegate;
            packageToSendStartWD.PayloadType = typeof(EmptyPayload);
       
            // Send start command to MCU
            serialWrapper.SerialWrite(packageToSendStartWD);

            // Configure start watchdog status reading command
            packageToSendStartMonitoringWD.ASICID = 1;
            packageToSendStartMonitoringWD.Cmd = MCUCommand.START_MONITORING_WATCHDOG;
            packageToSendStartMonitoringWD.Deleg = StatusMonitoringDelagate;
            packageToSendStartMonitoringWD.PayloadType = typeof(WDStatusPayload);
            packageToSendStartMonitoringWD.IsContinuous = true;
       
            // Send start status reading command to MCU
            serialWrapper.SerialWrite(packageToSendStartMonitoringWD);

            // Store message ID for stopping monitoring
            _msgIdForMonitoring = packageToSendStartMonitoringWD.MsgID;
        }

        /// <summary>
        /// Execute Stop watchdog command
        /// </summary>
        private void StopWatchdogExecute(object obj)
        {
            TransmitCommunicationPackage<EmptyPayload> packageToSendStopWD = new TransmitCommunicationPackage<EmptyPayload>();
            TransmitCommunicationPackage<EmptyPayload> packageToSendStopMonitoringWD = new TransmitCommunicationPackage<EmptyPayload>();

            // Configure start watchdog command
            packageToSendStopWD.ASICID = 1;
            packageToSendStopWD.Cmd = MCUCommand.START_WATCHDOG;
            packageToSendStopWD.Deleg = StopWatchdogDelegate;
            packageToSendStopWD.PayloadType = typeof(EmptyPayload);
       
            // Send start command to MCU
            serialWrapper.SerialWrite(packageToSendStopWD);

            // Configure start watchdog status reading command
            packageToSendStopMonitoringWD.ASICID = 1;
            packageToSendStopMonitoringWD.Cmd = MCUCommand.STOP_MONITORING_WATCHDOG;
            packageToSendStopMonitoringWD.Deleg = (package) => 
                { 
                    // Remove item for monitoring
                    if (_msgIdForMonitoring != null)
                    {
                        serialWrapper.RemoveWaitlistItem(_msgIdForMonitoring);
                        _msgIdForMonitoring = null;
                    }  
                    return; 
                };
            packageToSendStopMonitoringWD.PayloadType = typeof(EmptyPayload);
       
            // Send start status reading command to MCU
            serialWrapper.SerialWrite(packageToSendStopMonitoringWD);
        }

        private void ReadConfigDelegate(IReceiveCommunicationPackage response)
        {
            // Handle response absent scenario
            if (response.Status == MCUStatus.RESPONSE_ABSENT)
            {
                AddError("Doesn't receive data from MCU in expected timeframe", nameof(ReadConfigFromASIC));
                logger.Error($"Error response received. Status: {response.Status}");
                return;
            }

            // Typecast response to actual type
            ReceiveCommunicationPackage<AddressDataPayload> mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>) response;

            // Change state if response received
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(ReadConfigFromASIC));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
                return;
            }
            else if (mcuResponse.Payload.Data.Count < 5)
            {
                AddError($"Unexpected amount of readout data received. Expected 5, but got {mcuResponse.Payload.Data.Count}.", nameof(ReadConfigFromASIC));
                logger.Error($"Unexpected amount of readout data received. Expected 5, but got {mcuResponse.Payload.Data.Count}.");
                return;
            }

            // Update configuration
            _spi_config_wd1.Data         = mcuResponse.Payload.Data[0];
            _spi_config_wd2.Data         = mcuResponse.Payload.Data[1];
            _spi_config_wd_decouple.Data = mcuResponse.Payload.Data[2];
            _spi_config_wd_thres0.Data   = mcuResponse.Payload.Data[3];
            _spi_set_wdsettings.Data     = mcuResponse.Payload.Data[4];

            // Unpacking of data for AB15
            WD1ResponseTime = (int) _spi_config_wd1.spi_set_responsetime_wd1.Data;
            WD2ResponseTime = (int) _spi_config_wd2.spi_set_responsetime_wd2.Data;

            WD1LockTime = (int)_spi_config_wd1.spi_set_locktime_wd1.Data;
            WD2LockTime = (int)_spi_config_wd2.spi_set_locktime_wd2.Data;

            // AB12 code, TODO: remove when AB15 available
            // Values share same step as AB15 scale
            WD1ResponseTime = 63; 
            WD2ResponseTime = 16;

            WD1LockTime = 0;
            WD2LockTime = 10; // Underflow limit

            // Fire trigger for state machine
            _stateMachine.Fire(Triggers.GotConfiguration);
        }

        private void WriteConfigDelagate(IReceiveCommunicationPackage response)
        {
            // Handle response absent scenario
            if (response.Status == MCUStatus.RESPONSE_ABSENT)
            {
                AddError("Doesn't receive data from MCU in expected timeframe", nameof(WriteConfigToASIC));
                logger.Error($"Error response received. Status: {response.Status}");
                return;
            }

            // Typecast response to actual type
            ReceiveCommunicationPackage<EmptyPayload> mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>) response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(WriteConfigToASIC));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
            }

            _stateMachine.Fire(Triggers.ConfigurationLoaded);
        }

        private void StartWatchdogDelegate(IReceiveCommunicationPackage response)
        {
            // Handle response absent scenario
            if (response.Status == MCUStatus.RESPONSE_ABSENT)
            {
                AddError("Doesn't receive data from MCU in expected timeframe", nameof(StartWatchdog));
                logger.Error($"Error response received. Status: {response.Status}");
                return;
            }

            // Typecast response to actual type
            ReceiveCommunicationPackage<EmptyPayload> mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>) response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(StartWatchdog));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
            }

            _stateMachine.Fire(Triggers.StartedWD);
        }

        private void StopWatchdogDelegate(IReceiveCommunicationPackage response)
        {
            // Handle response absent scenario
            if (response.Status == MCUStatus.RESPONSE_ABSENT)
            {
                AddError("Doesn't receive data from MCU in expected timeframe", nameof(StopWatchdog));
                logger.Error($"Error response received. Status: {response.Status}");
                return;
            }

            // Typecast response to actual type
            ReceiveCommunicationPackage<EmptyPayload> mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>) response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(StopWatchdog));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
            }

            _stateMachine.Fire(Triggers.StoppedWD);
        }

        private void StatusMonitoringDelagate(IReceiveCommunicationPackage response)
        {
            // Handle response absent scenario
            if (response.Status == MCUStatus.RESPONSE_ABSENT)
            {
                AddError("Doesn't receive data from MCU in expected timeframe", nameof(StartWatchdog));
                logger.Error($"Error response received. Status: {response.Status}");
                return;
            }

            // Typecast response to actual type
            ReceiveCommunicationPackage<WDStatusPayload> mcuResponse = (ReceiveCommunicationPackage<WDStatusPayload>) response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(StartWatchdog));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
                return;
            }

            // Update statuses
            // TODO: add implementation for AB15
            // Implementation for AB12
            WDFaultStatus = (mcuResponse.Payload.WatchdogStatus.WatchdogFault) ? (FaultStatus.Fault) : (FaultStatus.Good);

            WD1FaultStatus = (mcuResponse.Payload.WatchdogStatus.SlowWatchdogFault) ? (FaultStatus.Fault) : (FaultStatus.Good);
            WD2FaultStatus = (mcuResponse.Payload.WatchdogStatus.FastWatchdogFault) ? (FaultStatus.Fault) : (FaultStatus.Good);

            OSCMONFaultStatus = (mcuResponse.Payload.WatchdogStatus.OscillatorFault) ? (FaultStatus.Fault) : (FaultStatus.Good);

            WD1QAFaultStatus = (mcuResponse.Payload.WatchdogStatus.SlowWatchdogQAFault) ? (FaultStatus.Fault) : (FaultStatus.Good);
            WD2QAFaultStatus = (mcuResponse.Payload.WatchdogStatus.FastWatchdogQAFault) ? (FaultStatus.Fault) : (FaultStatus.Good);
        }

        #endregion // Commands
    }
}