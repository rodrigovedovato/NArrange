namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the RegionConfiguration class.
    /// </summary>
    [TestFixture]
    public class RegionConfigurationTests
    {
        #region Methods

        /// <summary>
        /// Tests the ICloneable implementation.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            RegionConfiguration regionConfiguration = new RegionConfiguration();
            regionConfiguration.Name = "Some Region";
            regionConfiguration.DirectivesEnabled = false;

            RegionConfiguration clone = regionConfiguration.Clone() as RegionConfiguration;
            Assert.IsNotNull(clone, "Clone did not return a valid instance.");

            Assert.AreEqual(regionConfiguration.Name, clone.Name);
            Assert.AreEqual(regionConfiguration.DirectivesEnabled, clone.DirectivesEnabled);
        }

        /// <summary>
        /// Tests the creation of a new RegionConfiguration.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            RegionConfiguration regionConfiguration = new RegionConfiguration();

            //
            // Verify default state
            //
            Assert.IsNull(regionConfiguration.Name, "Unexpected default value for Name.");
            Assert.IsNotNull(regionConfiguration.Elements, "Elements collection should not be null.");
            Assert.AreEqual(0, regionConfiguration.Elements.Count, "Elements collection should be empty.");
            Assert.IsTrue(regionConfiguration.DirectivesEnabled, "Directives should be enabled.");
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            RegionConfiguration regionConfiguration = new RegionConfiguration();
            regionConfiguration.Name = "Test Region";

            string str = regionConfiguration.ToString();

            Assert.AreEqual("Region: Test Region", str, "Unexpected string representation.");
        }

        #endregion Methods
    }
}