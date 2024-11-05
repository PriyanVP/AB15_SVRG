using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds channel specific configuration data
    /// </summary>
    public class FiringChannelConfigurationRecord : INotifyPropertyChanged
    {
        /// <summary>
        /// <inheritdoc cref="ASICID" path='/summary'/>
        /// </summary>
        public int asicID;

        /// <summary>
        /// ASIC ID
        /// </summary>
        public int ASICID 
        { 
            get
            {
                return asicID;
            } 
            set
            {
                asicID = value;
                OnPropertyChanged();
            } 
        }

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
        /// <inheritdoc cref="Mode" path='/summary'/>
        /// </summary>
        private int mode = 0;
        
        /// <summary>
        /// Channel firing mode
        /// </summary>
        public int Mode 
        { 
            get
            {
                return mode;
            } 
            set
            {
                // Verify range
                if ((value < 0) || (value > 9))
                {
                    throw new ArgumentOutOfRangeException($"Unexpected value");
                }
                mode = value;
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
