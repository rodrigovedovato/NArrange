namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the CommentElement class.
    /// </summary>
    [TestFixture]
    public class CommentElementTests : CodeElementTests<CommentElement>
    {
        #region Methods

        /// <summary>
        /// Tests constructing a new CommentElement.
        /// </summary>
        [Test]
        public void CreateDefaultTest()
        {
            CommentElement element = new CommentElement();

            //
            // Verify default values
            //
            Assert.AreEqual(string.Empty, element.Name, "Unexpected default value for Name.");
            Assert.AreEqual(CommentType.Line, element.Type, "Unexpected default value for Type.");
            Assert.IsNull(element.Text, "Unexpected default value for Text.");
        }

        /// <summary>
        /// Tests constructing a new CommentElement.
        /// </summary>
        [Test]
        public void CreateTextAndTypeTest()
        {
            CommentElement element = new CommentElement("Test", CommentType.Block);

            //
            // Verify default values
            //
            Assert.AreEqual(string.Empty, element.Name, "Unexpected default value for Name.");
            Assert.AreEqual(CommentType.Block, element.Type, "Unexpected value for Type.");
            Assert.AreEqual("Test", element.Text, "Unexpected value for Text.");
        }

        /// <summary>
        /// Tests constructing a new CommentElement.
        /// </summary>
        [Test]
        public void CreateTypeTest()
        {
            CommentElement element = new CommentElement(CommentType.XmlLine);

            //
            // Verify default values
            //
            Assert.AreEqual(string.Empty, element.Name, "Unexpected default value for Name.");
            Assert.AreEqual(CommentType.XmlLine, element.Type, "Unexpected value for Type.");
            Assert.IsNull(element.Text, "Unexpected default value for Text.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected override CommentElement DoCreateClonePrototype()
        {
            CommentElement prototype = new CommentElement("Block comment here...", CommentType.Block);

            return prototype;
        }

        /// <summary>
        /// Performs the ToString test.
        /// </summary>
        protected override void DoToStringTest()
        {
            CommentElement commentElement = new CommentElement(
                "This is a block comment...", CommentType.Block);
            Assert.AreEqual(
                commentElement.Text,
                commentElement.ToString(),
                "Unexpected string representation.");
        }

        /// <summary>
        /// Verifies that a clone has the same state as the original.
        /// </summary>
        /// <param name="original">Original element.</param>
        /// <param name="clone">Clone element.</param>
        protected override void DoVerifyClone(CommentElement original, CommentElement clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            Assert.AreEqual(original.Text, clone.Text);
            Assert.AreEqual(original.Type, clone.Type);
        }

        #endregion Methods
    }
}