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
    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    /// Utility methods for zipping and unzipping files.
    /// </summary>
    public static class ZipUtilities
    {
        #region Methods

        /// <summary>
        /// Unzips the specified zip file to the destination directory.
        /// </summary>
        /// <param name="zipFileName">Zip file name.</param>
        /// <param name="targetDirectory">Target extraction directory.</param>
        public static void Unzip(string zipFileName, string targetDirectory)
        {
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(zipFileName, targetDirectory, null);
        }

        /// <summary>
        /// Writes the specified directory to a new zip file.
        /// </summary>
        /// <param name="sourceDirectory">Directory with files to add.</param>
        /// <param name="zipFileName">Output file name.</param>
        public static void Zip(string sourceDirectory, string zipFileName)
        {
            FastZip fastZip = new FastZip();
            fastZip.RestoreAttributesOnExtract = true;
            fastZip.RestoreDateTimeOnExtract = true;
            fastZip.CreateZip(zipFileName, sourceDirectory, true, null);
        }

        #endregion Methods
    }
}