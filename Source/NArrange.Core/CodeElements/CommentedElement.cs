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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Code element base class for elements with header comments.
    /// </summary>
    public abstract class CommentedElement : CodeElement
    {
        #region Fields

        /// <summary>
        /// Synchronization lock for the comments collection.
        /// </summary>
        private readonly object _commentsLock = new object();

        /// <summary>
        /// Comments for this element.
        /// </summary>
        private List<ICommentElement> _comments;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the collection of header comments.
        /// </summary>
        public ReadOnlyCollection<ICommentElement> HeaderComments
        {
            get
            {
                return BaseHeaderComments.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the base header comment collection.
        /// </summary>
        protected List<ICommentElement> BaseHeaderComments
        {
            get
            {
                if (_comments == null)
                {
                    lock (_commentsLock)
                    {
                        if (_comments == null)
                        {
                            _comments = new List<ICommentElement>();
                        }
                    }
                }

                return _comments;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds a header comment to this element.
        /// </summary>
        /// <param name="comment">The comment.</param>
        public void AddHeaderComment(ICommentElement comment)
        {
            BaseHeaderComments.Add(comment);
        }

        /// <summary>
        /// Adds a header comment line to this element.
        /// </summary>
        /// <param name="commentLine">The comment line.</param>
        public void AddHeaderCommentLine(string commentLine)
        {
            BaseHeaderComments.Add(new CommentElement(commentLine));
        }

        /// <summary>
        /// Adds a header comment line to this element.
        /// </summary>
        /// <param name="commentLine">Comment line text.</param>
        /// <param name="xmlComment">Whether or not the comment is an XML comment.</param>
        public void AddHeaderCommentLine(string commentLine, bool xmlComment)
        {
            if (xmlComment)
            {
                BaseHeaderComments.Add(new CommentElement(commentLine, CommentType.XmlLine));
            }
            else
            {
                AddHeaderCommentLine(commentLine);
            }
        }

        /// <summary>
        /// Clears all header comments.
        /// </summary>
        public void ClearHeaderCommentLines()
        {
            BaseHeaderComments.Clear();
        }

        /// <summary>
        /// Creates a clone of the instance and assigns any state.
        /// </summary>
        /// <returns>Clone of this instance.</returns>
        public override object Clone()
        {
            CommentedElement clone = base.Clone() as CommentedElement;

            foreach (ICommentElement comment in HeaderComments)
            {
                ICommentElement commentClone = comment.Clone() as ICommentElement;
                clone.AddHeaderComment(commentClone);
            }

            return clone;
        }

        #endregion Methods
    }
}