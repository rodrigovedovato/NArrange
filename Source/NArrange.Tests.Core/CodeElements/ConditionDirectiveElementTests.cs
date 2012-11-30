namespace NArrange.Tests.Core.CodeElements
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ConditionDirectiveElement class.
    /// </summary>
    [TestFixture]
    public class ConditionDirectiveElementTests : CommentedElementTests<ConditionDirectiveElement>
    {
        #region Methods

        /// <summary>
        /// Tests constructing a new UsingElement.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            UsingElement element = new UsingElement();

            //
            // Verify default values
            //
            Assert.AreEqual(string.Empty, element.Name, "Unexpected default value for Name.");
            Assert.IsNull(element.Redefine, "Unexpected default value for Alias.");

            Assert.IsNotNull(element.Children, "Children collection should not be null.");
            Assert.AreEqual(0, element.Children.Count, "Children collection should be empty.");
            Assert.IsNotNull(element.HeaderComments, "HeaderCommentLines collection should not be null.");
            Assert.AreEqual(0, element.HeaderComments.Count, "HeaderCommentLines collection should be empty.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected override ConditionDirectiveElement DoCreateClonePrototype()
        {
            ConditionDirectiveElement prototype = new ConditionDirectiveElement();
            prototype.ConditionExpression = "DEBUG";
            FieldElement field1 = new FieldElement();
            field1.Name = "field1";
            field1.Type = "string";
            prototype.AddChild(field1);

            prototype.ElseCondition = new ConditionDirectiveElement();
            prototype.ElseCondition.ConditionExpression = null;
            FieldElement field2 = new FieldElement();
            field2.Name = "field1";
            field2.Type = "string";
            prototype.ElseCondition.AddChild(field2);

            return prototype;
        }

        /// <summary>
        /// Verifies that a clone has the same state as the original.
        /// </summary>
        /// <param name="original">Original element.</param>
        /// <param name="clone">Clone element.</param>
        protected override void DoVerifyClone(ConditionDirectiveElement original, ConditionDirectiveElement clone)
        {
            Assert.AreEqual(original.ConditionExpression, clone.ConditionExpression);
            Assert.AreEqual(original.Children.Count, clone.Children.Count);

            Assert.IsNotNull(original.ElseCondition);
            Assert.IsNotNull(clone.ElseCondition);
            Assert.AreNotSame(original.ElseCondition, clone.ElseCondition);
            Assert.AreEqual(original.ElseCondition.ConditionExpression, clone.ElseCondition.ConditionExpression);
            Assert.AreEqual(original.ElseCondition.Children.Count, clone.ElseCondition.Children.Count);
        }

        #endregion Methods
    }
}