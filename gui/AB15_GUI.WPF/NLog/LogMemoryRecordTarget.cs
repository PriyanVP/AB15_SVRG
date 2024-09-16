using NLog.Targets;
using NLog;
using System.Collections.Generic;
using AB15_GUI.WPF.Models;
using System.Windows.Threading;

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
        private const int MaxListSize = 50;

        private readonly object _lock = new object();

        /// <summary>
        /// Internal list that stores records and implements notifications
        /// </summary>
        private readonly ThreadSafeObservableList<LoggerRecord> _logs = new ThreadSafeObservableList<LoggerRecord>(MaxListSize);

        /// <summary>
        /// Internal list for observable list for <see cref="Logs"/>.
        /// </summary>
        private ThreadSafeObservableList<LoggerRecord> _observableLogs = new ThreadSafeObservableList<LoggerRecord>(MaxListSize);

        /// <summary>
        /// Gets the list of logs gathered in the <see cref="MemoryRecordTarget"/>.
        /// Exposed as IList. Underlying implementation implements change notifications for the list
        /// </summary>
        /// <remarks>
        /// Thread safety is implemented using lock
        /// </remarks>
        public IList<LoggerRecord> Logs => _observableLogs;

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
            // TODO: refactor thread synchronization approach. Current one is closely coupled to WPF
            _logs.Add(logRecord);
            SynchronizeLists();
        }

        /// <summary>
        /// Method to update content of bindable list
        /// </summary>
        private void SynchronizeLists()
        {
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                lock (_lock)
                {
                    _observableLogs.Clear();
                    foreach (var item in _logs)
                    {
                        _observableLogs.Add(item);
                    }
                }
            }, DispatcherPriority.Background);
        }
    }
}
