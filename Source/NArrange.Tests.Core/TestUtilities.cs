namespace NArrange.Tests.Core
{
    using System.CodeDom.Compiler;
    using System.IO;

    using NUnit.Framework;

    /// <summary>
    /// Test utilities.
    /// </summary>
    public static class TestUtilities
    {
        #region Properties

        /// <summary>
        /// Gets the test code configuration files.
        /// </summary>
        public static FileInfo[] TestConfigurationFiles
        {
            get
            {
                DirectoryInfo testConfigDirectory = new DirectoryInfo("TestConfigurations");
                FileInfo[] testConfigFiles = testConfigDirectory.GetFiles("*.xml");

                return testConfigFiles;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Verifies that a file is not empty.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public static void AssertNotEmpty(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                Assert.IsTrue(fs.Length > 0, "File {0} should not be empty.", fileName);
            }
        }

        /// <summary>
        /// Retrieves a compiler error from a compiler result.
        /// </summary>
        /// <param name="results">Compiler results.</param>
        /// <returns>Compiler error if present, otherwise null.</returns>
        public static CompilerError GetCompilerError(CompilerResults results)
        {
            CompilerError error = null;

            foreach (CompilerError compilerError in results.Errors)
            {
                if (!compilerError.IsWarning)
                {
                    error = compilerError;
                    break;
                }
            }

            return error;
        }

        #endregion Methods
    }
}