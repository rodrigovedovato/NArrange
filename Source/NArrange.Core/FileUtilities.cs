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
    using System.IO;
    using System.Text;

    /// <summary>
    /// File utility methods.
    /// </summary>
    public static class FileUtilities
    {
        #region Methods

        /// <summary>
        /// Gets the string representation of a file attribute.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="file">The file to get the attribute for.</param>
        /// <returns>The file attribute as text.</returns>
        public static string GetAttribute(FileAttributeType attributeType, FileInfo file)
        {
            string attributeString = null;

            if (file != null)
            {
                switch (attributeType)
                {
                    case FileAttributeType.Name:
                        attributeString = file.Name;
                        break;

                    case FileAttributeType.Path:
                        attributeString = file.FullName;
                        break;

                    case FileAttributeType.Attributes:
                        attributeString = EnumUtilities.ToString(file.Attributes);
                        break;

                    default:
                        attributeString = string.Empty;
                        break;
                }
            }

            if (attributeString == null)
            {
                attributeString = string.Empty;
            }

            return attributeString;
        }

        /// <summary>
        /// Reads a file to determine its encoding.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filename)
        {
            // Byte Order Marks(BOMs) - from writing U+FEFF
            // 00 00 FE FF      UTF-32, big-endian
            // FF FE 00 00      UTF-32, little-endian
            // FE FF            UTF-16, big-endian
            // FF FE            UTF-16, little-endian
            // EF BB BF         UTF-8
            byte[][] bomMap = new byte[][]
            {
                // new byte[]{ 0x00, 0x00, 0xFE, 0xFF },
                new byte[] { 0xFF, 0xFE, 0x00, 0x00 },
                new byte[] { 0xFE, 0xFF },
                new byte[] { 0xFF, 0xFE },
                new byte[] { 0xEF, 0xBB, 0xBF }
            };

            Encoding[] bomEncodings = new Encoding[]
            {
                // null,
                Encoding.UTF32,
                Encoding.BigEndianUnicode,
                Encoding.Unicode,
                Encoding.UTF8
            };

            Encoding encoding = null;
            byte[] bom = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };

            using (Stream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                int unicodeCount = 0;
                bool ansi = true;
                byte[] data = new byte[1];

                while (fileStream.Read(data, 0, 1) > 0)
                {
                    if (fileStream.Position < 4)
                    {
                        bom[fileStream.Position - 1] = data[0];
                    }
                    else if (fileStream.Position == 4)
                    {
                        for (int bomMapIndex = 0; bomMapIndex < bomMap.Length; bomMapIndex++)
                        {
                            byte[] bomCandidate = bomMap[bomMapIndex];

                            bool isMatch = true;
                            for (int bomTestIndex = 0; bomTestIndex < bomCandidate.Length && isMatch;
                                bomTestIndex++)
                            {
                                if (bom[bomTestIndex] != bomCandidate[bomTestIndex])
                                {
                                    isMatch = false;
                                }
                            }

                            if (isMatch)
                            {
                                encoding = bomEncodings[bomMapIndex];
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                    //
                    // Just to be safe, also check for a non-ansi byte
                    // in the first few bytes.  This should trigger Unicode detection
                    // with the StreamReader below.
                    //
                    if (data[0] < 32)
                    {
                        unicodeCount++;
                        if (unicodeCount > 2)
                        {
                            ansi = false;
                            break;
                        }
                    }
                }

                if (encoding == null)
                {
                    fileStream.Position = 0;
                    string text = string.Empty;
                    long length = 0;
                    using (StreamReader reader = new StreamReader(fileStream, true))
                    {
                        length = fileStream.Length;
                        text = reader.ReadToEnd();
                        encoding = reader.CurrentEncoding;
                    }

                    if (ansi && text.Length == length)
                    {
                        encoding = Encoding.Default;
                    }
                }
            }

            return encoding;
        }

        #endregion Methods
    }
}