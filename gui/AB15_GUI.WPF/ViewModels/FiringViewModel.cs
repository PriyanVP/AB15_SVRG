using System;
using System.Windows.Input;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.ViewModels.Commands;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Generated.Registers;
using AB15_GUI.WPF.Services.Interfaces;
using System.Collections.ObjectModel;
using Stateless;
using Stateless.Graph;
using System.Collections.Generic;


namespace AB15_GUI.WPF.ViewModels
{
    public class FiringViewModel : ViewModelBase
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
        public FiringViewModel(ILoggingService logger, ISerialWrapper serialWrapper, IASICWrapper asicWrapper)
        {
            // Assign references to objects to local variables
            this.logger = logger;
            this.serialWrapper = serialWrapper;
            this.asicWrapper = asicWrapper;
            logger.Trace("In FiringViewModel");

            // Init help messages for UI
            InitHelpMessages();

           // Configure state machine
            _stateMachine = new StateMachine<State, Triggers>(State.InitialState);

            // Emulate initial transition after POR
            _stateMachine.Configure(State.InitialState)
                         .Permit(Triggers.POR, State.Idle);

            _stateMachine.Configure(State.Idle)
                         .Permit(Triggers.ConfigurationSending, State.InConfiguration)
                         .Permit(Triggers.FatalError, State.Error);

            _stateMachine.Configure(State.InConfiguration)
                         .Permit(Triggers.ConfigurationLoaded, State.Configured)
                         .Permit(Triggers.FatalError, State.Error);

            _stateMachine.Configure(State.Configured)
                         .Permit(Triggers.EnteredNormalMode, State.Running)
                         .Permit(Triggers.FatalError, State.Error);

            _stateMachine.Configure(State.Running)
                         .Permit(Triggers.FatalError, State.Error);

            // Action that will be executed on every state change
            _stateMachine.OnTransitionCompleted((transition) => ExecuteStateTransition());

            // DOT graph of state machine
            // Can be used with debugger to plot state machine visualization
            string graph = UmlDotGraph.Format(_stateMachine.GetInfo());

            // Init commands for controls
            // ConfigurationChanged  = new RelayCommand(ConfigurationChangedExecute);

            // Fire transition to Idle state
            _stateMachine.Fire(Triggers.POR);

            // Events from ASIC // TODO: some of these handlers should be moved out of Firing VM
            this.asicWrapper.ASICs[0].InitModeEntered += InitModeEnteredHandler;
            this.asicWrapper.ASICs[0].ConfigurationLoaded += ConfigurationLoadedHandler;
            this.asicWrapper.ASICs[0].ConfigurationLocked += ConfigurationLockedHandler;
            this.asicWrapper.ASICs[0].NormalModeEntered += NormalModeEnteredHandler;
            this.asicWrapper.ASICs[0].RequestConfiguration += RequestConfigurationHandler;
            this.asicWrapper.ASICs[0].TestMode1Entered += TestMode1EnteredHandler;
            this.asicWrapper.ASICs[0].TestMode2Entered += TestMode2EnteredHandler;
            

            // TODO: remove
            FiringConfigurationTable.Add(new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 1, Mode = 1 });
            FiringConfigurationTable.Add(new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 2, Mode = 2 });
            FiringConfigurationTable.Add(new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 3, Mode = 3 });
            FiringConfigurationTable.Add(new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 4, Mode = 4 });

            FiringResultTable.Add(new FiringResultRecord() { ASICID = 1, ChannelID = 1, ToFire = false, WasFired = true, FiringCntHigh = 0, FiringCntLow = 0 });
            FiringResultTable.Add(new FiringResultRecord() { ASICID = 1, ChannelID = 2, ToFire = false, WasFired = true, FiringCntHigh = 0, FiringCntLow = 0 });
            FiringResultTable.Add(new FiringResultRecord() { ASICID = 1, ChannelID = 3, ToFire = false, WasFired = true, FiringCntHigh = 0, FiringCntLow = 0 });
            FiringResultTable.Add(new FiringResultRecord() { ASICID = 1, ChannelID = 4, ToFire = false, WasFired = true, FiringCntHigh = 0, FiringCntLow = 0 });

        }

        private void TestMode2EnteredHandler(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TestMode1EnteredHandler(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event handler that will be called when before loading configuration to ASIC
        /// If event raised all subscribers provide configuration to central config storage
        /// </summary>
        /// <param name="sender">object that called this event. Must be one of ASIC objects</param>
        /// <param name="e">unused</param>
        private void RequestConfigurationHandler(object? sender, EventArgs e)
        {
            // Precondition
            if (sender == null)
            {
                throw new ArgumentNullException("Incorrect argument - can't be null!");
            }

            // Typecast sender to actual type
            IASIC caller = (IASIC) sender;

            // Append firing configuration to global ASIC configuration
            caller.AppendConfigRegisters(_firingConfig);

            // Unsubscribe from event - by design can't be used twice
            caller.RequestConfiguration -= RequestConfigurationHandler;
        }

        private void NormalModeEnteredHandler(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InitModeEnteredHandler(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ConfigurationLockedHandler(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event handler that will be called when configuration is loaded to ASIC
        /// </summary>
        /// <param name="sender">object that called this event. Must be one of ASIC objects</param>
        /// <param name="e">unused</param>
        private void ConfigurationLoadedHandler(object? sender, EventArgs e)
        {
            // Precondition
            if (sender == null)
            {
                throw new ArgumentNullException("Incorrect argument - can't be null!");
            }
            // Typecast sender to actual type
            IASIC caller = (IASIC) sender;

            // Response received - unlock command usage
            _writeConfigurationCommand.InProgress = false;
            OnPropertyChanged(nameof(WriteConfigurationCommandEn));

            // Fire state machine transition to configuration loaded
            _stateMachine.Fire(Triggers.ConfigurationLoaded);

            // Fill list for table on Firing tab
            // TODO:

            // Unsubscribe from event - by design can be fired only once
            caller.ConfigurationLoaded -= ConfigurationLoadedHandler;
        }

        #region State_Machine

        /// <summary>
        /// State values for firing feature, required for centralized flags handling for UI
        /// </summary>
        public enum State
        {
            InitialState,          /* Initial dummy state, require to handle initial transition */
            Idle,                  /* Default state, entered after startup */
            InConfiguration,       /* State when configuration is being loaded to ASIC */
            Configured,            /* State when configuration was loaded to ASIC */
            Running,               /* ASIC entered normal mode */
            Error                  /* Some error occurred during flow */
        }

        /// <summary>
        /// Triggers for state transitions for firing feature
        /// </summary>
        private enum Triggers
        {
            POR,                   /* Transition after startup */
            ConfigurationSending,   /* Configuration from GUI is being loaded to ASIC */
            ConfigurationLoaded,   /* Configuration from GUI was loaded to ASIC */
            EnteredNormalMode,     /* ASIC entered normal mode */
            FatalError             /* Fatal error occurred - not possible to continue operation. Exit only by reset TODO: verify approach */
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
                    _writeConfigurationCommand.Enable       =
                    _transferToNormalModeCommand.Enable     =
                    _fireSimultaneousCommand.Enable         =
                    _startStopCyclicReadingCommand.Enable   =

                    // Configuration/firing enable handling
                    IsConfigControlsEnabled = true;
                    IsFiringControlsEnabled = false;
                    break;
                case State.InConfiguration:
                    break;
                case State.Configured:
                    break;
                case State.Running:
                    break;
                case State.Error:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_stateMachine.State), $"Unexpected state received {_stateMachine.State}");
            }

            // Request update of buttons states
            OnPropertyChanged(nameof(WriteConfigurationCommandEn));
            OnPropertyChanged(nameof(TransferToNormalModeCommandEn));
            OnPropertyChanged(nameof(FireSimultaneousCommandEn));
            OnPropertyChanged(nameof(StartStopCyclicReadingCommandEn));
        }

        #endregion //State_Machine
        
        #region Bindable_Properties

        /// <summary>
        /// Observable collection for configuration table on Firing tab
        /// </summary>
        public ObservableCollection<FiringChannelConfigurationRecord> FiringConfigurationTable { get; set; } = new ObservableCollection<FiringChannelConfigurationRecord>();

        /// <summary>
        /// Observable collection for firing tab table
        /// </summary>
        public ObservableCollection<FiringResultRecord> FiringResultTable { get; set; } = new ObservableCollection<FiringResultRecord>();


        // TODO: add properties for every needed UI element

        /// <summary>
        /// <inheritdoc cref="IsConfigControlsEnabled" path='/summary'/>
        /// </summary>
        private bool isConfigControlsEnabled;
        
        /// <summary>
        /// Flag to indicate if configuration controls are allowed to be modified
        /// </summary>
        public bool IsConfigControlsEnabled
        {
            get => isConfigControlsEnabled;
            set 
            {
                // Do nothing if value is not changed
                if (isConfigControlsEnabled == value) return;

                isConfigControlsEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="IsFiringControlsEnabled" path='/summary'/>
        /// </summary>
        private bool isFiringControlsEnabled;
        
        /// <summary>
        /// Flag to indicate if firing controls are allowed to be modified (except Fire buttons)
        /// </summary>
        public bool IsFiringControlsEnabled
        {
            get => isFiringControlsEnabled;
            set 
            {
                // Do nothing if value is not changed
                if (isFiringControlsEnabled == value) return;

                isFiringControlsEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="IsVH1aCurrentSinkEn" path='/summary'/>
        /// </summary>
        private bool isVH1aCurrentSinkEn;
        
        /// <summary>
        /// Flag to indicate if VH1a current sink set
        /// </summary>
        public bool IsVH1aCurrentSinkEn
        {
            get => isVH1aCurrentSinkEn;
            set 
            {
                // Do nothing if value is not changed
                if (isVH1aCurrentSinkEn == value) return;

                isVH1aCurrentSinkEn = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="IsVH2CurrentSinkEn" path='/summary'/>
        /// </summary>
        private bool isVH2CurrentSinkEn;
        
        /// <summary>
        /// Flag to indicate if VH2 current sink set
        /// </summary>
        public bool IsVH2CurrentSinkEn
        {
            get => isVH2CurrentSinkEn;
            set 
            {
                // Do nothing if value is not changed
                if (isVH2CurrentSinkEn == value) return;

                isVH2CurrentSinkEn = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="IsLowsideOvercurrentSwitchOffEn" path='/summary'/>
        /// </summary>
        private bool isLowsideOvercurrentSwitchOffEn;
        
        /// <summary>
        /// Flag to indicate if lowside behavior in case of OC fault set to switch off
        /// </summary>
        public bool IsLowsideOvercurrentSwitchOffEn
        {
            get => isLowsideOvercurrentSwitchOffEn;
            set 
            {
                // Do nothing if value is not changed
                if (isLowsideOvercurrentSwitchOffEn == value) return;

                isLowsideOvercurrentSwitchOffEn = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// <inheritdoc cref="IsAlternativeFiringModeEn" path='/summary'/>
        /// </summary>
        private bool isAlternativeFiringModeEn;
        
        /// <summary>
        /// Flag to indicate if alternative firing mode enabled
        /// </summary>
        public bool IsAlternativeFiringModeEn
        {
            get => isAlternativeFiringModeEn;
            set 
            {
                // Do nothing if value is not changed
                if (isAlternativeFiringModeEn == value) return;

                isAlternativeFiringModeEn = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="FiringScenarioIndex" path='/summary'/>
        /// </summary>
        private int firingScenarioIndex;
        
        /// <summary>
        /// Index of currently selected firing scenario
        /// </summary>
        public int FiringScenarioIndex
        {
            get => firingScenarioIndex;
            set 
            {
                // Do nothing if value is not changed
                if (firingScenarioIndex == value) return;

                firingScenarioIndex = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Set messages for help provider on UI
        /// </summary>
        private void InitHelpMessages()
        {
            // Bindable properties help messages
            // AddHelpMsg(nameof(), $"");

            // Commands
            // AddHelpMsg(nameof(), $"");

            // UI elements help messages
        }



        #endregion // Bindable_Properties

        #region Internal_configuration

        /// <summary>
        /// List that holds all configuration for firing
        /// Note: only registers that should be written in INIT mode
        /// </summary>
        private List<IRegister> _firingConfig = new List<IRegister>()
        {
            new Reg_FLM_Config_ch2_1(),
            new Reg_FLM_Config_ch4_3(),
            new Reg_FLM_Config_ch6_5(),
            new Reg_FLM_Config_ch8_7(),
            new Reg_FLM_Config_ch10_9(),
            new Reg_FLM_Config_ch12_11(),
            new Reg_FLM_Config_ch14_13(),
            new Reg_FLM_Config_ch16_15(),
            new Reg_FLM_Config_ch18_17(),
            new Reg_FLM_Config_ch20_19()
        };

        // private Reg_
        // private Reg_
        // private Reg_
        // private Reg_
        // private Reg_
        // private Reg_
        // private Reg_
        // private Reg_
        // private Reg_
        // private Reg_
        // private Reg_
        // private Reg_

        #endregion // Internal_configuration

        #region Commands

        // TODO: 2 buttons, 1 toggle switch - create commands and
        // TODO: enable flag for config
        // TODO: usage of request configuration, init mode entered, normal mode entered events
        // TODO: state machine for flags handling
        // TODO: logic for adding description based on Mode for Config
        // TODO: logic for Test mode 1, 2

        // /// <summary>
        // /// Transfer to Normal mode button/command enable state
        // /// </summary>
        // private CommandState _configurationChangedCommand        = new CommandState();
        
        // /// <summary>
        // /// Bindable  button/command enable state
        // /// </summary>
        // public bool ConfigurationChangedCommandEn => _configurationChangedCommand.IsEnabled;

        /// <summary>
        ///  button/command enable state
        /// </summary>
        private CommandState _writeConfigurationCommand        = new CommandState();
        
        /// <summary>
        /// Bindable  button/command enable state
        /// </summary>
        public bool WriteConfigurationCommandEn => _writeConfigurationCommand.IsEnabled;

        /// <summary>
        ///  button/command enable state
        /// </summary>
        private CommandState _transferToNormalModeCommand        = new CommandState();
        
        /// <summary>
        /// Bindable  button/command enable state
        /// </summary>
        public bool TransferToNormalModeCommandEn => _transferToNormalModeCommand.IsEnabled;

        // /// <summary>
        // ///  button/command enable state
        // /// </summary>
        // private CommandState _firingScenarioChangedCommand        = new CommandState();
        
        // /// <summary>
        // /// Bindable  button/command enable state
        // /// </summary>
        // public bool FiringScenarioChangedCommandEn => _firingScenarioChangedCommand.IsEnabled;

        /// <summary>
        ///  button/command enable state
        /// </summary>
        private CommandState _fireSimultaneousCommand        = new CommandState();
        
        /// <summary>
        /// Bindable  button/command enable state
        /// </summary>
        public bool FireSimultaneousCommandEn => _fireSimultaneousCommand.IsEnabled;

        /// <summary>
        ///  button/command enable state
        /// </summary>
        private CommandState _startStopCyclicReadingCommand        = new CommandState();

        /// <summary>
        /// Bindable  button/command enable state
        /// </summary>
        public bool StartStopCyclicReadingCommandEn => _startStopCyclicReadingCommand.IsEnabled;

        // /// <summary>
        // /// Command handler for changing configuration
        // /// </summary>
        // public ICommand ConfigurationChanged { get; }

        /// <summary>
        /// Command handler for executing transferring to Normal mode sequence
        /// </summary>
        public ICommand TransferToNormalMode { get; }

        // /// <summary>
        // /// Command handler for changing firing scenario
        // /// </summary>
        // public ICommand FiringScenarioChanged { get; }

        /// <summary>
        /// Command handler for executing firing simultaneously
        /// </summary>
        public ICommand FireSimultaneous { get; }

        /// <summary>
        /// Command handler for starting/stopping cyclic FLM data reading
        /// </summary>
        public ICommand StartStopCyclicReading { get; }

        /// <summary>
        /// Execute change of configuration
        /// </summary>
        private void ConfigurationChanged(object commandParameter)
        {
            // // Handle that command execution can only be done once in a row
            // if (_readWDConfigCommand.IsEnabled == false) return;
            // _readWDConfigCommand.InProgress = true;
            // OnPropertyChanged(nameof(ReadWDConfigCommandEn));

            // logger.Debug($"Pressed read config button");

            // Typecast parameter from View to actual type
            string selectedScenario = (string) commandParameter;

            // Choosing preset configuration based on selected option
            switch (selectedScenario)
            {
                // TODO: add implementation
                case "A - 1":
                    break;
                case "B - 2":
                    break;
                case "C - 2":
                    break;
                case "D - 2":
                    break;
                case "E - 10":
                    break;
                case "F - 20":
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected scenario selected: {selectedScenario}");
            }
        }

        /// <summary>
        /// Execute change of firing scenario
        /// </summary>
        private void FiringScenarioChangedExecute(object commandParameter)
        {
            // // Handle that command execution can only be done once in a row
            // if (_readWDConfigCommand.IsEnabled == false) return;
            // _readWDConfigCommand.InProgress = true;
            // OnPropertyChanged(nameof(ReadWDConfigCommandEn));

            // logger.Debug($"Pressed read config button");

            // Typecast parameter from View to actual type
            string selectedScenario = (string) commandParameter;

            // Choosing preset configuration based on selected option
            switch (selectedScenario)
            {
                // TODO: add implementation
                case "A - 1":
                    break;
                case "B - 2":
                    break;
                case "C - 2":
                    break;
                case "D - 2":
                    break;
                case "E - 10":
                    break;
                case "F - 20":
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected scenario selected: {selectedScenario}");
            }
        }

        /// <summary>
        /// Execute Write config to ASIC command
        /// Note: will write ALL configuration and apply cyclic CRC
        /// </summary>
        private void WriteConfigToASICExecute(object obj)
        {
            // Handle that command execution can only be done once in a row
            if (_writeConfigurationCommand.IsEnabled == false) return;
            _writeConfigurationCommand.InProgress = true;
            OnPropertyChanged(nameof(WriteConfigurationCommandEn));

            logger.Debug($"Pressed write config button");

            // Fire state machine trigger to lock UI controls
            _stateMachine.Fire(Triggers.ConfigurationSending);

            // Call corresponding ASIC methods to write configuration
            asicWrapper.ASICs[0].OnRequestConfiguration();      // Raise event to request configuration from all subscribers TODO: verify if config data is updated
            asicWrapper.ASICs[0].WriteConfigurationWithCRC(); 
        }








        #endregion // Commands
    }
}
