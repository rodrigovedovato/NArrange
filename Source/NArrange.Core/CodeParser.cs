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
    using System.IO;
    using System.Text.RegularExpressions;

    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Base code parser implementation.
    /// </summary>
    public abstract class CodeParser : ICodeElementParser
    {
        #region Fields

        /// <summary>
        /// Default block length (for instantiating string builders)
        /// </summary>
        protected const int DefaultBlockLength = 256;

        /// <summary>
        /// Default word length (for instantiating string builders)
        /// </summary>
        protected const int DefaultWordLength = 16;

        /// <summary>
        /// Empty character.
        /// </summary>
        protected const char EmptyChar = '\0';

        /// <summary>
        /// Whitepace characters.
        /// </summary>
        protected static readonly char[] WhiteSpaceCharacters = { ' ', '\t', '\r', '\n' };

        /// <summary>
        /// Buffer for reading a character.
        /// </summary>
        private readonly char[] _charBuffer = new char[1];

        /// <summary>
        /// Code configuration.
        /// </summary>
        private CodeConfiguration _configuration;

        /// <summary>
        /// Current character.
        /// </summary>
        private char _currCh;

        /// <summary>
        /// Current line number.
        /// </summary>
        private int _lineNumber = 1;

        /// <summary>
        /// Current line character position.
        /// </summary>
        private int _position = 1;

        /// <summary>
        /// Previous character.
        /// </summary>
        private char _prevCh;

        /// <summary>
        /// Input text reader.
        /// </summary>
        private TextReader _reader;

        /// <summary>
        /// Regular expression cache.
        /// </summary>
        private Dictionary<string, Regex> _regexCache = new Dictionary<string, Regex>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the code configuration.
        /// </summary>
        public CodeConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = CodeConfiguration.Default;
                }

                return _configuration;
            }
            set
            {
                _configuration = value;
            }
        }

        /// <summary>
        /// Gets the most recently read character.
        /// </summary>
        protected char CurrentChar
        {
            get
            {
                return _currCh;
            }
        }

        /// <summary>
        /// Gets the next character in the stream, if any.
        /// </summary>
        /// <returns>Next character, if none then EmptyChar.</returns>
        protected char NextChar
        {
            get
            {
                int data = _reader.Peek();
                if (data > 0)
                {
                    char ch = (char)data;
                    return ch;
                }
                else
                {
                    return EmptyChar;
                }
            }
        }

        /// <summary>
        /// Gets the previously read character (i.e. before CurrentChar)
        /// </summary>
        protected char PreviousChar
        {
            get
            {
                return _prevCh;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether or not the specified character is whitespace.
        /// </summary>
        /// <param name="character">Character to test.</param>
        /// <returns>True if the character is whitespace, otherwise false.</returns>
        public static bool IsWhiteSpace(char character)
        {
            return character == ' ' || character == '\t' ||
                character == '\n' || character == '\r';
        }

        /// <summary>
        /// Parses a collection of code elements from a stream reader.
        /// </summary>
        /// <param name="reader">Code stream reader</param>
        /// <returns>A collection of parsed code elements.</returns>
        public ReadOnlyCollection<ICodeElement> Parse(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            List<ICodeElement> codeElements = new List<ICodeElement>();

            Reset();
            _reader = reader;

            codeElements = DoParseElements();

            return codeElements.AsReadOnly();
        }

        /// <summary>
        /// Applies attributes and comments to a code element.
        /// </summary>
        /// <param name="codeElement">Code element to apply to.</param>
        /// <param name="comments">Comments to apply.</param>
        /// <param name="attributes">Attributes to apply.</param>
        protected static void ApplyCommentsAndAttributes(
            CodeElement codeElement,
            ReadOnlyCollection<ICommentElement> comments,
            ReadOnlyCollection<AttributeElement> attributes)
        {
            CommentedElement commentedElement = codeElement as CommentedElement;
            if (commentedElement != null)
            {
                //
                // Add any header comments
                //
                foreach (ICommentElement comment in comments)
                {
                    commentedElement.AddHeaderComment(comment);
                }
            }

            AttributedElement attributedElement = codeElement as AttributedElement;
            if (attributedElement != null)
            {
                foreach (AttributeElement attribute in attributes)
                {
                    attributedElement.AddAttribute(attribute);

                    //
                    // Treat attribute comments as header comments
                    //
                    if (attribute.HeaderComments.Count > 0)
                    {
                        foreach (ICommentElement comment in attribute.HeaderComments)
                        {
                            attributedElement.AddHeaderComment(comment);
                        }

                        attribute.ClearHeaderCommentLines();
                    }
                }
            }
        }

        /// <summary>
        /// Parses elements from the current point in the stream.
        /// </summary>
        /// <returns>List of parsed code elements.</returns>
        protected abstract List<ICodeElement> DoParseElements();

        /// <summary>
        /// Eats the specified character.
        /// </summary>
        /// <param name="character">Character to eat.</param>
        protected void EatChar(char character)
        {
            EatWhiteSpace();
            TryReadChar();
            if (CurrentChar != character)
            {
                OnParseError("Expected " + character);
            }
        }

        /// <summary>
        /// Reads until the next non-whitespace character is reached.
        /// </summary>
        protected void EatWhiteSpace()
        {
            EatWhiteSpace(WhiteSpaceTypes.All);
        }

        /// <summary>
        /// Reads until the next non-whitespace character is reached.
        /// </summary>
        /// <param name="whiteSpaceType">Whitespace type flags.</param>
        protected void EatWhiteSpace(WhiteSpaceTypes whiteSpaceType)
        {
            int data = _reader.Peek();
            while (data > 0)
            {
                char ch = (char)data;

                if ((((whiteSpaceType & WhiteSpaceTypes.Space) == WhiteSpaceTypes.Space) && ch == ' ') ||
                    (((whiteSpaceType & WhiteSpaceTypes.Tab) == WhiteSpaceTypes.Tab) && ch == '\t') ||
                    (((whiteSpaceType & WhiteSpaceTypes.CarriageReturn) == WhiteSpaceTypes.CarriageReturn) && ch == '\r') ||
                    (((whiteSpaceType & WhiteSpaceTypes.Linefeed) == WhiteSpaceTypes.Linefeed) && ch == '\n'))
                {
                    TryReadChar();
                }
                else
                {
                    return;
                }

                data = _reader.Peek();
            }
        }

        /// <summary>
        /// Gets text from a comment directive, if present.
        /// </summary>
        /// <param name="comment">Comment element containing the directive.</param>
        /// <param name="pattern">Regular expression pattern.</param>
        /// <param name="group">Group key for the text to retrieve.</param>
        /// <returns>Text for the comment directive.</returns>
        protected string GetCommentDirectiveText(CommentElement comment, string pattern, string group)
        {
            string text = null;

            if (comment != null && comment.Type == CommentType.Line && !string.IsNullOrEmpty(pattern))
            {
                Regex regex = null;
                if (!_regexCache.TryGetValue(pattern, out regex))
                {
                    regex = new Regex(pattern, RegexOptions.IgnoreCase);
                    _regexCache.Add(pattern, regex);
                }

                string commentText = comment.Text.Trim();

                Match match = regex.Match(commentText);

                if (match != null && match.Length > 0)
                {
                    Group textGroup = match.Groups[group];
                    if (textGroup != null)
                    {
                        text = textGroup.Value.Trim();
                    }
                }
            }

            return text;
        }

        /// <summary>
        /// Throws a parse error 
        /// </summary>
        /// <param name="message">Parse error message.</param>
        protected void OnParseError(string message)
        {
            throw new ParseException(message, _lineNumber, _position);
        }

        /// <summary>
        /// Reads the current line. Does not update PreviousChar.
        /// </summary>
        /// <returns>The line read.</returns>
        protected string ReadLine()
        {
            string line = _reader.ReadLine();
            _lineNumber++;
            _position = 1;

            return line;
        }

        /// <summary>
        /// Tries to read the specified character from the stream and update
        /// the CurrentChar property.
        /// </summary>
        /// <param name="character">Character to read.</param>
        /// <returns>True if the character was read, otherwise false.</returns>
        protected bool TryReadChar(char character)
        {
            int data = _reader.Peek();
            char nextCh = (char)data;
            if (nextCh == character)
            {
                TryReadChar();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to read any character from the stream and places it in the
        /// CurrentChar property.
        /// </summary>
        /// <returns>True if a character was read, otherwise false.</returns>
        protected bool TryReadChar()
        {
            if (_reader.Read(_charBuffer, 0, 1) > 0)
            {
                _prevCh = _currCh;
                _currCh = _charBuffer[0];

                if (_currCh == '\n')
                {
                    _lineNumber++;
                    _position = 1;
                }
                else
                {
                    _position++;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Reset the parser to a state to handle parsing another input.
        /// </summary>
        private void Reset()
        {
            _currCh = '\0';
            _prevCh = '\0';
            _lineNumber = 1;
            _position = 1;
        }

        #endregion Methods
    }
}