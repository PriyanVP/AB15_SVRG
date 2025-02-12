using System;
using System.Collections.Generic;
using AB15_GUI.WPF.Services;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.NLog;
using System.Threading.Tasks;

namespace AB15_GUI.Tests.Services
{
    /// <summary>
    /// Checking Waitlist class
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-84" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-84" />
    /// </tc_links>
    [TestFixture]
    [Parallelizable(scope: ParallelScope.Self)]
    public class Waitlist_Tests
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
            loggerMock = new LoggerMock(); // Same logger will be used across all tests
        }

        /// <summary>
        /// Clean up environment after finishing tests
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
        }

        [Test, Description("Checking that valid items are added to waitlist")]
        [NonParallelizable]
        public void WhenValidItemsAreAdded_ThenWaitlistItemIsCorrect([Values(typeof(EmptyPayload))] Type payloadType, [Values(true, false)] bool isContinuous)
        {
            // Arrange
            Waitlist waitlist = new Waitlist(loggerMock);
            Task<IReceiveCommunicationPackage>? task;
            int? msgID;
          
            // Act
            (msgID,  task) = waitlist.AddItemToWaitlist(payloadType, isContinuous);

            // Assert          
            // Verify if added successfully
            Assert.That(msgID, Is.Not.Null);
            Assert.That(task, Is.Not.Null);
        }

        #region Mocks

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
