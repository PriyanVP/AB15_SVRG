using System;

namespace AB15_GUI.WPF.Models
{
    public class LoggerRecord
    {
        public DateTime TimeStamp { get; set; }

        public string? Level { get; set; }

        public int? Index { get; set; }

        public string? Message { get; set; }

        // TODO: add info about class and line number?
    }
}
