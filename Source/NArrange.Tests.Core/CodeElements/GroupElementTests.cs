namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the GroupElement class.
    /// </summary>
    [TestFixture]
    public class GroupElementTests : CodeElementTests<GroupElement>
    {
        #region Methods

        /// <summary>
        /// Tests constructing a new GroupElement.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            GroupElement element = new GroupElement();

            //
            // Verify default values
            //
            Assert.AreEqual(ElementType.NotSpecified, element.ElementType, "Unexpected element type.");
            Assert.AreEqual(string.Empty, element.Name, "Unexpected default value for Name.");
            Assert.IsNotNull(element.Children, "Children collection should not be null.");
            Assert.AreEqual(0, element.Children.Count, "Children collection should be empty.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected override GroupElement DoCreateClonePrototype()
        {
            GroupElement prototype = new GroupElement();
            prototype.Name = "Test Group";
            prototype.SeparatorType = GroupSeparatorType.Custom;
            prototype.CustomSeparator = "//\r\n// Some elements\r\n//";

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
        protected override void DoVerifyClone(GroupElement original, GroupElement clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            Assert.AreEqual(original.SeparatorType, clone.SeparatorType);
            Assert.AreEqual(original.CustomSeparator, clone.CustomSeparator);
        }

        #endregion Methods
    }
}