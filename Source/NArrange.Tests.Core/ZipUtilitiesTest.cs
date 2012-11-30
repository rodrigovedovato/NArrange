namespace NArrange.Tests.Core
{
    using System.IO;

    using NArrange.Core;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ZipUtilities class.
    /// </summary>
    [TestFixture]
    public class ZipUtilitiesTest
    {
        #region Methods

        /// <summary>
        /// Tests zipping and unzipping files.
        /// </summary>
        [Test]
        public void ZipAndUnzipTest()
        {
            string sourceFolder = BackupUtilities.CreateTempFilePath();
            string destinationFolder = BackupUtilities.CreateTempFilePath();
            string zipFile = BackupUtilities.CreateTempFilePath() + ".zip";

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

                ZipUtilities.Zip(sourceFolder, zipFile);

                Assert.IsTrue(File.Exists(zipFile), "Expected zip file to exist.");
                TestUtilities.AssertNotEmpty(zipFile);

                ZipUtilities.Unzip(zipFile, destinationFolder);

                FileSystemInfo[] fileSystemInfos =
                    new DirectoryInfo(destinationFolder).GetFileSystemInfos();
                Assert.IsTrue(fileSystemInfos.Length > 0, "Unzip did not create any files in the destination directory.");

                file1 = Path.Combine(destinationFolder, "File1.txt");
                file2Directory = Path.Combine(destinationFolder, "a");
                file2 = Path.Combine(file2Directory, "File2.txt");
                file3Directory = Path.Combine(destinationFolder, "b");
                file3 = Path.Combine(file3Directory, "File3.txt");

                Assert.IsTrue(File.Exists(file1), "Unzipped file was not found.");
                Assert.AreEqual(file1Text, File.ReadAllText(file1), "Unexpected file contents.");
                Assert.IsTrue(File.Exists(file2), "Unzipped file was not found.");
                Assert.AreEqual(file2Text, File.ReadAllText(file2), "Unexpected file contents.");
                Assert.IsTrue(File.Exists(file3), "Unzipped file was not found.");
                Assert.AreEqual(file3Text, File.ReadAllText(file3), "Unexpected file contents.");
            }
            finally
            {
                try
                {
                    Directory.Delete(sourceFolder, true);
                    Directory.Delete(destinationFolder, true);
                    File.Delete(zipFile);
                }
                catch
                {
                }
            }
        }

        #endregion Methods
    }
}