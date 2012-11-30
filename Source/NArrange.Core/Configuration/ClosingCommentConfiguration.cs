#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2007-2008 James Nies and NArrange contributors.
 *      All rights reserved.
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
    /// Specifies closing comments configuration.
    /// </summary>
    [XmlType("ClosingComments")]
    public class ClosingCommentConfiguration : ICloneable
    {
        #region Fields

        /// <summary>
        /// Whether or not closing comments should be written.
        /// </summary>
        private bool _enabled;

        /// <summary>
        /// Closing comment text format.
        /// </summary>
        private string _format;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether closing comments are enabled.
        /// </summary>
        [XmlAttribute("Enabled")]
        [Description("Whether or not comments will be inserted at the end of member blocks.")]
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        [XmlAttribute("Format")]
        [Description("The format string for closing comments, consisting of attribute parameters and text.")]
        public string Format
        {
            get
            {
                return _format;
            }
            set
            {
                _format = value;
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
            ClosingCommentConfiguration clone = new ClosingCommentConfiguration();

            clone._enabled = _enabled;
            clone._format = _format;

            return clone;
        }

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return string.Format(
                Thread.CurrentThread.CurrentCulture,
                "Closing comment: {0}, {1}",
                Enabled,
                Format);
        }

        #endregion Methods
    }
}