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

            // Workaround for thread sync to work correctly TODO: refactor approach to remove close coupling
            if (Application.Current == null)
            { 
                new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown }; 
            }
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
            Waitlist waitlist = new Waitlist(logger);
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

        #endregion // Mocks
    }
}
