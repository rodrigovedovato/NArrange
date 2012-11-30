namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ConstructorElement class.
    /// </summary>
    [TestFixture]
    public class ConstructorElementTests : AttributedElementTests<ConstructorElement>
    {
        #region Methods

        /// <summary>
        /// Tests the creation of a new instance.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            ConstructorElement constructorElement = new ConstructorElement();

            //
            // Verify default property values
            //
            Assert.AreEqual(
                ElementType.Constructor,
                constructorElement.ElementType,
                "Unexpected element type.");
            Assert.AreEqual(
                CodeAccess.Public,
                constructorElement.Access,
                "Unexpected default value for Access.");
            Assert.IsNull(constructorElement.Parameters, "Unexpected default value for Params.");
            Assert.IsNotNull(constructorElement.Attributes, "Attributes collection should be instantiated.");
            Assert.AreEqual(0, constructorElement.Attributes.Count, "Attributes collection should be empty.");
            Assert.IsNull(constructorElement.BodyText, "Unexpected default value for BodyText.");
            Assert.IsNotNull(constructorElement.Children, "Children collection should be instantiated.");
            Assert.AreEqual(0, constructorElement.Children.Count, "Children collection should be empty.");
            Assert.IsNotNull(constructorElement.HeaderComments, "HeaderCommentLines collection should not be null.");
            Assert.AreEqual(0, constructorElement.HeaderComments.Count, "HeaderCommentLines collection should be empty.");
            Assert.IsFalse(constructorElement.IsAbstract, "Unexpected default value for IsAbstract.");
            Assert.IsFalse(constructorElement.IsSealed, "Unexpected default value for IsSealed.");
            Assert.IsFalse(constructorElement.IsStatic, "Unexpected default value for IsStatic.");
            Assert.AreEqual(string.Empty, constructorElement.Name, "Unexpected default value for Name.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected override ConstructorElement DoCreateClonePrototype()
        {
            ConstructorElement prototype = new ConstructorElement();
            prototype.Name = "SomeClass";
            prototype.Access = CodeAccess.Internal;
            prototype.AddAttribute(new AttributeElement("Obsolete"));
            prototype.Parameters = "int val";
            prototype.Type = "SomeClass";

            prototype.AddHeaderCommentLine("/// <summary>");
            prototype.AddHeaderCommentLine("/// This is a constructor.");
            prototype.AddHeaderCommentLine("/// </summary>");

            prototype.BodyText = "{_val = val;}";

            prototype.MemberModifiers = MemberModifiers.Abstract;

            return prototype;
        }

        /// <summary>
        /// Verifies the clone was succesful.
        /// </summary>
        /// <param name="original">Original element.</param>
        /// <param name="clone">Clone element.</param>
        protected override void DoVerifyClone(ConstructorElement original, ConstructorElement clone)
        {
            Assert.AreEqual(original.Name, clone.Name, "Name was not copied correctly.");
            Assert.AreEqual(original.Parameters, clone.Parameters, "Params was not copied correctly.");
            Assert.AreEqual(original.Access, clone.Access, "Access was not copied correctly.");
            Assert.AreEqual(original.Attributes.Count, clone.Attributes.Count, "Attributes were not copied correctly.");
            Assert.AreEqual(original.BodyText, clone.BodyText, "BodyText was not copied correctly.");
            Assert.AreEqual(original.Children.Count, clone.Children.Count, "Children were not copied correctly.");
            Assert.AreEqual(original.HeaderComments.Count, clone.HeaderComments.Count, "HeaderCommentLines were not copied correctly.");
            Assert.AreEqual(original.IsAbstract, clone.IsAbstract, "IsAbstract was not copied correctly.");
            Assert.AreEqual(original.IsSealed, clone.IsSealed, "IsSealed was not copied correctly.");
            Assert.AreEqual(original.IsStatic, clone.IsStatic, "IsStatic was not copied correctly.");
            Assert.AreEqual(original.Type, clone.Type, "Type was not copied correctly.");
        }

        #endregion Methods
    }
}