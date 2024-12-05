using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds channel specific firing error data
    /// </summary>
    public class FiringChannelErrorRecord : INotifyPropertyChanged
    {
        /// <summary>
        /// <inheritdoc cref="ChannelID" path='/summary'/>
        /// </summary>
        public int channelID;
        
        /// <summary>
        /// Channel ID
        /// </summary>
        public int ChannelID 
        { 
            get
            {
                return channelID;
            } 
            set
            {
                channelID = value;
                OnPropertyChanged();
            } 
        }
        
        /// <summary>
        /// <inheritdoc cref="Identifier" path='/summary'/>
        /// </summary>
        private string identifier = "None";
        
        /// <summary>
        /// Custom channel identifier
        /// Note: identifier is not guaranteed to be unique
        /// </summary>
        public string Identifier
        { 
            get
            {
                return identifier;
            }
            set
            {
                // Length limitation
                if (value.Length > 15) return;
                identifier = value;
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
