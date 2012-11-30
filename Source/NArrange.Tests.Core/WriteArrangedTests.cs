namespace NArrange.Tests.Core
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.IO;

    using NArrange.Core;
    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Base test fixture for writing arranged code elements.
    /// </summary>
    /// <typeparam name="TCodeParser">Parser type.</typeparam>
    /// <typeparam name="TCodeWriter">Code writer type.</typeparam>
    public abstract class WriteArrangedTests<TCodeParser, TCodeWriter>
        where TCodeParser : ICodeElementParser, new()
        where TCodeWriter : ICodeElementWriter, new()
    {
        #region Properties

        /// <summary>
        /// Gets an array of valid test files.
        /// </summary>
        public abstract ISourceCodeTestFile[] ValidTestFiles
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Tests writing a tree of arranged elements.
        /// </summary>
        [Test]
        public void WriteArrangedElementTest()
        {
            string outputDirectory = "Arranged";
            if (Directory.Exists(outputDirectory))
            {
                try
                {
                    Directory.Delete(outputDirectory, true);
                }
                catch
                {
                }
            }

            ReadOnlyCollection<ICodeElement> testElements;

            ISourceCodeTestFile[] testFiles = ValidTestFiles;
            foreach (ISourceCodeTestFile testFile in testFiles)
            {
                using (TextReader reader = testFile.GetReader())
                {
                    TCodeParser parser = new TCodeParser();
                    testElements = parser.Parse(reader);

                    Assert.IsTrue(testElements.Count > 0, "Test file does not contain any elements.");
                }

                foreach (FileInfo configFile in TestUtilities.TestConfigurationFiles)
                {
                    try
                    {
                        CodeConfiguration configuration = CodeConfiguration.Load(configFile.FullName);
                        CodeArranger arranger = new CodeArranger(configuration);

                        ReadOnlyCollection<ICodeElement> arranged = arranger.Arrange(testElements);

                        //
                        // Write the arranged elements
                        //
                        StringWriter writer = new StringWriter();
                        TCodeWriter codeWriter = new TCodeWriter();
                        codeWriter.Configuration = configuration;
                        codeWriter.Write(arranged, writer);

                        string text = writer.ToString();

                        // Write the file to the output directory for further analysis
                        Directory.CreateDirectory(outputDirectory);

                        string configurationDirectory = Path.Combine(
                            outputDirectory,
                            Path.GetFileNameWithoutExtension(configFile.FullName));
                        Directory.CreateDirectory(configurationDirectory);

                        File.WriteAllText(Path.Combine(configurationDirectory, testFile.Name), text);

                        //
                        // Verify that the arranged file still compiles sucessfully.
                        //
                        CompilerResults results = Compile(text, testFile.Name);
                        CompilerError error = TestUtilities.GetCompilerError(results);
                        if (error != null)
                        {
                            Assert.Fail(
                                "Arranged source code should not produce compiler errors. " +
                                "Error: {0} - {1}, line {2}, column {3} ",
                                error.ErrorText,
                                testFile.Name,
                                error.Line,
                                error.Column);
                        }
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail("Configuration " + configFile + ": " + ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Compiles source code text to an assembly with the specified name.
        /// </summary>
        /// <param name="text">Text to compile.</param>
        /// <param name="assemblyName">Output assembly name.</param>
        /// <returns>Compiler results.</returns>
        protected abstract CompilerResults Compile(string text, string assemblyName);

        #endregion Methods
    }
}