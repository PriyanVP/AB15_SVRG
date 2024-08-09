using System;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Command state abstraction to handle common UI scenarios
    /// </summary>
    public class CommandState
    {
        /// <summary>
        /// Enable command functionality
        /// Write only
        /// </summary>
        public bool Enable { private get; set; } = false;

        /// <summary>
        /// Flag to indicate if command is in execution to avoid reentry
        /// Write only
        /// </summary>
        public bool InProgress { private get; set; } = false;

        /// <summary>
        /// Flag indicating if command can be executed now
        /// Command can't be executed if disabled or if in process of previous execution
        /// Read only
        /// </summary>
        public bool IsEnabled 
        { 
            get => (Enable && (!InProgress));
        }
    }
}
