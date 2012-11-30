namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the RegionElement class.
    /// </summary>
    [TestFixture]
    public class RegionElementTests : CodeElementTests<RegionElement>
    {
        #region Methods

        /// <summary>
        /// Tests constructing a new RegionElement.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            RegionElement element = new RegionElement();

            //
            // Verify default values
            //
            Assert.AreEqual(ElementType.Region, element.ElementType, "Unexpected element type.");
            Assert.AreEqual(string.Empty, element.Name, "Unexpected default value for Name.");
            Assert.IsNotNull(element.Children, "Children collection should not be null.");
            Assert.AreEqual(0, element.Children.Count, "Children collection should be empty.");
            Assert.IsTrue(element.DirectivesEnabled, "Directives should be enabled by default.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected override RegionElement DoCreateClonePrototype()
        {
            RegionElement prototype = new RegionElement();
            prototype.Name = "Test Region";
            prototype.DirectivesEnabled = false;

            return prototype;
        }

        /// <summary>
        /// Test for ToString()
        /// </summary>
        protected override void DoToStringTest()
        {
            NamespaceElement element = new NamespaceElement();
            element.Name = "Test";

            string str = element.ToString();
            Assert.AreEqual("Test", str, "Unexpected value returned for ToString.");
        }

        /// <summary>
        /// Verifies that a clone has the same state as the original.
        /// </summary>
        /// <param name="original">Original element.</param>
        /// <param name="clone">Clone element.</param>
        protected override void DoVerifyClone(RegionElement original, RegionElement clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            Assert.AreEqual(original.DirectivesEnabled, clone.DirectivesEnabled);
        }

        #endregion Methods
    }
}