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
    /// Configuration element for regions.
    /// </summary>
    [XmlType("Region")]
    [DisplayName("Region")]
    public class RegionConfiguration : ConfigurationElement
    {
        #region Fields

        /// <summary>
        /// Default for whether or not region directives are enabled.
        /// </summary>
        private const bool DefaultDirectivesEnabled = true;

        /// <summary>
        /// Whether or not directives should be written for the region.
        /// </summary>
        private bool _directivesEnabled = DefaultDirectivesEnabled;

        /// <summary>
        /// Region name.
        /// </summary>
        private string _name;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether or not this region 
        /// should use directives.
        /// </summary>
        [XmlAttribute("DirectivesEnabled")]
        [Description("Directives enabled")]
        [DefaultValue(DefaultDirectivesEnabled)]
        public bool DirectivesEnabled
        {
            get
            {
                return _directivesEnabled;
            }
            set
            {
                _directivesEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the region name.
        /// </summary>
        [XmlAttribute("Name")]
        [Description("The region name")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format(
                Thread.CurrentThread.CurrentCulture,
                "Region: {0}",
                Name);
        }

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>Clone of this instance.</returns>
        protected override ConfigurationElement DoClone()
        {
            RegionConfiguration clone = new RegionConfiguration();

            clone._name = _name;
            clone._directivesEnabled = _directivesEnabled;

            return clone;
        }

        #endregion Methods
    }
}