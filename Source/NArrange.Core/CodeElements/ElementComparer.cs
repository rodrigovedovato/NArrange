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
    using System.Collections.Generic;

    /// <summary>
    /// Comparer for ICodeElements.
    /// </summary>
    public class ElementComparer : IComparer<ICodeElement>
    {
        #region Fields

        /// <summary>
        /// Comparison element attribute.
        /// </summary>
        private ElementAttributeType _compareAttribute;

        /// <summary>
        /// Comparison delegate.
        /// </summary>
        private Comparison<ICodeElement> _comparison;

        /// <summary>
        /// Inner comparer.
        /// </summary>
        private IComparer<ICodeElement> _innerComparer;

        /// <summary>
        /// Sort direction.
        /// </summary>
        private SortDirection _sortDirection;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new ElementComparer.
        /// </summary>
        /// <param name="compareAttribute">The compare attribute.</param>
        /// <param name="sortDirection">The sort direction.</param>
        public ElementComparer(ElementAttributeType compareAttribute, SortDirection sortDirection)
            : this(compareAttribute, sortDirection, null)
        {
        }

        /// <summary>
        /// Create a new ElementComparer.
        /// </summary>
        /// <param name="compareAttribute">The compare attribute.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="innerComparer">The inner comparer.</param>
        public ElementComparer(
            ElementAttributeType compareAttribute,
            SortDirection sortDirection,
            IComparer<ICodeElement> innerComparer)
        {
            _compareAttribute = compareAttribute;
            _sortDirection = sortDirection;
            _innerComparer = innerComparer;

            _comparison = CreateComparison(compareAttribute);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zerox is less than y.Zerox equals y.Greater than zerox is greater than y.
        /// </returns>
        public int Compare(ICodeElement x, ICodeElement y)
        {
            int compareValue = 0;

            if (_sortDirection != SortDirection.None)
            {
                compareValue = _comparison(x, y);

                //
                // Inner sort?
                //
                if (compareValue == 0)
                {
                    if (_innerComparer != null)
                    {
                        compareValue = _innerComparer.Compare(x, y);
                    }
                }
                else if (_sortDirection == SortDirection.Descending)
                {
                    compareValue = -compareValue;
                }
            }

            return compareValue;
        }

        /// <summary>
        /// Creates a comparison delegate based on the configuration.
        /// </summary>
        /// <param name="compareAttribute">The compare attribute.</param>
        /// <returns>
        /// Comparision delegate for two code elements.
        /// </returns>
        public Comparison<ICodeElement> CreateComparison(ElementAttributeType compareAttribute)
        {
            Comparison<ICodeElement> comparison = delegate(ICodeElement x, ICodeElement y)
            {
                int compareValue = 0;

                if (x == null && y != null)
                {
                    compareValue = -1;
                }
                else if (x != null && y == null)
                {
                    compareValue = 1;
                }
                else
                {
                    switch (compareAttribute)
                    {
                        case ElementAttributeType.Access:
                            AttributedElement attributedX = x as AttributedElement;
                            AttributedElement attributedY = y as AttributedElement;
                            if (attributedX != null && attributedY != null)
                            {
                                compareValue = attributedX.Access.CompareTo(attributedY.Access);
                            }
                            break;

                        case ElementAttributeType.Modifier:
                            MemberElement memberX = x as MemberElement;
                            MemberElement memberY = y as MemberElement;
                            if (memberX != null && memberY != null)
                            {
                                compareValue = memberX.MemberModifiers.CompareTo(memberY.MemberModifiers);
                            }
                            break;

                        case ElementAttributeType.ElementType:
                            compareValue = x.ElementType.CompareTo(y.ElementType);
                            break;

                        default:
                            if (compareAttribute == ElementAttributeType.Type &&
                                x is TypeElement && y is TypeElement)
                            {
                                compareValue = ((TypeElement)x).Type.CompareTo(((TypeElement)y).Type);
                            }
                            else if (compareAttribute == ElementAttributeType.Type &&
                                x is UsingElement && y is UsingElement)
                            {
                                compareValue = ((UsingElement)x).Type.CompareTo(((UsingElement)y).Type);
                            }
                            else
                            {
                                string attributeX = ElementUtilities.GetAttribute(compareAttribute, x);
                                string attributeY = ElementUtilities.GetAttribute(compareAttribute, y);
                                compareValue = StringComparer.OrdinalIgnoreCase.Compare(attributeX, attributeY);
                            }
                            break;
                    }
                }

                return compareValue;
            };

            return comparison;
        }

        #endregion Methods
    }
}