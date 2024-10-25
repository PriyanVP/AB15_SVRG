// using System;
// using System.Collections.Generic;
// using NLog;
// using Stateless;
// using Stateless.Graph;
// using System.Windows.Input;
// using AB15_GUI.WPF.ViewModels.Commands;
// using AB15_GUI.WPF.Models.Interfaces;
// using AB15_GUI.WPF.Models;
// using AB15_GUI.WPF.Models.Generated.Registers;
// using AB15_GUI.WPF.Services.Interfaces;

// namespace AB15_GUI.WPF.ViewModels
// {
//     public class FiringViewModel : ViewModelBase
//     {
//         /// <summary>
//         /// Local logger instance
//         /// </summary>
//         private readonly Logger logger;

//         /// <summary>
//         /// SerialWrapper reference to perform communication with MCU
//         /// </summary>
//         private readonly ISerialWrapper serialWrapper;

//         /// <summary>
//         /// Constructor
//         /// </summary>
//         public FiringViewModel(Logger logger, ISerialWrapper serialWrapper)
//         {
//             this.logger = logger;
//             this.serialWrapper = serialWrapper;
//             logger.Trace("In FiringViewModel");

//             // Init help messages for UI
//             InitHelpMessages();

//             // Configure state machine
//             _stateMachine = new StateMachine<State, Triggers>(State.InitialState);

//             // // Emulate initial transition after POR
//             // _stateMachine.Configure(State.InitialState)
//             //              .Permit(Triggers.POR, State.Idle);

//             // _stateMachine.Configure(State.Idle)
//             //              .Permit(Triggers.GotConfiguration, State.InConfiguration);

//             // _stateMachine.Configure(State.InConfiguration)
//             //              .Permit(Triggers.ConfigurationLoaded, State.Configured)
//             //              .Ignore(Triggers.GotConfiguration)
//             //              .Ignore(Triggers.ConfigurationChanged);

//             // _stateMachine.Configure(State.Configured)
//             //              .Permit(Triggers.ConfigurationChanged, State.InConfiguration)
//             //              .Permit(Triggers.StartedWD, State.Running)
//             //              .Ignore(Triggers.GotConfiguration)
//             //              .Ignore(Triggers.StoppedWD);

//             // _stateMachine.Configure(State.Running)
//             //              .Permit(Triggers.StoppedWD, State.InConfiguration);

//             // // Action that will be executed on every state change
//             // _stateMachine.OnTransitionCompleted((transition) => ExecuteStateTransition());

//             // // DOT graph of state machine
//             // // Can be used with debugger to plot state machine visualization
//             // string graph = UmlDotGraph.Format(_stateMachine.GetInfo());

//             // // Init commands for buttons
//             // ReadConfigFromASIC  = new RelayCommand(ReadConfigFromASICExecute);
//             // WriteConfigToASIC   = new RelayCommand(WriteConfigToASICExecute);

//             // StartWatchdog       = new RelayCommand(StartWatchdogExecute);
//             // StopWatchdog        = new RelayCommand(StopWatchdogExecute);

//             // // Fire transition to Idle state
//             // _stateMachine.Fire(Triggers.POR);
//         }

//         #region State_Machine

//         /// <summary>
//         /// State values for WD feature, required for centralized flags handling for UI
//         /// </summary>
//         public enum State
//         {
//             InitialState,          /* Intial dummy state, require to handle initial transition */
//             Idle,                  /* Default state, entered after startup */
//             InConfiguration,       /* State after at least one reading of WD config in ASIC */
//             Configured,            /* State when current WD config is loaded to ASIC */
//             Running                /* WD is running on MCU */
//         }

//         /// <summary>
//         /// State values for WD feature, required for centralized flags handling for UI
//         /// </summary>
//         private enum Triggers
//         {
//             POR,                   /* Transition after startup */
//             GotConfiguration,      /* Got configuration from ASIC */
//             ConfigurationChanged,  /* Configuration in GUI changed */
//             ConfigurationLoaded,   /* Configuration in from GUI loaded to ASIC */
//             StartedWD,             /* WD is started */
//             StoppedWD              /* WD is stopped */
//         }

//         /// <summary>
//         /// State machine to hold state of WD backend and handle transitions
//         /// </summary>
//         private readonly StateMachine<State, Triggers> _stateMachine;

//         /// <summary>
//         /// Property for current state of state machine observation
//         /// Used for testing
//         /// </summary>
//         public State StateObservation => _stateMachine.State;

//         /// <summary>
//         /// Method to update flags in centralized way. All flags updates should be done there
//         /// </summary>
//         private void ExecuteStateTransition()
//         {
//             logger.Debug($"In state machine transition: state {_stateMachine.State}");

