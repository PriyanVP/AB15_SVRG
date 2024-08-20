using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.NLog;
using AB15_GUI.WPF.Services.Interfaces;
using AB15_GUI.WPF.ViewModels;
using NLog;
using System.Collections.Generic;
using System.Windows;
using System;

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
        private WatchdogViewModel watchdogVM;
        private SerialWrapperMock serialMock;

        /// <summary>
        /// Set up test environment
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            // Workaround for thread sync to work correctly TODO: refactor approach to remove close coupling
            if (Application.Current == null)
            { 
                new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown }; 
            }

            logger = LogManager.Setup()
                                .SetupExtensions(ext => ext.RegisterLayoutRenderer<BuildConfigurationLayoutRenderer>("build-configuration"))
                                .SetupExtensions(ext => ext.RegisterTarget<LogMemoryRecordTarget>("MemoryRecord"))
                                .GetCurrentClassLogger(); // Same logger will be used across all tests
            serialMock = new SerialWrapperMock();
            watchdogVM = new WatchdogViewModel(logger, serialMock);
        }

        /// <summary>
        /// Clean up environment after finishing tests
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
            LogManager.Shutdown();
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
            Assert.That(watchdogVM.ReadConfigFromASIC.CanExecute(null), Is.True);
            Assert.That(watchdogVM.WriteConfigToASIC.CanExecute(null),  Is.False);
            Assert.That(watchdogVM.StartWatchdog.CanExecute(null),      Is.False);
            Assert.That(watchdogVM.StopWatchdog.CanExecute(null),       Is.False);
            Assert.That(watchdogVM.IsConfigEnable,                      Is.False);
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
            // serialMock.receiveCommunicationPackages.Add(receiveCommunicationPackage); // commented for AB12
            commandCanExecuteBefore = watchdogVM.ReadConfigFromASIC.CanExecute(null);
            watchdogVM.ReadConfigFromASIC.Execute(null); // Emulate transition

            // Assert
            // Expected data transmitted
            // Assert.That(serialMock.transmittedPackages.Count, Is.EqualTo(1));
            // Assert.That(serialMock.transmittedPackages[0].Cmd, Is.EqualTo(MCUCommand.)); // should be read sequence

            // Expected state after transition
            Assert.That(watchdogVM.StateObservation, Is.EqualTo(WatchdogViewModel.State.InConfiguration));

            // Expected commands can execute states and config enable flag
            Assert.That(commandCanExecuteBefore,                        Is.True);
            Assert.That(watchdogVM.ReadConfigFromASIC.CanExecute(null), Is.True);
            Assert.That(watchdogVM.WriteConfigToASIC.CanExecute(null),  Is.True);
            Assert.That(watchdogVM.StartWatchdog.CanExecute(null),      Is.False);
            Assert.That(watchdogVM.StopWatchdog.CanExecute(null),       Is.False);
            Assert.That(watchdogVM.IsConfigEnable,                      Is.True);

            // Observable properties are expected
            Assert.That(watchdogVM.WD1ResponseTime, Is.EqualTo(63));    // AB12 values
            Assert.That(watchdogVM.WD2ResponseTime, Is.EqualTo(16));    // AB12 values
            Assert.That(watchdogVM.WD1LockTime, Is.EqualTo(0));         // AB12 values
            Assert.That(watchdogVM.WD2LockTime, Is.EqualTo(10));        // AB12 values
            // More for AB15
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
            commandCanExecuteBefore = watchdogVM.WriteConfigToASIC.CanExecute(null);
            watchdogVM.WriteConfigToASIC.Execute(null); // Emulate transition

            // Assert
            // Expected data transmitted
            Assert.That(serialMock.transmittedPackages.Count, Is.EqualTo(1));
            Assert.That(serialMock.transmittedPackages[0].Cmd, Is.EqualTo(MCUCommand.CONFIGURE_WATCHDOG));

            // Expected state after transition
            Assert.That(watchdogVM.StateObservation, Is.EqualTo(WatchdogViewModel.State.Configured));

            // Expected commands can execute states and config enable flag
            Assert.That(commandCanExecuteBefore,                        Is.True);
            Assert.That(watchdogVM.ReadConfigFromASIC.CanExecute(null), Is.True);
            Assert.That(watchdogVM.WriteConfigToASIC.CanExecute(null),  Is.True);
            Assert.That(watchdogVM.StartWatchdog.CanExecute(null),      Is.True);
            Assert.That(watchdogVM.StopWatchdog.CanExecute(null),       Is.True);
            Assert.That(watchdogVM.IsConfigEnable,                      Is.True);
        }

        [Test, Order(4), Description("Check parameters in Running state")]
        [NonParallelizable]
        public void WhenWDSequenceCorrect_ThenRunningWorksExpectedly()
        {
            // Arrange
            bool commandCanExecuteBefore;
            ReceiveCommunicationPackage<EmptyPayload> receiveCommunicationPackage = new ReceiveCommunicationPackage<EmptyPayload>();
            receiveCommunicationPackage.ASICID = 1;
            receiveCommunicationPackage.Status = MCUStatus.ACK;

            ReceiveCommunicationPackage<WDStatusPayload> receiveCommunicationPackageMonitor = new ReceiveCommunicationPackage<WDStatusPayload>();
            receiveCommunicationPackageMonitor.ASICID = 1;
            receiveCommunicationPackageMonitor.Payload.Deserialize(MCUStatus.DATA, new List<byte>() {0, 0, 1});
            
            serialMock.transmittedPackages.Clear();
            serialMock.receiveCommunicationPackages.Clear();

            // Act
            serialMock.receiveCommunicationPackages.Add(receiveCommunicationPackage);
            serialMock.receiveCommunicationPackages.Add(receiveCommunicationPackageMonitor);
            commandCanExecuteBefore = watchdogVM.StartWatchdog.CanExecute(null);
            watchdogVM.StartWatchdog.Execute(null); // Emulate transition

            // Assert
            // Expected data transmitted
            Assert.That(serialMock.transmittedPackages.Count, Is.EqualTo(2));
            Assert.That(serialMock.transmittedPackages[0].Cmd, Is.EqualTo(MCUCommand.START_WATCHDOG));
            Assert.That(serialMock.transmittedPackages[1].Cmd, Is.EqualTo(MCUCommand.START_MONITORING_WATCHDOG));

            // Expected state after transition
            Assert.That(watchdogVM.StateObservation, Is.EqualTo(WatchdogViewModel.State.Running));

            // Expected commands can execute states and config enable flag
            Assert.That(commandCanExecuteBefore,                        Is.True);
            Assert.That(watchdogVM.ReadConfigFromASIC.CanExecute(null), Is.True);
            Assert.That(watchdogVM.WriteConfigToASIC.CanExecute(null),  Is.False);
            Assert.That(watchdogVM.StartWatchdog.CanExecute(null),      Is.False);
            Assert.That(watchdogVM.StopWatchdog.CanExecute(null),       Is.True);
            Assert.That(watchdogVM.IsConfigEnable,                      Is.False);

            // Expected flags (for AB12)
            Assert.That(watchdogVM.WDFaultStatus, Is.EqualTo(FaultStatus.Fault));
        }

        [Test, Order(5), Description("Check parameters after stop")]
        [NonParallelizable]
        public void WhenWDSequenceCorrect_ThenStoppingWorksExpectedly()
        {
            // Arrange
            bool commandCanExecuteBefore;
            ReceiveCommunicationPackage<EmptyPayload> receiveCommunicationPackage = new ReceiveCommunicationPackage<EmptyPayload>();
            receiveCommunicationPackage.ASICID = 1;
            receiveCommunicationPackage.Status = MCUStatus.ACK;

            ReceiveCommunicationPackage<WDStatusPayload> receiveCommunicationPackageMonitor = new ReceiveCommunicationPackage<WDStatusPayload>();
            receiveCommunicationPackageMonitor.ASICID = 1;
            receiveCommunicationPackageMonitor.Status = MCUStatus.ACK;
            serialMock.transmittedPackages.Clear();
            serialMock.receiveCommunicationPackages.Clear();
            serialMock.WaitlistItemRemoved = false;

            // Act
            serialMock.receiveCommunicationPackages.Add(receiveCommunicationPackage);
            serialMock.receiveCommunicationPackages.Add(receiveCommunicationPackageMonitor);
            commandCanExecuteBefore = watchdogVM.StopWatchdog.CanExecute(null);
            watchdogVM.StopWatchdog.Execute(null); // Emulate transition

            // Assert
            // Expected data transmitted
            Assert.That(serialMock.transmittedPackages.Count, Is.EqualTo(2));
            Assert.That(serialMock.transmittedPackages[0].Cmd, Is.EqualTo(MCUCommand.STOP_WATCHDOG));
            Assert.That(serialMock.transmittedPackages[1].Cmd, Is.EqualTo(MCUCommand.STOP_MONITORING_WATCHDOG));

            // Expected state after transition
            Assert.That(watchdogVM.StateObservation, Is.EqualTo(WatchdogViewModel.State.InConfiguration));

            // Expected commands can execute states and config enable flag
            Assert.That(commandCanExecuteBefore,                        Is.True);
            Assert.That(watchdogVM.ReadConfigFromASIC.CanExecute(null), Is.True);
            Assert.That(watchdogVM.WriteConfigToASIC.CanExecute(null),  Is.True);
            Assert.That(watchdogVM.StartWatchdog.CanExecute(null),      Is.False);
            Assert.That(watchdogVM.StopWatchdog.CanExecute(null),       Is.False);
            Assert.That(watchdogVM.IsConfigEnable,                      Is.True);

            // Check that waitlist item removal was called for stop monitoring watchdog
            Assert.That(serialMock.WaitlistItemRemoved, Is.True);
        }

        #region Mocks

        public class SerialWrapperMock : ISerialWrapper
        {
            public List<string> AvailableCOMPorts => throw new System.NotImplementedException();

            public bool IsCOMPortPresent => throw new System.NotImplementedException();

            public string? ManualComPortName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

            public void Dispose() => throw new System.NotImplementedException();

            public bool ReconnectCOMPort() => throw new System.NotImplementedException();

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
            /// Property to configure emulation of received package
            /// </summary>
            public List<IReceiveCommunicationPackage> receiveCommunicationPackages = new List<IReceiveCommunicationPackage>();

            /// <summary>
            /// Observable field for storing transmitted packages
            /// </summary>
            public List<ITransmitCommunicationPackage> transmittedPackages = new List<ITransmitCommunicationPackage>();

            public bool SerialWrite(ITransmitCommunicationPackage packageToSend)
            {
                // Store transmitted packages
                transmittedPackages.Add(packageToSend);

                // TODO: not needed?
                switch (packageToSend.Cmd)
                {
                    case MCUCommand.CONFIGURE_WATCHDOG:
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

                // Emulate recieving response from MCU
                if (receiveCommunicationPackages.Count != 0)
                {
                    packageToSend.Deleg(receiveCommunicationPackages[transmittedPackages.Count - 1]);
                }

                return true;
            }
        }

        #endregion // Mocks
    }
}