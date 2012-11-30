namespace NArrange.Tests.ConsoleApplication
{
    using System;

    using NArrange.ConsoleApplication;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the CommandArguments class.
    /// </summary>
    [TestFixture]
    public class CommandArgumentsTests
    {
        #region Methods

        /// <summary>
        /// Tests parsing an empty string arg
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseEmptyArgTest()
        {
            CommandArguments.Parse("Input.cs", string.Empty);
        }

        /// <summary>
        /// Tests parsing an empty string[]
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseEmptyArrayTest()
        {
            CommandArguments.Parse(new string[] { });
        }

        /// <summary>
        /// Tests parsing an input file with an invalid backup flag
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInputBackupInvalidTest()
        {
            CommandArguments.Parse("Input.cs", "/bakup");
        }

        /// <summary>
        /// Tests parsing an input file with backup and restore both specified
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInputBackupRestoreTest()
        {
            CommandArguments.Parse("Input.cs", "/b", "/r");
        }

        /// <summary>
        /// Tests parsing an input file with backup specified
        /// </summary>
        [Test]
        public void ParseInputBackupTest()
        {
            string[][] argsList =
                {
                    new string[] { "Input.cs", "/b" },
                    new string[] { "Input.cs", "-b" },
                    new string[] { "Input.cs", "/B" },
                    new string[] { "Input.cs", "/backup" },
                    new string[] { "Input.cs", "/Backup" },
                    new string[] { "Input.cs", "/BACKUP" },
                    new string[] { "Input.cs", "-BACKUP" }
                };

            foreach (string[] args in argsList)
            {
                CommandArguments commandArgs = CommandArguments.Parse(args);

                Assert.IsNull(commandArgs.Configuration, "Unexpected value for Configuration");
                Assert.AreEqual("Input.cs", commandArgs.Input, "Unexpected value for Input");
                Assert.IsNull(commandArgs.Output, "Unexpected value for Output");
                Assert.IsTrue(commandArgs.Backup, "Unexpected value for Backup");
                Assert.IsFalse(commandArgs.Restore, "Unexpected value for Restore");
                Assert.IsFalse(commandArgs.Trace, "Unexpected value for Trace");
            }
        }

        /// <summary>
        /// Tests parsing an input file with an empty flag
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInputConfigurationEmptyFlagTest()
        {
            CommandArguments.Parse("Input.cs", "/");
        }

        /// <summary>
        /// Tests parsing an input file with an invalid configuration file
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInputConfigurationFileEmptyTest()
        {
            CommandArguments.Parse("Input.cs", "/c:");
        }

        /// <summary>
        /// Tests parsing an input file with an invalid configuration file
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInputConfigurationFileNotSpecifiedTest()
        {
            CommandArguments.Parse("Input.cs", "/configuration");
        }

        /// <summary>
        /// Tests parsing an input file with an invalid configuration flag
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInputConfigurationInvalidTest()
        {
            CommandArguments.Parse("Input.cs", "/confguration");
        }

        /// <summary>
        /// Tests parsing an input file with a configuration specified
        /// </summary>
        [Test]
        public void ParseInputConfigurationTest()
        {
            string[][] argsList =
                {
                    new string[] { "Input.cs", "/c:c:\\temp\\MyConfig.xml" },
                    new string[] { "Input.cs", "/C:c:\\temp\\MyConfig.xml" },
                    new string[] { "Input.cs", "/configuration:c:\\temp\\MyConfig.xml" },
                    new string[] { "Input.cs", "/Configuration:c:\\temp\\MyConfig.xml" },
                    new string[] { "Input.cs", "/CONFIGURATION:c:\\temp\\MyConfig.xml" }
                };

            foreach (string[] args in argsList)
            {
                CommandArguments commandArgs = CommandArguments.Parse(args);

                Assert.AreEqual(
                    "c:\\temp\\MyConfig.xml",
                    commandArgs.Configuration,
                    "Unexpected value for Configuration");
                Assert.AreEqual("Input.cs", commandArgs.Input, "Unexpected value for Input");
                Assert.IsNull(commandArgs.Output, "Unexpected value for Output");
                Assert.IsFalse(commandArgs.Backup, "Unexpected value for Backup");
                Assert.IsFalse(commandArgs.Restore, "Unexpected value for Restore");
                Assert.IsFalse(commandArgs.Trace, "Unexpected value for Trace");
            }
        }

        /// <summary>
        /// Tests parsing an input and output file with backup specified.  This is invalid.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInputOutputBackupTest()
        {
            CommandArguments.Parse("Input.cs", "Output.cs", "/b");
        }

        /// <summary>
        /// Tests parsing an input and output file with an extraneous argument.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInputOutputExtraneousTest()
        {
            CommandArguments.Parse("Input.cs", "Output.cs", "Extraneous.cs");
        }

        /// <summary>
        /// Tests parsing an input and output file with restore specified.  This is invalid.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInputOutputRestoreTest()
        {
            CommandArguments.Parse("Input.cs", "Output.cs", "/r");
        }

        /// <summary>
        /// Tests parsing an input and output file
        /// </summary>
        [Test]
        public void ParseInputOutputTest()
        {
            CommandArguments commandArgs = CommandArguments.Parse("Input.cs", "Output.cs");

            Assert.IsNull(commandArgs.Configuration, "Unexpected value for Configuration");
            Assert.AreEqual("Input.cs", commandArgs.Input, "Unexpected value for Input");
            Assert.AreEqual("Output.cs", commandArgs.Output, "Unexpected value for Output");
            Assert.IsFalse(commandArgs.Backup, "Unexpected value for Backup");
            Assert.IsFalse(commandArgs.Restore, "Unexpected value for Restore");
            Assert.IsFalse(commandArgs.Trace, "Unexpected value for Trace");
        }

        /// <summary>
        /// Tests parsing an input file with an invalid restore flag
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInputRestoreInvalidTest()
        {
            CommandArguments.Parse("Input.cs", "/resore");
        }

        /// <summary>
        /// Tests parsing an input file with restore specified
        /// </summary>
        [Test]
        public void ParseInputRestoreTest()
        {
            string[][] argsList =
                {
                    new string[] { "Input.cs", "/r" },
                    new string[] { "Input.cs", "-r" },
                    new string[] { "Input.cs", "/R" },
                    new string[] { "Input.cs", "/restore" },
                    new string[] { "Input.cs", "/Restore" },
                    new string[] { "Input.cs", "/RESTORE" },
                    new string[] { "Input.cs", "-RESTORE" }
                };

            foreach (string[] args in argsList)
            {
                CommandArguments commandArgs = CommandArguments.Parse(args);

                Assert.IsNull(commandArgs.Configuration, "Unexpected value for Configuration");
                Assert.AreEqual("Input.cs", commandArgs.Input, "Unexpected value for Input");
                Assert.IsNull(commandArgs.Output, "Unexpected value for Output");
                Assert.IsFalse(commandArgs.Backup, "Unexpected value for Backup");
                Assert.IsTrue(commandArgs.Restore, "Unexpected value for Restore");
                Assert.IsFalse(commandArgs.Trace, "Unexpected value for Trace");
            }
        }

        /// <summary>
        /// Tests parsing just an input file
        /// </summary>
        [Test]
        public void ParseInputTest()
        {
            CommandArguments commandArgs = CommandArguments.Parse("Input.cs");

            Assert.IsNull(commandArgs.Configuration, "Unexpected value for Configuration");
            Assert.AreEqual("Input.cs", commandArgs.Input, "Unexpected value for Input");
            Assert.IsNull(commandArgs.Output, "Unexpected value for Output");
            Assert.IsFalse(commandArgs.Backup, "Unexpected value for Backup");
            Assert.IsFalse(commandArgs.Restore, "Unexpected value for Restore");
            Assert.IsFalse(commandArgs.Trace, "Unexpected value for Trace");
        }

        /// <summary>
        /// Tests parsing an input file with an invalid trace flag
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInputTraceInvalidTest()
        {
            CommandArguments.Parse("Input.cs", "/trce");
        }

        /// <summary>
        /// Tests parsing an input file with trace specified
        /// </summary>
        [Test]
        public void ParseInputTraceTest()
        {
            string[][] argsList =
                {
                    new string[] { "Input.cs", "/t" },
                    new string[] { "Input.cs", "/T" },
                    new string[] { "Input.cs", "/trace" },
                    new string[] { "Input.cs", "/Trace" },
                    new string[] { "Input.cs", "/TRACE" }
                };

            foreach (string[] args in argsList)
            {
                CommandArguments commandArgs = CommandArguments.Parse(args);

                Assert.IsNull(commandArgs.Configuration, "Unexpected value for Configuration");
                Assert.AreEqual("Input.cs", commandArgs.Input, "Unexpected value for Input");
                Assert.IsNull(commandArgs.Output, "Unexpected value for Output");
                Assert.IsFalse(commandArgs.Backup, "Unexpected value for Backup");
                Assert.IsFalse(commandArgs.Restore, "Unexpected value for Restore");
                Assert.IsTrue(commandArgs.Trace, "Unexpected value for Trace");
            }
        }

        /// <summary>
        /// Tests parsing with multiple flags
        /// </summary>
        [Test]
        public void ParseMultipleTest()
        {
            CommandArguments commandArgs = CommandArguments.Parse(
                "Input.cs", "/b", "/CONFIGURATION:c:\\temp\\MyConfig.xml", "/t");

            Assert.AreEqual(
                "c:\\temp\\MyConfig.xml",
                commandArgs.Configuration,
                "Unexpected value for Configuration");
            Assert.AreEqual("Input.cs", commandArgs.Input, "Unexpected value for Input");
            Assert.IsNull(commandArgs.Output, "Unexpected value for Output");
            Assert.IsTrue(commandArgs.Backup, "Unexpected value for Backup");
            Assert.IsFalse(commandArgs.Restore, "Unexpected value for Restore");
            Assert.IsTrue(commandArgs.Trace, "Unexpected value for Trace");
        }

        /// <summary>
        /// Tests parsing a null string arg
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseNullArgTest()
        {
            CommandArguments.Parse("Input.cs", null);
        }

        /// <summary>
        /// Tests parsing a null string[]
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseNullArrayTest()
        {
            CommandArguments.Parse(null);
        }

        /// <summary>
        /// Tests parsing an input file with an unknown flag
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseUnknownFlagTest()
        {
            CommandArguments.Parse("Input.cs", "/z");
        }

        #endregion Methods
    }
}