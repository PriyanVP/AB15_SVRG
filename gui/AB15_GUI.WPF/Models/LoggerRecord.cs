using System;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class for storing Logger records
    /// </summary>
    public class LoggerRecord
    {
        /// <summary>
        /// Time when log record was created
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Logging level of the record
        /// </summary>
        public string? Level { get; set; }

        /// <summary>
        /// Index of the message in current session
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// Log message text
        /// </summary>
        public string? Message { get; set; }
    }
}
