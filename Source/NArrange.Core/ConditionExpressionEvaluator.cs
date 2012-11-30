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

namespace NArrange.Core
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;

    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Class for evaluating filter expressions.
    /// </summary>
    public sealed class ConditionExpressionEvaluator
    {
        #region Fields

        /// <summary>
        /// Synchronization lock for the singleton instance.
        /// </summary>
        private static readonly object _instanceLock = new object();

        /// <summary>
        /// Singleton instance field.
        /// </summary>
        private static ConditionExpressionEvaluator _instance;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new ConditionExpressionEvaluator.
        /// </summary>
        private ConditionExpressionEvaluator()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the single instance of the expression evaluator.
        /// </summary>
        public static ConditionExpressionEvaluator Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConditionExpressionEvaluator();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Evaluates an expression against the specified file.
        /// </summary>
        /// <param name="conditionExpression">The condition expression.</param>
        /// <param name="file">The file to evaluate against.</param>
        /// <returns>True if the expression evaluates to true, otherwise false.</returns>
        public bool Evaluate(IConditionExpression conditionExpression, FileInfo file)
        {
            bool result = false;

            if (conditionExpression == null)
            {
                throw new ArgumentNullException("conditionExpression");
            }
            else if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            result = Evaluate<FileInfo>(conditionExpression, file);

            return result;
        }

        /// <summary>
        /// Evaluates an expression against the specified element.
        /// </summary>
        /// <param name="conditionExpression">The condition expression.</param>
        /// <param name="element">The element.</param>
        /// <returns>True if the expression holds true for the element, otherwise false.</returns>
        public bool Evaluate(IConditionExpression conditionExpression, ICodeElement element)
        {
            bool result = false;

            if (conditionExpression == null)
            {
                throw new ArgumentNullException("conditionExpression");
            }
            else if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            result = Evaluate<ICodeElement>(conditionExpression, element);

            return result;
        }

        /// <summary>
        /// Gets the expression value.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="element">The element.</param>
        /// <returns>The expression value as text.</returns>
        private static string GetExpressionValue(IConditionExpression expression, ICodeElement element)
        {
            string value = string.Empty;

            if (expression != null && element != null)
            {
                StringExpression stringExpression = expression as StringExpression;
                if (stringExpression != null)
                {
                    value = stringExpression.Text;
                }
                else
                {
                    ElementAttributeExpression attributeExpression = expression as ElementAttributeExpression;

                    if (attributeExpression.Scope == ElementAttributeScope.Parent)
                    {
                        element = element.Parent;
                    }

                    if (attributeExpression != null)
                    {
                        value = ElementUtilities.GetAttribute(attributeExpression.ElementAttribute, element);
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the expression value.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>The expression value as text.</returns>
        private static string GetExpressionValue(IConditionExpression expression, object entity)
        {
            string expressionValue = string.Empty;

            ICodeElement element = entity as ICodeElement;
            if (element != null)
            {
                expressionValue = GetExpressionValue(expression, element);
            }
            else
            {
                FileInfo file = entity as FileInfo;
                if (file != null)
                {
                    expressionValue = GetExpressionValue(expression, file);
                }
            }

            return expressionValue;
        }

        /// <summary>
        /// Gets the expression value.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="file">The file to evaluate the expression against.</param>
        /// <returns>The expression value as text.</returns>
        private static string GetExpressionValue(IConditionExpression expression, FileInfo file)
        {
            string value = string.Empty;

            if (expression != null && file != null)
            {
                StringExpression stringExpression = expression as StringExpression;
                if (stringExpression != null)
                {
                    value = stringExpression.Text;
                }
                else
                {
                    FileAttributeExpression attributeExpression = expression as FileAttributeExpression;
                    if (attributeExpression != null)
                    {
                        value = FileUtilities.GetAttribute(attributeExpression.FileAttribute, file);
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Evaluates an expression against the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="conditionExpression">The condition expression.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>True if the expression holds true, otherwise false.</returns>
        private bool Evaluate<TEntity>(IConditionExpression conditionExpression, TEntity entity)
        {
            bool result = false;

            BinaryOperatorExpression binaryOperatorExpression = conditionExpression as BinaryOperatorExpression;
            if (binaryOperatorExpression != null)
            {
                string leftStr, rightStr;
                bool leftResult, rightResult;

                switch (binaryOperatorExpression.Operator)
                {
                    case BinaryExpressionOperator.Equal:
                        leftStr = GetExpressionValue(binaryOperatorExpression.Left, entity);
                        rightStr = GetExpressionValue(binaryOperatorExpression.Right, entity);
                        result = leftStr == rightStr;
                        break;

                    case BinaryExpressionOperator.NotEqual:
                        leftStr = GetExpressionValue(binaryOperatorExpression.Left, entity);
                        rightStr = GetExpressionValue(binaryOperatorExpression.Right, entity);
                        result = leftStr != rightStr;
                        break;

                    case BinaryExpressionOperator.Contains:
                        leftStr = GetExpressionValue(binaryOperatorExpression.Left, entity);
                        rightStr = GetExpressionValue(binaryOperatorExpression.Right, entity);
                        result = leftStr.Contains(rightStr);
                        break;

                    case BinaryExpressionOperator.Matches:
                        leftStr = GetExpressionValue(binaryOperatorExpression.Left, entity);
                        rightStr = GetExpressionValue(binaryOperatorExpression.Right, entity);
                        Regex regex = new Regex(rightStr);
                        result = regex.IsMatch(leftStr);
                        break;

                    case BinaryExpressionOperator.And:
                        leftResult = Evaluate(binaryOperatorExpression.Left, entity);
                        rightResult = Evaluate(binaryOperatorExpression.Right, entity);
                        result = leftResult && rightResult;
                        break;

                    case BinaryExpressionOperator.Or:
                        leftResult = Evaluate(binaryOperatorExpression.Left, entity);
                        rightResult = Evaluate(binaryOperatorExpression.Right, entity);
                        result = leftResult || rightResult;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(
                            string.Format(
                            Thread.CurrentThread.CurrentCulture,
                            "Unsupported operator type {0}",
                            binaryOperatorExpression.Operator));
                }
            }
            else
            {
                UnaryOperatorExpression unaryOperatorExpression = conditionExpression as UnaryOperatorExpression;
                if (unaryOperatorExpression != null)
                {
                    switch (unaryOperatorExpression.Operator)
                    {
                        case UnaryExpressionOperator.Negate:
                            result = !Evaluate(unaryOperatorExpression.InnerExpression, entity);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(
                                string.Format(
                                Thread.CurrentThread.CurrentCulture,
                                "Unsupported operator type {0}",
                                unaryOperatorExpression.Operator));
                    }
                }
            }

            return result;
        }

        #endregion Methods
    }
}