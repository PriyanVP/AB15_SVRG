using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Services;
using AB15_GUI.WPF.Services.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;
using System.Collections.Concurrent;
using System.Linq;
using AB15_GUI.WPF.NLog;
using System.Threading;
using System.Threading.Tasks;

namespace AB15_GUI.Tests.Services
{
    /// <summary>
    /// Checking SerialWrapper class
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-84" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-84" />
    /// </tc_links>
    [TestFixture]
    [Parallelizable(scope: ParallelScope.Fixtures)]
    public class SerialWrapper_Tests
    {
        /// <summary>
        /// Logger reference with custom configuration
        /// </summary>
        private ILoggingService loggerMock;

        /// <summary>
        /// Set up test environment
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            loggerMock = new LoggerMock();
        }

        /// <summary>
        /// Clean up environment after finishing tests
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
        }

        // TODO: require updates

        // [TestCaseSource(nameof(ValidTransmitTestCases)), Description("Checking that valid data is send to COM port correctly")]
        // [NonParallelizable]
        // public async void WhenValidPackageIsWritten_ThenDataSendToCOMPortIsCorrect((List<byte> expectedPackage, bool isContinuous) tcParams)
        // {
        //     // Arrange
        //     TransmitPackageMock package = new TransmitPackageMock();
        //     WaitlistMock waitlistMockMock = new WaitlistMock();
        //     SerialCommMock serialCommMock = new SerialCommMock();
        //     SerialWrapper serialWrapper = new SerialWrapper(loggerMock, serialCommMock, waitlistMockMock);
          
        //     // Act
        //     package.Package = tcParams.expectedPackage; // mock to pass data
        //     package.IsPackageValid = true;
        //     package.PayloadType = typeof(ByteListSerializableMock);
        //     package.IsContinuous = tcParams.isContinuous;
        //     ReceiveCommunicationPackage<EmptyPayload>? mcuResponseStopWD = (ReceiveCommunicationPackage<EmptyPayload>?) await serialWrapper.SerialWriteAsync(package);

        //     // Assert
        //     // Package was passed to low level object for communicating with COM port successfully
        //     Assert.That(serialCommMock.WrittenPackages.First(), Is.EqualTo(tcParams.expectedPackage));

        //     // Verify waitlistMock content
        //     Assert.That(waitlistMockMock.WaitlistItems.First().isContinuous, Is.EqualTo(tcParams.isContinuous));
        //     Assert.That(waitlistMockMock.WaitlistItems.First().payloadType, Is.EqualTo(typeof(ByteListSerializableMock)));
        // }

        // [TestCaseSource(nameof(ValidReceiveTestCases)), Description("Checking that valid responses are invoked by delegates")]
        // [NonParallelizable]
        // public void WhenValidPackageIsReceived_ThenCorrectDelegateIsInvoked(List<byte> expectedPackage)
        // {
        //     // Arrange
        //     ReceiveCommunicationPackage<ByteListSerializableMock> packageGlobal = null;
        //     WaitlistMock waitlistMock = new WaitlistMock();
        //     SerialCommMock serialCommMock = new SerialCommMock();
        //     SerialWrapper serialWrapper = new SerialWrapper(loggerMock, serialCommMock, waitlistMock);           
        //     Action<IReceiveCommunicationPackage>? deleg = (package) => 
        //         {
        //             packageGlobal = (ReceiveCommunicationPackage<ByteListSerializableMock>) package;
        //         };
          
        //     // Act
        //     waitlistMock.AddItemToWaitlist(typeof(ByteListSerializableMock));
        //     foreach(byte itm in expectedPackage)
        //     {
        //         serialCommMock.ReceiveBuffer.Enqueue(itm);
        //     }
        //     Thread.Sleep(1000);

        //     // Assert
        //     // Delegate was called
        //     Assert.That(packageGlobal, Is.Not.Null);

        //     // Validation passed
        //     Assert.That(packageGlobal.IsPackageValid, Is.True);

        //     // Message ID is correct
        //     Assert.That(packageGlobal.MsgID, Is.EqualTo(expectedPackage[SerialPackageConstants.MsgIDPosition]));

        //     // ASIC ID is correct
        //     Assert.That(packageGlobal.ASICID, Is.EqualTo(expectedPackage[SerialPackageConstants.ASICIDPosition]));

        //     // Status is correct
        //     Assert.That(packageGlobal.Status, Is.EqualTo((MCUStatus)expectedPackage[SerialPackageConstants.CmdStatusPosition]));

        //     // Payload extrackted in tstPackage is payload field of package
        //     Assert.That(packageGlobal.Payload.ReceivedData, Is.EqualTo(expectedPackage.Slice(SerialPackageConstants.PayloadPosition, expectedPackage[SerialPackageConstants.PayloadLengthPosition])));
        // }

        // [TestCaseSource(nameof(InvalidReceiveTestCases)), Description("Checking that invalid responses are not invoked")]
        // [NonParallelizable]
        // public async Task WhenInvalidPackageIsReceived_ThenItNotPassesValidation(List<byte> inPackage)
        // {
        //     // Arrange
        //     ManualResetEvent timerEventFinished = new ManualResetEvent(false);
        //     ReceiveCommunicationPackage<ByteListSerializableMock> packageGlobal = null;
        //     WaitlistMock waitlistMock = new WaitlistMock();
        //     SerialCommMock serialCommMock = new SerialCommMock();
        //     SerialWrapper serialWrapper = new SerialWrapper(loggerMock, serialCommMock, waitlistMock);           
        //     Action<IReceiveCommunicationPackage>? deleg = (package) => 
        //         {
        //             packageGlobal = (ReceiveCommunicationPackage<ByteListSerializableMock>) package;
        //         };
          
        //     // Act
        //     waitlistMock.AddItemToWaitlist(typeof(ByteListSerializableMock));
        //     foreach(byte itm in inPackage)
        //     {
        //         serialCommMock.ReceiveBuffer.Enqueue(itm);
        //     }
        //     await Task.Delay(1000);

        //     // Assert
        //     // Package was removed from SerialComm
        //     Assert.That(serialCommMock.ReceiveBuffer.Count, Is.LessThan(SerialPackageConstants.MinPackageLength));

        //     // No delegate was called
        //     Assert.That(packageGlobal, Is.Null);
        // }

        /// <summary>
        /// List of test cases data (valid TransmitCommunicationPackage scenarios)
        /// </summary>
        public static IEnumerable<(List<byte>, bool)> ValidTransmitTestCases()
        {
            yield return (new List<byte>() { 0xAB, 0x01, 0x00, 0x02, 0x00, 0x69, 0xBA }, true);
            yield return (new List<byte>() { 0xAB, 0xAB, 0x01, 0x03, 0x01, 0xBA, 0x88, 0xBA }, false);
            yield return (new List<byte>() { 0xAB, 0xBA, 0x02, 0x07, 0x03, 0xAA, 0xBB, 0xCC, 0xC2, 0xBA }, true);
            yield return (new List<byte>() { 0xAB, 0x00, 0x03, 0x08, 0x00, 0x40, 0xBA }, false);
        }

        /// <summary>
        /// List of test cases data (valid ReceiveCommunicationPackage scenarios)
        /// </summary>
        public static IEnumerable<List<byte>> ValidReceiveTestCases()
        {
            yield return new List<byte>() { 0xAB, 0x01, 0x00, 0x82, 0x00, 0xDF, 0xBA };
            yield return new List<byte>() { 0xAB, 0xAB, 0x01, 0x83, 0x01, 0xBA, 0x83, 0xBA };
            yield return new List<byte>() { 0xAB, 0xBA, 0x02, 0x87, 0x03, 0xAA, 0xBB, 0xCC, 0x55, 0xBA };
            yield return new List<byte>() { 0xAB, 0x00, 0x03, 0x88, 0x00, 0xF6, 0xBA };
        }

        /// <summary>
        /// List of test cases data (invalid ReceiveCommunicationPackage scenarios)
        /// </summary>
        public static IEnumerable<List<byte>> InvalidReceiveTestCases()
        {
            yield return new List<byte>() { 0xAB, 0x01, 0x00, 0x82, 0x00, 0xDF };                   // No end byte
            yield return new List<byte>() { 0xAB, 0xAB, 0x01, 0x83, 0x01, 0xBA, 0x82, 0xBA };       // Invalid CRC
            yield return new List<byte>() { 0xBA, 0x02, 0x87, 0x03, 0xAA, 0xBB, 0xCC, 0x55, 0xBA }; // No start byte
            yield return new List<byte>() { 0xAB, 0x01, 0x00, 0x02, 0x00, 0x69, 0xBA };             // Valid transmit package
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
            public List<(Type payloadType, bool isContinuous)> WaitlistItems = 
                                new List<(Type payloadType, bool isContinuous)>();

            public (int? msgID, Task<IReceiveCommunicationPackage>? task) AddItemToWaitlist(Type payloadType, bool isContinuous = false)
            {
                WaitlistItems.Add((payloadType, isContinuous));
                return (0, Task.FromResult<IReceiveCommunicationPackage>(null));
            }

            public void ClearWaitlist()
            {
                throw new NotImplementedException();
            }

            public Task<IReceiveCommunicationPackage?> GetContinuousTaskInstance(int? msgID)
            {
                throw new NotImplementedException();
            }

            public void HandleResponse(List<byte> package)
            {
                throw new NotImplementedException();
            }

            public bool RemoveItemFromWaitlist(int? msgID)
            {
                throw new NotImplementedException();
            }

            public void RemoveOutdatedItems()
            {
                throw new NotImplementedException();
            }
        }

        public class ReceivePackageMock : IReceiveCommunicationPackage
        {
            public int MsgID { get; set; }

            public int ASICID { get; set; }

            public MCUStatus Status { get; set; }

            public bool IsPackageValid { get; set; }

            public bool UnpackingState { get; set; }

            public List<byte> ReceivedData { get; set; }

            public bool UnpackPackage(List<byte> receivedPackage)
            {
                ReceivedData = receivedPackage;
                return UnpackingState;
            }
        }

        public class TransmitPackageMock : ITransmitCommunicationPackage
        {
            public int? MsgID { get; set; }
            

            public int ASICID { get; set; }
            public MCUCommand Cmd { get; set; }

            public Type PayloadType { get; set; }

            public bool IsPackageValid { get; set; }

            public List<byte> Package { get; set; } = new List<byte>();
            public bool IsContinuous { get; set; }

            public List<byte> GetPackage()
            {
                return Package;
            }
        }

        
        /// <summary>
        /// Mock target for ReceiveCommunicationPackage examination
        /// </summary>
        public class ByteListSerializableMock : IByteListSerializable
        {
            public List<byte> ReceivedData = new List<byte>();

            public string? Error { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

        public class LoggerMock : ILoggingService
        {
            public bool IsDebugEnabled => false;

            public bool IsErrorEnabled => throw new NotImplementedException();

            public bool IsFatalEnabled => throw new NotImplementedException();

            public bool IsInfoEnabled => throw new NotImplementedException();

            public bool IsTraceEnabled => throw new NotImplementedException();

            public bool IsWarnEnabled => throw new NotImplementedException();

            public string Name => throw new NotImplementedException();

            public void Debug(string format, params object[] args)
            {
                // Do nothing
                return;
            }

            public void Debug(Exception exception, string format, params object[] args)
            {
                // Do nothing
                return;
            }

            public void Error(string format, params object[] args)
            {
                // Do nothing
                return;
            }

            public void Error(Exception exception, string format, params object[] args)
            {
                // Do nothing
                return;
            }

            public void Fatal(string format, params object[] args)
            {
                // Do nothing
                return;
            }

            public void Fatal(Exception exception, string format, params object[] args)
            {
                // Do nothing
                return;
            }

            public void Info(string format, params object[] args)
            {
                // Do nothing
                return;
            }

            public void Info(Exception exception, string format, params object[] args)
            {
                // Do nothing
                return;
            }

            public void Trace(string format, params object[] args)
            {
                // Do nothing
                return;
            }

            public void Trace(Exception exception, string format, params object[] args)
            {
                // Do nothing
                return;
            }

            public void Warn(string format, params object[] args)
            {
                // Do nothing
                return;
            }

            public void Warn(Exception exception, string format, params object[] args)
            {
                // Do nothing
                return;
            }
        }

        #endregion // Mocks
    }
}
