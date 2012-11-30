namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the FieldElement class.
    /// </summary>
    [TestFixture]
    public class FieldElementTests : AttributedElementTests<FieldElement>
    {
        #region Methods

        /// <summary>
        /// Tests the creation of a new instance.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            FieldElement fieldElement = new FieldElement();

            //
            // Verify default field values
            //
            Assert.AreEqual(ElementType.Field, fieldElement.ElementType, "Unexpected element type.");
            Assert.AreEqual(CodeAccess.Public, fieldElement.Access, "Unexpected default value for Access.");
            Assert.IsNull(fieldElement.InitialValue, "Unexpected default value for InitialValue.");
            Assert.IsFalse(fieldElement.IsVolatile, "Unexpected default value for IsVolatile.");
            Assert.IsNotNull(fieldElement.Attributes, "Attributes collection should be instantiated.");
            Assert.AreEqual(0, fieldElement.Attributes.Count, "Attributes collection should be empty.");
            Assert.IsNull(fieldElement.BodyText, "Unexpected default value for BodyText.");
            Assert.IsNotNull(fieldElement.Children, "Children collection should be instantiated.");
            Assert.AreEqual(0, fieldElement.Children.Count, "Children collection should be empty.");
            Assert.IsNotNull(fieldElement.HeaderComments, "HeaderCommentLines collection should not be null.");
            Assert.AreEqual(0, fieldElement.HeaderComments.Count, "HeaderCommentLines collection should be empty.");
            Assert.IsFalse(fieldElement.IsAbstract, "Unexpected default value for IsAbstract.");
            Assert.IsFalse(fieldElement.IsSealed, "Unexpected default value for IsSealed.");
            Assert.IsFalse(fieldElement.IsStatic, "Unexpected default value for IsStatic.");
            Assert.AreEqual(string.Empty, fieldElement.Name, "Unexpected default value for Name.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected override FieldElement DoCreateClonePrototype()
        {
            FieldElement prototype = new FieldElement();
            prototype.Name = "SomeField";
            prototype.Access = CodeAccess.Private;
            prototype.AddAttribute(new AttributeElement("Obsolete"));
            prototype.Type = "int";
            prototype.IsVolatile = true;

            prototype.AddHeaderCommentLine("/// <summary>");
            prototype.AddHeaderCommentLine("/// This is a field.");
            prototype.AddHeaderCommentLine("/// </summary>");

            prototype.InitialValue = "4";

            prototype.MemberModifiers = MemberModifiers.Abstract;

            prototype.TrailingComment = new CommentElement("This is a trailing comment");

            return prototype;
        }

        /// <summary>
        /// Verifies the clone was succesful.
        /// </summary>
        /// <param name="original">Original element.</param>
        /// <param name="clone">Clone element.</param>
        protected override void DoVerifyClone(FieldElement original, FieldElement clone)
        {
            Assert.AreEqual(original.Name, clone.Name, "Name was not copied correctly.");
            Assert.AreEqual(original.Access, clone.Access, "Access was not copied correctly.");
            Assert.AreEqual(
                original.Attributes.Count,
                clone.Attributes.Count,
                "Attributes were not copied correctly.");
            Assert.AreEqual(original.BodyText, clone.BodyText, "BodyText was not copied correctly.");
            Assert.AreEqual(
                original.Children.Count,
                clone.Children.Count,
                "Children were not copied correctly.");
            Assert.AreEqual(
                original.HeaderComments.Count,
                clone.HeaderComments.Count,
                "HeaderCommentLines were not copied correctly.");
            Assert.AreEqual(original.IsAbstract, clone.IsAbstract, "IsAbstract was not copied correctly.");
            Assert.AreEqual(original.IsSealed, clone.IsSealed, "IsSealed was not copied correctly.");
            Assert.AreEqual(original.IsStatic, clone.IsStatic, "IsStatic was not copied correctly.");
            Assert.AreEqual(original.Type, clone.Type, "Type was not copied correctly.");
            Assert.AreEqual(original.InitialValue, clone.InitialValue, "InitialValue was not copied correctly.");
            Assert.AreEqual(original.IsVolatile, clone.IsVolatile, "IsVolatile was not copied correctly.");
            Assert.IsNotNull(clone.TrailingComment, "TrailingComment was not copied correctly.");
            Assert.AreNotSame(
                original.TrailingComment,
                clone.TrailingComment,
                "TrailingComment was not copied correctly.");
            Assert.AreEqual(
                original.TrailingComment.Text,
                clone.TrailingComment.Text,
                "TrailingComment was not copied correctly.");
        }

        #endregion Methods
    }
}