#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2007-2008 James Nies and NArrange contributors.
 *    All rights reserved.
 *
 * This program and the accompanying materials are made available under
 * the terms of the Common Public License v1.0 which accompanies this
 * distribution.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in
 * the documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *<author>James Nies</author>
 *<contributor>Justin Dearing</contributor>
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Class for parsing filter expressions.
    /// </summary>
    public sealed class ConditionExpressionParser
    {
        #region Fields

        /// <summary>
        /// Expression end.
        /// </summary>
        public const char ExpressionEnd = ')';

        /// <summary>
        /// Character that marks the start of an attribute expression.
        /// </summary>
        public const char ExpressionPrefix = '$';

        /// <summary>
        /// Expression start.
        /// </summary>
        public const char ExpressionStart = '(';

        /// <summary>
        /// File attribute scope.
        /// </summary>
        public const string FileAttributeScope = "File";

        /// <summary>
        /// Scope separator.
        /// </summary>
        public const char ScopeSeparator = '.';

        /// <summary>
        /// Singleton sych lock.
        /// </summary>
        private static readonly object _instanceLock = new object();

        /// <summary>
        /// Singleton instance field.
        /// </summary>
        private static ConditionExpressionParser _instance;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new FilterExpressionParser.
        /// </summary>
        private ConditionExpressionParser()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the single instance of the expression parser.
        /// </summary>
        public static ConditionExpressionParser Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConditionExpressionParser();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Parses and expression to an expression tree.
        /// </summary>
        /// <param name="expression">Condition expression text.</param>
        /// <returns>A condition expression instance.</returns>
        public IConditionExpression Parse(string expression)
        {
            const int DefaultExpressionLength = 128;
            IConditionExpression conditionExpression = null;

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else if (expression.Trim().Length == 0)
            {
                throw new ArgumentException("expression");
            }

            List<IConditionExpression> nodes = new List<IConditionExpression>();

            StringReader reader = new StringReader(expression);

            StringBuilder expressionBuilder = new StringBuilder(DefaultExpressionLength);

            bool inString = false;
            bool inAttribute = false;
            int depth = 0;

            int data = reader.Read();
            while (data > 0)
            {
                char ch = (char)data;
                char nextCh = (char)reader.Peek();

                if (inString && ch != '\'')
                {
                    expressionBuilder.Append(ch);
                }
                else
                {
                    switch (ch)
                    {
                        case ' ':
                        case '\t':
                        case '\r':
                        case '\n':
                            // Eat whitespace
                            break;

                        case ExpressionPrefix:
                            CheckForInvalidOperator(expression, expressionBuilder);
                            if (nextCh == ExpressionStart)
                            {
                                inAttribute = true;
                                reader.Read();
                            }
                            break;

                        case '=':
                            if (nextCh == '=')
                            {
                                nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.Equal));
                                reader.Read();
                            }
                            else if (nextCh == '~')
                            {
                                nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.Matches));
                                reader.Read();
                            }
                            break;

                        case '!':
                            if (nextCh == '=')
                            {
                                nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.NotEqual));
                                reader.Read();
                            }
                            else
                            {
                                expressionBuilder.Append(ch);
                            }
                            break;

                        case ':':
                            nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.Contains));
                            reader.Read();
                            break;

                        case 'O':
                            if (nextCh == 'r' && !inAttribute && !inString)
                            {
                                nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.Or));
                                reader.Read();
                            }
                            else
                            {
                                expressionBuilder.Append(ch);
                            }
                            break;

                        case 'A':
                            if (nextCh == 'n' && !inAttribute && !inString)
                            {
                                reader.Read();
                                nextCh = (char)reader.Peek();
                                if (nextCh == 'd')
                                {
                                    nodes.Add(new OperatorExpressionPlaceholder(BinaryExpressionOperator.And));
                                    reader.Read();
                                }
                            }
                            else
                            {
                                expressionBuilder.Append(ch);
                            }
                            break;

                        case ExpressionEnd:
                            if (inAttribute)
                            {
                                string attribute = expressionBuilder.ToString();
                                expressionBuilder = new StringBuilder(DefaultExpressionLength);
                                ElementAttributeScope elementScope = ElementAttributeScope.Element;
                                bool isFileExpression = false;

                                int separatorIndex = attribute.LastIndexOf(ScopeSeparator);
                                if (separatorIndex > 0)
                                {
                                    try
                                    {
                                        string attributeScope = attribute.Substring(0, separatorIndex);
                                        attribute = attribute.Substring(separatorIndex + 1);

                                        if (attributeScope == FileAttributeScope)
                                        {
                                            isFileExpression = true;
                                        }
                                        else
                                        {
                                            elementScope = (ElementAttributeScope)
                                                Enum.Parse(typeof(ElementAttributeScope), attributeScope);
                                        }
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        OnInvalidExpression(expression, "Unknown attribute scope: {0}", ex.Message);
                                    }
                                }

                                if (isFileExpression)
                                {
                                    FileAttributeType fileAttribute = FileAttributeType.None;

                                    try
                                    {
                                        fileAttribute = (FileAttributeType)
                                            Enum.Parse(typeof(FileAttributeType), attribute);
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        OnInvalidExpression(expression, "Unknown attribute: {0}", ex.Message);
                                    }

                                    FileAttributeExpression attributeExpresion = new FileAttributeExpression(
                                        fileAttribute);
                                    nodes.Add(attributeExpresion);
                                }
                                else
                                {
                                    ElementAttributeType elementAttribute = ElementAttributeType.None;

                                    try
                                    {
                                        elementAttribute = (ElementAttributeType)
                                            Enum.Parse(typeof(ElementAttributeType), attribute);
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        OnInvalidExpression(expression, "Unknown attribute: {0}", ex.Message);
                                    }

                                    ElementAttributeExpression attributeExpresion = new ElementAttributeExpression(
                                        elementAttribute, elementScope);
                                    nodes.Add(attributeExpresion);
                                }

                                inAttribute = false;
                            }
                            else if (expressionBuilder.Length > 0 && nodes.Count > 0)
                            {
                                IConditionExpression innerExpression = nodes[nodes.Count - 1];
                                nodes.RemoveAt(nodes.Count - 1);

                                string unaryOperatorString = expressionBuilder.ToString().Trim();
                                expressionBuilder = new StringBuilder(DefaultExpressionLength);

                                UnaryExpressionOperator? unaryOperator = null;

                                if (unaryOperatorString == "!")
                                {
                                    unaryOperator = UnaryExpressionOperator.Negate;
                                }
                                else
                                {
                                    OnInvalidExpression(expression,
                                        "Invalid operator {0}", unaryOperatorString);
                                }

                                UnaryOperatorExpression unaryOperatorExpression = new UnaryOperatorExpression(
                                    unaryOperator.Value, innerExpression);

                                nodes.Add(unaryOperatorExpression);
                                depth--;
                            }
                            else
                            {
                                depth--;
                            }
                            break;

                        case ExpressionStart:
                            IConditionExpression nestedExpression = null;
                            StringBuilder childExpressionBuilder = new StringBuilder(DefaultExpressionLength);
                            data = reader.Read();
                            ch = (char)data;
                            nextCh = (char)reader.Peek();
                            depth++;
                            while (data > 0)
                            {
                                if (ch == ExpressionPrefix && nextCh == ExpressionStart)
                                {
                                    inAttribute = true;
                                    childExpressionBuilder.Append(ExpressionPrefix);
                                    data = reader.Read();
                                    childExpressionBuilder.Append(ExpressionStart);
                                }
                                else if (ch == ExpressionStart && !inAttribute)
                                {
                                    depth++;
                                    childExpressionBuilder.Append(ExpressionStart);
                                }
                                else if (nextCh == ExpressionEnd)
                                {
                                    childExpressionBuilder.Append(ch);

                                    if (inAttribute || depth > 1)
                                    {
                                        if (inAttribute)
                                        {
                                            inAttribute = false;
                                        }
                                        else if (depth > 1)
                                        {
                                            depth--;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    childExpressionBuilder.Append(ch);
                                }

                                data = reader.Read();
                                ch = (char)data;
                                nextCh = (char)reader.Peek();
                            }

                            try
                            {
                                nestedExpression = Parse(childExpressionBuilder.ToString());
                            }
                            catch (ArgumentException)
                            {
                                OnInvalidExpression(expression);
                            }
                            nodes.Add(nestedExpression);
                            break;

                        case '\'':
                            if (inString)
                            {
                                if (nextCh == '\'')
                                {
                                    expressionBuilder.Append(ch);
                                    reader.Read();
                                }
                                else
                                {
                                    string str = expressionBuilder.ToString();
                                    expressionBuilder = new StringBuilder(DefaultExpressionLength);
                                    StringExpression stringExpression = new StringExpression(str);
                                    nodes.Add(stringExpression);
                                    inString = false;
                                }
                            }
                            else
                            {
                                CheckForInvalidOperator(expression, expressionBuilder);
                                inString = true;
                            }

                            break;

                        default:
                            expressionBuilder.Append(ch);
                            break;
                    }
                }

                data = reader.Read();
            }

            if (inString)
            {
                OnInvalidExpression(expression, "Expected '");
            }
            else if (inAttribute || depth > 0)
            {
                OnInvalidExpression(expression, "Expected )");
            }
            else if (depth < 0)
            {
                OnInvalidExpression(expression, "Unmatched )");
            }

            //
            // Assembly the flat list of expressions and expression placeholders into an
            // expression tree.
            //
            conditionExpression = AssembleExpressionTree(nodes.AsReadOnly(), expression);

            return conditionExpression;
        }

        /// <summary>
        /// Takes in a list of expressions and operator expression placeholders and
        /// builds an expression tree node.
        /// </summary>
        /// <param name="originalNodes">Original nodes.</param>
        /// <param name="originalExpression">Original expression.</param>
        /// <returns>A condition expression.</returns>
        private static IConditionExpression AssembleExpressionTree(
            ReadOnlyCollection<IConditionExpression> originalNodes, string originalExpression)
        {
            IConditionExpression conditionExpression = null;

            List<IConditionExpression> nodes = new List<IConditionExpression>(originalNodes);

            //
            // Build a queue that represents the binary operator precedence
            //
            Queue<BinaryExpressionOperator> operatorPrecedence = new Queue<BinaryExpressionOperator>();
            operatorPrecedence.Enqueue(BinaryExpressionOperator.Equal);
            operatorPrecedence.Enqueue(BinaryExpressionOperator.NotEqual);
            operatorPrecedence.Enqueue(BinaryExpressionOperator.Contains);
            operatorPrecedence.Enqueue(BinaryExpressionOperator.Matches);
            operatorPrecedence.Enqueue(BinaryExpressionOperator.And);
            operatorPrecedence.Enqueue(BinaryExpressionOperator.Or);

            //
            // Loop through the nodes and merge them by operator precedence
            //
            BinaryExpressionOperator currentOperator = operatorPrecedence.Dequeue();
            while (nodes.Count > 1)
            {
                for (int nodeIndex = 1; nodeIndex < nodes.Count - 1; nodeIndex++)
                {
                    OperatorExpressionPlaceholder operatorExpressionPlaceHolder =
                        nodes[nodeIndex] as OperatorExpressionPlaceholder;

                    if (operatorExpressionPlaceHolder != null &&
                        operatorExpressionPlaceHolder.Operator == currentOperator)
                    {
                        IConditionExpression left = nodes[nodeIndex - 1];
                        IConditionExpression right = nodes[nodeIndex + 1];

                        if ((operatorExpressionPlaceHolder.Operator == BinaryExpressionOperator.Equal ||
                            operatorExpressionPlaceHolder.Operator == BinaryExpressionOperator.Contains) &&
                            !(left is LeafExpression && right is LeafExpression))
                        {
                            OnInvalidExpression(originalExpression);
                        }

                        BinaryOperatorExpression operatorExpression = new BinaryOperatorExpression(
                            operatorExpressionPlaceHolder.Operator, left, right);

                        nodes[nodeIndex] = operatorExpression;
                        nodes.Remove(left);
                        nodes.Remove(right);

                        //
                        // Restart processing of this level
                        //
                        nodeIndex = 0;
                    }
                }

                if (operatorPrecedence.Count > 0)
                {
                    currentOperator = operatorPrecedence.Dequeue();
                }
                else
                {
                    break;
                }
            }

            //
            // At the end of everything, we should have a single binary or unary
            // condition expression.  Anything else is invalid and a format exception
            // will be thrown.
            //
            if (nodes.Count == 1)
            {
                conditionExpression = nodes[0] as BinaryOperatorExpression;
                if (conditionExpression == null)
                {
                    conditionExpression = nodes[0] as UnaryOperatorExpression;
                }
            }

            if (conditionExpression == null)
            {
                OnInvalidExpression(originalExpression);
            }

            return conditionExpression;
        }

        /// <summary>
        /// Checks for invalid operator.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="expressionBuilder">The expression builder.</param>
        private static void CheckForInvalidOperator(string expression, StringBuilder expressionBuilder)
        {
            if (expressionBuilder.Length > 0)
            {
                OnInvalidExpression(expression, "Invalid operator {0}", expressionBuilder);
            }
        }

        /// <summary>
        /// Called when an invalid expression is encountered.
        /// </summary>
        /// <param name="expression">Invalid expression text.</param>
        private static void OnInvalidExpression(string expression)
        {
            OnInvalidExpression(expression, string.Empty);
        }

        /// <summary>
        /// Called when an invalid expression is encountered.
        /// </summary>
        /// <param name="expression">Invalid expression text.</param>
        /// <param name="message">Message or message format.</param>
        /// <param name="args">Message arguments.</param>
        private static void OnInvalidExpression(string expression, string message, params object[] args)
        {
            StringBuilder messageBuilder = new StringBuilder();

            messageBuilder.AppendFormat(
                Thread.CurrentThread.CurrentCulture, "Invalid expression \"{0}\"", expression);

            if (!string.IsNullOrEmpty(message))
            {
                messageBuilder.Append(" - ");
                messageBuilder.AppendFormat(
                    Thread.CurrentThread.CurrentCulture, message, args);
            }

            throw new FormatException(messageBuilder.ToString());
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Operator expression.
        /// </summary>
        private class OperatorExpressionPlaceholder : LeafExpression
        {
            #region Fields

            /// <summary>
            /// Operator type.
            /// </summary>
            private readonly BinaryExpressionOperator _operatorType;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Creates a new operator expression.
            /// </summary>
            /// <param name="operatorType">Binary expression operator type.</param>
            public OperatorExpressionPlaceholder(BinaryExpressionOperator operatorType)
            {
                _operatorType = operatorType;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the expression operator.
            /// </summary>
            public BinaryExpressionOperator Operator
            {
                get
                {
                    return _operatorType;
                }
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}