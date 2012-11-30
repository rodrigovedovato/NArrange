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

namespace NArrange.Core.Configuration
{
    using System.ComponentModel;
    using System.Threading;
    using System.Xml.Serialization;

    /// <summary>
    /// Element reference configuration.
    /// </summary>
    [XmlType("ElementRef")]
    [DisplayName("Element Reference")]
    public class ElementReferenceConfiguration : ConfigurationElement
    {
        #region Fields

        /// <summary>
        /// The referenced element ID.
        /// </summary>
        private string _id;

        /// <summary>
        /// The referenced element, if loaded.
        /// </summary>
        private ElementConfiguration _referencedElement;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the collection of element configurations.
        /// </summary>
        [Browsable(false)]
        public override ConfigurationElementCollection Elements
        {
            get
            {
                return base.Elements;
            }
        }

        /// <summary>
        /// Gets or sets the element reference identifier.
        /// </summary>
        [XmlAttribute("Id")]
        [Description("The unique identifier of the referenced element.")]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Gets the referenced element configuration.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public ElementConfiguration ReferencedElement
        {
            get
            {
                return _referencedElement;
            }
            internal set
            {
                if (value != null)
                {
                    _referencedElement = value.Clone() as ElementConfiguration;
                    _referencedElement.Id = null;
                }
                else
                {
                    _referencedElement = null;
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets a string representation.
        /// </summary>
        /// <returns>The string represetation.</returns>
        public override string ToString()
        {
            return string.Format(
                Thread.CurrentThread.CurrentCulture, "Element Reference: {0}", Id);
        }

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>Clone of this instance.</returns>
        protected override ConfigurationElement DoClone()
        {
            ElementReferenceConfiguration clone = new ElementReferenceConfiguration();

            clone._id = _id;

            if (_referencedElement != null)
            {
                ElementConfiguration referenceClone = _referencedElement.Clone() as ElementConfiguration;
                clone._referencedElement = referenceClone;
            }

            return clone;
        }

        #endregion Methods
    }
}