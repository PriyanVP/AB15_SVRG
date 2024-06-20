using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Services;
using AB15_GUI.WPF.Services.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;
using System.Collections.Concurrent;
using System.Linq;
using AB15_GUI.WPF.NLog;
using NLog;
using System.Windows.Controls;

namespace AB15_GUI.Tests.Services
{
    /// <summary>
    /// Checking SerialWrapper class
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-84" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-84" />
    /// </tc_links>
    [TestFixture]
    [Parallelizable(scope: ParallelScope.Self)]
    public class SerialWrapper_Tests
    {
        /// <summary>
        /// Logger reference with custom configuration
        /// </summary>
        private Logger logger;

        /// <summary>
        /// Set up test environment
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            logger = LogManager.Setup()
                    .SetupExtensions(ext => ext.RegisterLayoutRenderer<BuildConfigurationLayoutRenderer>("build-configuration"))
                    .SetupExtensions(ext => ext.RegisterTarget<LogMemoryRecordTarget>("MemoryRecord"))
                    .GetCurrentClassLogger(); // Same logger will be used across all tests
        }

        /// <summary>
        /// Clean up environment after finishing tests
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
        }

        [TestCaseSource(nameof(ValidTestCases)), Description("Checking that valid data is send to COM port correctly")]
        [NonParallelizable]
        public void WhenValidPackageIsWritten_ThenDataSendToCOMPortIsCorrect((List<byte> expectedPackage, bool isContinuous) tcParams) // TODO: not ready
        {
            // Arrange
            TransmitPackageMock package = new TransmitPackageMock();
            WaitlistMock waitlist = new WaitlistMock();
            SerialCommMock serialCommMock = new SerialCommMock();
            SerialWrapper serialWrapper = new SerialWrapper(logger, serialCommMock, waitlist);
            Action<IReceiveCommunicationPackage>? deleg = (package) => {};
          
            // Act
            package.Package = tcParams.expectedPackage; // mock to pass data
            package.IsPackageValid = true;
            package.PayloadType = typeof(IReceiveCommunicationPackage);
            serialWrapper.SerialWrite(package, deleg, tcParams.isContinuous);

            // Assert
            // Package was passed to low level object for communicating with COM port successfully
            Assert.That(serialCommMock.WrittenPackages.First(), Is.EqualTo(tcParams.expectedPackage));

            // Verify waitlist content
            Assert.That(waitlist.WaitlistItems.First().deleg, Is.EqualTo(deleg));
            Assert.That(waitlist.WaitlistItems.First().isContinuous, Is.EqualTo(tcParams.isContinuous));
            Assert.That(waitlist.WaitlistItems.First().payloadType, Is.EqualTo(typeof(IReceiveCommunicationPackage)));
        }


        //// TODO: test read
        //[TestCaseSource(nameof(InValidTestCases)), Description("Checking that invalid data is not packed and error is reported")]
        //public void WhenDataIsNotValid_ThenDataHandledExpectedly(List<byte> fullPackage) // TODO: not ready
        //{
        //   // Arrange
        //   TransmitCommunicationPackage<ByteListSeializableMock> tstPackage = new TransmitCommunicationPackage<ByteListSeializableMock>();

        //   // Act
        //   tstPackage.UnpackPackage(receivedPackage);

        //   // Assert
        //   // Validation hasn't passed
        //   Assert.That(tstPackage.IsPackageValid, Is.False);

        //   // Message ID is empty
        //   Assert.That(tstPackage.MsgID, Is.EqualTo(default(int)));

        //   // ASIC ID is empty
        //   Assert.That(tstPackage.ASICID, Is.EqualTo(default(int)));

        //   // Status is empty
        //   Assert.That(tstPackage.Status, Is.EqualTo(MCUStatus._STATUS_MIN));

        //   // Payload is empty
        //   Assert.That(tstPackage.Payload.ReceivedData.Count, Is.EqualTo(0));
        //}


        /// <summary>
        /// List of test cases data (valid TransmitCommunicationPackage scenarios)
        /// </summary>
        public static IEnumerable<(List<byte>, bool)> ValidTestCases()
        {
            yield return (new List<byte>() { 0xAB, 0x01, 0x00, 0x02, 0x00, 0x69, 0xBA }, true);
            yield return (new List<byte>() { 0xAB, 0xAB, 0x01, 0x03, 0x01, 0xBA, 0x88, 0xBA }, false);
            yield return (new List<byte>() { 0xAB, 0xBA, 0x02, 0x07, 0x03, 0xAA, 0xBB, 0xCC, 0xC2, 0xBA }, true);
            yield return (new List<byte>() { 0xAB, 0x00, 0x03, 0x08, 0x00, 0x40, 0xBA }, false);
        }

        #region Mocks

        public class SerialCommMock : ISerialComm
        {
            public ConcurrentQueue<byte> ReceiveBuffer { get; set; } = new ConcurrentQueue<byte>();

            public string? ManualCOMPortName { get; set; }

            public List<List<byte>> WrittenPackages { get; set; } = new List<List<byte>>();

            public bool ConnectCOMPort()
            {
                throw new NotImplementedException();
            }

            public List<string> GetCOMPorts()
            {
                throw new NotImplementedException();
            }

            public void Write(byte[] sendData, int length)
            {
                WrittenPackages.Add(new List<byte>(sendData.Take(length)) );
            }
        }

        public class WaitlistMock : IWaitlist
        {
            // For observation
            public List<(Action<IReceiveCommunicationPackage> deleg, Type payloadType, bool isContinuous)> WaitlistItems = 
                                new List<(Action<IReceiveCommunicationPackage> deleg, Type payloadType, bool isContinuous)>();

            public (int msgID, bool isAddedSuccessfully) AddItemToWaitlist(Action<IReceiveCommunicationPackage> deleg, Type payloadType, bool isContinuous = false)
            {
                WaitlistItems.Add((deleg, payloadType, isContinuous));
                return (0, true);
            }

            public void ClearWaitlist()
            {
                throw new NotImplementedException();
            }

            public Action<IReceiveCommunicationPackage>? GetDelegate(IReceiveCommunicationPackage receivedPackage)
            {
                return WaitlistItems[0].deleg;
            }

            public Type? GetPayloadType(int msgID)
            {
                throw new NotImplementedException();
            }

            public bool RemoveItemFromWaitlist(int? msgID)
            {
                throw new NotImplementedException();
            }

            public List<Action<IReceiveCommunicationPackage>> RemoveOutdatedItems()
            {
                return new List<Action<IReceiveCommunicationPackage>>();
            }
        }

        public class ReceivePackageMock : IReceiveCommunicationPackage
        {
            public int MsgID { get; set; }

            public int ASICID { get; set; }

            public MCUStatus Status { get; set; }

            public bool IsPackageValid { get; set; }

            public bool UnpackingState { get; set; }

            public bool UnpackPackage(List<byte> receivedPackage)
            {
                return UnpackingState;
            }
        }

        public class TransmitPackageMock : ITransmitCommunicationPackage
        {
            public int MsgID { get; set; }

            public int ASICID { get; set; }
            public MCUCommand Cmd { get; set; }

            public Type PayloadType { get; set; }

            public bool IsPackageValid { get; set; }

            public List<byte> Package { get; set; } = new List<byte>();

            public List<byte> GetPackage()
            {
                return Package;
            }
        }

        #endregion // Mocks
    }
}
