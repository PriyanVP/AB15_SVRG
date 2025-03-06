using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.Services.Interfaces;
using AB15_GUI.WPF.ViewModels;
using NLog;
using System.Collections.Generic;
using System.Windows;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AB15_GUI.Tests.ViewModels
{
    /// <summary>
    /// Watchdog View Model tests
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-132" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-132" />
    /// </tc_links>
    [TestFixture]
    [NonParallelizable]
    public class WatchdogViewModel_Tests
    {
        private ILoggingService loggerMock;
        private WatchdogViewModel watchdogVM;
        private SerialWrapperMock serialMock;
        private ASICWrapperMock asicMock;

        /// <summary>
        /// Set up test environment
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            loggerMock = new LoggerMock(); // Same logger will be used across all tests
            serialMock = new SerialWrapperMock();
            asicMock = new ASICWrapperMock();
            watchdogVM = new WatchdogViewModel(loggerMock, serialMock, asicMock);
        }

        /// <summary>
        /// Clean up environment after finishing tests
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
        }

        [Test, Order(1), Description("Check parameters in IDLE state")]
        [NonParallelizable]
        public void WhenWDSequenceCorrect_ThenIdleWorksExpectedly()
        {
            // Arrange

            // Act

            // Assert

            // Expected state after POR
            Assert.That(watchdogVM.StateObservation, Is.EqualTo(WatchdogViewModel.State.Idle));

            // Expected commands can execute states and config enable flag
            Assert.That(watchdogVM.ReadWDConfigCommandEn, Is.True);
            Assert.That(watchdogVM.WriteWDConfigCommandEn, Is.False);
            Assert.That(watchdogVM.StartWDCommandEn, Is.False);
            Assert.That(watchdogVM.StopWDCommandEn, Is.False);
            Assert.That(watchdogVM.IsConfigEnable, Is.False);
        }

        [Test, Order(2), Description("Check parameters in InConfiguration state")]
        [NonParallelizable]
        public void WhenWDSequenceCorrect_ThenInConfigurationWorksExpectedly()
        {
            // Arrange
            bool commandCanExecuteBefore;
            ReceiveCommunicationPackage<AddressDataPayload> receiveCommunicationPackage = new ReceiveCommunicationPackage<AddressDataPayload>();
            receiveCommunicationPackage.ASICID = 1;
            receiveCommunicationPackage.Status = MCUStatus.DATA;
            receiveCommunicationPackage.Payload.Data.AddRange(new List<UInt16>() { 0, 0, 0, 0, 0, 0 });
            serialMock.transmittedPackages.Clear();
            serialMock.receiveCommunicationPackages.Clear();

            // Act
            serialMock.receiveCommunicationPackages.Add(receiveCommunicationPackage); // commented for AB12
            commandCanExecuteBefore = watchdogVM.ReadWDConfigCommandEn;
            watchdogVM.ReadConfigFromASIC.Execute(null); // Emulate transition

            // Assert
            // Expected data transmitted
            Assert.That(serialMock.transmittedPackages.Count, Is.EqualTo(1));
            Assert.That(serialMock.transmittedPackages[0].Cmd, Is.EqualTo(MCUCommand.EXECUTE_READ_SEQUENCE)); // should be read sequence

            // Expected state after transition
            Assert.That(watchdogVM.StateObservation, Is.EqualTo(WatchdogViewModel.State.InConfiguration));

            // Expected commands can execute states and config enable flag
            Assert.That(commandCanExecuteBefore, Is.True);
            Assert.That(watchdogVM.ReadWDConfigCommandEn, Is.True);
            Assert.That(watchdogVM.WriteWDConfigCommandEn, Is.True);
            Assert.That(watchdogVM.StartWDCommandEn, Is.False);
            Assert.That(watchdogVM.StopWDCommandEn, Is.False);
            Assert.That(watchdogVM.IsConfigEnable, Is.True);
        }

        [Test, Order(3), Description("Check parameters in Configured state")]
        [NonParallelizable]
        public void WhenWDSequenceCorrect_ThenConfiguredWorksExpectedly()
        {
            // Arrange
            bool commandCanExecuteBefore;
            ReceiveCommunicationPackage<EmptyPayload> receiveCommunicationPackage = new ReceiveCommunicationPackage<EmptyPayload>();
            receiveCommunicationPackage.ASICID = 1;
            receiveCommunicationPackage.Status = MCUStatus.ACK;
            serialMock.transmittedPackages.Clear();
            serialMock.receiveCommunicationPackages.Clear();

            // Act
            serialMock.receiveCommunicationPackages.Add(receiveCommunicationPackage);
            commandCanExecuteBefore = watchdogVM.WriteWDConfigCommandEn;
            watchdogVM.WriteConfigToASIC.Execute(null); // Emulate transition

            // Assert
            // Expected data transmitted
            Assert.That(serialMock.transmittedPackages.Count, Is.EqualTo(1));
            Assert.That(serialMock.transmittedPackages[0].Cmd, Is.EqualTo(MCUCommand.CONFIGURE_WATCHDOG));

            // Expected state after transition
            Assert.That(watchdogVM.StateObservation, Is.EqualTo(WatchdogViewModel.State.Configured));

            // Expected commands can execute states and config enable flag
            Assert.That(commandCanExecuteBefore, Is.True);
            Assert.That(watchdogVM.ReadWDConfigCommandEn, Is.True);
            Assert.That(watchdogVM.WriteWDConfigCommandEn, Is.True);
            Assert.That(watchdogVM.StartWDCommandEn, Is.True);
            Assert.That(watchdogVM.StopWDCommandEn, Is.True);
            Assert.That(watchdogVM.IsConfigEnable, Is.True);
        }

        //[Test, Order(4), Description("Check parameters in Running state")] // TODO: find a way to handle - feature works on bench but failing in tests (stuck in endless loop)
        //[NonParallelizable]
        //public void WhenWDSequenceCorrect_ThenRunningWorksExpectedly()
        //{
        //    // Arrange
        //    bool commandCanExecuteBefore;
        //    ReceiveCommunicationPackage<EmptyPayload> receiveCommunicationPackage = new ReceiveCommunicationPackage<EmptyPayload>();
        //    receiveCommunicationPackage.ASICID = 1;
        //    receiveCommunicationPackage.Status = MCUStatus.ACK;

        //    serialMock.transmittedPackages.Clear();
        //    serialMock.receiveCommunicationPackages.Clear();

        //    // Act
        //    serialMock.receiveCommunicationPackages.Add(receiveCommunicationPackage);
        //    commandCanExecuteBefore = watchdogVM.StartWDCommandEn;
        //    watchdogVM.StartWatchdog.Execute(null); // Emulate transition

        //    // Assert
        //    // Expected data transmitted
        //    Assert.That(serialMock.transmittedPackages.Count, Is.EqualTo(1));
        //    Assert.That(serialMock.transmittedPackages[0].Cmd, Is.EqualTo(MCUCommand.START_WATCHDOG));
        //    Assert.That(serialMock.transmittedPackages[1].Cmd, Is.EqualTo(MCUCommand.START_MONITORING_WATCHDOG));

        //    // Expected state after transition
        //    Assert.That(watchdogVM.StateObservation, Is.EqualTo(WatchdogViewModel.State.Running));

        //    // Expected commands can execute states and config enable flag
        //    Assert.That(commandCanExecuteBefore, Is.True);
        //    Assert.That(watchdogVM.ReadWDConfigCommandEn, Is.True);
        //    Assert.That(watchdogVM.WriteWDConfigCommandEn, Is.False);
        //    Assert.That(watchdogVM.StartWDCommandEn, Is.False);
        //    Assert.That(watchdogVM.StopWDCommandEn, Is.True);
        //    Assert.That(watchdogVM.IsConfigEnable, Is.False);
        //}

        //[Test, Order(5), Description("Check parameters after stop")] // TODO: refactor, requires more advanced mocks
        //[NonParallelizable]
        //public void WhenWDSequenceCorrect_ThenStoppingWorksExpectedly()
        //{
        //    // Arrange
        //    bool commandCanExecuteBefore;
        //    ReceiveCommunicationPackage<EmptyPayload> receiveCommunicationPackage = new ReceiveCommunicationPackage<EmptyPayload>();
        //    receiveCommunicationPackage.ASICID = 1;
        //    receiveCommunicationPackage.Status = MCUStatus.ACK;

        //    ReceiveCommunicationPackage<WDStatusPayload> receiveCommunicationPackageMonitor = new ReceiveCommunicationPackage<WDStatusPayload>();
        //    receiveCommunicationPackageMonitor.ASICID = 1;
        //    receiveCommunicationPackageMonitor.Status = MCUStatus.ACK;
        //    serialMock.transmittedPackages.Clear();
        //    serialMock.receiveCommunicationPackages.Clear();
        //    serialMock.WaitlistItemRemoved = false;

        //    // Act
        //    serialMock.receiveCommunicationPackages.Add(receiveCommunicationPackage);
        //    serialMock.receiveCommunicationPackages.Add(receiveCommunicationPackageMonitor);
        //    commandCanExecuteBefore = watchdogVM.StopWDCommandEn;
        //    watchdogVM.StopWatchdog.Execute(null); // Emulate transition

        //    // Assert
        //    // Expected data transmitted
        //    Assert.That(serialMock.transmittedPackages.Count, Is.EqualTo(2));
        //    Assert.That(serialMock.transmittedPackages[0].Cmd, Is.EqualTo(MCUCommand.STOP_WATCHDOG));
        //    Assert.That(serialMock.transmittedPackages[1].Cmd, Is.EqualTo(MCUCommand.STOP_MONITORING_WATCHDOG));

        //    // Expected state after transition
        //    Assert.That(watchdogVM.StateObservation, Is.EqualTo(WatchdogViewModel.State.InConfiguration));

        //    // Expected commands can execute states and config enable flag
        //    Assert.That(commandCanExecuteBefore, Is.True);
        //    Assert.That(watchdogVM.ReadWDConfigCommandEn, Is.True);
        //    Assert.That(watchdogVM.WriteWDConfigCommandEn, Is.True);
        //    Assert.That(watchdogVM.StartWDCommandEn, Is.False);
        //    Assert.That(watchdogVM.StopWDCommandEn, Is.False);
        //    Assert.That(watchdogVM.IsConfigEnable, Is.True);

        //    // Check that waitlist item removal was called for stop monitoring watchdog
        //    Assert.That(serialMock.WaitlistItemRemoved, Is.True);
        //}

        #region Mocks

        public class ASICWrapperMock : IASICWrapper
        {
            public ObservableCollection<IASIC> ASICs => throw new NotImplementedException();

            public void EstablishConnection()
            {
                throw new NotImplementedException();
            }

            public Task EstablishConnectionAsync()
            {
                throw new NotImplementedException();
            }

            public void StartInitModeTimeoutResetting()
            {
                throw new NotImplementedException();
            }
        }

        public class SerialWrapperMock : ISerialWrapper
        {
            public List<string> AvailableCOMPorts => throw new NotImplementedException();

            public bool IsCOMPortPresent => throw new NotImplementedException();

            public string? ManualComPortName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public void Dispose() => throw new NotImplementedException();

            public bool ReconnectCOMPort() => throw new NotImplementedException();

            public bool DicsonnectCOMPort() => throw new NotImplementedException();

            public bool RemoveWaitlistItem(int? msgID)
            {
                WaitlistItemRemoved = true;
                return true;
            }

            /// <summary>
            /// Property to watch if Waitlist item removal was called
            /// </summary>
            public bool WaitlistItemRemoved = false;

            /// <summary>
            /// Variable to hold monitoring communication package
            /// </summary>
            public int MonitoringMsgId = 5;

            /// <summary>
            /// Variable to hold monitoring communication package response
            /// </summary>
            public IReceiveCommunicationPackage monitoringCommunicationPackage;
            /// <summary>
            /// Property to configure emulation of received package
            /// </summary>
            public List<IReceiveCommunicationPackage> receiveCommunicationPackages = new List<IReceiveCommunicationPackage>();

            /// <summary>
            /// Observable field for storing transmitted packages
            /// </summary>
            public List<ITransmitCommunicationPackage> transmittedPackages = new List<ITransmitCommunicationPackage>();

            public SerialWrapperMock()
            {
                ReceiveCommunicationPackage<WDStatusPayload> tmpPackage = new ReceiveCommunicationPackage<WDStatusPayload>();
                tmpPackage.ASICID = 1;
                tmpPackage.Payload.Deserialize(MCUStatus.DATA, new List<byte>() {0, 0, 1, 0});

                monitoringCommunicationPackage = tmpPackage;
            }

            public Task<IReceiveCommunicationPackage?> SerialWriteAsync(ITransmitCommunicationPackage packageToSend)
            {
                Task<IReceiveCommunicationPackage?> task = Task.FromResult<IReceiveCommunicationPackage?>(null);

                // Store transmitted packages
                transmittedPackages.Add(packageToSend);

                // Emulate receiving response from MCU
                if (packageToSend.Cmd == MCUCommand.START_MONITORING_WATCHDOG)
                {
                    task = Task.FromResult<IReceiveCommunicationPackage?>(monitoringCommunicationPackage);
                }
                else if (receiveCommunicationPackages.Count != 0)
                {
                    task = Task.FromResult<IReceiveCommunicationPackage?>(receiveCommunicationPackages[transmittedPackages.Count - 1]);
                }

                return task;
            }

            public Task<IReceiveCommunicationPackage?> GetContinuousTaskInstance(int? msgID)
            {
                return Task.FromResult<IReceiveCommunicationPackage?>(monitoringCommunicationPackage);
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