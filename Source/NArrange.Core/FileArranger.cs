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
 *<contributor>Justin Dearing</contributor>
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Core
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.IO;
	using System.Reflection;
	using System.Text;

	using NArrange.Core.CodeElements;
	using NArrange.Core.Configuration;

	/// <summary>
	/// Class for arranging source code files.
	/// </summary>
	public sealed class FileArranger
	{
		#region Fields

		/// <summary>
		/// Configuration file name supplied.
		/// </summary>
		private readonly string _configFile;

		/// <summary>
		/// Logger to write messages to.
		/// </summary>
		private readonly ILogger _logger;

		/// <summary>
		/// Arrange results.
		/// </summary>
		private Dictionary<string, ArrangeResult> _arrangeResults;

		/// <summary>
		/// The code arranger for arranging source files.
		/// </summary>
		private CodeArranger _codeArranger;

		/// <summary>
		/// Code configuration.
		/// </summary>
		private CodeConfiguration _configuration;

		/// <summary>
		/// Default file encoding.
		/// </summary>
		private Encoding _encoding;

		/// <summary>
		/// Number of files parsed.
		/// </summary>
		private int _filesParsed;

		/// <summary>
		/// Number of files written.
		/// </summary>
		private int _filesWritten;

		/// <summary>
		/// Project manager for retrieving solution/project/directory source files
		/// and the parsers associated with those files.
		/// </summary>
		private ProjectManager _projectManager;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new file arranger.
		/// </summary>
		/// <param name="configFile">The config file.</param>
		/// <param name="logger">The logger.</param>
		public FileArranger(string configFile, ILogger logger)
		{
			_configFile = configFile;
			_logger = logger;
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Arranges a file, project or solution.
		/// </summary>
		/// <param name="inputFile">The input file.</param>
		/// <param name="outputFile">The output file.</param>
		/// <returns>True if succesful, otherwise false.</returns>
		public bool Arrange(string inputFile, string outputFile)
		{
			return Arrange(inputFile, outputFile, false);
		}

		/// <summary>
		/// Arranges a file, project or solution.
		/// </summary>
		/// <param name="inputFile">The input file.</param>
		/// <param name="outputFile">The output file.</param>
		/// <param name="backup">Whether or not to create a backup.</param>
		/// <returns>True if succesful, otherwise false.</returns>
		public bool Arrange(string inputFile, string outputFile, bool backup)
		{
			bool success = true;

			success = Initialize();

			if (success)
			{
				bool isProject = _projectManager.IsProject(inputFile);
				bool isSolution = !isProject && ProjectManager.IsSolution(inputFile);
				bool isDirectory = !isProject && !isSolution &&
					string.IsNullOrEmpty(Path.GetExtension(inputFile)) && Directory.Exists(inputFile);

				if (!(isProject || isSolution || isDirectory))
				{
					// Necessary when the user is arranging a file that was opened using the F7 shortcut

					if (File.Exists(String.Concat(inputFile, ".cs")))
					{
						inputFile = String.Concat(inputFile, ".cs");
					}
					else if (File.Exists(String.Concat(inputFile, ".vb")))
					{
						inputFile = String.Concat(inputFile, ".vb");
					}

					if (outputFile == null)
					{
						outputFile = new FileInfo(inputFile).FullName;
					}

					bool canParse = _projectManager.CanParse(inputFile);
					if (!canParse)
					{
						LogMessage(
							LogLevel.Warning,
							"No assembly is registered to handle file {0}.  Please update the configuration or select a valid file.",
							inputFile);
						success = false;
					}
				}

				if (success)
				{
					ReadOnlyCollection<string> sourceFiles = _projectManager.GetSourceFiles(inputFile);
					if (sourceFiles.Count > 0)
					{
						LogMessage(LogLevel.Verbose, "Parsing files...");

						foreach (string sourceFile in sourceFiles)
						{
							if (string.IsNullOrEmpty(outputFile))
							{
								ArrangeSourceFile(sourceFile, sourceFile);
							}
							else
							{
								ArrangeSourceFile(sourceFile, outputFile);
							}
						}

						if (success && _arrangeResults.Count > 0)
						{
							success = WriteFiles(inputFile, backup);
						}
					}
					else
					{
						if (isSolution)
						{
							LogMessage(
								LogLevel.Warning,
								"Solution {0} does not contain any supported source files.",
								inputFile);
						}
						else if (isProject)
						{
							LogMessage(
								LogLevel.Warning,
								"Project {0} does not contain any supported source files.",
							   inputFile);
						}
						else
						{
							LogMessage(
								LogLevel.Warning,
								"Directory {0} does not contain any supported source files.",
							   inputFile);
						}
					}

					if (_filesParsed == 0 && (sourceFiles.Count <= 1 && !(isProject || isSolution || isDirectory)))
					{
						success = false;
					}
				}
			}

			LogMessage(LogLevel.Verbose, "{0} files written.", _filesWritten);

			return success;
		}

		/// <summary>
		/// Gets a configuration load error.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <param name="ex">The exception.</param>
		/// <returns>The configuration load error text.</returns>
		private static string GetConfigurationLoadError(string filename, Exception ex)
		{
			StringBuilder messageBuilder = new StringBuilder(
				string.Format(
				CultureInfo.CurrentUICulture,
				"Unable to load configuration file {0}: {1}",
				filename,
				ex.Message));

			if (ex.InnerException != null)
			{
				messageBuilder.AppendFormat(" {0}", ex.InnerException.Message);
			}

			return messageBuilder.ToString();
		}

		/// <summary>
		/// Arranges code elements.
		/// </summary>
		/// <param name="elements">The elements.</param>
		/// <returns>Collection of arranged elements.</returns>
		private ReadOnlyCollection<ICodeElement> ArrangeElements(ReadOnlyCollection<ICodeElement> elements)
		{
			if (_codeArranger == null)
			{
				_codeArranger = new CodeArranger(_configuration);
			}

			elements = _codeArranger.Arrange(elements);

			return elements;
		}

		/// <summary>
		/// Arranges an individual source file.
		/// </summary>
		/// <param name="inputFile">The input file.</param>
		/// <param name="outputFile">The output file.</param>
		private void ArrangeSourceFile(string inputFile, string outputFile)
		{
			ReadOnlyCollection<ICodeElement> elements = null;
			string inputFileText = null;

			Encoding encoding = _encoding;

			try
			{
				FileAttributes fileAttributes = File.GetAttributes(inputFile);
				if (inputFile == outputFile &&
					((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly))
				{
					LogMessage(LogLevel.Warning, "File {0} is read-only", inputFile);
				}
				else
				{
					if (encoding == null)
					{
						encoding = FileUtilities.GetEncoding(inputFile);
					}

					inputFileText = File.ReadAllText(inputFile, encoding);
					elements = _projectManager.ParseElements(inputFile, inputFileText);
					LogMessage(LogLevel.Trace, "Parsed {0}", inputFile);
				}
			}
			catch (DirectoryNotFoundException)
			{
				LogMessage(LogLevel.Warning, "File {0} does not exist.", inputFile);
			}
			catch (FileNotFoundException)
			{
				LogMessage(LogLevel.Warning, "File {0} does not exist.", inputFile);
			}
			catch (IOException ioException)
			{
				LogMessage(
					LogLevel.Warning,
					"Unable to read file {0}: {1}",
					inputFile,
					ioException.ToString());
			}
			catch (UnauthorizedAccessException ioException)
			{
				LogMessage(
					LogLevel.Warning,
					"Unable to read file {0}: {1}",
					inputFile,
					ioException.Message);
			}
			catch (ParseException parseException)
			{
				LogMessage(
					LogLevel.Warning,
					"Unable to parse file {0}: {1}",
					inputFile,
					parseException.Message);
			}
			catch (Exception parseException)
			{
				LogMessage(
					LogLevel.Warning,
					"Unable to parse file {0}: {1}",
					inputFile,
					parseException.Message);
			}

			if (elements != null)
			{
				try
				{
					elements = ArrangeElements(elements);
				}
				catch (InvalidOperationException invalidEx)
				{
					LogMessage(
						LogLevel.Warning,
						"Unable to arrange file {0}: {1}",
					   inputFile,
					   invalidEx.ToString());

					elements = null;
				}
			}

			string outputFileText = null;
			if (elements != null)
			{
				ICodeElementWriter codeWriter = _projectManager.GetSourceHandler(inputFile).CodeWriter;
				codeWriter.Configuration = _configuration;

				StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
				try
				{
					codeWriter.Write(elements, writer);
				}
				catch (Exception ex)
				{
					LogMessage(LogLevel.Error, ex.ToString());
					throw;
				}

				outputFileText = writer.ToString();
			}

			if (outputFileText != null)
			{
				//
				// Store the arranged elements so that we can create a backup before writing
				//
				_arrangeResults.Add(
					outputFile,
					new ArrangeResult(encoding, inputFile, inputFileText, outputFile, outputFileText));
			}
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <returns>True if succesful, otherwise false.</returns>
		private bool Initialize()
		{
			bool success = true;

			_filesParsed = 0;
			_filesWritten = 0;
			_arrangeResults = new Dictionary<string, ArrangeResult>();

			try
			{
				LoadConfiguration(_configFile);
			}
			catch (InvalidOperationException xmlException)
			{
				string message = GetConfigurationLoadError(_configFile, xmlException);
				LogMessage(LogLevel.Error, message);
				success = false;
			}
			catch (IOException ioException)
			{
				string message = GetConfigurationLoadError(_configFile, ioException);
				LogMessage(LogLevel.Error, message);
				success = false;
			}
			catch (UnauthorizedAccessException authException)
			{
				string message = GetConfigurationLoadError(_configFile, authException);
				LogMessage(LogLevel.Error, message);
				success = false;
			}
			catch (TargetInvocationException invEx)
			{
				LogMessage(
					LogLevel.Error,
					"Unable to load extension assembly from configuration file {0}: {1}",
					_configFile,
					invEx.Message);
				success = false;
			}

			return success;
		}

		/// <summary>
		/// Loads the configuration file that specifies how elements will be arranged.
		/// </summary>
		/// <param name="configFile">The config file.</param>
		private void LoadConfiguration(string configFile)
		{
			if (_configuration == null)
			{
				if (configFile != null)
				{
					_configuration = CodeConfiguration.Load(configFile);
				}
				else
				{
					_configuration = CodeConfiguration.Default;
				}

				_projectManager = new ProjectManager(_configuration);
				_encoding = _configuration.Encoding.GetEncoding();
			}
		}

		/// <summary>
		/// Log a message.
		/// </summary>
		/// <param name="level">The level.</param>
		/// <param name="message">The message or message format.</param>
		/// <param name="args">The message format args.</param>
		private void LogMessage(LogLevel level, string message, params object[] args)
		{
			if (_logger != null)
			{
				_logger.LogMessage(level, message, args);
			}
		}

		/// <summary>
		/// Writes a source file.
		/// </summary>
		/// <param name="arrangeResult">The arrange result.</param>
		/// <returns>True if succesful, otherwise false.</returns>
		private bool WriteFile(ArrangeResult arrangeResult)
		{
			bool success = true;

			try
			{
				File.WriteAllText(
					arrangeResult.OutputFile,
					arrangeResult.OutputFileText,
					arrangeResult.Encoding);

				LogMessage(LogLevel.Trace, "Wrote {0}", arrangeResult.OutputFile);
			}
			catch (IOException ioEx)
			{
				LogMessage(
					LogLevel.Warning,
					"Unable to write file {0}: {1}",
					arrangeResult.OutputFile,
					ioEx.Message);
				success = false;
			}
			catch (UnauthorizedAccessException ioEx)
			{
				LogMessage(
					LogLevel.Warning,
					"Unable to write file {0}: {1}",
					arrangeResult.OutputFile,
					ioEx.Message);
				success = false;
			}

			if (success)
			{
				_filesWritten++;
			}

			return success;
		}

		/// <summary>
		/// Writes the files.
		/// </summary>
		/// <param name="inputFile">The input file.</param>
		/// <param name="backup">Whether or not a backup should be written.</param>
		/// <returns>True if succesful, otherwise false.</returns>
		private bool WriteFiles(string inputFile, bool backup)
		{
			bool success = true;

			List<string> filesToModify = new List<string>();
			_filesParsed = _arrangeResults.Count;
			LogMessage(LogLevel.Verbose, "{0} files parsed.", _filesParsed);

			Dictionary<string, ArrangeResult>.Enumerator enumerator = _arrangeResults.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ArrangeResult fileResult = enumerator.Current.Value;
				if (fileResult.Modified)
				{
					filesToModify.Add(fileResult.OutputFile);
				}
				else
				{
					LogMessage(LogLevel.Trace, "File {0} will not be modified", fileResult.OutputFile);
				}
			}

			if (backup && filesToModify.Count > 0)
			{
				try
				{
					LogMessage(LogLevel.Verbose, "Creating backup for {0}...", inputFile);
					string key = BackupUtilities.CreateFileNameKey(inputFile);
					string backupLocation = BackupUtilities.BackupFiles(
						BackupUtilities.BackupRoot, key, filesToModify);
					LogMessage(LogLevel.Info, "Backup created at {0}", backupLocation);
				}
				catch (Exception ex)
				{
					LogMessage(
						LogLevel.Error, "Unable to create backup for {0} - {1}", inputFile, ex.Message);
					success = false;
					_filesParsed = 0;
				}
			}

			if (success)
			{
				LogMessage(LogLevel.Verbose, "Writing files...");
				foreach (string fileToModify in filesToModify)
				{
					WriteFile(_arrangeResults[fileToModify]);
				}
			}

			return success;
		}

		#endregion Methods

		#region Nested Types

		/// <summary>
		/// Arrange result.
		/// </summary>
		private class ArrangeResult
		{
			#region Fields

			/// <summary>
			/// Encoding to use when writing the file.
			/// </summary>
			private readonly Encoding _encoding;

			/// <summary>
			/// Input file name.
			/// </summary>
			private readonly string _inputFile;

			/// <summary>
			/// Whether or not the file was modified after arranging.
			/// </summary>
			private readonly bool _modified;

			/// <summary>
			/// Output file name.
			/// </summary>
			private readonly string _outputFile;

			/// <summary>
			/// Output file text.
			/// </summary>
			private readonly string _outputFileText;

			#endregion Fields

			#region Constructors

			/// <summary>
			/// Creates a new ArrangeResult.
			/// </summary>
			/// <param name="encoding">Encoding to use for writing.</param>
			/// <param name="inputFile">Input file name.</param>
			/// <param name="inputFileText">Input file text.</param>
			/// <param name="outputFile">Output file name.</param>
			/// <param name="outputFileText">Output file text.</param>
			public ArrangeResult(
				Encoding encoding,
				string inputFile,
				string inputFileText,
				string outputFile,
				string outputFileText)
			{
				_encoding = encoding;
				_inputFile = inputFile;
				_outputFile = outputFile;
				_outputFileText = outputFileText;
				_modified = _inputFile != _outputFile ||
					inputFileText != outputFileText;
			}

			#endregion Constructors

			#region Properties

			/// <summary>
			/// Gets the encoding to use for writing.
			/// </summary>
			public Encoding Encoding
			{
				get
				{
					return _encoding;
				}
			}

			/// <summary>
			/// Gets a value indicating whether or not the file contents were modified.
			/// </summary>
			public bool Modified
			{
				get
				{
					return _modified;
				}
			}

			/// <summary>
			/// Gets the output file name.
			/// </summary>
			public string OutputFile
			{
				get
				{
					return _outputFile;
				}
			}

			/// <summary>
			/// Gets the output file text.
			/// </summary>
			public string OutputFileText
			{
				get
				{
					return _outputFileText;
				}
			}

			#endregion Properties
		}

		#endregion Nested Types
	}
}