namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the RegionFormattingConfiguration class.
    /// </summary>
    [TestFixture]
    public class RegionFormatConfigurationTests
    {
        #region Methods

        /// <summary>
        /// Tests the ICloneable implementation.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            RegionFormatConfiguration regionsConfiguration = new RegionFormatConfiguration();
            regionsConfiguration.EndRegionNameEnabled = true;

            RegionFormatConfiguration clone = regionsConfiguration.Clone() as RegionFormatConfiguration;
            Assert.IsNotNull(clone, "Clone did not return a valid instance.");

            Assert.AreEqual(regionsConfiguration.EndRegionNameEnabled, clone.EndRegionNameEnabled);
        }

        /// <summary>
        /// Tests the creation of a new RegionsConfiguration.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            RegionFormatConfiguration regionsConfiguration = new RegionFormatConfiguration();

            //
            // Verify default state
            //
            Assert.IsTrue(regionsConfiguration.EndRegionNameEnabled, "Unexpected default value for EndRegionNameEnabled.");
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            RegionFormatConfiguration regionsConfiguration = new RegionFormatConfiguration();
            regionsConfiguration.EndRegionNameEnabled = true;

            string str = regionsConfiguration.ToString();

            Assert.AreEqual("Regions: EndRegionNameEnabled - True", str, "Unexpected string representation.");
        }

        #endregion Methods
    }
}