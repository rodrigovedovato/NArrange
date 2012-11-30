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
    using System.Threading;
    using System.Xml.Serialization;

    /// <summary>
    /// Specifies tab style configuration.
    /// </summary>
    [XmlType("Tabs")]
    public class TabConfiguration : ICloneable
    {
        #region Fields

        /// <summary>
        /// Spaces per tab.
        /// </summary>
        private int _spacesPerTab;

        /// <summary>
        /// Tab style (e.g. tabs or spaces)
        /// </summary>
        private TabStyle _tabStyle;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new TabConfiguration instance.
        /// </summary>
        public TabConfiguration()
        {
            _tabStyle = TabStyle.Spaces;
            _spacesPerTab = 4;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the number of spaces per tab.
        /// </summary>
        [XmlAttribute("SpacesPerTab")]
        [Description("The number of spaces per tab. Used for conversion between spaces and tabs.")]
        [DisplayName("Spaces per tab")]
        public int SpacesPerTab
        {
            get
            {
                return _spacesPerTab;
            }
            set
            {
                _spacesPerTab = value;
            }
        }

        /// <summary>
        /// Gets or sets the tab style.
        /// </summary>
        [XmlAttribute("Style")]
        [Description("The indentation style.")]
        public TabStyle TabStyle
        {
            get
            {
                return _tabStyle;
            }
            set
            {
                _tabStyle = value;
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
        public object Clone()
        {
            TabConfiguration clone = new TabConfiguration();

            clone._tabStyle = _tabStyle;
            clone._spacesPerTab = _spacesPerTab;

            return clone;
        }

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format(
                Thread.CurrentThread.CurrentCulture,
                "Tabs: {0}, {1}",
                TabStyle,
                SpacesPerTab);
        }

        #endregion Methods
    }
}