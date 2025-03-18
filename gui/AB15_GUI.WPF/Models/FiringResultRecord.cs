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
        private bool toFire = false;
        
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
        // TODO firecnt_translation - Changed strategy from counting number of sides deployed - Cleanup 
        // private int numberOfSidesDeployed = 0;
        private bool hsWasFired = false;
        private bool lsWasFired = false;

        /// <summary>
        /// Flag indicating if channel was fired
        /// Note: has to be cleared before firing attempt, any write request will clear
        /// </summary>
        public bool WasFired 
        { 
            get
            {
                // Report successful firing only if both sides were deployed
                return (hsWasFired && lsWasFired);
            } 
            set
            {
                // Any input value will cause counter reset
                // TODO firecnt_translation - Changed strategy from counting number of sides deployed - Cleanup 
                // numberOfSidesDeployed = 0;
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
                // TODO firecnt_translation - Changed strategy from counting number of sides deployed - Cleanup 
                // Increment counter if firing occurred
                if (value > 0)
                {
                    // TODO firecnt_translation - Changed strategy from counting number of sides deployed - Cleanup 
                    //numberOfSidesDeployed++;
                    hsWasFired = true;
                    OnPropertyChanged(nameof(WasFired));
                }
                else
                {
                    hsWasFired = false;
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
                // TODO firecnt_translation - Changed strategy from counting number of sides deployed - Cleanup 
                // Increment counter if firing occurred
                if (value > 0)
                {
                    // TODO firecnt_translation - Changed strategy from counting number of sides deployed - Cleanup 
                    //numberOfSidesDeployed++;
                    lsWasFired = true;
                    OnPropertyChanged(nameof(WasFired));
                }
                else
                {
                    lsWasFired = false;
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
