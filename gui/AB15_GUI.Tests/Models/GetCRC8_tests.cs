using System.Collections.Generic;
using AB15_GUI.WPF.Models;

namespace AB15_GUI.Tests.Models
{
    /// <summary>
    /// Checking CRC8 algorithm. Required for communication PC <-> MCU
    /// </summary>
    /// <tc_links>
    ///     <link ID="" Link="" />
    /// </tc_links>
    [TestFixture]
    [Parallelizable(scope: ParallelScope.Self)]
    public class GetCRC8_tests
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

        [TestCaseSource(nameof(CRC8TestCases)), Description("Checking that CRC is calculated expetedly")]
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
    }
}
