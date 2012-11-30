namespace NArrange.Tests.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;

    using NArrange.Core;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the MSBuildProjectParser class.
    /// </summary>
    [TestFixture]
    public class MSBuildProjectParserTests
    {
        #region Fields

        /// <summary>
        /// Test project file name.
        /// </summary>
        private string _testProjectFile;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Writes the test project to a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void WriteTestProject(string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(
               typeof(MSBuildProjectParserTests), "TestProject.csproj"))
            {
                Assert.IsNotNull(stream, "Test stream could not be retrieved.");

                StreamReader reader = new StreamReader(stream);
                string contents = reader.ReadToEnd();

                File.WriteAllText(fileName, contents);
            }
        }

        /// <summary>
        /// Tests parsing a null project fileName.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseNullTest()
        {
            MSBuildProjectParser projectParser = new MSBuildProjectParser();
            projectParser.Parse(null);
        }

        /// <summary>
        /// Tests parsing project source files.
        /// </summary>
        [Test]
        public void ParseTest()
        {
            string[] testSourceFiles = new string[]
            {
                Path.Combine(Path.GetTempPath(), "ClassMembers.cs"),
                Path.Combine(Path.GetTempPath(), "ClassDefinition.cs"),
                Path.Combine(Path.GetTempPath(), "BlahBlahBlah.cs"),
                Path.Combine(Path.GetTempPath(), "Folder1\\Class2.cs"),
                Path.Combine(Path.GetTempPath(), "Folder1\\Folder2\\Class3.cs"),
                Path.Combine(Path.GetTempPath(), "Properties\\AssemblyInfo.cs"),
                Path.Combine(Path.GetTempPath(), "Test.Designer.cs")
            };

            MSBuildProjectParser projectParser = new MSBuildProjectParser();

            ReadOnlyCollection<string> sourceFiles = projectParser.Parse(_testProjectFile);

            Assert.AreEqual(testSourceFiles.Length, sourceFiles.Count, "Unexpected number of source files.");

            foreach (string testSourceFile in testSourceFiles)
            {
                Assert.IsTrue(
                    sourceFiles.Contains(testSourceFile),
                    "Test source file {0} was not included in the source file list.",
                    testSourceFile);
            }
        }

        /// <summary>
        /// Performs test fixture setup.
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _testProjectFile = Path.GetTempFileName() + ".csproj";

            WriteTestProject(_testProjectFile);
        }

        /// <summary>
        /// Performs test fixture cleanup.
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            try
            {
                if (_testProjectFile != null)
                {
                    File.Delete(_testProjectFile);
                }
            }
            catch
            {
            }
        }

        #endregion Methods
    }
}