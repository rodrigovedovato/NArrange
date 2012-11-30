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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// MonoDevelop solution file parser.
    /// </summary>
    public sealed class MonoDevelopSolutionParser : ISolutionParser
    {
        #region Fields

        /// <summary>
        /// File extensions handled by this solution parser.
        /// </summary>
        private readonly List<string> _extensions = new List<string>(new string[] { "mds" });

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets a list of extensions supported by this solution parser.
        /// </summary>
        public ReadOnlyCollection<string> Extensions
        {
            get
            {
                return _extensions.AsReadOnly();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Parses project file names from a solution file.
        /// </summary>
        /// <param name="solutionFile">Solution file name.</param>
        /// <returns>A list of project file names</returns>
        public ReadOnlyCollection<string> Parse(string solutionFile)
        {
            if (solutionFile == null)
            {
                throw new ArgumentNullException("solutionFile");
            }

            string solutionPath = Path.GetDirectoryName(solutionFile);

            List<string> projectFiles = new List<string>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(solutionFile);

            XmlNodeList nodes = xmlDocument.SelectNodes("//Entry");
            foreach (XmlNode node in nodes)
            {
                XmlAttribute nameAttribute = node.Attributes["filename"];
                if (nameAttribute != null)
                {
                    string projectFile = nameAttribute.Value;
                    string projectPath = Path.Combine(solutionPath, projectFile);
                    if (!string.IsNullOrEmpty(Path.GetExtension(projectPath)))
                    {
                        projectFiles.Add(projectPath);
                    }
                }
            }

            return projectFiles.AsReadOnly();
        }

        #endregion Methods
    }
}