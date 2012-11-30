namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the NamespaceElement class.
    /// </summary>
    [TestFixture]
    public class NamespaceElementTests : CodeElementTests<NamespaceElement>
    {
        #region Methods

        /// <summary>
        /// Tests constructing a new NamespaceElement.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            NamespaceElement element = new NamespaceElement();

            //
            // Verify default values
            //
            Assert.AreEqual(ElementType.Namespace, element.ElementType, "Unexpected element type.");
            Assert.AreEqual(string.Empty, element.Name, "Unexpected default value for Name.");
            Assert.IsNotNull(element.Children, "Children collection should not be null.");
            Assert.AreEqual(0, element.Children.Count, "Children collection should be empty.");
            Assert.IsNotNull(element.HeaderComments, "HeaderCommentLines collection should not be null.");
            Assert.AreEqual(0, element.HeaderComments.Count, "HeaderCommentLines collection should be empty.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected override NamespaceElement DoCreateClonePrototype()
        {
            NamespaceElement prototype = new NamespaceElement();
            prototype.Name = "SampleNamespace";

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
        protected override void DoVerifyClone(NamespaceElement original, NamespaceElement clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
        }

        #endregion Methods
    }
}