//             switch (_stateMachine.State)
//             {
//                 case State.Idle:
//                     // Buttons/commands enable handling
//                     _readWDConfigCommand.Enable   = true;
//                     _writeWDConfigCommand.Enable  = false;
//                     _startWDCommand.Enable        = false;
//                     _stopWDCommand.Enable         = false;

//                     // Configuration enable handling
//                     IsConfigEnable = false;
//                     break;
//                 case State.InConfiguration:
//                     // Buttons/commands enable handling
//                     _readWDConfigCommand.Enable   = true;
//                     _writeWDConfigCommand.Enable  = true;
//                     _startWDCommand.Enable        = false;
//                     _stopWDCommand.Enable         = false;

//                     // Configuration enable handling
//                     IsConfigEnable = true;
//                     break;
//                 case State.Configured:
//                     // Buttons/commands enable handling
//                     _readWDConfigCommand.Enable   = true;
//                     _writeWDConfigCommand.Enable  = true;
//                     _startWDCommand.Enable        = true;
//                     _stopWDCommand.Enable         = true;

//                     // Configuration enable handling
//                     IsConfigEnable = true;
//                     break;
//                 case State.Running:
//                     // Buttons/commands enable handling
//                     _readWDConfigCommand.Enable   = true;
//                     _writeWDConfigCommand.Enable  = false;
//                     _startWDCommand.Enable        = false;
//                     _stopWDCommand.Enable         = true;

//                     // Configuration enable handling
//                     IsConfigEnable = false;
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(_stateMachine.State), "Unexpected state received");
//             }

//             // Request update of buttons states
//             OnPropertyChanged(nameof(ReadWDConfigCommandEn));
//             OnPropertyChanged(nameof(WriteWDConfigCommandEn));
//             OnPropertyChanged(nameof(StartWDCommandEn));
//             OnPropertyChanged(nameof(StopWDCommandEn));
//         }

//         #endregion //State_Machine

//         #region Bindable_Properties

//         private void InitHelpMessages()
//         {
//             // Bindable properties help messages
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");

//             // Commands
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");
//             // AddHelpMsg(nameof(), $"");

//             // UI elements help messages
//         }

//         /// <summary>
//         /// Enable EN0 thresholds configuration
//         /// </summary>
//         private bool isEN0Enabled;
//         public bool IsEN0Enabled
//         {
//             get => isEN0Enabled;
//             set 
//             {
//                 // Do nothing if value is not changed
//                 if (isEN0Enabled == value) return;

//                 isEN0Enabled = value;
//                 OnPropertyChanged();
//                 _stateMachine.Fire(Triggers.ConfigurationChanged);
//             }
//         }

//         /// <summary>
//         /// OSCMON fault flag
//         /// </summary>
//         private FaultStatus oscmonFaultStatus;
//         public FaultStatus OSCMONFaultStatus
//         {
//             get => oscmonFaultStatus;
//             set
//             {
//                 oscmonFaultStatus = value;
//                 OnPropertyChanged();
//             }
//         }

//         #endregion // Bindable_Properties

//         #region Internal_configuration

//         private Reg_spi_config_wd1 _spi_config_wd1 = new Reg_spi_config_wd1();

//         #endregion // Internal_configuration
        
//         #region Commands

//         /// <summary>
//         /// Monitoring message ID
//         /// </summary>
//         private int? _msgIdForMonitoring = null;

//         /// <summary>
//         /// Read WD config button/command enable state
//         /// </summary>
//         private CommandState _readWDConfigCommand   = new CommandState();

//         /// <summary>
//         /// Bindable read WD config button/command enable state
//         /// </summary>
//         public bool ReadWDConfigCommandEn => _readWDConfigCommand.IsEnabled;

//         /// <summary>
//         /// Command handler for Read config from ASIC button
//         /// </summary>
//         public ICommand ReadConfigFromASIC { get; }

//         /// <summary>
//         /// Execute Read config from ASIC command
//         /// </summary>
//         private void ReadConfigFromASICExecute(object obj)
//         {
//             // Handle that command execution can only be done once in a row
//             if (_readWDConfigCommand.IsEnabled == false) return;
//             _readWDConfigCommand.InProgress = true;
//             OnPropertyChanged(nameof(ReadWDConfigCommandEn));

//             logger.Debug($"Pressed read config button");
            
