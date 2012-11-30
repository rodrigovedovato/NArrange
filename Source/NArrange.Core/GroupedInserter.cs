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
    using System.Text.RegularExpressions;

    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Grouped inserter.
    /// </summary>
    public class GroupedInserter : IElementInserter
    {
        #region Fields

        /// <summary>
        /// Regex for capturing the portion of the attribute to be used
        /// for grouping.
        /// </summary>
        private readonly Regex _captureRegex;

        /// <summary>
        /// Group by specifying the configuration for this inserter.
        /// </summary>
        private readonly GroupBy _groupBy;

        /// <summary>
        /// Inner inserter.
        /// </summary>
        private readonly IElementInserter _innerInserter;

        /// <summary>
        /// Sort comparer for inserting elements into groups.
        /// </summary>
        private IComparer<ICodeElement> _sortComparer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new GroupedInserter using the specified grouping configuration.
        /// </summary>
        /// <param name="groupBy">The group by.</param>
        public GroupedInserter(GroupBy groupBy)
            : this(groupBy, null)
        {
        }

        /// <summary>
        /// Creates a new GroupedInserter using the specified grouping configuration
        /// and sorter.
        /// </summary>
        /// <param name="groupBy">The group by.</param>
        /// <param name="innerInserter">The inner inserter.</param>
        public GroupedInserter(GroupBy groupBy, IElementInserter innerInserter)
        {
            if (groupBy == null)
            {
                throw new ArgumentNullException("groupBy");
            }

            _groupBy = groupBy.Clone() as GroupBy;
            _innerInserter = innerInserter;

            if (!string.IsNullOrEmpty(groupBy.AttributeCapture))
            {
                _captureRegex = new Regex(_groupBy.AttributeCapture);
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Inserts the element within the parent.
        /// </summary>
        /// <param name="parentElement">Parent element to insert into.</param>
        /// <param name="codeElement">Code element to insert.</param>
        public void InsertElement(ICodeElement parentElement, ICodeElement codeElement)
        {
            GroupElement group = null;

            string groupName = GetGroupName(_groupBy.By, codeElement);

            foreach (ICodeElement childElement in parentElement.Children)
            {
                GroupElement groupElement = childElement as GroupElement;
                if (groupElement != null && groupElement.Name == groupName)
                {
                    group = groupElement;
                    break;
                }
            }

            if (group == null)
            {
                group = new GroupElement();
                group.Name = groupName;
                group.SeparatorType = _groupBy.SeparatorType;
                group.CustomSeparator = _groupBy.CustomSeparator;

                int insertIndex = parentElement.Children.Count;

                if (_groupBy.Direction != SortDirection.None)
                {
                    // Sort the groups by the attribute selected for grouping
                    for (int compareIndex = parentElement.Children.Count - 1; compareIndex >= 0;
                        compareIndex--)
                    {
                        GroupElement siblingGroup = parentElement.Children[insertIndex - 1] as GroupElement;
                        if (siblingGroup != null && siblingGroup.Children.Count > 0)
                        {
                            // This may not be the most accurate way to do this, but just compare
                            // against the first element in the sibling group.
                            ICodeElement compareElement = siblingGroup.Children[0];

                            // For nested groups, we need to drill down to find the first element.
                            while (compareElement is GroupElement && compareElement.Children.Count > 0)
                            {
                                compareElement = compareElement.Children[0];
                            }

                            // Create the element comparer if necessary
                            if (_sortComparer == null)
                            {
                                _sortComparer = new ElementComparer(_groupBy.By, _groupBy.Direction);
                            }

                            int compareValue = _sortComparer.Compare(codeElement, compareElement);

                            // System using directives should always be placed first in the file.
                            if (compareValue < 0 && (!(codeElement is UsingElement) || siblingGroup.Name != "System") ||
                                (codeElement is UsingElement && groupName == "System"))
                            {
                                insertIndex = compareIndex;
                            }
                        }
                    }
                }

                if (insertIndex < parentElement.Children.Count)
                {
                    parentElement.InsertChild(insertIndex, group);
                }
                else
                {
                    parentElement.AddChild(group);
                }
            }

            if (_innerInserter != null)
            {
                _innerInserter.InsertElement(group, codeElement);
            }
            else
            {
                group.AddChild(codeElement);
            }
        }

        /// <summary>
        /// Gets the name of the group the element falls into.
        /// </summary>
        /// <param name="elementFilterType">Type of the element filter.</param>
        /// <param name="codeElement">The code element.</param>
        /// <returns>The group name.</returns>
        private string GetGroupName(ElementAttributeType elementFilterType, ICodeElement codeElement)
        {
            string groupName = string.Empty;

            string attribute = ElementUtilities.GetAttribute(elementFilterType, codeElement);

            if (_captureRegex != null)
            {
                Match match = _captureRegex.Match(attribute);
                if (match != null && match.Groups.Count > 1)
                {
                    groupName = match.Groups[1].Value;
                }
            }
            else
            {
                groupName = attribute;
            }

            return groupName;
        }

        #endregion Methods
    }
}