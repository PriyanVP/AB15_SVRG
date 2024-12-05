using System.Collections.Generic;
using AB15_GUI.WPF.Models;
using static AB15_GUI.WPF.Models.WDStatusPayload;

namespace AB15_GUI.Tests.Models
{
    /// <summary>
    /// Checking WDStatusPayload class
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-132" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-132" />
    /// </tc_links>
    [TestFixture]
    [Parallelizable(scope: ParallelScope.Self)]
    public class WDStatusPayload_Tests
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

        //[TestCaseSource(nameof(ValidTestCases)), Description("Checking that valid packages are unpacked correctly")]
        //public void WhenSuccessfulResponse_ThenDataUnpackedSuccessfully((MCUStatus status, List<byte> inPayload) tcParams)
        //{
        //    // Arrange
        //    WDStatusPayload payload   = new WDStatusPayload();
        //    WDStatus expectedWDStatus = new WDStatus();

        //    // Act
        //    payload.Deserialize(tcParams.status, tcParams.inPayload);

        //    // TODO decode some bytes by hand
        //    //byte wdsMSB = tcParams.inPayload[0];
        //    //byte wdsLSB = tcParams.inPayload[1];
        //    bool wdf    = ( tcParams.inPayload[2] != 0 );

        //    // Test data is either "no WD faults active at all" or "all WD faults active"
        //    // Set all WD fault flags to be identical to the WDF flag in the third byte
        //    expectedWDStatus.WatchdogFault = wdf;

        //    // expectedWDStatus.FastWatchdogFault = wdf;
        //    // expectedWDStatus.SlowWatchdogFault = wdf;
        //    // expectedWDStatus.OscillatorFault = wdf;

        //    // expectedWDStatus.FastWatchdogUnderflow = wdf;
        //    // expectedWDStatus.FastWatchdogOverflow = wdf;
        //    // expectedWDStatus.FastWatchdogQAFault = wdf;
        //    // expectedWDStatus.SlowWatchdogOverflow = wdf;
        //    // expectedWDStatus.SlowWatchdogQAFault = wdf;
        //    // expectedWDStatus.OscillatorUnderflow = wdf;
        //    // expectedWDStatus.OscillatorOverflow = wdf;

        //    // Assert
        //    // No errors were found
        //    Assert.That(payload.Error, Is.Null);

        //    Assert.That(payload.WatchdogStatus.WatchdogFault, Is.EqualTo(expectedWDStatus.WatchdogFault));
        //}

        
        [TestCaseSource(nameof(InvalidTestCases)), Description("Checking that invalid packages are unpacked correctly")]
        public void WhenErrorResponse_ThenDataErrorIsSet((MCUStatus status, List<byte> inPayload) tcParams)
        {
            // Arrange
            WDStatusPayload payload = new WDStatusPayload();

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
            yield return (MCUStatus.DATA, new List<byte>() {0x00,0x00,0x00}); // no  wd faults active
            yield return (MCUStatus.DATA, new List<byte>() {0x1C,0x7F,0x01}); // all wd faults active
        }

        /// <summary>
        /// List of valid test cases data
        /// </summary>
        public static IEnumerable<(MCUStatus, List<byte>)> InvalidTestCases()
        {
            yield return (MCUStatus.ERROR, new List<byte>() {0xC4, 0xC2}); // too short (2 bytes)
            yield return (MCUStatus.ERROR, new List<byte>() {});           // no payload
            yield return (MCUStatus.RESPONSE_ABSENT, new List<byte>() {}); // no response
        }
    }
}
