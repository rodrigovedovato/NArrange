namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Base test fixture for code elements with attributes.
    /// </summary>
    /// <typeparam name="TCodeElement">Code element type.</typeparam>
    public abstract class AttributedElementTests<TCodeElement> : CodeElementTests<TCodeElement>
        where TCodeElement : AttributedElement, new()
    {
        #region Methods

        /// <summary>
        /// Tests the AddAttribute method.
        /// </summary>
        [Test]
        public void AddAttributeTest()
        {
            TCodeElement codeElement = new TCodeElement();

            AttributeElement attribute1 = new AttributeElement("Test");
            AttributeElement attribute2 = new AttributeElement("Ignore");
            codeElement.AddAttribute(attribute1);
            codeElement.AddAttribute(attribute2);

            Assert.AreEqual(2, codeElement.Attributes.Count, "Attributes were not added correctly.");

            Assert.AreSame(codeElement, attribute1.Parent, "Attribute parent was not set correctly.");
            Assert.AreSame(codeElement, attribute2.Parent, "Attribute parent was not set correctly.");

            codeElement.AddAttribute(attribute2);

            Assert.AreEqual(2, codeElement.Attributes.Count, "Attribute should not have been added again.");

            Assert.IsTrue(codeElement.Attributes.Contains(attribute1));
            Assert.IsTrue(codeElement.Attributes.Contains(attribute2));
        }

        /// <summary>
        /// Tests the ClearAttributes method.
        /// </summary>
        [Test]
        public void ClearAttributesTest()
        {
            TCodeElement codeElement = new TCodeElement();

            AttributeElement attribute1 = new AttributeElement("Test");
            AttributeElement attribute2 = new AttributeElement("Ignore");
            codeElement.AddAttribute(attribute1);
            codeElement.AddAttribute(attribute2);

            Assert.AreEqual(2, codeElement.Attributes.Count, "Attributes were not added correctly.");

            codeElement.ClearAttributes();

            Assert.AreEqual(0, codeElement.Attributes.Count, "Attributes were not cleared correctly.");

            Assert.IsNull(attribute1.Parent, "Attribute parent should have been cleared.");
            Assert.IsNull(attribute2.Parent, "Attribute parent should have been cleared.");
        }

        /// <summary>
        /// Tests the AddAttribute method.
        /// </summary>
        [Test]
        public void RemoveAttributeTest()
        {
            TCodeElement codeElement1 = new TCodeElement();

            AttributeElement attribute1 = new AttributeElement("Test");
            AttributeElement attribute2 = new AttributeElement("Ignore");
            codeElement1.AddAttribute(attribute1);
            codeElement1.AddAttribute(attribute2);

            Assert.AreEqual(2, codeElement1.Attributes.Count, "Attributes were not added correctly.");

            //
            // Remove the attribute using the method
            //
            codeElement1.RemoveAttribute(attribute2);

            Assert.AreEqual(1, codeElement1.Attributes.Count, "Attribute should have been removed.");

            Assert.IsTrue(codeElement1.Attributes.Contains(attribute1));
            Assert.IsFalse(codeElement1.Attributes.Contains(attribute2));

            //
            // Remove the attribute by assigning a different parent
            //
            TCodeElement codeElement2 = new TCodeElement();
            attribute1.Parent = codeElement2;

            Assert.AreEqual(
                0,
                codeElement1.Attributes.Count,
                "Attribute should have been removed from the original element.");
            Assert.AreEqual(
                1,
                codeElement2.Attributes.Count,
                "Attribute should have been added to the new element.");

            Assert.IsFalse(codeElement1.Attributes.Contains(attribute1));
            Assert.IsTrue(codeElement2.Attributes.Contains(attribute1));
        }

        #endregion Methods
    }
}