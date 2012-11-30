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
    /// Specifies element grouping (used to group elements with line 
    /// separation in between)
    /// </summary>
    [XmlType("Group")]
    public class GroupBy : ICloneable
    {
        #region Fields

        /// <summary>
        /// The element attribute to group by.
        /// </summary>
        private ElementAttributeType _by;

        /// <summary>
        /// Custom separator text.
        /// </summary>
        private string _customSeparator;

        /// <summary>
        /// The sort direction for groups.
        /// </summary>
        private SortDirection _direction;
        private GroupBy _innerGroupBy;

        /// <summary>
        /// The regular expression text, if any, to be used to capture only a portion of the 
        /// attribute.
        /// </summary>
        private string _matchCapture;

        /// <summary>
        /// Separator type between elements.
        /// </summary>
        private GroupSeparatorType _separatorType;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the regular expression that specifies which portion
        /// of the element attribute should be used for grouping.
        /// </summary>
        [XmlAttribute("AttributeCapture")]
        [Description("The regular expression specifying the text that should be captured from the element attribute.")]
        [DisplayName("Attribute capture")]
        public string AttributeCapture
        {
            get
            {
                return _matchCapture;
            }
            set
            {
                _matchCapture = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute elements should be grouped by.
        /// </summary>
        [XmlAttribute("By")]
        [Description("The attribute elements should be grouped by.")]
        public ElementAttributeType By
        {
            get
            {
                return _by;
            }
            set
            {
                _by = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom separator string.
        /// </summary>
        [XmlAttribute("CustomSeparator")]
        [DefaultValue(null)]
        [Description("The text to insert between groups.")]
        [DisplayName("Custom separator")]
        public string CustomSeparator
        {
            get
            {
                return _customSeparator;
            }
            set
            {
                _customSeparator = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        [XmlAttribute("Direction")]
        [DefaultValue(SortDirection.None)]
        [Description("The sort direction for groups.")]
        public SortDirection Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        /// <summary>
        /// Gets or sets the inner grouping specification.
        /// </summary>
        [XmlElement("Group")]
        [Description("The inner group that will also apply when grouping elements.")]
        [DisplayName("Inner group by")]
        public GroupBy InnerGroupBy
        {
            get
            {
                return _innerGroupBy;
            }
            set
            {
                _innerGroupBy = value;
            }
        }

        /// <summary>
        /// Gets or sets the separator type.
        /// </summary>
        [XmlAttribute("SeparatorType")]
        [Description("Specifies how groups should be separated.")]
        [DisplayName("Separator type")]
        public GroupSeparatorType SeparatorType
        {
            get
            {
                return _separatorType;
            }
            set
            {
                _separatorType = value;
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
            GroupBy clone = new GroupBy();

            clone._by = _by;
            clone._matchCapture = _matchCapture;
            clone._customSeparator = _customSeparator;
            clone._separatorType = _separatorType;
            clone._direction = _direction;

            if (_innerGroupBy != null)
            {
                clone._innerGroupBy = _innerGroupBy.Clone() as GroupBy;
            }

            return clone;
        }

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format(
               Thread.CurrentThread.CurrentCulture, "Group by: {0}", _by);
        }

        #endregion Methods
    }
}