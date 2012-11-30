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
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Interface for a code element.
    /// </summary>
    public interface ICodeElement : ICloneable
    {
        #region Properties

        /// <summary>
        /// Gets any children of this code element.
        /// </summary>
        /// <remarks>For a class, this would return
        /// all member, methods, properties and nested types.</remarks>
        ReadOnlyCollection<ICodeElement> Children
        {
            get;
        }

        /// <summary>
        /// Gets the element type.
        /// </summary>
        ElementType ElementType
        {
            get;
        }

        /// <summary>
        /// Gets the name of the code element.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets or sets the parent of this element.
        /// </summary>
        ICodeElement Parent
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Allows an ICodeElementVisitor to process (or visit) this element.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        /// <remarks>See the Gang of Four Visitor design pattern.</remarks>
        void Accept(ICodeElementVisitor visitor);

        /// <summary>
        /// Adds a child to this element.
        /// </summary>
        /// <param name="childElement">The child element to add.</param>
        void AddChild(ICodeElement childElement);

        /// <summary>
        /// Removes all child elements.
        /// </summary>
        void ClearChildren();

        /// <summary>
        /// Inserts a child at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="childElement">The child element.</param>
        void InsertChild(int index, ICodeElement childElement);

        /// <summary>
        /// Removes a child from this element.
        /// </summary>
        /// <param name="childElement">The child element to remove.</param>
        void RemoveChild(ICodeElement childElement);

        #endregion Methods
    }
}