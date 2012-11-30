namespace NArrange.Tests.Core
{
    using System;

    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the SourceHandler class.
    /// </summary>
    [TestFixture]
    public class SourceHandlerTests
    {
        #region Methods

        /// <summary>
        /// Tests creating a new extension handler.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            string assemblyName = "NArrange.CSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            SourceHandlerConfiguration configuration = new SourceHandlerConfiguration();
            configuration.AssemblyName = assemblyName;

            SourceHandler handler = new SourceHandler(configuration);

            Assert.IsNotNull(handler.CodeParser, "Parser was not created.");
            Assert.IsNotNull(handler.CodeWriter, "Writer was not created.");
        }

        /// <summary>
        /// Tests creating with a null configuration.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateWithNullConfigurationTest()
        {
            new SourceHandler(null);
        }

        #endregion Methods
    }
}