namespace NArrange.Tests.Core
{
    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ElementFilter class.
    /// </summary>
    [TestFixture]
    public class ElementFilterTests
    {
        #region Methods

        /// <summary>
        /// Tests access filtering.
        /// </summary>
        [Test]
        public void IsMatchAccessTest()
        {
            ElementFilter filter = new ElementFilter("$(Access) : 'Protected'");

            //
            // Not a match
            //
            FieldElement publicField = new FieldElement();
            publicField.Access = CodeAccess.Public;
            Assert.IsFalse(filter.IsMatch(publicField), "IsMatch did not return the expected value.");

            //
            // Match
            //
            FieldElement protectedField = new FieldElement();
            protectedField.Access = CodeAccess.Protected;
            Assert.IsTrue(filter.IsMatch(protectedField), "IsMatch did not return the expected value.");

            //
            // Multi flag
            //
            FieldElement protectedInternalField = new FieldElement();
            protectedInternalField.Access = CodeAccess.Protected | CodeAccess.Internal;
            Assert.IsTrue(filter.IsMatch(protectedInternalField), "IsMatch did not return the expected value.");

            //
            // Null
            //
            Assert.IsFalse(filter.IsMatch(null), "IsMatch did not return the expected value.");
        }

        /// <summary>
        /// Tests name filtering.
        /// </summary>
        [Test]
        public void IsMatchNameTest()
        {
            ElementFilter filter = new ElementFilter("$(Name) : 'Style'");

            //
            // Not a match
            //
            FieldElement noMatch = new FieldElement();
            noMatch.Name = "Test";
            Assert.IsFalse(filter.IsMatch(noMatch), "IsMatch did not return the expected value.");

            //
            // Match
            //
            FieldElement match = new FieldElement();
            match.Name = "Style";
            Assert.IsTrue(filter.IsMatch(match), "IsMatch did not return the expected value.");
            match.Name = "ElementStyle";
            Assert.IsTrue(filter.IsMatch(match), "IsMatch did not return the expected value.");
            match.Name = "StyleElement";
            Assert.IsTrue(filter.IsMatch(match), "IsMatch did not return the expected value.");

            //
            // Null
            //
            Assert.IsFalse(filter.IsMatch(null), "IsMatch did not return the expected value.");
        }

        /// <summary>
        /// Tests the RequiredScope property.
        /// </summary>
        [Test]
        public void RequiredScope()
        {
            ElementFilter filter;

            filter = new ElementFilter("$(Name) : 'Style'");
            Assert.AreEqual(ElementAttributeScope.Element, filter.RequiredScope);

            filter = new ElementFilter("$(Name) : 'Style' Or $(Name) : 'Test'");
            Assert.AreEqual(ElementAttributeScope.Element, filter.RequiredScope);

            filter = new ElementFilter("$(Parent.Name) : 'Style'");
            Assert.AreEqual(ElementAttributeScope.Parent, filter.RequiredScope);

            filter = new ElementFilter("$(Parent.Name) : 'Style' Or $(Name) : 'Style'");
            Assert.AreEqual(ElementAttributeScope.Parent, filter.RequiredScope);

            filter = new ElementFilter("$(Name) : 'Style' Or $(Parent.Name) : 'Style'");
            Assert.AreEqual(ElementAttributeScope.Parent, filter.RequiredScope);
        }

        #endregion Methods
    }
}