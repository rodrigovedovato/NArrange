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

namespace NArrange.Core
{
    using System.IO;

    using NArrange.Core.Configuration;

    /// <summary>
    /// Class for determining whether or not a file matches 
    /// filter criteria.
    /// </summary>
    public class FileFilter : IFileFilter
    {
        #region Fields

        /// <summary>
        /// Condition that must be met by files, if any.
        /// </summary>
        private readonly IConditionExpression _conditionExpression;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new FileFilter.
        /// </summary>
        /// <param name="conditionExpression">The condition expression text.</param>
        public FileFilter(string conditionExpression)
        {
            _conditionExpression = ConditionExpressionParser.Instance.Parse(conditionExpression);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Determines whether or not the specified file matches the
        /// filter criteria.
        /// </summary>
        /// <param name="file">File info for the file to analyze.</param>
        /// <returns>
        /// True if the file matches the filter, otherwise false.
        /// </returns>
        public bool IsMatch(FileInfo file)
        {
            bool isMatch = false;

            if (file != null)
            {
                isMatch = ConditionExpressionEvaluator.Instance.Evaluate(_conditionExpression, file);
            }

            return isMatch;
        }

        #endregion Methods
    }
}