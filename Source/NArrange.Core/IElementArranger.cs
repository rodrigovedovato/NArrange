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

namespace NArrange.Core
{
    using NArrange.Core.CodeElements;

    /// <summary>
    /// Element arranger interface.
    /// </summary>
    public interface IElementArranger
    {
        #region Methods

        /// <summary>
        /// Arranges the specified element within the parent element.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="codeElement">The code element.</param>
        void ArrangeElement(ICodeElement parentElement, ICodeElement codeElement);

        /// <summary>
        /// Determines whether or not this arranger can handle arrangement of
        /// the specified element.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can arrange the specified code element; otherwise, <c>false</c>.
        /// </returns>
        bool CanArrange(ICodeElement codeElement);

        /// <summary>
        /// Determines whether or not this arranger can handle arrangement of
        /// the specified element.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="codeElement">The code element.</param>
        /// <returns>
        /// <c>true</c> if this instance can arrange the specified parent element; otherwise, <c>false</c>.
        /// </returns>
        bool CanArrange(ICodeElement parentElement, ICodeElement codeElement);

        #endregion Methods
    }
}