namespace NArrange.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Text;

    using NArrange.Core;

    /// <summary>
    /// Test logger.
    /// </summary>
    public class TestLogger : ILogger
    {
        #region Fields

        /// <summary>
        /// Logged events collection.
        /// </summary>
        private List<TestLogEvent> _events = new List<TestLogEvent>();

        /// <summary>
        /// Whether or not events should also be written to the console.
        /// </summary>
        private bool _writeToConsole = false;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the log event history.
        /// </summary>
        public ReadOnlyCollection<TestLogEvent> Events
        {
            get
            {
                return _events.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not messages should be written 
        /// to the console.
        /// </summary>
        public bool WriteToConsole
        {
            get
            {
                return _writeToConsole;
            }
            set
            {
                _writeToConsole = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Clears the test log.
        /// </summary>
        public void Clear()
        {
            _events.Clear();
        }

        /// <summary>
        /// Determines if the specified message exists in the log.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        /// <returns>Whether or not the message exists in the log.</returns>
        public bool HasMessage(LogLevel level, string message)
        {
            bool hasMessage = false;

            foreach (TestLogEvent logEvent in _events)
            {
                if (logEvent.Level == level && logEvent.Message == message)
                {
                    hasMessage = true;
                    break;
                }
            }

            return hasMessage;
        }

        /// <summary>
        /// Determines if a partial matching message exists in the log.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        /// <returns>Whether or not the message exists in the log.</returns>
        public bool HasPartialMessage(LogLevel level, string message)
        {
            bool hasMessage = false;

            foreach (TestLogEvent logEvent in _events)
            {
                if (logEvent.Level == level && logEvent.Message.Contains(message))
                {
                    hasMessage = true;
                    break;
                }
            }

            return hasMessage;
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        /// <param name="args">Message arguments.</param>
        public void LogMessage(LogLevel level, string message, params object[] args)
        {
            string formatted = string.Format(
                CultureInfo.InvariantCulture, message, args);

            if (WriteToConsole)
            {
                Console.WriteLine(formatted);
            }

            TestLogEvent logEvent = new TestLogEvent(level, formatted);

            _events.Add(logEvent);
        }

        /// <summary>
        /// Gets the text of all events.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            StringBuilder textBuilder = new StringBuilder();
            foreach (TestLogEvent logEvent in Events)
            {
                textBuilder.AppendLine(logEvent.ToString());
            }

            return textBuilder.ToString();
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Test log event.
        /// </summary>
        public struct TestLogEvent
        {
            #region Fields

            /// <summary>
            /// Log level.
            /// </summary>
            public readonly LogLevel Level;

            /// <summary>
            /// Log message.
            /// </summary>
            public readonly string Message;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Creates a new test log event.
            /// </summary>
            /// <param name="level">Log level.</param>
            /// <param name="message">Log message.</param>
            public TestLogEvent(LogLevel level, string message)
            {
                Level = level;
                Message = message;
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Gets the string representation.
            /// </summary>
            /// <returns>String representation.</returns>
            public override string ToString()
            {
                return string.Format("{0}: {1}", Level, Message);
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}