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

    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Standard IElementArranger implementation.
    /// </summary>
    public class ElementArranger : IElementArranger
    {
        #region Fields

        /// <summary>
        /// Element configuration.
        /// </summary>
        private readonly ElementConfiguration _elementConfiguration;

        /// <summary>
        /// Parent configuration.
        /// </summary>
        private readonly ConfigurationElement _parentConfiguration;

        /// <summary>
        /// Arranger for children.
        /// </summary>
        private IElementArranger _childrenArranger;

        /// <summary>
        /// Element filter that elements must match to be arranged by this
        /// arranger.
        /// </summary>
        private IElementFilter _filter;

        /// <summary>
        /// Inserter that controls how elements are inserted.
        /// </summary>
        private IElementInserter _inserter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new ElementArranger.
        /// </summary>
        /// <param name="elementConfiguration">Element configuration.</param>
        /// <param name="parentConfiguration">Parent configuration.</param>
        protected internal ElementArranger(
            ElementConfiguration elementConfiguration, ConfigurationElement parentConfiguration)
        {
            if (elementConfiguration == null)
            {
                throw new ArgumentNullException("elementConfiguration");
            }

            _elementConfiguration = elementConfiguration;
            _parentConfiguration = parentConfiguration;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Arranges the element in within the code tree represented in the specified
        /// builder.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="codeElement">The code element.</param>
        public virtual void ArrangeElement(ICodeElement parentElement, ICodeElement codeElement)
        {
            if (codeElement.Children.Count > 0)
            {
                if (_childrenArranger == null)
                {
                    _childrenArranger = ElementArrangerFactory.CreateChildrenArranger(_elementConfiguration);
                }

                if (_childrenArranger != null)
                {
                    List<ICodeElement> children = new List<ICodeElement>(codeElement.Children);
                    codeElement.ClearChildren();

                    foreach (ICodeElement childElement in children)
                    {
                        ArrangeChildElement(codeElement, childElement);
                    }

                    //
                    // For condition directives, arrange the children of each node in the list.
                    //
                    ConditionDirectiveElement conditionDirective = codeElement as ConditionDirectiveElement;
                    if (conditionDirective != null)
                    {
                        //
                        // Skip the first instance since we've already arranged those child elements.
                        //
                        conditionDirective = conditionDirective.ElseCondition;
                    }
                    while (conditionDirective != null)
                    {
                        children = new List<ICodeElement>(conditionDirective.Children);
                        conditionDirective.ClearChildren();

                        foreach (ICodeElement childElement in children)
                        {
                            ArrangeChildElement(conditionDirective, childElement);
                        }
                        conditionDirective = conditionDirective.ElseCondition;
                    }
                }
            }

            if (_inserter == null)
            {
                _inserter = CreateElementInserter(
                    _elementConfiguration.ElementType,
                    _elementConfiguration.SortBy,
                    _elementConfiguration.GroupBy,
                    _parentConfiguration);
            }

            // For Type elements, if interdependent static fields are present, correct their
            // ordering.
            TypeElement typeElement = codeElement as TypeElement;
            if (typeElement != null &&
                (typeElement.Type == TypeElementType.Class || typeElement.Type == TypeElementType.Structure || typeElement.Type == TypeElementType.Module))
            {
                CorrectStaticFieldDependencies(typeElement);
            }

            _inserter.InsertElement(parentElement, codeElement);
        }

        /// <summary>
        /// Determines whether or not the specified element can be arranged by
        /// this arranger.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can arrange the specified code element; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanArrange(ICodeElement codeElement)
        {
            return CanArrange(null, codeElement);
        }

        /// <summary>
        /// Determines whether or not the specified element can be arranged by
        /// this arranger.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="codeElement">The code element.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can arrange the specified parent element; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanArrange(ICodeElement parentElement, ICodeElement codeElement)
        {
            // Clone the instance and assign the parent
            ICodeElement testCodeElement = codeElement;

            if (_filter == null && _elementConfiguration.FilterBy != null)
            {
                _filter = CreateElementFilter(_elementConfiguration.FilterBy);
            }

            if (parentElement != null &&
                _filter != null && _filter.RequiredScope == ElementAttributeScope.Parent)
            {
                testCodeElement = codeElement.Clone() as ICodeElement;
                testCodeElement.Parent = parentElement.Clone() as ICodeElement;
            }

            return (_elementConfiguration.ElementType == ElementType.NotSpecified ||
                codeElement.ElementType == _elementConfiguration.ElementType) &&
                (_filter == null || _filter.IsMatch(testCodeElement));
        }

        /// <summary>
        /// Creates an element filter.
        /// </summary>
        /// <param name="filterBy">The filter by.</param>
        /// <returns>An element filter.</returns>
        private static IElementFilter CreateElementFilter(FilterBy filterBy)
        {
            IElementFilter filter = null;

            if (filterBy != null)
            {
                filter = new ElementFilter(filterBy.Condition);
            }

            return filter;
        }

        /// <summary>
        /// Creates an element inserter.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <param name="groupBy">The group by.</param>
        /// <param name="parentConfiguration">The parent configuration.</param>
        /// <returns>An appropriate inserter.</returns>
        private static IElementInserter CreateElementInserter(
            ElementType elementType,
            SortBy sortBy,
            GroupBy groupBy,
            ConfigurationElement parentConfiguration)
        {
            IElementInserter inserter = null;

            if (sortBy != null)
            {
                inserter = new SortedInserter(elementType, sortBy);
            }

            if (groupBy != null && groupBy.InnerGroupBy != null)
            {
                inserter = new GroupedInserter(groupBy.InnerGroupBy, inserter);
            }

            if (groupBy != null)
            {
                inserter = new GroupedInserter(groupBy, inserter);
            }

            if (inserter == null)
            {
                inserter = new DefaultElementInserter();
            }

            return inserter;
        }

        /// <summary>
        /// Arranges the child element.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <param name="childElement">The child element.</param>
        private void ArrangeChildElement(ICodeElement codeElement, ICodeElement childElement)
        {
            //
            // Region elements are ignored.  Only process their children.
            //
            RegionElement regionElement = childElement as RegionElement;
            if (regionElement != null)
            {
                List<ICodeElement> regionChildren = new List<ICodeElement>(regionElement.Children);
                regionElement.ClearChildren();

                foreach (ICodeElement regionChildElement in regionChildren)
                {
                    _childrenArranger.ArrangeElement(codeElement, regionChildElement);
                }
            }
            else
            {
                _childrenArranger.ArrangeElement(codeElement, childElement);
            }
        }

        /// <summary>
        /// If dependent static fields exist, then correct their ordering regardless
        /// of arranging rules.
        /// </summary>
        /// <param name="typeElement">Type code element.</param>
        private void CorrectStaticFieldDependencies(TypeElement typeElement)
        {
            FieldElement[] staticFields = GetTypeStaticFields(typeElement);

            for (int fieldIndex = 0; fieldIndex < staticFields.Length; fieldIndex++)
            {
                FieldElement staticField = staticFields[fieldIndex];

                if (!string.IsNullOrEmpty(staticField.InitialValue))
                {
                    for (int compareFieldIndex = 0; compareFieldIndex < staticFields.Length; compareFieldIndex++)
                    {
                        FieldElement compareStaticField = staticFields[compareFieldIndex];

                        if (compareStaticField != staticField)
                        {
                            // If the static field references this field, then move the referenced
                            // field if necessary.
                            // TODO: Consider checking to see if the referenced name is not within
                            // a string.
                            if (staticField.InitialValue.Contains(compareStaticField.Name))
                            {
                                int fieldPosition = staticField.Parent.Children.IndexOf(staticField);
                                if (!(staticField.Parent == compareStaticField.Parent &&
                                    compareStaticField.Parent.Children.IndexOf(compareStaticField) < fieldPosition))
                                {
                                    staticField.Parent.InsertChild(fieldPosition, compareStaticField);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all static fields for a Type.
        /// </summary>
        /// <param name="typeElement">Type element.</param>
        /// <returns>All static fields for the specified Type element.</returns>
        private FieldElement[] GetTypeStaticFields(TypeElement typeElement)
        {
            List<FieldElement> staticFields = new List<FieldElement>();

            Action<ICodeElement> findStaticFields = delegate(ICodeElement codeElement)
            {
                FieldElement fieldElement = codeElement as FieldElement;
                if (fieldElement != null && fieldElement.MemberModifiers == MemberModifiers.Static)
                {
                    bool isTypeChild = false;
                    ICodeElement parentElement = codeElement.Parent;
                    while (!isTypeChild && parentElement != null)
                    {
                        isTypeChild = parentElement == typeElement;
                        if (!isTypeChild)
                        {
                            parentElement = parentElement.Parent;
                        }
                    }

                    if (isTypeChild)
                    {
                        staticFields.Add(fieldElement);
                    }
                }
            };

            ElementUtilities.ProcessElementTree(typeElement, findStaticFields);

            return staticFields.ToArray();
        }

        #endregion Methods
    }
}