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
    /// <summary>
    /// Attribute code element.
    /// </summary>
    public sealed class AttributeElement : TextCodeElement, IAttributeElement
    {
        #region Fields

        /// <summary>
        /// Parent element.
        /// </summary>
        private ICodeElement _parent;

        /// <summary>
        /// Target or scope.
        /// </summary>
        private string _target;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new attribute element.
        /// </summary>
        public AttributeElement()
        {
        }

        /// <summary>
        /// Creates a new attribute element with the specified name.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        public AttributeElement(string name)
        {
            Name = name;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the element type.
        /// </summary>
        public override ElementType ElementType
        {
            get
            {
                return ElementType.Attribute;
            }
        }

        /// <summary>
        /// Gets or sets the parent element.
        /// </summary>
        public override ICodeElement Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (value != _parent)
                {
                    if (_parent != null)
                    {
                        AttributedElement attributedElement = _parent as AttributedElement;
                        if (attributedElement != null)
                        {
                            attributedElement.RemoveAttribute(this);
                        }
                        else
                        {
                            _parent.RemoveChild(this);
                        }
                    }

                    _parent = value;
                    if (_parent != null)
                    {
                        AttributedElement attributedElement = _parent as AttributedElement;
                        if (attributedElement != null)
                        {
                            if (!attributedElement.Attributes.Contains(this))
                            {
                                attributedElement.AddAttribute(this);
                            }
                        }
                        else if (!_parent.Children.Contains(this))
                        {
                            _parent.AddChild(this);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the attribute target. 
        /// </summary>
        public string Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Allows an ICodeElementVisitor to process (or visit) this element.
        /// </summary>
        /// <remarks>See the Gang of Four Visitor design pattern.</remarks>
        /// <param name="visitor">Code element visitor.</param>
        public override void Accept(ICodeElementVisitor visitor)
        {
            visitor.VisitAttributeElement(this);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A clone of the code element.</returns>
        protected override CodeElement DoClone()
        {
            AttributeElement clone = new AttributeElement();
            clone._target = _target;

            return clone;
        }

        #endregion Methods
    }
}