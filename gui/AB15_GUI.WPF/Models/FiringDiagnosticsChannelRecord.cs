using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    #region ObservableMembers

    /// <summary>
    /// Data record class that holds diagnostics data for single channel in FLM
    /// </summary>
    public class FiringDiagnosticsChannelRecord : INotifyPropertyChanged
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
        }

        #endregion // ObservableMembers

        #region Data

        /// <summary>
        /// Powerstage test result
        /// </summary>
        public PstChannelTestResult PstResult { get; set; } = new PstChannelTestResult();

        /// <summary>
        /// Short detection test results
        /// </summary>
        public ShortDetectionTestResult ShortDetectionResult { get; set; } = new ShortDetectionTestResult();

        /// <summary>
        /// Squib measurements
        /// </summary>
        public SquibTestResult SquibResult { get; set; } = new SquibTestResult();

        /// <summary>
        /// Lea diode polarity error
        /// </summary>
        public bool LeaDiodePolarityError { get; set; } = false;

        /// <summary>
        /// Capacity test error
        /// </summary>
        public bool CapacityTestError { get; set; } = false;

        /// <summary>
        /// Coupling S2X error
        /// </summary>
        public bool CouplingS2XError { get; set; } = false;

        /// <summary>
        /// Coupling detect error
        /// </summary>
        public bool CouplingDetectError { get; set; } = false;

        /// <summary>
        /// Overcurrent error
        /// </summary>
        public bool OvercurrentError { get; set; } = false;

        /// <summary>
        /// Method to update observable status based on data
        /// </summary>
        public void UpdateStatus()
        {
            // Update status based on data
            string status = "";

            status += PstResult.DiagStatus;
            status += ShortDetectionResult.DiagStatus;
            status += SquibResult.DiagStatus;

            if (LeaDiodePolarityError) 
            {
                status += $"Lea Diode Polarity Error{Environment.NewLine}";
            }
            else
            {
                status += $"Lea Diode Polarity OK{Environment.NewLine}";
            }

            if (CapacityTestError)
            {
                status += $"Capacity Test Error{Environment.NewLine}";
            }
            else
            {
                status += $"Capacity Test OK{Environment.NewLine}";
            }

            if (CouplingS2XError)
            {
                status += $"Coupling S2X Error{Environment.NewLine}";
            }
            else
            {
                status += $"Coupling S2X OK{Environment.NewLine}";
            }

            if (CouplingDetectError)
            {
                status += $"Coupling Detect Error{Environment.NewLine}";
            }
            else
            {
                status += $"Coupling Detect OK{Environment.NewLine}";
            }

            if (OvercurrentError)
            {
                status += $"Overcurrent Error{Environment.NewLine}";
            }
            else
            {
                status += $"Overcurrent OK{Environment.NewLine}";
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
