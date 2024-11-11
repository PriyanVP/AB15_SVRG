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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            WriteConfiguration      = new RelayCommand(WriteConfigToASICExecute);
            TransferToNormalMode    = new RelayCommand(TransferToNormalModeExecute);
            FireSimultaneous        = new RelayCommand(FireSimultaneousCommandExecute);
            StartStopCyclicReading  = new RelayCommand(StartStopCyclicReadingExecute);

            // Fire transition to Idle state
            _stateMachine.Fire(Triggers.POR);

            // Events from ASIC // TODO: some of these handlers should be moved out of Firing VM
            // this.asicWrapper.ASICs[0].InitModeEntered += InitModeEnteredHandler; // TODO: add in future 
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
                    _writeConfigurationCommand.Enable       = true;
                    _transferToNormalModeCommand.Enable     = false;
                    _fireSimultaneousCommand.Enable         = false;
                    _startStopCyclicReadingCommand.Enable   = true;

                    // Configuration/firing enable handling
                    IsConfigControlsEnabled = true;
                    IsFiringControlsEnabled = false;
                    break;

                case State.InConfiguration:
                    // Buttons/commands enable handling
                    _writeConfigurationCommand.Enable       = false;
                    _transferToNormalModeCommand.Enable     = false;
                    _fireSimultaneousCommand.Enable         = false;
                    _startStopCyclicReadingCommand.Enable   = true;

                    // Configuration/firing enable handling
                    IsConfigControlsEnabled = false;
                    IsFiringControlsEnabled = false;
                    break;

                case State.Configured:
                    // Buttons/commands enable handling
                    _writeConfigurationCommand.Enable       = false;
                    _transferToNormalModeCommand.Enable     = true;
                    _fireSimultaneousCommand.Enable         = false;
                    _startStopCyclicReadingCommand.Enable   = true;

                    // Configuration/firing enable handling
                    IsConfigControlsEnabled = false;
                    IsFiringControlsEnabled = false;
                    break;

                case State.Running:
                    // Buttons/commands enable handling
                    _writeConfigurationCommand.Enable       = false;
                    _transferToNormalModeCommand.Enable     = false;
                    _fireSimultaneousCommand.Enable         = true;
                    _startStopCyclicReadingCommand.Enable   = true;

                    // Configuration/firing enable handling
                    IsConfigControlsEnabled = false;
                    IsFiringControlsEnabled = true;
                    break;

                case State.Error:
                    // Buttons/commands enable handling
                    _writeConfigurationCommand.Enable       = false;
                    _transferToNormalModeCommand.Enable     = false;
                    _fireSimultaneousCommand.Enable         = false;
                    _startStopCyclicReadingCommand.Enable   = false;

                    // Configuration/firing enable handling
                    IsConfigControlsEnabled = false;
                    IsFiringControlsEnabled = false;
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

        // TODO: add properties for every needed UI element - monitoring page tables missing (add model)

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
        /// <inheritdoc cref="FiringConfigurationIndex" path='/summary'/>
        /// </summary>
        private int firingConfigurationIndex;
        
        /// <summary>
        /// Index of currently selected firing scenario
        /// </summary>
        public int FiringConfigurationIndex
        {
            get => firingConfigurationIndex;
            set 
            {
                // Do nothing if value is not changed
                if (firingConfigurationIndex == value) return;

                // Update property
                firingConfigurationIndex = value;
                OnPropertyChanged();

                // Call method to update table
                ConfigurationChanged();
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

                ClearErrors();

                // Validate if selected scenario is applicable
                switch (value)
                {
                    case 0:
                        if (FiringConfigurationIndex != 0)
                        {
                            AddError($"Scenario {value} is not applicable for current configuration", nameof(FiringScenarioIndex));
                            return;
                        }
                        break;
                    case 1:
                    case 2:
                        if (FiringConfigurationIndex != 1)
                        {
                            AddError($"Scenario {value} is not applicable for current configuration", nameof(FiringScenarioIndex));
                            return;
                        }
                        break;
                    case 3:
                    case 4:
                    case 5:
                        if (FiringConfigurationIndex != 2)
                        {
                            AddError($"Scenario {value} is not applicable for current configuration", nameof(FiringScenarioIndex));
                            return;
                        }
                        break;
                    case 6:
                    case 7:
                        if (FiringConfigurationIndex != 3)
                        {
                            AddError($"Scenario {value} is not applicable for current configuration", nameof(FiringScenarioIndex));
                            return;
                        }
                        break;
                    case 8:
                    case 9:
                        if (FiringConfigurationIndex != 4)
                        {
                            AddError($"Scenario {value} is not applicable for current configuration", nameof(FiringScenarioIndex));
                            return;
                        }
                        break;
                    case 10:
                        if (FiringConfigurationIndex != 5)
                        {
                            AddError($"Scenario {value} is not applicable for current configuration", nameof(FiringScenarioIndex));
                            return;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unexpected index value for firing scenario: {value}");
                }

                // Update property
                firingScenarioIndex = value;
                OnPropertyChanged();

                // Call method to update table
                FiringScenarioChanged();
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
            new Reg_FLM_Config_ch2_1()   { Data = 0x0000 },
            new Reg_FLM_Config_ch4_3()   { Data = 0x0000 },
            new Reg_FLM_Config_ch6_5()   { Data = 0x0000 },
            new Reg_FLM_Config_ch8_7()   { Data = 0x0000 },
            new Reg_FLM_Config_ch10_9()  { Data = 0x0000 },
            new Reg_FLM_Config_ch12_11() { Data = 0x0000 },
            new Reg_FLM_Config_ch14_13() { Data = 0x0000 },
            new Reg_FLM_Config_ch16_15() { Data = 0x0000 },
            new Reg_FLM_Config_ch18_17() { Data = 0x0000 },
            new Reg_FLM_Config_ch20_19() { Data = 0x0000 }
        };

        /// <summary>
        /// Collects data from UI and fills list with internal configuration
        /// </summary>
        private void CollectConfiguration()
        {
            // General configuration
            // Get register for writing general configuration - will modify data in list (reference to reg is used)
            Reg_FLM_Config_ch2_1 regWithGeneralConfigFields = (Reg_FLM_Config_ch2_1) _firingConfig.Find(x => x.Name == "FLM_Config_ch2_1");
            regWithGeneralConfigFields.flm_vh1a_source_disable.Data = (IsVH1aCurrentSinkEn) ? ((ushort) 0) : ((ushort) 1);
            regWithGeneralConfigFields.flm_vh2_source_disable.Data = (IsVH2CurrentSinkEn) ? ((ushort) 0) : ((ushort) 1);
            regWithGeneralConfigFields.flm_oc_behavior_sel.Data = (IsLowsideOvercurrentSwitchOffEn) ? ((ushort) 1) : ((ushort) 0); // TODO: check mapping to UI

            // Channel configuration
            foreach (FiringChannelConfigurationRecord channelConfigRecord in FiringConfigurationTable)
            {
                // Get reference to correct register // TODO: can be optimized
                switch (channelConfigRecord.ChannelID)
                {
                    case 1:
                        Reg_FLM_Config_ch2_1 reg_ch1 = (Reg_FLM_Config_ch2_1) _firingConfig.Find(x => x.Name == "FLM_Config_ch2_1");
                        reg_ch1.flm_mode_ch1.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch1.flm_mode_parity_ch1.Data = (UInt16) (reg_ch1.flm_mode_ch1.Data % 2); // TODO: verify approach
                        break;
                    case 2:
                        Reg_FLM_Config_ch2_1 reg_ch2 = (Reg_FLM_Config_ch2_1) _firingConfig.Find(x => x.Name == "FLM_Config_ch2_1");
                        reg_ch2.flm_mode_ch2.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch2.flm_mode_parity_ch2.Data = (UInt16) (reg_ch2.flm_mode_ch2.Data % 2);
                        break;
                    case 3:
                        Reg_FLM_Config_ch4_3 reg_ch3 = (Reg_FLM_Config_ch4_3) _firingConfig.Find(x => x.Name == "FLM_Config_ch4_3");
                        reg_ch3.flm_mode_ch3.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch3.flm_mode_parity_ch3.Data = (UInt16) (reg_ch3.flm_mode_ch3.Data % 2);
                        break;
                    case 4:
                        Reg_FLM_Config_ch4_3 reg_ch4 = (Reg_FLM_Config_ch4_3) _firingConfig.Find(x => x.Name == "FLM_Config_ch4_3");
                        reg_ch4.flm_mode_ch4.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch4.flm_mode_parity_ch4.Data = (UInt16) (reg_ch4.flm_mode_ch4.Data % 2);
                        break;
                    case 5:
                        Reg_FLM_Config_ch6_5 reg_ch5 = (Reg_FLM_Config_ch6_5) _firingConfig.Find(x => x.Name == "FLM_Config_ch6_5");
                        reg_ch5.flm_mode_ch5.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch5.flm_mode_parity_ch5.Data = (UInt16) (reg_ch5.flm_mode_ch5.Data % 2);
                        break;
                    case 6:
                        Reg_FLM_Config_ch6_5 reg_ch6 = (Reg_FLM_Config_ch6_5) _firingConfig.Find(x => x.Name == "FLM_Config_ch6_5");
                        reg_ch6.flm_mode_ch6.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch6.flm_mode_parity_ch6.Data = (UInt16) (reg_ch6.flm_mode_ch6.Data % 2);
                        break;
                    case 7:
                        Reg_FLM_Config_ch8_7 reg_ch7 = (Reg_FLM_Config_ch8_7) _firingConfig.Find(x => x.Name == "FLM_Config_ch8_7");
                        reg_ch7.flm_mode_ch7.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch7.flm_mode_parity_ch7.Data = (UInt16) (reg_ch7.flm_mode_ch7.Data % 2);
                        break;
                    case 8:
                        Reg_FLM_Config_ch8_7 reg_ch8 = (Reg_FLM_Config_ch8_7) _firingConfig.Find(x => x.Name == "FLM_Config_ch8_7");
                        reg_ch8.flm_mode_ch8.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch8.flm_mode_parity_ch8.Data = (UInt16) (reg_ch8.flm_mode_ch8.Data % 2);
                        break;
                    case 9:
                        Reg_FLM_Config_ch10_9 reg_ch9 = (Reg_FLM_Config_ch10_9) _firingConfig.Find(x => x.Name == "FLM_Config_ch10_9");
                        reg_ch9.flm_mode_ch9.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch9.flm_mode_parity_ch9.Data = (UInt16) (reg_ch9.flm_mode_ch9.Data % 2);
                        break;
                    case 10:
                        Reg_FLM_Config_ch10_9 reg_ch10 = (Reg_FLM_Config_ch10_9) _firingConfig.Find(x => x.Name == "FLM_Config_ch10_9");
                        reg_ch10.flm_mode_ch10.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch10.flm_mode_parity_ch10.Data = (UInt16) (reg_ch10.flm_mode_ch10.Data % 2);
                        break;
                    case 11:
                        Reg_FLM_Config_ch12_11 reg_ch11 = (Reg_FLM_Config_ch12_11) _firingConfig.Find(x => x.Name == "FLM_Config_ch12_11");
                        reg_ch11.flm_mode_ch11.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch11.flm_mode_parity_ch11.Data = (UInt16) (reg_ch11.flm_mode_ch11.Data % 2);
                        break;
                    case 12:
                        Reg_FLM_Config_ch12_11 reg_ch12 = (Reg_FLM_Config_ch12_11) _firingConfig.Find(x => x.Name == "FLM_Config_ch12_11");
                        reg_ch12.flm_mode_ch12.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch12.flm_mode_parity_ch12.Data = (UInt16) (reg_ch12.flm_mode_ch12.Data % 2);
                        break; 
                    case 13:
                        Reg_FLM_Config_ch14_13 reg_ch13 = (Reg_FLM_Config_ch14_13) _firingConfig.Find(x => x.Name == "FLM_Config_ch14_13");
                        reg_ch13.flm_mode_ch13.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch13.flm_mode_parity_ch13.Data = (UInt16) (reg_ch13.flm_mode_ch13.Data % 2);
                        break;
                    case 14:
                        Reg_FLM_Config_ch14_13 reg_ch14 = (Reg_FLM_Config_ch14_13) _firingConfig.Find(x => x.Name == "FLM_Config_ch14_13");
                        reg_ch14.flm_mode_ch14.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch14.flm_mode_parity_ch14.Data = (UInt16) (reg_ch14.flm_mode_ch14.Data % 2);
                        break;
                    case 15:
                        Reg_FLM_Config_ch16_15 reg_ch15 = (Reg_FLM_Config_ch16_15) _firingConfig.Find(x => x.Name == "FLM_Config_ch16_15");
                        reg_ch15.flm_mode_ch15.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch15.flm_mode_parity_ch15.Data = (UInt16) (reg_ch15.flm_mode_ch15.Data % 2);
                        break;
                    case 16:
                        Reg_FLM_Config_ch16_15 reg_ch16 = (Reg_FLM_Config_ch16_15) _firingConfig.Find(x => x.Name == "FLM_Config_ch16_15");
                        reg_ch16.flm_mode_ch16.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch16.flm_mode_parity_ch16.Data = (UInt16) (reg_ch16.flm_mode_ch16.Data % 2);
                        break;  
                    case 17:
                        Reg_FLM_Config_ch18_17 reg_ch17 = (Reg_FLM_Config_ch18_17) _firingConfig.Find(x => x.Name == "FLM_Config_ch18_17");
                        reg_ch17.flm_mode_ch17.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch17.flm_mode_parity_ch17.Data = (UInt16) (reg_ch17.flm_mode_ch17.Data % 2);
                        break;
                    case 18:
                        Reg_FLM_Config_ch18_17 reg_ch18 = (Reg_FLM_Config_ch18_17) _firingConfig.Find(x => x.Name == "FLM_Config_ch18_17");
                        reg_ch18.flm_mode_ch18.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch18.flm_mode_parity_ch18.Data = (UInt16) (reg_ch18.flm_mode_ch18.Data % 2);
                        break;
                    case 19:
                        Reg_FLM_Config_ch20_19 reg_ch19 = (Reg_FLM_Config_ch20_19) _firingConfig.Find(x => x.Name == "FLM_Config_ch20_19");
                        reg_ch19.flm_mode_ch19.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch19.flm_mode_parity_ch19.Data = (UInt16) (reg_ch19.flm_mode_ch19.Data % 2);
                        break;
                    case 20:
                        Reg_FLM_Config_ch20_19 reg_ch20 = (Reg_FLM_Config_ch20_19) _firingConfig.Find(x => x.Name == "FLM_Config_ch20_19");
                        reg_ch20.flm_mode_ch20.Data = (UInt16) channelConfigRecord.Mode;
                        reg_ch20.flm_mode_parity_ch20.Data = (UInt16) (reg_ch20.flm_mode_ch20.Data % 2);
                        break; 
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected channel ID! Should be in rage 1-20");             
                }
            }
        }

        #endregion // Internal_configuration

        #region Commands

        // TODO: 2 buttons, 1 toggle switch - create commands and handlers
        // TODO: logic for adding description based on Mode for Config -> hard to do, can be done manually (in model)

        /// <summary>
        /// Command handler for executing transferring to Normal mode sequence
        /// </summary>
        public ICommand WriteConfiguration { get; }

        /// <summary>
        /// Command handler for executing transferring to Normal mode sequence
        /// </summary>
        public ICommand TransferToNormalMode { get; }

        /// <summary>
        /// Command handler for executing firing simultaneously
        /// </summary>
        public ICommand FireSimultaneous { get; }

        /// <summary>
        /// Command handler for starting/stopping cyclic FLM data reading
        /// </summary>
        public ICommand StartStopCyclicReading { get; }

        /// <summary>
        /// Write configuration button/command enable state
        /// </summary>
        private CommandState _writeConfigurationCommand        = new CommandState();
        
        /// <summary>
        /// Bindable Write configuration button/command enable state
        /// </summary>
        public bool WriteConfigurationCommandEn => _writeConfigurationCommand.IsEnabled;

        /// <summary>
        /// Transfer to normal mode button/command enable state
        /// </summary>
        private CommandState _transferToNormalModeCommand        = new CommandState();
        
        /// <summary>
        /// Bindable Transfer to normal mode button/command enable state
        /// </summary>
        public bool TransferToNormalModeCommandEn => _transferToNormalModeCommand.IsEnabled;

        /// <summary>
        /// Fire simultaneous button/command enable state
        /// </summary>
        private CommandState _fireSimultaneousCommand        = new CommandState();
        
        /// <summary>
        /// Bindable Fire simultaneous button/command enable state
        /// </summary>
        public bool FireSimultaneousCommandEn => _fireSimultaneousCommand.IsEnabled;

        /// <summary>
        /// Start/stop button/command enable state
        /// </summary>
        private CommandState _startStopCyclicReadingCommand        = new CommandState();

        /// <summary>
        /// Bindable Start/stop button/command enable state
        /// </summary>
        public bool StartStopCyclicReadingCommandEn => _startStopCyclicReadingCommand.IsEnabled;

        /// <summary>
        /// Execute change of configuration // TODO: change approach for more flexible in future (no predefined scenarios, but full customization)
        /// </summary>
        private void ConfigurationChanged()
        {
            logger.Debug($"Changed configuration in drop-down on Configuration tab");

            // Clear list
            FiringConfigurationTable.Clear();

            // Choosing preset configuration based on selected option
            switch (FiringConfigurationIndex)
            {
                case 0:
                    // Scenario A - 1
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 1, Mode = 0x2 } );
                    break;
                case 1:
                    // Scenario B - 2
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 1, Mode = 0x2 } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 2, Mode = 0x2 } );
                    break;
                case 2:
                    // Scenario C - 2
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 1, Mode = 0xC } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 2, Mode = 0xC } );
                    break;
                case 3:
                    // Scenario D - 2
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 1, Mode = 0x2 } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 5, Mode = 0x2 } );
                    break;
                case 4:
                    // Scenario E - 10
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 1, Mode = 0x2 } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 2, Mode = 0x2 } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 5, Mode = 0x2 } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 6, Mode = 0x2 } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 9, Mode = 0x2 } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 10, Mode = 0x2 } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 13, Mode = 0x2 } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 14, Mode = 0x2 } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 17, Mode = 0x2 } );
                    FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = 18, Mode = 0x2 } );
                    break;
                case 5:
                    // Scenario F - 20
                    for (int i = 1; i < 21; i++)
                    {
                        FiringConfigurationTable.Add( new FiringChannelConfigurationRecord() { ASICID = 1, ChannelID = i, Mode = 0x2 } );
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected scenario selected: {FiringConfigurationIndex}");
            }
        }

        /// <summary>
        /// Execute change of firing scenario // TODO: add implementation
        /// </summary>
        private void FiringScenarioChanged()
        {
            logger.Debug($"Changed firing scenario in drop-down on Configuration tab");

            // Reset flags
            for (int i = 0; i < 20; i++)
            {
                FiringResultTable[i].WasFired = false;
                FiringResultTable[i].ToFire = false;
            }

            // Choosing preset configuration based on selected option
            switch (FiringScenarioIndex)
            {
                case 0:
                    // A.1
                    FiringResultTable[0].ToFire = true;
                    break;
                case 1:
                    // B.1
                    FiringResultTable[0].ToFire = true;
                    break;
                case 2:
                    // B.2
                    FiringResultTable[0].ToFire = true;
                    FiringResultTable[1].ToFire = true;
                    break;
                case 3:
                    // C.1
                    FiringResultTable[0].ToFire = true;
                    break;
                case 4:
                    // C.1a - now requires user to change Alternative firing mode switch
                    FiringResultTable[0].ToFire = true;
                    break;
                case 5:
                    // C.2a - now requires user to change Alternative firing mode switch
                    FiringResultTable[0].ToFire = true;
                    FiringResultTable[1].ToFire = true;
                    break;
                case 6:
                    // D.1
                    FiringResultTable[0].ToFire = true;
                    break;
                case 7:
                    // D.2
                    FiringResultTable[0].ToFire = true;
                    FiringResultTable[1].ToFire = true;
                    break;
                case 8:
                    // E.5
                    FiringResultTable[0].ToFire = true;
                    FiringResultTable[2].ToFire = true;
                    FiringResultTable[4].ToFire = true;
                    FiringResultTable[6].ToFire = true;
                    FiringResultTable[8].ToFire = true;
                    break;
                case 9:
                    // E.10
                    for (int i = 0; i < 10; i++)
                    {
                        FiringResultTable[i].ToFire = true;
                    }
                    break;
                case 10:
                    // F.20
                    for (int i = 0; i < 20; i++)
                    {
                        FiringResultTable[i].ToFire = true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected index value for firing scenario: {FiringScenarioIndex}");
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

            // Collect configuration from UI
            CollectConfiguration();

            // Call corresponding ASIC methods to write configuration
            asicWrapper.ASICs[0].OnRequestConfiguration();      // Raise event to request configuration from all subscribers TODO: verify if config data is updated
            asicWrapper.ASICs[0].WriteConfigurationWithCRC(); 
        }

        /// <summary>
        /// Execute Transfer to Normal mode ASIC command // TODO: find better place
        /// </summary>
        private void TransferToNormalModeExecute(object obj)
        {
            // Handle that command execution can only be done once in a row
            if (_transferToNormalModeCommand.IsEnabled == false) return;
            _transferToNormalModeCommand.InProgress = true;
            OnPropertyChanged(nameof(TransferToNormalModeCommandEn));

            logger.Debug($"Pressed Transfer to Normal mode button");

            // Call corresponding ASIC methods to cause start of transferring to normal mode sequence
            asicWrapper.ASICs[0].LockConfiguration();      // Raise event to request configuration from all subscribers TODO: verify if config data is updated
        }

        private TaskCompletionSource<bool>? _taskCompletionSource = null;

        /// <summary>
        /// Execute Firing simultaneous command
        /// </summary>
        private async void FireSimultaneousCommandExecute(object obj)
        {
            // Handle that command execution can only be done once in a row
            if (_fireSimultaneousCommand.IsEnabled == false) return;
            _fireSimultaneousCommand.InProgress = true;
            OnPropertyChanged(nameof(FireSimultaneousCommandEn));

            logger.Debug($"Pressed Fire simultaneous button");

            // == Step 1 - unlocking ==

            // Initialize a new TaskCompletionSource instance for each call
            _taskCompletionSource = new TaskCompletionSource<bool>();

            // Get indexes of configured channels
            List<int> channelIndexes = FiringResultTable.Where(itm => itm.ToFire)
                                                        .Select(itm => itm.ChannelID).ToList();

            // Unlock FLM modules if there are channels to fire in these modules
            Reg_FLM_Unlock reg_FLM_Unlock = new Reg_FLM_Unlock();
            reg_FLM_Unlock.Data = 0x0000;
            reg_FLM_Unlock.flm_unlock_hs_module1.Data = Convert.ToUInt16(channelIndexes.Any(itm => ((itm >= 1) && (itm <= 4))));
            reg_FLM_Unlock.flm_unlock_ls_module1.Data = reg_FLM_Unlock.flm_unlock_hs_module1.Data;
            reg_FLM_Unlock.flm_unlock_hs_module2.Data = Convert.ToUInt16(channelIndexes.Any(itm => ((itm >= 5) && (itm <= 8))));
            reg_FLM_Unlock.flm_unlock_ls_module2.Data = reg_FLM_Unlock.flm_unlock_hs_module2.Data;
            reg_FLM_Unlock.flm_unlock_hs_module3.Data = Convert.ToUInt16(channelIndexes.Any(itm => ((itm >= 9) && (itm <= 12))));
            reg_FLM_Unlock.flm_unlock_ls_module3.Data = reg_FLM_Unlock.flm_unlock_hs_module3.Data;
            reg_FLM_Unlock.flm_unlock_hs_module4.Data = Convert.ToUInt16(channelIndexes.Any(itm => ((itm >= 13) && (itm <= 16))));
            reg_FLM_Unlock.flm_unlock_ls_module4.Data = reg_FLM_Unlock.flm_unlock_hs_module4.Data;
            reg_FLM_Unlock.flm_unlock_hs_module5.Data = Convert.ToUInt16(channelIndexes.Any(itm => ((itm >= 17) && (itm <= 20))));
            reg_FLM_Unlock.flm_unlock_ls_module5.Data = reg_FLM_Unlock.flm_unlock_hs_module5.Data;

            // Set alternative mode if enabled
            reg_FLM_Unlock.flm_fire_mode_sel.Data = Convert.ToUInt16(IsAlternativeFiringModeEn);

            // Set code unlock
            reg_FLM_Unlock.flm_code_unlock.Data = 0x00;

            // Create package to MCU
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 1;
            packageToSend.Cmd = MCUCommand.WRITE_REG;
            packageToSend.Deleg = FireSimultaneousDelegate_step1;
            packageToSend.PayloadType = typeof(AddressDataPayload);
            packageToSend.Payload.Address.Add(reg_FLM_Unlock.Address);
            packageToSend.Payload.Data.Add(reg_FLM_Unlock.Data);

            // Send command to MCU
            serialWrapper.SerialWrite(packageToSend);

            // Wait asynchronously without blocking the main thread for completing step
            await _taskCompletionSource.Task;  

            // == Step 2 - firing ==

            // Initialize a new TaskCompletionSource instance for each call
            _taskCompletionSource = new TaskCompletionSource<bool>();

            // Registers for firing
            Reg_FLM_HS_LS_on_ch7_1   flm_HS_LS_On_Ch7_1   = new Reg_FLM_HS_LS_on_ch7_1();
            Reg_FLM_HS_LS_on_ch14_8  flm_HS_LS_On_Ch14_8  = new Reg_FLM_HS_LS_on_ch14_8();
            Reg_FLM_HS_LS_on_ch20_15 flm_HS_LS_On_Ch20_15 = new Reg_FLM_HS_LS_on_ch20_15();

            // Fill registers // TODO: find better way
            foreach (var channelIdx in channelIndexes)
            {
                switch (channelIdx)
                {
                    case 1:
                        flm_HS_LS_On_Ch7_1.flm_hs_on_ch1.Data = 0x1;
                        flm_HS_LS_On_Ch7_1.flm_ls_on_ch1.Data = 0x1;
                        break;
                    case 2:
                        flm_HS_LS_On_Ch7_1.flm_hs_on_ch2.Data = 0x1;
                        flm_HS_LS_On_Ch7_1.flm_ls_on_ch2.Data = 0x1;
                        break;
                    case 3:
                        flm_HS_LS_On_Ch7_1.flm_hs_on_ch3.Data = 0x1;
                        flm_HS_LS_On_Ch7_1.flm_ls_on_ch3.Data = 0x1;
                        break;
                    case 4:
                        flm_HS_LS_On_Ch7_1.flm_hs_on_ch4.Data = 0x1;
                        flm_HS_LS_On_Ch7_1.flm_ls_on_ch4.Data = 0x1;
                        break;
                    case 5:
                        flm_HS_LS_On_Ch7_1.flm_hs_on_ch5.Data = 0x1;
                        flm_HS_LS_On_Ch7_1.flm_ls_on_ch5.Data = 0x1;
                        break;
                    case 6:
                        flm_HS_LS_On_Ch7_1.flm_hs_on_ch6.Data = 0x1;
                        flm_HS_LS_On_Ch7_1.flm_ls_on_ch6.Data = 0x1;
                        break;
                    case 7:
                        flm_HS_LS_On_Ch7_1.flm_hs_on_ch7.Data = 0x1;
                        flm_HS_LS_On_Ch7_1.flm_ls_on_ch7.Data = 0x1;
                        break;
                    case 8:
                        flm_HS_LS_On_Ch14_8.flm_hs_on_ch8.Data = 0x1;
                        flm_HS_LS_On_Ch14_8.flm_ls_on_ch8.Data = 0x1;
                        break;
                    case 9:
                        flm_HS_LS_On_Ch14_8.flm_hs_on_ch9.Data = 0x1;
                        flm_HS_LS_On_Ch14_8.flm_ls_on_ch9.Data = 0x1;
                        break;
                    case 10:
                        flm_HS_LS_On_Ch14_8.flm_hs_on_ch10.Data = 0x1;
                        flm_HS_LS_On_Ch14_8.flm_ls_on_ch10.Data = 0x1;
                        break;
                    case 11:
                        flm_HS_LS_On_Ch14_8.flm_hs_on_ch11.Data = 0x1;
                        flm_HS_LS_On_Ch14_8.flm_ls_on_ch11.Data = 0x1;
                        break;
                    case 12:
                        flm_HS_LS_On_Ch14_8.flm_hs_on_ch12.Data = 0x1;
                        flm_HS_LS_On_Ch14_8.flm_ls_on_ch12.Data = 0x1;
                        break;
                    case 13:
                        flm_HS_LS_On_Ch14_8.flm_hs_on_ch13.Data = 0x1;
                        flm_HS_LS_On_Ch14_8.flm_ls_on_ch13.Data = 0x1;
                        break;
                    case 14:
                        flm_HS_LS_On_Ch14_8.flm_hs_on_ch14.Data = 0x1;
                        flm_HS_LS_On_Ch14_8.flm_ls_on_ch14.Data = 0x1;
                        break;
                    case 15:
                        flm_HS_LS_On_Ch20_15.flm_hs_on_ch15.Data = 0x1;
                        flm_HS_LS_On_Ch20_15.flm_ls_on_ch15.Data = 0x1;
                        break;
                    case 16:
                        flm_HS_LS_On_Ch20_15.flm_hs_on_ch16.Data = 0x1;
                        flm_HS_LS_On_Ch20_15.flm_ls_on_ch16.Data = 0x1;
                        break;
                    case 17:
                        flm_HS_LS_On_Ch20_15.flm_hs_on_ch17.Data = 0x1;
                        flm_HS_LS_On_Ch20_15.flm_ls_on_ch17.Data = 0x1;
                        break;
                    case 18:
                        flm_HS_LS_On_Ch20_15.flm_hs_on_ch18.Data = 0x1;
                        flm_HS_LS_On_Ch20_15.flm_ls_on_ch18.Data = 0x1;
                        break;
                    case 19:
                        flm_HS_LS_On_Ch20_15.flm_hs_on_ch19.Data = 0x1;
                        flm_HS_LS_On_Ch20_15.flm_ls_on_ch19.Data = 0x1;
                        break;
                    case 20:
                        flm_HS_LS_On_Ch20_15.flm_hs_on_ch20.Data = 0x1;
                        flm_HS_LS_On_Ch20_15.flm_ls_on_ch20.Data = 0x1;
                        break;
                }
            }

            // Set unlock codes
            flm_HS_LS_On_Ch7_1.flm_code_ch7_1.Data = 0x01;
            flm_HS_LS_On_Ch14_8.flm_code_ch14_8.Data = 0x10;
            flm_HS_LS_On_Ch20_15.flm_code_ch20_15.Data = 0x11;

            // Create package to MCU
            packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 1;
            packageToSend.Cmd = MCUCommand.EXECUTE_WRITE_SEQUENCE;
            packageToSend.Deleg = FireSimultaneousDelegate_step2;
            packageToSend.PayloadType = typeof(AddressDataPayload);
            packageToSend.Payload.Address.Add(flm_HS_LS_On_Ch7_1.Address);
            packageToSend.Payload.Data.Add(flm_HS_LS_On_Ch7_1.Data);
            packageToSend.Payload.Address.Add(flm_HS_LS_On_Ch14_8.Address);
            packageToSend.Payload.Data.Add(flm_HS_LS_On_Ch14_8.Data);
            packageToSend.Payload.Address.Add(flm_HS_LS_On_Ch20_15.Address);
            packageToSend.Payload.Data.Add(flm_HS_LS_On_Ch20_15.Data);

            // Send command to MCU
            serialWrapper.SerialWrite(packageToSend);    

            // Wait asynchronously without blocking the main thread for completing step
            await _taskCompletionSource.Task;   

            // == Step 3 - cleaning & locking ==

            _taskCompletionSource = null;

            // Lock all channels
            reg_FLM_Unlock.Data = 0x0000;

            // Set alternative mode if enabled
            reg_FLM_Unlock.flm_fire_mode_sel.Data = Convert.ToUInt16(IsAlternativeFiringModeEn);

            // Set code unlock
            reg_FLM_Unlock.flm_code_unlock.Data = 0x00;

            // Fill registers for finishing firing - all channels disabled
            flm_HS_LS_On_Ch7_1.Data   = 0x0000;   
            flm_HS_LS_On_Ch14_8.Data  = 0x0000; 
            flm_HS_LS_On_Ch20_15.Data = 0x0000;

            // Set unlock codes
            flm_HS_LS_On_Ch7_1.flm_code_ch7_1.Data = 0x01;
            flm_HS_LS_On_Ch14_8.flm_code_ch14_8.Data = 0x10;
            flm_HS_LS_On_Ch20_15.flm_code_ch20_15.Data = 0x11;

            // Create package to MCU
            packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 1;
            packageToSend.Cmd = MCUCommand.EXECUTE_WRITE_SEQUENCE;
            packageToSend.Deleg = FireSimultaneousDelegate_step3;
            packageToSend.PayloadType = typeof(AddressDataPayload);
            packageToSend.Payload.Address.Add(flm_HS_LS_On_Ch7_1.Address);
            packageToSend.Payload.Data.Add(flm_HS_LS_On_Ch7_1.Data);
            packageToSend.Payload.Address.Add(flm_HS_LS_On_Ch14_8.Address);
            packageToSend.Payload.Data.Add(flm_HS_LS_On_Ch14_8.Data);
            packageToSend.Payload.Address.Add(flm_HS_LS_On_Ch20_15.Address);
            packageToSend.Payload.Data.Add(flm_HS_LS_On_Ch20_15.Data);
            packageToSend.Payload.Address.Add(reg_FLM_Unlock.Address);
            packageToSend.Payload.Data.Add(reg_FLM_Unlock.Data);

            // Send command to MCU
            serialWrapper.SerialWrite(packageToSend);   
        }

        /// <summary>
        /// Execute start/stop cyclic reading for FLM command
        /// </summary>
        private void StartStopCyclicReadingExecute(object obj)
        {
            // TODO: should use parameter containing on/off flag
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method that will initiate TestMode1 diagnostics
        /// </summary>
        private void ExecuteTestMode1Diagnostics()
        {
            // TODO: add implementation
            throw new NotImplementedException();

            // TODO: should include trigger for transiting to TestMode2
            asicWrapper.ASICs[0].ExecuteTestMode1Transition();
        }

        /// <summary>
        /// Method that will initiate TestMode2 diagnostics
        /// </summary>
        private void ExecuteTestMode2Diagnostics()
        {
            // TODO: add implementation
            throw new NotImplementedException();

            // TODO: should include trigger for transiting to Normal mode
            asicWrapper.ASICs[0].ExecuteTestMode2Transition();
        }

        #endregion // Commands

        #region Commands_delegates

        /// <summary>
        /// Delegate for Fire simultaneous command - unlock
        /// </summary>
        /// <param name="response">MCU response package</param>
        private void FireSimultaneousDelegate_step1(IReceiveCommunicationPackage response)
        {
            // Ensure there is a valid TaskCompletionSource to complete
            if (_taskCompletionSource != null && !_taskCompletionSource.Task.IsCompleted)
            {
                _taskCompletionSource.SetResult(true); // Completes the task - allow proceeding 
            }

            // Typecast response to actual type
            ReceiveCommunicationPackage<AddressDataPayload> mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>) response;

            // Change state if response received
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(FireSimultaneous));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
                return;
            }

            // Emulate delay TODO: find better approach
            Thread.Sleep(1);
        }

        /// <summary>
        /// Delegate for Fire simultaneous command - fire
        /// </summary>
        /// <param name="response">MCU response package</param>
        private void FireSimultaneousDelegate_step2(IReceiveCommunicationPackage response)
        {
            // Ensure there is a valid TaskCompletionSource to complete
            if (_taskCompletionSource != null && !_taskCompletionSource.Task.IsCompleted)
            {
                _taskCompletionSource.SetResult(true); // Completes the task - allow proceeding 
            }

            // Typecast response to actual type
            ReceiveCommunicationPackage<AddressDataPayload> mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>) response;

            // Change state if response received
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(FireSimultaneous));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
                return;
            }

            // Emulate delay TODO: find better approach
            Thread.Sleep(13);
        }

        /// <summary>
        /// Delegate for Fire simultaneous command - stop firing and lock
        /// </summary>
        /// <param name="response">MCU response package</param>
        private void FireSimultaneousDelegate_step3(IReceiveCommunicationPackage response)
        {
            // Response received - unlock command usage
            _fireSimultaneousCommand.InProgress = false;
            OnPropertyChanged(nameof(FireSimultaneousCommandEn));

            // Ensure there is a valid TaskCompletionSource to complete
            if (_taskCompletionSource != null && !_taskCompletionSource.Task.IsCompleted)
            {
                _taskCompletionSource.SetResult(true); // Completes the task - allow proceeding 
            }

            // Typecast response to actual type
            ReceiveCommunicationPackage<AddressDataPayload> mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>) response;

            // Change state if response received
            if (mcuResponse.Payload.Error is not null)
            {
                AddError(mcuResponse.Payload.Error, nameof(FireSimultaneous));
                logger.Error($"Error response received. Status: {mcuResponse.Status}");
                return;
            }
        }

        #endregion // Commands_delegates

        #region ASIC_events

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
            // Fill firing results table with data based on configuration table
            foreach (FiringChannelConfigurationRecord channelRecord in FiringConfigurationTable)
            {
                FiringResultTable.Add(new FiringResultRecord() { ASICID = channelRecord.ASICID, ChannelID = channelRecord.ChannelID, Identifier = channelRecord.Identifier });
            }

            // Unsubscribe from event - by design can be fired only once
            caller.ConfigurationLoaded -= ConfigurationLoadedHandler;
        }

        /// <summary>
        /// Event handler that will be called when configuration is locked (EOP)
        /// </summary>
        /// <param name="sender">object that called this event. Must be one of ASIC objects</param>
        /// <param name="e">unused</param>
        private void ConfigurationLockedHandler(object? sender, EventArgs e)
        {
            // Precondition
            if (sender == null)
            {
                throw new ArgumentNullException("Incorrect argument - can't be null!");
            }

            // Typecast sender to actual type
            IASIC caller = (IASIC) sender;

            // Response received - unlock command usage
            _transferToNormalModeCommand.InProgress = false;
            OnPropertyChanged(nameof(TransferToNormalModeCommandEn));

            // Unsubscribe from event - by design can be fired only once
            caller.ConfigurationLocked -= ConfigurationLockedHandler;
        }

        /// <summary>
        /// Event handler that will be called when ASIC enters Test mode 1
        /// </summary>
        /// <param name="sender">object that called this event. Must be one of ASIC objects</param>
        /// <param name="e">unused</param>
        private void TestMode1EnteredHandler(object? sender, EventArgs e)
        {
            // Precondition
            if (sender == null)
            {
                throw new ArgumentNullException("Incorrect argument - can't be null!");
            }

            // Typecast sender to actual type
            IASIC caller = (IASIC) sender;

            // TODO: add implementation
            ExecuteTestMode1Diagnostics();

            // Unsubscribe from event - by design can be fired only once
            caller.TestMode1Entered -= TestMode1EnteredHandler;
        }

        /// <summary>
        /// Event handler that will be called when ASIC enters Test mode 2
        /// </summary>
        /// <param name="sender">object that called this event. Must be one of ASIC objects</param>
        /// <param name="e">unused</param>
        private void TestMode2EnteredHandler(object? sender, EventArgs e)
        {
            // Precondition
            if (sender == null)
            {
                throw new ArgumentNullException("Incorrect argument - can't be null!");
            }

            // Typecast sender to actual type
            IASIC caller = (IASIC) sender;

            // TODO: add implementation
            ExecuteTestMode2Diagnostics();

            // Unsubscribe from event - by design can be fired only once
            caller.TestMode2Entered -= TestMode2EnteredHandler;
        }

        /// <summary>
        /// Event handler that will be called when ASIC enters Normal mode
        /// </summary>
        /// <param name="sender">object that called this event. Must be one of ASIC objects</param>
        /// <param name="e">unused</param>
        private void NormalModeEnteredHandler(object? sender, EventArgs e)
        {
            // Precondition
            if (sender == null)
            {
                throw new ArgumentNullException("Incorrect argument - can't be null!");
            }

            // Typecast sender to actual type
            IASIC caller = (IASIC) sender;

            // Fire corresponding state machine trigger
            _stateMachine.Fire(Triggers.EnteredNormalMode);

            // Unsubscribe from event - by design can be fired only once
            caller.NormalModeEntered -= NormalModeEnteredHandler;
        }

        #endregion // ASIC_events
    }
}