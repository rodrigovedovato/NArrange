namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the TabConfiguration class.
    /// </summary>
    [TestFixture]
    public class TabConfigurationTests
    {
        #region Methods

        /// <summary>
        /// Tests the ICloneable implementation.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            TabConfiguration tabConfiguration = new TabConfiguration();
            tabConfiguration.TabStyle = TabStyle.Tabs;
            tabConfiguration.SpacesPerTab = 8;

            TabConfiguration clone = tabConfiguration.Clone() as TabConfiguration;
            Assert.IsNotNull(clone, "Clone did not return a valid instance.");

            Assert.AreEqual(tabConfiguration.TabStyle, clone.TabStyle);
            Assert.AreEqual(tabConfiguration.SpacesPerTab, clone.SpacesPerTab);
        }

        /// <summary>
        /// Tests the creation of a new TabConfiguration.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            TabConfiguration tabConfiguration = new TabConfiguration();

            //
            // Verify default state
            //
            Assert.AreEqual(TabStyle.Spaces, tabConfiguration.TabStyle, "Unexpected default value for Style.");
            Assert.AreEqual(4, tabConfiguration.SpacesPerTab, "Unexpected default value for SpacesPerTab.");
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            TabConfiguration tabConfiguration = new TabConfiguration();
            tabConfiguration.TabStyle = TabStyle.Spaces;
            tabConfiguration.SpacesPerTab = 8;

            string str = tabConfiguration.ToString();

            Assert.AreEqual("Tabs: Spaces, 8", str, "Unexpected string representation.");
        }

        #endregion Methods
    }
}