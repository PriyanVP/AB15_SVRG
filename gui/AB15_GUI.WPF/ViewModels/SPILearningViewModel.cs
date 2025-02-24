using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.ViewModels.Commands;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Services.Interfaces;

namespace AB15_GUI.WPF.ViewModels
{
    public class SPILearningViewModel : ViewModelBase
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
        public SPILearningViewModel(ILoggingService logger, ISerialWrapper serialWrapper, IASICWrapper asicWrapper) :
                base(logger)
        {
            // Assign references to objects to local variables
            this.logger = logger;
            this.serialWrapper = serialWrapper;
            this.asicWrapper = asicWrapper;
            logger.Trace("In SPILearningViewModel");

            // Init help messages for UI
            InitHelpMessages();

            // Enables synchronization for multithread access to observable collection
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(SPICommunicationTable, _lock);

            // Init commands for controls
            SPICommand = new RelayCommand(SPICommandExecuteAsync);
            RawValueUpdatedCommand = new RelayCommand(RawValueUpdated);

            // Enable all by default
            IsSPILearningEn = true;
            SPICommandCommandEn = true;

            // Create record for SPI transaction
            RefToActiveRecord = new SPITransactionRecord();

            // TESTING WITH MOCKUP DATA
            SPICommunicationTable = new ObservableCollection<SPITransactionRecord>();
            for (var indexer = 0; indexer < 30; indexer++)
            {
                SPICommunicationTable.Add(new SPITransactionRecord(new MISORecord() { RawMISO = (uint)(123456 * indexer) }, new MOSIRecord() { RawMOSI = (uint)(246810 * indexer) }));
            }
        }
        
        #region Bindable_Properties

        /// <summary>
        /// Observable collection for displaying SPI transactions
        /// </summary>
        public ObservableCollection<SPITransactionRecord> SPICommunicationTable { get; private set; } = new ObservableCollection<SPITransactionRecord>();

        /// <summary>
        /// <inheritdoc cref="RefToActiveRecord" path='/summary'/>
        /// </summary>
        private SPITransactionRecord refToActiveRecord;
        
