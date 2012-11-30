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
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Core.CodeElements
{
    using System;
    using System.IO;
    using System.Text;

    using NArrange.Core.Configuration;

    /// <summary>
    /// Element utility methods.
    /// </summary>
    public static class ElementUtilities
    {
        #region Methods

        /// <summary>
        /// Gets a string representation of a code element using the specified
        /// format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="codeElement">The code element.</param>
        /// <returns>Formatted string representation of the code element.</returns>
        public static string Format(string format, ICodeElement codeElement)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            else if (codeElement == null)
            {
                throw new ArgumentNullException("codeElement");
            }

            StringBuilder formatted = new StringBuilder(format.Length * 2);
            StringBuilder attributeBuilder = null;
            bool inAttribute = false;

            using (StringReader reader = new StringReader(format))
            {
                int data = reader.Read();
                while (data > 0)
                {
                    char ch = (char)data;

                    if (ch == ConditionExpressionParser.ExpressionPrefix &&
                        (char)(reader.Peek()) == ConditionExpressionParser.ExpressionStart)
                    {
                        reader.Read();
                        attributeBuilder = new StringBuilder(16);
                        inAttribute = true;
                    }
                    else if (inAttribute)
                    {
                        if (ch == ConditionExpressionParser.ExpressionEnd)
                        {
                            ElementAttributeType elementAttribute = (ElementAttributeType)Enum.Parse(
                                typeof(ElementAttributeType), attributeBuilder.ToString());

                            string attribute = GetAttribute(elementAttribute, codeElement);
                            formatted.Append(attribute);
                            attributeBuilder = new StringBuilder(16);
                            inAttribute = false;
                        }
                        else
                        {
                            attributeBuilder.Append(ch);
                        }
                    }
                    else
                    {
                        formatted.Append(ch);
                    }

                    data = reader.Read();
                }
            }

            return formatted.ToString();
        }

        /// <summary>
        /// Gets the string representation of a code element attribute.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="codeElement">The code element.</param>
        /// <returns>The code element attribute as text.</returns>
        public static string GetAttribute(ElementAttributeType attributeType, ICodeElement codeElement)
        {
            string attributeString = null;
            MemberElement memberElement;
            TypeElement typeElement;

            if (codeElement != null)
            {
                switch (attributeType)
                {
                    case ElementAttributeType.Name:
                        attributeString = codeElement.Name;
                        break;

                    case ElementAttributeType.Access:
                        AttributedElement attributedElement = codeElement as AttributedElement;
                        if (attributedElement != null)
                        {
                            attributeString = EnumUtilities.ToString(attributedElement.Access);
                        }
                        break;

                    case ElementAttributeType.ElementType:
                        attributeString = EnumUtilities.ToString(codeElement.ElementType);
                        break;

                    case ElementAttributeType.Type:
                        attributeString = GetTypeAttribute(codeElement);
                        break;

                    case ElementAttributeType.Attributes:
                        attributeString = GetAttributesAttribute(codeElement);
                        break;

                    case ElementAttributeType.Modifier:
                        memberElement = codeElement as MemberElement;
                        if (memberElement != null)
                        {
                            attributeString = EnumUtilities.ToString(memberElement.MemberModifiers);
                        }
                        else
                        {
                            typeElement = codeElement as TypeElement;
                            if (typeElement != null)
                            {
                                attributeString = EnumUtilities.ToString(typeElement.TypeModifiers);
                            }
                        }
                        break;

                    default:
                        attributeString = string.Empty;
                        break;
                }
            }

            if (attributeString == null)
            {
                attributeString = string.Empty;
            }

            return attributeString;
        }

        /// <summary>
        /// Processes each element in a code element tree.
        /// </summary>
        /// <param name="codeElement">Code element to process.</param>
        /// <param name="action">Action that performs processing.</param>
        public static void ProcessElementTree(ICodeElement codeElement, Action<ICodeElement> action)
        {
            if (codeElement != null)
            {
                action(codeElement);
                foreach (ICodeElement childElement in codeElement.Children)
                {
                    ProcessElementTree(childElement, action);
                }
            }
        }

        /// <summary>
        /// Gets the Attributes attribute.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <returns>Attributes as a comma-separated list.</returns>
        private static string GetAttributesAttribute(ICodeElement codeElement)
        {
            StringBuilder attributesBuilder = new StringBuilder();

            AttributedElement attributedElement = codeElement as AttributedElement;
            if (attributedElement != null)
            {
                for (int attributeIndex = 0; attributeIndex < attributedElement.Attributes.Count; attributeIndex++)
                {
                    IAttributeElement attribute = attributedElement.Attributes[attributeIndex];
                    attributesBuilder.Append(attribute.Name);

                    foreach (ICodeElement attributeChild in attribute.Children)
                    {
                        IAttributeElement childAttributeElement = attributeChild as IAttributeElement;
                        if (childAttributeElement != null)
                        {
                            attributesBuilder.Append(", ");
                            attributesBuilder.Append(childAttributeElement.Name);
                        }
                    }

                    if (attributeIndex < attributedElement.Attributes.Count - 1)
                    {
                        attributesBuilder.Append(", ");
                    }
                }
            }

            return attributesBuilder.ToString();
        }

        /// <summary>
        /// Gets the type attribute.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <returns>The type attibute as text.</returns>
        private static string GetTypeAttribute(ICodeElement codeElement)
        {
            string attributeString = string.Empty;

            MemberElement memberElement = codeElement as MemberElement;
            if (memberElement != null)
            {
                attributeString = memberElement.Type;
            }
            else
            {
                switch (codeElement.ElementType)
                {
                    case ElementType.Type:
                        attributeString = EnumUtilities.ToString(((TypeElement)codeElement).Type);
                        break;

                    case ElementType.Comment:
                        attributeString = EnumUtilities.ToString(((CommentElement)codeElement).Type);
                        break;

                    case ElementType.Using:
                        attributeString = EnumUtilities.ToString(((UsingElement)codeElement).Type);
                        break;
                }
            }

            return attributeString;
        }

        #endregion Methods
    }
}