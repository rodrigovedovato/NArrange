namespace NArrange.Tests.VisualBasic
{
    using System.Reflection;

    using NArrange.VisualBasic;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the VBSymbol class.
    /// </summary>
    [TestFixture]
    public class VBSymbolTests
    {
        #region Methods

        /// <summary>
        /// Tests the IsVBSymbol method
        /// </summary>
        [Test]
        public void IsVBSymbolTest()
        {
            //
            // Using reflection, verify that all symbol constants are determined
            // to be symbols.
            //
            FieldInfo[] symbolFields = typeof(VBSymbol).GetFields(
                BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo symbolField in symbolFields)
            {
                if (symbolField.FieldType == typeof(char))
                {
                    char fieldValue = (char)symbolField.GetValue(null);
                    Assert.IsTrue(VBSymbol.IsVBSymbol(fieldValue), "Field value should be considered a VB symbol.");
                }
            }

            Assert.IsFalse(VBSymbol.IsVBSymbol('1'));
            Assert.IsFalse(VBSymbol.IsVBSymbol('A'));
            Assert.IsFalse(VBSymbol.IsVBSymbol('$'));
            Assert.IsFalse(VBSymbol.IsVBSymbol('*'));
        }

        #endregion Methods
    }
}