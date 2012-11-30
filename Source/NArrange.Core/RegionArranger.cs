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

namespace NArrange.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;

    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Element arranger for adding members in regions.
    /// </summary>
    public class RegionArranger : IElementArranger
    {
        #region Fields

        /// <summary>
        /// Other region names at this level.
        /// </summary>
        private readonly ReadOnlyCollection<string> _levelRegions;

        /// <summary>
        /// Parent configuration.
        /// </summary>
        private readonly ConfigurationElement _parentConfiguration;

        /// <summary>
        /// Arranger for child elements.
        /// </summary>
        private IElementArranger _childrenArranger;

        /// <summary>
        /// Configuration for the region.
        /// </summary>
        private RegionConfiguration _regionConfiguration;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new RegionArranger.
        /// </summary>
        /// <param name="regionConfiguration">Region configuration.</param>
        /// <param name="parentConfiguration">Parent configuration.</param>
        public RegionArranger(RegionConfiguration regionConfiguration, ConfigurationElement parentConfiguration)
        {
            if (regionConfiguration == null)
            {
                throw new ArgumentNullException("regionConfiguration");
            }

            if (parentConfiguration == null)
            {
                throw new ArgumentNullException("parentConfiguration");
            }

            _regionConfiguration = regionConfiguration;
            _parentConfiguration = parentConfiguration;

            List<string> levelRegions = new List<string>();
            foreach (ConfigurationElement siblingConfiguration in
                _parentConfiguration.Elements)
            {
                RegionConfiguration siblingRegionConfiguration = siblingConfiguration as RegionConfiguration;
                if (siblingRegionConfiguration != null)
                {
                    levelRegions.Add(siblingRegionConfiguration.Name);
                }
            }

            _levelRegions = levelRegions.AsReadOnly();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Arranges the specified code element into the parent.
        /// </summary>
        /// <param name="parentElement">Parent element to arrange within.</param>
        /// <param name="codeElement">Code element to arrange.</param>
        public void ArrangeElement(ICodeElement parentElement, ICodeElement codeElement)
        {
            InitializeChildrenArranger();

            if (codeElement != null)
            {
                RegionElement region = null;

                string regionName = _regionConfiguration.Name;
                bool directivesEnabled = _regionConfiguration.DirectivesEnabled;

                foreach (ICodeElement childElement in parentElement.Children)
                {
                    RegionElement regionElement = childElement as RegionElement;
                    if (regionElement != null && regionElement.Name == regionName)
                    {
                        region = regionElement;
                        break;
                    }
                }

                if (region == null)
                {
                    region = new RegionElement();
                    region.Name = regionName;
                    region.DirectivesEnabled = directivesEnabled;

                    if (parentElement.Children.Count == 0)
                    {
                        parentElement.AddChild(region);
                    }
                    else
                    {
                        //
                        // Determine where to insert the new region
                        //
                        int insertIndex = 0;
                        int compareIndex = _levelRegions.IndexOf(region.Name);

                        for (int siblingIndex = 0; siblingIndex < parentElement.Children.Count;
                            siblingIndex++)
                        {
                            RegionElement siblingRegion = parentElement.Children[siblingIndex]
                                as RegionElement;
                            if (siblingRegion != null)
                            {
                                insertIndex = siblingIndex;

                                int siblingCompareIndex = _levelRegions.IndexOf(siblingRegion.Name);
                                if (compareIndex <= siblingCompareIndex)
                                {
                                    break;
                                }
                                else
                                {
                                    insertIndex++;
                                }
                            }
                            else
                            {
                                insertIndex++;
                            }
                        }

                        parentElement.InsertChild(insertIndex, region);
                    }
                }

                _childrenArranger.ArrangeElement(region, codeElement);
            }
        }

        /// <summary>
        /// Determines whether or not this arranger can handle arrangement of
        /// the specified element.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can arrange the specified code element; otherwise, <c>false</c>.
        /// </returns>
        public bool CanArrange(ICodeElement codeElement)
        {
            InitializeChildrenArranger();
            return _childrenArranger.CanArrange(codeElement);
        }

        /// <summary>
        /// Determines whether or not this arranger can handle arrangement of
        /// the specified element.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="codeElement">The code element.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can arrange the specified parent element; otherwise, <c>false</c>.
        /// </returns>
        public bool CanArrange(ICodeElement parentElement, ICodeElement codeElement)
        {
            InitializeChildrenArranger();
            return _childrenArranger.CanArrange(parentElement, codeElement);
        }

        /// <summary>
        /// Intitializes the arranger for children of the region.
        /// </summary>
        private void InitializeChildrenArranger()
        {
            if (_childrenArranger == null)
            {
                _childrenArranger = ElementArrangerFactory.CreateChildrenArranger(_regionConfiguration);
            }
        }

        #endregion Methods
    }
}