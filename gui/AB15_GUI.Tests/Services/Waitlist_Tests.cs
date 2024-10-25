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
using System.Threading;
using System.Windows;
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

        [Test, Description("Checking that valid data is send to COM port correctly")]
        [NonParallelizable]
        public void WhenValidItemsAreAdded_ThenWaitlistItemIsCorrect([Values(typeof(EmptyPayload))] Type payloadType, [Values(true, false)] bool isContinuous)
        {
            // Arrange
            Action<IReceiveCommunicationPackage> delegIn = (package) => {};
            Action<IReceiveCommunicationPackage>? delegOut = null;
            Waitlist waitlist = new Waitlist(loggerMock);
            ReceivePackageMock receivedPackage = new ReceivePackageMock();
            bool isAddedSuccessfully = false;
          
            // Act
            receivedPackage.IsPackageValid = true;
            (receivedPackage.MsgID, isAddedSuccessfully) = waitlist.AddItemToWaitlist(delegIn, payloadType, isContinuous);
            delegOut = waitlist.GetDelegate(receivedPackage);

            // Assert
            // Check that flag indicating successful adding is valid
            Assert.That(isAddedSuccessfully, Is.True);

            // Verify Delegate
            Assert.That(delegOut, Is.EqualTo(delegIn));
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
            public bool IsDebugEnabled => throw new NotImplementedException();

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
