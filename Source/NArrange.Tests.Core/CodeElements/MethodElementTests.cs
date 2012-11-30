namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the MethodElement class.
    /// </summary>
    [TestFixture]
    public class MethodElementTests : AttributedElementTests<MethodElement>
    {
        #region Methods

        /// <summary>
        /// Tests the creation of a new instance.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            MethodElement methodElement = new MethodElement();

            //
            // Verify default property values
            //
            Assert.AreEqual(ElementType.Method, methodElement.ElementType, "Unexpected element type.");
            Assert.AreEqual(CodeAccess.Public, methodElement.Access, "Unexpected default value for Access.");
            Assert.IsNull(methodElement.Parameters, "Unexpected default value for Params.");
            Assert.IsNotNull(methodElement.Attributes, "Attributes collection should be instantiated.");
            Assert.AreEqual(0, methodElement.Attributes.Count, "Attributes collection should be empty.");
            Assert.IsNull(methodElement.BodyText, "Unexpected default value for BodyText.");
            Assert.IsNotNull(methodElement.Children, "Children collection should be instantiated.");
            Assert.AreEqual(0, methodElement.Children.Count, "Children collection should be empty.");
            Assert.IsNotNull(methodElement.HeaderComments, "HeaderCommentLines collection should not be null.");
            Assert.AreEqual(0, methodElement.HeaderComments.Count, "HeaderCommentLines collection should be empty.");
            Assert.IsFalse(methodElement.IsAbstract, "Unexpected default value for IsAbstract.");
            Assert.IsFalse(methodElement.IsSealed, "Unexpected default value for IsSealed.");
            Assert.IsFalse(methodElement.IsStatic, "Unexpected default value for IsStatic.");
            Assert.AreEqual(string.Empty, methodElement.Name, "Unexpected default value for Name.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected override MethodElement DoCreateClonePrototype()
        {
            MethodElement prototype = new MethodElement();
            prototype.Name = "SomeMethod";
            prototype.Access = CodeAccess.Internal;
            prototype.AddAttribute(new AttributeElement("Obsolete"));
            prototype.Parameters = "T val";
            prototype.Type = "bool";
            prototype.AddTypeParameter(
                new TypeParameter("T", "class", "new()"));
            prototype.MemberModifiers = MemberModifiers.Abstract;
            prototype.AddImplementation(
                new InterfaceReference("ISomeInterface.SomeMethod", InterfaceReferenceType.Interface));
            prototype.AddImplementation(
                new InterfaceReference("IAnotherInterface.SomeMethod", InterfaceReferenceType.Interface));

            prototype.AddHeaderCommentLine("/// <summary>");
            prototype.AddHeaderCommentLine("/// This is a method.");
            prototype.AddHeaderCommentLine("/// </summary>");

            prototype.BodyText = "{return T != null;}";

            return prototype;
        }

        /// <summary>
        /// Verifies the clone was succesful.
        /// </summary>
        /// <param name="original">Original element.</param>
        /// <param name="clone">Clone element.</param>
        protected override void DoVerifyClone(MethodElement original, MethodElement clone)
        {
            Assert.AreEqual(original.Name, clone.Name, "Name was not copied correctly.");
            Assert.AreEqual(original.Parameters, clone.Parameters, "Params was not copied correctly.");
            Assert.AreEqual(original.Access, clone.Access, "Access was not copied correctly.");
            Assert.AreEqual(original.Attributes.Count, clone.Attributes.Count, "Attributes were not copied correctly.");
            Assert.AreEqual(original.BodyText, clone.BodyText, "BodyText was not copied correctly.");
            Assert.AreEqual(original.Children.Count, clone.Children.Count, "Children were not copied correctly.");
            Assert.AreEqual(original.HeaderComments.Count, clone.HeaderComments.Count, "HeaderCommentLines were not copied correctly.");
            Assert.AreEqual(original.TypeParameters.Count, clone.TypeParameters.Count, "TypeParameters were not copied correctly.");
            Assert.AreEqual(original.IsAbstract, clone.IsAbstract, "IsAbstract was not copied correctly.");
            Assert.AreEqual(original.IsSealed, clone.IsSealed, "IsSealed was not copied correctly.");
            Assert.AreEqual(original.IsStatic, clone.IsStatic, "IsStatic was not copied correctly.");
            Assert.AreEqual(original.Type, clone.Type, "Type was not copied correctly.");
            Assert.AreEqual(original.IsOperator, clone.IsOperator, "IsOperator was not copied correctly.");
            Assert.AreEqual(original.OperatorType, clone.OperatorType, "OperatorType was not copied correctly.");

            Assert.AreEqual(
                original.Implements.Count,
                clone.Implements.Count,
                "Interface implementations were not copied correctly.");
            for (int implementsIndex = 0; implementsIndex < original.Implements.Count;
                implementsIndex++)
            {
                InterfaceReference originalImplements = original.Implements[implementsIndex];
                InterfaceReference clonedImplements = clone.Implements[implementsIndex];

                Assert.AreEqual(
                    originalImplements.Name,
                    clonedImplements.Name,
                     "Interface implementations were not copied correctly.");
                Assert.AreEqual(
                    originalImplements.ReferenceType,
                    clonedImplements.ReferenceType,
                     "Interface implementations were not copied correctly.");
            }
        }

        #endregion Methods
    }
}