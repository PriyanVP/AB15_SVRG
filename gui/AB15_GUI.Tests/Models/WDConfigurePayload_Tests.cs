using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.Tests.Models
{
    /// <summary>
    /// Checking WDConfigurePayload class
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-132" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-132" />
    /// </tc_links>
    [TestFixture]
    [Parallelizable(scope: ParallelScope.Self)]
    public class WDConfigurePayload_Tests
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

//        [TestCaseSource(nameof(ValidTestCases)), Description("Checking that valid data is packed correctly")]
//        public void WhenDataIsValid_ThenDataIsPackedCorrectly(List<byte> expectedPayload)
//        {
//            // Arrange
//            WDConfigurePayload tstPayload = new();
//            WDConfiguration tstConfiguration = new();
//            // dummy data - current implementation will serialize to six bytes of zeros
//            // expectedPayload = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
//            // dummy data - current implementation will serialize to six bytes of zeros
//            tstConfiguration.FastWatchdogLockTime = 0;
//            tstConfiguration.SlowWatchdogLockTime = 0;
//            tstConfiguration.FastWatchdogResponseTime = 0;
//            tstConfiguration.SlowWatchdogResponseTime = 0;
//            tstConfiguration.FastWatchdogEn0DisThreshold = 0;
//            tstConfiguration.SlowWatchdogEn0DisThreshold = 0;
            
//            tstPayload.WatchdogConfiguration = tstConfiguration;

//            // Act
//            List<byte> constructedPayload = tstPayload.Serialize();

//            // Assert
//            // Payload content is expected
//            Assert.That(constructedPayload, Is.EqualTo(expectedPayload));

//        }

//        [TestCaseSource(nameof(InValidTestCases)), Description("Checking that invalid data is not packed and Config is null")]
//        public void WhenDataIsNotValid_ThenDataHandledExpectedly(List<byte> expectedPayload)
//        {
//            // Arrange
//            WDConfigurePayload tstPayload = new();

//            // Act
// //            List<byte> constructedPayload = tstPayload.Serialize();

//            // Assert
//            // Payload content is expected
//            Assert.That(tstPayload.WatchdogConfiguration, Is.Null);

//        }

        /// <summary>
        /// List of test cases data for valid TransmitCommunicationPackage scenarios
        /// </summary>
        public static IEnumerable<List<byte>> ValidTestCases()
        {
            yield return new List<byte>() { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        }

        /// <summary>
        /// List of test cases data for invalid TransmitCommunicationPackage scenarios
        /// </summary>
        public static IEnumerable<List<byte>> InValidTestCases() 
        {
            yield return new List<byte>() { 0x00 };
        }

    }
}
