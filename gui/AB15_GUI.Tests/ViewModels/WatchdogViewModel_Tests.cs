using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.Services.Interfaces;
using AB15_GUI.WPF.ViewModels;
using NLog;
using System.Collections.Generic;
using System.Windows;

namespace AB15_GUI.Tests.ViewModels
{
    /// <summary>
    /// Watchdog View Model tests
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-" Link="" />
    /// </tc_links>
    [TestFixture]
    [NonParallelizable]
    public class WatchdogViewModel_Tests
    {
        private Logger logger;
        private LogMemoryRecordTarget target;

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
            target = (LogMemoryRecordTarget)LogManager.Configuration.FindTargetByName("memory");

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
            LogManager.Shutdown();
        }

        // [Test, Description("Messages adding count test")]
        // [NonParallelizable]
        // public void WhenLoggerViewModelInitialized_ThenNewLogRecordsAreAdded([Random(20, 30, 3)] int addedItemsCount)
        // {
        //     // Arrange
        //     int numberOfMessages = 0;
        //     LoggerViewModel viewModel = new LoggerViewModel(logger, target);
        //     target.Logs.Clear();


        //     // Act
        //     for (int i = 0; i < addedItemsCount; i++)
        //     {
        //         logger.Fatal($"some message {i}");
        //     }
        //     numberOfMessages = viewModel.LoggerRecords.Count;

        //     // Assert
        //     // Number of records expected
        //     Assert.That(numberOfMessages, Is.EqualTo(addedItemsCount));
        // }

        // [Test, Description("Messages adding format test")]
        // [NonParallelizable]
        // public void WhenLoggerViewModelInitialized_ThenNewLogRecordsAreExpected([Random(20, 30, 3)] int addedItemsCount)
        // {
        //     // Arrange
        //     LoggerViewModel viewModel = new LoggerViewModel(logger, target);
        //     target.Logs.Clear();

        //     // Act
        //     for (int i = 0; i < addedItemsCount; i++)
        //     {
        //         logger.Fatal($"some message {i}");
        //     }

        //     // Assert
        //     // Records are expected
        //     for (int i = 0; i < addedItemsCount; i++)
        //     {
        //         Assert.That(viewModel.LoggerRecords[i].Message, Is.EqualTo($"some message {i}"));
        //         Assert.That(viewModel.LoggerRecords[i].Level, Is.Not.Null);
        //         Assert.That(viewModel.LoggerRecords[i].Index, Is.Not.Null);
        //     }
        // }
    }

    #region Mocks

    public class SerialWrapperMock : ISerialWrapper
    {
        public List<string> AvailableCOMPorts => throw new System.NotImplementedException();

        public bool IsCOMPortPresent => throw new System.NotImplementedException();

        public string? ManualComPortName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public bool ReconnectCOMPort()
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveWaitlistItem(int? msgID)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Payload Error property for response
        /// </summary>
        public string? PayloadError = null;

        /// <summary>
        /// Status of response
        /// </summary>
        public MCUStatus ResponseStatus;

        /// <summary>
        /// Observable field for storing transmitted package
        /// </summary>
        public ITransmitCommunicationPackage transmittedPackage;

        public bool SerialWrite(ITransmitCommunicationPackage packageToSend)
        {
            // Store last transmitted package to variable
            transmittedPackage = packageToSend;

            // TODO: fill in
            switch (packageToSend.Cmd)
            {
                case MCUCommand.CONFIGURE_WATCHDOG:
                    // Emulate response from MCU
                    ReceiveCommunicationPackage<EmptyPayload> mcuResponse = new ReceiveCommunicationPackage<EmptyPayload>();
                    mcuResponse.Status = ResponseStatus;
                    mcuResponse.Payload.Error = PayloadError;
                    break;
                case MCUCommand.START_WATCHDOG:
                    break;
                case MCUCommand.STOP_WATCHDOG:
                    break;
                case MCUCommand.START_MONITORING_WATCHDOG:
                    break;
                case MCUCommand.STOP_MONITORING_WATCHDOG:
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            return true;
        }
    }

    #endregion // Mocks
}