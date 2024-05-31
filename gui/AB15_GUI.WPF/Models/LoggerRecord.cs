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

        /// <summary>
        ///  Color associated with logging level property
        /// </summary>
        public string LevelColor
        {
            get 
            {
                string color;
                switch (Level)
                {
                    case "TRACE":
                        color = "grey";
                        break;
                    case "DEBUG":
                        color = "light green";
                        break;
                    case "INFO":
                        color = "green";
                        break;
                    case "WARN":
                        color = "yellow";
                        break;
                    case "ERROR":
                        color = "red";
                        break;
                    case "FATAL":
                        color = "dark red";
                        break;
                    default:
                        color = "black";
                        break;
                }
                return color;
            }
        } 
    }
}