        /// <summary>
        /// Reference to current SPI transaction record
        /// </summary>
        public SPITransactionRecord RefToActiveRecord
        {
            get => refToActiveRecord;
            set 
            {
                refToActiveRecord = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="SelectedASICToSend" path='/summary'/>
        /// </summary>
        private DeviceIDs selectedASICToSend = DeviceIDs.SPI1_CS1MASTER; 
        
        /// <summary>
        /// The selected Device ID
        /// </summary>
        public DeviceIDs SelectedASICToSend
        {
            get => selectedASICToSend;
            set 
            {
                selectedASICToSend = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// <inheritdoc cref="IsSPILearningEn" path='/summary'/>
        /// </summary>
        private bool isSPILearningEn;
        
        /// <summary>
        /// Flag to indicate if SPI Learning tab can be used
        /// </summary>
        public bool IsSPILearningEn
        {
            get => isSPILearningEn;
            set 
            {
                isSPILearningEn = value;
                OnPropertyChanged();
            }
        }

        // /// <summary>
        // /// <inheritdoc cref="FiringConfigurationTextField" path='/summary'/>
        // /// </summary>
        // private string firingConfigurationTextField;
        
        // /// <summary>
        // /// Binding property to display text on text block on Configuration tab
        // /// </summary>
        // public string FiringConfigurationTextField
        // {
        //     get => firingConfigurationTextField;
        //     set 
        //     {
        //         // Do nothing if value is not changed
        //         if (firingConfigurationTextField == value) return;

        //         firingConfigurationTextField = value;
        //         OnPropertyChanged();
        //     }
        // }

        /// <summary>
        /// <inheritdoc cref="IsWriteRegisterSelected" path='/summary'/>
        /// </summary>
        private bool isWriteRegisterSelected;
        
        /// <summary>
        /// Flag to indicate if writing register should be done (if false, then write operation will be executed)
        /// </summary>
        public bool IsWriteRegisterSelected
        {
            get => isWriteRegisterSelected;
            set 
            {
                isWriteRegisterSelected = value;
                OnPropertyChanged();

                // Also update SPI transaction
                if (IsRawSPISelected == false)
                {
                    RefToActiveRecord.MOSI.Data = isWriteRegisterSelected ? RefToActiveRecord.MOSI.Data : ((UInt16) 0x0);
                    RefToActiveRecord.MOSI.RwFlag = isWriteRegisterSelected;
                }
            }
        }

        /// <summary>
        /// <inheritdoc cref="IsRawSPISelected" path='/summary'/>
        /// </summary>
        private bool isRawSPISelected;
        
        /// <summary>
        /// Flag to indicate if raw SPI command will be executed
        /// </summary>
        public bool IsRawSPISelected
        {
            get => isRawSPISelected;
            set 
            {
                isRawSPISelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Set messages for help provider on UI
        /// </summary>
        private void InitHelpMessages()
        {
            // Bindable properties help messages
            AddHelpMsg(nameof(IsRawSPISelected), "Enable or disable the RAW value input");
            AddHelpMsg(nameof(RefToActiveRecord.Time), "Frame Time");
            // MOSI
            AddHelpMsg(nameof(RefToActiveRecord.MOSI.Address), "Register address (10 Bit)");
            AddHelpMsg(nameof(RefToActiveRecord.MOSI.RwFlag), "Indicates transfer direction for the address:\r\n R = read\r\n W = write");
            AddHelpMsg("MOSI" + nameof(RefToActiveRecord.MOSI.Data), "Data being written into registers");
            AddHelpMsg("MOSI" + nameof(RefToActiveRecord.MOSI.CRC), "Checksum input: Calculated using bits 31 to 5 of MOSI frame");
            AddHelpMsg(nameof(RefToActiveRecord.MOSI.RawMOSI), "RAW Receive Frame (MOSI)");
            // MISO
            AddHelpMsg(nameof(RefToActiveRecord.MISO.GS5), "Transfer Failure Flag (TFF):\r\n 0 = OK\r\n 1 = Failure in previous SPI Communication");
            AddHelpMsg(nameof(RefToActiveRecord.MISO.GS4), "Test Active Flag (TST):\r\n 0 = OK\r\n 1 = ASIC in test mode");
            AddHelpMsg(nameof(RefToActiveRecord.MISO.GS3), "End Of Programming (EOP):\r\n 0 = EOP1 and EOP2 set\r\n 1 = EOP1 and/or EOP2 not set");
            AddHelpMsg(nameof(RefToActiveRecord.MISO.GS2), "APB Bus Transaction Error (APB):\r\n 0 = OK\r\n 1 = Failure in the previous Read/Write APB transaction");
            AddHelpMsg(nameof(RefToActiveRecord.MISO.GS1), "ADC Error (ADC):\r\n 0 = OK\r\n 1 = ADC error has occurred");
            AddHelpMsg(nameof(RefToActiveRecord.MISO.GS0), "APB Access Time Error (ATE):\r\n 0 = Ok\r\n 1 = APB bus access from the previous SPI frame was not executed on time and correctness previous request is not guaranteed");
            AddHelpMsg(nameof(RefToActiveRecord.MISO.SF), "N/A");
            AddHelpMsg(nameof(RefToActiveRecord.MISO.S0), "ASIC Error Flag:\r\n 0 = ASIC error not present\r\n 1 = Oscillator Monitor Error (Oscillator failure) or Current APB Read Access Time Error (ATE) or Current APB Read Bus Transaction Error (APB)");
            AddHelpMsg(nameof(RefToActiveRecord.MISO.AdditionalStatus), "Additional Status flags");
            AddHelpMsg("MISO" + nameof(RefToActiveRecord.MISO.Data), "Data read from the register (Set to 0 in case of SPI WR transaction)");
            AddHelpMsg("MISO" + nameof(RefToActiveRecord.MISO.CRC), "Checksum output: Calculated using bits 31 to 3 of MISO frame");
            AddHelpMsg(nameof(RefToActiveRecord.MISO.RawMISO), "RAW Transmit Frame (MISO)");

            // Commands
            AddHelpMsg(nameof(SPICommand), "Execute the selected SPI command");

            // UI elements help messages
            AddHelpMsg("RW", "Indicates transfer direction for the address:\r\n 0 = read\r\n 1 = write");
            AddHelpMsg("NA", "Unused constant for bit 0 and bit 1");
            AddHelpMsg("ReadWriteSwitch", "Switch between Read and Write frame type");
            AddHelpMsg("DeviceIDSelector", "Device ID");
        }

        #endregion // Bindable_Properties

        #region Commands
       
        /// <summary>
        /// Command handler for executing SPI command
        /// </summary>
        public ICommand SPICommand { get; }
       
        /// <summary>
        /// Bindable SPI command button/command enable state
        /// </summary>
        public bool SPICommandCommandEn { get; set; }

        /// <summary>
        /// Method that will execute SPI command (read, write or raw)
        /// </summary>
        private async void SPICommandExecuteAsync(object obj)
        {
            // Handle that command execution can only be done once in a row
            if (SPICommandCommandEn == false) return;
            OnPropertyChanged(nameof(SPICommand));
            IsSPILearningEn = false;

            // Update flag in record
            RefToActiveRecord.IsRawSPIFrame = IsRawSPISelected;

            // Create package to MCU
            TransmitCommunicationPackage<AddressDataPayload> packageToSend = new TransmitCommunicationPackage<AddressDataPayload>();
            packageToSend.ASICID = (int)SelectedASICToSend;
            packageToSend.Cmd = MCUCommand.WRITE_RAW_DATA_SPI;
            packageToSend.PayloadType = typeof(AddressDataPayload);
            packageToSend.Payload.Data.Add((UInt16) (RefToActiveRecord.MOSI.RawMOSI & 0xFFFF));        // 16 LSB
            packageToSend.Payload.Data.Add((UInt16)((RefToActiveRecord.MOSI.RawMOSI >> 16) & 0xFFFF)); // 16 MSB

            // Send command to MCU and wait for response
            ReceiveCommunicationPackage<AddressDataPayload>? mcuResponse = (ReceiveCommunicationPackage<AddressDataPayload>?) await serialWrapper.SerialWriteAsync(packageToSend);

            // Response received - unlock command usage
            SPICommandCommandEn = true;
            OnPropertyChanged(nameof(SPICommand));
            IsSPILearningEn = true;

            if (IsResponseValid(mcuResponse, nameof(SPICommand)) == false) return;
      
            // Update MISO frame only if data available
            if (mcuResponse.Payload.Data.Count == 2)
            {
                // If not updated here - error will be shown. MISO contains error frame (all 1s) by default
                RefToActiveRecord.MISO.RawMISO = (uint) (mcuResponse.Payload.Data[1] << 16) | mcuResponse.Payload.Data[0];
            }

            // Update table with SPI records
            RefToActiveRecord.Time = DateTime.Now;
            SPICommunicationTable.Add(RefToActiveRecord);

            // Create next record
            MOSIRecord CurrentMOSIRecordCopy = RefToActiveRecord.MOSI.Copy();
            RefToActiveRecord = new SPITransactionRecord(mosi: CurrentMOSIRecordCopy);
        }

        /// <summary>
        /// Command handler for updating IsWriteRegisterSelected based on RawValue 
        /// </summary>
        public ICommand RawValueUpdatedCommand { get; }

        /// <summary>
        /// Method that will update IsWriteRegisterSelected based on RawValue 
        /// </summary>
        private void RawValueUpdated(object obj)
        {
            IsWriteRegisterSelected = RefToActiveRecord.MOSI.RwFlag;
        }

        #endregion // Commands
    }
}