using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stateless;
using Stateless.Graph;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.ViewModels.Commands;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Generated.Registers;
using AB15_GUI.WPF.Services.Interfaces;

namespace AB15_GUI.WPF.ViewModels
{
    /// <summary>
    /// Faults status value definition
    /// </summary>

    public class ConfigurationViewModel : ViewModelBase
    {

        /// <summary>
        /// Local logger instance
        /// </summary>
        private readonly ILoggingService logger;

        /// <summary>
        /// Lock object for thread synchronization
        /// </summary>
        private readonly object _lock = new object();

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
        public ConfigurationViewModel(ILoggingService logger, ISerialWrapper serialWrapper, IASICWrapper asicWrapper) :
                base(logger)
        {
            // Assign references to objects to local variables
            this.logger = logger;
            this.serialWrapper = serialWrapper;
            this.asicWrapper = asicWrapper;
            logger.Trace("In ConfigurationViewModel");

            // Init observable items
            PsiSensorData = new PsiData();
            PsiStatusList = new ObservableCollection<ObservableRegister>()
                {
                    new ObservableRegister(new Reg_PSI_Status_Ch1()),
                    new ObservableRegister(new Reg_PSI_Status_Ch2()),
                    new ObservableRegister(new Reg_PSI_Status_Ch3()),
                    new ObservableRegister(new Reg_PSI_Status_Ch4()),
                    new ObservableRegister(new Reg_PSI_Status_Ch5()),
                    new ObservableRegister(new Reg_PSI_Status_Ch6()),
                    new ObservableRegister(new Reg_PSI_Status_Ch7()),
                    new ObservableRegister(new Reg_PSI_Status_Ch8())
                };

            // Init commands
            ReadPsiSensorData = new RelayCommand(ReadPsiSensorDataExecuteAsync);
            ReadPsiStatus = new RelayCommand(ReadPsiStatusExecuteAsync);
            ReadMonitorSpiStatus = new RelayCommand(ReadMonitorSpiStatusExecuteAsync);
            ReadPsiConfiguration = new RelayCommand(ReadPsiConfigurationExecuteAsync);
            WritePsiConfiguration = new RelayCommand(WritePsiConfigurationExecuteAsync);
            ResetPsiConfiguration = new RelayCommand(ResetPsiConfigurationExecuteAsync);

            ReadUartConfiguration = new RelayCommand(ReadUartConfigurationExecuteAsync);
            ReadUartStatus = new RelayCommand(ReadUartStatusExecuteAsync);
            // WriteUartStatus = new RelayCommand(WriteUartStatusExecuteAsync);
            // ResetUartStatus = new RelayCommand(ResetUartStatusExecuteAsync);
            SetResetUartFrames = new RelayCommand(SetResetUartFramesExecuteAsync);

            // General page enables
            IsPsiPageEnabled = true;
            IsUartPageEnabled = true;

            // Init monitor SPI with default values
            MonSpi1Data = "0x0000";
            MonSpi1Sid = "0x0000";
            MonSpi2Data = "0x0000";
            MonSpi2Sid = "0x0000";

            // Default values
            PsiSupply = 0xFF;
            PsiGenMaskSync = 0x01;

            // Statuses and indicators init
            SyncPulseGeneretingStatus = FaultStatus.Fault;
            PSIDataReceivedStatus = FaultStatus.Fault;
            PSITopStatus = FaultStatus.Fault;
            UARTTopStatus = FaultStatus.Fault;

            PSITopStatusText = "Deactivated";
            UARTTopStatusText = "Deactivated";

            // Events from ASIC // TODO: some of these handlers should be moved out of Firing VM
            this.asicWrapper.ASICs[0].InitModeEntered += InitModeEnteredHandler;
            this.asicWrapper.ASICs[0].NormalModeEntered += NormalModeEnteredHandler;
        }

        #region State_Machine

        #endregion //State_Machine

        #region Bindable_Properties

        /// <summary>
        /// <inheritdoc cref="IsPsiPageEnabled" path='/summary'/>
        /// </summary>
        private bool isPsiPageEnabled;
        
