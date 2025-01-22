using System;
using System.Collections.Generic;
using AB15_GUI.WPF.NLog;
using Stateless;
using Stateless.Graph;
using System.Windows.Input;
using AB15_GUI.WPF.ViewModels.Commands;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Generated.Registers;
using AB15_GUI.WPF.Services.Interfaces;
using System.Threading.Tasks;

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
        private readonly ILoggingService logger;

        /// <summary>
        /// SerialWrapper reference to perform communication with MCU
        /// </summary>
        private readonly ISerialWrapper serialWrapper;

        /// <summary>
        /// ASIC wrapper holding references to every available ASIC
        /// </summary>
        private readonly IASICWrapper asicWrapper;

        /// <summary>
        /// Constructor
        /// </summary>
        public WatchdogViewModel(ILoggingService logger, ISerialWrapper serialWrapper, IASICWrapper asicWrapper)
        {
            this.logger = logger;
            this.serialWrapper = serialWrapper;
            this.asicWrapper = asicWrapper;
            logger.Trace("In WatchdogViewModel");

            // Init help messages for UI
            InitHelpMessages();

            // Configure state machine
            _stateMachine = new StateMachine<State, Triggers>(State.InitialState);

            // Emulate initial transition after POR
            _stateMachine.Configure(State.InitialState)
                         .Permit(Triggers.POR, State.Idle);

            // TODO: add locked state to be in before configuration is loaded to ASIC

            _stateMachine.Configure(State.Idle)
                         .Permit(Triggers.GotConfiguration, State.InConfiguration);

            _stateMachine.Configure(State.InConfiguration)
                         .Permit(Triggers.ConfigurationLoaded, State.Configured)
                         .Ignore(Triggers.GotConfiguration)
                         .Ignore(Triggers.ConfigurationChanged);

            _stateMachine.Configure(State.Configured)
                         .Permit(Triggers.ConfigurationChanged, State.InConfiguration)
                         .Permit(Triggers.StartedWD, State.Running)
                         .Ignore(Triggers.ConfigurationLoaded)
                         .Ignore(Triggers.GotConfiguration)
                         .Ignore(Triggers.StoppedWD);

            _stateMachine.Configure(State.Running)
                         .Permit(Triggers.StoppedWD, State.InConfiguration);

            // Action that will be executed on every state change
            _stateMachine.OnTransitionCompleted((transition) => ExecuteStateTransition());

            // DOT graph of state machine
            // Can be used with debugger to plot state machine visualization
            string graph = UmlDotGraph.Format(_stateMachine.GetInfo());

            // Init commands for buttons
            ReadConfigFromASIC  = new RelayCommand(ReadConfigFromASICExecute);
            WriteConfigToASIC   = new RelayCommand(WriteConfigToASICExecute);

            StartWatchdog       = new RelayCommand(StartWatchdogExecute);
            StopWatchdog        = new RelayCommand(StopWatchdogExecute);

            // Fire transition to Idle state
            _stateMachine.Fire(Triggers.POR);
        }

        #region State_Machine

        /// <summary>
        /// State values for WD feature, required for centralized flags handling for UI
        /// </summary>
        public enum State
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
            logger.Debug($"In state machine transition: state {_stateMachine.State}");

            switch (_stateMachine.State)
            {
                case State.Idle:
                    // Buttons/commands enable handling
                    _readWDConfigCommand.Enable   = true;
                    _writeWDConfigCommand.Enable  = false;
                    _startWDCommand.Enable        = false;
                    _stopWDCommand.Enable         = false;

                    // Configuration enable handling
                    IsConfigEnable = false;
                    break;
                case State.InConfiguration:
                    // Buttons/commands enable handling
                    _readWDConfigCommand.Enable   = true;
                    _writeWDConfigCommand.Enable  = true;
                    _startWDCommand.Enable        = false;
                    _stopWDCommand.Enable         = false;

                    // Configuration enable handling
                    IsConfigEnable = true;
                    break;
                case State.Configured:
                    // Buttons/commands enable handling
                    _readWDConfigCommand.Enable   = true;
                    _writeWDConfigCommand.Enable  = true;
                    _startWDCommand.Enable        = true;
                    _stopWDCommand.Enable         = true;

                    // Configuration enable handling
                    IsConfigEnable = true;
                    break;
                case State.Running:
                    // Buttons/commands enable handling
                    _readWDConfigCommand.Enable   = true;
                    _writeWDConfigCommand.Enable  = false;
                    _startWDCommand.Enable        = false;
                    _stopWDCommand.Enable         = true;

                    // Configuration enable handling
                    IsConfigEnable = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_stateMachine.State), "Unexpected state received");
            }

            // Request update of buttons states
            OnPropertyChanged(nameof(ReadWDConfigCommandEn));
            OnPropertyChanged(nameof(WriteWDConfigCommandEn));
            OnPropertyChanged(nameof(StartWDCommandEn));
            OnPropertyChanged(nameof(StopWDCommandEn));
        }

        #endregion //State_Machine

        #region Bindable_Properties

        private void InitHelpMessages()
        {
            // Bindable properties help messages
            AddHelpMsg(nameof(IsEN0Enabled), "Indicates if EN0 pin is enabled");
            AddHelpMsg(nameof(WD1ResponseTime), $"WD1 response time.{Environment.NewLine}Correct WD acknowledge timing window is{Environment.NewLine}LockTime < AckPeriod < LockTime+ResponseTime");
            AddHelpMsg(nameof(WD1LockTime), "WD1 lock time. Acknowledge transactions during this time are erroneous.");
            AddHelpMsg(nameof(WD2ResponseTime), $"WD2 response time.{Environment.NewLine}Correct WD acknowledge timing window is{Environment.NewLine}LockTime < AckPeriod < LockTime+ResponseTime");
            AddHelpMsg(nameof(WD2LockTime), "WD2 lock time. Acknowledge transactions during this time are erroneous.");
            AddHelpMsg(nameof(WD1EN0DisableThreshold), "EN0 disable threshold for WD1");
            AddHelpMsg(nameof(WD2EN0DisableThreshold), "EN0 disable threshold for WD2");
            AddHelpMsg(nameof(WDFaultStatus), "Top level WD fault flag OR of individual WD fault flags");
            AddHelpMsg(nameof(WD1FaultStatus), "WD1 fault status");
            AddHelpMsg(nameof(WD2FaultStatus), "WD2 fault status");
            AddHelpMsg(nameof(ErrorPinFaultStatus), "Fault status on error pin");
            AddHelpMsg(nameof(WD1TimerFaultStatus), "WD1 acknowledge timing fault flag");
            AddHelpMsg(nameof(WD2TimerFaultStatus), "WD2 acknowledge timing fault flag");
            AddHelpMsg(nameof(OSCMONFaultStatus), "OSCMON fault flag (clock deviation is out of acceptable range)");
            AddHelpMsg(nameof(WD1QAFaultStatus), "WD1 incorrect answer");
            AddHelpMsg(nameof(WD2QAFaultStatus), "WD2 incorrect answer");
            AddHelpMsg(nameof(OscillatorFaultStatus), "Oscillator fault caused previous reset");
            AddHelpMsg(nameof(WDResetFaultStatus), "Watchdog caused previous reset");
            AddHelpMsg(nameof(WD1CounterFaultStatus), "WD1 counter threshold reached");
            AddHelpMsg(nameof(QA1FaultStatus), "WD1 QA fault threshold reached (wrong answer)");
            AddHelpMsg(nameof(WD2CounterFaultStatus), "WD2 counter threshold reached");
            AddHelpMsg(nameof(QA2FaultStatus), "WD2 QA fault threshold reached (wrong answer)");
            AddHelpMsg(nameof(EN0HightStatus), "EN0 pin state");
            AddHelpMsg(nameof(WD1FaultCounter), "WD1 fault counter");
            AddHelpMsg(nameof(WD1TimingMonitorCounter), "WD1 timing monitor results");
            AddHelpMsg(nameof(WD1ErrorEventsCounter), "Number of counted error events");
            AddHelpMsg(nameof(WD1QAFailureCounter), "WD1 QA failure counter value");
            AddHelpMsg(nameof(WD2FaultCounter), "WD2 fault counter");
            AddHelpMsg(nameof(WD2TimingMonitorCounter), "WD2 timing monitor results");
            AddHelpMsg(nameof(WD2QAFailureCounter), "WD2 QA failure counter value");

            // Commands
            AddHelpMsg(nameof(ReadConfigFromASIC), $"Read watchdog configuration stored in ASIC registers.{Environment.NewLine}WARNING: reset cause will be cleared after first read!");
            AddHelpMsg(nameof(WriteConfigToASIC), "Write configuration of WD to ASIC. Config won't be locked");
            AddHelpMsg(nameof(StartWatchdog), "Start watchdog and watchdog status reading. Configuration will be locked");
            AddHelpMsg(nameof(StopWatchdog), "Stop watchdog and watchdog status reading. Configuration will remain locked");

            // UI elements help messages
        }

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
        /// Enable for configuration fields 
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

        /// <summary>
        /// WD1 fault counter
        /// </summary>
        private int wd1FaultCounter;
        public int WD1FaultCounter
        {
            get => wd1FaultCounter;
            set
            {
                wd1FaultCounter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD1 timing monitor results
        /// </summary>
        private int wd1TimingMonitorCounter;
        public int WD1TimingMonitorCounter
        {
            get => wd1TimingMonitorCounter;
            set
            {
                wd1TimingMonitorCounter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Number of error events
        /// </summary>
        private int wd1ErrorEventsCounter;
        public int WD1ErrorEventsCounter
        {
            get => wd1ErrorEventsCounter;
            set
            {
                wd1ErrorEventsCounter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD1 QA failure counter
        /// </summary>
        private int wd1QAFailureCounter;
        public int WD1QAFailureCounter
        {
            get => wd1QAFailureCounter;
            set
            {
                wd1QAFailureCounter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD2 fault counter
        /// </summary>
        private int wd2FaultCounter;
        public int WD2FaultCounter
        {
            get => wd2FaultCounter;
            set
            {
                wd2FaultCounter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD2 timing monitor results
        /// </summary>
        private int wd2TimingMonitorCounter;
        public int WD2TimingMonitorCounter
        {
            get => wd2TimingMonitorCounter;
            set
            {
                wd2TimingMonitorCounter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// WD2 QA failure counter
        /// </summary>
        private int wd2QAFailureCounter;
        public int WD2QAFailureCounter
        {
            get => wd2QAFailureCounter;
            set
            {
                wd2QAFailureCounter = value;
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
        private Reg_spi_read_res_cause _spi_read_res_cause = new Reg_spi_read_res_cause();

        #endregion // Internal_configuration
        
        #region Commands

        /// <summary>
        /// WD monitoring enable status
        /// </summary>
        private bool _wdMonitoringEn = false;

        /// <summary>
        /// Monitoring message ID
        /// </summary>
        private int? _msgIdForMonitoring = null;

        /// <summary>
        /// Read WD config button/command enable state
        /// </summary>
        private CommandState _readWDConfigCommand   = new CommandState();

        /// <summary>
        /// Bindable read WD config button/command enable state
        /// </summary>
        public bool ReadWDConfigCommandEn => _readWDConfigCommand.IsEnabled;

        /// <summary>
        /// Write WD config button/command enable state
        /// </summary>
        private CommandState _writeWDConfigCommand  = new CommandState();

        /// <summary>
        /// Bindable write WD config button/command enable state
        /// </summary>
        public bool WriteWDConfigCommandEn => _writeWDConfigCommand.IsEnabled;

        /// <summary>
        /// Start WD button/command enable state
        /// </summary>
        private CommandState _startWDCommand        = new CommandState();
        
        /// <summary>
        /// Bindable start WD button/command enable state
        /// </summary>
        public bool StartWDCommandEn => _startWDCommand.IsEnabled;

        /// <summary>
        /// Stop WD button/command enable state
        /// </summary>
        private CommandState _stopWDCommand         = new CommandState();
        
        /// <summary>
        /// Bindable stop WD button/command enable state
        /// </summary>
        public bool StopWDCommandEn => _stopWDCommand.IsEnabled;

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
        private async void ReadConfigFromASICExecute(object obj)
        {
            // Handle that command execution can only be done once in a row
            if (_readWDConfigCommand.IsEnabled == false) return;
            _readWDConfigCommand.InProgress = true;
            OnPropertyChanged(nameof(ReadWDConfigCommandEn));

            logger.Debug($"Pressed read config button");
            
            // Create package to MCU
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 1;
            packageToSend.Cmd = MCUCommand.EXECUTE_READ_SEQUENCE;
            packageToSend.PayloadType = typeof(AddressDataPayload);
            packageToSend.Payload.Address.Add(_spi_config_wd1.Address);
            packageToSend.Payload.Address.Add(_spi_config_wd2.Address);
            packageToSend.Payload.Address.Add(_spi_config_wd_decouple.Address);
            packageToSend.Payload.Address.Add(_spi_config_wd_thres0.Address);
            packageToSend.Payload.Address.Add(_spi_set_wdsettings.Address);
            packageToSend.Payload.Address.Add(_spi_read_res_cause.Address);

            // Send command to MCU and wait for response
            ReceiveCommunicationPackage<AddressDataPayload>? mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>?) await serialWrapper.SerialWriteAsync(packageToSend);

            // Response received - unlock command usage
            _readWDConfigCommand.InProgress = false;
            OnPropertyChanged(nameof(ReadWDConfigCommandEn));

            // Change state if response received
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(ReadConfigFromASIC));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
                return;
            }
            else if (mcuResponse.Payload.Data.Count < 6)
            {
                AddError($"Unexpected amount of readout data received. Expected 6, but got {mcuResponse.Payload.Data.Count}.", nameof(ReadConfigFromASIC));
                logger.Error($"Unexpected amount of readout data received. Expected 6, but got {mcuResponse.Payload.Data.Count}.");
                return;
            }

            // Clear errors 
            ClearErrors(nameof(ReadConfigFromASIC));

            // Fire trigger for state machine
            _stateMachine.Fire(Triggers.GotConfiguration);

            // Update configuration
            _spi_config_wd1.Data         = mcuResponse.Payload.Data[0];
            _spi_config_wd2.Data         = mcuResponse.Payload.Data[1];
            _spi_config_wd_decouple.Data = mcuResponse.Payload.Data[2];
            _spi_config_wd_thres0.Data   = mcuResponse.Payload.Data[3];
            _spi_set_wdsettings.Data     = mcuResponse.Payload.Data[4];
            _spi_read_res_cause.Data     = mcuResponse.Payload.Data[5];

            // Unpacking of data for AB15
            WD1ResponseTime = (int) _spi_config_wd1.spi_set_responsetime_wd1.Data;
            WD2ResponseTime = (int) _spi_config_wd2.spi_set_responsetime_wd2.Data;

            WD1LockTime = (int)_spi_config_wd1.spi_set_locktime_wd1.Data;
            WD2LockTime = (int)_spi_config_wd2.spi_set_locktime_wd2.Data;

            WD1EN0DisableThreshold = _spi_config_wd_thres0.spi_set_en0_thre_wd1.Data;
            WD2EN0DisableThreshold = _spi_config_wd_thres0.spi_set_en0_thre_wd2.Data;

            // Warning: Clear on read ASIC registers
            OscillatorFaultStatus = (_spi_read_res_cause.rc_oscfail_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
            WDResetFaultStatus = (_spi_read_res_cause.rc_sl_req_reset_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
            WD1CounterFaultStatus = (_spi_read_res_cause.rc_wd1_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
            WD2CounterFaultStatus = (_spi_read_res_cause.rc_wd2_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
            QA1FaultStatus = (_spi_read_res_cause.rc_qa1_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
            QA2FaultStatus = (_spi_read_res_cause.rc_qa2_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);

            logger.Debug($"Received read config delegate");
        }

        /// <summary>
        /// Execute Write config to ASIC command
        /// </summary>
        private async void WriteConfigToASICExecute(object obj)
        {
            // Handle that command execution can only be done once in a row
            if (_writeWDConfigCommand.IsEnabled == false) return;
            _writeWDConfigCommand.InProgress = true;
            OnPropertyChanged(nameof(WriteWDConfigCommandEn));

            logger.Debug($"Pressed write config button");
            
            // Create package to MCU
            TransmitCommunicationPackage<WDConfigurePayload> packageToSend = new TransmitCommunicationPackage<WDConfigurePayload>();

            packageToSend.ASICID = 1;
            packageToSend.Cmd = MCUCommand.CONFIGURE_WATCHDOG;
            packageToSend.PayloadType = typeof(EmptyPayload);

            // Unpack data from UI to fields of registers
            // Load configs with data from ASIC
            packageToSend.Payload.spi_config_wd1.Data = _spi_config_wd1.Data;
            packageToSend.Payload.spi_config_wd2.Data = _spi_config_wd2.Data;
            packageToSend.Payload.spi_config_wd_decouple.Data = _spi_config_wd_decouple.Data;

            // Modify some fields based on UI input
            packageToSend.Payload.spi_config_wd1.spi_set_locktime_wd1.Data = (UInt16) WD1LockTime;
            packageToSend.Payload.spi_config_wd1.spi_set_responsetime_wd1.Data = (UInt16) WD1ResponseTime;

            packageToSend.Payload.spi_config_wd2.spi_set_locktime_wd2.Data = (UInt16) WD2LockTime;
            packageToSend.Payload.spi_config_wd2.spi_set_responsetime_wd2.Data = (UInt16) WD2ResponseTime;

            packageToSend.Payload.spi_config_wd_decouple.spi_decouple_wd_en0.Data = (UInt16) ((IsEN0Enabled) ? (1) : (0));

            packageToSend.Payload.spi_config_wd_thres0.spi_set_en0_thre_wd1.Data = (UInt16) WD1EN0DisableThreshold;
            packageToSend.Payload.spi_config_wd_thres0.spi_set_en0_thre_wd2.Data = (UInt16) WD2EN0DisableThreshold;

            // Send command to MCU
            ReceiveCommunicationPackage<EmptyPayload>? mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>?) await serialWrapper.SerialWriteAsync(packageToSend);

            logger.Debug($"Received write WD config delegate");

            // Response received - unlock command usage
            _writeWDConfigCommand.InProgress = false;
            OnPropertyChanged(nameof(WriteWDConfigCommandEn));

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(WriteConfigToASIC));
                logger.Error($"Error response received. Status: {mcuResponse.Status}. Message: {mcuResponse.Payload.Error}");
                return;
            }

            // Clear errors 
            ClearErrors(nameof(WriteConfigToASIC));

            _stateMachine.Fire(Triggers.ConfigurationLoaded);
        }

        /// <summary>
        /// Execute Start watchdog command
        /// </summary>
        private async void StartWatchdogExecute(object obj)
        {
            // Handle that command execution can only be done once in a row
            if (_startWDCommand.IsEnabled == false) return;
            _startWDCommand.InProgress = true;
            OnPropertyChanged(nameof(StartWDCommandEn));

            logger.Debug($"Pressed start WD button");
            
            // Create package to MCU
            TransmitCommunicationPackage<EmptyPayload> packageToSendStartWD = new TransmitCommunicationPackage<EmptyPayload>();

            // Configure start watchdog command
            packageToSendStartWD.ASICID = 1;
            packageToSendStartWD.Cmd = MCUCommand.START_WATCHDOG;
            packageToSendStartWD.PayloadType = typeof(EmptyPayload);
       
            // Send start command to MCU
            ReceiveCommunicationPackage<EmptyPayload>? mcuResponseStartWD = (ReceiveCommunicationPackage<EmptyPayload>?) await serialWrapper.SerialWriteAsync(packageToSendStartWD);

            // Response received - unlock command usage
            _startWDCommand.InProgress = false;
            OnPropertyChanged(nameof(StartWDCommandEn));
        
            // Clear errors 
            ClearErrors(nameof(StartWatchdog));

            // If error received - pass it to error provider
            if (mcuResponseStartWD.Payload.Error is not null)
            {
                AddError(mcuResponseStartWD.Payload.Error, nameof(StartWatchdog));
                logger.Error($"Error response received. Status: {mcuResponseStartWD.Status}");
                return;
            }

            _stateMachine.Fire(Triggers.StartedWD);

            logger.Debug($"Received start wd delegate");

            // Start and execute monitoring - execution will stop here till monitoring is stopped
            MonitorWDStatus();
        }

        /// <summary>
        /// Execute Stop watchdog command
        /// </summary>
        private async void StopWatchdogExecute(object obj)
        {
            // Handle that command execution can only be done once in a row
            if (_stopWDCommand.IsEnabled == false) return;
            _stopWDCommand.InProgress = true;
            OnPropertyChanged(nameof(StopWDCommandEn));

            logger.Debug($"Pressed stop WD button");
            
            // Create package to MCU
            TransmitCommunicationPackage<EmptyPayload> packageToSendStopWD = new TransmitCommunicationPackage<EmptyPayload>();

            // Configure start watchdog command
            packageToSendStopWD.ASICID = 1;
            packageToSendStopWD.Cmd = MCUCommand.STOP_WATCHDOG;
            packageToSendStopWD.PayloadType = typeof(EmptyPayload);
       
            // Send start command to MCU
            ReceiveCommunicationPackage<EmptyPayload>? mcuResponseStopWD = (ReceiveCommunicationPackage<EmptyPayload>?) await serialWrapper.SerialWriteAsync(packageToSendStopWD);

            // Response received - unlock command usage
            _stopWDCommand.InProgress = false;
            OnPropertyChanged(nameof(StopWDCommandEn));

            // If error received - pass it to error provider
            if (mcuResponseStopWD.Payload.Error is not null)
            {
                AddError(mcuResponseStopWD.Payload.Error, nameof(StopWatchdog));
                logger.Error($"Error response received. Status: {mcuResponseStopWD.Status}");
                return;
            }

            // Clear errors 
            ClearErrors(nameof(StopWatchdog));

            _stateMachine.Fire(Triggers.StoppedWD);

            logger.Debug($"Received stop wd delegate");

            // Execute stop monitoring
            StopWDStatusMonitoring();
        }

        /// <summary>
        /// Method to start and handle responses from WD monitoring
        /// </summary>
        private async void MonitorWDStatus()
        {
            TransmitCommunicationPackage<EmptyPayload> packageToSendStartMonitoringWD = new TransmitCommunicationPackage<EmptyPayload>();

           // Configure start watchdog status reading command
            packageToSendStartMonitoringWD.ASICID = 1;
            packageToSendStartMonitoringWD.Cmd = MCUCommand.START_MONITORING_WATCHDOG;
            packageToSendStartMonitoringWD.PayloadType = typeof(WDStatusPayload);
            packageToSendStartMonitoringWD.IsContinuous = true;

            Task<IReceiveCommunicationPackage?> responseTask = serialWrapper.SerialWriteAsync(packageToSendStartMonitoringWD);

            // Store message ID for monitoring and set it to enabled
            _msgIdForMonitoring = packageToSendStartMonitoringWD.MsgID;
            _wdMonitoringEn = true;

            ReceiveCommunicationPackage<WDStatusPayload>? mcuResponseMonitoring;
            while (_wdMonitoringEn)
            {
                // Send start status reading command to MCU
                mcuResponseMonitoring = (ReceiveCommunicationPackage<WDStatusPayload>?) await responseTask;

                // Arm next iteration
                responseTask = serialWrapper.GetContinuousTaskInstance((int) _msgIdForMonitoring);

                // Clear errors 
                ClearErrors(nameof(StartWatchdog));

                // If error received - pass it to error provider
                if (mcuResponseMonitoring is null)
                {
                    AddError("Received null when trying to send start monitoring package", nameof(StartWatchdog));
                    logger.Error($"Error response received. Status: fault on sending start wd monitoring");
                    continue;
                }
                if (mcuResponseMonitoring.Payload.Error is not null)
                {
                    AddError(mcuResponseMonitoring.Payload.Error, nameof(StartWatchdog));
                    logger.Error($"Error response received. Status: {mcuResponseMonitoring.Status}");
                    continue;
                }

                // Unpack received data
                WDFaultStatus = (mcuResponseMonitoring.Payload.spi_read_wdstatus2.slff_set_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);

                WD1FaultStatus = (mcuResponseMonitoring.Payload.spi_read_wdstatus1.wd1_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
                WD2FaultStatus = (mcuResponseMonitoring.Payload.spi_read_wdstatus2.wd2_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);

                OSCMONFaultStatus = (mcuResponseMonitoring.Payload.spi_read_wdstatus2.oscfail_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
                // ErrorPinFaultStatus = (mcuResponseMonitoring.Payload != 0) ? (FaultStatus.Fault) : (FaultStatus.Good); // TODO: no field in regmap

                // WD1TimerFaultStatus = (mcuResponseMonitoring.Payload != 0) ? (FaultStatus.Fault) : (FaultStatus.Good); // TODO: no field in regmap
                // WD2TimerFaultStatus = (mcuResponseMonitoring.Payload != 0) ? (FaultStatus.Fault) : (FaultStatus.Good); // TODO: no field in regmap

                WD1QAFaultStatus = (mcuResponseMonitoring.Payload.spi_read_wdqa.qa1_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
                WD2QAFaultStatus = (mcuResponseMonitoring.Payload.spi_read_wdqa.qa2_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);

                EN0HightStatus = (mcuResponseMonitoring.Payload.spi_read_enx.nen0c_read_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);

                WD1FaultCounter = mcuResponseMonitoring.Payload.spi_read_wdstatus1.wd1_cnt_spi.Data;
                WD2FaultCounter = mcuResponseMonitoring.Payload.spi_read_wdstatus2.wd2_cnt_spi.Data;

                WD1TimingMonitorCounter = mcuResponseMonitoring.Payload.spi_read_wdstatus1.wd1_tmon_spi.Data;
                WD2TimingMonitorCounter = mcuResponseMonitoring.Payload.spi_read_wdstatus2.wd2_tmon_spi.Data;

                WD1ErrorEventsCounter = mcuResponseMonitoring.Payload.spi_read_wdstatus1.error_count_spi.Data;
            }
        }

        /// <summary>
        /// Stop WD status monitoring
        /// </summary>
        private async void StopWDStatusMonitoring()
        {
            TransmitCommunicationPackage<EmptyPayload> packageToSendStopMonitoringWD = new TransmitCommunicationPackage<EmptyPayload>();

            // Configure start watchdog status reading command
            packageToSendStopMonitoringWD.ASICID = 1;
            packageToSendStopMonitoringWD.Cmd = MCUCommand.STOP_MONITORING_WATCHDOG;
            packageToSendStopMonitoringWD.PayloadType = typeof(EmptyPayload);
       
            // Send stop monitoring command
            ReceiveCommunicationPackage<EmptyPayload>? mcuResponseStopMonitoringWD = (ReceiveCommunicationPackage<EmptyPayload>?) await serialWrapper.SerialWriteAsync(packageToSendStopMonitoringWD);

            // If error received - pass it to error provider
            if (mcuResponseStopMonitoringWD is null)
            {
                AddError("Received null when trying to send start monitoring package", nameof(StartWatchdog));
                logger.Error($"Error response received. Status: fault on sending start wd monitoring");
                return;
            }
            if (mcuResponseStopMonitoringWD.Payload.Error is not null)
            {
                AddError(mcuResponseStopMonitoringWD.Payload.Error, nameof(StartWatchdog));
                logger.Error($"Error response received. Status: {mcuResponseStopMonitoringWD.Status}");
                return;
            }

            // Store message ID for monitoring and set it to enabled
            serialWrapper.RemoveWaitlistItem(_msgIdForMonitoring);
            _msgIdForMonitoring = null;
            _wdMonitoringEn = false;
        }

        #endregion // Commands
    }
}