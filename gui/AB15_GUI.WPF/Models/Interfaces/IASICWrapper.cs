using System;
using System.Collections.ObjectModel;

namespace AB15_GUI.WPF.Models.Interfaces
{
    /// <summary>
    /// ASIC wrapper. Organizes work of few ASICs
    /// </summary>
    public interface IASICWrapper
    {
        /// <summary>
        /// List of ASICs connected to the MCU. ASIcs are passes via interface for testability
        /// Can hold max 4 ASICs (HW limitation)
        /// </summary>
        /// <typeparam name="IASIC">ASIC class interface</typeparam>
        ObservableCollection<IASIC> ASICs { get; }

        /// <summary>
        /// Connect to all available ASICs
        /// </summary>
        void EstablishConnection();

        /// <summary>
        /// Arm ASIC's to reset INIT mode timeout
        /// </summary>
        void StartInitModeTimeoutResetting();
    }
}