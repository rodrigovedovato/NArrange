namespace NArrange.Tests.Core.Configuration
{
    using System;

    using NArrange.Core;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the FilterExpressionParser class.
    /// </summary>
    [TestFixture]
    public class ConditionExpressionParserTests
    {
        #region Methods

        /// <summary>
        /// Tests parsing an AND expression.
        /// </summary>
        [Test]
        public void ParseAndExpressionTest()
        {
            Action<string> testExpression = delegate(string condition)
            {
                IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                    condition);
                Assert.IsNotNull(expression, "Expected an expression instance.");

                BinaryOperatorExpression operatorExpression = expression as BinaryOperatorExpression;
                Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.And, operatorExpression.Operator, "Unexpected operator.");

                //
                // Left
                //
                BinaryOperatorExpression leftExpression = expression.Left as BinaryOperatorExpression;
                Assert.IsNotNull(leftExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, leftExpression.Operator, "Unexpected operator.");

                ElementAttributeExpression leftAttributeExpression = leftExpression.Left as ElementAttributeExpression;
                Assert.IsNotNull(leftAttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, leftAttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression leftStringExpression = leftExpression.Right as StringExpression;
                Assert.IsNotNull(leftStringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 1", leftStringExpression.Text, "String expression was not parsed correctly.");

                //
                // Right
                //
                BinaryOperatorExpression rightExpression = expression.Right as BinaryOperatorExpression;
                Assert.IsNotNull(rightExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, rightExpression.Operator, "Unexpected operator.");

                ElementAttributeExpression rightAttributeExpression = rightExpression.Left as ElementAttributeExpression;
                Assert.IsNotNull(rightAttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, rightAttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression rightStringExpression = rightExpression.Right as StringExpression;
                Assert.IsNotNull(rightStringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 2", rightStringExpression.Text, "String expression was not parsed correctly.");
            };

            string expressionText;
            expressionText = "($(Name) == 'Test 1') And ($(Name) == 'Test 2')";
            testExpression(expressionText);

            expressionText = "$(Name) == 'Test 1' And $(Name) == 'Test 2'";
            testExpression(expressionText);
        }

        /// <summary>
        /// Tests parsing an expression.
        /// </summary>
        [Test]
        public void ParseAndOrExpressionTest()
        {
            Action<string> testExpression = delegate(string condition)
            {
                IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                    condition);
                Assert.IsNotNull(expression, "Expected an expression instance.");

                BinaryOperatorExpression operatorExpression = expression as BinaryOperatorExpression;
                Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.And, operatorExpression.Operator, "Unexpected operator.");

                //
                // And left
                //
                BinaryOperatorExpression testExpression3 = operatorExpression.Left as BinaryOperatorExpression;
                Assert.IsNotNull(testExpression3, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, testExpression3.Operator, "Unexpected operator.");

                ElementAttributeExpression test3AttributeExpression = testExpression3.Left as ElementAttributeExpression;
                Assert.IsNotNull(test3AttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, test3AttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression test3StringExpression = testExpression3.Right as StringExpression;
                Assert.IsNotNull(test3StringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 3", test3StringExpression.Text, "String expression was not parsed correctly.");

                //
                // And right
                //
                BinaryOperatorExpression orRightExpression = operatorExpression.Right as BinaryOperatorExpression;
                Assert.IsNotNull(orRightExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Or, orRightExpression.Operator, "Unexpected operator.");

                //
                // Or Left
                //
                BinaryOperatorExpression orLeftExpression = orRightExpression.Left as BinaryOperatorExpression;
                Assert.IsNotNull(orLeftExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, orLeftExpression.Operator, "Unexpected operator.");

                ElementAttributeExpression test1AttributeExpression = orLeftExpression.Left as ElementAttributeExpression;
                Assert.IsNotNull(test1AttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, test1AttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression test1StringExpression = orLeftExpression.Right as StringExpression;
                Assert.IsNotNull(test1StringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 1", test1StringExpression.Text, "String expression was not parsed correctly.");

                //
                // Or Right
                //
                BinaryOperatorExpression testExpression2 = orRightExpression.Right as BinaryOperatorExpression;
                Assert.IsNotNull(testExpression2, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, testExpression2.Operator, "Unexpected operator.");

                ElementAttributeExpression test2AttributeExpression = testExpression2.Left as ElementAttributeExpression;
                Assert.IsNotNull(test2AttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, test2AttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression test2StringExpression = testExpression2.Right as StringExpression;
                Assert.IsNotNull(test2StringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 2", test2StringExpression.Text, "String expression was not parsed correctly.");
            };

            string expressionText;
            expressionText = "($(Name) == 'Test 3') And (($(Name) == 'Test 1') Or ($(Name) == 'Test 2'))";
            testExpression(expressionText);

            expressionText = "$(Name) == 'Test 3' And ($(Name) == 'Test 1' Or $(Name) == 'Test 2')";
        }

        /// <summary>
        /// Tests parsing a complex negate expression.
        /// </summary>
        [Test]
        public void ParseComplexNegateTest()
        {
            string expression =
                "!($(Element.Name) : 'Test' Or $(Access) == 'Protected' Or " +
                "$(Access) == 'Private' And $(Element.Name) : 'OrAnd' And $(Type) == 'int')";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
            Assert.IsNotNull(conditionExpression, "Expected an expression instance.");

            string expressionString = conditionExpression.ToString();
            Assert.AreEqual(
                "!(((($(Element.Name) : 'Test') Or ($(Element.Access) == 'Protected')) Or " +
                "((($(Element.Access) == 'Private') And ($(Element.Name) : 'OrAnd')) And ($(Element.Type) == 'int'))))",
                expressionString,
                "Unexpected parsed expression.");

            // Parse the ToString representation and verify we get the same result
            conditionExpression = ConditionExpressionParser.Instance.Parse(
                expressionString);
            Assert.AreEqual(expressionString, conditionExpression.ToString(), "Unexpected parsed expression.");
        }

        /// <summary>
        /// Tests parsing a complex expression.
        /// </summary>
        [Test]
        public void ParseComplexTest()
        {
            string expression =
                "$(Element.Name) : 'Test' Or $(Access) == 'Protected' Or " +
                "$(Access) == 'Private' And $(Element.Name) : 'OrAnd' And $(Type) == 'int'";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
            Assert.IsNotNull(conditionExpression, "Expected an expression instance.");

            string expressionString = conditionExpression.ToString();
            Assert.AreEqual(
                "((($(Element.Name) : 'Test') Or ($(Element.Access) == 'Protected')) Or " +
                "((($(Element.Access) == 'Private') And ($(Element.Name) : 'OrAnd')) And ($(Element.Type) == 'int')))",
                expressionString,
                "Unexpected parsed expression.");

            // Parse the ToString representation and verify we get the same result
            conditionExpression = ConditionExpressionParser.Instance.Parse(
                expressionString);
            Assert.AreEqual(expressionString, conditionExpression.ToString(), "Unexpected parsed expression.");
        }

        /// <summary>
        /// Tests parsing a null expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseEmptyTest()
        {
            IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                string.Empty);
        }

        /// <summary>
        /// Tests parsing an equals expression.
        /// </summary>
        [Test]
        public void ParseEqualsExpressionTest()
        {
            string[] variations = new string[]
            {
                "$(Name) == 'Test'",
                "$(Name)   ==   'Test'",
                "($(Name) == 'Test')",
                "(($(Name) == 'Test'))"
            };

            foreach (string variation in variations)
            {
                try
                {
                    IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                        variation);
                    Assert.IsNotNull(expression, "Expected an expression instance for {0}", variation);

                    BinaryOperatorExpression operatorExpression = expression as BinaryOperatorExpression;
                    Assert.IsNotNull(operatorExpression, "Expected an operator expression for {0}", variation);
                    Assert.AreEqual(BinaryExpressionOperator.Equal, operatorExpression.Operator, "Unexpected operator");

                    ElementAttributeExpression attributeExpression = operatorExpression.Left as ElementAttributeExpression;
                    Assert.IsNotNull(attributeExpression, "Unexpected left node type.");
                    Assert.AreEqual(ElementAttributeType.Name, attributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");
                    Assert.AreEqual(ElementAttributeScope.Element, attributeExpression.Scope, "Attribute scope was not parsed correctly.");

                    StringExpression stringExpression = operatorExpression.Right as StringExpression;
                    Assert.IsNotNull(stringExpression, "Unexpected right node type.");
                    Assert.AreEqual("Test", stringExpression.Text, "String expression was not parsed correctly.");
                }
                catch (FormatException ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
        }

        /// <summary>
        /// Tests parsing a file name contains expression.
        /// </summary>
        [Test]
        public void ParseFileNameContainsExpressionTest()
        {
            string expressionText = "$(File.Name) : '.Designer.'";

            IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                expressionText);
            Assert.IsNotNull(expression, "Expected an expression instance.");

            BinaryOperatorExpression operatorExpression = expression as BinaryOperatorExpression;
            Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
            Assert.AreEqual(BinaryExpressionOperator.Contains, operatorExpression.Operator, "Unexpected operator.");

            FileAttributeExpression attributeExpression = operatorExpression.Left as FileAttributeExpression;
            Assert.IsNotNull(attributeExpression, "Unexpected left node type.");
            Assert.AreEqual(FileAttributeType.Name, attributeExpression.FileAttribute, "Attribute expression was not parsed correctly.");

            StringExpression stringExpression = operatorExpression.Right as StringExpression;
            Assert.IsNotNull(stringExpression, "Unexpected right node type.");
            Assert.AreEqual(".Designer.", stringExpression.Text, "String expression was not parsed correctly.");
        }

        /// <summary>
        /// Tests parsing a file name doesn't contain expression.
        /// </summary>
        [Test]
        public void ParseFileNotNameContainsExpressionTest()
        {
            string expressionText = "!($(File.Name) : '.Designer.')";

            IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                expressionText);
            Assert.IsNotNull(expression, "Expected an expression instance.");

            UnaryOperatorExpression negateExpression = expression as UnaryOperatorExpression;
            Assert.IsNotNull(negateExpression, "Expected a unary operator expression.");
            Assert.AreEqual(UnaryExpressionOperator.Negate, negateExpression.Operator, "Unexpected operator.");

            BinaryOperatorExpression operatorExpression = negateExpression.InnerExpression as BinaryOperatorExpression;
            Assert.IsNotNull(operatorExpression, "Expected a binary operator expression.");
            Assert.AreEqual(BinaryExpressionOperator.Contains, operatorExpression.Operator, "Unexpected operator.");

            FileAttributeExpression attributeExpression = operatorExpression.Left as FileAttributeExpression;
            Assert.IsNotNull(attributeExpression, "Unexpected left node type.");
            Assert.AreEqual(FileAttributeType.Name, attributeExpression.FileAttribute, "Attribute expression was not parsed correctly.");

            StringExpression stringExpression = operatorExpression.Right as StringExpression;
            Assert.IsNotNull(stringExpression, "Unexpected right node type.");
            Assert.AreEqual(".Designer.", stringExpression.Text, "String expression was not parsed correctly.");
        }

        /// <summary>
        /// Tests parsing an invalid attribute scope.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Unknown attribute scope")]
        public void ParseInvalidAttributeScopeTest()
        {
            string expression = "$(Foo.Name) : 'Test'";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid attribute name.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidAttributeTest()
        {
            string expression = "$(Or) : 'Test'";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid binary operator.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Invalid operator #")]
        public void ParseInvalidBinaryOperatorTest1()
        {
            string expression = "$(File.Name) # 'Or')";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid binary operator.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Invalid operator #")]
        public void ParseInvalidBinaryOperatorTest2()
        {
            string expression = "$(File.Name) : 'Or' # ($(File.Name) : 'And')";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid cased binary operator.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Invalid operator or")]
        public void ParseInvalidCasedBinaryOperatorTest()
        {
            string expression = "$(File.Name) : 'Or' or $(File.Name) : 'And'";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Unmatched )")]
        public void ParseInvalidExpressionExtraParenthesesTest1()
        {
            string expression = "($(Name) == 'Test'))";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Unmatched )")]
        public void ParseInvalidExpressionExtraParenthesesTest2()
        {
            string expression = "$(Name) == 'Test')";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidExpressionMissingOperatorTest()
        {
            string expression = "$(Name) == 'Test' $(Name) == 'Foo'";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Expected )")]
        public void ParseInvalidExpressionMissingParenthesesTest()
        {
            string expression = "($(Name) == 'Test'";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Expected '")]
        public void ParseInvalidExpressionMissingQuoteTest()
        {
            string expression = "$(Name) == 'Test";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidExpressionNoOperatorTest()
        {
            string expression = "$(Name)";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidExpressionNoTermsTest()
        {
            string expression = "()";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidExpressionStringTest()
        {
            string expression = "'Test'";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidExpressionTooManyTermsTest()
        {
            string expression = "$(Name) == 'Test' == $(Name)";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid file attribute.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Unknown attribute")]
        public void ParseInvalidFileAttributeScopeTest()
        {
            string expression = "$(File.Blah) : 'Test'";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid nested expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidNestedExpressionTest()
        {
            string expression = "$(File.Name) :  'Test' And ()";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing an invalid unary operator.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException),
            MatchType = MessageMatch.Contains,
            ExpectedMessage = "Invalid operator #")]
        public void ParseInvalidUnaryOperatorTest()
        {
            string expression = "#($(File.Name) : 'Or')";

            IConditionExpression conditionExpression = ConditionExpressionParser.Instance.Parse(
                expression);
        }

        /// <summary>
        /// Tests parsing a regular expression match expression.
        /// </summary>
        [Test]
        public void ParseMatchesExpressionTest()
        {
            string expressionText = "$(Name) =~ '.*\\.Tests\\..*'";

            IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                expressionText);
            Assert.IsNotNull(expression, "Expected an expression instance.");

            BinaryOperatorExpression operatorExpression = expression as BinaryOperatorExpression;
            Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
            Assert.AreEqual(BinaryExpressionOperator.Matches, operatorExpression.Operator, "Unexpected operator.");

            ElementAttributeExpression attributeExpression = operatorExpression.Left as ElementAttributeExpression;
            Assert.IsNotNull(attributeExpression, "Unexpected left node type.");
            Assert.AreEqual(ElementAttributeType.Name, attributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");
            Assert.AreEqual(ElementAttributeScope.Element, attributeExpression.Scope, "Attribute scope was not parsed correctly.");

            StringExpression stringExpression = operatorExpression.Right as StringExpression;
            Assert.IsNotNull(stringExpression, "Unexpected right node type.");
            Assert.AreEqual(".*\\.Tests\\..*", stringExpression.Text, "String expression was not parsed correctly.");
        }

        /// <summary>
        /// Tests parsing a negation expression.
        /// </summary>
        [Test]
        public void ParseNegateExpressionTest()
        {
            string[] variations = new string[]
            {
                "!($(Name) : 'Test')",
                "!(($(Name) : 'Test'))",
                "!((($(Name) : 'Test')))"
            };

            foreach (string variation in variations)
            {
                IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                    variation);
                Assert.IsNotNull(expression, "Expected an expression instance from {0}", variation);

                UnaryOperatorExpression negateExpression = expression as UnaryOperatorExpression;
                Assert.IsNotNull(negateExpression, "Expected a unary operator expression from {0}", variation);
                Assert.AreEqual(UnaryExpressionOperator.Negate, negateExpression.Operator, "Unexpected operator.");

                BinaryOperatorExpression binaryOperatorExpression = negateExpression.InnerExpression as BinaryOperatorExpression;
                Assert.IsNotNull(binaryOperatorExpression, "Expected a binary operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Contains, binaryOperatorExpression.Operator, "Unexpected operator.");

                ElementAttributeExpression attributeExpression = binaryOperatorExpression.Left as ElementAttributeExpression;
                Assert.IsNotNull(attributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, attributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");
                Assert.AreEqual(ElementAttributeScope.Element, attributeExpression.Scope, "Attribute scope was not parsed correctly.");

                StringExpression stringExpression = binaryOperatorExpression.Right as StringExpression;
                Assert.IsNotNull(stringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test", stringExpression.Text, "String expression was not parsed correctly.");
            }
        }

        /// <summary>
        /// Tests parsing a null expression.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseNullTest()
        {
            IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                null);
        }

        /// <summary>
        /// Tests parsing an expression.
        /// </summary>
        [Test]
        public void ParseOrAndExpressionTest1()
        {
            Action<string> testExpression = delegate(string condition)
            {
                IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                    condition);
                Assert.IsNotNull(expression, "Expected an expression instance.");

                BinaryOperatorExpression operatorExpression = expression as BinaryOperatorExpression;
                Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Or, operatorExpression.Operator, "Unexpected operator.");

                //
                // Or left
                //
                BinaryOperatorExpression andExpression = operatorExpression.Left as BinaryOperatorExpression;
                Assert.IsNotNull(andExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.And, andExpression.Operator, "Unexpected operator.");

                //
                // And Left
                //
                BinaryOperatorExpression testExpression1 = andExpression.Left as BinaryOperatorExpression;
                Assert.IsNotNull(testExpression1, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, testExpression1.Operator, "Unexpected operator.");

                ElementAttributeExpression test1AttributeExpression = testExpression1.Left as ElementAttributeExpression;
                Assert.IsNotNull(test1AttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, test1AttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression test1StringExpression = testExpression1.Right as StringExpression;
                Assert.IsNotNull(test1StringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 1", test1StringExpression.Text, "String expression was not parsed correctly.");

                //
                // And Right
                //
                BinaryOperatorExpression testExpression2 = andExpression.Right as BinaryOperatorExpression;
                Assert.IsNotNull(testExpression2, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, testExpression2.Operator, "Unexpected operator.");

                ElementAttributeExpression test2AttributeExpression = testExpression2.Left as ElementAttributeExpression;
                Assert.IsNotNull(test2AttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, test2AttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression test2StringExpression = testExpression2.Right as StringExpression;
                Assert.IsNotNull(test2StringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 2", test2StringExpression.Text, "String expression was not parsed correctly.");

                //
                // Or right
                //
                BinaryOperatorExpression testExpression3 = operatorExpression.Right as BinaryOperatorExpression;
                Assert.IsNotNull(testExpression3, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, testExpression3.Operator, "Unexpected operator.");

                ElementAttributeExpression test3AttributeExpression = testExpression3.Left as ElementAttributeExpression;
                Assert.IsNotNull(test3AttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, test3AttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression test3StringExpression = testExpression3.Right as StringExpression;
                Assert.IsNotNull(test3StringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 3", test3StringExpression.Text, "String expression was not parsed correctly.");
            };

            string expressionText;
            expressionText = "(($(Name) == 'Test 1') And ($(Name) == 'Test 2')) Or ($(Name) == 'Test 3')";
            testExpression(expressionText);

            expressionText = "$(Name) == 'Test 1' And $(Name) == 'Test 2' Or $(Name) == 'Test 3'";
            testExpression(expressionText);
        }

        /// <summary>
        /// Tests parsing an expression.
        /// </summary>
        [Test]
        public void ParseOrAndExpressionTest2()
        {
            Action<string> testExpression = delegate(string condition)
            {
                IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                    condition);
                Assert.IsNotNull(expression, "Expected an expression instance.");

                BinaryOperatorExpression operatorExpression = expression as BinaryOperatorExpression;
                Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Or, operatorExpression.Operator, "Unexpected operator.");

                //
                // Or left
                //
                BinaryOperatorExpression testExpression1 = operatorExpression.Left as BinaryOperatorExpression;
                Assert.IsNotNull(testExpression1, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, testExpression1.Operator, "Unexpected operator.");

                ElementAttributeExpression test1AttributeExpression = testExpression1.Left as ElementAttributeExpression;
                Assert.IsNotNull(test1AttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, test1AttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression test1StringExpression = testExpression1.Right as StringExpression;
                Assert.IsNotNull(test1StringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 1", test1StringExpression.Text, "String expression was not parsed correctly.");

                //
                // Or right
                //
                BinaryOperatorExpression andExpression = operatorExpression.Right as BinaryOperatorExpression;
                Assert.IsNotNull(andExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.And, andExpression.Operator, "Unexpected operator.");

                //
                // And Left
                //
                BinaryOperatorExpression testExpression2 = andExpression.Left as BinaryOperatorExpression;
                Assert.IsNotNull(testExpression2, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, testExpression2.Operator, "Unexpected operator.");

                ElementAttributeExpression test2AttributeExpression = testExpression2.Left as ElementAttributeExpression;
                Assert.IsNotNull(test2AttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, test2AttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression test2StringExpression = testExpression2.Right as StringExpression;
                Assert.IsNotNull(test2StringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 2", test2StringExpression.Text, "String expression was not parsed correctly.");

                //
                // And Right
                //
                BinaryOperatorExpression testExpression3 = andExpression.Right as BinaryOperatorExpression;
                Assert.IsNotNull(testExpression3, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, testExpression3.Operator, "Unexpected operator.");

                ElementAttributeExpression test3AttributeExpression = testExpression3.Left as ElementAttributeExpression;
                Assert.IsNotNull(test3AttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, test3AttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression test3StringExpression = testExpression3.Right as StringExpression;
                Assert.IsNotNull(test3StringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 3", test3StringExpression.Text, "String expression was not parsed correctly.");
            };

            string expressionText;
            expressionText = "(($(Name) == 'Test 1') Or (($(Name) == 'Test 2') And ($(Name) == 'Test 3')))";
            testExpression(expressionText);

            expressionText = "$(Name) == 'Test 1' Or $(Name) == 'Test 2' And $(Name) == 'Test 3'";
            testExpression(expressionText);
        }

        /// <summary>
        /// Tests parsing an OR expression.
        /// </summary>
        [Test]
        public void ParseOrExpressionTest()
        {
            Action<string> testExpression = delegate(string condition)
            {
                IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                    condition);
                Assert.IsNotNull(expression, "Expected an expression instance.");

                BinaryOperatorExpression operatorExpression = expression as BinaryOperatorExpression;
                Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Or, operatorExpression.Operator, "Unexpected operator.");

                //
                // Left
                //
                BinaryOperatorExpression leftExpression = expression.Left as BinaryOperatorExpression;
                Assert.IsNotNull(leftExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, leftExpression.Operator, "Unexpected operator.");

                ElementAttributeExpression leftAttributeExpression = leftExpression.Left as ElementAttributeExpression;
                Assert.IsNotNull(leftAttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, leftAttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression leftStringExpression = leftExpression.Right as StringExpression;
                Assert.IsNotNull(leftStringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 1", leftStringExpression.Text, "String expression was not parsed correctly.");

                //
                // Right
                //
                BinaryOperatorExpression rightExpression = expression.Right as BinaryOperatorExpression;
                Assert.IsNotNull(rightExpression, "Expected an operator expression.");
                Assert.AreEqual(BinaryExpressionOperator.Equal, rightExpression.Operator, "Unexpected operator.");

                ElementAttributeExpression rightAttributeExpression = rightExpression.Left as ElementAttributeExpression;
                Assert.IsNotNull(rightAttributeExpression, "Unexpected left node type.");
                Assert.AreEqual(ElementAttributeType.Name, rightAttributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");

                StringExpression rightStringExpression = rightExpression.Right as StringExpression;
                Assert.IsNotNull(rightStringExpression, "Unexpected right node type.");
                Assert.AreEqual("Test 2", rightStringExpression.Text, "String expression was not parsed correctly.");
            };

            string expressionText;
            expressionText = "($(Name) == 'Test 1') Or ($(Name) == 'Test 2')";
            testExpression(expressionText);

            expressionText = "$(Name) == 'Test 1' Or $(Name) == 'Test 2'";
            testExpression(expressionText);
        }

        /// <summary>
        /// Tests parsing a parent name contains expression.
        /// </summary>
        [Test]
        public void ParseParentNameContainsExpressionTest()
        {
            string expressionText = "$(Parent.Name) : 'Converter'";

            IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                expressionText);
            Assert.IsNotNull(expression, "Expected an expression instance.");

            BinaryOperatorExpression operatorExpression = expression as BinaryOperatorExpression;
            Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
            Assert.AreEqual(BinaryExpressionOperator.Contains, operatorExpression.Operator, "Unexpected operator.");

            ElementAttributeExpression attributeExpression = operatorExpression.Left as ElementAttributeExpression;
            Assert.IsNotNull(attributeExpression, "Unexpected left node type.");
            Assert.AreEqual(ElementAttributeType.Name, attributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");
            Assert.AreEqual(ElementAttributeScope.Parent, attributeExpression.Scope, "Attribute scope was not parsed correctly.");

            StringExpression stringExpression = operatorExpression.Right as StringExpression;
            Assert.IsNotNull(stringExpression, "Unexpected right node type.");
            Assert.AreEqual("Converter", stringExpression.Text, "String expression was not parsed correctly.");
        }

        /// <summary>
        /// Tests parsing a string expression with an escaped apostrophe.
        /// </summary>
        [Test]
        public void ParseStringExpressionEscapedApostropheEndTest()
        {
            string expressionText = "$(Name) == 'Test it'''";

            IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                expressionText);
            Assert.IsNotNull(expression, "Expected an expression instance.");

            BinaryOperatorExpression operatorExpression = expression as BinaryOperatorExpression;
            Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
            Assert.AreEqual(BinaryExpressionOperator.Equal, operatorExpression.Operator, "Unexpected operator.");

            ElementAttributeExpression attributeExpression = operatorExpression.Left as ElementAttributeExpression;
            Assert.IsNotNull(attributeExpression, "Unexpected left node type.");
            Assert.AreEqual(ElementAttributeType.Name, attributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");
            Assert.AreEqual(ElementAttributeScope.Element, attributeExpression.Scope, "Attribute scope was not parsed correctly.");

            StringExpression stringExpression = operatorExpression.Right as StringExpression;
            Assert.IsNotNull(stringExpression, "Expected a string expression.");
            Assert.AreEqual("Test it'", stringExpression.Text, "Unexpected expression text.");
        }

        /// <summary>
        /// Tests parsing a string expression with an escaped apostrophe.
        /// </summary>
        [Test]
        public void ParseStringExpressionEscapedApostropheTest()
        {
            string expressionText = "$(Name) == 'Test '' it'";

            IConditionExpression expression = ConditionExpressionParser.Instance.Parse(
                expressionText);
            Assert.IsNotNull(expression, "Expected an expression instance.");

            BinaryOperatorExpression operatorExpression = expression as BinaryOperatorExpression;
            Assert.IsNotNull(operatorExpression, "Expected an operator expression.");
            Assert.AreEqual(BinaryExpressionOperator.Equal, operatorExpression.Operator, "Unexpected operator.");

            ElementAttributeExpression attributeExpression = operatorExpression.Left as ElementAttributeExpression;
            Assert.IsNotNull(attributeExpression, "Unexpected left node type.");
            Assert.AreEqual(ElementAttributeType.Name, attributeExpression.ElementAttribute, "Attribute expression was not parsed correctly.");
            Assert.AreEqual(ElementAttributeScope.Element, attributeExpression.Scope, "Attribute scope was not parsed correctly.");

            StringExpression stringExpression = operatorExpression.Right as StringExpression;
            Assert.IsNotNull(stringExpression, "Expected a string expression.");
            Assert.AreEqual("Test ' it", stringExpression.Text, "Unexpected expression text.");
        }

        #endregion Methods
    }
}