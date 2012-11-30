namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the TypeElement class.
    /// </summary>
    [TestFixture]
    public class TypeElementTests : AttributedElementTests<TypeElement>
    {
        #region Methods

        /// <summary>
        /// Tests the creation of a new instance.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            TypeElement typeElement = new TypeElement();

            //
            // Verify default property values
            //
            Assert.AreEqual(TypeElementType.Class, typeElement.Type, "Unexpected default value for Type.");
            Assert.AreEqual(CodeAccess.Public, typeElement.Access, "Unexpected default value for Access.");
            Assert.IsNotNull(typeElement.Attributes, "Attributes collection should be instantiated.");
            Assert.AreEqual(0, typeElement.Attributes.Count, "Attributes collection should be empty.");
            Assert.IsNull(typeElement.BodyText, "Unexpected default value for BodyText.");
            Assert.IsNotNull(typeElement.Children, "Children collection should be instantiated.");
            Assert.AreEqual(0, typeElement.Children.Count, "Children collection should be empty.");
            Assert.IsNotNull(typeElement.HeaderComments, "HeaderCommentLines collection should not be null.");
            Assert.AreEqual(0, typeElement.HeaderComments.Count, "HeaderCommentLines collection should be empty.");
            Assert.IsNotNull(typeElement.Interfaces, "Interfaces collection should not be null.");
            Assert.AreEqual(0, typeElement.Interfaces.Count, "Interfaces collection should be empty.");
            Assert.IsFalse(typeElement.IsAbstract, "Unexpected default value for IsAbstract.");
            Assert.IsFalse(typeElement.IsSealed, "Unexpected default value for IsSealed.");
            Assert.IsFalse(typeElement.IsStatic, "Unexpected default value for IsStatic.");
            Assert.AreEqual(string.Empty, typeElement.Name, "Unexpected default value for Name.");
            Assert.IsNotNull(typeElement.TypeParameters, "TypeParameters collection should not be null.");
            Assert.AreEqual(0, typeElement.TypeParameters.Count, "TypeParameters collection should be empty.");
        }

        /// <summary>
        /// Creates an instance for cloning.
        /// </summary>
        /// <returns>Clone prototype.</returns>
        protected override TypeElement DoCreateClonePrototype()
        {
            TypeElement prototype = new TypeElement();
            prototype.Name = "SomeType";
            prototype.Type = TypeElementType.Structure;
            prototype.Access = CodeAccess.Internal;
            prototype.AddAttribute(new AttributeElement("Obsolete"));

            ConstructorElement constructor = new ConstructorElement();
            constructor.Name = "SomeType";
            constructor.Access = CodeAccess.Public;
            prototype.AddChild(constructor);

            prototype.AddHeaderCommentLine("/// <summary>");
            prototype.AddHeaderCommentLine("/// This is a structure.");
            prototype.AddHeaderCommentLine("/// </summary>");

            prototype.AddInterface(
                new InterfaceReference("IDisposable", InterfaceReferenceType.Interface));

            prototype.BodyText = "test";

            prototype.TypeModifiers = TypeModifiers.Abstract;
            prototype.AddTypeParameter(new TypeParameter("T", "new()"));

            return prototype;
        }

        /// <summary>
        /// Verifies the clone was succesful.
        /// </summary>
        /// <param name="original">Original element.</param>
        /// <param name="clone">Clone element.</param>
        protected override void DoVerifyClone(TypeElement original, TypeElement clone)
        {
            Assert.AreEqual(original.Name, clone.Name, "Name was not copied correctly.");
            Assert.AreEqual(original.Access, clone.Access, "Access was not copied correctly.");
            Assert.AreEqual(original.Attributes.Count, clone.Attributes.Count, "Attributes were not copied correctly.");
            Assert.AreEqual(original.BodyText, clone.BodyText, "BodyText was not copied correctly.");
            Assert.AreEqual(original.Children.Count, clone.Children.Count, "Children were not copied correctly.");
            Assert.AreEqual(original.HeaderComments.Count, clone.HeaderComments.Count, "HeaderCommentLines were not copied correctly.");
            Assert.AreEqual(original.Interfaces.Count, clone.Interfaces.Count, "Interfaces were not copied correctly.");
            Assert.AreEqual(original.IsAbstract, clone.IsAbstract, "IsAbstract was not copied correctly.");
            Assert.AreEqual(original.IsSealed, clone.IsSealed, "IsSealed was not copied correctly.");
            Assert.AreEqual(original.IsStatic, clone.IsStatic, "IsStatic was not copied correctly.");
            Assert.AreEqual(original.Type, clone.Type, "Type was not copied correctly.");
            Assert.AreEqual(original.TypeParameters.Count, clone.TypeParameters.Count, "TypeParameters were not copied correctly.");
        }

        #endregion Methods
    }
}