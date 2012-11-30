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
    /// Field code element.
    /// </summary>
    public sealed class FieldElement : MemberElement
    {
        #region Fields

        /// <summary>
        /// Field initial value.
        /// </summary>
        private string _initialValue;

        /// <summary>
        /// Whether or not the field is volatile.
        /// </summary>
        private bool _isVolatile;

        /// <summary>
        /// Trailing comment.
        /// </summary>
        private ICommentElement _trailingComment;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the element type.
        /// </summary>
        public override ElementType ElementType
        {
            get
            {
                return ElementType.Field;
            }
        }

        /// <summary>
        /// Gets or sets the initial value of the field.
        /// </summary>
        public string InitialValue
        {
            get
            {
                return _initialValue;
            }
            set
            {
                _initialValue = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the field is a constant.
        /// </summary>
        public bool IsConstant
        {
            get
            {
                return (MemberModifiers & MemberModifiers.Constant) == MemberModifiers.Constant;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the field is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return (MemberModifiers & MemberModifiers.ReadOnly) == MemberModifiers.ReadOnly;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the field is volatile.
        /// </summary>
        public bool IsVolatile
        {
            get
            {
                return _isVolatile;
            }
            set
            {
                _isVolatile = value;
            }
        }

        /// <summary>
        /// Gets or sets the trailing comment.
        /// </summary>
        public ICommentElement TrailingComment
        {
            get
            {
                return _trailingComment;
            }
            set
            {
                _trailingComment = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Allows an ICodeElementVisitor to process (or visit) this element.
        /// </summary>
        /// <param name="visitor">Visitor to accept the code element.</param>
        /// <remarks>See the Gang of Four Visitor design pattern.</remarks>
        public override void Accept(ICodeElementVisitor visitor)
        {
            visitor.VisitFieldElement(this);
        }

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>Clone of the element with the member element state copied.</returns>
        protected override MemberElement DoMemberClone()
        {
            FieldElement fieldElement = new FieldElement();

            //
            // Copy state
            //
            fieldElement._initialValue = _initialValue;
            fieldElement._isVolatile = _isVolatile;
            if (_trailingComment != null)
            {
                fieldElement._trailingComment = _trailingComment.Clone() as ICommentElement;
            }

            return fieldElement;
        }

        #endregion Methods
    }
}