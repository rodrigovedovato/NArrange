namespace NArrange.Tests.VisualBasic
{
    using System.Reflection;

    using NArrange.VisualBasic;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the VBKeyword class.
    /// </summary>
    [TestFixture]
    public class VBKeywordTests
    {
        #region Methods

        /// <summary>
        /// Tests the IsVBKeyword method
        /// </summary>
        [Test]
        public void IsVBKeywordTest()
        {
            //
            // Using reflection, verify that all string constants are determined
            // to be keywords.
            //
            FieldInfo[] wordFields = typeof(VBSymbol).GetFields(
                BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo wordField in wordFields)
            {
                if (wordField.FieldType == typeof(string))
                {
                    string fieldValue = (string)wordField.GetValue(null);
                    Assert.IsTrue(VBKeyword.IsVBKeyword(fieldValue), "Field value should be considered a VB keyword.");
                }
            }

            Assert.IsFalse(VBKeyword.IsVBKeyword("Test"));
            Assert.IsFalse(VBKeyword.IsVBKeyword("unknown"));
            Assert.IsFalse(VBKeyword.IsVBKeyword("Find"));
            Assert.IsFalse(VBKeyword.IsVBKeyword(string.Empty));
            Assert.IsFalse(VBKeyword.IsVBKeyword(null));
        }

        /// <summary>
        /// Tests the Normalize method
        /// </summary>
        [Test]
        public void NormalizeTest()
        {
            //
            // Using reflection, verify for all keyword constants
            //
            FieldInfo[] keywordFields = typeof(VBKeyword).GetFields(
                BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo keyword in keywordFields)
            {
                if (keyword.FieldType == typeof(string))
                {
                    string fieldValue = (string)keyword.GetValue(null);

                    string lower = fieldValue.ToLower();
                    string normalized = VBKeyword.Normalize(lower);

                    Assert.AreEqual(
                        fieldValue, normalized, "Normalize did not correct casing for {0}", fieldValue);
                }
            }

            //
            // Normalize a non-VB keyword
            //
            Assert.AreEqual("Testit", VBKeyword.Normalize("testit"));
        }

        #endregion Methods
    }
}