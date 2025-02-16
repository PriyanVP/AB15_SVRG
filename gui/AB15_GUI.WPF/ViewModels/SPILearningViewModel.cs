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

        private string _hexValueAddress = "";

        public string HexValueAddress
        {
            get => _hexValueAddress;
            set
            {
                _hexValueAddress = value;
                OnPropertyChanged();
                if (value == "")
                {
                    RefToActiveRecord.MOSI.Address = 0;
                }
                else
                {
                    RefToActiveRecord.MOSI.Address = uint.Parse(value, System.Globalization.NumberStyles.HexNumber);
                }
            }
        }

        private string _hexValueData = "";

        public string HexValueData
        {
            get => _hexValueData;
            set
            {
                _hexValueData = value;
                OnPropertyChanged();
                if (value == "")
                {
                    RefToActiveRecord.MOSI.Data = 0;
                }
                else
                {
                    RefToActiveRecord.MOSI.Data = ushort.Parse(value, System.Globalization.NumberStyles.HexNumber);
                }
            }
        }

        private string _hexValueRaw = "";

        public string HexValueRaw
        {
            get => _hexValueData;
            set
            {
                _hexValueData = value;
                OnPropertyChanged();
                if (value == "")
                {
                    RefToActiveRecord.MOSI.RawMOSI = 0;
                }
                else
                {
                    RefToActiveRecord.MOSI.RawMOSI = uint.Parse(value, System.Globalization.NumberStyles.HexNumber);
                }
            }
        }


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

            // Enable all by default
            IsSPILearningEn = true;
            SPICommandCommandEn = true;

            // Create record for SPI transaction
            RefToActiveRecord = new SPITransactionRecord();
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
                RefToActiveRecord.MOSI.Data = (isWriteRegisterSelected) ? ((UInt16) 0x0) : (RefToActiveRecord.MOSI.Data);
                RefToActiveRecord.MOSI.RwFlag = isWriteRegisterSelected;
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
            // TODO: add later
            // Bindable properties help messages
            // AddHelpMsg(nameof(), $"");

            // Commands
            // AddHelpMsg(nameof(), $"");

            // UI elements help messages
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
            packageToSend.ASICID = 1; // TODO: should be a value from dropdown
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

        #endregion // Commands
    }
}