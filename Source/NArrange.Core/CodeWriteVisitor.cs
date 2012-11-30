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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;

    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Abstract code write visitor implementation.
    /// </summary>
    public abstract class CodeWriteVisitor : ICodeElementVisitor
    {
        #region Fields

        /// <summary>
        /// Default element block length.
        /// </summary>
        protected const int DefaultBlockLength = 256;

        /// <summary>
        /// Code configuration.
        /// </summary>
        private readonly CodeConfiguration _configuration;

        /// <summary>
        /// The text writer to write elements to.
        /// </summary>
        private readonly TextWriter _writer;

        /// <summary>
        /// Tab count used to control the indentation level at which elements are written.
        /// </summary>
        private int _tabCount;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new VBWriteVisitor.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="configuration">The configuration.</param>
        protected CodeWriteVisitor(TextWriter writer, CodeConfiguration configuration)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            Debug.Assert(configuration != null, "Configuration should not be null.");

            _writer = writer;
            _configuration = configuration;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the code configuration.
        /// </summary>
        protected CodeConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        /// <summary>
        /// Gets or sets the current tab count used for writing indented text.
        /// </summary>
        protected int TabCount
        {
            get
            {
                return _tabCount;
            }
            set
            {
                _tabCount = value;
            }
        }

        /// <summary>
        /// Gets the writer.
        /// </summary>
        protected TextWriter Writer
        {
            get
            {
                return _writer;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Processes an attribute element.
        /// </summary>
        /// <param name="element">Attribute code element.</param>
        public abstract void VisitAttributeElement(AttributeElement element);

        /// <summary>
        /// Processes a comment element.
        /// </summary>
        /// <param name="element">Comment code element.</param>
        public abstract void VisitCommentElement(CommentElement element);

        /// <summary>
        /// Processes a condition directive element.
        /// </summary>
        /// <param name="element">Condition directive code element.</param>
        public abstract void VisitConditionDirectiveElement(ConditionDirectiveElement element);

        /// <summary>
        /// Processes a constructor element.
        /// </summary>
        /// <param name="element">Constructor code element.</param>
        public abstract void VisitConstructorElement(ConstructorElement element);

        /// <summary>
        /// Processes a delegate element.
        /// </summary>
        /// <param name="element">Delegate code element.</param>
        public abstract void VisitDelegateElement(DelegateElement element);

        /// <summary>
        /// Processes a event element.
        /// </summary>
        /// <param name="element">Event code element.</param>
        public abstract void VisitEventElement(EventElement element);

        /// <summary>
        /// Processes a field element.
        /// </summary>
        /// <param name="element">Field code element.</param>
        public abstract void VisitFieldElement(FieldElement element);

        /// <summary>
        /// Processes a group element.
        /// </summary>
        /// <param name="element">Group element.</param>
        public virtual void VisitGroupElement(GroupElement element)
        {
            //
            // Process all children
            //
            for (int childIndex = 0; childIndex < element.Children.Count; childIndex++)
            {
                ICodeElement childElement = element.Children[childIndex];

                FieldElement childFieldElement = childElement as FieldElement;
                if (childIndex > 0 && childFieldElement != null &&
                    childFieldElement.HeaderComments.Count > 0)
                {
                    WriteIndentedLine();
                }

                childElement.Accept(this);

                if (childIndex < element.Children.Count - 1)
                {
                    if (element.SeparatorType == GroupSeparatorType.Custom)
                    {
                        WriteIndentedLine(element.CustomSeparator);
                    }
                    else
                    {
                        WriteIndentedLine();
                    }

                    if (childElement is GroupElement)
                    {
                        WriteIndentedLine();
                    }
                }
            }
        }

        /// <summary>
        /// Processes a method element.
        /// </summary>
        /// <param name="element">Method code element.</param>
        public abstract void VisitMethodElement(MethodElement element);

        /// <summary>
        /// Processes a namespace element.
        /// </summary>
        /// <param name="element">Namespace code element.</param>
        public abstract void VisitNamespaceElement(NamespaceElement element);

        /// <summary>
        /// Processes a property element.
        /// </summary>
        /// <param name="element">Property code element.</param>
        public abstract void VisitPropertyElement(PropertyElement element);

        /// <summary>
        /// Processes a region element.
        /// </summary>
        /// <param name="element">Region code element.</param>
        public void VisitRegionElement(RegionElement element)
        {
            RegionStyle regionStyle = _configuration.Formatting.Regions.Style;
            if (regionStyle == RegionStyle.Default)
            {
                // Use the default region style
                regionStyle = RegionStyle.Directive;
            }

            if (regionStyle == RegionStyle.NoDirective ||
                !element.DirectivesEnabled)
            {
                CodeWriter.WriteVisitElements(element.Children, Writer, this);
            }
            else
            {
                if (regionStyle == RegionStyle.Directive)
                {
                    WriteRegionBeginDirective(element);
                }
                else if (regionStyle == RegionStyle.CommentDirective)
                {
                    CommentElement commentDirective = new CommentElement(
                        string.Format(
                        CultureInfo.InvariantCulture,
                        Configuration.Formatting.Regions.CommentDirectiveBeginFormat,
                        element.Name).TrimEnd());
                    VisitCommentElement(commentDirective);
                }

                Writer.WriteLine();

                WriteChildren(element);

                if (element.Children.Count > 0)
                {
                    Writer.WriteLine();
                }

                if (regionStyle == RegionStyle.Directive)
                {
                    WriteRegionEndDirective(element);
                }
                else if (regionStyle == RegionStyle.CommentDirective)
                {
                    string regionName = string.Empty;
                    if (Configuration.Formatting.Regions.EndRegionNameEnabled)
                    {
                        regionName = element.Name;
                    }

                    CommentElement commentDirective = new CommentElement(
                        string.Format(
                        CultureInfo.InvariantCulture,
                        Configuration.Formatting.Regions.CommentDirectiveEndFormat,
                        regionName).TrimEnd());
                    VisitCommentElement(commentDirective);
                }
            }
        }

        /// <summary>
        /// Processes a type element.
        /// </summary>
        /// <param name="element">Type code element.</param>
        public abstract void VisitTypeElement(TypeElement element);

        /// <summary>
        /// Processes a using element.
        /// </summary>
        /// <param name="element">Using/Import directive code element.</param>
        public abstract void VisitUsingElement(UsingElement element);

        /// <summary>
        /// Gets the formatted text to write for a comment.
        /// </summary>
        /// <param name="comment">Comment with text.</param>
        /// <returns>Formatted comment text.</returns>
        protected string FormatCommentText(ICommentElement comment)
        {
            string commentText = null;

            if (comment != null)
            {
                switch (comment.Type)
                {
                    case CommentType.Line:
                        int tabCount = 0;
                        string commentLine = ProcessLineWhitepace(comment.Text, ref tabCount);
                        string leadingSpace = CreateTabWhitespace(tabCount);
                        commentText = leadingSpace + commentLine;
                        break;

                    default:
                        commentText = comment.Text;
                        break;
                }
            }

            return commentText;
        }

        /// <summary>
        /// Writes children for a block element.
        /// </summary>
        /// <param name="element">Element whose children will be written.</param>
        protected virtual void WriteBlockChildren(ICodeElement element)
        {
            if (element.Children.Count == 0)
            {
                Writer.WriteLine();
            }
            else
            {
                //
                // Process all children
                //
                WriteChildren(element);
            }
        }

        /// <summary>
        /// Writes child elements.
        /// </summary>
        /// <param name="element">Element whose children will be written.</param>
        protected void WriteChildren(ICodeElement element)
        {
            if (element.Children.Count > 0)
            {
                Writer.WriteLine();
            }

            CodeWriter.WriteVisitElements(element.Children, Writer, this);

            if (element.Children.Count > 0)
            {
                Writer.WriteLine();
            }
        }

        /// <summary>
        /// Writes a closing comment.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="commentPrefix">Comment prefix.</param>
        protected void WriteClosingComment(TextCodeElement element, string commentPrefix)
        {
            if (Configuration.Formatting.ClosingComments.Enabled)
            {
                string format = Configuration.Formatting.ClosingComments.Format;
                if (!string.IsNullOrEmpty(format))
                {
                    string formatted = element.ToString(format);
                    Writer.Write(
                        string.Format(CultureInfo.InvariantCulture, " {0}{1}", commentPrefix, formatted));
                }
            }
        }

        /// <summary>
        /// Writes a collection of comment lines.
        /// </summary>
        /// <param name="comments">The comments.</param>
        protected void WriteComments(ReadOnlyCollection<ICommentElement> comments)
        {
            foreach (ICommentElement comment in comments)
            {
                comment.Accept(this);
                WriteIndentedLine();
            }
        }

        /// <summary>
        /// Writes the specified text using the current TabCount.
        /// </summary>
        /// <param name="text">The text to write.</param>
        protected void WriteIndented(string text)
        {
            _writer.Write(CreateTabWhitespace(_tabCount));
            _writer.Write(text);
        }

        /// <summary>
        /// Writes a line of text using the current TabCount.
        /// </summary>
        /// <param name="text">The text to write.</param>
        protected void WriteIndentedLine(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                WriteIndented(text);
            }
            _writer.WriteLine();
        }

        /// <summary>
        /// Writes a new line using the current TabCount.
        /// </summary>
        protected void WriteIndentedLine()
        {
            WriteIndentedLine(string.Empty);
        }

        /// <summary>
        /// Writes a starting region directive.
        /// </summary>
        /// <param name="element">The region element.</param>
        protected abstract void WriteRegionBeginDirective(RegionElement element);

        /// <summary>
        /// Writes an ending region directive.
        /// </summary>
        /// <param name="element">The region element.</param>
        protected abstract void WriteRegionEndDirective(RegionElement element);

        /// <summary>
        /// Writes a block of text.
        /// </summary>
        /// <param name="text">Block of text to write.</param>
        protected void WriteTextBlock(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                int originalTabCount = TabCount;
                string[] lines = text.Split(new char[] { '\n' }, StringSplitOptions.None);

                bool previousLineBlank = false;

                for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
                {
                    string line = lines[lineIndex];
                    bool isLineBlank = line.Trim().Length == 0;

                    if (!isLineBlank ||
                        !Configuration.Formatting.LineSpacing.RemoveConsecutiveBlankLines ||
                        !previousLineBlank)
                    {
                        int lineTabCount = 0;

                        string formattedLine = ProcessLineWhitepace(line, ref lineTabCount);

                        if (lineTabCount > TabCount)
                        {
                            TabCount = lineTabCount;
                        }

                        if (lineIndex < lines.Length - 1)
                        {
                            WriteIndentedLine(formattedLine);
                        }
                        else
                        {
                            WriteIndented(formattedLine);
                        }
                    }

                    previousLineBlank = isLineBlank;

                    TabCount = originalTabCount;
                }
            }
        }

        /// <summary>
        /// Creates whitespace for the specified number of tabs.
        /// </summary>
        /// <param name="tabCount">Number of tabs worth of whiespace to create.</param>
        /// <returns>Whitespace string.</returns>
        private string CreateTabWhitespace(int tabCount)
        {
            string leadingSpace;
            if (Configuration.Formatting.Tabs.TabStyle == TabStyle.Spaces)
            {
                leadingSpace = new string(' ', Configuration.Formatting.Tabs.SpacesPerTab * tabCount);
            }
            else if (Configuration.Formatting.Tabs.TabStyle == TabStyle.Tabs)
            {
                leadingSpace = new string('\t', tabCount);
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(
                    Thread.CurrentThread.CurrentCulture,
                    "Unknown tab style {0}.",
                    _configuration.Formatting.Tabs.TabStyle));
            }

            return leadingSpace;
        }

        /// <summary>
        /// Processes leading/trailing whitespace for a line of text.
        /// </summary>
        /// <param name="line">Line to process.</param>
        /// <param name="lineTabCount">Number of preceding tabs.</param>
        /// <returns>Processed line.</returns>
        private string ProcessLineWhitepace(string line, ref int lineTabCount)
        {
            string formattedLine;

            StringBuilder lineBuilder = new StringBuilder(line);

            while (lineBuilder.Length > 0 && CodeParser.IsWhiteSpace(lineBuilder[0]))
            {
                if (lineBuilder[0] == '\t')
                {
                    lineBuilder.Remove(0, 1);
                    lineTabCount++;
                }
                else if (lineBuilder[0] == ' ')
                {
                    int spaceCount = 0;
                    int index = 0;
                    while (lineBuilder.Length > 0 && index < lineBuilder.Length &&
                        lineBuilder[index] == ' ')
                    {
                        spaceCount++;
                        if (spaceCount == Configuration.Formatting.Tabs.SpacesPerTab)
                        {
                            lineBuilder.Remove(0, Configuration.Formatting.Tabs.SpacesPerTab);
                            spaceCount = 0;
                            index = 0;
                            lineTabCount++;
                        }
                        else
                        {
                            index++;
                        }
                    }

                    if (!(lineBuilder.Length > 0 && lineBuilder[0] == '\t'))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            formattedLine = lineBuilder.ToString().TrimEnd();

            return formattedLine;
        }

        #endregion Methods
    }
}