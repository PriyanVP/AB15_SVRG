using NLog.Targets;
using NLog;
using System.Collections.Generic;
using AB15_GUI.WPF.Models;

namespace AB15_GUI.WPF.NLog
{
    /// <summary>
    /// Writes log messages to memory for programmatic retrieval
    /// Records stored in format useful for UI binding
    /// </summary>
    [Target("MemoryRecord")]
    public sealed class LogMemoryRecordTarget : TargetWithLayout
    {
        /// <summary>
        /// Constant that defines max size of logs list
        /// </summary> 
        private const int MaxListSize = 100;

        /// <summary>
        /// Internal list that stores records and implements notifications
        /// </summary>
        private readonly ThreadSafeObservableList<LoggerRecord> _logs = new ThreadSafeObservableList<LoggerRecord>(MaxListSize);

        public LogMemoryRecordTarget()
        {
            //
        }

        /// <summary>
        /// Gets the list of logs gathered in the <see cref="MemoryRecordTarget"/>.
        /// Exposed as IList. Underlying implementation implements change notifications for the list
        /// </summary>
        /// <remarks>
        /// Thread safety is implemented using lock
        /// </remarks>
        public IList<LoggerRecord> Logs => _logs;

        /// <summary>
        /// Event handler that is called when new log record received
        /// </summary>
        /// <param name="logEvent">log messaage with all required fields</param>
        protected override void Write(LogEventInfo logEvent)
        {
            LoggerRecord logRecord = new LoggerRecord();
            logRecord.TimeStamp = logEvent.TimeStamp;
            logRecord.Index = logEvent.SequenceID;
            logRecord.Level = logEvent.Level.ToString().ToUpper();
            logRecord.Message = logEvent.Message;

            // Invoking operations on logs list on GUI thread
            App.Current.Dispatcher.Invoke(() =>
            {
                _logs.Add(logRecord);
            });
        }
    }
}
