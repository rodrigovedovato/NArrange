namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the unary operator expression class.
    /// </summary>
    [TestFixture]
    public class UnaryOperatorExpressionTests
    {
        #region Methods

        /// <summary>
        /// Gets the string representation of the operator expression with an invalid operator type.
        /// </summary>
        [Test]
        public void ToStringInvalidOperatorTest()
        {
            ElementAttributeExpression attributeExpression = new ElementAttributeExpression(
                ElementAttributeType.Name);
            StringExpression stringExpression = new StringExpression("Test");
            BinaryOperatorExpression equalsExpression = new BinaryOperatorExpression(
                BinaryExpressionOperator.Equal,
                attributeExpression,
                stringExpression);
            UnaryOperatorExpression operatorExpression = new UnaryOperatorExpression(
                (UnaryExpressionOperator)int.MinValue,
                equalsExpression);

            Assert.AreEqual(string.Format("{0}(($(Element.Name) == 'Test'))", int.MinValue), operatorExpression.ToString());
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
            BinaryOperatorExpression equalsExpression = new BinaryOperatorExpression(
                BinaryExpressionOperator.Equal,
                attributeExpression,
                stringExpression);
            UnaryOperatorExpression operatorExpression = new UnaryOperatorExpression(
                UnaryExpressionOperator.Negate,
                equalsExpression);

            Assert.AreEqual("!(($(Element.Name) == 'Test'))", operatorExpression.ToString());
        }

        #endregion Methods
    }
}