using System;
using System.Collections.Generic;
using NLog;
using Stateless;
using Stateless.Graph;
using System.Windows.Input;
using AB15_GUI.WPF.ViewModels.Commands;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Genereted.Registers;
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