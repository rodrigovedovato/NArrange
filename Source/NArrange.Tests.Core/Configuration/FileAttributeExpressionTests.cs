namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the FileAttributeExpression class.
    /// </summary>
    [TestFixture]
    public class FileAttributeExpressionTests
    {
        #region Methods

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            FileAttributeExpression expression = new FileAttributeExpression(FileAttributeType.Path);

            Assert.AreEqual("$(File.Path)", expression.ToString(), "Unexpected string representation.");
        }

        #endregion Methods
    }
}