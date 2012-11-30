namespace NArrange.Tests.Core
{
    using System;

    using NArrange.Core;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the SolutionParser class.
    /// </summary>
    [TestFixture]
    public class SolutionParserTests
    {
        #region Methods

        /// <summary>
        /// Tests the extensions property.
        /// </summary>
        [Test]
        public void ExtensionsTest()
        {
            Assert.IsTrue(SolutionParser.Instance.Extensions.Contains("sln"));
            Assert.IsTrue(SolutionParser.Instance.Extensions.Contains("mds"));
        }

        /// <summary>
        /// Tests the singleton instance.
        /// </summary>
        [Test]
        public void InstanceTest()
        {
            SolutionParser instance = SolutionParser.Instance;
            Assert.AreSame(instance, SolutionParser.Instance);

            Assert.IsNotNull(instance.Extensions);
        }

        /// <summary>
        /// Tests the IsSolution method.
        /// </summary>
        [Test]
        public void IsSolutionTest()
        {
            Assert.IsTrue(SolutionParser.Instance.IsSolution("test.sln"));
            Assert.IsTrue(SolutionParser.Instance.IsSolution("test.SLN"));
            Assert.IsTrue(SolutionParser.Instance.IsSolution("test.mds"));
            Assert.IsTrue(SolutionParser.Instance.IsSolution("test.MDS"));
        }

        /// <summary>
        /// Tests parsing a null filename.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseNullTest()
        {
            SolutionParser.Instance.Parse(null);
        }

        #endregion Methods
    }
}