using System;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds channel specific firing data
    /// </summary>
    public class FiringResultRecord
    {
        /// <summary>
        /// ASIC ID
        /// </summary>
        public int ASICID { get; set; }
        
        /// <summary>
        /// Channel ID
        /// </summary>
        public int ChannelID { get; set; }
        
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
            }
        }
        
        /// <summary>
        /// Flag indicating if channel is selected for firing
        /// </summary>
        public bool ToFire { get; set; }
        
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
                }

                firingCntHigh = value;
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
                }

                firingCntLow = value;
            } 
        }
    }
}
