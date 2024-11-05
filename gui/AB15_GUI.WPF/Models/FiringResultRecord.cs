using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds channel specific firing data
    /// </summary>
    public class FiringResultRecord : INotifyPropertyChanged
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
        /// <inheritdoc cref="ToFire" path='/summary'/>
        /// </summary>
        private bool toFire;
        
        /// <summary>
        /// Flag indicating if channel is selected for firing
        /// </summary>
        public bool ToFire 
        { 
            get
            {
                return toFire;
            }
            set
            {
                toFire = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Indicate how many sides of channel were deployed (2 corresponds to firing)
        /// </summary>
        private int numberOfSidesDeployed = 0;

        /// <summary>
        /// Flag indicating if channel was fired
        /// Note: has to be cleared before firing attempt, any write request will clear
        /// </summary>
        public bool WasFired 
        { 
            get
            {
                // Report successful firing only if both sides were deployed
                return (numberOfSidesDeployed == 2);
            } 
            set
            {
                // Any input value will cause counter reset
                numberOfSidesDeployed = 0;
                OnPropertyChanged();
            } 
        }
      
        /// <summary>
        /// <inheritdoc cref="FiringCntHigh" path='/summary'/>
        /// </summary>
        private int firingCntHigh = 0;

        /// <summary>
        /// Highside firing counter
        /// </summary>
        public int FiringCntHigh 
        { 
            get
            {
                return firingCntHigh;
            } 
            set
            {
                // Increment counter if firing occurred
                if (firingCntHigh != value)
                {
                    numberOfSidesDeployed++;
                    OnPropertyChanged(nameof(WasFired));
                }

                firingCntHigh = value;
                OnPropertyChanged();
            } 
        }

        /// <summary>
        /// <inheritdoc cref="FiringCntLow" path='/summary'/>
        /// </summary>
        private int firingCntLow = 0;

        /// <summary>
        /// Lowside firing counter
        /// </summary>
        public int FiringCntLow 
        { 
            get
            {
                return firingCntLow;
            } 
            set
            {
                // Increment counter if firing occurred
                if (firingCntHigh != value)
                {
                    numberOfSidesDeployed++;
                    OnPropertyChanged(nameof(WasFired));
                }

                firingCntLow = value;
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
