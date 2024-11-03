using System;
using System.Collections.Generic;
using System.ComponentModel;
using AB15_GUI.WPF.Models;

namespace AB15_GUI.WPF.Models.Interfaces
{
    /// <summary>
    /// ASIC top level control and status interface
    /// </summary>
    public interface IASIC
    {
        /// <summary>
        /// ASIC ID relative to MCU
        /// 0 - not set, 1 - master, 2-4 - slaves
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Flag indicating if communication to ASIC is operational
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// ASIC EOP flag. If set most configuration is locked
        /// </summary>
        bool EOP { get; }

        /// <summary>
        /// ASIC state
        /// </summary>
        ASICState State { get; }

        /// <summary>
        /// List that holds configuration data for ASIC 
        /// (data that is written in INIT mode and used for CRC Cyclic checker)
        /// </summary>
        List<IRegister> ConfigData { get; }
        
        
        
        /// <summary>
        /// Event for notification if ASIC has entered INIT mode
        /// </summary>
        event EventHandler? InitModeEntered; 

        /// <summary>
        /// Event for notification if configuration data was loaded and cyclic CRC applied
        /// </summary>
        event EventHandler? ConfigurationLoaded;
        
        /// <summary>
        /// Event for notification if EOP flag is set
        /// </summary>
        event EventHandler? ConfigurationLocked;

        /// <summary>
        /// Event for notification if Test mode 1 is entered
        /// </summary> 
        event EventHandler? TestMode1Entered;
        
        /// <summary>
        /// Event for notification if Test mode 2 is entered
        /// </summary> 
        event EventHandler? TestMode2Entered;

        /// <summary>
        /// Event for notification if Normal mode is entered
        /// </summary>   
        event EventHandler? NormalModeEntered;

        /// <summary>
        /// Event for notification if error in any of the callbacks is set
        /// </summary>
        event EventHandler<CallbackErrorEventArgs>? ErrorCallbackReceived;

        /// <summary>
        /// Event for notification if property has changed
        /// </summary>
        event PropertyChangedEventHandler? PropertyChanged;


        /// <summary>
        /// Method to add list with configuration registers to global list
        /// Values are added as references (2-side modification possible)
        /// Sorting is performed afterwards for ease of analysis
        /// </summary>
        /// <param name="listToAppend">list that will be appended</param>
        void AppendConfigRegisters(List<IRegister> listToAppend);
        
        /// <summary>
        /// Start timer to arm periodic ASIC state readings
        /// </summary>
        /// <param name="timeout">timer timeout in ms. Defaults to 0.5s</param>   
        void StartPeriodicStateReading(int timeout = 500);

        /// <summary>
        /// Stop and dispose timer. No update of ASIC state will be done automatically
        /// </summary>        
        void StopPeriodicStateReading();

        /// <summary>
        /// Write configuration data and apply CRC
        /// </summary>
        void WriteConfigurationWithCRC();

        /// <summary>
        /// Lock configuration (EOP)
        /// </summary>        
        void LockConfiguration();

        /// <summary>
        /// Execute SPI_COLDSTART1 command
        /// Will cause ASIC reset (unconditionally)
        /// </summary>
        public void ExecuteSPIColdstart1();
        
        /// <summary>
        /// Execute transition from test mode 1 command
        /// </summary>        
        void ExecuteTestMode1Transition();
        
        /// <summary>
        /// Execute transition from test mode 2 command
        /// </summary>        
        void ExecuteTestMode2Transition();
        
        /// <summary>
        /// Read current ASIC state
        /// </summary>        
        void GetASICState();
    }
}