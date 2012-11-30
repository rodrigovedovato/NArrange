#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2007-2008 James Nies and NArrange contributors.
 *      All rights reserved.
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
 * Contributors:
 *      James Nies
 *      - Initial creation
 *      Justin Dearing
 *      - Removed unused using statements
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.TestSourceFinder
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;

    using NArrange.Tests.Core;
    using NArrange.Tests.CSharp;
    using NArrange.Tests.VisualBasic;

    /// <summary>
    /// The TestSourceFinder application finds valid source files in the specified directory that
    /// do not have any dependencies.  These source files are written to the specified output
    /// directory and can be used as test cases for NArrange (see SourceTester app).
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        /// The application entry point.
        /// </summary>
        /// <param name="args">The commend args.</param>
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Expected input and output directories as parameters.");
            }

            string inputDirectory = args[0];
            string outputDirectory = args[1];

            try
            {
                FindFiles(inputDirectory, outputDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Compiles the source file.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="source">The source.</param>
        /// <returns>Compiler results.</returns>
        private static CompilerResults CompileSourceFile(FileInfo sourceFile, string source)
        {
            CompilerResults results = null;

            string extension = sourceFile.Extension.TrimStart('.').ToUpperInvariant();

            switch (extension)
            {
                case "CS":
                    results = CSharpTestFile.Compile(source, sourceFile.GetHashCode().ToString());
                    break;

                case "VB":
                    results = VBTestFile.Compile(source, sourceFile.GetHashCode().ToString());
                    break;
            }

            return results;
        }

        /// <summary>
        /// Finds files to process.
        /// </summary>
        /// <param name="inputDirectory">The input directory.</param>
        /// <param name="outputDirectory">The output directory.</param>
        private static void FindFiles(string inputDirectory, string outputDirectory)
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            Console.WriteLine("Finding source files...");
            FileInfo[] allSourceFiles = GetSourceFileNames(inputDirectory);

            int processed = 0;
            int copied = 0;

            Console.WriteLine("Analyzing source files...");
            foreach (FileInfo sourceFile in allSourceFiles)
            {
                Console.WriteLine("Analyzing {0}", sourceFile.FullName);
                processed++;

                string source;
                using (StreamReader reader = sourceFile.OpenText())
                {
                    source = reader.ReadToEnd();
                }

                CompilerResults results = CompileSourceFile(
                    sourceFile, source);

                CompilerError error = TestUtilities.GetCompilerError(results);
                if (error == null)
                {
                    Console.WriteLine("Succesfully compiled {0}", sourceFile.FullName);
                    string destination = Path.Combine(outputDirectory, sourceFile.Name);
                    sourceFile.CopyTo(destination, true);
                    copied++;
                }
            }

            Console.WriteLine("Processed " +
                processed.ToString() + " source files");
            Console.WriteLine("Copied " +
                copied.ToString() + " source files");
        }

        /// <summary>
        /// Gets the source file names.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <returns>Array of source filess.</returns>
        private static FileInfo[] GetSourceFileNames(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            List<FileInfo> sourceFiles = new List<FileInfo>();
            sourceFiles.AddRange(directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories));
            sourceFiles.AddRange(directoryInfo.GetFiles("*.vb", SearchOption.AllDirectories));

            return sourceFiles.ToArray();
        }

        #endregion Methods
    }
}