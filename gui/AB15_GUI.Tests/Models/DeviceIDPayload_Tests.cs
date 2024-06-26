using System.Collections.Generic;
using AB15_GUI.WPF.Models;

namespace AB15_GUI.Tests.Models
{
    /// <summary>
    /// Checking DeviceID Payload
    /// </summary>
    /// <tc_links>
    ///     <link ID="" Link="" />
    /// </tc_links>
    [TestFixture]
    [Parallelizable(scope: ParallelScope.Self)]
    public class DeviceIDPayload_Tests
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

        [TestCaseSource(nameof(ValidTestCases)), Description("Checking that valid packages are unpacked correctly")]
        public void WhenSuccessfulResponce_ThenDataUnpackedSuccessfully((MCUStatus status, List<byte> inPayload) tcParams)
        {
            // Arrange
            string? expectedDeviceId;
            int deviceID;
            DeviceIDPayload payload = new DeviceIDPayload();

            // Act
            payload.Deserialize(tcParams.status, tcParams.inPayload);
            deviceID = (int) tcParams.inPayload[0];
            expectedDeviceId = (deviceID == 0xC4) ? ("CG904") :
                               (deviceID == 0xC3) ? ("CG903") :
                               (deviceID == 0xC2) ? ("CG902") : 
                                                    (null);

            // Assert
            // No errors were found
            Assert.That(payload.Error, Is.Null);

            // Device ID was found correctly
            Assert.That(payload.DeviceID, Is.EqualTo(expectedDeviceId));
        }

        
        [TestCaseSource(nameof(InvalidTestCases)), Description("Checking that invalid packages are unpacked correctly")]
        public void WhenErrorResponce_ThenDataErrorIsSet((MCUStatus status, List<byte> inPayload) tcParams)
        {
            // Arrange
            DeviceIDPayload payload = new DeviceIDPayload();

            // Act
            payload.Deserialize(tcParams.status, tcParams.inPayload);

            // Assert
            // No errors were found
            Assert.That(payload.Error, Is.Not.Null);
        }

        /// <summary>
        /// List of valid test cases data
        /// </summary>
        public static IEnumerable<(MCUStatus, List<byte>)> ValidTestCases()
        {
            yield return (MCUStatus.DATA, new List<byte>() {0xC4});
            yield return (MCUStatus.DATA, new List<byte>() {0xC3});
            yield return (MCUStatus.DATA, new List<byte>() {0xC2});
            yield return (MCUStatus.DATA, new List<byte>() {0x00});
        }

        /// <summary>
        /// List of valid test cases data
        /// </summary>
        public static IEnumerable<(MCUStatus, List<byte>)> InvalidTestCases()
        {
            yield return (MCUStatus.ERROR, new List<byte>() {0xC4, 0xC2});
            yield return (MCUStatus.ERROR, new List<byte>() {});
            yield return (MCUStatus.RESPONSE_ABSENT, new List<byte>() {});
        }
    }
}
