namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the UsingConfiguration class.
    /// </summary>
    [TestFixture]
    public class UsingConfigurationTests
    {
        #region Methods

        /// <summary>
        /// Tests the ICloneable implementation.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            UsingConfiguration usingConfiguration = new UsingConfiguration();
            usingConfiguration.MoveTo = CodeLevel.Namespace;

            UsingConfiguration clone = usingConfiguration.Clone() as UsingConfiguration;
            Assert.IsNotNull(clone, "Clone did not return a valid instance.");

            Assert.AreEqual(
                usingConfiguration.MoveTo,
                clone.MoveTo);
        }

        /// <summary>
        /// Tests the creation of a new UsingConfiguration.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            UsingConfiguration usingConfiguration = new UsingConfiguration();

            //
            // Verify default state
            //
            Assert.AreEqual(
                CodeLevel.None,
                usingConfiguration.MoveTo,
                "Unexpected default value for MoveTo.");
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            UsingConfiguration usingConfiguration = new UsingConfiguration();
            usingConfiguration.MoveTo = CodeLevel.File;

            string str = usingConfiguration.ToString();

            Assert.AreEqual("Usings: MoveTo - File", str, "Unexpected string representation.");
        }

        #endregion Methods
    }
}