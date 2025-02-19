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

            SyncPulsGeneratingStatusText = "ON";
        }

        #region State_Machine

        #endregion //State_Machine

        #region Bindable_Properties

        /// <summary>
        /// </summary>
        private string uartTopStatusText;
        
        /// <summary>
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
        /// </summary>
        private string uart1StatusText;
        
        /// <summary>
        /// </summary>
        public string UART1StatusText
        {
            get => uart1StatusText;
            set 
            {
                uart1StatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private string syncPulsGeneratingStatusText;
        
        /// <summary>
        /// </summary>
        public string SyncPulsGeneratingStatusText
        {
            get => syncPulsGeneratingStatusText;
            set 
            {
                syncPulsGeneratingStatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private string crashDataPSIStatusText;
        
        /// <summary>
        /// </summary>
        public string CrashDataPSIStatusText
        {
            get => crashDataPSIStatusText;
            set 
            {
                crashDataPSIStatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private string psiTopStatusText;
        
        /// <summary>
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
        /// </summary>
        private string ch1StatusText;
        
        /// <summary>
        /// </summary>
        public string Ch1StatusText
        {
            get => ch1StatusText;
            set 
            {
                ch1StatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private string ch2StatusText;
        
        /// <summary>
        /// </summary>
        public string Ch2StatusText
        {
            get => ch2StatusText;
            set 
            {
                ch2StatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private string ch3StatusText;
        
        /// <summary>
        /// </summary>
        public string Ch3StatusText
        {
            get => ch3StatusText;
            set 
            {
                ch3StatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private string ch4StatusText;
        
        /// <summary>
        /// </summary>
        public string Ch4StatusText
        {
            get => ch4StatusText;
            set 
            {
                ch4StatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private string ch5StatusText;
        
        /// <summary>
        /// </summary>
        public string Ch5StatusText
        {
            get => ch5StatusText;
            set 
            {
                ch5StatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private string ch6StatusText;
        
        /// <summary>
        /// </summary>
        public string Ch6StatusText
        {
            get => ch6StatusText;
            set 
            {
                ch6StatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private string ch7StatusText;
        
        /// <summary>
        /// </summary>
        public string Ch7StatusText
        {
            get => ch7StatusText;
            set 
            {
                ch7StatusText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private string ch8StatusText;
        
        /// <summary>
        /// </summary>
        public string Ch8StatusText
        {
            get => ch8StatusText;
            set 
            {
                ch8StatusText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot1Ch1SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot1Ch1SensorDataText
        {
            get => slot1Ch1SensorDataText;
            set 
            {
                slot1Ch1SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot1Ch2SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot1Ch2SensorDataText
        {
            get => slot1Ch2SensorDataText;
            set 
            {
                slot1Ch2SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot1Ch3SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot1Ch3SensorDataText
        {
            get => slot1Ch3SensorDataText;
            set 
            {
                slot1Ch3SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot1Ch4SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot1Ch4SensorDataText
        {
            get => slot1Ch4SensorDataText;
            set 
            {
                slot1Ch4SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot1Ch5SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot1Ch5SensorDataText
        {
            get => slot1Ch5SensorDataText;
            set 
            {
                slot1Ch5SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot1Ch6SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot1Ch6SensorDataText
        {
            get => slot1Ch6SensorDataText;
            set 
            {
                slot1Ch6SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot1Ch7SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot1Ch7SensorDataText
        {
            get => slot1Ch7SensorDataText;
            set 
            {
                slot1Ch7SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot1Ch8SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot1Ch8SensorDataText
        {
            get => slot1Ch8SensorDataText;
            set 
            {
                slot1Ch8SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot2Ch1SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot2Ch1SensorDataText
        {
            get => slot2Ch1SensorDataText;
            set 
            {
                slot2Ch1SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot2Ch2SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot2Ch2SensorDataText
        {
            get => slot2Ch2SensorDataText;
            set 
            {
                slot2Ch2SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot2Ch3SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot2Ch3SensorDataText
        {
            get => slot2Ch3SensorDataText;
            set 
            {
                slot2Ch3SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot2Ch4SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot2Ch4SensorDataText
        {
            get => slot2Ch4SensorDataText;
            set 
            {
                slot2Ch4SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot2Ch5SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot2Ch5SensorDataText
        {
            get => slot2Ch5SensorDataText;
            set 
            {
                slot2Ch5SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot2Ch6SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot2Ch6SensorDataText
        {
            get => slot2Ch6SensorDataText;
            set 
            {
                slot2Ch6SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot2Ch7SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot2Ch7SensorDataText
        {
            get => slot2Ch7SensorDataText;
            set 
            {
                slot2Ch7SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot2Ch8SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot2Ch8SensorDataText
        {
            get => slot2Ch8SensorDataText;
            set 
            {
                slot2Ch8SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot3Ch1SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot3Ch1SensorDataText
        {
            get => slot3Ch1SensorDataText;
            set 
            {
                slot3Ch1SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot3Ch2SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot3Ch2SensorDataText
        {
            get => slot3Ch2SensorDataText;
            set 
            {
                slot3Ch2SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot3Ch3SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot3Ch3SensorDataText
        {
            get => slot3Ch3SensorDataText;
            set 
            {
                slot3Ch3SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot3Ch4SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot3Ch4SensorDataText
        {
            get => slot3Ch4SensorDataText;
            set 
            {
                slot3Ch4SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot3Ch5SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot3Ch5SensorDataText
        {
            get => slot3Ch5SensorDataText;
            set 
            {
                slot3Ch5SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot3Ch6SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot3Ch6SensorDataText
        {
            get => slot3Ch6SensorDataText;
            set 
            {
                slot3Ch6SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot3Ch7SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot3Ch7SensorDataText
        {
            get => slot3Ch7SensorDataText;
            set 
            {
                slot3Ch7SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot3Ch8SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot3Ch8SensorDataText
        {
            get => slot3Ch8SensorDataText;
            set 
            {
                slot3Ch8SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot4Ch1SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot4Ch1SensorDataText
        {
            get => slot4Ch1SensorDataText;
            set 
            {
                slot4Ch1SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot4Ch2SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot4Ch2SensorDataText
        {
            get => slot4Ch2SensorDataText;
            set 
            {
                slot4Ch2SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot4Ch3SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot4Ch3SensorDataText
        {
            get => slot4Ch3SensorDataText;
            set 
            {
                slot4Ch3SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot4Ch4SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot4Ch4SensorDataText
        {
            get => slot4Ch4SensorDataText;
            set 
            {
                slot4Ch4SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot4Ch5SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot4Ch5SensorDataText
        {
            get => slot4Ch5SensorDataText;
            set 
            {
                slot4Ch5SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot4Ch6SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot4Ch6SensorDataText
        {
            get => slot4Ch6SensorDataText;
            set 
            {
                slot4Ch6SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot4Ch7SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot4Ch7SensorDataText
        {
            get => slot4Ch7SensorDataText;
            set 
            {
                slot4Ch7SensorDataText = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        private string slot4Ch8SensorDataText;
        
        /// <summary>
        /// </summary>
        public string Slot4Ch8SensorDataText
        {
            get => slot4Ch8SensorDataText;
            set 
            {
                slot4Ch8SensorDataText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private FaultStatus uartTopStatus;
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
        /// </summary>
        private FaultStatus syncPulsGeneretingStatus;
        public FaultStatus SyncPulsGeneretingStatus
        {
            get => syncPulsGeneretingStatus;
            set
            {
                syncPulsGeneretingStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
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
        /// </summary>
        private FaultStatus psiTopStatus;
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
        /// </summary>
        private FaultStatus psiDataReceivedStatus;
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
        /// </summary>
        private FaultStatus crashDataPSIStatus;
        public FaultStatus CrashDataPSIStatus
        {
            get => crashDataPSIStatus;
            set
            {
                crashDataPSIStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// </summary>
        private FaultStatus ch1Status;
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

        #endregion // Bindable_Properties

        #region Internal_configuration

        #endregion // Internal_configuration

        #region Commands

        #endregion // Commands

        #region ASIC_events

        #endregion // ASIC_events
    }
}