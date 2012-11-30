namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ElementConfiguration class.
    /// </summary>
    [TestFixture]
    public class ElementConfigurationTests
    {
        #region Methods

        /// <summary>
        /// Tests the ICloneable implementation.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            ElementConfiguration prototype = new ElementConfiguration();
            prototype.ElementType = ElementType.Delegate;
            prototype.Id = "Test";

            FilterBy filterBy = new FilterBy();
            filterBy.Condition = "$(Name) == 'Test'";
            prototype.FilterBy = filterBy;

            GroupBy groupBy = new GroupBy();
            groupBy.By = ElementAttributeType.Access;
            prototype.GroupBy = groupBy;

            SortBy sortBy = new SortBy();
            sortBy.By = ElementAttributeType.Name;
            prototype.SortBy = sortBy;

            ElementConfiguration clone = prototype.Clone() as ElementConfiguration;
            Assert.IsNotNull(clone, "Clone did not return an instance.");

            Assert.AreEqual(prototype.ElementType, clone.ElementType, "ElementType was not cloned correctly.");
            Assert.AreEqual(prototype.Id, clone.Id, "Id was not cloned correctly.");

            Assert.AreEqual(prototype.FilterBy.Condition, clone.FilterBy.Condition, "FilterBy was not cloned correctly.");
            Assert.AreEqual(prototype.GroupBy.By, clone.GroupBy.By, "GroupBy was not cloned correctly.");
            Assert.AreEqual(prototype.SortBy.By, clone.SortBy.By, "SortBy was not cloned correctly.");
        }

        /// <summary>
        /// Tests the creation of a new ElementConfiguration.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            ElementConfiguration elementConfiguration = new ElementConfiguration();

            //
            // Verify default state
            //
            Assert.AreEqual(ElementType.NotSpecified, elementConfiguration.ElementType, "Unexpected default value for ElementType.");
            Assert.IsNull(elementConfiguration.Id, "Unexpected default value for Id.");
            Assert.IsNull(elementConfiguration.FilterBy, "Unexpected default value for FilterBy.");
            Assert.IsNull(elementConfiguration.GroupBy, "Unexpected default value for GroupBy.");
            Assert.IsNull(elementConfiguration.SortBy, "Unexpected default value for SortBy.");
            Assert.IsNotNull(elementConfiguration.Elements, "Elements collection should not be null.");
            Assert.AreEqual(0, elementConfiguration.Elements.Count, "Elements collection should be empty.");
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            ElementConfiguration elementConfiguration = new ElementConfiguration();
            elementConfiguration.ElementType = ElementType.Method;

            string str = elementConfiguration.ToString();

            Assert.AreEqual("Element: Type - Method", str, "Unexpected string representation.");
        }

        #endregion Methods
    }
}