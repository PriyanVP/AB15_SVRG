using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Data record class that holds all diagnostics data for FLM
    /// </summary>
    public class FiringDiagnosticsData : INotifyPropertyChanged
    {
        private const int NUM_CHANNELS = 20;

        /// <summary>
        /// Constructor for diagnostics model
        /// </summary>
        public FiringDiagnosticsData() // TODO: handling of uncofigured channels missing (only PST?)
        {
            // Populate channel records
            for (int i = 1; i <= NUM_CHANNELS; i++)
            {
                ChannelRecords.Add(new FiringDiagnosticsChannelRecord() { ChannelID = i });
            }

            // Populate voltage records
            string[] voltageNames = { "VH1a", "VH2", "VH3", "VH4", "VH5", "VH6", "VH7", "VH8", "VH9", "VH10", "VH1b" };
            foreach (var voltageName in voltageNames)
            {
                VoltageRecords.Add(new VoltageRecord() { Name = voltageName });
            }
        }

        /// <summary>
        /// <inheritdoc cref="ChannelRecords" path='/summary'/>
        /// </summary>
        private ObservableCollection<FiringDiagnosticsChannelRecord> channelRecords = new ObservableCollection<FiringDiagnosticsChannelRecord>();
        
        /// <summary>
        /// Diagnostics results for specific channel
        /// </summary>
        public ObservableCollection<FiringDiagnosticsChannelRecord> ChannelRecords
        {
            get { return channelRecords; }
            set
            {
                channelRecords = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <inheritdoc cref="VoltageRecords" path='/summary'/>
        /// </summary>
        private ObservableCollection<VoltageRecord> voltageRecords = new ObservableCollection<VoltageRecord>();
        
        /// <summary>
        /// Diagnostics results for voltage measurements
        /// </summary>
        public ObservableCollection<VoltageRecord> VoltageRecords
        {
            get { return voltageRecords; }
            set
            {
                voltageRecords = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Unpack and store data from periodic diagnostics to diagnostics model
        /// </summary>
        /// <param name="diagData">diagnostics data</param>
        public void UnpackPeriodicDiagnostics(FiringDiagnosticsPayload diagData)
        {
            // Unpack short detection results
            for (int i = 0; i < ChannelRecords.Count; i++)
            {
                var shortErrors = diagData.ShortErrors[i];
                ChannelRecords[i].ShortDetectionResult.IGH_S2G = shortErrors.ighS2g;
                ChannelRecords[i].ShortDetectionResult.IGH_S2B = shortErrors.ighS2b;
                ChannelRecords[i].ShortDetectionResult.IGL_S2G = shortErrors.iglS2g;
                ChannelRecords[i].ShortDetectionResult.IGL_S2B = shortErrors.iglS2b;
            }

            // Unpack voltage measurement results
            for (int i = 0; i < VoltageRecords.Count; i++)
            {
                var voltageStatus = diagData.VoltagesStatuses[i];
                VoltageRecords[i].Value   = voltageStatus.voltageValue;
                VoltageRecords[i].IsValid = voltageStatus.isValid;
            }

            // Unpack squib diagnostics results
            for (int i = 0; i < ChannelRecords.Count; i++)
            {
                var squibData = diagData.SquibData[i];
                ChannelRecords[i].SquibResult.DetectionError                 = squibData.detError;
                ChannelRecords[i].SquibResult.ResistanceMeasurementValid     = squibData.resMeasValid;
                ChannelRecords[i].SquibResult.ResistanceMeasurementError     = squibData.resMeasError;
                ChannelRecords[i].SquibResult.ResistanceMeasurementPgndxLoss = squibData.resMeasPgndxLoss;
                ChannelRecords[i].SquibResult.ResistanceMeasurementValue     = squibData.resMeasValue;
            }

            // Update status for each channel record
            foreach (var channelRecord in ChannelRecords)
            {
                channelRecord.UpdateStatus();
            }

            // Update status for each voltage record
            foreach (var voltageRecord in VoltageRecords)
            {
                voltageRecord.UpdateStatus();
            }
        }
        
        /// <summary>
        /// Unpack and store data from powerstage diagnostics to diagnostics model
        /// </summary>
        /// <param name="isLowsideTest">true if TestMode1, false - TestMode2</param>
        /// <param name="diagData">powerstage test data</param>
        /// <param name="configuredChannels">list with indexes of configured channels</param>
        public void UnpackPowerstageDiagnostics(bool isLowsideTest, TestModePayload diagData, List<int> configuredChannels)
        {
            // Unpack data
            for (int i = 0; i < diagData.Data.Count; i++)
            {
                // Update ignore flags based on configured channels
                ChannelRecords[i].PstResult.IgnoreResults = !configuredChannels.Contains(i + 1); // Channel ID is 1-based

                if (isLowsideTest)
                {
                    ChannelRecords[i].PstResult.Lowside  = diagData.Data[i];
                }
                else
                {
                    ChannelRecords[i].PstResult.Highside = diagData.Data[i];
                }
            }

            // Update status for each channel record
            foreach (var channelRecord in ChannelRecords)
            {
                channelRecord.UpdateStatus();
            }
        }

        /// <summary>
        /// Unpack and store data from cross coupling diagnostics to diagnostics model
        /// </summary>
        /// <param name="diagData">cross coupling data (master)</param>
        public void UnpackCrossCouplingData(uint diagData)
        {
            // Unpack data
            for (int i = 0; i < ChannelRecords.Count; i++)
            {
                ChannelRecords[i].CouplingS2XError = (diagData & (1 << i)) != 0;
            }

            // Update status for each channel record
            foreach (var channelRecord in ChannelRecords)
            {
                channelRecord.UpdateStatus();
            }
        }

        /// <summary>
        /// Unpack and store data from coupling detect diagnostics to diagnostics model
        /// </summary>
        /// <param name="diagData">coupling detect data (slave)</param>
        public void UnpackCouplingDetectData(uint diagData)
        {
            // Unpack data
            for (int i = 0; i < ChannelRecords.Count; i++)
            {
                ChannelRecords[i].CouplingDetectError = (diagData & (1 << i)) != 0;
            }

            // Update status for each channel record
            foreach (var channelRecord in ChannelRecords)
            {
                channelRecord.UpdateStatus();
            }
        }

        /// <summary>
        /// Unpack and store data from LEA diode polarity diagnostics to diagnostics model
        /// </summary>
        /// <param name="diagData">LEA diode error data</param>
        public void UnpackLeaDiodeData(uint diagData)
        {
            // Unpack data
            for (int i = 0; i < ChannelRecords.Count; i++)
            {
                ChannelRecords[i].LeaDiodePolarityError = (diagData & (1 << i)) != 0;
            }

            // Update status for each channel record
            foreach (var channelRecord in ChannelRecords)
            {
                channelRecord.UpdateStatus();
            }
        }

        /// <summary>
        /// Unpack and store data from capacity test diagnostics to diagnostics model
        /// </summary>
        /// <param name="diagData">capacity test data</param>
        public void UnpackCapacityTestData(uint diagData)
        {
            // Unpack data
            for (int i = 0; i < ChannelRecords.Count; i++)
            {
                ChannelRecords[i].CapacityTestError = (diagData & (1 << i)) != 0;
            }

            // Update status for each channel record
            foreach (var channelRecord in ChannelRecords)
            {
                channelRecord.UpdateStatus();
            }
        }

        /// <summary>
        /// Unpack and store data from overcurrent diagnostics to diagnostics model
        /// </summary>
        /// <param name="diagData">overcurrent data</param>
        public void UnpackOvercurrentData(uint diagData)
        {
            // Unpack data
            for (int i = 0; i < ChannelRecords.Count; i++)
            {
                ChannelRecords[i].OvercurrentError = (diagData & (1 << i)) != 0;
            }

            // Update status for each channel record
            foreach (var channelRecord in ChannelRecords)
            {
                channelRecord.UpdateStatus();
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
