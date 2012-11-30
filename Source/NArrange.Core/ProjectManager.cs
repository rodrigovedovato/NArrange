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
    using System.Globalization;
    using System.IO;

    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Provides project/file retrieval and storage functionality.
    /// </summary>
    public class ProjectManager
    {
        #region Fields

        /// <summary>
        /// Code configuration.
        /// </summary>
        private readonly CodeConfiguration _configuration;

        /// <summary>
        /// Project extension handler dictionary.
        /// </summary>
        private Dictionary<string, ProjectHandler> _projectExtensionHandlers;

        /// <summary>
        /// Source file extension handler dictionary.
        /// </summary>
        private Dictionary<string, SourceHandler> _sourceExtensionHandlers;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new FileManager.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ProjectManager(CodeConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _configuration = configuration;

            Initialize();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <returns>The file extension.</returns>
        public static string GetExtension(string inputFile)
        {
            return Path.GetExtension(inputFile).TrimStart('.');
        }

        /// <summary>
        /// Determines whether or not the specified file is a solution.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <returns>
        /// 	<c>true</c> if the specified input file is solution; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSolution(string inputFile)
        {
            return SolutionParser.Instance.IsSolution(inputFile);
        }

        /// <summary>
        /// Determines whether or not the specified file can be parsed.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can parse the specified input file; otherwise, <c>false</c>.
        /// </returns>
        public bool CanParse(string inputFile)
        {
            return GetSourceHandler(inputFile) != null;
        }

        /// <summary>
        /// Gets all parse-able source files that are children of the specified
        /// solution or project.  If an individual source file is provided, the
        /// same file name is returned.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The list of source files.</returns>
        public ReadOnlyCollection<string> GetSourceFiles(string fileName)
        {
            List<string> sourceFiles = new List<string>();

            if (IsSolution(fileName))
            {
                sourceFiles.AddRange(GetSolutionSourceFiles(fileName));
            }
            else if (IsProject(fileName))
            {
                sourceFiles.AddRange(GetProjectSourceFiles(fileName));
            }
            else if (IsRecognizedSourceFile(fileName))
            {
                sourceFiles.Add(fileName);
            }
            else if (Directory.Exists(fileName))
            {
                sourceFiles.AddRange(GetDirectorySourceFiles(fileName));
            }

            return sourceFiles.AsReadOnly();
        }

        /// <summary>
        /// Retrieves an extension handler.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Source handler.</returns>
        public SourceHandler GetSourceHandler(string fileName)
        {
            string extension = GetExtension(fileName);

            SourceHandler handler = null;
            _sourceExtensionHandlers.TryGetValue(extension, out handler);

            return handler;
        }

        /// <summary>
        /// Determines whether or not the specified file is a project.
        /// </summary>
        /// <param name="inputFile">Input file.</param>
        /// <returns>Whether or not the file is a recognized project file.</returns>
        public bool IsProject(string inputFile)
        {
            return _projectExtensionHandlers.ContainsKey(GetExtension(inputFile));
        }

        /// <summary>
        /// Parses code elements from the input file.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="text">The text of the input file.</param>
        /// <returns>Collection of parsed code elements.</returns>
        public ReadOnlyCollection<ICodeElement> ParseElements(string inputFile, string text)
        {
            ReadOnlyCollection<ICodeElement> elements = null;
            SourceHandler sourceHandler = GetSourceHandler(inputFile);
            if (sourceHandler != null)
            {
                ICodeElementParser parser = sourceHandler.CodeParser;
                if (parser != null)
                {
                    parser.Configuration = _configuration;
                    using (StringReader reader = new StringReader(text))
                    {
                        elements = parser.Parse(reader);
                    }
                }
            }

            return elements;
        }

        /// <summary>
        /// Gets a boolean indicating whether or not the file is a 
        /// recognized source file.
        /// </summary>
        /// <param name="fileName">File to test.</param>
        /// <param name="extensions">Extension configurations.</param>
        /// <returns>A boolean indicating whehther or not the file is recognized.</returns>
        private static bool IsRecognizedFile(string fileName, ExtensionConfigurationCollection extensions)
        {
            bool isRecognizedFile = true;

            string extension = GetExtension(fileName);
            ExtensionConfiguration extensionConfiguration = null;
            foreach (ExtensionConfiguration extensionEntry in extensions)
            {
                if (extensionEntry.Name == extension)
                {
                    extensionConfiguration = extensionEntry;
                    break;
                }
            }

            if (extensionConfiguration != null && extensionConfiguration.FilterBy != null)
            {
                FilterBy filterBy = extensionConfiguration.FilterBy;
                FileFilter fileFilter = new FileFilter(filterBy.Condition);
                if (File.Exists(fileName))
                {
                    isRecognizedFile = fileFilter.IsMatch(new FileInfo(fileName));
                }
            }

            return isRecognizedFile;
        }

        /// <summary>
        /// Gets all source file for a directory.
        /// </summary>
        /// <param name="directory">Directory to process.</param>
        /// <returns>Collection of source files in the directory.</returns>
        private ReadOnlyCollection<string> GetDirectorySourceFiles(string directory)
        {
            List<string> sourceFiles = new List<string>();

            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                if (IsRecognizedSourceFile(file.FullName))
                {
                    sourceFiles.Add(file.FullName);
                }
            }

            return sourceFiles.AsReadOnly();
        }

        /// <summary>
        /// Retrieves an extension handler for a project file.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>Project handler for the file if available, otherwise null.</returns>
        private ProjectHandler GetProjectHandler(string fileName)
        {
            string extension = GetExtension(fileName);

            ProjectHandler projectHandler = null;
            _projectExtensionHandlers.TryGetValue(extension, out projectHandler);

            return projectHandler;
        }

        /// <summary>
        /// Gets all source files associated with a project.
        /// </summary>
        /// <param name="fileName">Project file name.</param>
        /// <returns>A collection of source files associated with the project.</returns>
        private ReadOnlyCollection<string> GetProjectSourceFiles(string fileName)
        {
            List<string> sourceFiles = new List<string>();

            ProjectHandler handler = GetProjectHandler(fileName);
            if (handler != null)
            {
                bool isRecognizedProject = IsRecognizedFile(fileName, handler.Configuration.ProjectExtensions);

                if (isRecognizedProject)
                {
                    IProjectParser projectParser = handler.ProjectParser;
                    ReadOnlyCollection<string> fileNames = projectParser.Parse(fileName);
                    foreach (string sourceFile in fileNames)
                    {
                        if (IsRecognizedSourceFile(sourceFile))
                        {
                            sourceFiles.Add(sourceFile);
                        }
                    }
                }
            }

            return sourceFiles.AsReadOnly();
        }

        /// <summary>
        /// Gets all source files associated with a solution.
        /// </summary>
        /// <param name="fileName">Solution file name.</param>
        /// <returns>Collection of source file names.</returns>
        private ReadOnlyCollection<string> GetSolutionSourceFiles(string fileName)
        {
            List<string> sourceFiles = new List<string>();

            ReadOnlyCollection<string> projectFiles = SolutionParser.Instance.Parse(fileName);

            foreach (string projectFile in projectFiles)
            {
                sourceFiles.AddRange(GetProjectSourceFiles(projectFile));
            }

            return sourceFiles.AsReadOnly();
        }

        /// <summary>
        /// Initializes the manager from the configuration supplied 
        /// during constuction.
        /// </summary>
        private void Initialize()
        {
            //
            // Load extension handlers
            //
            _projectExtensionHandlers = new Dictionary<string, ProjectHandler>(
                StringComparer.OrdinalIgnoreCase);
            _sourceExtensionHandlers = new Dictionary<string, SourceHandler>(
                StringComparer.OrdinalIgnoreCase);
            foreach (HandlerConfiguration handlerConfiguration in _configuration.Handlers)
            {
                switch (handlerConfiguration.HandlerType)
                {
                    case HandlerType.Source:
                        SourceHandlerConfiguration sourceConfiguration = handlerConfiguration as SourceHandlerConfiguration;
                        SourceHandler sourceHandler = new SourceHandler(sourceConfiguration);
                        foreach (ExtensionConfiguration extension in sourceConfiguration.SourceExtensions)
                        {
                            _sourceExtensionHandlers.Add(extension.Name, sourceHandler);
                        }
                        break;

                    case HandlerType.Project:
                        ProjectHandlerConfiguration projectConfiguration = handlerConfiguration as ProjectHandlerConfiguration;
                        ProjectHandler projectHandler = new ProjectHandler(projectConfiguration);
                        foreach (ExtensionConfiguration extension in projectConfiguration.ProjectExtensions)
                        {
                            _projectExtensionHandlers.Add(extension.Name, projectHandler);
                        }
                        break;

                    default:
                        throw new InvalidOperationException(
                            string.Format(
                            CultureInfo.InvariantCulture,
                            "Unrecognized handler configuration {0}",
                            handlerConfiguration));
                }
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether or not the file is a 
        /// recognized source file.
        /// </summary>
        /// <param name="fileName">File to test.</param>
        /// <returns>A boolean indicating whehther or not the file is recognized.</returns>
        private bool IsRecognizedSourceFile(string fileName)
        {
            bool recognized = false;

            SourceHandler handler = GetSourceHandler(fileName);
            if (handler != null)
            {
                recognized = IsRecognizedFile(fileName, handler.Configuration.SourceExtensions);
            }

            return recognized;
        }

        #endregion Methods
    }
}