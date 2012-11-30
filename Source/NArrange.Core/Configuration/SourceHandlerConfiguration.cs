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
    /// Specifies source code extension handler assembly.
    /// </summary>
    [XmlType("SourceHandler")]
    [DisplayName("Source Handler")]
    public class SourceHandlerConfiguration : HandlerConfiguration
    {
        #region Fields

        /// <summary>
        /// Code language name.
        /// </summary>
        private string _language;

        /// <summary>
        /// Project extensions associated with the language.
        /// </summary>
        private ExtensionConfigurationCollection _projectExtensions;

        /// <summary>
        /// Source file extensions associated with the language.
        /// </summary>
        private ExtensionConfigurationCollection _sourceExtensions;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the handler type.
        /// </summary>
        public override HandlerType HandlerType
        {
            get
            {
                return HandlerType.Source;
            }
        }

        /// <summary>
        /// Gets or sets the language name.
        /// </summary>
        [XmlAttribute("Language")]
        [Description("The key for the language.")]
        public string Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
            }
        }

        /// <summary>
        /// Gets the collection of project extensions (Obsolete).
        /// </summary>
        [XmlArrayItem(typeof(ExtensionConfiguration))]
        [Description("The list of project file extensions recognized for the language " +
            "(Obsolete, this should now be specified in the project handler configuration).")]
        [DisplayName("Project extensions (Obsolete)")]
        [ReadOnly(true)]
        [Browsable(false)]
        public ExtensionConfigurationCollection ProjectExtensions
        {
            get
            {
                if (_projectExtensions == null)
                {
                    lock (this)
                    {
                        if (_projectExtensions == null)
                        {
                            _projectExtensions = new ExtensionConfigurationCollection();
                        }
                    }
                }

                return _projectExtensions;
            }
        }

        /// <summary>
        /// Gets the collection of source extensions.
        /// </summary>
        [XmlArrayItem(typeof(ExtensionConfiguration))]
        [Description("The list of source code file extensions recognized for the language.")]
        [DisplayName("Source extensions")]
        public ExtensionConfigurationCollection SourceExtensions
        {
            get
            {
                if (_sourceExtensions == null)
                {
                    lock (this)
                    {
                        if (_sourceExtensions == null)
                        {
                            _sourceExtensions = new ExtensionConfigurationCollection();
                        }
                    }
                }

                return _sourceExtensions;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format(
                Thread.CurrentThread.CurrentCulture,
                "Source Handler: {0}",
                _language);
        }

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>Clone of the instance.</returns>
        protected override HandlerConfiguration DoClone()
        {
            SourceHandlerConfiguration clone = new SourceHandlerConfiguration();

            clone._language = _language;

            foreach (ExtensionConfiguration extension in SourceExtensions)
            {
                ExtensionConfiguration extensionClone = extension.Clone() as ExtensionConfiguration;
                clone.SourceExtensions.Add(extensionClone);
            }

            return clone;
        }

        #endregion Methods
    }
}