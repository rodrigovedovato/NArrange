namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the SourceHandlerConfiguration class.
    /// </summary>
    [TestFixture]
    public class SourceHandlerConfigurationTests
    {
        #region Methods

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            SourceHandlerConfiguration handlerConfiguration = new SourceHandlerConfiguration();
            handlerConfiguration.Language = "TestLanguage";

            string str = handlerConfiguration.ToString();
            Assert.AreEqual("Source Handler: TestLanguage", str, "Unexpected string representation.");
        }

        #endregion Methods
    }
}