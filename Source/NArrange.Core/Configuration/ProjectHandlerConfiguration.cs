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
    /// Specifies project extensions.
    /// </summary>
    [XmlType("ProjectHandler")]
    [DisplayName("Project Handler")]
    public class ProjectHandlerConfiguration : HandlerConfiguration
    {
        #region Fields

        /// <summary>
        /// Parser Type name within the assembly.
        /// </summary>
        private string _parserType;

        /// <summary>
        /// Project extension configurations.
        /// </summary>
        private ExtensionConfigurationCollection _projectExtensions;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the handler type.
        /// </summary>
        public override HandlerType HandlerType
        {
            get
            {
                return HandlerType.Project;
            }
        }

        /// <summary>
        /// Gets or sets the parser Type..
        /// </summary>
        [XmlAttribute("Parser")]
        [Description("The fully-qualified Type name for the associated parser.")]
        [DisplayName("Parser")]
        public string ParserType
        {
            get
            {
                return _parserType;
            }
            set
            {
                _parserType = value;
            }
        }

        /// <summary>
        /// Gets the list of project extension configurations.
        /// </summary>
        [XmlArrayItem(typeof(ExtensionConfiguration))]
        [Description("The list of project file extensions supported by the project parser.")]
        [DisplayName("Project extensions")]
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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format(
                Thread.CurrentThread.CurrentCulture, "Project Handler: {0}", _parserType);
        }

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>Clone of the instance.</returns>
        protected override HandlerConfiguration DoClone()
        {
            ProjectHandlerConfiguration clone = new ProjectHandlerConfiguration();

            clone._parserType = _parserType;

            foreach (ExtensionConfiguration extension in ProjectExtensions)
            {
                ExtensionConfiguration extensionClone = extension.Clone() as ExtensionConfiguration;
                clone.ProjectExtensions.Add(extensionClone);
            }

            return clone;
        }

        #endregion Methods
    }
}