        /// <summary>
        /// Top level enable control for PSI page
        /// </summary>
        public bool IsPsiPageEnabled
        {
            get => isPsiPageEnabled;
            set 
            {
                isPsiPageEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="IsUartPageEnabled" path='/summary'/>
        /// </summary>
        private bool isUartPageEnabled;
        
        /// <summary>
        /// Top level enable control for UART page
        /// </summary>
        public bool IsUartPageEnabled
        {
            get => isUartPageEnabled;
            set 
            {
                isUartPageEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="UARTTopStatusText" path='/summary'/>
        /// </summary>
        private string uartTopStatusText;
        
        /// <summary>
        /// Text to be displayed in indicator on page. Represents UART communication status
        /// </summary>
        public string UARTTopStatusText
        {
            get => uartTopStatusText;
            set 
            {
                uartTopStatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="UARTStatusText" path='/summary'/>
        /// </summary>
        private string uartStatusText;
        
        /// <summary>
        /// Hex content of UART status register
        /// </summary>
        public string UARTStatusText
        {
            get => uartStatusText;
            set 
            {
                uartStatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="PSITopStatusText" path='/summary'/>
        /// </summary>
        private string psiTopStatusText;
        
        /// <summary>
        /// Text to be displayed in indicator on page. Represents PSI communication status
        /// </summary>
        public string PSITopStatusText
        {
            get => psiTopStatusText;
            set 
            {
                psiTopStatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="PsiSensorData" path='/summary'/>
        /// </summary>
        private PsiData psiSensorData;
        
        /// <summary>
        /// PSI sensor data for all channels and slots
        /// </summary>
        public PsiData PsiSensorData
        {
            get => psiSensorData;
            set 
            {
                psiSensorData = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// PSI status list with observability on each element
        /// </summary>
        public ObservableCollection<ObservableRegister> PsiStatusList { get; set; }

        /// <summary>
        /// <inheritdoc cref="UARTTopStatus" path='/summary'/>
        /// </summary>
        private FaultStatus uartTopStatus;
        
        /// <summary>
        /// Color flag indicating status of UART
        /// </summary>
        public FaultStatus UARTTopStatus
        {
            get => uartTopStatus;
            set
            {
                uartTopStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="MonSpi1Data" path='/summary'/>
        /// </summary>
        private string monSpi1Data;
        
        /// <summary>
        /// MON SPI1 data
        /// </summary>
        public string MonSpi1Data
        {
            get => monSpi1Data;
            set
            {
                monSpi1Data = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="MonSpi1Sid" path='/summary'/>
        /// </summary>
        private string monSpi1Sid;
        
        /// <summary>
        /// MON SPI1 SID
        /// </summary>
        public string MonSpi1Sid
        {
            get => monSpi1Sid;
            set
            {
                monSpi1Sid = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="MonSpi2Data" path='/summary'/>
        /// </summary>
        private string monSpi2Data;
        
        /// <summary>
        /// MON SPI2 data
        /// </summary>
        public string MonSpi2Data
        {
            get => monSpi2Data;
            set
            {
                monSpi2Data = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="MonSpi2Sid" path='/summary'/>
        /// </summary>
        private string monSpi2Sid;
        
        /// <summary>
        /// MON SPI2 SID
        /// </summary>
        public string MonSpi2Sid
        {
            get => monSpi2Sid;
            set
            {
                monSpi2Sid = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="PsiSupply" path='/summary'/>
        /// </summary>
        private uint psiSupply;
        
        /// <summary>
        /// PSI SUPPLY register value
        /// </summary>
        public uint PsiSupply
        {
            get => psiSupply;
            set
            {
                psiSupply = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="PsiGenMaskSync" path='/summary'/>
        /// </summary>
        private uint psiGenMaskSync;
        
        /// <summary>
        /// PSI GEN MASK SYNC register value
        /// </summary>
        public uint PsiGenMaskSync
        {
            get => psiGenMaskSync;
            set
            {
                psiGenMaskSync = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="SyncPulseGeneretingStatus" path='/summary'/>
        /// </summary>
        private FaultStatus syncPulseGeneretingStatus;
        
        /// <summary>
        /// Sync pulse (for PSI) generation status
        /// </summary>
        public FaultStatus SyncPulseGeneretingStatus
        {
            get => syncPulseGeneretingStatus;
            set
            {
                syncPulseGeneretingStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private FaultStatus flmFiringWithPSIStatus;
        public FaultStatus FLMFiringWithPSIStatus
        {
            get => flmFiringWithPSIStatus;
            set
            {
                flmFiringWithPSIStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="PSITopStatus" path='/summary'/>
        /// </summary>
        private FaultStatus psiTopStatus;

        /// <summary>
        /// Color flag of PSI status indicator
        /// </summary>
        public FaultStatus PSITopStatus
        {
            get => psiTopStatus;
            set
            {
                psiTopStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="PSIDataReceivedStatus" path='/summary'/>
        /// </summary>
        private FaultStatus psiDataReceivedStatus;
        
        /// <summary>
        /// PSI data was received at least once
        /// </summary>
        public FaultStatus PSIDataReceivedStatus
        {
            get => psiDataReceivedStatus;
            set
            {
                psiDataReceivedStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="PSIDataReceivedStatus" path='/summary'/>
        /// </summary>
        private FaultStatus ch1Status;

        /// <summary>
        /// 
        /// </summary>
        public FaultStatus Ch1Status
        {
            get => ch1Status;
            set
            {
                ch1Status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private FaultStatus ch2Status;
        public FaultStatus Ch2Status
        {
            get => ch2Status;
            set
            {
                ch2Status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private FaultStatus ch3Status;
        public FaultStatus Ch3Status
        {
            get => ch3Status;
            set
            {
                ch3Status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///  
        /// </summary>
        private FaultStatus ch4Status;
        public FaultStatus Ch4Status
        {
            get => ch4Status;
            set
            {
                ch4Status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private FaultStatus ch5Status;
        public FaultStatus Ch5Status
        {
            get => ch5Status;
            set
            {
                ch5Status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private FaultStatus ch6Status;
        public FaultStatus Ch6Status
        {
            get => ch6Status;
            set
            {
                ch6Status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private FaultStatus ch7Status;
        public FaultStatus Ch7Status
        {
            get => ch7Status;
            set
            {
                ch7Status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private FaultStatus ch8Status;
        public FaultStatus Ch8Status
        {
            get => ch8Status;
            set
            {
                ch8Status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="UartFrame1Text" path='/summary'/>
        /// </summary>
        private string uartFrame1Text;
        
        /// <summary>
        /// Text in UART frame 1
        /// </summary>
        public string UartFrame1Text
        {
            get => uartFrame1Text;
            set 
            {
                uartFrame1Text = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="UartFrame2Text" path='/summary'/>
        /// </summary>
        private string uartFrame2Text;
        
        /// <summary>
        /// Text in UART frame 2
        /// </summary>
        public string UartFrame2Text
        {
            get => uartFrame2Text;
            set 
            {
                uartFrame2Text = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// <inheritdoc cref="UartFrame3Text" path='/summary'/>
        /// </summary>
        private string uartFrame3Text;
        
        /// <summary>
        /// Text in UART frame 1
        /// </summary>
        public string UartFrame3Text
        {
            get => uartFrame3Text;
            set 
            {
                uartFrame3Text = value;
                OnPropertyChanged();
            }
        }
        
        #endregion // Bindable_Properties

        #region Internal_configuration

        #endregion // Internal_configuration

        #region Commands

        /// <summary>
        /// Read PSI sensor data command
        /// </summary>
        ICommand ReadPsiSensorData { get; }

        /// <summary>
        /// Read PSI sensor data button/command enable state
        /// </summary>
        private CommandState _readPsiSensorDataCommand        = new CommandState();
        
        /// <summary>
        /// Bindable Read PSI sensor data button/command enable state
        /// </summary>
        public bool ReadPsiSensorDataCommandEn => _readPsiSensorDataCommand.IsEnabled;

        /// <summary>
        /// Read PSI status command
        /// </summary>
        ICommand ReadPsiStatus { get; }

        /// <summary>
        /// Read PSI status button/command enable state
        /// </summary>
        private CommandState _readPsiStatusCommand = new CommandState();

        /// <summary>
        /// Bindable Read PSI status button/command enable state
        /// </summary>
        public bool ReadPsiStatusCommandEn => _readPsiStatusCommand.IsEnabled;

        /// <summary>
        /// Read Monitor SPI status command
        /// </summary>
        ICommand ReadMonitorSpiStatus { get; }

        /// <summary>
        /// Read Monitor SPI status button/command enable state
        /// </summary>
        private CommandState _readMonitorSpiStatusCommand = new CommandState();

        /// <summary>
        /// Bindable Read Monitor SPI status button/command enable state
        /// </summary>
        public bool ReadMonitorSpiStatusCommandEn => _readMonitorSpiStatusCommand.IsEnabled;

        /// <summary>
        /// Read PSI configuration command
        /// </summary>
        ICommand ReadPsiConfiguration { get; }

        /// <summary>
        /// Read PSI configuration button/command enable state
        /// </summary>
        private CommandState _readPsiConfigurationCommand = new CommandState();

        /// <summary>
        /// Bindable Read PSI configuration button/command enable state
        /// </summary>
        public bool ReadPsiConfigurationCommandEn => _readPsiConfigurationCommand.IsEnabled;

        /// <summary>
        /// Write PSI configuration command
        /// </summary>
        ICommand WritePsiConfiguration { get; }

        /// <summary>
        /// Write PSI configuration button/command enable state
        /// </summary>
        private CommandState _writePsiConfigurationCommand = new CommandState();

        /// <summary>
        /// Bindable Write PSI configuration button/command enable state
        /// </summary>
        public bool WritePsiConfigurationCommandEn => _writePsiConfigurationCommand.IsEnabled;

        /// <summary>
        /// Reset PSI configuration command
        /// </summary>
        ICommand ResetPsiConfiguration { get; }

        /// <summary>
        /// Reset PSI configuration button/command enable state
        /// </summary>
        private CommandState _resetPsiConfigurationCommand = new CommandState();

        /// <summary>
        /// Bindable Reset PSI configuration button/command enable state
        /// </summary>
        public bool ResetPsiConfigurationCommandEn => _resetPsiConfigurationCommand.IsEnabled;

        /// <summary>
        /// Read UART configuration command
        /// </summary>
        ICommand ReadUartConfiguration { get; }

        /// <summary>
        /// Read UART configuration button/command enable state
        /// </summary>
        private CommandState _readUartConfigurationCommand = new CommandState();

        /// <summary>
        /// Bindable Read UART configuration button/command enable state
        /// </summary>
        public bool ReadUartConfigurationCommandEn => _readUartConfigurationCommand.IsEnabled;

        /// <summary>
        /// Read UART status command
        /// </summary>
        ICommand ReadUartStatus { get; }

        /// <summary>
        /// Read UART status button/command enable state
        /// </summary>
        private CommandState _readUartStatusCommand = new CommandState();

        /// <summary>
        /// Bindable Read UART status button/command enable state
        /// </summary>
        public bool ReadUartStatusCommandEn => _readUartStatusCommand.IsEnabled;

        /// <summary>
        /// Set/Reset UART frames command
        /// </summary>
        ICommand SetResetUartFrames { get; }

        /// <summary>
        /// Set/Reset UART frames button/command enable state
        /// </summary>
        private CommandState _setResetUartFramesCommand = new CommandState();

        /// <summary>
        /// Bindable Set/Reset UART frames button/command enable state
        /// </summary>
        public bool SetResetUartFramesCommandEn => _setResetUartFramesCommand.IsEnabled;

        /// <summary>
        /// Execute Read PSI sensor data command
        /// </summary>
        private async void ReadPsiSensorDataExecuteAsync(object obj)
        {
            // Handle that command execution can only be done once in a row
            if (_readPsiSensorDataCommand.IsEnabled == false) return;
            _readPsiSensorDataCommand.InProgress = true;
            OnPropertyChanged(nameof(ReadPsiSensorDataCommandEn));

            // Read sensor data
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 2; // using CS_MON1
            packageToSend.Cmd = MCUCommand.EXECUTE_READ_SEQUENCE;
            packageToSend.PayloadType = typeof(AddressDataPayload);
            packageToSend.Payload.Address.AddRange(PsiSensorData.Addresses);

            // Send command to MCU and wait for response
            ReceiveCommunicationPackage<AddressDataPayload>? mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>?) await serialWrapper.SerialWriteAsync(packageToSend);

            _readPsiSensorDataCommand.InProgress = false;
            OnPropertyChanged(nameof(ReadPsiSensorDataCommandEn));

            // Validate response
            if (IsResponseValid(mcuResponse, nameof(ReadPsiSensorData)) == false) return;

            // Update sensor data
            PsiSensorData.UpdateData(mcuResponse.Payload.Data);
        }

        /// <summary>
        /// Execute Read PSI status command
        /// </summary>
        private async void ReadPsiStatusExecuteAsync(object obj)
        {
            if (_readPsiStatusCommand.IsEnabled == false) return;
            _readPsiStatusCommand.InProgress = true;
            OnPropertyChanged(nameof(ReadPsiStatusCommandEn));

            // Read sensor data
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 2; // using CS_MON1
            packageToSend.Cmd = MCUCommand.EXECUTE_READ_SEQUENCE;
            packageToSend.PayloadType = typeof(AddressDataPayload);
            for (int i = 0; i < PsiStatusList.Count; i++)
            {
                packageToSend.Payload.Address.Add(PsiStatusList[i].Register.Address);
            }

            // Send command to MCU and wait for response
            ReceiveCommunicationPackage<AddressDataPayload>? mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>?) await serialWrapper.SerialWriteAsync(packageToSend);

            _readPsiStatusCommand.InProgress = false;
            OnPropertyChanged(nameof(ReadPsiStatusCommandEn));

            // Validate response
            if (IsResponseValid(mcuResponse, nameof(ReadMonitorSpiStatus)) == false) return;

            // Update sensor data
            for (int i = 0; i < PsiStatusList.Count; i++)
            {
                PsiStatusList[i].Data = mcuResponse.Payload.Data[i];
            }
        }

        /// <summary>
        /// Execute Read Monitor SPI status command
        /// </summary>
        private async void ReadMonitorSpiStatusExecuteAsync(object obj)
        {
            if (_readMonitorSpiStatusCommand.IsEnabled == false) return;
            _readMonitorSpiStatusCommand.InProgress = true;
            OnPropertyChanged(nameof(ReadMonitorSpiStatusCommandEn));

            // Register models
            Reg_MON_SPI1_read_mon_data reg_MON_SPI1_Read_Mon_Data = new Reg_MON_SPI1_read_mon_data();
            Reg_MON_SPI1_read_mon_sid reg_MON_SPI1_Read_Mon_Sid = new Reg_MON_SPI1_read_mon_sid();
            Reg_MON_SPI2_read_mon_data reg_MON_SPI2_Read_Mon_Data = new Reg_MON_SPI2_read_mon_data();
            Reg_MON_SPI2_read_mon_sid reg_MON_SPI2_Read_Mon_Sid = new Reg_MON_SPI2_read_mon_sid();

            // Read sensor data
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 2; // using CS_MON1
            packageToSend.Cmd = MCUCommand.EXECUTE_READ_SEQUENCE;
            packageToSend.PayloadType = typeof(AddressDataPayload);
            packageToSend.Payload.Address.Add(reg_MON_SPI1_Read_Mon_Data.Address);
            packageToSend.Payload.Address.Add(reg_MON_SPI1_Read_Mon_Sid.Address);
            packageToSend.Payload.Address.Add(reg_MON_SPI2_Read_Mon_Data.Address);
            packageToSend.Payload.Address.Add(reg_MON_SPI2_Read_Mon_Sid.Address);

            // Send command to MCU and wait for response
            ReceiveCommunicationPackage<AddressDataPayload>? mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>?) await serialWrapper.SerialWriteAsync(packageToSend);

            _readMonitorSpiStatusCommand.InProgress = false;
            OnPropertyChanged(nameof(ReadMonitorSpiStatusCommandEn));

            // Validate response
            if (IsResponseValid(mcuResponse, nameof(ReadMonitorSpiStatus)) == false) return;

            // Update observable properties
            MonSpi1Data = $"0x{mcuResponse.Payload.Data[0]:X4}";
            MonSpi1Sid = $"0x{mcuResponse.Payload.Data[1]:X4}";
            MonSpi2Data = $"0x{mcuResponse.Payload.Data[2]:X4}";
            MonSpi2Sid = $"0x{mcuResponse.Payload.Data[3]:X4}";
        }

        /// <summary>
        /// Execute Read PSI configuration command
        /// </summary>
        private async void ReadPsiConfigurationExecuteAsync(object obj)
        {
            if (_readPsiConfigurationCommand.IsEnabled == false) return;
            _readPsiConfigurationCommand.InProgress = true;
            OnPropertyChanged(nameof(ReadPsiConfigurationCommandEn));

            // Register models
            Reg_PSI_Supply reg_PSI_Supply = new Reg_PSI_Supply();
            Reg_PSI_Gen_Mask_Sync reg_PSI_Gen_Mask_Sync = new Reg_PSI_Gen_Mask_Sync();

            // Read sensor data
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 2; // using CS_MON1
            packageToSend.Cmd = MCUCommand.EXECUTE_READ_SEQUENCE;
            packageToSend.PayloadType = typeof(AddressDataPayload);
            packageToSend.Payload.Address.Add(reg_PSI_Supply.Address);
            packageToSend.Payload.Address.Add(reg_PSI_Gen_Mask_Sync.Address);

            // Send command to MCU and wait for response
            ReceiveCommunicationPackage<AddressDataPayload>? mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>?) await serialWrapper.SerialWriteAsync(packageToSend);

            _readPsiConfigurationCommand.InProgress = false;
            OnPropertyChanged(nameof(ReadPsiConfigurationCommandEn));

            // Validate response
            if (IsResponseValid(mcuResponse, nameof(ReadPsiConfiguration)) == false) return;

            // Update observable properties
            PsiSupply = mcuResponse.Payload.Data[0];
            PsiGenMaskSync = mcuResponse.Payload.Data[1];
        }

        /// <summary>
        /// Execute Write PSI configuration command
        /// </summary>
        private async void WritePsiConfigurationExecuteAsync(object obj)
        {
            if (_writePsiConfigurationCommand.IsEnabled == false) return;
            _writePsiConfigurationCommand.InProgress = true;
            OnPropertyChanged(nameof(WritePsiConfigurationCommandEn));

            // Register models
            Reg_PSI_Supply reg_PSI_Supply = new Reg_PSI_Supply();
            Reg_PSI_Gen_Mask_Sync reg_PSI_Gen_Mask_Sync = new Reg_PSI_Gen_Mask_Sync();

            // Read sensor data
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 2; // using CS_MON1
            packageToSend.Cmd = MCUCommand.EXECUTE_WRITE_SEQUENCE;
            packageToSend.PayloadType = typeof(AddressDataPayload);
            packageToSend.Payload.Address.Add(reg_PSI_Supply.Address);
            packageToSend.Payload.Data.Add((ushort)PsiSupply);
            packageToSend.Payload.Address.Add(reg_PSI_Gen_Mask_Sync.Address);
            packageToSend.Payload.Data.Add((ushort)PsiGenMaskSync);

            // Send command to MCU and wait for response
            ReceiveCommunicationPackage<AddressDataPayload>? mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>?) await serialWrapper.SerialWriteAsync(packageToSend);

            _writePsiConfigurationCommand.InProgress = false;
            OnPropertyChanged(nameof(WritePsiConfigurationCommandEn));

            // Validate response
            if (IsResponseValid(mcuResponse, nameof(WritePsiConfiguration)) == false) return;
        }

        /// <summary>
        /// Execute Reset PSI configuration command
        /// </summary>
        private async void ResetPsiConfigurationExecuteAsync(object obj)
        {
            if (_resetPsiConfigurationCommand.IsEnabled == false) return;
            _resetPsiConfigurationCommand.InProgress = true;
            OnPropertyChanged(nameof(ResetPsiConfigurationCommandEn));

            // Reset values to default
            PsiSupply = 0xFF;
            PsiGenMaskSync = 0x01;

            _resetPsiConfigurationCommand.InProgress = false;
            OnPropertyChanged(nameof(ResetPsiConfigurationCommandEn));
        }

        /// <summary>
        /// Execute Read UART configuration command
        /// </summary>
        private async void ReadUartConfigurationExecuteAsync(object obj)
        {
            if (_readUartConfigurationCommand.IsEnabled == false) return;
            _readUartConfigurationCommand.InProgress = true;
            OnPropertyChanged(nameof(ReadUartConfigurationCommandEn));

            // ...command execution logic...

            _readUartConfigurationCommand.InProgress = false;
            OnPropertyChanged(nameof(ReadUartConfigurationCommandEn));
        }

        /// <summary>
        /// Execute Read UART status command
        /// </summary>
        private async void ReadUartStatusExecuteAsync(object obj)
        {
            if (_readUartStatusCommand.IsEnabled == false) return;
            _readUartStatusCommand.InProgress = true;
            OnPropertyChanged(nameof(ReadUartStatusCommandEn));

            // ...command execution logic...

            _readUartStatusCommand.InProgress = false;
            OnPropertyChanged(nameof(ReadUartStatusCommandEn));
        }

        /// <summary>
        /// Execute Set/Reset UART frames command
        /// </summary>
        private async void SetResetUartFramesExecuteAsync(object parameter)
        {
            if (_setResetUartFramesCommand.IsEnabled == false) return;
            _setResetUartFramesCommand.InProgress = true;
            OnPropertyChanged(nameof(SetResetUartFramesCommandEn));

            // Define which parameter was used
            string commandOption = (parameter as string) ?? "Reset"; // Defaults to reset

            if (commandOption == "Set")
            {
                // Transfer data to FiringVM
                // TODO: implement
            }
            else
            {
                // Reset to default
                UartFrame1Text = "0x55";
                UartFrame2Text = "0x01";
                UartFrame3Text = "0xFE";
            }

            _setResetUartFramesCommand.InProgress = false;
            OnPropertyChanged(nameof(SetResetUartFramesCommandEn));
        }

        #endregion // Commands

        #region ASIC_events

        /// <summary>
        /// Event handler that will be called when ASIC enters init mode
        /// </summary>
        /// <param name="sender">object that called this event. Must be one of ASIC objects</param>
        /// <param name="e">unused</param>
        private async void InitModeEnteredHandler(object? sender, EventArgs e)
        {
            // Precondition
            if (sender == null)
            {
                throw new ArgumentNullException("Incorrect argument - can't be null!");
            }

            // Typecast sender to actual type
            IASIC caller = (IASIC) sender;

            // TODO: think of way to remove duplication - same operations is already done in Firing VM
            // Check if UART is enabled
            Reg_SAFE_SETTINGS reg_SAFE_SETTINGS = new Reg_SAFE_SETTINGS();
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = 1;
            packageToSend.Cmd = MCUCommand.READ_REG;
            packageToSend.PayloadType = typeof(AddressDataPayload);
            packageToSend.Payload.Address.Add(reg_SAFE_SETTINGS.Address);

            // Send command to MCU and wait for response
            ReceiveCommunicationPackage<AddressDataPayload>? mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>?) await serialWrapper.SerialWriteAsync(packageToSend);

            // Validate response
            if (IsResponseValid(mcuResponse, null) == false) return;

            // Report UART/PSI enable status
            reg_SAFE_SETTINGS.Data = mcuResponse.Payload.Data[0];
            UARTTopStatus = (reg_SAFE_SETTINGS.disable_master_mode.Data > 1) ? FaultStatus.Good : FaultStatus.Fault;
            UARTTopStatusText = (UARTTopStatus == FaultStatus.Good) ? "Activated" : "Deactivated";

            PSITopStatus = (reg_SAFE_SETTINGS.disable_master_mode.Data < 3) ? FaultStatus.Good : FaultStatus.Fault;
            PSITopStatusText = (PSITopStatus == FaultStatus.Good) ? "Activated" : "Deactivated";

            // Unsubscribe from event - by design can be fired only once
            caller.InitModeEntered -= InitModeEnteredHandler;
        }

        /// <summary>
        /// Event handler that will be called when ASIC enters normal mode
        /// </summary>
        /// <param name="sender">object that called this event. Must be one of ASIC objects</param>
        /// <param name="e">unused</param>
        private async void NormalModeEnteredHandler(object? sender, EventArgs e)
        {
            // Precondition
            if (sender == null)
            {
                throw new ArgumentNullException("Incorrect argument - can't be null!");
            }

            // Typecast sender to actual type
            IASIC caller = (IASIC) sender;

            // Actual logic

            // Unsubscribe from event - by design can be fired only once
            caller.InitModeEntered -= NormalModeEnteredHandler;
        }

        #endregion // ASIC_events
    }
}