//             // Create package to MCU
//             TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
//             packageToSend.ASICID = 1;
//             // packageToSend.Cmd = MCUCommand. // TODO: add
//             packageToSend.PayloadType = typeof(AddressDataPayload);
//             packageToSend.Payload.Address.Add(_spi_config_wd1.Address);
//             packageToSend.Payload.Address.Add(_spi_config_wd2.Address);
//             packageToSend.Payload.Address.Add(_spi_config_wd_decouple.Address);
//             packageToSend.Payload.Address.Add(_spi_config_wd_thres0.Address);
//             packageToSend.Payload.Address.Add(_spi_set_wdsettings.Address);
//             packageToSend.Payload.Address.Add(_spi_read_res_cause.Address);

//             #if AB12_PLATFORM

//             // Temporary implementation for AB12, replace by actual on AB15
//             ReceiveCommunicationPackage<AddressDataPayload> placeholderPackage = new ReceiveCommunicationPackage<AddressDataPayload>();
//             placeholderPackage.ASICID = 1;
//             placeholderPackage.Status = MCUStatus.DATA;
//             placeholderPackage.Payload.Data.AddRange(new List<UInt16>() { 0, 0, 0, 0, 0, 0 });
//             ReadConfigDelegate(placeholderPackage);

//             #else

//             // Send command to MCU
//             serialWrapper.SerialWrite(packageToSend);

//             #endif
//         }

//         /// <summary>
//         /// Method that will be called when response for read WD config command is received
//         /// </summary>
//         /// <param name="response">MCU response package</param>
//         private void ReadConfigDelegate(IReceiveCommunicationPackage response)
//         {
//             // Response received - unlock command usage
//             _readWDConfigCommand.InProgress = false;
//             OnPropertyChanged(nameof(ReadWDConfigCommandEn));

//             // Typecast response to actual type
//             ReceiveCommunicationPackage<AddressDataPayload> mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>) response;

//             // Change state if response received
//             if (mcuResponse.Payload.Error is not null)
//             {
//                 AddError(mcuResponse.Payload.Error, nameof(ReadConfigFromASIC));
//                 logger.Error($"Error response received. Status: {mcuResponse.Status}");
//                 return;
//             }
//             else if (mcuResponse.Payload.Data.Count < 5)
//             {
//                 AddError($"Unexpected amount of readout data received. Expected 5, but got {mcuResponse.Payload.Data.Count}.", nameof(ReadConfigFromASIC));
//                 logger.Error($"Unexpected amount of readout data received. Expected 5, but got {mcuResponse.Payload.Data.Count}.");
//                 return;
//             }

//             // Clear errors 
//             ClearErrors(nameof(ReadConfigFromASIC));

//             // Fire trigger for state machine
//             _stateMachine.Fire(Triggers.GotConfiguration);

//             // Update configuration
//             _spi_config_wd1.Data         = mcuResponse.Payload.Data[0];
//             _spi_config_wd2.Data         = mcuResponse.Payload.Data[1];
//             _spi_config_wd_decouple.Data = mcuResponse.Payload.Data[2];
//             _spi_config_wd_thres0.Data   = mcuResponse.Payload.Data[3];
//             _spi_set_wdsettings.Data     = mcuResponse.Payload.Data[4];
//             _spi_read_res_cause.Data     = mcuResponse.Payload.Data[5];

//             // Unpacking of data for AB15
//             WD1ResponseTime = (int) _spi_config_wd1.spi_set_responsetime_wd1.Data;
//             WD2ResponseTime = (int) _spi_config_wd2.spi_set_responsetime_wd2.Data;

//             WD1LockTime = (int)_spi_config_wd1.spi_set_locktime_wd1.Data;
//             WD2LockTime = (int)_spi_config_wd2.spi_set_locktime_wd2.Data;

//             // Warning: Clear on read ASIC registers
//             OscillatorFaultStatus = (_spi_read_res_cause.rc_oscfail_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
//             WDResetFaultStatus = (_spi_read_res_cause.rc_sl_req_reset_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
//             WD1CounterFaultStatus = (_spi_read_res_cause.rc_wd1_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
//             WD2CounterFaultStatus = (_spi_read_res_cause.rc_wd2_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
//             QA1FaultStatus = (_spi_read_res_cause.rc_qa1_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);
//             QA2FaultStatus = (_spi_read_res_cause.rc_qa2_set_slff_spi.Data != 0) ? (FaultStatus.Fault) : (FaultStatus.Good);

//             #if AB12_PLATFORM
//             // AB12 code
//             // Values share same step as AB15 scale
//             WD1ResponseTime = 63; 
//             WD2ResponseTime = 16;

//             WD1LockTime = 0;
//             WD2LockTime = 10; // Underflow limit
//             #endif

//             logger.Debug($"Received read config delegate");
//         }

//         #endregion // Commands
//     }
// }