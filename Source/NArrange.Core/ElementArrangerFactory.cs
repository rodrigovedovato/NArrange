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

    using NArrange.Core.Configuration;

    /// <summary>
    /// Class for creating ElementArranger instances based on configuration
    /// information.
    /// </summary>
    public static class ElementArrangerFactory
    {
        #region Methods

        /// <summary>
        /// Creates an element arranger using the specified configuration information.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="parentConfiguration">The parent configuration.</param>
        /// <returns>
        /// Returns an IElementArranger if succesful, otherwise null.
        /// </returns>
        public static IElementArranger CreateElementArranger(
            ConfigurationElement configuration,
            ConfigurationElement parentConfiguration)
        {
            IElementArranger arranger = null;

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            //
            // If this is an element reference, build the arranger using the referenced
            // element configuration instead.
            //
            ElementReferenceConfiguration elementReference = configuration as ElementReferenceConfiguration;
            if (elementReference != null && elementReference.ReferencedElement != null)
            {
                configuration = elementReference.ReferencedElement;
            }

            ElementConfiguration elementConfiguration = configuration as ElementConfiguration;
            if (elementConfiguration != null)
            {
                arranger = new ElementArranger(elementConfiguration, parentConfiguration);
            }
            else
            {
                RegionConfiguration regionConfiguration = configuration as RegionConfiguration;
                if (regionConfiguration != null)
                {
                    arranger = new RegionArranger(regionConfiguration, parentConfiguration);
                }
                else
                {
                    arranger = CreateChildrenArranger(configuration);
                }
            }

            return arranger;
        }

        /// <summary>
        /// Creates an arranger for the children of elements associated with the 
        /// specified cofiguration.
        /// </summary>
        /// <param name="parentConfiguration">Parent configuration.</param>
        /// <returns>Element arranger for children.</returns>
        internal static IElementArranger CreateChildrenArranger(ConfigurationElement parentConfiguration)
        {
            ChainElementArranger childrenArranger = new ChainElementArranger();
            foreach (ConfigurationElement childConfiguration in parentConfiguration.Elements)
            {
                IElementArranger childElementArranger = CreateElementArranger(
                        childConfiguration, parentConfiguration);

                if (childElementArranger != null)
                {
                    childrenArranger.AddArranger(childElementArranger);
                }
            }

            return childrenArranger;
        }

        #endregion Methods
    }
}