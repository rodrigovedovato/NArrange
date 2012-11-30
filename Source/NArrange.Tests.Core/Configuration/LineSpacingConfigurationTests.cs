namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the LineSpacingConfiguration class.
    /// </summary>
    [TestFixture]
    public class LineSpacingConfigurationTests
    {
        #region Methods

        /// <summary>
        /// Tests the ICloneable implementation.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            LineSpacingConfiguration lineSpacingConfiguration = new LineSpacingConfiguration();
            lineSpacingConfiguration.RemoveConsecutiveBlankLines = true;

            LineSpacingConfiguration clone = lineSpacingConfiguration.Clone() as LineSpacingConfiguration;
            Assert.IsNotNull(clone, "Clone did not return a valid instance.");

            Assert.AreEqual(
                lineSpacingConfiguration.RemoveConsecutiveBlankLines,
                clone.RemoveConsecutiveBlankLines);
        }

        /// <summary>
        /// Tests the creation of a new LineSpacingConfiguration.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            LineSpacingConfiguration lineSpacingConfiguration = new LineSpacingConfiguration();

            //
            // Verify default state
            //
            Assert.IsFalse(
                lineSpacingConfiguration.RemoveConsecutiveBlankLines,
                "Unexpected default value for RemoveConsecutiveBlankLines.");
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            LineSpacingConfiguration lineSpacingConfiguration = new LineSpacingConfiguration();
            lineSpacingConfiguration.RemoveConsecutiveBlankLines = true;

            string str = lineSpacingConfiguration.ToString();

            Assert.AreEqual("LineSpacing: RemoveConsecutiveBlankLines - True", str, "Unexpected string representation.");
        }

        #endregion Methods
    }
}