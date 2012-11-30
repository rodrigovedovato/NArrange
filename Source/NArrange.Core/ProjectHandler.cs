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
    using System.Reflection;

    using NArrange.Core.Configuration;

    /// <summary>
    /// This class provides instances for handling project parsing requests 
    /// based on file extension.
    /// </summary>
    public sealed class ProjectHandler
    {
        #region Fields

        /// <summary>
        /// Configuration for the project handler.
        /// </summary>
        private readonly ProjectHandlerConfiguration _configuration;

        /// <summary>
        /// Assembly used for loading the project handler Type.
        /// </summary>
        private Assembly _assembly;

        /// <summary>
        /// Project parser for getting a list of source files in a project.
        /// </summary>
        private IProjectParser _projectParser;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new ProjectHandler.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ProjectHandler(ProjectHandlerConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _configuration = configuration;

            Initialize();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the handler configuration used to create this ProjectHandler.
        /// </summary>
        public ProjectHandlerConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        /// <summary>
        /// Gets the project parser associated with the extension.
        /// </summary>
        public IProjectParser ProjectParser
        {
            get
            {
                return _projectParser;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes the extension handler.
        /// </summary>
        private void Initialize()
        {
            Type projectParserType = null;
            string assemblyName = _configuration.AssemblyName;
            if (string.IsNullOrEmpty(assemblyName))
            {
                _assembly = GetType().Assembly;
            }
            else
            {
                _assembly = Assembly.Load(assemblyName);
            }

            string projectParserTypeName = _configuration.ParserType;
            if (string.IsNullOrEmpty(projectParserTypeName))
            {
                projectParserType = typeof(MSBuildProjectParser);
            }
            else
            {
                projectParserType = _assembly.GetType(projectParserTypeName);
            }

            _projectParser = Activator.CreateInstance(projectParserType) as IProjectParser;
        }

        #endregion Methods
    }
}