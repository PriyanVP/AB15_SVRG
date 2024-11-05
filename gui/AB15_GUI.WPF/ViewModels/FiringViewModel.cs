using System;
using System.Windows.Input;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.ViewModels.Commands;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Generated.Registers;
using AB15_GUI.WPF.Services.Interfaces;
using System.Collections.ObjectModel;


namespace AB15_GUI.WPF.ViewModels
{
    /// <summary>
    /// Faults status value definition TODO: is needed?
    /// </summary>
    public enum AsicChannelConfigStatus
    {
        NotConfigured,
        Configured,
        NotActive
    }

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
            this.logger = logger;
            this.serialWrapper = serialWrapper;
            this.asicWrapper = asicWrapper;
            logger.Trace("In FiringViewModel");

            // Init help messages for UI
            InitHelpMessages();

            // Init commands for controls
            ConfigurationChanged  = new RelayCommand(ConfigurationChangedExecute);

            // Init stage commands triggering
            ConfigurationChangedExecute("A - 1");

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




        
        #region Bindable_Properties

        // TODO:  refactor?

        public ObservableCollection<FiringChannelConfigurationRecord> FiringConfigurationTable { get; set; } = new ObservableCollection<FiringChannelConfigurationRecord>();


        public ObservableCollection<FiringResultRecord> FiringResultTable { get; set; } = new ObservableCollection<FiringResultRecord>();

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

        // private Reg_FLM_Config_ch2_1 
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

        // private Reg_spi_config_wd1 _spi_config_wd1 = new Reg_spi_config_wd1();
        // private Reg_spi_config_wd2 _spi_config_wd2 = new Reg_spi_config_wd2();
        // private Reg_spi_config_wd_decouple _spi_config_wd_decouple = new Reg_spi_config_wd_decouple();
        // private Reg_spi_config_wd_thres0 _spi_config_wd_thres0 = new Reg_spi_config_wd_thres0();
        // private Reg_spi_set_wdsettings _spi_set_wdsettings = new Reg_spi_set_wdsettings();
        // private Reg_spi_read_res_cause _spi_read_res_cause = new Reg_spi_read_res_cause();

        #endregion // Internal_configuration

        #region Commands

        /// <summary>
        /// Command handler for changing configuration
        /// </summary>
        public ICommand ConfigurationChanged { get; }



        /// <summary>
        /// Execute change of configuration
        /// </summary>
        private void ConfigurationChangedExecute(object commandParameter)
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








        #endregion // Commands




    }
}
