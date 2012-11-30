namespace NArrange.Tests.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;

    using NArrange.Core;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the MSBuildSolutionParser class.
    /// </summary>
    [TestFixture]
    public class MSBuildSolutionParserTests
    {
        #region Fields

        /// <summary>
        /// Test solution file name.
        /// </summary>
        private string _testSolutionFile;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Writes the test solution to a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void WriteTestSolution(string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(
               typeof(MSBuildSolutionParserTests), "TestSolution.sln"))
            {
                Assert.IsNotNull(stream, "Test stream could not be retrieved.");

                StreamReader reader = new StreamReader(stream);
                string contents = reader.ReadToEnd();

                File.WriteAllText(fileName, contents);
            }
        }

        /// <summary>
        /// Tests parsing a null fileName.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseNullTest()
        {
            MSBuildSolutionParser solutionParser = new MSBuildSolutionParser();
            solutionParser.Parse(null);
        }

        /// <summary>
        /// Tests parsing project source files.
        /// </summary>
        [Test]
        public void ParseTest()
        {
            string[] testProjectFiles = new string[]
            {
                Path.Combine(Path.GetTempPath(), "TestProject.csproj")
            };

            MSBuildSolutionParser solutionParser = new MSBuildSolutionParser();
            ReadOnlyCollection<string> projectFiles = solutionParser.Parse(_testSolutionFile);

            Assert.AreEqual(testProjectFiles.Length, projectFiles.Count, "Unexpected number of project files.");

            foreach (string testProjectFile in testProjectFiles)
            {
                Assert.IsTrue(
                    projectFiles.Contains(testProjectFile),
                    "Test project file {0} was not included in the project file list.",
                    testProjectFile);
            }
        }

        /// <summary>
        /// Performs test fixture setup.
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _testSolutionFile = Path.GetTempFileName() + ".csproj";

            WriteTestSolution(_testSolutionFile);
        }

        /// <summary>
        /// Performs test fixture cleanup.
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            try
            {
                if (_testSolutionFile != null)
                {
                    File.Delete(_testSolutionFile);
                }
            }
            catch
            {
            }
        }

        #endregion Methods
    }
}