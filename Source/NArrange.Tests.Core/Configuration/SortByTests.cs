namespace NArrange.Tests.Core.Configuration
{
    using System.ComponentModel;

    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the SortBy class.
    /// </summary>
    [TestFixture]
    public class SortByTests
    {
        #region Methods

        /// <summary>
        /// Tests cloning the configuration element.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Name;
            sortBy.Direction = SortDirection.Descending;

            SortBy innerSortBy = new SortBy();
            innerSortBy.By = ElementAttributeType.Type;
            innerSortBy.Direction = SortDirection.Ascending;
            sortBy.InnerSortBy = innerSortBy;

            SortBy clone = sortBy.Clone() as SortBy;
            Assert.AreEqual(sortBy.By, clone.By, "By was not copied correctly");
            Assert.AreEqual(sortBy.Direction, clone.Direction, "Direction was not copied correctly");
            Assert.IsNotNull(clone.InnerSortBy, "InnerSortBy was not copied correctly");
            Assert.AreEqual(sortBy.InnerSortBy.By, clone.InnerSortBy.By, "InnerSortBy was not copied correctly");
            Assert.AreEqual(sortBy.InnerSortBy.Direction, clone.InnerSortBy.Direction, "InnerSortBy was not copied correctly");
        }

        /// <summary>
        /// Tests the creation of a new SortBy.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            SortBy sortBy = new SortBy();

            //
            // Verify default state
            //
            Assert.AreEqual(ElementAttributeType.None, sortBy.By, "Unexpected default value for By.");
            Assert.AreEqual(SortDirection.Ascending, sortBy.Direction, "Unexpected default value for Direction.");
            Assert.IsNull(sortBy.InnerSortBy, "Unexpected default value for InnerSortBy.");
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Name;

            string str = sortBy.ToString();

            Assert.AreEqual("Sort by: Name", str, "Unexpected string representation.");
        }

        #endregion Methods
    }
}