namespace NArrange.Tests.Core
{
    using System;

    using NArrange.Core;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ProjectManager class.
    /// </summary>
    [TestFixture]
    public class ProjectManagerTests
    {
        #region Methods

        /// <summary>
        /// Tests creating a new ProjectManager with a null configuration.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConfigurationTest()
        {
            ProjectManager projectManager = new ProjectManager(null);
        }

        #endregion Methods
    }
}