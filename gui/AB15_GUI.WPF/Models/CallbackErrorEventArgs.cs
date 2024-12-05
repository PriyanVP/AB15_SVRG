using System;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class to pass error data from callback method
    /// </summary>
    public class CallbackErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Error message. Expected to be set in callback delegate for communication with MCU
        /// </summary>
        public string? Error { get; set; }
    }
}