using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    #region ObservableMembers

    /// <summary>
    /// Data record class that holds voltage measurement value
    /// </summary>
    public class VoltageRecord : INotifyPropertyChanged
    {     
        /// <summary>
        /// Voltage channel name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag to indicate if the voltage measurement is valid
        /// </summary>
        public bool IsValid { get; set; } = false;

        /// <summary>
        /// Measured voltage value
        /// </summary>
        public uint Value { get; set; }

        /// <summary>
        /// <inheritdoc cref="Status" path='/summary'/>
        /// </summary>
        private string status = "";
        
        /// <summary>
        /// Status of criteria
        /// </summary>
        public string Status
        { 
            get
            {
                return status;
            }
        }

        #endregion // ObservableMembers

        #region Data

        /// <summary>
        /// Method to update observable status based on data
        /// </summary>
        public void UpdateStatus()
        {
            // Update status based on data
            string status = "";

            if (IsValid)
            {
                status += "Voltage measurement is valid.";
            }
            else
            {
                status += "Voltage measurement is invalid.";
            }

            OnPropertyChanged(nameof(Status));
        }

        #endregion // Data

        #region Services

        /// <summary>
        /// Event for notification if property has changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise event to notify about property change
        /// </summary>
        /// <param name="propertyName">name of property</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Sanity check
            if (propertyName == null) throw new ArgumentException("Property name can't be null!");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // Services
    }
}
