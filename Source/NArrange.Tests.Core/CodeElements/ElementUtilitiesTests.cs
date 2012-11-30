namespace NArrange.Tests.Core.CodeElements
{
    using System;

    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ElementUtilities class.
    /// </summary>
    [TestFixture]
    public class ElementUtilitiesTests
    {
        #region Methods

        /// <summary>
        /// Tests Format with a null element.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatNullElementTest()
        {
            ElementUtilities.Format("$(Name)", null);
        }

        /// <summary>
        /// Tests Format with a null format string.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatNullStringTest()
        {
            ElementUtilities.Format(null, new MethodElement());
        }

        /// <summary>
        /// Tests the Format method with a valid format string and element.
        /// </summary>
        [Test]
        public void FormatTest()
        {
            MethodElement methodElement = new MethodElement();
            methodElement.Name = "Test";

            string formatted = ElementUtilities.Format(
                "End $(ElementType) $(Name)",
                methodElement);

            Assert.AreEqual("End Method Test", formatted, "Unexpected formatted result.");
        }

        /// <summary>
        /// Tests the GetAttribute method for Access.
        /// </summary>
        [Test]
        public void GetAttributeAccessTest()
        {
            FieldElement fieldElement = new FieldElement();
            fieldElement.Name = "TestField";
            fieldElement.Access = CodeAccess.Protected;

            string attribute = ElementUtilities.GetAttribute(ElementAttributeType.Access, fieldElement);
            Assert.AreEqual("Protected", attribute, "Unexpected attribute.");
        }

        /// <summary>
        /// Tests the GetAttribute method for Attributes.
        /// </summary>
        [Test]
        public void GetAttributeAttributesTest()
        {
            FieldElement fieldElement = new FieldElement();
            fieldElement.Name = "TestField";

            string attribute = ElementUtilities.GetAttribute(ElementAttributeType.Attributes, fieldElement);
            Assert.AreEqual(string.Empty, attribute, "Unexpected attribute.");

            //
            // Add some attributes to the element.
            //
            AttributeElement attribute1 = new AttributeElement();
            attribute1.Name = "Attribute1";
            attribute1.BodyText = "false";

            AttributeElement attribute2 = new AttributeElement();
            attribute2.Name = "Attribute2";
            attribute2.BodyText = "false";

            fieldElement.AddAttribute(attribute1);
            fieldElement.AddAttribute(attribute2);

            attribute = ElementUtilities.GetAttribute(ElementAttributeType.Attributes, fieldElement);
            Assert.AreEqual("Attribute1, Attribute2", attribute, "Unexpected attribute.");

            //
            // Add nested attributes to the element.
            //
            fieldElement.ClearAttributes();
            attribute1 = new AttributeElement();
            attribute1.Name = "Attribute1";
            attribute1.BodyText = "false";

            attribute2 = new AttributeElement();
            attribute2.Name = "Attribute2";
            attribute2.BodyText = "false";
            attribute1.AddChild(attribute2);

            fieldElement.AddAttribute(attribute1);

            attribute = ElementUtilities.GetAttribute(ElementAttributeType.Attributes, fieldElement);
            Assert.AreEqual("Attribute1, Attribute2", attribute, "Unexpected attribute.");
        }

        /// <summary>
        /// Tests the GetAttribute method for ElementType.
        /// </summary>
        [Test]
        public void GetAttributeElementTypeTest()
        {
            FieldElement fieldElement = new FieldElement();
            fieldElement.Name = "TestField";
            fieldElement.Access = CodeAccess.Protected;

            string attribute = ElementUtilities.GetAttribute(ElementAttributeType.ElementType, fieldElement);
            Assert.AreEqual("Field", attribute, "Unexpected attribute.");
        }

        /// <summary>
        /// Tests the GetAttribute method for Modifier.
        /// </summary>
        [Test]
        public void GetAttributeModifierTest()
        {
            FieldElement fieldElement = new FieldElement();
            fieldElement.Name = "TestField";
            fieldElement.Access = CodeAccess.Protected;
            fieldElement.Type = "int";
            fieldElement.MemberModifiers = MemberModifiers.Static;

            string attribute = ElementUtilities.GetAttribute(ElementAttributeType.Modifier, fieldElement);
            Assert.AreEqual("Static", attribute, "Unexpected attribute.");

            TypeElement typeElement = new TypeElement();
            typeElement.TypeModifiers = TypeModifiers.Sealed;

            attribute = ElementUtilities.GetAttribute(ElementAttributeType.Modifier, typeElement);
            Assert.AreEqual("Sealed", attribute, "Unexpected attribute.");

            UsingElement usingElement = new UsingElement();
            usingElement.Name = "System";

            attribute = ElementUtilities.GetAttribute(ElementAttributeType.Modifier, usingElement);
            Assert.AreEqual(string.Empty, attribute, "Unexpected attribute.");
        }

        /// <summary>
        /// Tests the GetAttribute method for Name.
        /// </summary>
        [Test]
        public void GetAttributeNameTest()
        {
            FieldElement fieldElement = new FieldElement();
            fieldElement.Name = "TestField";

            string attribute = ElementUtilities.GetAttribute(ElementAttributeType.Name, fieldElement);
            Assert.AreEqual("TestField", attribute, "Unexpected attribute.");
        }

        /// <summary>
        /// Tests the GetAttribute method for None.
        /// </summary>
        [Test]
        public void GetAttributeNoneTest()
        {
            FieldElement fieldElement = new FieldElement();
            fieldElement.Name = "TestField";

            string attribute = ElementUtilities.GetAttribute(ElementAttributeType.None, fieldElement);
            Assert.AreEqual(string.Empty, attribute, "Unexpected attribute.");
        }

        /// <summary>
        /// Tests the GetAttribute method for Type.
        /// </summary>
        [Test]
        public void GetAttributeTypeTest()
        {
            FieldElement fieldElement = new FieldElement();
            fieldElement.Name = "TestField";
            fieldElement.Access = CodeAccess.Protected;
            fieldElement.Type = "int";

            string attribute = ElementUtilities.GetAttribute(ElementAttributeType.Type, fieldElement);
            Assert.AreEqual("int", attribute, "Unexpected attribute.");

            TypeElement typeElement = new TypeElement();
            typeElement.Type = TypeElementType.Interface;

            attribute = ElementUtilities.GetAttribute(ElementAttributeType.Type, typeElement);
            Assert.AreEqual("Interface", attribute, "Unexpected attribute.");

            CommentElement commentElement = new CommentElement(CommentType.Block);

            attribute = ElementUtilities.GetAttribute(ElementAttributeType.Type, commentElement);
            Assert.AreEqual("Block", attribute, "Unexpected attribute.");

            UsingElement usingElement = new UsingElement();
            usingElement.Name = "MySystem";
            usingElement.Redefine = "System";

            attribute = ElementUtilities.GetAttribute(ElementAttributeType.Type, usingElement);
            Assert.AreEqual("Alias", attribute, "Unexpected attribute.");

            ConditionDirectiveElement conditionElement = new ConditionDirectiveElement();

            attribute = ElementUtilities.GetAttribute(ElementAttributeType.Type, conditionElement);
            Assert.AreEqual(string.Empty, attribute, "Unexpected attribute.");
        }

        #endregion Methods
    }
}