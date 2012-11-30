namespace NArrange.Tests.Core
{
    using System;
    using System.IO;

    using NArrange.Core;
    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ConditionExpressionEvaluator class.
    /// </summary>
    [TestFixture]
    public class ConditionExpressionEvaluatorTests
    {
        #region Methods

        /// <summary>
        /// Tests the Evaluate method with an And expression.
        /// </summary>
        [Test]
        public void EvaluateAndTest()
        {
            IConditionExpression nameExpression = new BinaryOperatorExpression(
                BinaryExpressionOperator.Equal,
                new ElementAttributeExpression(ElementAttributeType.Name),
                new StringExpression("Test"));

            IConditionExpression attributeExpression = new BinaryOperatorExpression(
                BinaryExpressionOperator.Equal,
               new ElementAttributeExpression(ElementAttributeType.Access),
               new StringExpression("Protected"));

            IConditionExpression expression = new BinaryOperatorExpression(
               BinaryExpressionOperator.And, nameExpression, attributeExpression);

            FieldElement element = new FieldElement();
            element.Name = "Test";
            element.Access = CodeAccess.Protected;

            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");

            element.Name = "Foo";
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsFalse(result, "Unexpected expression evaluation result.");

            element.Name = "Test";
            element.Access = CodeAccess.Private;
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsFalse(result, "Unexpected expression evaluation result.");
        }

        /// <summary>
        /// Tests the Evaluate method with a null expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EvaluateElementExpressionNullTest()
        {
            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                null, new FieldElement());
        }

        /// <summary>
        /// Tests the Evaluate method with a Contains expression.
        /// </summary>
        [Test]
        public void EvaluateElementNameContainsTest()
        {
            IConditionExpression expression = new BinaryOperatorExpression(
               BinaryExpressionOperator.Contains,
               new ElementAttributeExpression(ElementAttributeType.Name),
               new StringExpression("Test"));

            FieldElement element = new FieldElement();
            element.Name = "Test";

            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");

            element.Name = "OnTest1";
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");

            element.Name = "Foo";
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsFalse(result, "Unexpected expression evaluation result.");
        }

        /// <summary>
        /// Tests the Evaluate method with an Equal expression.
        /// </summary>
        [Test]
        public void EvaluateElementNameEqualTest()
        {
            IConditionExpression expression = new BinaryOperatorExpression(
               BinaryExpressionOperator.Equal,
               new ElementAttributeExpression(ElementAttributeType.Name),
               new StringExpression("Test"));

            FieldElement element = new FieldElement();
            element.Name = "Test";

            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");

            element.Name = "Test1";
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsFalse(result, "Unexpected expression evaluation result.");
        }

        /// <summary>
        /// Tests the Evaluate method with a Matches expression.
        /// </summary>
        [Test]
        public void EvaluateElementNameMatchesTest()
        {
            IConditionExpression expression = new BinaryOperatorExpression(
               BinaryExpressionOperator.Matches,
               new ElementAttributeExpression(ElementAttributeType.Name),
               new StringExpression("IDisposable\\..*"));

            MethodElement element = new MethodElement();
            element.Name = "IDisposable.Dispose";

            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");

            element.Name = "IDisposable.Test";
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");

            element.Name = "IDisposable";
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsFalse(result, "Unexpected expression evaluation result.");
        }

        /// <summary>
        /// Tests the Evaluate method with a not equal expression.
        /// </summary>
        [Test]
        public void EvaluateElementNameNotEqualTest()
        {
            IConditionExpression expression = new BinaryOperatorExpression(
               BinaryExpressionOperator.NotEqual,
               new ElementAttributeExpression(ElementAttributeType.Name),
               new StringExpression("Test"));

            FieldElement element = new FieldElement();
            element.Name = "Test";

            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsFalse(result, "Unexpected expression evaluation result.");

            element.Name = "Temp";
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");
        }

        /// <summary>
        /// Tests the Evaluate method with a null element.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EvaluateElementNullTest()
        {
            IConditionExpression expression = new BinaryOperatorExpression(
                BinaryExpressionOperator.Equal,
                new ElementAttributeExpression(ElementAttributeType.Name),
                new StringExpression("Test"));

            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, null as ICodeElement);
        }

        /// <summary>
        /// Tests the Evaluate method with a Contains expression and a parent scope.
        /// </summary>
        [Test]
        public void EvaluateElementParentAttributesContainsTest()
        {
            IConditionExpression expression = new BinaryOperatorExpression(
               BinaryExpressionOperator.Contains,
               new ElementAttributeExpression(ElementAttributeType.Attributes, ElementAttributeScope.Parent),
               new StringExpression("Attribute2"));

            FieldElement element = new FieldElement();
            element.Name = "Test";

            TypeElement typeElement = new TypeElement();
            typeElement.Type = TypeElementType.Structure;
            typeElement.Name = "TestType";
            typeElement.AddChild(element);

            typeElement.AddAttribute(new AttributeElement("Attribute1"));
            typeElement.AddAttribute(new AttributeElement("Attribute24"));

            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");

            typeElement.ClearAttributes();
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsFalse(result, "Unexpected expression evaluation result.");
        }

        /// <summary>
        /// Tests the Evaluate method with an Attributes Contains expression.
        /// </summary>
        [Test]
        public void EvaluateFileAttributesContainsTest()
        {
            string testFile = Path.GetTempFileName();
            try
            {
                IConditionExpression expression = new BinaryOperatorExpression(
                   BinaryExpressionOperator.Contains,
                   new FileAttributeExpression(FileAttributeType.Attributes),
                   new StringExpression("ReadOnly"));

                FileInfo file = new FileInfo(testFile);
                file.Attributes = FileAttributes.ReadOnly | FileAttributes.Hidden;

                bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                    expression, file);
                Assert.IsTrue(result, "Unexpected expression evaluation result.");

                file.Attributes = FileAttributes.Normal;
                file = new FileInfo(testFile);
                result = ConditionExpressionEvaluator.Instance.Evaluate(
                    expression, file);
                Assert.IsFalse(result, "Unexpected expression evaluation result.");
            }
            finally
            {
                try
                {
                    File.Delete(testFile);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Tests the Evaluate method with a null expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EvaluateFileExpressionNullTest()
        {
            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                null, new FileInfo("Test"));
        }

        /// <summary>
        /// Tests the Evaluate method with a Contains expression.
        /// </summary>
        [Test]
        public void EvaluateFileNameContainsTest()
        {
            IConditionExpression expression = new BinaryOperatorExpression(
               BinaryExpressionOperator.Contains,
               new FileAttributeExpression(FileAttributeType.Name),
               new StringExpression("Test"));

            FileInfo file = new FileInfo("Test");

            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, file);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");

            file = new FileInfo("Blah");
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, file);
            Assert.IsFalse(result, "Unexpected expression evaluation result.");
        }

        /// <summary>
        /// Tests the Evaluate method with a null file.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EvaluateFileNullTest()
        {
            IConditionExpression expression = new BinaryOperatorExpression(
                BinaryExpressionOperator.Equal,
                new FileAttributeExpression(FileAttributeType.Name),
                new StringExpression("Test"));

            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, null as FileInfo);
        }

        /// <summary>
        /// Tests the Evaluate method with an Path Contains expression.
        /// </summary>
        [Test]
        public void EvaluateFilePathContainsTest()
        {
            string testFile1 = Path.GetTempFileName();
            string testFile2 = Path.GetTempFileName();
            try
            {
                IConditionExpression expression = new BinaryOperatorExpression(
                   BinaryExpressionOperator.Contains,
                   new FileAttributeExpression(FileAttributeType.Path),
                   new StringExpression(Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(testFile1))));

                FileInfo file = new FileInfo(testFile1);

                bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                    expression, file);
                Assert.IsTrue(result, "Unexpected expression evaluation result.");

                file = new FileInfo(testFile2);
                result = ConditionExpressionEvaluator.Instance.Evaluate(
                    expression, file);
                Assert.IsFalse(result, "Unexpected expression evaluation result.");
            }
            finally
            {
                try
                {
                    File.Delete(testFile1);
                    File.Delete(testFile2);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Tests the Evaluate method with a an operator element that has an 
        /// unknown operator type.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EvaluateInvalidOperatorTest()
        {
            IConditionExpression expression = new BinaryOperatorExpression(
                (BinaryExpressionOperator)int.MinValue,
                new ElementAttributeExpression(ElementAttributeType.Name),
                new StringExpression("Test"));

            FieldElement element = new FieldElement();
            element.Name = "Test";

            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
        }

        /// <summary>
        /// Tests the Evaluate method with an Or expression.
        /// </summary>
        [Test]
        public void EvaluateOrTest()
        {
            IConditionExpression nameExpression = new BinaryOperatorExpression(
                BinaryExpressionOperator.Equal,
                new ElementAttributeExpression(ElementAttributeType.Name),
                new StringExpression("Test"));

            IConditionExpression accessExpression = new BinaryOperatorExpression(
                BinaryExpressionOperator.Equal,
                new ElementAttributeExpression(ElementAttributeType.Access),
                new StringExpression("Protected"));

            IConditionExpression expression =
               new BinaryOperatorExpression(BinaryExpressionOperator.Or, nameExpression, accessExpression);

            FieldElement element = new FieldElement();
            element.Name = "Test";
            element.Access = CodeAccess.Protected;

            bool result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");

            element.Name = "Foo";
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");

            element.Name = "Test";
            element.Access = CodeAccess.Private;
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsTrue(result, "Unexpected expression evaluation result.");

            element.Name = "Foo";
            result = ConditionExpressionEvaluator.Instance.Evaluate(
                expression, element);
            Assert.IsFalse(result, "Unexpected expression evaluation result.");
        }

        #endregion Methods
    }
}