namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the binary operator expression class.
    /// </summary>
    [TestFixture]
    public class BinaryOperatorExpressionTests
    {
        #region Methods

        /// <summary>
        /// Gets the string representation of the operator expression with an invalid operator type.
        /// </summary>
        [Test]
        public void ToStringInvalidOperatorTest()
        {
            ElementAttributeExpression attributeExpression = new ElementAttributeExpression(ElementAttributeType.Name);
            StringExpression stringExpression = new StringExpression("Test");
            BinaryOperatorExpression operatorExpression = new BinaryOperatorExpression(
                (BinaryExpressionOperator)int.MinValue,
                attributeExpression,
                stringExpression);

            Assert.AreEqual(string.Format("($(Element.Name) {0} 'Test')", int.MinValue), operatorExpression.ToString());
        }

        /// <summary>
        /// Gets the string representation of the operator expression.
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            ElementAttributeExpression attributeExpression = new ElementAttributeExpression(
                ElementAttributeType.Name);
            StringExpression stringExpression = new StringExpression("Test");
            BinaryOperatorExpression operatorExpression = new BinaryOperatorExpression(
                BinaryExpressionOperator.Equal,
                attributeExpression,
                stringExpression);

            Assert.AreEqual("($(Element.Name) == 'Test')", operatorExpression.ToString());
        }

        #endregion Methods
    }
}