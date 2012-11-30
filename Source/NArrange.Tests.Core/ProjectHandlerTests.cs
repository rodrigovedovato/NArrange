namespace NArrange.Tests.Core
{
    using System;

    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ProjectHandler class.
    /// </summary>
    [TestFixture]
    public class ProjectHandlerTests
    {
        #region Methods

        /// <summary>
        /// Tests creating a new project handler.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            ProjectHandlerConfiguration configuration = new ProjectHandlerConfiguration();
            configuration.ParserType = "NArrange.Core.MonoDevelopProjectParser";

            ProjectHandler handler = new ProjectHandler(configuration);

            Assert.IsNotNull(handler.ProjectParser, "Project parser was not created.");
            Assert.IsInstanceOfType(typeof(MonoDevelopProjectParser), handler.ProjectParser);
        }

        /// <summary>
        /// Tests creating a new project handler.
        /// </summary>
        [Test]
        public void CreateWithAssemblyTest()
        {
            string assemblyName = "NArrange.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            ProjectHandlerConfiguration configuration = new ProjectHandlerConfiguration();
            configuration.AssemblyName = assemblyName;
            configuration.ParserType = "NArrange.Core.MSBuildProjectParser";

            ProjectHandler handler = new ProjectHandler(configuration);

            Assert.IsNotNull(handler.ProjectParser, "Project parser was not created.");
            Assert.IsInstanceOfType(typeof(MSBuildProjectParser), handler.ProjectParser);
        }

        /// <summary>
        /// Tests creating a project handler with a default configuration.
        /// </summary>
        [Test]
        public void CreateWithDefaultConfigurationTest()
        {
            ProjectHandlerConfiguration configuration = new ProjectHandlerConfiguration();
            ProjectHandler projectHandler = new ProjectHandler(configuration);

            Assert.IsNotNull(projectHandler.ProjectParser, "Expected a project parser instance.");
            Assert.IsInstanceOfType(typeof(MSBuildProjectParser), projectHandler.ProjectParser);
        }

        /// <summary>
        /// Tests creating with a null configuration.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateWithNullConfigurationTest()
        {
            new ProjectHandler(null);
        }

        #endregion Methods
    }
}