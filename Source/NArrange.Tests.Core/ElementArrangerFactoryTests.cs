namespace NArrange.Tests.Core
{
    using System;

    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ElementArrangerFactory class.
    /// </summary>
    [TestFixture]
    public class ElementArrangerFactoryTests
    {
        #region Methods

        /// <summary>
        /// Tests calling CreateElementArranger with a null configuration.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateElementArrangerNullConfigurationTest()
        {
            ElementArrangerFactory.CreateElementArranger(null, new ElementConfiguration());
        }

        #endregion Methods
    }
}