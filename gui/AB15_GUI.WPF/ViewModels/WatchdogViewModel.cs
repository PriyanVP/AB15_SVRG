using AB15_GUI.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Windows.Input;
using System.Windows;
using AB15_GUI.WPF.ViewModels.Commands;

namespace AB15_GUI.WPF.ViewModels
{
    /// <summary>
    /// Faults status value definition
    /// </summary>
    public enum UIFaultStatus
    {
        NoStatus,
        Good,
        Fault
    }

    public class WatchdogViewModel : ViewModelBase
    {
        /// <summary>
        /// toggle enable to configure EN0 thresholds 
        /// </summary>
        private bool isEN0Enabled;
        public bool IsEN0Enabled
        {
            get => isEN0Enabled;
            set 
            { 
                isEN0Enabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// toggle enable to stop watchdog button
        /// </summary>
        private bool isStopWDButtonEnabled;
        public bool IsStopWDButtonEnabled
        {
            get => isStopWDButtonEnabled;
            set
            {
                isStopWDButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// toggle enable to start watchdog button
        /// </summary>
        private bool isStartWDButtonEnabled;
        public bool IsStartWDButtonEnabled
        {
            get => isStartWDButtonEnabled;
            set
            {
                isStartWDButtonEnabled = value;
                OnPropertyChanged();
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
                wd1ResponseTime = value;
                OnPropertyChanged();
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
                wd1LockTime = value;
                OnPropertyChanged();
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
                wd2ResponseTime = value;
                OnPropertyChanged();
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
                wd1EN0DisableThreshold = value;
                OnPropertyChanged();
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
                wd2EN0DisableThreshold = value;
                OnPropertyChanged();
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
                wd2LockTime = value;
                OnPropertyChanged();
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
        /// Event from Read config from ASIC button
        /// </summary>
        private ICommand readConfigFromASIC;
        public ICommand ReadConfigFromASIC
        {
            get => readConfigFromASIC;
            set
            {
                readConfigFromASIC = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Event from Write config to ASIC button
        /// </summary>
        private ICommand writeConfigToASIC;
        public ICommand WriteConfigToASIC
        {
            get => writeConfigToASIC;
            set
            {
                writeConfigToASIC = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Global wd fault flag
        /// </summary>
        private UIFaultStatus wdFaultStatus;
        public UIFaultStatus WDFaultStatus
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
        private UIFaultStatus wd1FaultStatus;
        public UIFaultStatus WD1FaultStatus
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
        private UIFaultStatus wd2FaultStatus;
        public UIFaultStatus WD2FaultStatus
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
        private UIFaultStatus errorPinFaultStatus;
        public UIFaultStatus ErrorPinFaultStatus
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
        private UIFaultStatus wd1TimerFaultStatus;
        public UIFaultStatus WD1TimerFaultStatus
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
        private UIFaultStatus wd2TimerFaultStatus;
        public UIFaultStatus WD2TimerFaultStatus
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
        private UIFaultStatus oscmonFaultStatus;
        public UIFaultStatus OSCMONFaultStatus
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
        private UIFaultStatus wd1QAFaultStatus;
        public UIFaultStatus WD1QAFaultStatus
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
        private UIFaultStatus wd2QAFaultStatus;
        public UIFaultStatus WD2QAFaultStatus
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
        private UIFaultStatus oscillatorFaultStatus;
        public UIFaultStatus OscillatorFaultStatus
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
        private UIFaultStatus wdResetFaultStatus;
        public UIFaultStatus WDResetFaultStatus
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
        private UIFaultStatus wd1CounterFaultStatus;
        public UIFaultStatus WD1CounterFaultStatus
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
        private UIFaultStatus qa1FaultStatus;
        public UIFaultStatus QA1FaultStatus
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
        private UIFaultStatus wd2CounterFaultStatus;
        public UIFaultStatus WD2CounterFaultStatus
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
        private UIFaultStatus qa2FaultStatus;
        public UIFaultStatus QA2FaultStatus
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
        private UIFaultStatus en0HightStatus;
        public UIFaultStatus EN0HightStatus
        {
            get => en0HightStatus;
            set
            {
                en0HightStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Local logger instance
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public WatchdogViewModel(Logger logger)
        {
            this.logger = logger;
            logger.Trace("In WatchdogViewModel");

            // set defoult value
            //IsConfigEnable = false;
            //IsStartWDButtonEnabled = false;
            //IsStopWDButtonEnabled = false;

            // Init commands for buttons
            ReadConfigFromASIC = new RelayCommand(ReadConfigFromASICExecute, ReadConfigFromASICCanExecute);
            WriteConfigToASIC = new RelayCommand(WriteConfigToASICExecute, WriteConfigToASICCanExecute);

        }

        /// <summary>
        /// Read config from ASIC check command arguments 
        /// </summary>
        /// <returns>true if command can be executed</returns>
        private bool ReadConfigFromASICCanExecute(object obj)
        {
            // TODO Add actual check
            return true;
        }

        /// <summary>
        /// Execute  Read config from ASIC command
        /// </summary>
        private void ReadConfigFromASICExecute(object obj)
        {
        }

        /// <summary>
        /// Write config to ASIC check command arguments 
        /// </summary>
        /// <returns>true if command can be executed</returns>
        private bool WriteConfigToASICCanExecute(object obj)
        {
            // TODO Add actual check
            return true;
        }

        /// <summary>
        /// Execute write config to ASIC command
        /// </summary>
        private void WriteConfigToASICExecute(object obj)
        {
        }
    }
}
