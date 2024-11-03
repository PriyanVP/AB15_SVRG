using System;
using System.Collections.ObjectModel;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// ASIC wrapper. Organizes work of few ASICs
    /// </summary>
    public class ASICWrapper : IASICWrapper
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ASICWrapper(IASIC ASIC1)
        {
            // Assign IDs
            ASIC1.ID = 1;

            // Add ASICs to list
            ASICs.Add(ASIC1);
        }

        /// <summary>
        /// List of ASICs connected to the MCU. ASIcs are passes via interface for testability
        /// Can hold max 4 ASICs (HW limitation)
        /// </summary>
        /// <typeparam name="IASIC">ASIC class interface</typeparam>
        public ObservableCollection<IASIC> ASICs { get; private set; } = new ObservableCollection<IASIC>();

        /// <summary>
        /// Connect to all available ASICs
        /// </summary>
        public void EstablishConnection()
        {
            // Call ASIC reset - will ensure ASICs are in expected state and will determine if ASICs are present
            foreach (var ASIC in ASICs)
            {
                ASIC.ExecuteSPIColdstart1();
                ASIC.StartPeriodicStateReading();
            }
        }
    }
}
