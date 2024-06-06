using System;
using NUnit.Framework;
using FlaUI.Core;
using FlaUI.Core.Conditions;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System.IO;

namespace AB15_GUI.Regression.Requirements
{
    /// <summary>
    /// Logger tests. Tests are ordered and executed sequentially
    /// Warning: tests prerequisites should be fulfilled before running these tests
    /// </summary>
    /// <prerequisites>
    ///     1) AB15_GUI.WPF source code is up to date and can be built
    /// </prerequisites>
    /// <tc_links>
    ///     <link ID="ABEVBSW-64" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-64" />
    /// </tc_links>
    [TestFixture, NonParallelizable, Order(1)]
    public class Logger_Tests
    {
        /// <summary>
        /// Application under test handle
        /// </summary>
        private Application app;

        /// <summary>
        /// Condition factory instance. Required for conditional search of GUI elements
        /// </summary>
        private ConditionFactory cf;

        /// <summary>
        /// Automation instance
        /// </summary>
        private UIA3Automation automation;

        /// <summary>
        /// Reference to MainView of GUI
        /// </summary>
        private Window? mainView;

        /// <summary>
        /// Reference to LoggerView of GUI
        /// </summary>
        private Window? loggerView = null;

        /// <summary>
        /// Set up test environment. This method is called only once before running tests
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            // Get build type of Regression project - will be used for determinig what WPF build type to test
            string buildType;
            #if DEBUG
                buildType = "Debug";
            #elif RELEASE
                buildType = "Release";
            #else
                throw new ValueError("Unexpected build type.");
            #endif

            // Command line arguments (for modifying logging level)
            string cmdLineArgs = "-loggerTests true";

            // Construct path to executable
            string pathToExe = Path.Join(Environment.CurrentDirectory.Split("AB15_GUI.Regression")[0], @"AB15_GUI.WPF\bin\", buildType, @"net8.0-windows\AB15_GUI.WPF.exe");

            // Open application under test
            app = Application.Launch(pathToExe, cmdLineArgs);

            // Instantiate condition factory
            cf = new ConditionFactory(new UIA3PropertyLibrary());

            // Create automation class instance
            automation = new UIA3Automation();

            // Get reference to main view
            mainView = app.GetMainWindow(automation);
        }

        /// <summary>
        /// Clean up environment after finishing tests
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
            app.Close();
            automation.Dispose();
        }

        /// <summary>
        /// Checking that logger window is opened
        /// This test if passed will initialize loggerView reference
        /// </summary>
        [Test, Order(1)]
        [NonParallelizable]
        public void WhenLoggerIsPromptedToOpen_ThenLoggerOpensSuccessfully() 
        {
            // TODO: add when functionality for opening logger will be available on main view
            // TODO: 1) Should apply control action to open logger window (like button click)

            // Arrange

            // Act
            Window[] allWindows = app.GetAllTopLevelWindows(automation);
            foreach (Window window in allWindows)
            {
                if (window.Title == "LoggerWindow")
                {
                    loggerView = window;
                    break;
                }
            }

            // Assert
            Assert.That(loggerView, Is.Not.Null);
        }

        /// <summary>
        /// Checking that records in table are in expected format
        /// </summary>
        [Test, Order(2)]
        [NonParallelizable]
        public void WhenMessagesArePresent_ThenItemFormatIsExpected()
        {
            // TODO: add when functionality for opening logger will be available on main view
            // TODO: 1) Should apply control action to open logger window (like button click)

            // Arrange
            var test = loggerView.FindAllDescendants();
            CheckBox? formatSwitch = loggerView.FindFirstDescendant(cf.ByName("RecordFormatToggle"))?.AsCheckBox();
            //var formatText = loggerView.FindFirstDescendant(cf.ByAutomationId)

            // Act
            Window[] allWindows = app.GetAllTopLevelWindows(automation);
            foreach (Window window in allWindows)
            {
                if (window.Title == "LoggerWindow")
                {
                    loggerView = window;
                    break;
                }
            }

            // Assert
            Assert.That(loggerView, Is.Not.Null);
        }
    }
}