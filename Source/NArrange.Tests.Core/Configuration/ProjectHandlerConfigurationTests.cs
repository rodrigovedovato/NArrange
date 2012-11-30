namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ProjectHandlerConfiguration class.
    /// </summary>
    [TestFixture]
    public class ProjectHandlerConfigurationTests
    {
        #region Methods

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            ProjectHandlerConfiguration handlerConfiguration = new ProjectHandlerConfiguration();
            handlerConfiguration.ParserType = "TestAssembly.TestParser";

            string str = handlerConfiguration.ToString();
            Assert.AreEqual("Project Handler: TestAssembly.TestParser", str, "Unexpected string representation.");
        }

        #endregion Methods
    }
}