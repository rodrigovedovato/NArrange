namespace NArrange.Tests.CSharp
{
    using System.Reflection;

    using NArrange.CSharp;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the CSharpSymbol class.
    /// </summary>
    [TestFixture]
    public class CSharpSymbolTests
    {
        #region Methods

        /// <summary>
        /// Tests the IsCSharpSymbol method.
        /// </summary>
        [Test]
        public void IsCSharpSymbolTest()
        {
            //
            // Using reflection, verify that all symbol constants are determined
            // to be symbols.
            //
            FieldInfo[] symbolFields = typeof(CSharpSymbol).GetFields(
                BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo symbolField in symbolFields)
            {
                if (symbolField.FieldType == typeof(char))
                {
                    char fieldValue = (char)symbolField.GetValue(null);
                    Assert.IsTrue(CSharpSymbol.IsCSharpSymbol(fieldValue), "Field value should be considered a C# symbol.");
                }
            }

            Assert.IsFalse(CSharpSymbol.IsCSharpSymbol('1'));
            Assert.IsFalse(CSharpSymbol.IsCSharpSymbol('A'));
            Assert.IsFalse(CSharpSymbol.IsCSharpSymbol('$'));
            Assert.IsFalse(CSharpSymbol.IsCSharpSymbol('_'));
        }

        #endregion Methods
    }
}