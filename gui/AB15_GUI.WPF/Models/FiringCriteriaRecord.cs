using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds ASIC specific statuses for firing monitoring
    /// </summary>
    public class FiringCriteriaRecord : INotifyPropertyChanged
    {
        /// <summary>
        /// <inheritdoc cref="Criteria" path='/summary'/>
        /// </summary>
        private string criteria = "";
        
        /// <summary>
        /// Criteria for displaying in UI
        /// </summary>
        public string Criteria 
        { 
            get
            {
                return criteria;
            }
            set
            {
                criteria = value;
                OnPropertyChanged();
            }
        }

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
            set
            {
                // Do not update property if value is same as previously
                if (status == value) return;

                status = value;
                OnPropertyChanged();
            }
        }
        
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
