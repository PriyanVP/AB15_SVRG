using System;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// PST class that includes lowside and highside test results and diagnostic status
    /// </summary>
    public class PstChannelTestResult
    {
        /// <summary>
        /// <inheritdoc cref="Lowside" path='/summary'/>
        /// </summary>
        private PstChannelSideTestResult lowside;

        /// <summary>
        /// <inheritdoc cref="Highside" path='/summary'/>
        /// </summary>
        private PstChannelSideTestResult highside;

        /// <summary>
        /// <inheritdoc cref="DiagStatus" path='/summary'/>
        /// </summary>
        private string diagStatus;

        /// <summary>
        /// Flag to indicate if results should be ignored
        /// </summary>
        public bool IgnoreResults { get; set; } = false;

        /// <summary>
        /// Lowside test results
        /// </summary>
        public PstChannelSideTestResult Lowside
        {
            get => lowside;
            set
            {
                if (!lowside.Equals(value))
                {
                    lowside = value;
                    UpdateDiagStatus();
                }
            }
        }

        /// <summary>
        /// Highside test results
        /// </summary>
        public PstChannelSideTestResult Highside
        {
            get => highside;
            set
            {
                if (!highside.Equals(value))
                {
                    highside = value;
                    UpdateDiagStatus();
                }
            }
        }

        /// <summary>
        /// Diagnostic summary
        /// </summary>
        public string DiagStatus
        {
            get
            {
                return diagStatus;
            }
            private set 
            {
                diagStatus = value;
            }
        }

        /// <summary>
        /// Updates the diagnostic status based on lowside and highside test results
        /// </summary>
        private void UpdateDiagStatus()
        {
            // Update diagStatus based on lowside and highside
            DiagStatus = "";
            if (Lowside.Error != null)
            {
                DiagStatus += $"Lowside {lowside.Error}{Environment.NewLine}";
            }
            
            if (Highside.Error != null)
            {
                DiagStatus += $"Highside {highside.Error}{Environment.NewLine}";
            }

            if (Lowside.Error == null && Highside.Error == null)
            {
                DiagStatus = $"PST OK{Environment.NewLine}";
            }

            // Special option to handle cases where results should be ignored
            if (IgnoreResults)
            {
                DiagStatus = "PST skipped";
            }
        }
    }
}