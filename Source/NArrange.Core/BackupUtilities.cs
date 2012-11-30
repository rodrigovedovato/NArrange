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
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Class for backing up and restoring source files.  Utilizes zip archives.
    /// </summary>
    public static class BackupUtilities
    {
        #region Fields

        /// <summary>
        /// Default backup root directory.
        /// </summary>
        public static readonly string BackupRoot = Path.Combine(
            Path.GetTempPath(), "NArrange");

        /// <summary>
        /// Backup index file name.
        /// </summary>
        private const string IndexFileName = "index.txt";

        /// <summary>
        /// Index file token separator.
        /// </summary>
        private const char IndexSeparator = ',';

        /// <summary>
        /// Backup zip file name.
        /// </summary>
        private const string ZipFileName = "files.zip";

        /// <summary>
        /// Max string length for an integer.
        /// </summary>
        private static readonly int MaxIntLength = 
            int.MinValue.ToString(CultureInfo.InvariantCulture).Length;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Creates a backup of the specified files using the specified key.
        /// </summary>
        /// <param name="backupRoot">Backup root directory.</param>
        /// <param name="key">Backup key.</param>
        /// <param name="fileNames">Files to backup.</param>
        /// <returns>Backup location</returns>
        public static string BackupFiles(
            string backupRoot,
            string key,
            IEnumerable<string> fileNames)
        {
            if (backupRoot == null || backupRoot.Trim().Length == 0)
            {
                throw new ArgumentException("Invalid backup location", "backupRoot");
            }
            else if (key == null || key.Trim().Length == 0)
            {
                throw new ArgumentException("Invalid backup key", "key");
            }

            if (!Directory.Exists(backupRoot))
            {
                Directory.CreateDirectory(backupRoot);
            }

            DateTime backupDate = DateTime.Now;
            string dateDirectory = backupDate.ToFileTime().ToString(CultureInfo.InvariantCulture);
            string keyRoot = Path.Combine(backupRoot, key);
            if (!Directory.Exists(keyRoot))
            {
                Directory.CreateDirectory(keyRoot);
            }

            string backupLocation = Path.Combine(keyRoot, dateDirectory);
            Directory.CreateDirectory(backupLocation);

            string zipFile = Path.Combine(backupLocation, ZipFileName);

            // Copy all files to a temporary working directory
            string workingDirectory = CreateTempFilePath();
            Directory.CreateDirectory(workingDirectory);
            try
            {
                string indexFile = Path.Combine(backupLocation, IndexFileName);
                using (FileStream fs = new FileStream(indexFile, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        foreach (string fileName in fileNames)
                        {
                            string fileKey = CreateFileNameKey(fileName);
                            string fileBackupName = fileKey + "." + ProjectManager.GetExtension(fileName);
                            writer.WriteLine(fileBackupName + IndexSeparator + fileName);

                            string fileBackupPath =
                                Path.Combine(workingDirectory, fileBackupName);
                            File.Copy(fileName, fileBackupPath);
                        }
                    }
                }

                // Zip up all files to backup
                ZipUtilities.Zip(workingDirectory, zipFile);
            }
            finally
            {
                TryDeleteDirectory(workingDirectory);
            }

            return backupLocation;
        }

        /// <summary>
        /// Creates a system unique key for the specified fileName.
        /// </summary>
        /// <param name="fileName">File name or path.</param>
        /// <returns>Unique key for the file name (hash).</returns>
        public static string CreateFileNameKey(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            else if (fileName.Trim().Length == 0)
            {
                throw new ArgumentException("Invalid fileName", "fileName");
            }

            return fileName.ToUpperInvariant().GetHashCode()
                .ToString(CultureInfo.InvariantCulture)
                .Replace('-', '_').PadLeft(MaxIntLength, '_');
        }

        /// <summary>
        /// Gets a new temporary file path for use as a directory or filename.
        /// </summary>
        /// <returns>Temp file path.</returns>
        public static string CreateTempFilePath()
        {
            string fileName = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString().Replace('-', '_'));
            return fileName;
        }

        /// <summary>
        /// Restores all files associated with the specified key.
        /// </summary>
        /// <param name="backupRoot">Backup root directory.</param>
        /// <param name="key">Backup key.</param>
        /// <returns>A boolean value indicating whether or not the operation was succesful.</returns>
        public static bool RestoreFiles(string backupRoot, string key)
        {
            bool success = false;

            if (backupRoot == null || backupRoot.Trim().Length == 0)
            {
                throw new ArgumentException("Invalid backup location", "backupRoot");
            }
            else if (key == null || key.Trim().Length == 0)
            {
                throw new ArgumentException("Invalid backup key", "key");
            }

            string keyRoot = Path.Combine(backupRoot, key);

            // Find the most recent timestamped folder
            DirectoryInfo keyDirectory = new DirectoryInfo(keyRoot);
            SortedList<string, DirectoryInfo> timestampedDirectories =
                new SortedList<string, DirectoryInfo>();
            DirectoryInfo[] childDirectories = keyDirectory.GetDirectories();
            foreach (DirectoryInfo childDirectory in childDirectories)
            {
                timestampedDirectories.Add(childDirectory.Name, childDirectory);
            }

            if (timestampedDirectories.Count > 0)
            {
                string dateDirectory = timestampedDirectories.Values[timestampedDirectories.Count - 1].Name;

                string backupLocation = Path.Combine(keyRoot, dateDirectory);
                string zipFile = Path.Combine(backupLocation, ZipFileName);

                // Extract all files to a temporary working directory
                string workingDirectory = CreateTempFilePath();
                Directory.CreateDirectory(workingDirectory);
                try
                {
                    // Unzip and copy all files to restore
                    ZipUtilities.Unzip(zipFile, workingDirectory);

                    string indexFile = Path.Combine(backupLocation, IndexFileName);
                    using (FileStream fs = new FileStream(indexFile, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(fs))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                int separatorIndex = line.IndexOf(IndexSeparator);
                                string fileBackupName = line.Substring(0, separatorIndex);
                                string fileBackupPath =
                                        Path.Combine(workingDirectory, fileBackupName);

                                string restorePath = line.Substring(separatorIndex + 1);

                                string backupText = File.ReadAllText(fileBackupPath);
                                string restoreText = null;
                                if (File.Exists(restorePath))
                                {
                                    restoreText = File.ReadAllText(restorePath);
                                    File.SetAttributes(restorePath, FileAttributes.Normal);
                                }

                                if (backupText != restoreText)
                                {
                                    File.Copy(fileBackupPath, restorePath, true);
                                }
                            }
                        }
                    }

                    // Remove the restored backup so that a consecutive restore
                    // will process the next in the history
                    Directory.Delete(backupLocation, true);

                    success = true;
                }
                finally
                {
                    TryDeleteDirectory(workingDirectory);
                }
            }

            return success;
        }

        /// <summary>
        /// Attempts to delete a directory, catching any exceptions.
        /// </summary>
        /// <param name="workingDirectory">Working directory path.</param>
        /// <returns>Returns a value indicating whether or not the directory was deleted.</returns>
        private static bool TryDeleteDirectory(string workingDirectory)
        {
            try
            {
                Directory.Delete(workingDirectory, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion Methods
    }
}