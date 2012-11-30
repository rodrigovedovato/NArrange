namespace NArrange.Tests.Core
{
    using System.IO;
    using System.Text;

    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the FileUtilities class.
    /// </summary>
    [TestFixture]
    public class FileUtilitiesTests
    {
        #region Fields

        /// <summary>
        /// Directory containing files for encoding testing.
        /// </summary>
        private const string EncodingTestFileDirectory = "EncodingTestFiles";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Tests the GetEncoding method.
        /// </summary>
        [Test]
        public void GetEncodingTest()
        {
            DirectoryInfo testFileDirectory = new DirectoryInfo(EncodingTestFileDirectory);
            FileInfo[] encodingTestFiles = testFileDirectory.GetFiles();

            foreach (FileInfo file in encodingTestFiles)
            {
                string codePageString = file.Name.Substring(0, file.Name.IndexOf('_'));
                int codePage;
                if (int.TryParse(codePageString, out codePage))
                {
                    Encoding encoding = FileUtilities.GetEncoding(file.FullName);
                    Assert.AreEqual(
                        codePage,
                        encoding.CodePage,
                        "Unexpected Encoding.CodePage for encoding test file {0}.",
                        file.Name);
                }
                else
                {
                    Assert.Fail("Could not get code page value for encoding test file {0}.", file.Name);
                }
            }
        }

        #endregion Methods
    }
}