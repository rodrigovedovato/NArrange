namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ExtensionConfiguration class.
    /// </summary>
    [TestFixture]
    public class ExtensionConfigurationTests
    {
        #region Methods

        /// <summary>
        /// Tests the ICloneable implementation.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            ExtensionConfiguration extensionConfiguration = new ExtensionConfiguration();
            extensionConfiguration.Name = "cs";

            FilterBy filter = new FilterBy();
            filter.Condition = "$(File.Name) != 'Test.cs'";
            extensionConfiguration.FilterBy = filter;

            ExtensionConfiguration clone = extensionConfiguration.Clone() as ExtensionConfiguration;
            Assert.IsNotNull(clone, "Clone did not return a valid instance.");

            Assert.AreEqual(extensionConfiguration.Name, clone.Name);
            Assert.IsNotNull(clone.FilterBy, "FilterBy was not cloned.");
            Assert.AreEqual(extensionConfiguration.FilterBy.Condition, clone.FilterBy.Condition);
        }

        /// <summary>
        /// Tests the creation of a new ExtensionConfiguration.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            ExtensionConfiguration extensionConfiguration = new ExtensionConfiguration();

            //
            // Verify default state
            //
            Assert.IsNull(extensionConfiguration.Name, "Unexpected default value for Name.");
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            ExtensionConfiguration extensionConfiguration = new ExtensionConfiguration();
            extensionConfiguration.Name = "cs";

            string str = extensionConfiguration.ToString();

            Assert.AreEqual("Extension: cs", str, "Unexpected string representation.");
        }

        #endregion Methods
    }
}