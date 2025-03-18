using System;
using FlaUI.Core.Conditions;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System.IO;
using System.Text.RegularExpressions;
using Window = FlaUI.Core.AutomationElements.Window;
using Application = FlaUI.Core.Application;

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
        /// Collection of expected logging levels
        /// </summary>
        private string[] logLevels = { "TRACE", "DEBUG", "INFO", "WARN", "ERROR", "FATAL" };

        /// <summary>
        /// Set up test environment. This method is called only once before running tests
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            // Get build type of Regression project - will be used for determining what WPF build type to test
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
            // Arrange
            if (mainView != null)
            {
                Button? undockButton = mainView.FindFirstDescendant(cf.ByAutomationId("UndockLoggerButton"))?.AsButton();
                if (undockButton != null)
                {
                    undockButton.Click();
                }
            }

            // Act
            Window[] allWindows = app.GetAllTopLevelWindows(automation);
            foreach (Window window in allWindows)
            {
                if (window.Title == "Logger Window")
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
            // Arrange
            int iterationsCount;
            string dateTimePattern;

            CheckBox? formatSwitch = loggerView.FindFirstDescendant(cf.ByAutomationId("RecordFormatToggle"))?.AsCheckBox();
            ListBox? listView = loggerView.FindFirstDescendant(cf.ByAutomationId("LogTable"))?.AsListBox();
            AutomationElement[]? listViewItems = listView?.FindAllChildren();

            // Act
            iterationsCount = (listViewItems.Length > 10) ? 10 : listViewItems.Length;
            dateTimePattern = (bool) formatSwitch.IsChecked ? (@"\d{4}-\d{2}-d{2}T\d{2}:\d{2}:\d\{2}\.\d{3}") : (@"\d{2}:\d{2}:\d{2}");

            // Assert
            for (int i = 0; i < iterationsCount; i++)
            {
                AutomationElement[] elementsOfRecord = listViewItems[i].AsListBoxItem().FindAllChildren();

                // Check number of fields
                Assert.That(elementsOfRecord.Length, Is.EqualTo(4));

                // Check that datetime column mathces expected pattern
                Assert.That(Regex.IsMatch(elementsOfRecord[0].Name, dateTimePattern));

                // Check if log level is from expected range
                Assert.That(elementsOfRecord[1].Name, Is.AnyOf(logLevels));

                // Check that column with numbers contains numbers
                Assert.That(int.Parse(elementsOfRecord[2].Name), Is.TypeOf<int>());

                // Check that message colum is not empty
                Assert.That(elementsOfRecord[3].Name, Is.Not.Null);

            }
        }
    }
}