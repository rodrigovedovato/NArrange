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

namespace NArrange.ConsoleApplication
{
	using System;
	using System.Reflection;
	using System.Text;

	using NArrange.Core;

	/// <summary>
	/// NArrange console application
	/// </summary>
	public class Program
	{
		#region Fields

		/// <summary>
		/// Execution error code.
		/// </summary>
		private const int Fail = -1;

		#endregion Fields

		#region Methods

		/// <summary>
		/// Gets the copyright text.
		/// </summary>
		/// <returns>Application copyright text.</returns>
		public static string GetCopyrightText()
		{
			StringBuilder copyrightText = new StringBuilder();

			Assembly assembly = Assembly.GetExecutingAssembly();

			object[] copyrightAttributes = assembly.GetCustomAttributes(
				typeof(AssemblyCopyrightAttribute), false);
			if (copyrightAttributes.Length > 0)
			{
				AssemblyCopyrightAttribute copyRight = copyrightAttributes[0] as AssemblyCopyrightAttribute;
				copyrightText.AppendLine(copyRight.Copyright.Replace("©", "(C)"));
				copyrightText.AppendLine("All rights reserved.");
				copyrightText.AppendLine("http://www.NArrange.net");
				copyrightText.AppendLine();
				copyrightText.AppendLine("Zip functionality courtesy of ic#code (Mike Krueger, John Reilly).");
			}

			return copyrightText.ToString();
		}

		/// <summary>
		/// Gets usage information text.
		/// </summary>
		/// <returns>Application usage text.</returns>
		public static string GetUsageText()
		{
			StringBuilder usage = new StringBuilder();

			char switchChar = CommandArguments.WindowsSwitch;
			string separator = "\t";
			if (MonoUtilities.IsMonoRuntime)
			{
				switchChar = CommandArguments.UnixSwitch;
				separator = "  ";
			}

			usage.AppendLine("Usage:");
			usage.AppendFormat("narrange-console <input> [output] [{0}c:configuration]", switchChar);
			usage.AppendLine();
			usage.AppendFormat("{0}[{1}b] [{1}r] [{1}t]", separator, switchChar);
			usage.AppendLine();
			usage.AppendLine();
			usage.AppendLine();
			usage.AppendFormat("input{0}Specifies the source code file, project, solution or ", separator);
			usage.AppendLine();
			usage.AppendFormat("{0}directory to arrange.", separator);
			usage.AppendLine();
			usage.AppendLine();
			usage.AppendFormat("output{0}For a single source file, specifies the output file ", separator);
			usage.AppendLine();
			usage.AppendFormat("{0}to write arranged code to.", separator);
			usage.AppendLine();
			usage.AppendFormat("{0}[Optional] If not specified the input source", separator);
			usage.AppendLine();
			usage.AppendFormat("{0}file will be overwritten.", separator);
			usage.AppendLine();
			usage.AppendLine();
			usage.AppendFormat("{1}c{0}Configuration - Specifies the XML configuration file to use.", separator, switchChar);
			usage.AppendLine();
			usage.AppendFormat("{0}[Optional] If not specified the default ", separator);
			usage.AppendLine();
			usage.AppendFormat("{0}configuration will be used.", separator);
			usage.AppendLine();
			usage.AppendLine();
			usage.AppendFormat("{1}b{0}Backup - Specifies to create a backup before arranging", separator, switchChar);
			usage.AppendLine();
			usage.AppendFormat("{0}[Optional] If not specified, no backup will be created.", separator);
			usage.AppendLine();
			usage.AppendFormat("{0}Only valid if an output file is not specified ", separator);
			usage.AppendLine();
			usage.AppendFormat("{0}and cannot be used in conjunction with Restore.", separator);
			usage.AppendLine();
			usage.AppendLine();
			usage.AppendFormat("{1}r{0}Restore - Restores arranged files from the latest backup", separator, switchChar);
			usage.AppendLine();
			usage.AppendFormat("{0}[Optional] When this flag is provided, no files will be arranged.", separator);
			usage.AppendLine();
			usage.AppendFormat("{0}Only valid if an output file is not specified ", separator);
			usage.AppendLine();
			usage.AppendFormat("{0}and cannot be used in conjunction with Backup.", separator);
			usage.AppendLine();
			usage.AppendLine();
			usage.AppendFormat("{1}t{0}Trace - Detailed logging", separator, switchChar);
			usage.AppendLine();

			return usage.ToString();
		}

		/// <summary>
		/// Application entry point.
		/// </summary>
		/// <param name="args">Command argument strings.</param>
		public static void Main(string[] args)
		{
			ConsoleLogger logger = new ConsoleLogger();

			Assembly assembly = Assembly.GetExecutingAssembly();
			Version version = assembly.GetName().Version;
			Console.WriteLine();
			ConsoleLogger.WriteMessage(ConsoleColor.Cyan, "NArrange {0}", version);
			Console.WriteLine(new string('_', 60));

			string copyrightText = GetCopyrightText();
			if (!string.IsNullOrEmpty(copyrightText))
			{
				Console.Write(copyrightText);
				Console.WriteLine();
			}

			if (args.Length < 1 || args[0] == "?" || args[0] == "/?" || args[0] == "help")
			{
				WriteUsage();
				Environment.Exit(Fail);
			}

			CommandArguments commandArgs = null;
			try
			{
				commandArgs = CommandArguments.Parse(args);
			}
			catch (ArgumentException)
			{
				WriteUsage();
				Environment.Exit(Fail);
			}

			logger.Trace = commandArgs.Trace;
			bool success = false;
			try
			{
				success = Run(logger, commandArgs);
			}
			catch (Exception ex)
			{
				logger.LogMessage(LogLevel.Error, ex.ToString());
			}

			if (!success)
			{
				Environment.Exit(Fail);
			}
		}

		/// <summary>
		/// Runs NArrange using the specified arguments.
		/// </summary>
		/// <param name="logger">Logger for messages.</param>
		/// <param name="commandArgs">Command arguments.</param>
		/// <returns>True if succesful, otherwise false.</returns>
		public static bool Run(ILogger logger, CommandArguments commandArgs)
		{
			bool success = true;

			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			else if (commandArgs == null)
			{
				throw new ArgumentNullException("commandArgs");
			}

			if (commandArgs.Restore)
			{
				logger.LogMessage(LogLevel.Verbose, "Restoring {0}...", commandArgs.Input);
				string key = BackupUtilities.CreateFileNameKey(commandArgs.Input);
				try
				{
					success = BackupUtilities.RestoreFiles(BackupUtilities.BackupRoot, key);
				}
				catch (Exception ex)
				{
					logger.LogMessage(LogLevel.Warning, ex.Message);
					success = false;
				}

				if (success)
				{
					logger.LogMessage(LogLevel.Info, "Restored");
				}
				else
				{
					logger.LogMessage(LogLevel.Error, "Restore failed");
				}
			}
			else
			{
				//
				// Arrange the source code file
				//
				FileArranger fileArranger = new FileArranger(commandArgs.Configuration, logger);
				success = fileArranger.Arrange(commandArgs.Input, commandArgs.Output, commandArgs.Backup);

				if (!success)
				{
					logger.LogMessage(LogLevel.Error, "Unable to arrange {0}.", commandArgs.Input);
				}
				else
				{
					logger.LogMessage(LogLevel.Info, "Arrange successful.");
				}
			}

			return success;
		}

		/// <summary>
		/// Writes usage information to the console.
		/// </summary>
		private static void WriteUsage()
		{
			string usage = GetUsageText();
			Console.Write(usage);
			Console.WriteLine();
		}

		#endregion Methods
	}
}