namespace NArrange.Tests.Core
{
    using System;
    using System.IO;

    using NArrange.Core;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the BackupUtilities class.
    /// </summary>
    [TestFixture]
    public class BackupUtilitiesTests
    {
        #region Methods

        /// <summary>
        /// Tests the Backup and Restore methods by creating a backup, modifying files
        /// and restoring.
        /// </summary>
        [Test]
        public void BackupAndRestoreTest()
        {
            string backupRoot = BackupUtilities.CreateTempFilePath();
            string sourceFolder = BackupUtilities.CreateTempFilePath();
            string destinationFolder = BackupUtilities.CreateTempFilePath();

            try
            {
                Directory.CreateDirectory(sourceFolder);

                string file1Text = "This is test file 1.";
                string file2Text = "This is test file 2.";
                string file3Text = "This is test file 3.";

                string file1 = Path.Combine(sourceFolder, "File1.txt");
                string file2Directory = Path.Combine(sourceFolder, "a");
                string file2 = Path.Combine(file2Directory, "File2.txt");
                string file3Directory = Path.Combine(sourceFolder, "b");
                string file3 = Path.Combine(file3Directory, "File3.txt");

                File.WriteAllText(file1, file1Text);
                Directory.CreateDirectory(file2Directory);
                File.WriteAllText(file2, file2Text);
                Directory.CreateDirectory(file3Directory);
                File.WriteAllText(file3, file3Text);

                string key = BackupUtilities.CreateFileNameKey("Test");
                string backupLocation = BackupUtilities.BackupFiles(
                    backupRoot,
                    key,
                    new string[] { file1, file2, file3 });

                string zipFile = Path.Combine(backupLocation, "files.zip");
                Assert.IsTrue(File.Exists(zipFile), "Expected zip file to exist after backup.");
                TestUtilities.AssertNotEmpty(zipFile);

                //
                // Modify the original files
                //
                File.WriteAllText(file1, "Blah");
                File.WriteAllText(file2, "Blah");
                File.Delete(file3);

                BackupUtilities.RestoreFiles(backupRoot, key);

                Assert.IsTrue(File.Exists(file1), "Restored file was not found.");
                Assert.AreEqual(file1Text, File.ReadAllText(file1), "Unexpected file contents.");
                Assert.IsTrue(File.Exists(file2), "Restored file was not found.");
                Assert.AreEqual(file2Text, File.ReadAllText(file2), "Unexpected file contents.");
                Assert.IsTrue(File.Exists(file3), "Restored file was not found.");
                Assert.AreEqual(file3Text, File.ReadAllText(file3), "Unexpected file contents.");
            }
            finally
            {
                try
                {
                    Directory.Delete(sourceFolder, true);
                    Directory.Delete(destinationFolder, true);
                    Directory.Delete(backupRoot, true);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Tests the BackupFiles method with a null backup root directory.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupFilesNullBackupRootTest()
        {
            BackupUtilities.BackupFiles(null, "123456789", new string[] { "c:\test\test.cs" });
        }

        /// <summary>
        /// Tests the BackupFiles method with a null key.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BackupFilesNullKeyTest()
        {
            BackupUtilities.BackupFiles("c:\temp", null, new string[] { "c:\test\test.cs" });
        }

        /// <summary>
        /// Tests the GetFileNameKey method with an empty string.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFileNameKeyEmptyTest()
        {
            string key = BackupUtilities.CreateFileNameKey(string.Empty);
        }

        /// <summary>
        /// Tests the GetFileNameKey method with null.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFileNameKeyNullTest()
        {
            string key = BackupUtilities.CreateFileNameKey(null);
        }

        /// <summary>
        /// Tests the GetFileNameKey method.
        /// </summary>
        [Test]
        public void GetFileNameKeyTest()
        {
            const string TestFileName1 = @"c:\temp\This Is Some Folder\This Is Some File.cs";
            const string TestFileName2 = @"c:\temp\this is some folder\this is some file.cs";
            const string TestFileName3 = @"e:\temp\this is some folder\this is some file.cs";
            const string TestKey = "_1132715649";

            //
            // The same key should be returned accross multiple runs.
            //
            string key1 = BackupUtilities.CreateFileNameKey(TestFileName1);
            Assert.AreEqual(TestKey, key1);

            //
            // Case should be ignored
            //
            string key2 = BackupUtilities.CreateFileNameKey(TestFileName2);
            Assert.AreEqual(TestKey, key2);

            //
            // Different files should produce different keys
            //
            string key3 = BackupUtilities.CreateFileNameKey(TestFileName3);
            Assert.IsNotNull(key3, "Key should not be null.");
            Assert.IsNotEmpty(key3, "Key should not be empty.");
            Assert.AreNotEqual(key2, key3, "Keys should be unique per file.");
        }

        /// <summary>
        /// Tests the RestoreFiles method with a null backup root directory.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RestoreFilesNullBackupRootTest()
        {
            BackupUtilities.RestoreFiles(null, "123456789");
        }

        /// <summary>
        /// Tests the RestoreFiles method with a null key.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RestoreFilesNullKeyTest()
        {
            BackupUtilities.RestoreFiles("c:\temp", null);
        }

        #endregion Methods
    }
}