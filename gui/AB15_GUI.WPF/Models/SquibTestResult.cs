using System;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Squib resistance results
    /// </summary>
    public class SquibTestResult
    {
        /// <summary>
        /// <inheritdoc cref="DetectionError" path='/summary'/>
        /// </summary>
        private bool detectionError;

        /// <summary>
        /// <inheritdoc cref="ResistanceMeasurementValid" path='/summary'/>
        /// </summary>
        private bool resistanceMeasurementValid;

        /// <summary>
        /// <inheritdoc cref="RESISTAnceMeasurementError" path='/summary'/>
        /// </summary>
        private bool resistanceMeasurementError;

        /// <summary>
        /// <inheritdoc cref="ResistanceMeasurementPgndxLoss" path='/summary'/>
        /// </summary>
        private bool resistanceMeasurementPgndxLoss;

        /// <summary>
        /// <inheritdoc cref="ResistanceMeasurementValue" path='/summary'/>
        /// </summary>
        private uint resistanceMeasurementValue;

        /// <summary>
        /// <inheritdoc cref="DiagStatus" path='/summary'/>
        /// </summary>
        private string diagStatus;

        /// <summary>
        /// Squib detection error
        /// </summary>
        public bool DetectionError
        {
            get => detectionError;
            set
            {
                if (!detectionError.Equals(value))
                {
                    detectionError = value;
                    UpdateDiagStatus();
                }
            }
        }

        /// <summary>
        /// Squib resistance measurement valid flag
        /// </summary>
        public bool ResistanceMeasurementValid
        {
            get => resistanceMeasurementValid;
            set
            {
                if (!resistanceMeasurementValid.Equals(value))
                {
                    resistanceMeasurementValid = value;
                    UpdateDiagStatus();
                }
            }
        }

        /// <summary>
        /// Squib resistance measurement error flag
        /// </summary>
        public bool ResistanceMeasurementError
        {
            get => resistanceMeasurementError;
            set
            {
                if (!resistanceMeasurementError.Equals(value))
                {
                    resistanceMeasurementError = value;
                    UpdateDiagStatus();
                }
            }
        }

        /// <summary>
        /// Squib resistance measurement PGNDX loss flag
        /// </summary>
        public bool ResistanceMeasurementPgndxLoss
        {
            get => resistanceMeasurementPgndxLoss;
            set
            {
                if (!resistanceMeasurementPgndxLoss.Equals(value))
                {
                    resistanceMeasurementPgndxLoss = value;
                    UpdateDiagStatus();
                }
            }
        }

        /// <summary>
        /// Squib resistance measurement value
        /// </summary>
        public uint ResistanceMeasurementValue
        {
            get => resistanceMeasurementValue;
            set
            {
                if (!resistanceMeasurementValue.Equals(value))
                {
                    resistanceMeasurementValue = value;
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

            if (DetectionError)
            {
                DiagStatus += $"SQUIB detection error{Environment.NewLine}";
            }

            if (ResistanceMeasurementValid == false)
            {
                DiagStatus += $"SQUIB resistance measurement is not valid{Environment.NewLine}";
            }

            if (ResistanceMeasurementError)
            {
                DiagStatus += $"Error during SQUIB resistance measurement{Environment.NewLine}";
            }

            if (ResistanceMeasurementPgndxLoss)
            {
                DiagStatus += $"Lowside pgndx downbond loss during SQUIB resistance measurement{Environment.NewLine}";
            }

            if (DetectionError || ResistanceMeasurementValid || ResistanceMeasurementError || ResistanceMeasurementPgndxLoss == false)
            {
                DiagStatus = $"No short circuits detected{Environment.NewLine}";
            }
        }
    }
}