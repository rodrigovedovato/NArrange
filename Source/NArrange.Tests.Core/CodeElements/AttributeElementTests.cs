namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the AttributeElement class.
    /// </summary>
    [TestFixture]
    public class AttributeElementTests : CommentedElementTests<AttributeElement>
    {
        #region Methods

        /// <summary>
        /// Tests constructing a new UsingElement.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            AttributeElement element = new AttributeElement();

            //
            // Verify default values
            //
            Assert.IsNull(element.Target, "Unexpected default value for Target.");

            Assert.IsNotNull(element.Children, "Children collection should not be null.");
            Assert.AreEqual(0, element.Children.Count, "Children collection should be empty.");
            Assert.IsNotNull(element.HeaderComments, "HeaderCommentLines collection should not be null.");
            Assert.AreEqual(0, element.HeaderComments.Count, "HeaderCommentLines collection should be empty.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>A clone prototype for testing.</returns>
        protected override AttributeElement DoCreateClonePrototype()
        {
            AttributeElement prototype = new AttributeElement();
            prototype.Name = "SampleAttribute";
            prototype.Target = "class";
            prototype.BodyText = "\"Test\"";

            AttributeElement child1 = new AttributeElement();
            AttributeElement child2 = new AttributeElement();

            prototype.AddChild(child1);
            prototype.AddChild(child2);

            return prototype;
        }

        /// <summary>
        /// Verifies that a clone has the same state as the original.
        /// </summary>
        /// <param name="original">Attribute element.</param>
        /// <param name="clone">Clone instance.</param>
        protected override void DoVerifyClone(AttributeElement original, AttributeElement clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            Assert.AreEqual(original.Target, clone.Target);
            Assert.AreEqual(original.BodyText, clone.BodyText);

            Assert.AreEqual(original.Children.Count, clone.Children.Count);
        }

        #endregion Methods
    }
}