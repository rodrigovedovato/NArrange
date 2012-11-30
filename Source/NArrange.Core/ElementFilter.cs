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
    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Class for determining whether or not an element matches 
    /// filter criteria.
    /// </summary>
    public class ElementFilter : IElementFilter
    {
        #region Fields

        /// <summary>
        /// Filter condition expression.
        /// </summary>
        private readonly IConditionExpression _conditionExpression;

        /// <summary>
        /// Filter condition scope that is required for evaluation.
        /// </summary>
        private readonly ElementAttributeScope _requiredScope;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new ElementFilter.
        /// </summary>
        /// <param name="conditionExpression">The condition expression.</param>
        public ElementFilter(string conditionExpression)
        {
            _conditionExpression = ConditionExpressionParser.Instance.Parse(conditionExpression);
            _requiredScope = GetRequiredScope(_conditionExpression);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the required scope information for evaluating the condition.
        /// </summary>
        public ElementAttributeScope RequiredScope
        {
            get
            {
                return _requiredScope;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether or not the specified code element matches the
        /// filter criteria.
        /// </summary>
        /// <param name="codeElement">Code element to analyze.</param>
        /// <returns>
        /// True if the elemet matches the filter, otherwise false.
        /// </returns>
        public bool IsMatch(ICodeElement codeElement)
        {
            bool isMatch = false;

            if (codeElement != null)
            {
                isMatch = ConditionExpressionEvaluator.Instance.Evaluate(_conditionExpression, codeElement);
            }

            return isMatch;
        }

        /// <summary>
        /// Gets the scope required to evaluate the condition.
        /// </summary>
        /// <param name="expression">Condition expression.</param>
        /// <returns>Element attribute scope.</returns>
        private ElementAttributeScope GetRequiredScope(IConditionExpression expression)
        {
            ElementAttributeScope scope = ElementAttributeScope.Element;

            if (expression != null)
            {
                ElementAttributeExpression attributeExpression = expression as ElementAttributeExpression;
                if (attributeExpression != null)
                {
                    scope = attributeExpression.Scope;
                }
                else
                {
                    ElementAttributeScope leftScope = GetRequiredScope(expression.Left);
                    ElementAttributeScope rightScope = GetRequiredScope(expression.Right);

                    if (leftScope > rightScope)
                    {
                        scope = leftScope;
                    }
                    else
                    {
                        scope = rightScope;
                    }
                }
            }

            return scope;
        }

        #endregion Methods
    }
}