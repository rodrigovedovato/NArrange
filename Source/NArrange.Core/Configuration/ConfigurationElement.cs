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
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Base configuration element class.
    /// </summary>
    public abstract class ConfigurationElement : ICloneable
    {
        #region Fields

        /// <summary>
        /// Child element collection.
        /// </summary>
        private ConfigurationElementCollection _elements;

        /// <summary>
        /// Child element collection sychronization lock object.
        /// </summary>
        private object _elementsSynchLock = new object();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the collection of child elements.
        /// </summary>
        [XmlArrayItem(typeof(ElementConfiguration))]
        [XmlArrayItem(typeof(RegionConfiguration))]
        [XmlArrayItem(typeof(ElementReferenceConfiguration))]
        [Description("The list of child element configurations.")]
        public virtual ConfigurationElementCollection Elements
        {
            get
            {
                if (_elements == null)
                {
                    lock (_elementsSynchLock)
                    {
                        if (_elements == null)
                        {
                            _elements = new ConfigurationElementCollection();
                        }
                    }
                }

                return _elements;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            ConfigurationElement configurationElement = BaseClone();

            return configurationElement;
        }

        /// <summary>
        /// Creates a new instance and copies state.
        /// </summary>
        /// <returns>Creates an clone of the instance with the base state copied.</returns>
        protected ConfigurationElement BaseClone()
        {
            ConfigurationElement clone = DoClone();

            foreach (ConfigurationElement child in Elements)
            {
                ConfigurationElement childClone = child.Clone() as ConfigurationElement;
                clone.Elements.Add(childClone);
            }

            return clone;
        }

        /// <summary>
        /// Creates a new instance of this type and copies state.
        /// </summary>
        /// <returns>Clone of this instance.</returns>
        protected abstract ConfigurationElement DoClone();

        #endregion Methods
    }
}