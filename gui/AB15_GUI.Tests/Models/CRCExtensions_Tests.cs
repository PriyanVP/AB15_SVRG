using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models;

namespace AB15_GUI.Tests.Models
{
    /// <summary>
    /// Checking CRC algorithms. 
    /// CRC8 - Required for communication PC <-> MCU
    /// CRC16 - Required for CRC check on ASIC level
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-84" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-84" />
    ///     <link ID="ABEVBSW-" Link="" /> // TODO: 
    /// </tc_links>
    [TestFixture]
    [Parallelizable(scope: ParallelScope.Self)]
    public class CRCExtensions_Tests
    {
        /// <summary>
        /// Set up test environment
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
        }

        /// <summary>
        /// Clean up environment after finishing tests
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
        }

        [TestCaseSource(nameof(CRC3TestCases)), Description("Checking that CRC3 is calculated expectedly")]
        public void WhenDataIsProvided_ThenCRC8IsCalculatedExpectedly((uint expectedCRC, int startIdx, int endIdx, UInt32 data) testCaseParams)
        {
            // Arrange
            uint calculatedCRC;

            // Act
            calculatedCRC = testCaseParams.data.GetCRC3(testCaseParams.startIdx, testCaseParams.endIdx);

            // Assert
            Assert.That(calculatedCRC, Is.EqualTo(testCaseParams.expectedCRC));
        }

        [TestCaseSource(nameof(CRC8TestCases)), Description("Checking that CRC8 is calculated expectedly")]
        public void WhenDataIsProvided_ThenCRC8IsCalculatedExpectedly((int expectedCRC, int startIdx, List<byte> data) testCaseParams)
        {
            // Arrange
            byte calculatedCRC;
            int length;

            // Act
            length = testCaseParams.data.Count - testCaseParams.startIdx;
            calculatedCRC = testCaseParams.data.GetCRC8(testCaseParams.startIdx, length);

            // Assert
            Assert.That(calculatedCRC, Is.EqualTo(testCaseParams.expectedCRC));
        }

        [TestCaseSource(nameof(CRC16TestCases)), Description("Checking that CRC16 is calculated expectedly")]
        public void WhenDataIsProvided_ThenCRC16IsCalculatedExpectedly((int expectedCRC, int startIdx, List<UInt16> data) testCaseParams)
        {
            // Arrange
            UInt16 calculatedCRC;
            int length;

            // Act
            length = testCaseParams.data.Count - testCaseParams.startIdx;
            calculatedCRC = testCaseParams.data.GetCRC16(testCaseParams.startIdx, length);

            // Assert
            Assert.That(calculatedCRC, Is.EqualTo(testCaseParams.expectedCRC));
        }

        /// <summary>
        /// List of test cases data for CRC3 tests
        /// </summary>
        public static IEnumerable<(uint, int, int, UInt32)> CRC3TestCases()
        {
            yield return (0x4, 5, 31, 0x00400010);
            yield return (0x6, 3, 26, 0x30000C46);
        }

        /// <summary>
        /// List of test cases data for CRC8 tests
        /// </summary>
        public static IEnumerable<(int, int, List<byte>)> CRC8TestCases()
        {
            yield return (0x4F, 0, new List<byte>() {0xBE, 0xEF});
            yield return (0x55, 0, new List<byte>() {0x00});
            yield return (0x55, 3, new List<byte>() {0x01, 0x50, 0xFF, 0x00});
            yield return (0xB4, 0, new List<byte>() {0x05, 0x08, 0x17, 0x55, 0xFF, 0xAE});
            yield return (0xD8, 1, new List<byte>() {0xAB, 0x0, 0x80, 0x6, 0xFE, 0xCD, 0x00, 0x00, 0x34, 0x12});
            yield return (0xA2, 1, new List<byte>() {0xAB, 0x1F, 0x02, 0x04, 0x00, 0x00, 0x00, 0x00});
        }

        /// <summary>
        /// List of test cases data for CRC16 tests
        /// </summary>
        public static IEnumerable<(int, int, List<UInt16>)> CRC16TestCases()
        {
            yield return (0xA113, 0, new List<UInt16>() {0x0000, 0x1234, 0x2345, 0x89FF, 0xABFC, 0x0000});
            yield return (0xE8CB, 0, new List<UInt16>() {0x0110, 0x1224, 0x2335, 0xFFFF, 0x0000});
        }
    }
}
