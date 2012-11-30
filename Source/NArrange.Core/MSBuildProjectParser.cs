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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Parses an individual MSBuild project (e.g. .csproj, .vbproj) for 
    /// individual source file names.
    /// </summary>
    public class MSBuildProjectParser : IProjectParser
    {
        #region Methods

        /// <summary>
        /// Parses source file names from a project file.
        /// </summary>
        /// <param name="projectFile">Project file name.</param>
        /// <returns>A list of source code filenames</returns>
        public virtual ReadOnlyCollection<string> Parse(string projectFile)
        {
            if (projectFile == null)
            {
                throw new ArgumentNullException("projectFile");
            }

            string projectPath = Path.GetDirectoryName(projectFile);
            List<string> sourceFiles = new List<string>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(projectFile);

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            namespaceManager.AddNamespace("ns", "http://schemas.microsoft.com/developer/msbuild/2003");

            XmlNodeList nodes = xmlDocument.SelectNodes("//ns:Compile", namespaceManager);
            foreach (XmlNode node in nodes)
            {
                XmlAttribute includeAttribute = node.Attributes["Include"];
                if (includeAttribute != null)
                {
                    if (node.SelectSingleNode("ns:Link", namespaceManager) == null)
                    {
                        string fileName = includeAttribute.Value;

                        string sourceFilePath = Path.Combine(projectPath, fileName);
                        sourceFiles.Add(sourceFilePath);
                    }
                }
            }

            return sourceFiles.AsReadOnly();
        }

        #endregion Methods
    }
}