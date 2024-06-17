using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;

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
    public class ReceiveCommunicationPackage_Tests
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

        [TestCaseSource(nameof(ValidTestCases)), Description("Checking that valid data is unpacked correctly")]
        public void WhenDataIsValid_ThenDataIsUnpackedCorrectly(List<byte> receivedPackage)
        {
            // Arrange
            ReceiveCommunicationPackage<ByteListSeializableMock> tstPackage = new ReceiveCommunicationPackage<ByteListSeializableMock>();

            // Act
            tstPackage.UnpackPackage(receivedPackage);

            // Assert
            // Validation passed
            Assert.That(tstPackage.IsPackageValid, Is.True);

            // Message ID is correct
            Assert.That(tstPackage.MsgID, Is.EqualTo(receivedPackage[SerialPackageConstants.MsgIDPosition]));

            // ASIC ID is correct
            Assert.That(tstPackage.ASICID, Is.EqualTo(receivedPackage[SerialPackageConstants.ASICIDPosition]));

            // Status is correct
            Assert.That(tstPackage.Status, Is.EqualTo((MCUStatus)receivedPackage[SerialPackageConstants.CmdStatusPosition]));

            // Payload extrackted in tstPackage is payload field of package
            Assert.That(tstPackage.Payload.ReceivedData, Is.EqualTo(receivedPackage.Slice(SerialPackageConstants.PayloadPosition, receivedPackage[SerialPackageConstants.PayloadLengthPosition])));
        }

        [TestCaseSource(nameof(InValidTestCases)), Description("Checking that invalid data is unpacked expectedly")]
        public void WhenDataIsNotValid_ThenDataHandledExpectedly(List<byte> receivedPackage)
        {
            // Arrange
            //byte calculatedCRC = receivedPackage.GetCRC8(1, receivedPackage.Count - 3);

            //// Act
            //length = testCaseParams.data.Count - testCaseParams.startIdx;
            //calculatedCRC = testCaseParams.data.GetCRC8(testCaseParams.startIdx, length);

            //// Assert
            //Assert.That(calculatedCRC, Is.EqualTo(testCaseParams.expectedCRC));
        }

        /// <summary>
        /// List of test cases data for valid ReceiveCommunicationPackage scenarios
        /// </summary>
        public static IEnumerable<List<byte>> ValidTestCases()
        {
            yield return new List<byte>() { 0xAB, 0x01, 0x00, 0x82, 0x00, 0xDF, 0xBA };
            yield return new List<byte>() { 0xAB, 0xAB, 0x01, 0x83, 0x01, 0xBA, 0x83, 0xBA };
            yield return new List<byte>() { 0xAB, 0xBA, 0x02, 0x87, 0x03, 0xAA, 0xBB, 0xCC, 0x55, 0xBA };
            yield return new List<byte>() { 0xAB, 0x00, 0x03, 0x88, 0x00, 0xF6, 0xBA };
        }

        /// <summary>
        /// List of test cases data for invalid ReceiveCommunicationPackage scenarios
        /// </summary>
        public static IEnumerable<List<byte>> InValidTestCases() // TODO: CRC not correct
        {
            yield return new List<byte>() { 0xAB, 0x01, 0x00, 0x82, 0x00, 0x69, 0xBA };
            yield return new List<byte>() { 0xAB, 0xAB, 0x01, 0x83, 0x01, 0xBA, 0x88, 0xBA };
            yield return new List<byte>() { 0xAB, 0xBA, 0x02, 0x87, 0x03, 0xAA, 0xBB, 0xCC, 0xC2, 0xBA };
            yield return new List<byte>() { 0xAB, 0x00, 0x03, 0x08, 0x00, 0x40, 0xBA };
        }

        /// <summary>
        /// Mock target for ReceiveCommunicationPackage examination
        /// </summary>
        public class ByteListSeializableMock : IByteListSerializable
        {
            public List<byte> ReceivedData;

            public void Deserialize(MCUStatus status, List<byte> rawData)
            {
                ReceivedData = rawData;
            }

            public List<byte> Serialize()
            {
                // Unused
                throw new NotImplementedException();
            }
        }
    }
}
