using System;
using AB15_GUI.WPF.Models.Genereted.Registers;

namespace AB15_GUI.Tests.Models.Genereted.Registers
{
    /// <summary>
    /// Checking that ident_high register class was generated expectedly
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-118" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-118" />
    /// </tc_links>
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class Reg_ident_high_Tests
    {
        // Arrange constants for tests
        private const UInt16 ExpectedResetValue = 0x241;
        private const string ExpectedName = "ident_high";
        private const UInt16 ExpectedAddress = 0x0000;
        private const string ExpectedDescription = "Letters in chip identifier";
        private const string ExpectedAccess = "read-only";

        private const string ExpectedLetter1Name = "letter1";
        private const string ExpectedLetter1Description = "1st letter of chip identifier, for RA150 it's        ' '";
        private const UInt16 ExpectedLetter1BitOffset = 10;
        private const UInt16 ExpectedLetter1BitWidth = 5;

        private const string ExpectedLetter2Name = "letter2";
        private const string ExpectedLetter2Description = "2nd letter of chip identifier, for RA150 it's        'R': 0x12";
        private const UInt16 ExpectedLetter2BitOffset = 5;
        private const UInt16 ExpectedLetter2BitWidth = 5;

        private const string ExpectedLetter3Name = "letter3";
        private const string ExpectedLetter3Description = "3rd letter of chip identifier, for RA150 it's        'A': 0x01";
        private const UInt16 ExpectedLetter3BitOffset = 0;
        private const UInt16 ExpectedLetter3BitWidth = 5;

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

        [Test, Description("Check that properties in ident_high are expected")]
        public void WhenRegisterIsInstantiated_PropertiesHaveExpectedValues()
        {
            // Arrange
            var register = new Reg_ident_high();

            // Act

            // Assert
            Assert.That(register.ResetValue, Is.EqualTo(ExpectedResetValue), "ResetValue should be initialized correctly.");
            Assert.That(register.Name, Is.EqualTo(ExpectedName), "Name should be initialized correctly.");
            Assert.That(register.Address, Is.EqualTo(ExpectedAddress), "Address should be initialized correctly.");
            Assert.That(register.Description, Is.EqualTo(ExpectedDescription), "Description should be initialized correctly.");
            Assert.That(register.Access, Is.EqualTo(ExpectedAccess), "Access should be initialized correctly.");
        }

        [Test, Description("Check that constants in Letter1 are expected")]
        public void WhenRegisterIsInstantiated_Letter1PropertiesHaveExpectedValues()
        {
            // Arrange
            var register = new Reg_ident_high();
            var letter1 = register.letter1;

            // Act

            // Assert
            Assert.That(letter1.Name, Is.EqualTo(ExpectedLetter1Name), "Letter1 Name should be initialized correctly.");
            Assert.That(letter1.Description, Is.EqualTo(ExpectedLetter1Description), "Letter1 Description should be initialized correctly.");
            Assert.That(letter1.BitOffset, Is.EqualTo(ExpectedLetter1BitOffset), "Letter1 BitOffset should be initialized correctly.");
            Assert.That(letter1.BitWidth, Is.EqualTo(ExpectedLetter1BitWidth), "Letter1 BitWidth should be initialized correctly.");
        }

        [Test, Description("Check that constants in Letter2 are expected")]
        public void WhenRegisterIsInstantiated_Letter2PropertiesHaveExpectedValues()
        {
            // Arrange
            var register = new Reg_ident_high();
            var letter2 = register.letter2;

            // Act

            // Assert
            Assert.That(letter2.Name, Is.EqualTo(ExpectedLetter2Name), "Letter2 Name should be initialized correctly.");
            Assert.That(letter2.Description, Is.EqualTo(ExpectedLetter2Description), "Letter2 Description should be initialized correctly.");
            Assert.That(letter2.BitOffset, Is.EqualTo(ExpectedLetter2BitOffset), "Letter2 BitOffset should be initialized correctly.");
            Assert.That(letter2.BitWidth, Is.EqualTo(ExpectedLetter2BitWidth), "Letter2 BitWidth should be initialized correctly.");
        }

        [Test, Description("Check that constants in Letter3 are expected")]
        public void WhenRegisterIsInstantiated_Letter3PropertiesHaveExpectedValues()
        {
            // Arrange
            var register = new Reg_ident_high();
            var letter3 = register.letter3;

            // Act

            // Assert
            Assert.That(letter3.Name, Is.EqualTo(ExpectedLetter3Name), "Letter3 Name should be initialized correctly.");
            Assert.That(letter3.Description, Is.EqualTo(ExpectedLetter3Description), "Letter3 Description should be initialized correctly.");
            Assert.That(letter3.BitOffset, Is.EqualTo(ExpectedLetter3BitOffset), "Letter3 BitOffset should be initialized correctly.");
            Assert.That(letter3.BitWidth, Is.EqualTo(ExpectedLetter3BitWidth), "Letter3 BitWidth should be initialized correctly.");
        }

        [Test, Description("Check Data property get method")]
        [TestCase((UInt16)0b00001, (UInt16)0b010010, (UInt16)0b00001, ExpectedResult = (UInt16)0b011001000001)] // R:0x12, A:0x01
        [TestCase((UInt16)0b00010, (UInt16)0b011000, (UInt16)0b00010, ExpectedResult = (UInt16)0b101100000010)] // arbitrary values
        public UInt16 WhenFieldsAreSet_ThenRegisterDataIsCorrect(UInt16 letter1, UInt16 letter2, UInt16 letter3)
        {
            // Arrange
            var register = new Reg_ident_high();
            register.letter1.Data = letter1;
            register.letter2.Data = letter2;
            register.letter3.Data = letter3;

            // Act
            return register.Data;

            // Assert
            // Done implicitly comparing ExpectedResult with return value
        }

        [Test, Description("Check Data property set method")]
        [TestCase((UInt16)0b011001000001, (UInt16)0b00001, (UInt16)0b010010, (UInt16)0b00001)] // R:0x12, A:0x01
        [TestCase((UInt16)0b101100000010, (UInt16)0b00010, (UInt16)0b011000, (UInt16)0b00010)] // arbitrary values
        public void WhenRegisterDataIsSet_ThenFieldValuesAreExpected(UInt16 data, UInt16 expectedLetter1, UInt16 expectedLetter2, UInt16 expectedLetter3)
        {
            // Arrange
            var register = new Reg_ident_high();

            // Act
            register.Data = data;

            // Assert
            Assert.That(expectedLetter1, Is.EqualTo(register.letter1.Data), "Letter1 data should be set correctly.");
            Assert.That(expectedLetter2, Is.EqualTo(register.letter2.Data), "Letter2 data should be set correctly.");
            Assert.That(expectedLetter3, Is.EqualTo(register.letter3.Data), "Letter3 data should be set correctly.");
        }

        [Test, Description("Check OutOfRange exception for field data")]
        [TestCase((UInt16)0b111111)] // Exceeding bit width for letter1
        public void WhenLetter1DataOutOfRange_ThenExpectedExceptionIsRaised(UInt16 invalidData)
        {
            // Arrange
            var letter1 = new Reg_ident_high.Field_letter1();

            // Act
            ArgumentOutOfRangeException? ex = Assert.Throws<ArgumentOutOfRangeException>(() => letter1.Data = invalidData);

            // Assert
            Assert.That(ex.Message, Does.Contain("Expected max value of"));
        }
    }
}