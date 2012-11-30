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

namespace NArrange.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    /// <summary>
    /// Formatting configuration.
    /// </summary>
    public class FormattingConfiguration : ICloneable
    {
        #region Fields

        /// <summary>
        /// Closing comment configuration.
        /// </summary>
        private ClosingCommentConfiguration _closingComments;

        /// <summary>
        /// Line spacing configuration.
        /// </summary>
        private LineSpacingConfiguration _lineSpacing;

        /// <summary>
        /// Region formatting configuration.
        /// </summary>
        private RegionFormatConfiguration _regions;

        /// <summary>
        /// Tab configuration.
        /// </summary>
        private TabConfiguration _tabs;

        /// <summary>
        /// Using/import directive configuration.
        /// </summary>
        private UsingConfiguration _usings;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the closing comment configuration.
        /// </summary>
        [Description("The settings for closing comments.")]
        [DisplayName("Closing comments")]
        [ReadOnly(true)]
        public ClosingCommentConfiguration ClosingComments
        {
            get
            {
                if (_closingComments == null)
                {
                    lock (this)
                    {
                        if (_closingComments == null)
                        {
                            //
                            // Default closing comment configuration
                            //
                            _closingComments = new ClosingCommentConfiguration();
                        }
                    }
                }

                return _closingComments;
            }
            set
            {
                _closingComments = value;
            }
        }

        /// <summary>
        /// Gets or sets the line spacing configuration.
        /// </summary>
        [Description("The settings for line spacing.")]
        [ReadOnly(true)]
        public LineSpacingConfiguration LineSpacing
        {
            get
            {
                if (_lineSpacing == null)
                {
                    lock (this)
                    {
                        if (_lineSpacing == null)
                        {
                            //
                            // Default line spacing configuration
                            //
                            _lineSpacing = new LineSpacingConfiguration();
                        }
                    }
                }

                return _lineSpacing;
            }
            set
            {
                _lineSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the regions configuration.
        /// </summary>
        [Description("The settings for all regions.")]
        [ReadOnly(true)]
        public RegionFormatConfiguration Regions
        {
            get
            {
                if (_regions == null)
                {
                    lock (this)
                    {
                        if (_regions == null)
                        {
                            //
                            // Default regions configuration
                            //
                            _regions = new RegionFormatConfiguration();
                        }
                    }
                }

                return _regions;
            }
            set
            {
                _regions = value;
            }
        }

        /// <summary>
        /// Gets or sets the tab configuration.
        /// </summary>
        [Description("The settings for indentation.")]
        [ReadOnly(true)]
        public TabConfiguration Tabs
        {
            get
            {
                if (_tabs == null)
                {
                    lock (this)
                    {
                        if (_tabs == null)
                        {
                            //
                            // Default tab configuration
                            //
                            _tabs = new TabConfiguration();
                        }
                    }
                }

                return _tabs;
            }
            set
            {
                _tabs = value;
            }
        }

        /// <summary>
        /// Gets or sets the using directive configuration.
        /// </summary>
        [Description("The settings for using/import directives.")]
        [DisplayName("Using directives")]
        [ReadOnly(true)]
        public UsingConfiguration Usings
        {
            get
            {
                if (_usings == null)
                {
                    lock (this)
                    {
                        if (_usings == null)
                        {
                            //
                            // Default using configuration
                            //
                            _usings = new UsingConfiguration();
                        }
                    }
                }

                return _usings;
            }
            set
            {
                _usings = value;
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
            FormattingConfiguration clone = new FormattingConfiguration();

            if (_closingComments != null)
            {
                clone._closingComments = _closingComments.Clone() as ClosingCommentConfiguration;
            }

            if (_regions != null)
            {
                clone._regions = _regions.Clone() as RegionFormatConfiguration;
            }

            if (_tabs != null)
            {
                clone._tabs = _tabs.Clone() as TabConfiguration;
            }

            if (_lineSpacing != null)
            {
                clone._lineSpacing = _lineSpacing.Clone() as LineSpacingConfiguration;
            }

            if (_usings != null)
            {
                clone._usings = _usings.Clone() as UsingConfiguration;
            }

            return clone;
        }

        #endregion Methods
    }
}