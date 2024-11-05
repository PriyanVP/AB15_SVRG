using System;
using NLog;
using AB15_GUI.WPF.Services.Interfaces;

namespace AB15_GUI.WPF.ViewModels
{
    /// <summary>
    /// Faults status value definition
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
        private readonly Logger logger;

        /// <summary>
        /// SerialWrapper reference to perform communication with MCU
        /// </summary>
        private readonly ISerialWrapper serialWrapper;

        /// <summary>
        /// Constructor
        /// </summary>
        public FiringViewModel(Logger logger, ISerialWrapper serialWrapper)
        {
            this.logger = logger;
            this.serialWrapper = serialWrapper;
            logger.Trace("In FiringViewModel");

        }
    }
}
