using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class that is used for unpacking FLM diagnostics data
    /// </summary>
    public class FiringDiagnosticsPayload : IByteListSerializable
    {
        /// <summary>
        /// Property to report errors
        /// </summary>
        public string? Error { get; set; } = null;

        /// <summary>
        /// Short detection results. Each item corresponds to channel
        /// </summary>
        /// <param name="ighS2g">highside short to ground</param>
        /// <param name="ighS2b">highside short to battery</param>
        /// <param name="iglS2g">lowside short to ground</param>
        /// <param name="iglS2b">lowside short to battery</param>
        public List<(bool ighS2g, bool ighS2b, bool iglS2g, bool iglS2b)> ShortErrors { get; set; } = new List<(bool ighS2g, bool ighS2b, bool iglS2g, bool iglS2b)>();
       
        /// <summary>
        /// Voltage diagnostics results
        /// </summary>
        /// <param name="voltageValue"></param>
        /// <param name="isValid"></param>
        public List<(ushort voltageValue, bool isValid)> VoltagesStatuses { get; set; } = new List<(ushort value, bool isValid)>();

        /// <summary>
        /// Squib diagnostics results
        /// </summary>
        /// <param name="detError">squib detection error</param>
        /// <param name="resMeasValid">squib resistance measurement valid flag</param>
        /// <param name="resMeasError">squib resistance measurement error flag</param>
        /// <param name="resMeasPgndxLoss">squib resistance measurement PGNDX loss flag</param>
        /// <param name="resMeasValue">squib resistance measurement value</param>
        public List<(bool detError, bool resMeasValid, bool resMeasError, bool resMeasPgndxLoss, uint resMeasValue)> SquibData { get; set; } = new List<(bool detError, bool resMeasValid, bool resMeasError, bool resMeasPgndxLoss, uint resMeasValue)>();

        /// <summary>
        /// Convert byte list to field values
        /// </summary>
        /// <param name="status">status field of package</param>
        /// <param name="rawData">Data that should be converted to parameters values (payload only)</param>
        public void Deserialize(MCUStatus status, List<byte> rawData)
        {
            // Apply different handling based on status
            switch (status)
            {
                case MCUStatus.ERROR:
                    // Error handling -> store data to property
                    Error = $"Message with error status received: Payload {string.Join(" ", rawData)}";
                    break;
                case MCUStatus.RESPONSE_ABSENT:
                    // Error handling -> store data to property
                    Error = $"Message with response from MCU wasn't received in expected timeframe.";
                    break;
                case MCUStatus.DATA:
                    // Data was received
                    // Check if data in payload is expected
                    if (rawData.Count != 86)
                    {
                        Error = $"Received package with unexpected number of bytes in payload. Expected 86, but got {rawData.Count}";
                        break;
                    }

                    // Unpack first 10 bytes into ShortErrors
                    for (int i = 0; i < 10; i += 2)
                    {
                        uint value = BitOperationsExtensions.ConstructWordFromBytes(rawData[i], rawData[i + 1]);

                        for (int j = 0; j < 4; j++)
                        {
                            value >>= j*4;
                            ShortErrors.Add((
                                (value & 0x0001) != 0,
                                (value & 0x0002) != 0,
                                (value & 0x0004) != 0,
                                (value & 0x0008) != 0
                            ));
                        }
                    }

                    // Unpack voltage measurement data
                    uint isValidFlags = BitOperationsExtensions.ConstructWordFromBytes(rawData[32], rawData[33]);
                    for (int i = 10; i < 32; i += 2)
                    {
                        int bitPosition = (i - 10) / 2;
                        bool isValid = (isValidFlags & (1 << bitPosition)) != 0;
                        VoltagesStatuses.Add((
                            BitOperationsExtensions.ConstructWordFromBytes(rawData[i], rawData[i+1]),
                            isValid
                        ));
                    }

                    // Unpack squib related statuses
                    uint detErrorFlags          = BitOperationsExtensions.ConstructWordFromBytes(rawData[34], rawData[35], rawData[36]);
                    uint resMeasErrorFlags      = BitOperationsExtensions.ConstructWordFromBytes(rawData[77], rawData[78], rawData[79]);
                    uint resMeasValidFlags      = BitOperationsExtensions.ConstructWordFromBytes(rawData[80], rawData[81], rawData[82]);
                    uint resMeasPgndxLossFlags  = BitOperationsExtensions.ConstructWordFromBytes(rawData[83], rawData[84], rawData[85]);

                    for (int i = 37; i < 77; i += 2)
                    {
                        int bitPosition = (i - 37) / 2;
                        SquibData.Add((
                            (detErrorFlags & (1 << bitPosition)) != 0,
                            (resMeasValidFlags & (1 << bitPosition)) != 0,
                            (resMeasErrorFlags & (1 << bitPosition)) != 0,
                            (resMeasPgndxLossFlags & (1 << bitPosition)) != 0,
                            BitOperationsExtensions.ConstructWordFromBytes(rawData[i], rawData[i+1])
                        ));
                    }

                    break;
                default:
                    throw new ArgumentException($"Unexpected status received: {status}");
            }
        }

        /// <summary>
        /// Converts payload data to byte list
        /// </summary>
        /// <returns>Empty list</returns>
        public List<byte> Serialize()
        {
            return new List<byte>();
        }
    }
}
