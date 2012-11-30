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

    /// <summary>
    /// Console logger.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        #region Fields

        /// <summary>
        /// Console message color for errors.
        /// </summary>
        private const ConsoleColor ErrorColor = ConsoleColor.Red;

        /// <summary>
        /// Console message color for informational messages.
        /// </summary>
        private const ConsoleColor InfoColor = ConsoleColor.Cyan;

        /// <summary>
        /// Console message color for trace messages.
        /// </summary>
        private const ConsoleColor TraceColor = ConsoleColor.Gray;

        /// <summary>
        /// Console message color for warnings.
        /// </summary>
        private const ConsoleColor WarningColor = ConsoleColor.Yellow;

        /// <summary>
        /// Log trace messages to the console.
        /// </summary>
        private bool _trace;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether or not trace messages should be 
        /// written.
        /// </summary>
        public bool Trace
        {
            get
            {
                return _trace;
            }
            set
            {
                _trace = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Writes a message to the console using the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="message">The message or format.</param>
        /// <param name="args">The message format arguments.</param>
        public static void WriteMessage(ConsoleColor color, string message, params object[] args)
        {
            ConsoleColor origColor = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = color;
                if (args != null && args.Length > 0)
                {
                    Console.WriteLine(message, args);
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
            finally
            {
                Console.ForegroundColor = origColor;
            }
        }

        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        /// <param name="args">Message arguments.</param>
        public void LogMessage(LogLevel level, string message, params object[] args)
        {
            switch (level)
            {
                case LogLevel.Error:
                    WriteMessage(ErrorColor, message, args);
                    break;

                case LogLevel.Warning:
                    WriteMessage(WarningColor, message, args);
                    break;

                case LogLevel.Info:
                    WriteMessage(InfoColor, message, args);
                    break;

                case LogLevel.Trace:
                    if (_trace)
                    {
                        WriteMessage(TraceColor, message, args);
                    }
                    break;

                default:
                    WriteMessage(Console.ForegroundColor, message, args);
                    break;
            }
        }

        #endregion Methods
    }
}