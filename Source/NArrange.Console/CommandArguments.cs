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
    using System.Collections.Generic;

    using NArrange.Core;

    /// <summary>
    /// Command arguments class for the NArrange console application.
    /// </summary>
    public sealed class CommandArguments
    {
        #region Fields

        /// <summary>
        /// Unix flag/switch prefix character.
        /// </summary>
        public const char UnixSwitch = '-';

        /// <summary>
        /// Windows flag/switch prefix character.
        /// </summary>
        public const char WindowsSwitch = '/';

        /// <summary>
        /// Perform backup.
        /// </summary>
        private bool _backup;

        /// <summary>
        /// Configuration file.
        /// </summary>
        private string _configuration;

        /// <summary>
        /// Input file or directory.
        /// </summary>
        private string _input;

        /// <summary>
        /// Output file or directory.
        /// </summary>
        private string _output;

        /// <summary>
        /// Perform restore.
        /// </summary>
        private bool _restore;

        /// <summary>
        /// Trace logging.
        /// </summary>
        private bool _trace;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Do not allow creation.
        /// </summary>
        private CommandArguments()
        {
        }

        #endregion Constructors

        #region Enumerations

        /// <summary>
        /// Enumeration that defines the valid command argument flags
        /// </summary>
        private enum CommandArgumentFlag
        {
            /// <summary>
            /// Unknown command argument.
            /// </summary>
            Unknown,

            /// <summary>
            /// Configuration file.
            /// </summary>
            Configuration,

            /// <summary>
            /// Perform backup.
            /// </summary>
            Backup,

            /// <summary>
            /// Perform restore.
            /// </summary>
            Restore,

            /// <summary>
            /// Trace logging.
            /// </summary>
            Trace
        }

        #endregion Enumerations

        #region Properties

        /// <summary>
        /// Gets a value indicating whether or not a backup should be performed.
        /// </summary>
        public bool Backup
        {
            get
            {
                return _backup;
            }
        }

        /// <summary>
        /// Gets the configuration file.
        /// </summary>
        public string Configuration
        {
            get
            {
                return _configuration;
            }
        }

        /// <summary>
        /// Gets the input file.
        /// </summary>
        public string Input
        {
            get
            {
                return _input;
            }
        }

        /// <summary>
        /// Gets the output file.
        /// </summary>
        public string Output
        {
            get
            {
                return _output;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not a restore should be performed.
        /// </summary>
        public bool Restore
        {
            get
            {
                return _restore;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not tracing should be performed.
        /// </summary>
        public bool Trace
        {
            get
            {
                return _trace;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Parses command arguments from an array of strings.
        /// </summary>
        /// <param name="args">String array arguments</param>
        /// <returns>A CommandArguments instance</returns>'=
        /// <exception cref="ArgumentException"/>
        public static CommandArguments Parse(params string[] args)
        {
            CommandArguments commandArguments = new CommandArguments();

            if (args == null || args.Length == 0)
            {
                OnParseError();
            }

            List<string> argList = new List<string>(args);
            for (int argIndex = 0; argIndex < argList.Count; argIndex++)
            {
                string arg = argList[argIndex];

                if (string.IsNullOrEmpty(arg))
                {
                    OnParseError();
                }

                if (arg.Length > 0 &&
                    ((!MonoUtilities.IsMonoRuntime && arg[0] == WindowsSwitch) ||
                    arg[0] == UnixSwitch))
                {
                    string argLower = arg.ToLower();
                    if (arg.Length >= 2)
                    {
                        string flagParts = arg.Substring(1);
                        int separatorIndex = flagParts.IndexOf(':');
                        string flagString = flagParts;
                        if (separatorIndex > 0)
                        {
                            flagString = flagParts.Substring(0, separatorIndex);
                        }

                        CommandArgumentFlag flag = ParseFlag(flagString);

                        switch (flag)
                        {
                            case CommandArgumentFlag.Configuration:
                                if (separatorIndex < 0)
                                {
                                    OnParseError(arg);
                                }
                                commandArguments._configuration = flagParts.Substring(separatorIndex + 1);
                                if (string.IsNullOrEmpty(commandArguments._configuration))
                                {
                                    OnParseError(arg);
                                }
                                break;

                            case CommandArgumentFlag.Backup:
                                commandArguments._backup = true;
                                break;

                            case CommandArgumentFlag.Restore:
                                commandArguments._restore = true;
                                break;

                            case CommandArgumentFlag.Trace:
                                commandArguments._trace = true;
                                break;

                            default:
                                OnParseError();
                                break;
                        }

                        argList.RemoveAt(argIndex);
                        argIndex--;
                    }
                    else
                    {
                        OnParseError(arg);
                    }
                }
                else
                {
                    if (commandArguments._input == null)
                    {
                        commandArguments._input = arg;
                        argList.RemoveAt(argIndex);
                        argIndex--;
                    }
                    else if (commandArguments._output == null)
                    {
                        commandArguments._output = arg;
                        argList.RemoveAt(argIndex);
                        argIndex--;
                    }
                }
            }

            if (argList.Count > 0)
            {
                OnParseError();
            }

            if (commandArguments._backup && commandArguments._restore)
            {
                OnParseError();
            }

            if (commandArguments.Output != null &&
                (commandArguments._backup || commandArguments._restore))
            {
                OnParseError();
            }

            return commandArguments;
        }

        /// <summary>
        /// Called when a parsing error occurs.
        /// </summary>
        /// <param name="invalidFlag">Invalid flag text.</param>
        private static void OnParseError(string invalidFlag)
        {
            throw new ArgumentException("Invalid flag " + invalidFlag);
        }

        /// <summary>
        /// Called when a parsing error occurs.
        /// </summary>
        private static void OnParseError()
        {
            throw new ArgumentException("args");
        }

        /// <summary>
        /// Parses a command argument flag from a text string.
        /// </summary>
        /// <param name="flagString">Command argument flag text.</param>
        /// <returns>Command argument flag.</returns>
        private static CommandArgumentFlag ParseFlag(string flagString)
        {
            CommandArgumentFlag flag = CommandArgumentFlag.Unknown;

            string[] enumStrings = Enum.GetNames(typeof(CommandArgumentFlag));
            foreach (string enumString in enumStrings)
            {
                string fullName = enumString.ToLower();
                char abbr = fullName[0];
                string lowerFlagString = flagString.ToLower();

                if (lowerFlagString == abbr.ToString() || lowerFlagString == fullName)
                {
                    flag = (CommandArgumentFlag)Enum.Parse(typeof(CommandArgumentFlag), enumString);
                }
            }

            return flag;
        }

        #endregion Methods
    }
}