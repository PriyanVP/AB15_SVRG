using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.Tests.Models
{
    /// <summary>
    /// Checking TransmitCommunicationPackage class
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-84" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-84" />
    /// </tc_links>
    [TestFixture]
    [Parallelizable(scope: ParallelScope.Self)]
    public class TransmitCommunicationPackage_Tests
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

        [TestCaseSource(nameof(ValidTestCases)), Description("Checking that valid data is packed correctly")]
        public void WhenDataIsValid_ThenDataIsPackedCorrectly(List<byte> expectedPackage)
        {
            // Arrange
            TransmitCommunicationPackage<ByteListSeializableMock> tstPackage = new TransmitCommunicationPackage<ByteListSeializableMock>();
            tstPackage.MsgID = expectedPackage[SerialPackageConstants.MsgIDPosition];
            tstPackage.ASICID = expectedPackage[SerialPackageConstants.ASICIDPosition];
            tstPackage.Cmd = (MCUCommand)expectedPackage[SerialPackageConstants.CmdStatusPosition];
            tstPackage.Payload.TransmitData = expectedPackage.Slice(SerialPackageConstants.PayloadPosition, expectedPackage[SerialPackageConstants.PayloadLengthPosition]);
            tstPackage.PayloadType = typeof(ByteListSeializableMock);

            // Act
            List<byte> constructedPackage = tstPackage.GetPackage();

            // Assert
            // Paclage content is expected
            Assert.That(constructedPackage, Is.EqualTo(expectedPackage));

            // Validation passed
            Assert.That(tstPackage.IsPackageValid, Is.True);

            // Payload type is correct
            Assert.That(tstPackage.PayloadType, Is.EqualTo(typeof(ByteListSeializableMock)));
        }

        [TestCaseSource(nameof(InValidTestCases)), Description("Checking that invalid data is not packed and error is reported")]
        public void WhenDataIsNotValid_ThenDataHandledExpectedly((int msgId, int asicID, int cmd) tcParams)
        {
            // Arrange
            TransmitCommunicationPackage<ByteListSeializableMock> tstPackage = new TransmitCommunicationPackage<ByteListSeializableMock>();
            tstPackage.MsgID = tcParams.msgId;
            tstPackage.ASICID = tcParams.asicID;
            tstPackage.Cmd = (MCUCommand)tcParams.cmd;

            // Act
            List<byte> constructedPackage = tstPackage.GetPackage();

            // Assert
            // Validation hasn't passed
            Assert.That(tstPackage.IsPackageValid, Is.False);

            // Message ID is empty
            Assert.That(constructedPackage.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// List of test cases data for valid TransmitCommunicationPackage scenarios
        /// </summary>
        public static IEnumerable<List<byte>> ValidTestCases()
        {
            yield return new List<byte>() { 0xAB, 0x01, 0x00, 0x02, 0x00, 0x69, 0xBA };
            yield return new List<byte>() { 0xAB, 0xAB, 0x01, 0x03, 0x01, 0xBA, 0x88, 0xBA };
            yield return new List<byte>() { 0xAB, 0xBA, 0x02, 0x07, 0x03, 0xAA, 0xBB, 0xCC, 0xC2, 0xBA };
            yield return new List<byte>() { 0xAB, 0x00, 0x03, 0x08, 0x00, 0x40, 0xBA };
        }

        /// <summary>
        /// List of test cases data for invalid TransmitCommunicationPackage scenarios
        /// </summary>
        public static IEnumerable<(int, int, int)> InValidTestCases()
        {
            yield return (0, 0, (int) MCUCommand._EXT_CMD_MAX);
            yield return (0, 0, (int) MCUCommand._CMD_MIN);
            yield return (0, -1, 0);
            yield return (0, 500, 0);
        }

        /// <summary>
        /// Mock target for TransmitCommunicationPackage examination
        /// </summary>
        public class ByteListSeializableMock : IByteListSerializable
        {
            public List<byte> TransmitData = new List<byte>();

            public void Deserialize(MCUStatus status, List<byte> rawData)
            {
                // Unused
                throw new NotImplementedException();
            }

            public List<byte> Serialize()
            {
                //
                return TransmitData;
            }
        }
    }
}
