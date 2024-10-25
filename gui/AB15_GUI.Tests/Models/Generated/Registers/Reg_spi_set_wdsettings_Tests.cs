using System;
using AB15_GUI.WPF.Models.Generated.Registers;

namespace AB15_GUI.Tests.Models.Generated.Registers
{
    /// <summary>
    /// Checking that spi_set_wdsettings register class was generated expectedly
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-118" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-118" />
    /// </tc_links>
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class Reg_spi_set_wdsettings_Tests
    {
        // Arrange constants for tests
        private const UInt16 ExpectedResetValue = 0x0000;
        private const string ExpectedName = "spi_set_wdsettings";
        private const UInt16 ExpectedAddress = 0x003D;
        private const string ExpectedDescription = "Watchdog settings register. ";
        private const string ExpectedAccess = "read-write";

        /// <summary>
        /// Set up test environment
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
        }

        /// <summary>
        /// Clean up environment after finishing tests
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
        }

        [Test, Description("Check that properties in spi_set_wdsettings are expected")]
        public void WhenRegisterIsInstantiated_PropertiesHaveExpectedValues()
        {
            // Arrange
            var register = new Reg_spi_set_wdsettings();

            // Act

            // Assert
            Assert.That(register.ResetValue, Is.EqualTo(ExpectedResetValue), "ResetValue should be initialized correctly.");
            Assert.That(register.Name, Is.EqualTo(ExpectedName), "Name should be initialized correctly.");
            Assert.That(register.Address, Is.EqualTo(ExpectedAddress), "Address should be initialized correctly.");
            Assert.That(register.Description, Is.EqualTo(ExpectedDescription), "Description should be initialized correctly.");
            Assert.That(register.Access, Is.EqualTo(ExpectedAccess), "Access should be initialized correctly.");
        }
    }
}
