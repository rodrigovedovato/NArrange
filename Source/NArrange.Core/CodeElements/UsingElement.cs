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
    /// Represents a using/import statement within code.
    /// </summary>
    public sealed class UsingElement : CommentedElement
    {
        #region Fields

        /// <summary>
        /// Whether or not this using element can be moved between code levels.
        /// This should be determined by the language parser.
        /// </summary>
        private bool _isMovable;

        /// <summary>
        /// Import redefine.
        /// </summary>
        private string _redefine;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new Using directive code element.
        /// </summary>
        public UsingElement()
        {
        }

        /// <summary>
        /// Creates a new Using directive code element.
        /// </summary>
        /// <param name="name">Namespace or type name.</param>
        public UsingElement(string name)
            : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Creates a new Using directive code element.
        /// </summary>
        /// <param name="name">Namespace or type name.</param>
        /// <param name="redefine">Redefined type or namespace.</param>
        public UsingElement(string name, string redefine)
            : this(name)
        {
            this.Redefine = redefine;
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
                return ElementType.Using;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this using element 
        /// can be moved between code levels (e.g. file or namespace) as 
        /// determined by the language parser.
        /// </summary>
        public bool IsMovable
        {
            get
            {
                return _isMovable;
            }
            set
            {
                _isMovable = value;
            }
        }

        /// <summary>
        /// Gets or sets the namespace to be redefined to the name of this 
        /// UsingElement.
        /// </summary>
        public string Redefine
        {
            get
            {
                return _redefine;
            }
            set
            {
                _redefine = value;
            }
        }

        /// <summary>
        /// Gets the type of the using statement.
        /// </summary>
        public UsingType Type
        {
            get
            {
                UsingType usingType = UsingType.Namespace;
                if (!string.IsNullOrEmpty(this.Redefine))
                {
                    usingType = UsingType.Alias;
                }

                return usingType;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Allows an ICodeElementVisitor to process (or visit) this element.
        /// </summary>
        /// <remarks>See the Gang of Four Visitor design pattern.</remarks>
        /// <param name="visitor">The visitory that will accept the element.</param>
        public override void Accept(ICodeElementVisitor visitor)
        {
            visitor.VisitUsingElement(this);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Clone of the code element.</returns>
        protected override CodeElement DoClone()
        {
            UsingElement clone = new UsingElement();
            clone._redefine = _redefine;
            clone._isMovable = _isMovable;

            return clone;
        }

        #endregion Methods
    }
}