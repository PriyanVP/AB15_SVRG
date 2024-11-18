using System;
using System.Timers;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.Services.Interfaces;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models.Generated.Registers;
using System.Linq;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// ASIC top level control and status
    /// </summary>
    public class ASIC : IASIC, INotifyPropertyChanged
    {
        /// <summary>
        /// SerialWrapper reference to perform communication with MCU
        /// </summary>
        private readonly ISerialWrapper serialWrapper;

        /// <summary>
        /// Logger reference with custom configuration
        /// </summary>
        private readonly ILoggingService logger;

        /// <summary>
        /// Timer to perform periodic ASIC state readings
        /// </summary>
        private Timer? stateReadingTimer = null;

        /// <summary>
        /// Timer to perform periodic INIT mode timeout resets
        /// </summary>
        private Timer? initModeResetTimer = null;

        /// <summary>
        /// Number of packages with configuration data that will be written to ASIC
        /// Null corresponds to unset value
        /// </summary>
        private int? expectedNumberOfConfigPackages = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"><inheritdoc cref="logger" path='/summary'/></param>
        /// <param name="serialWrapper"><inheritdoc cref="serialWrapper" path='/summary'/></param>
        public ASIC(ILoggingService logger, ISerialWrapper serialWrapper)
        {
            this.serialWrapper = serialWrapper;
            this.logger = logger;
        }

        #region Properties

        /// <summary>
        /// <inheritdoc cref="ID" path='/summary'/>
        /// </summary>
        private int id = 0;

        /// <summary>
        /// ASIC ID relative to MCU
        /// 0 - not set, 1 - master, 2-4 - slaves
        /// </summary>
        public int ID
        {
            get => id;
            set
            {
                // Check if valid ASIC ID is received
                if ((value < 0) || (value > 4)) throw new ArgumentOutOfRangeException(nameof(ID), $"Unsupported option for ASIC ID: {value}");

                id = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="IsOnline" path='/summary'/>
        /// </summary>
        private bool isOnline = false;

        /// <summary>
        /// Flag indicating if communication to ASIC is operational
        /// </summary>
        public bool IsOnline
        {
            get => isOnline;
            private set
            {
                isOnline = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="EOP" path='/summary'/>
        /// </summary>
        private bool eop = false;

        /// <summary>
        /// ASIC EOP flag. If set most configuration is locked
        /// </summary>
        public bool EOP
        {
            get => eop;
            private set
            {
                eop = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="State" path='/summary'/>
        /// </summary>
        private ASICState state = 0;

        /// <summary>
        /// ASIC state
        /// </summary>
        public ASICState State
        {
            get => state;
            private set
            {
                // Check if valid ASIC state is received
                if (!Enum.IsDefined(typeof(ASICState), value)) throw new ArgumentOutOfRangeException(nameof(ID), $"Unsupported option for ASIC state: {value}");

                state = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// List that holds configuration data for ASIC 
        /// (data that is written in INIT mode and used for CRC Cyclic checker)
        /// </summary>
        public List<IRegister> ConfigData { get; private set; } = new List<IRegister>();

        #endregion // Properties

        #region Events

        /// <summary>
        /// Event for notification if ASIC has entered INIT mode
        /// </summary>
        public event EventHandler? InitModeEntered;

        /// <summary>
        /// Event for notification if configuration data was loaded and cyclic CRC applied
        /// </summary>
        public event EventHandler? ConfigurationLoaded;

        /// <summary>
        /// Event for notification if EOP flag is set
        /// </summary>
        public event EventHandler? ConfigurationLocked;

        /// <summary>
        /// Event for notification if Test mode 1 is entered
        /// </summary>
        public event EventHandler? TestMode1Entered;

        /// <summary>
        /// Event for notification if Test mode 2 is entered
        /// </summary>        
        public event EventHandler? TestMode2Entered;

        /// <summary>
        /// Event for notification if Normal mode is entered
        /// </summary>        
        public event EventHandler? NormalModeEntered;

        /// <summary>
        /// Event for requesting configuration data
        /// </summary>        
        public event EventHandler? RequestConfiguration;

        /// <summary>
        /// Event for notification if error in any of the callbacks is set
        /// </summary>
        public event EventHandler<CallbackErrorEventArgs>? ErrorCallbackReceived;


        /// <summary>
        /// Raise event to notify about ASIC entering Init mode
        /// </summary>
        private void OnInitModeEntered()
        {
            InitModeEntered?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Raise event to notify about that configuration was loaded and cyclic CRC was applied
        /// </summary>
        private void OnConfigurationLoaded()
        {
            ConfigurationLoaded?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Raise event to notify about EOP flag set
        /// </summary>
        private void OnConfigurationLocked()
        {
            ConfigurationLocked?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Raise event to notify about entering Test mode 1
        /// </summary>
        private void OnTestMode1Entered()
        {
            TestMode1Entered?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Raise event to notify about entering Test mode 2
        /// </summary>
        private void OnTestMode2Entered()
        {
            TestMode2Entered?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Raise event to notify about entering Normal mode
        /// </summary>
        private void OnNormalModeEntered()
        {
            NormalModeEntered?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Raise event to request configuration
        /// </summary>
        public void OnRequestConfiguration()
        {
            RequestConfiguration?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Raise event to notify about error in callback
        /// </summary>
        /// <param name="errorMsg">class containing error message received in callback handler</param>
        private void OnErrorCallbackReceived(CallbackErrorEventArgs? errorMsg)
        {
            ErrorCallbackReceived?.Invoke(this, errorMsg);
        }

        // TODO: request configuration

        #endregion // Events

        #region Methods

        /// <summary>
        /// Method to add list with configuration registers to global list
        /// Values are added as references (2-side modification possible)
        /// Sorting is performed afterwards for ease of analysis
        /// </summary>
        /// <param name="listToAppend">list that will be appended</param>
        public void AppendConfigRegisters(List<IRegister> listToAppend)
        {
            // Remove elements if already in list
            int tmpIdx;
            foreach (IRegister itm in listToAppend)
            {
                tmpIdx = ConfigData.FindIndex(x => x.Address == itm.Address);
                if (tmpIdx != -1)
                {
                    ConfigData.RemoveAt(tmpIdx);
                }
            }

            // Add new registers to list
            ConfigData.AddRange(listToAppend);

            // Sort list (for uniformity and manual analysis)
            ConfigData.Sort((x, y) => x.Address.CompareTo(y.Address));

            // Check if duplicates present TODO: is needed?
            if (ConfigData.DistinctBy(x => x.Address).Count() != ConfigData.Count)
            {
                string configDataSummary = "";
                ConfigData.ForEach(itm => configDataSummary = $"{configDataSummary}\n{itm.Name} 0x{itm.Address:X} 0x{itm.Data:X}");
                logger.Error($"Duplicates in ASIC{ID} configuration list present. List of registers:\n{configDataSummary}");
            }
        }

        /// <summary>
        /// Start timer to arm periodic ASIC state readings
        /// </summary>
        /// <param name="timeout">timer timeout in ms. Defaults to 0.5s</param>
        public void StartPeriodicStateReading(int timeout = 500)
        {
            logger.Debug($"Started periodic state reading (timer) on ASIC {ID}");

            GetASICState();

            // // Precondition check
            // if (stateReadingTimer != null)
            // {
            //     logger.Warn($"Tried to start timer on ASIC{ID} while it was already running");
            //     return;
            // }

            // // Arm timer
            // stateReadingTimer = new Timer();
            // stateReadingTimer.Elapsed += new ElapsedEventHandler(OnAsicStateReadingEvent);
            // stateReadingTimer.Interval = timeout;
            // stateReadingTimer.Enabled = true;
        }

        /// <summary>
        /// Stop and dispose timer. No update of ASIC state will be done automatically
        /// </summary>
        public void StopPeriodicStateReading()
        {
            logger.Debug($"Stopped periodic state reading (timer) on ASIC {ID}");

            // // Stop and dispose timer
            // stateReadingTimer.Enabled = false;
            // stateReadingTimer.Dispose();
            // stateReadingTimer = null;
        }

        /// <summary>
        /// Start timer to enlarge init mode period
        /// </summary>
        /// <param name="timeout">timer timeout in ms. Defaults to 2s</param>
        public void StartInitModeTimeoutResetting(int timeout = 2000)
        {
            logger.Debug($"Starting init mode timer reset on ASIC {ID}");

            // // Precondition check
            // if (initModeResetTimer != null)
            // {
            //     logger.Warn($"Tried to start init mode continuation timer on ASIC{ID} while it was already running");
            //     return;
            // }

            // // Arm timer
            // initModeResetTimer = new Timer();
            // initModeResetTimer.Elapsed += new ElapsedEventHandler(OnInitModeTimeoutResettingEvent);
            // initModeResetTimer.Interval = timeout;
            // initModeResetTimer.Enabled = true;
        }

        /// <summary>
        /// Stop and dispose timer for init mode enlargement
        /// </summary>
        public void StopInitModeTimeoutResetting()
        {
            logger.Debug($"Stopping init mode timer reset on ASIC {ID}");

            // Stop and dispose timer
            initModeResetTimer.Enabled = false;
            initModeResetTimer.Dispose();
            initModeResetTimer = null;
        }

        /// <summary>
        /// Write configuration data and apply CRC
        /// </summary>
        public void WriteConfigurationWithCRC()
        {
            logger.Debug($"Started execution of WriteConfiguration command on ASIC {ID}");

            TransmitCommunicationPackage<AddressDataPayload> packageToSend;

            // Send configuration as commands (may not fit in one command)
            int offset = 0;
            int itmIdx = 0;
            expectedNumberOfConfigPackages = 0;
            while (offset < ConfigData.Count)
            {
                // Create new package to write configuration
                packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
                packageToSend.ASICID = ID; // TODO: check for ASICs 2-4
                packageToSend.Cmd = MCUCommand.EXECUTE_WRITE_SEQUENCE;
                packageToSend.Deleg = WriteConfigurationDelegate;
                packageToSend.PayloadType = typeof(EmptyPayload);

                // Add up to max amount of registers to payload
                for (int i = 0; i < CommandSpecificConstants.writeSequenceMaxItems; i++)
                {
                    // Index of register to serialize
                    itmIdx = offset + i;

                    // Add address and data of register
                    packageToSend.Payload.Address.Add(ConfigData[itmIdx].Address);
                    packageToSend.Payload.Data.Add(ConfigData[itmIdx].Data);

                    // Reached end of sequence or end of loop
                    if ((itmIdx >= ConfigData.Count - 1) || (i == CommandSpecificConstants.writeSequenceMaxItems - 1))
                    {
                        offset = itmIdx + 1;
                        break;
                    }
                }

                // Increment number of packages
                expectedNumberOfConfigPackages++;

                // Send command to MCU
                serialWrapper.SerialWrite(packageToSend);
            }

            // Apply CRC for configuration
            logger.Debug($"Started execution of Cyclic CRC command on ASIC {ID}");

            // Get list of registers for CRC calculation
            List<ushort> dataForCRC = ConfigData.Select(itm => itm.Data).ToList();
            ushort cyclicCRC = dataForCRC.GetCRC16(0, dataForCRC.Count);

            // Create register content for Cyclic CRC check
            Reg_Cyclic_Checker_CRC_Config _Cyclic_Checker_CRC_Config = new Reg_Cyclic_Checker_CRC_Config();
            _Cyclic_Checker_CRC_Config.Data = 0x0;
            _Cyclic_Checker_CRC_Config.cyclic_checker_ref_crc.Data = cyclicCRC;

            // Construct command to MCU
            packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = ID; // TODO: check for ASICs 2-4
            packageToSend.Cmd = MCUCommand.WRITE_REG;
            packageToSend.Deleg = WriteConfigurationDelegate;
            packageToSend.PayloadType = typeof(EmptyPayload);
            packageToSend.Payload.Address.Add(_Cyclic_Checker_CRC_Config.Address);
            packageToSend.Payload.Data.Add(_Cyclic_Checker_CRC_Config.Data);

            // Increment number of packages
            expectedNumberOfConfigPackages++;

            // Send command to MCU
            serialWrapper.SerialWrite(packageToSend);
        }

        /// <summary>
        /// Lock configuration (EOP)
        /// </summary>
        public void LockConfiguration()
        {
            logger.Debug($"Started execution of EOP command on ASIC {ID}");

            // Create register content for EOP
            Reg_SysStates_Reset_Locked_Config _SysStates_Reset_Locked_Config = new Reg_SysStates_Reset_Locked_Config();
            _SysStates_Reset_Locked_Config.Data = 0x0;
            _SysStates_Reset_Locked_Config.end_of_programming.Data = 0x1;

            // Construct command to MCU
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = ID; // TODO: check for ASICs 2-4
            packageToSend.Cmd = MCUCommand.WRITE_REG;
            packageToSend.Deleg = LockConfigurationDelegate;
            packageToSend.PayloadType = typeof(EmptyPayload);
            packageToSend.Payload.Address.Add(_SysStates_Reset_Locked_Config.Address);
            packageToSend.Payload.Data.Add(_SysStates_Reset_Locked_Config.Data);

            // Send command to MCU
            serialWrapper.SerialWrite(packageToSend);
        }

        /// <summary>
        /// Execute SPI_COLDSTART1 command
        /// Will cause ASIC reset (unconditionally)
        /// </summary>
        public void ExecuteSPIColdstart1()
        {
            logger.Debug($"Started execution of SPIColdstart1 command on ASIC {ID}");

            // Create register content for executing SPI_COLDSTART1
            Reg_SysStates_Reset_Config _SysStates_Reset_Config = new Reg_SysStates_Reset_Config();
            _SysStates_Reset_Config.Data = 0x0;
            _SysStates_Reset_Config.spi_coldstart1.Data = 0x1;

            // Construct command to MCU
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = ID; // TODO: check for ASICs 2-4
            packageToSend.Cmd = MCUCommand.WRITE_REG;
            packageToSend.Deleg = SPIColdstart1Delegate;
            packageToSend.PayloadType = typeof(EmptyPayload);
            packageToSend.Payload.Address.Add(_SysStates_Reset_Config.Address);
            packageToSend.Payload.Data.Add(_SysStates_Reset_Config.Data);

            // Send command to MCU
            serialWrapper.SerialWrite(packageToSend);
        }

        /// <summary>
        /// Execute transition from test mode 1 command
        /// </summary>
        public void ExecuteTestMode1Transition()
        {
            logger.Debug($"Started execution of Test mode 1 transition command on ASIC {ID}");

            // Create register content for executing SPI_COLDSTART1
            Reg_SysStates_Reset_Config _SysStates_Reset_Config = new Reg_SysStates_Reset_Config();
            _SysStates_Reset_Config.Data = 0x0;
            _SysStates_Reset_Config.spi_exit_testmode1.Data = 0x1;

            // Construct command to MCU
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = ID; // TODO: check for ASICs 2-4
            packageToSend.Cmd = MCUCommand.WRITE_REG;
            packageToSend.Deleg = BasicWriteCommandDelegate;
            packageToSend.PayloadType = typeof(EmptyPayload);
            packageToSend.Payload.Address.Add(_SysStates_Reset_Config.Address);
            packageToSend.Payload.Data.Add(_SysStates_Reset_Config.Data);

            // Send command to MCU
            serialWrapper.SerialWrite(packageToSend);
        }

        /// <summary>
        /// Execute transition from test mode 2 command
        /// </summary>
        public void ExecuteTestMode2Transition()
        {
            logger.Debug($"Started execution of Test mode 1 transition command on ASIC {ID}");

            // Create register content for executing SPI_COLDSTART1
            Reg_SysStates_Reset_Config _SysStates_Reset_Config = new Reg_SysStates_Reset_Config();
            _SysStates_Reset_Config.Data = 0x0;
            _SysStates_Reset_Config.spi_exit_testmode2.Data = 0x1;

            // Construct command to MCU
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = ID; // TODO: check for ASICs 2-4
            packageToSend.Cmd = MCUCommand.WRITE_REG;
            packageToSend.Deleg = BasicWriteCommandDelegate;
            packageToSend.PayloadType = typeof(EmptyPayload);
            packageToSend.Payload.Address.Add(_SysStates_Reset_Config.Address);
            packageToSend.Payload.Data.Add(_SysStates_Reset_Config.Data);

            // Send command to MCU
            serialWrapper.SerialWrite(packageToSend);
        }

        /// <summary>
        /// Read current ASIC state
        /// </summary>
        private void GetASICState()
        {
            logger.Debug($"Started execution of GetASICState command on ASIC {ID}");

            // Create register content for executing SPI_COLDSTART1
            Reg_SYSTEM_STATE SYSTEM_STATE = new Reg_SYSTEM_STATE();
            SYSTEM_STATE.Data = 0x0;

            // Construct command to MCU
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = ID; // TODO: check for ASICs 2-4
            packageToSend.Cmd = MCUCommand.START_HACKED_TIMER;
            packageToSend.Deleg = ASICStateDelegate;
            packageToSend.IsContinuous = true;
            packageToSend.PayloadType = typeof(ReadRegisterPayload);
            packageToSend.Payload.Address.Add(SYSTEM_STATE.Address);

            // Send command to MCU
            serialWrapper.SerialWrite(packageToSend);
        }

        // /// <summary>
        // /// Reset INIT mode timeout
        // /// </summary>
        // private void ResetInitModeTimeout()
        // {
        //     logger.Debug($"Started execution of Reset ITM command on ASIC {ID}");

        //     // Create register content for executing SPI_COLDSTART1
        //     Reg_SysStates_Reset_Config _SysStates_Reset_Config = new Reg_SysStates_Reset_Config();
        //     _SysStates_Reset_Config.Data = 0x0;
        //     _SysStates_Reset_Config.spi_clear_imt.Data = 0x1;

        //     // Construct command to MCU
        //     TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
        //     packageToSend.ASICID = ID; // TODO: check for ASICs 2-4
        //     packageToSend.Cmd = MCUCommand.WRITE_REG;
        //     packageToSend.Deleg = BasicWriteCommandDelegate;
        //     packageToSend.PayloadType = typeof(EmptyPayload);
        //     packageToSend.Payload.Address.Add(_SysStates_Reset_Config.Address);
        //     packageToSend.Payload.Data.Add(_SysStates_Reset_Config.Data);

        //     // Send command to MCU
        //     serialWrapper.SerialWrite(packageToSend);
        // }

        #endregion // Methods

        #region CallbackHandlers

        // /// <summary>
        // /// Method that will be called periodically by timer to read ASIC state
        // /// </summary>
        // /// <param name="source">unused</param>
        // /// <param name="e">unused</param>
        // private void OnAsicStateReadingEvent(object source, ElapsedEventArgs e)
        // {
        //         // Execute ASIC state reading
        //         GetASICState();
        // }

        // /// <summary>
        // /// Method that will be called periodically by timer to reset INIT mode timeout
        // /// </summary>
        // /// <param name="source">unused</param>
        // /// <param name="e">unused</param>
        // private void OnInitModeTimeoutResettingEvent(object source, ElapsedEventArgs e)
        // {
        //         // Reset INIT mode timeout
        //         ResetInitModeTimeout();
        // }

        /// <summary>
        /// Method that will be called when response for SPIColdstart1 command is received
        /// </summary>
        /// <param name="response">MCU response package</param>
        private void SPIColdstart1Delegate(IReceiveCommunicationPackage response)
        {
            // Typecast response to actual type
            ReceiveCommunicationPackage<EmptyPayload> mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>)response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                string errorMsg = $"Error response received. Status: {mcuResponse.Status}. Message: {mcuResponse.Payload.Error}";
                OnErrorCallbackReceived(new CallbackErrorEventArgs() { Error = errorMsg });
                logger.Error(errorMsg);
                return;
            }

            // No error present - reset ASIC State property
            State = ASICState.npor_release;
        }

        /// <summary>
        /// Method that will be called when response for reading SYSTEM_STATE register command is received
        /// </summary>
        /// <param name="response">MCU response package</param>
        private void ASICStateDelegate(IReceiveCommunicationPackage response)
        {
            // Typecast response to actual type
            ReceiveCommunicationPackage<ReadRegisterPayload> mcuResponse = (ReceiveCommunicationPackage<ReadRegisterPayload>)response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                // Report missing communication
                if (mcuResponse.Status == MCUStatus.RESPONSE_ABSENT)
                {
                    IsOnline = false;
                }
                string errorMsg = $"Error response received. Status: {mcuResponse.Status}. Message: {mcuResponse.Payload.Error}";
                OnErrorCallbackReceived(new CallbackErrorEventArgs() { Error = errorMsg });
                logger.Error(errorMsg);
                return;
            }

            // No error present - update ASIC state
            ASICState prevState = State;
            State = (ASICState) mcuResponse.Payload.RegisterData;
            IsOnline = true;

            // Raise event if entered new mode
            if (prevState != State)
            {
                switch (State)
                {
                    case ASICState.init_mode:
                        OnInitModeEntered();
                        break;
                    case ASICState.test_mode1:
                        OnTestMode1Entered();
                        break;
                    case ASICState.test_mode2:
                        OnTestMode2Entered();
                        break;
                    case ASICState.normal_mode:
                        OnNormalModeEntered();
                        break;
                    default:
                        OnErrorCallbackReceived(new CallbackErrorEventArgs() { Error = $"Unexpected ASIC{ID} mode received {State}" });
                        break;
                }
            }
        }

        /// <summary>
        /// Method that will be called when response for writing configuration received
        /// </summary>
        /// <param name="response">MCU response package</param>
        private void WriteConfigurationDelegate(IReceiveCommunicationPackage response)
        {
            // Typecast response to actual type
            ReceiveCommunicationPackage<EmptyPayload> mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>)response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                string errorMsg = $"Error response received. Status: {mcuResponse.Status}. Message: {mcuResponse.Payload.Error}";
                OnErrorCallbackReceived(new CallbackErrorEventArgs() { Error = errorMsg });
                logger.Error(errorMsg);
                return;
            }

            // Check if this is the last expected response
            expectedNumberOfConfigPackages--;
            if (expectedNumberOfConfigPackages <= 0)
            {
                OnConfigurationLoaded();
                expectedNumberOfConfigPackages = null;
            }
        }

        /// <summary>
        /// Method that will be called when response for locking configuration (EOP) received
        /// </summary>
        /// <param name="response">MCU response package</param>
        private void LockConfigurationDelegate(IReceiveCommunicationPackage response)
        {
            // Typecast response to actual type
            ReceiveCommunicationPackage<EmptyPayload> mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>)response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                string errorMsg = $"Error response received. Status: {mcuResponse.Status}. Message: {mcuResponse.Payload.Error}";
                OnErrorCallbackReceived(new CallbackErrorEventArgs() { Error = errorMsg });
                logger.Error(errorMsg);
                return;
            }

            // Raise configuration locked event
            OnConfigurationLocked();
        }

        /// <summary>
        /// Method that will be called when response for write register command received
        /// </summary>
        /// <param name="response">MCU response package</param>
        private void BasicWriteCommandDelegate(IReceiveCommunicationPackage response)
        {
            // Typecast response to actual type
            ReceiveCommunicationPackage<EmptyPayload> mcuResponse = (ReceiveCommunicationPackage<EmptyPayload>)response;

            // If error received - pass it to error provider
            if (mcuResponse.Payload.Error is not null)
            {
                string errorMsg = $"Error response received. Status: {mcuResponse.Status}. Message: {mcuResponse.Payload.Error}";
                OnErrorCallbackReceived(new CallbackErrorEventArgs() { Error = errorMsg });
                logger.Error(errorMsg);
                return;
            }
        }

        #endregion // CallbackHandlers

        #region Services

        /// <summary>
        /// Event for notification if property has changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise event to notify about property change
        /// </summary>
        /// <param name="propertyName">name of property</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Sanity check
            if (propertyName == null) throw new ArgumentException("Property name can't be null!");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // Services
    }
}