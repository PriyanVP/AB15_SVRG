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
        public LoggerViewModel(Logger logger, LogMemoryRecordTarget target)
        {
            this.logger = logger;

            this.logger.Trace("In LoggerViewModel constructor");

            // debug code
            this.logger.Trace("debug test color log");
            this.logger.Debug("debug test color log");
            this.logger.Info("debug test color log");
            this.logger.Warn("debug test color log");
            this.logger.Error("debug test color log");
            this.logger.Fatal("debug test color log");

            // Connect logger data to observable list in ViewModel
            LoggerRecords = target.Logs;
        }
    }
}
