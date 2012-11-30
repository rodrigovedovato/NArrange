namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the FormattingConfiguration class.
    /// </summary>
    [TestFixture]
    public class FormattingConfigurationTests
    {
        #region Methods

        /// <summary>
        /// Tests the ICloneable implementation.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            FormattingConfiguration formattingConfiguration = new FormattingConfiguration();

            formattingConfiguration.ClosingComments.Format = "XXX";
            formattingConfiguration.Regions.CommentDirectiveBeginFormat = "YYY";
            formattingConfiguration.Tabs.SpacesPerTab = 100;
            formattingConfiguration.LineSpacing.RemoveConsecutiveBlankLines = true;
            formattingConfiguration.Usings.MoveTo = CodeLevel.File;

            FormattingConfiguration clone = formattingConfiguration.Clone() as FormattingConfiguration;
            Assert.IsNotNull(clone, "Clone did not return a valid instance.");

            Assert.AreEqual(
                formattingConfiguration.ClosingComments.Format,
                clone.ClosingComments.Format);
            Assert.AreEqual(
                formattingConfiguration.Regions.CommentDirectiveBeginFormat,
                clone.Regions.CommentDirectiveBeginFormat);
            Assert.AreEqual(
                formattingConfiguration.Tabs.SpacesPerTab,
                clone.Tabs.SpacesPerTab);
            Assert.AreEqual(
                formattingConfiguration.LineSpacing.RemoveConsecutiveBlankLines,
                clone.LineSpacing.RemoveConsecutiveBlankLines);
            Assert.AreEqual(
                formattingConfiguration.Usings.MoveTo,
                clone.Usings.MoveTo);
        }

        /// <summary>
        /// Tests the creation of a new FormattingConfiguration.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            FormattingConfiguration formattingConfiguration = new FormattingConfiguration();

            //
            // Verify default state
            //
            Assert.IsNotNull(formattingConfiguration.ClosingComments, "Expected an instance for ClosingComments");
            Assert.IsNotNull(formattingConfiguration.Regions, "Expected an instance for Regions");
            Assert.IsNotNull(formattingConfiguration.Tabs, "Expected an instance for Tabs");
            Assert.IsNotNull(formattingConfiguration.LineSpacing, "Expected an instance for LineSpacing");
            Assert.IsNotNull(formattingConfiguration.Usings, "Expected an instance Usings.");
        }

        #endregion Methods
    }
}