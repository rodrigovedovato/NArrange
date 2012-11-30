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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a code element that can have attributes.
    /// </summary>
    public abstract class AttributedElement : TextCodeElement, IAttributedElement
    {
        #region Fields

        /// <summary>
        /// Synch lock object for the attributes collection.
        /// </summary>
        private readonly object _attributesLock = new object();

        /// <summary>
        /// The element's accessibility.
        /// </summary>
        private CodeAccess _access;

        /// <summary>
        /// Attributes associated with this element.
        /// </summary>
        private List<IAttributeElement> _attributes;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new AttributedElement.
        /// </summary>
        protected AttributedElement()
        {
            _access = CodeAccess.Public;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the code element access level.
        /// </summary>
        public CodeAccess Access
        {
            get
            {
                return _access;
            }
            set
            {
                _access = value;
            }
        }

        /// <summary>
        /// Gets the read-only collection of attributes.
        /// </summary>
        public ReadOnlyCollection<IAttributeElement> Attributes
        {
            get
            {
                return BaseAttributes.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the writable collection of attributes.
        /// </summary>
        protected List<IAttributeElement> BaseAttributes
        {
            get
            {
                if (_attributes == null)
                {
                    lock (_attributesLock)
                    {
                        if (_attributes == null)
                        {
                            _attributes = new List<IAttributeElement>();
                        }
                    }
                }

                return _attributes;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds an attribute to this code element.
        /// </summary>
        /// <param name="attributeElement">Attribute element to add.</param>
        public void AddAttribute(IAttributeElement attributeElement)
        {
            if (attributeElement != null && !BaseAttributes.Contains(attributeElement))
            {
                lock (_attributesLock)
                {
                    if (attributeElement != null && !BaseAttributes.Contains(attributeElement))
                    {
                        BaseAttributes.Add(attributeElement);
                        attributeElement.Parent = this;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all attributes elements.
        /// </summary>
        public void ClearAttributes()
        {
            lock (_attributesLock)
            {
                for (int attributeIndex = 0; attributeIndex < Attributes.Count; attributeIndex++)
                {
                    IAttributeElement attribute = Attributes[attributeIndex];
                    if (attribute != null && attribute.Parent != null)
                    {
                        attribute.Parent = null;
                        attributeIndex--;
                    }
                }

                BaseAttributes.Clear();
            }
        }

        /// <summary>
        /// Removes an attribute from this element.
        /// </summary>
        /// <param name="attributeElement">Attribute element to remove.</param>
        public void RemoveAttribute(IAttributeElement attributeElement)
        {
            if (attributeElement != null && BaseAttributes.Contains(attributeElement))
            {
                lock (_attributesLock)
                {
                    if (attributeElement != null && BaseAttributes.Contains(attributeElement))
                    {
                        BaseAttributes.Remove(attributeElement);
                        attributeElement.Parent = null;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a clone of the instance and copies any state.
        /// </summary>
        /// <returns>Cloned attribute element state.</returns>
        protected abstract AttributedElement DoAttributedClone();

        /// <summary>
        /// Creates a clone of the instance and copies any state.
        /// </summary>
        /// <returns>A clone of the instance.</returns>
        protected override sealed CodeElement DoClone()
        {
            AttributedElement clone = DoAttributedClone();

            //
            // Copy state
            //
            clone._access = _access;
            lock (_attributesLock)
            {
                for (int attributeIndex = 0; attributeIndex < Attributes.Count; attributeIndex++)
                {
                    IAttributeElement attribute = Attributes[attributeIndex];
                    IAttributeElement attributeClone = attribute.Clone() as IAttributeElement;

                    clone.AddAttribute(attributeClone);
                }
            }

            return clone;
        }

        #endregion Methods
    }
}