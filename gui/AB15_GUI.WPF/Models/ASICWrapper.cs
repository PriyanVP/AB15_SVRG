using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

        // TODO: handling of offline ASIC's

        /// <summary>
        /// Connect to all available ASICs
        /// WARNING: will cause resets of all ASICs
        /// </summary>
        public async Task EstablishConnectionAsync()
        {
            // Call ASIC reset - will ensure ASICs are in expected state and will determine if ASICs are present
            foreach (var ASIC in ASICs)
            {
                await ASIC.ExecuteSPIColdstart1Async();
                ASIC.StartPeriodicStateReading();
            }
        }

        /// <summary>
        /// Arm ASIC's to reset INIT mode timeout
        /// </summary>
        public void StartInitModeTimeoutResetting()
        {
            foreach (var ASIC in ASICs)
            {
                ASIC.TestMode1Entered += StopInitModeTimeoutResetting;
                ASIC.StartInitModeTimeoutResetting();
            }
        }

        /// <summary>
        /// Event handler that will stop INIT mode timeout resetting and unsubscribe from event
        /// </summary>
        /// <param name="sender">object that called this event. Must be one of ASIC objects</param>
        /// <param name="e">unused</param>
        private void StopInitModeTimeoutResetting(object? sender, EventArgs e)
        {
            // Precondition
            if (sender == null)
            {
                throw new ArgumentNullException("Incorrect argument - can't be null!");
            }
            // Typecast sender to actual type
            IASIC caller = (IASIC) sender;

            // Stop init mode timeout resetting
            caller.StopInitModeTimeoutResetting();

            // Unsubscribe from event
            caller.TestMode1Entered -= StopInitModeTimeoutResetting;
        }
    }
}
