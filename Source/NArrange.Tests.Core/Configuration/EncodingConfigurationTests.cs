namespace NArrange.Tests.Core.Configuration
{
    using System;
    using System.Text;

    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the EncodingConfiguration class.
    /// </summary>
    [TestFixture]
    public class EncodingConfigurationTests
    {
        #region Methods

        /// <summary>
        /// Tests the ICloneable implementation.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            EncodingConfiguration encodingConfiguration = new EncodingConfiguration();
            encodingConfiguration.CodePage = "1252";

            EncodingConfiguration clone = encodingConfiguration.Clone() as EncodingConfiguration;
            Assert.IsNotNull(clone, "Clone did not return a valid instance.");

            Assert.AreEqual(encodingConfiguration.CodePage, clone.CodePage);
        }

        /// <summary>
        /// Tests the creation of a new EncodingConfiguration.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            EncodingConfiguration encodingConfiguration = new EncodingConfiguration();

            //
            // Verify default state
            //
            Assert.AreEqual(EncodingConfiguration.DetectCodePage, encodingConfiguration.CodePage, "Unexpected default value for CodePage.");
        }

        /// <summary>
        /// Tests GetEncoding() with an invalid codepage.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void GetEncodingInvalidFormatTest()
        {
            EncodingConfiguration config = new EncodingConfiguration();
            config.CodePage = "SFDS";

            config.GetEncoding();
        }

        /// <summary>
        /// Tests valid encoding scenarios.
        /// </summary>
        [Test]
        public void GetEncodingTest()
        {
            EncodingConfiguration config = new EncodingConfiguration();
            Encoding encoding = null;

            config.CodePage = "Default";
            encoding = config.GetEncoding();
            Assert.AreEqual(Encoding.Default.CodePage, encoding.CodePage, "Unexpected codepage.");
            config.CodePage = "default";
            encoding = config.GetEncoding();
            Assert.AreEqual(Encoding.Default.CodePage, encoding.CodePage, "Unexpected codepage.");

            config.CodePage = "65001";
            encoding = config.GetEncoding();
            Assert.AreEqual(65001, encoding.CodePage, "Unexpected codepage.");

            config.CodePage = "Detect";
            encoding = config.GetEncoding();
            Assert.IsNull(encoding, "Unexpected encoding.");
            config.CodePage = "detect";
            encoding = config.GetEncoding();
            Assert.IsNull(encoding, "Unexpected encoding.");
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            EncodingConfiguration encodingConfiguration = new EncodingConfiguration();
            encodingConfiguration.CodePage = "65001";

            string str = encodingConfiguration.ToString();

            Assert.AreEqual("Encoding: CodePage - 65001", str, "Unexpected string representation.");
        }

        #endregion Methods
    }
}