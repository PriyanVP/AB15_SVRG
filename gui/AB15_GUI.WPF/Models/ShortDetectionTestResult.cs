using System;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Short detection results
    /// </summary>
    public class ShortDetectionTestResult
    {
        /// <summary>
        /// <inheritdoc cref="IGH_S2G" path='/summary'/>
        /// </summary>
        private bool igh_s2g;

        /// <summary>
        /// <inheritdoc cref="IGH_S2B" path='/summary'/>
        /// </summary>
        private bool igh_s2b;

        /// <summary>
        /// <inheritdoc cref="IGL_S2G" path='/summary'/>
        /// </summary>
        private bool igl_s2g;

        /// <summary>
        /// <inheritdoc cref="IGL_S2B" path='/summary'/>
        /// </summary>
        private bool igl_s2b;

        /// <summary>
        /// <inheritdoc cref="DiagStatus" path='/summary'/>
        /// </summary>
        private string diagStatus;

        /// <summary>
        /// IGH short to ground result
        /// </summary>
        public bool IGH_S2G
        {
            get => igh_s2g;
            set
            {
                if (!igh_s2g.Equals(value))
                {
                    igh_s2g = value;
                    UpdateDiagStatus();
                }
            }
        }

        /// <summary>
        /// IGH short to battery result
        /// </summary>
        public bool IGH_S2B
        {
            get => igh_s2b;
            set
            {
                if (!igh_s2b.Equals(value))
                {
                    igh_s2b = value;
                    UpdateDiagStatus();
                }
            }
        }

        /// <summary>
        /// IGL short to ground result
        /// </summary>
        public bool IGL_S2G
        {
            get => igl_s2g;
            set
            {
                if (!igl_s2g.Equals(value))
                {
                    igl_s2g = value;
                    UpdateDiagStatus();
                }
            }
        }

        /// <summary>
        /// IGL short to battery result
        /// </summary>
        public bool IGL_S2B
        {
            get => igl_s2b;
            set
            {
                if (!igl_s2b.Equals(value))
                {
                    igl_s2b = value;
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
        /// Updates the diagnostic status based on test results
        /// </summary>
        private void UpdateDiagStatus()
        {
            // Update diagStatus based on lowside and highside
            DiagStatus = "";

            if (IGH_S2G)
            {
                DiagStatus += $"IGH short to ground{Environment.NewLine}";
            }

            if (IGH_S2B)
            {
                DiagStatus += $"IGH short to battery{Environment.NewLine}";
            }

            if (IGL_S2G)
            {
                DiagStatus += $"IGL short to ground{Environment.NewLine}";
            }

            if (IGL_S2B)
            {
                DiagStatus += $"IGL short to battery{Environment.NewLine}";
            }

            if (IGH_S2G || IGH_S2B || IGL_S2G || IGL_S2B == false)
            {
                DiagStatus = $"No short circuits detected{Environment.NewLine}";
            }
        }
    }
}