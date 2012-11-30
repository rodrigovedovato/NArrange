namespace NArrange.Tests.ConsoleApplication
{
    using System;
    using System.IO;
    using System.Reflection;

    using NArrange.ConsoleApplication;
    using NArrange.Core;
    using NArrange.Tests.Core;
    using NArrange.Tests.CSharp;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the Program class
    /// </summary>
    [TestFixture]
    public class ProgramTests
    {
        #region Fields

        /// <summary>
        /// Invalid extension test source file.
        /// </summary>
        private string _testInvalidExtensionFile;

        /// <summary>
        /// Invalid source file.
        /// </summary>
        private string _testInvalidSourceFile;

        /// <summary>
        /// MSBuild project file.
        /// </summary>
        private string _testMBBuildProjectFile;

        /// <summary>
        /// MonoDevelop project file.
        /// </summary>
        private string _testMonoDevelopProjectFile;

        /// <summary>
        /// MonoDevelop solution file.
        /// </summary>
        private string _testMonoDevelopSolutionFile;

        /// <summary>
        /// MSbuild solution file.
        /// </summary>
        private string _testMSBuildSolutionFile;

        /// <summary>
        /// Valid source file.
        /// </summary>
        private string _testValidSourceFile1;

        /// <summary>
        /// Valid source file.
        /// </summary>
        private string _testValidSourceFile2;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Tests arranging all files in a directory.
        /// </summary>
        [Test]
        public void ArrangeDirectoryEmptyTest()
        {
            string testSourceParentDirectory = Path.Combine(Path.GetTempPath(), "TestSource");

            try
            {
                try
                {
                    Directory.CreateDirectory(testSourceParentDirectory);
                }
                catch
                {
                }

                TestLogger logger = new TestLogger();
                bool success = Arrange(logger, testSourceParentDirectory);

                string log = logger.ToString();
                Assert.IsTrue(success, "Expected directory to be arranged succesfully - " + log);
                Assert.IsTrue(
                    logger.HasMessage(LogLevel.Verbose, "0 files written."),
                    "Expected 0 files to be written - " + log);
            }
            finally
            {
                try
                {
                    Directory.Delete(testSourceParentDirectory, true);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Tests arranging all files in a directory.
        /// </summary>
        [Test]
        public void ArrangeDirectoryTest()
        {
            string testSourceParentDirectory = Path.Combine(Path.GetTempPath(), "TestSource");
            string testSourceChildDirectory = Path.Combine(testSourceParentDirectory, "Child");

            try
            {
                try
                {
                    Directory.CreateDirectory(testSourceParentDirectory);
                }
                catch
                {
                }

                try
                {
                    Directory.CreateDirectory(testSourceChildDirectory);
                }
                catch
                {
                }

                File.Copy(
                    _testValidSourceFile1,
                    Path.Combine(testSourceParentDirectory, Path.GetFileName(_testValidSourceFile1)),
                    true);

                File.Copy(
                    _testValidSourceFile2,
                    Path.Combine(testSourceParentDirectory, Path.GetFileName(_testValidSourceFile2)),
                    true);

                File.Copy(
                    _testValidSourceFile1,
                    Path.Combine(testSourceChildDirectory, Path.GetFileName(_testValidSourceFile1)),
                    true);

                File.Copy(
                    _testValidSourceFile2,
                    Path.Combine(testSourceChildDirectory, Path.GetFileName(_testValidSourceFile2)),
                    true);

                TestLogger logger = new TestLogger();
                bool success = Arrange(logger, testSourceParentDirectory);

                string log = logger.ToString();
                Assert.IsTrue(success, "Expected directory to be arranged succesfully - " + log);
                Assert.IsTrue(
                    logger.HasMessage(LogLevel.Verbose, "4 files written."),
                    "Expected 4 files to be written - " + log);
            }
            finally
            {
                try
                {
                    Directory.Delete(testSourceParentDirectory, true);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Tests arranging a single source file with an invalid configuration
        /// </summary>
        [Test]
        public void ArrangeInvalidConfigurationTest()
        {
            string testConfiguration = Path.GetTempFileName();
            File.WriteAllText(testConfiguration, "<xml");

            try
            {
                TestLogger logger = new TestLogger();

                bool success = Arrange(logger, _testValidSourceFile1, "/c:" + testConfiguration);

                Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
                Assert.IsTrue(logger.HasPartialMessage(LogLevel.Error, "Unable to load configuration file"));
            }
            finally
            {
                try
                {
                    File.Delete(testConfiguration);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Tests arranging a single source file with an invalid configuration
        /// </summary>
        [Test]
        public void ArrangeInvalidExtensionAssemblyTest()
        {
            string xml =
                @"<CodeConfiguration xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                    <Handlers>
                        <SourceHandler Assembly='NArrange.BlahBlahBlahBlah, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'>
                            <ProjectExtensions>
                                <Extension Name='csproj'/>
                            </ProjectExtensions>
                            <SourceExtensions>
                                <Extension Name='cs'/>
                            </SourceExtensions>
                        </SourceHandler>
                    </Handlers>
                </CodeConfiguration>";

            string testConfiguration = Path.GetTempFileName();
            File.WriteAllText(testConfiguration, xml);

            try
            {
                TestLogger logger = new TestLogger();

                bool success = Arrange(logger, _testValidSourceFile1, "/c:" + testConfiguration);

                Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
                Assert.IsTrue(logger.HasPartialMessage(LogLevel.Error, "Unable to load configuration file"));
            }
            finally
            {
                try
                {
                    File.Delete(testConfiguration);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Tests arranging a single source file with an invalid extension
        /// </summary>
        [Test]
        public void ArrangeInvalidExtensionTest()
        {
            TestLogger logger = new TestLogger();

            bool success = Arrange(logger, _testInvalidExtensionFile);

            Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
            Assert.IsTrue(logger.HasPartialMessage(LogLevel.Warning, "No assembly is registered to handle file"));
        }

        /// <summary>
        /// Tests arranging a single invalid source file
        /// </summary>
        [Test]
        public void ArrangeInvalidSourceFileTest()
        {
            TestLogger logger = new TestLogger();

            bool success = Arrange(logger, _testInvalidSourceFile);

            Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
            Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "0 files written."));
        }

        /// <summary>
        /// Tests arranging an empty project file
        /// </summary>
        [Test]
        public void ArrangeMonoDevelopEmptyProjectTest()
        {
            TestLogger logger = new TestLogger();

            string emptyProjectFile = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString().Replace('-', '_') + ".mdp");
            File.WriteAllText(emptyProjectFile, "<Project></Project>");

            try
            {
                bool success = Arrange(logger, emptyProjectFile);

                Assert.IsTrue(success, "Expected file to be arranged succesfully.");
                Assert.IsTrue(
                    logger.HasPartialMessage(LogLevel.Warning, "does not contain any supported source files"));
            }
            finally
            {
                try
                {
                    File.Delete(emptyProjectFile);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Tests arranging a project file
        /// </summary>
        [Test]
        public void ArrangeMonoDevelopProjectTest()
        {
            TestLogger logger = new TestLogger();

            bool success = Arrange(logger, _testMonoDevelopProjectFile);

            Assert.IsTrue(success, "Expected file to be arranged succesfully. - {0}", logger.ToString());
            Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "2 files written."), logger.ToString());
        }

        /// <summary>
        /// Tests arranging a solution file
        /// </summary>
        [Test]
        public void ArrangeMonoDevelopSolutionTest()
        {
            TestLogger logger = new TestLogger();

            bool success = Arrange(logger, _testMonoDevelopSolutionFile);

            Assert.IsTrue(success, "Expected file to be arranged succesfully - {0}", logger.ToString());
            Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "2 files written."), logger.ToString());
        }

        /// <summary>
        /// Tests arranging an empty project file
        /// </summary>
        [Test]
        public void ArrangeMSBuildEmptyProjectTest()
        {
            TestLogger logger = new TestLogger();

            string emptyProjectFile = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString().Replace('-', '_') + ".csproj");
            File.WriteAllText(emptyProjectFile, "<Project></Project>");

            try
            {
                bool success = Arrange(logger, emptyProjectFile);

                Assert.IsTrue(success, "Expected file to be arranged succesfully.");
                Assert.IsTrue(
                    logger.HasPartialMessage(LogLevel.Warning, "does not contain any supported source files"));
            }
            finally
            {
                try
                {
                    File.Delete(emptyProjectFile);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Tests arranging a project file
        /// </summary>
        [Test]
        public void ArrangeMSBuildProjectTest()
        {
            TestLogger logger = new TestLogger();

            bool success = Arrange(logger, _testMBBuildProjectFile);

            Assert.IsTrue(success, "Expected file to be arranged succesfully - {0}", logger.ToString());
            Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "2 files written."), logger.ToString());
        }

        /// <summary>
        /// Tests arranging a solution file
        /// </summary>
        [Test]
        public void ArrangeMSBuildSolutionTest()
        {
            TestLogger logger = new TestLogger();

            bool success = Arrange(logger, _testMSBuildSolutionFile);

            Assert.IsTrue(success, "Expected file to be arranged succesfully - {0}", logger.ToString());
            Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "2 files written."), logger.ToString());
        }

        /// <summary>
        /// Tests arranging a single source file with an invalid configuration
        /// </summary>
        [Test]
        public void ArrangeNonExistantConfigurationTest()
        {
            TestLogger logger = new TestLogger();

            bool success = Arrange(logger, _testValidSourceFile1, "/c:blahblahblahblah.xml");

            Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
            Assert.IsTrue(logger.HasPartialMessage(LogLevel.Error, "Unable to load configuration file"));
        }

        /// <summary>
        /// Tests arranging a read-only source file
        /// </summary>
        [Test]
        public void ArrangeReadOnlySourceFileTest()
        {
            TestLogger logger = new TestLogger();

            File.SetAttributes(_testValidSourceFile1, FileAttributes.ReadOnly);

            try
            {
                bool success = Arrange(logger, _testValidSourceFile1);

                Assert.IsFalse(success, "Expected file to not be arranged succesfully.");
                Assert.IsTrue(logger.HasPartialMessage(LogLevel.Warning, "is read-only"));
                Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "0 files written."));
            }
            finally
            {
                File.SetAttributes(_testValidSourceFile1, FileAttributes.Normal);
            }
        }

        /// <summary>
        /// Tests arranging a single source file to an output file
        /// </summary>
        [Test]
        public void ArrangeSingleSourceFileOutputTest()
        {
            string tempFile = Path.GetTempFileName();

            try
            {
                TestLogger logger = new TestLogger();
                string originalText = File.ReadAllText(_testValidSourceFile1);

                bool success = Arrange(logger, _testValidSourceFile1, tempFile);

                string postText = File.ReadAllText(_testValidSourceFile1);

                Assert.AreEqual(originalText, postText, "Original file should not have been modified.");

                Assert.IsTrue(File.Exists(tempFile), "Output file was not created.");
                string arrangedText = File.ReadAllText(tempFile);
                Assert.IsTrue(arrangedText.Length > 0, "Output file is empty.");
                Assert.AreNotEqual(originalText, arrangedText, "File was not arranged.");

                Assert.IsTrue(success, "Expected file to be arranged succesfully.");
                Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "1 files written."));
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        /// <summary>
        /// Tests arranging a single source file and restoring
        /// </summary>
        [Test]
        public void ArrangeSingleSourceFileRestoreTest()
        {
            TestLogger logger = new TestLogger();

            string originalText = File.ReadAllText(_testValidSourceFile1);

            bool success = Arrange(logger, _testValidSourceFile1, "/b");
            Assert.IsTrue(success, "Expected file to be arranged succesfully.");
            Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "1 files written."));

            string arrangedText = File.ReadAllText(_testValidSourceFile1);
            Assert.IsTrue(arrangedText.Length > 0, "File is empty.");
            Assert.AreNotEqual(originalText, arrangedText, "File was not arranged.");

            logger.Clear();
            success = Arrange(logger, _testValidSourceFile1, "/r");
            Assert.IsTrue(success, "Expected file to be restored succesfully.");
            Assert.IsTrue(logger.HasMessage(LogLevel.Info, "Restored"));

            string restoredText = File.ReadAllText(_testValidSourceFile1);
            Assert.IsTrue(restoredText.Length > 0, "File is empty.");
            Assert.AreEqual(originalText, restoredText, "File was not restored.");
        }

        /// <summary>
        /// Tests arranging a single source file
        /// </summary>
        [Test]
        public void ArrangeSingleSourceFileTest()
        {
            TestLogger logger = new TestLogger();

            string originalText = File.ReadAllText(_testValidSourceFile1);

            bool success = Arrange(logger, _testValidSourceFile1);

            Assert.IsTrue(success, "Expected file to be arranged succesfully.");
            Assert.IsTrue(logger.HasMessage(LogLevel.Verbose, "1 files written."));

            string arrangedText = File.ReadAllText(_testValidSourceFile1);
            Assert.IsTrue(arrangedText.Length > 0, "File is empty.");
            Assert.AreNotEqual(originalText, arrangedText, "File was not arranged.");
        }

        /// <summary>
        /// Tests the GetCopyrightText method.
        /// </summary>
        [Test]
        public void GetCopyrightTextTest()
        {
            string copyrightText = Program.GetCopyrightText();
            Assert.IsNotNull(copyrightText, "Copyright text should not be null.");
            Assert.IsTrue(
                copyrightText.Contains("(C) 2007"),
                "Unexpected copyright text.");
        }

        /// <summary>
        /// Tests the GetUsageText method.
        /// </summary>
        [Test]
        public void GetUsageTextTest()
        {
            string usageText = Program.GetUsageText();
            Assert.IsNotNull(usageText, "Usage text should not be null.");
            Assert.IsNotEmpty(usageText, "Usage text should not be empty.");
        }

        /// <summary>
        /// Tests restoring an unknown input
        /// </summary>
        [Test]
        public void RestoreUnknownTest()
        {
            TestLogger logger = new TestLogger();

            bool success = Arrange(logger, Guid.NewGuid().ToString(), "/r");
            Assert.IsFalse(success, "Expected file to not be restored succesfully.");
            Assert.IsTrue(logger.HasMessage(LogLevel.Error, "Restore failed"));
        }

        /// <summary>
        /// Tests Run without args
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunNullArgsTest()
        {
            Program.Run(new TestLogger(), null);
        }

        /// <summary>
        /// Tests Run without a logger
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunNullLoggerTest()
        {
            Program.Run(null, CommandArguments.Parse("Input.cs"));
        }

        /// <summary>
        /// Performs test setup
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string contents = GetTestFileContents("ClassMembers.cs");
            _testValidSourceFile1 = Path.Combine(Path.GetTempPath(), "ClassMembers.cs");
            File.WriteAllText(_testValidSourceFile1, contents);

            contents = GetTestFileContents("ClassMembers.cs");
            _testValidSourceFile2 = Path.Combine(Path.GetTempPath(), "ClassMembers.cs");
            File.WriteAllText(_testValidSourceFile2, contents);

            _testMBBuildProjectFile = Path.Combine(Path.GetTempPath(), "TestProject.csproj");
            MSBuildProjectParserTests.WriteTestProject(_testMBBuildProjectFile);

            _testMSBuildSolutionFile = Path.Combine(Path.GetTempPath(), "TestSolution.sln");
            MSBuildSolutionParserTests.WriteTestSolution(_testMSBuildSolutionFile);

            _testMonoDevelopProjectFile = Path.Combine(Path.GetTempPath(), "TestProject.mdp");
            MonoDevelopProjectParserTests.WriteTestProject(_testMonoDevelopProjectFile);

            _testMonoDevelopSolutionFile = Path.Combine(Path.GetTempPath(), "TestSolution.mds");
            MonoDevelopSolutionParserTests.WriteTestSolution(_testMonoDevelopSolutionFile);

            contents = GetTestFileContents("ClassDefinition.cs");
            _testValidSourceFile2 = Path.Combine(Path.GetTempPath(), "ClassDefinition.cs");
            File.WriteAllText(_testValidSourceFile2, contents);

            _testInvalidSourceFile = Path.GetTempFileName() + ".cs";
            File.WriteAllText(_testInvalidSourceFile, "namespace SampleNamespace\r\n{");

            contents = GetTestFileContents("ClassMembers.cs");
            _testInvalidExtensionFile = Path.GetTempFileName() + ".zzz";
            File.WriteAllText(_testInvalidExtensionFile, contents);
        }

        /// <summary>
        /// Performs test fixture cleanup
        /// </summary>
        [TearDown]
        public void TestTearDown()
        {
            try
            {
                if (_testValidSourceFile1 != null)
                {
                    File.Delete(_testValidSourceFile1);
                    File.Delete(_testInvalidExtensionFile);
                    File.Delete(_testInvalidSourceFile);
                    File.Delete(_testValidSourceFile2);
                    File.Delete(_testMBBuildProjectFile);
                    File.Delete(_testMSBuildSolutionFile);
                    File.Delete(_testMonoDevelopProjectFile);
                    File.Delete(_testMonoDevelopSolutionFile);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets the test file contents.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The test file contents.</returns>
        private static string GetTestFileContents(string fileName)
        {
            string contents = null;

            using (Stream stream = CSharpTestFile.GetTestFileStream(fileName))
            {
                Assert.IsNotNull(stream, "Test stream could not be retrieved.");

                StreamReader reader = new StreamReader(stream);
                contents = reader.ReadToEnd();
            }

            return contents;
        }

        /// <summary>
        /// Arranges using the specified args and logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="args">The command args.</param>
        /// <returns>True if succesful, otherwise false.</returns>
        private bool Arrange(TestLogger logger, params string[] args)
        {
            CommandArguments commandArgs = CommandArguments.Parse(args);
            return Program.Run(logger, commandArgs);
        }

        #endregion Methods
    }
}