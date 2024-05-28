using System;
using System.Collections.Generic;
using NLog;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.NLog;

namespace AB15_GUI.WPF.ViewModels
{
    public class LoggerViewModel : ViewModelBase
    {
        /// <summary>
        /// Logger reference with custom configuration
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// Property that can be used to bind logger data to UI
        /// </summary>
        public IList<LoggerRecord> LoggerRecords { get; private set; }

        /// <summary>
        /// Constructor for creating LoggerViewModel. Data connection to observable property is done there
        /// </summary>
        /// <param name="logger">logger reference</param>
        public LoggerViewModel(Logger logger)
        {
            this.logger = logger;

            this.logger.Trace("In LoggerViewModel constructor");

            // Connect logger data to observable list in ViewModel
            LogMemoryRecordTarget target = (LogMemoryRecordTarget)LogManager.Configuration.FindTargetByName("memory");
            LoggerRecords = target.Logs;
        }
    }
}
