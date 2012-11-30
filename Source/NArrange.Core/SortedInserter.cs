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
    using System.Collections.Generic;
    using System.ComponentModel;

    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Sorted inserter.
    /// </summary>
    public class SortedInserter : IElementInserter
    {
        #region Fields

        /// <summary>
        /// Element type.
        /// </summary>
        private readonly ElementType _elementType;

        /// <summary>
        /// Sort configuration.
        /// </summary>
        private readonly SortBy _sortBy;

        /// <summary>
        /// Element comparer.
        /// </summary>
        private IComparer<ICodeElement> _comparer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new sorted inserter using the specified sorting configuration.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="sortBy">The sort by.</param>
        public SortedInserter(ElementType elementType, SortBy sortBy)
        {
            if (sortBy == null)
            {
                throw new ArgumentNullException("sortBy");
            }

            _elementType = elementType;
            _sortBy = sortBy.Clone() as SortBy;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Inserts an element into the parent using the strategy defined by the 
        /// sort configuration.
        /// </summary>
        /// <param name="parentElement">Parent element.</param>
        /// <param name="codeElement">Code element to insert.</param>
        public void InsertElement(ICodeElement parentElement, ICodeElement codeElement)
        {
            if (codeElement != null)
            {
                ICodeElement compareElement = null;

                int insertIndex = 0;

                if (parentElement.Children.Count > 0)
                {
                    if (_comparer == null)
                    {
                        _comparer = CreateComparer(_sortBy);
                    }

                    for (int elementIndex = 0; elementIndex < parentElement.Children.Count; elementIndex++)
                    {
                        compareElement = parentElement.Children[elementIndex];

                        bool greaterOrEqual =
                            (_elementType == ElementType.NotSpecified &&
                            _comparer.Compare(codeElement, compareElement) >= 0) ||
                            (_elementType != ElementType.NotSpecified &&
                            ((compareElement != null && compareElement.ElementType != _elementType) ||
                            _comparer.Compare(codeElement, compareElement) >= 0));
                        if (greaterOrEqual)
                        {
                            insertIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                parentElement.InsertChild(insertIndex, codeElement);
            }
        }

        /// <summary>
        /// Creates a comparer based on the sort configuration.
        /// </summary>
        /// <param name="sortBy">Sort configuration.</param>
        /// <returns>Comparer for two code elements.</returns>
        private IComparer<ICodeElement> CreateComparer(SortBy sortBy)
        {
            ElementComparer comparer = null;

            Stack<SortBy> sortByStack = new Stack<SortBy>();

            while (sortBy != null)
            {
                sortByStack.Push(sortBy);
                sortBy = sortBy.InnerSortBy;
            }

            while (sortByStack.Count > 0)
            {
                sortBy = sortByStack.Pop();
                comparer = new ElementComparer(sortBy.By, sortBy.Direction, comparer);
            }

            return comparer;
        }

        #endregion Methods
    }
}