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
    using System;
    using System.Collections.ObjectModel;
    using System.IO;

    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Base class for writing code elements to a file.
    /// </summary>
    public abstract class CodeWriter : ICodeElementWriter
    {
        #region Fields

        /// <summary>
        /// Code configuration.
        /// </summary>
        private CodeConfiguration _configuration;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the code configuration.
        /// </summary>
        public CodeConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = CodeConfiguration.Default;
                }

                return _configuration;
            }
            set
            {
                _configuration = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Writes each element using the specified visitor.
        /// </summary>
        /// <param name="codeElements">The code elements.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="visitor">The visitor.</param>
        public static void WriteVisitElements(
            ReadOnlyCollection<ICodeElement> codeElements,
            TextWriter writer,
            ICodeElementVisitor visitor)
        {
            for (int index = 0; index < codeElements.Count; index++)
            {
                ICodeElement codeElement = codeElements[index];
                if (codeElement != null)
                {
                    CommentedElement commentedElement = codeElement as CommentedElement;
                    if (index > 0 &&
                        ((commentedElement != null && commentedElement.HeaderComments.Count > 0) ||
                        codeElement is NamespaceElement || codeElement is TypeElement ||
                        codeElement is RegionElement || codeElement is ConditionDirectiveElement ||
                        (codeElement is MemberElement && !(codeElement is FieldElement)) ||
                        (!(codeElement is GroupElement) &&
                        codeElement.ElementType != codeElements[index - 1].ElementType) ||
                        codeElements[index - 1] is GroupElement))
                    {
                        writer.WriteLine();
                    }

                    codeElement.Accept(visitor);

                    if (codeElements.Count > 1 && index < codeElements.Count - 1)
                    {
                        writer.WriteLine();
                        if ((codeElement is RegionElement || codeElement is ConditionDirectiveElement) &&
                            codeElement.Parent == null &&
                            !(codeElements.Count > index + 1 && codeElements[index + 1] is NamespaceElement))
                        {
                            writer.WriteLine();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes code elements to the specified text writer.
        /// </summary>
        /// <param name="codeElements">Read only collection of elements</param>
        /// <param name="writer">Code file writer</param>
        public void Write(ReadOnlyCollection<ICodeElement> codeElements, TextWriter writer)
        {
            if (codeElements == null)
            {
                throw new ArgumentNullException("codeElements");
            }

            DoWriteElements(codeElements, writer);
        }

        /// <summary>
        /// Template method for inheritors to write a collection of code elements to the
        /// specified text writer.
        /// </summary>
        /// <param name="codeElements">The code elements.</param>
        /// <param name="writer">The writer.</param>
        protected abstract void DoWriteElements(ReadOnlyCollection<ICodeElement> codeElements, TextWriter writer);

        #endregion Methods
    }
}