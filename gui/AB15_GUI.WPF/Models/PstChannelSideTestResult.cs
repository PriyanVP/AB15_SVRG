using System;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Struct that is used for unpacking Test Mode channel data for one side
    /// </summary>
    public struct PstChannelSideTestResult
    {
        public bool PstNotValid { get; private set; }
        public bool PstPretestS2xErr { get; private set; }
        public bool PstTestS2xErr { get; private set; }
        public bool PstTimeoutErr { get; private set; }
        public bool TestGuardFail { get; private set; }
        public bool SpiOnChFail { get; private set; }

        public string? Error { get; private set; } = null;

        public PstChannelSideTestResult(byte rawData)
        {
            PstNotValid = (rawData & 0x01) != 0;
            PstPretestS2xErr = (rawData & 0x02) != 0;
            PstTestS2xErr = (rawData & 0x04) != 0;
            PstTimeoutErr = (rawData & 0x08) != 0;
            TestGuardFail = (rawData & 0x10) != 0;
            SpiOnChFail = (rawData & 0x20) != 0;

            // Generate error message
            Error = "";
            if (PstNotValid)
            {
                Error += $"PST not valid{Environment.NewLine}";
            }
            
            if (PstPretestS2xErr)
            {
                Error += $"PST pretest S2X error{Environment.NewLine}";
            }
            
            if (PstTestS2xErr)
            {
                Error += $"PST test S2X error{Environment.NewLine}";
            }
            
            if (PstTimeoutErr)
            {
                Error += $"PST timeout error{Environment.NewLine}";
            }
            
            if (TestGuardFail)
            {
                Error += $"Test guard fail{Environment.NewLine}";
            }
            
            if (SpiOnChFail)
            {
                Error += $"SPI on channel fail{Environment.NewLine}";
            }

            if (Error == "")
            {
                Error = null;
            }
        }
    }
}
