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
 *<contributor>Clément Franchini</contributor>
 *<contributor>Justin Dearing</contributor>
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.VisualBasic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Text;
    using System.Threading;

    using NArrange.Core;
    using NArrange.Core.CodeElements;

    /// <summary>
    /// NArrange Visual Basic parser implementation.
    /// </summary>
    public sealed class VBParser : CodeParser
    {
        #region Methods

        /// <summary>
        /// Parses elements from the current point in the stream.
        /// </summary>
        /// <returns>A list of parsed code elements.</returns>
        protected override List<ICodeElement> DoParseElements()
        {
            return ParseElements(null);
        }

        /// <summary>
        /// Creates a constructor from a method element.
        /// </summary>
        /// <param name="methodElement">The method element.</param>
        /// <returns>A constructor element.</returns>
        private static ConstructorElement CreateConstructor(MethodElement methodElement)
        {
            ConstructorElement constructor = new ConstructorElement();
            constructor.Name = methodElement.Name;
            constructor.Access = methodElement.Access;
            constructor.MemberModifiers = methodElement.MemberModifiers;
            constructor.Parameters = methodElement.Parameters;
            constructor.BodyText = methodElement.BodyText;

            return constructor;
        }

        /// <summary>
        /// Gets the member or type access.
        /// </summary>
        /// <param name="wordList">The word list.</param>
        /// <returns>Member or type access.</returns>
        private static CodeAccess GetAccess(StringCollection wordList)
        {
            CodeAccess access = CodeAccess.None;

            if (wordList.Contains(VBKeyword.Public))
            {
                access = CodeAccess.Public;
            }
            else if (wordList.Contains(VBKeyword.Private))
            {
                access = CodeAccess.Private;
            }
            else
            {
                if (wordList.Contains(VBKeyword.Protected))
                {
                    access |= CodeAccess.Protected;
                }

                if (wordList.Contains(VBKeyword.Friend))
                {
                    access |= CodeAccess.Internal;
                }
            }

            return access;
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <param name="wordList">The word list.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="typeElementType">Type of the type element.</param>
        private static void GetElementType(
            StringCollection wordList,
            out ElementType elementType,
            out TypeElementType? typeElementType)
        {
            elementType = ElementType.NotSpecified;
            typeElementType = null;

            if (wordList.Contains(VBKeyword.Class))
            {
                elementType = ElementType.Type;
                typeElementType = TypeElementType.Class;
                return;
            }

            if (wordList.Contains(VBKeyword.Structure))
            {
                elementType = ElementType.Type;
                typeElementType = TypeElementType.Structure;
                return;
            }

            if (wordList.Contains(VBKeyword.Enumeration))
            {
                elementType = ElementType.Type;
                typeElementType = TypeElementType.Enum;
                return;
            }

            if (wordList.Contains(VBKeyword.Interface))
            {
                elementType = ElementType.Type;
                typeElementType = TypeElementType.Interface;
                return;
            }

            if (wordList.Contains(VBKeyword.Module))
            {
                elementType = ElementType.Type;
                typeElementType = TypeElementType.Module;
                return;
            }

            if (wordList.Contains(VBKeyword.Property))
            {
                elementType = ElementType.Property;
                return;
            }

            if (wordList.Contains(VBKeyword.Sub) || wordList.Contains(VBKeyword.Function) ||
                wordList.Contains(VBKeyword.Operator))
            {
                elementType = ElementType.Method;
                return;
            }

            if (wordList.Contains(VBKeyword.Event))
            {
                elementType = ElementType.Event;
                return;
            }

            if (wordList.Contains(VBKeyword.Delegate))
            {
                elementType = ElementType.Delegate;
                return;
            }
        }

        /// <summary>
        /// Gets the member attributes.
        /// </summary>
        /// <param name="wordList">The word list.</param>
        /// <returns>Member attributes.</returns>
        private static MemberModifiers GetMemberAttributes(StringCollection wordList)
        {
            MemberModifiers memberAttributes;
            memberAttributes = MemberModifiers.None;

            bool isSealed = wordList.Contains(VBKeyword.NotOverridable) ||
                wordList.Contains(VBKeyword.NotInheritable);
            if (isSealed)
            {
                memberAttributes |= MemberModifiers.Sealed;
            }

            bool isAbstract = wordList.Contains(VBKeyword.MustOverride) ||
                wordList.Contains(VBKeyword.MustInherit);
            if (isAbstract)
            {
                memberAttributes |= MemberModifiers.Abstract;
            }

            bool isStatic = wordList.Contains(VBKeyword.Shared);
            if (isStatic)
            {
                memberAttributes |= MemberModifiers.Static;
            }

            bool isVirtual = wordList.Contains(VBKeyword.Overridable);
            if (isVirtual)
            {
                memberAttributes |= MemberModifiers.Virtual;
            }

            bool isOverride = wordList.Contains(VBKeyword.Overrides);
            if (isOverride)
            {
                memberAttributes |= MemberModifiers.Override;
            }

            bool isNew = wordList.Contains(VBKeyword.Shadows);
            if (isNew)
            {
                memberAttributes |= MemberModifiers.New;
            }

            bool isConstant = wordList.Contains(VBKeyword.Constant);
            if (isConstant)
            {
                memberAttributes |= MemberModifiers.Constant;
            }

            bool isReadOnly = wordList.Contains(VBKeyword.ReadOnly);
            if (isReadOnly)
            {
                memberAttributes |= MemberModifiers.ReadOnly;
            }

            bool isPartial = wordList.Contains(VBKeyword.Partial);
            if (isPartial)
            {
                memberAttributes |= MemberModifiers.Partial;
            }

            return memberAttributes;
        }

        /// <summary>
        /// Gets the type of the operator.
        /// </summary>
        /// <param name="wordList">The word list.</param>
        /// <returns>Operator type.</returns>
        private static OperatorType GetOperatorType(StringCollection wordList)
        {
            OperatorType operatorType = OperatorType.None;

            if (wordList.Contains(VBKeyword.Widening))
            {
                operatorType = OperatorType.Implicit;
            }
            else if (wordList.Contains(VBKeyword.Narrowing))
            {
                operatorType = OperatorType.Explicit;
            }

            return operatorType;
        }

        /// <summary>
        /// Determines whether or not the specified char is a VB special character
        /// that signals a break in an alias.
        /// </summary>
        /// <param name="ch">Character to test.</param>
        /// <returns>Whether or not the character signifies an alias break.</returns>
        private static bool IsAliasBreak(char ch)
        {
            return ch == VBSymbol.BeginParameterList ||
                    ch == VBSymbol.EndParameterList ||
                    ch == VBSymbol.BeginTypeConstraintList ||
                    ch == VBSymbol.EndTypeConstraintList ||
                    ch == Environment.NewLine[0] ||
                    ch == VBSymbol.AliasSeparator ||
                    ch == VBSymbol.BeginAttribute ||
                    ch == VBSymbol.EndAttribute ||
                    ch == VBSymbol.LineDelimiter;
        }

        /// <summary>
        /// Pushes the comments to code elements list.
        /// </summary>
        /// <param name="codeElements">The code elements.</param>
        /// <param name="comments">The comments.</param>
        private static void PushComments(List<ICodeElement> codeElements, List<ICommentElement> comments)
        {
            if (comments.Count > 0)
            {
                foreach (ICommentElement commentElement in comments)
                {
                    codeElements.Add(commentElement);
                }
                comments.Clear();
            }
        }

        /// <summary>
        /// Captures an type name alias from the stream.
        /// </summary>
        /// <param name="captureGeneric">Whether or not generic symbols should be captured.</param>
        /// <returns>Type name.</returns>
        private string CaptureTypeName(bool captureGeneric)
        {
            return CaptureWord(captureGeneric);
        }

        /// <summary>
        /// Captures an type name alias from the stream.
        /// </summary>
        /// <returns>The type name.</returns>
        private string CaptureTypeName()
        {
            return CaptureTypeName(true);
        }

        /// <summary>
        /// Captures an alias or keyword from the stream.
        /// </summary>
        /// <param name="captureGeneric">Whether or not generic symbols should be parsed.</param>
        /// <returns>Captured word.</returns>
        private string CaptureWord(bool captureGeneric)
        {
            StringBuilder read = new StringBuilder();
            EatLineContinuation(read);

            EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);

            StringBuilder word = new StringBuilder(DefaultWordLength);

            if (read.Length > 0 && read[read.Length - 1] == VBSymbol.LineContinuation)
            {
                //
                // This is the scenario where the identifier starts
                // with an underscore.
                //
                word.Append(VBSymbol.LineContinuation);
            }

            char nextChar = NextChar;
            while (nextChar != EmptyChar)
            {
                if (captureGeneric && nextChar == VBSymbol.BeginParameterList)
                {
                    TryReadChar();
                    word.Append(CurrentChar);
                    EatWhiteSpace();

                    if (char.ToLower(NextChar) == char.ToLower(VBKeyword.Of[0]))
                    {
                        TryReadChar();
                        word.Append(CurrentChar);

                        if (char.ToLower(NextChar) == char.ToLower(VBKeyword.Of[1]))
                        {
                            TryReadChar();
                            word.Append(CurrentChar);
                            word.Append(' ');

                            word.Append(ParseNestedText(
                                VBSymbol.BeginParameterList, VBSymbol.EndParameterList, false, true));
                            word.Append(VBSymbol.EndParameterList);
                        }
                    }
                    else if (NextChar == VBSymbol.EndParameterList)
                    {
                        TryReadChar();
                        word.Append(CurrentChar);
                    }
                }
                else if (IsWhiteSpace(nextChar) || IsAliasBreak(nextChar))
                {
                    break;
                }
                else
                {
                    TryReadChar();
                    word.Append(CurrentChar);
                }

                nextChar = NextChar;
            }

            return word.ToString();
        }

        /// <summary>
        /// Captures an alias or keyword from the stream.
        /// </summary>
        /// <returns>The captured word.</returns>
        private string CaptureWord()
        {
            return CaptureWord(false);
        }

        /// <summary>
        /// Eats a line continuation char(s).
        /// </summary>
        private void EatLineContinuation()
        {
            EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);
            while (IsWhiteSpace(CurrentChar) && NextChar == VBSymbol.LineContinuation)
            {
                TryReadChar();
                EatWhiteSpace();
            }
        }

        /// <summary>
        /// Eats a line continuations capturing the text into the specified
        /// string builder.
        /// </summary>
        /// <param name="builder">The string builder.</param>
        private void EatLineContinuation(StringBuilder builder)
        {
            while (true)
            {
                while (NextChar == ' ' || NextChar == '\t')
                {
                    TryReadChar();
                    builder.Append(CurrentChar);
                }

                if (NextChar != VBSymbol.LineContinuation)
                {
                    break;
                }
                TryReadChar();
                builder.Append(CurrentChar);

                while (IsWhiteSpace(NextChar))
                {
                    TryReadChar();
                    builder.Append(CurrentChar);
                }

                if (!IsWhiteSpace(NextChar))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Eats a word.
        /// </summary>
        /// <param name="word">The word to eat.</param>
        private void EatWord(string word)
        {
            this.EatWord(word, "Expected " + word);
        }

        /// <summary>
        /// Eats a specific word.
        /// </summary>
        /// <param name="word">The word to eat.</param>
        /// <param name="message">The message if the word is not parsed.</param>
        private void EatWord(string word, string message)
        {
            EatLineContinuation();

            EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);

            foreach (char ch in word.ToCharArray())
            {
                TryReadChar();
                if (char.ToLower(CurrentChar) != char.ToLower(ch))
                {
                    this.OnParseError(message);
                }
            }
        }

        /// <summary>
        /// Parses an attribute.
        /// </summary>
        /// <param name="comments">Comments to add.</param>
        /// <returns>An attribute code element.</returns>
        private AttributeElement ParseAttribute(ReadOnlyCollection<ICommentElement> comments)
        {
            return ParseAttribute(comments, false);
        }

        /// <summary>
        /// Parses an attribute.
        /// </summary>
        /// <param name="comments">The comments.</param>
        /// <param name="nested">Whether or not the attribute is nested/chained.</param>
        /// <returns>An attribute code element.</returns>
        private AttributeElement ParseAttribute(ReadOnlyCollection<ICommentElement> comments, bool nested)
        {
            AttributeElement attributeElement = new AttributeElement();

            string typeName = CaptureTypeName(false);
            EatLineContinuation();

            //
            // Check for an attribute target
            //
            if (TryReadChar(VBSymbol.LineDelimiter))
            {
                attributeElement.Target = typeName;
                typeName = CaptureTypeName(false);
                EatLineContinuation();
            }

            attributeElement.Name = typeName;

            if (NextChar == VBSymbol.BeginParameterList)
            {
                string attributeText = ParseNestedText(
                    VBSymbol.BeginParameterList, VBSymbol.EndParameterList, true, true);
                attributeElement.BodyText = attributeText;
            }

            EatLineContinuation();

            while (!nested && TryReadChar(VBSymbol.AliasSeparator))
            {
                if (NextChar != VBSymbol.AliasSeparator)
                {
                    AttributeElement childAttributeElement = ParseAttribute(null, true);
                    if (string.IsNullOrEmpty(childAttributeElement.Target))
                    {
                        childAttributeElement.Target = attributeElement.Target;
                    }
                    attributeElement.AddChild(childAttributeElement);
                }
            }

            EatLineContinuation();

            if (!nested)
            {
                EatChar(VBSymbol.EndAttribute);
            }

            if (comments != null && comments.Count > 0)
            {
                foreach (ICommentElement comment in comments)
                {
                    attributeElement.AddHeaderComment(comment);
                }
            }

            return attributeElement;
        }

        /// <summary>
        /// Parses a block.
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <returns>Block text.</returns>
        private string ParseBlock(string blockName)
        {
            EatWhiteSpace();

            StringBuilder blockText = new StringBuilder(DefaultBlockLength);

            bool blockRead = false;

            while (!blockRead && NextChar != EmptyChar)
            {
                string line = ReadLine();
                string trimmedLine = line.TrimStart();

                if (trimmedLine.Length >= VBKeyword.End.Length &&
                    trimmedLine.Substring(0, VBKeyword.End.Length).ToUpperInvariant() ==
                    VBKeyword.End.ToUpperInvariant())
                {
                    if (trimmedLine.Length > VBKeyword.End.Length &&
                        IsWhiteSpace(trimmedLine[VBKeyword.End.Length]))
                    {
                        string restOfLine = trimmedLine.Substring(VBKeyword.End.Length).Trim();

                        if (restOfLine.ToUpperInvariant().StartsWith(blockName.ToUpperInvariant()) &&
                            (restOfLine.Length == blockName.Length ||
                            (restOfLine.Length > blockName.Length && IsWhiteSpace(restOfLine[blockName.Length]))))
                        {
                            blockRead = true;
                        }
                        else if (restOfLine.Length == 1 && restOfLine[0] == VBSymbol.LineContinuation)
                        {
                            string continuationLine = ReadLine();
                            if (continuationLine.Trim().ToUpperInvariant() == blockName.ToUpperInvariant())
                            {
                                blockRead = true;
                            }
                            else
                            {
                                blockText.AppendLine(line);
                                blockText.AppendLine(continuationLine);
                                line = string.Empty;
                            }
                        }
                    }
                }

                if (!blockRead)
                {
                    blockText.AppendLine(line);
                }
            }

            if (!blockRead)
            {
                this.OnParseError("Unexpected end of file. Expected End " + blockName);
            }

            return blockText.ToString().Trim();
        }

        /// <summary>
        /// Parses a comment line.
        /// </summary>
        /// <returns>Comment element.</returns>
        private CommentElement ParseCommentLine()
        {
            CommentElement commentLine;

            StringBuilder commentTextBuilder = new StringBuilder(DefaultBlockLength);

            CommentType commentType = CommentType.Line;
            if (NextChar == VBSymbol.BeginComment)
            {
                TryReadChar();
                if (NextChar == VBSymbol.BeginComment)
                {
                    commentType = CommentType.XmlLine;
                    TryReadChar();
                }
                else
                {
                    commentTextBuilder.Append(VBSymbol.BeginComment);
                }
            }

            commentTextBuilder.Append(ReadLine());
            commentLine = new CommentElement(commentTextBuilder.ToString(), commentType);
            return commentLine;
        }

        /// <summary>
        /// Parses a condition directive.
        /// </summary>
        /// <param name="line">The line to process.</param>
        /// <param name="isIf">Whether or not the directive is an if condition.</param>
        /// <returns>Condition directive code element.</returns>
        private ConditionDirectiveElement ParseConditionDirective(string line, out bool isIf)
        {
            int separatorIndex = line.IndexOfAny(WhiteSpaceCharacters);

            string directive = null;
            if (separatorIndex > 0)
            {
                directive = line.Substring(0, separatorIndex);
            }
            else
            {
                directive = line;
            }

            directive = VBKeyword.Normalize(directive);

            string condition = null;
            if (separatorIndex > 0)
            {
                condition = line.Substring(separatorIndex + 1).Trim();
            }

            isIf = directive == VBKeyword.If;

            switch (directive)
            {
                case VBKeyword.If:
                case VBKeyword.ElseIf:
                    if (string.IsNullOrEmpty(condition))
                    {
                        this.OnParseError("Expected a condition expression");
                    }
                    break;

                case VBKeyword.Else:
                    break;

                default:
                    this.OnParseError(
                        string.Format(
                        CultureInfo.InvariantCulture,
                        "Unhandled preprocessor directive '{0}'",
                        directive));
                    break;
            }

            ConditionDirectiveElement conditionDirective = new ConditionDirectiveElement();
            conditionDirective.ConditionExpression = condition;

            return conditionDirective;
        }

        /// <summary>
        /// Parses a delegate.
        /// </summary>
        /// <param name="access">The member access.</param>
        /// <param name="memberAttributes">The member attributes.</param>
        /// <returns>Delegate code element.</returns>
        private DelegateElement ParseDelegate(
            CodeAccess access, MemberModifiers memberAttributes)
        {
            string delegateType = CaptureWord();

            bool isFunction = false;
            switch (VBKeyword.Normalize(delegateType))
            {
                case VBKeyword.Sub:
                    isFunction = false;
                    break;

                case VBKeyword.Function:
                    isFunction = true;
                    break;

                default:
                    this.OnParseError(
                        "Expected Sub or Function for delegate declaration");
                    break;
            }

            MethodElement methodElement = ParseMethod(
                access, memberAttributes, isFunction, true, false, OperatorType.None, false, false, null);

            DelegateElement delegateElement = new DelegateElement();
            delegateElement.Name = methodElement.Name;
            delegateElement.Access = methodElement.Access;
            delegateElement.MemberModifiers = methodElement.MemberModifiers;
            delegateElement.Parameters = methodElement.Parameters;
            delegateElement.BodyText = methodElement.BodyText;
            if (isFunction)
            {
                delegateElement.Type = methodElement.Type;
            }

            foreach (TypeParameter typeParameter in methodElement.TypeParameters)
            {
                delegateElement.AddTypeParameter(typeParameter);
            }

            return delegateElement;
        }

        /// <summary>
        /// Parses elements from the current point in the stream.
        /// </summary>
        /// <param name="parentElement">Parent element</param>
        /// <returns>A list of parsed code elements.</returns>
        private List<ICodeElement> ParseElements(ICodeElement parentElement)
        {
            List<ICodeElement> codeElements = new List<ICodeElement>();
            List<ICodeElement> siblingElements = new List<ICodeElement>();
            List<ICommentElement> comments = new List<ICommentElement>();
            List<AttributeElement> attributes = new List<AttributeElement>();
            Stack<ICodeElement> enclosingElementStack = new Stack<ICodeElement>();

            StringBuilder elementBuilder = new StringBuilder(DefaultBlockLength);

            char nextChar;
            bool end = false;
            bool lineContinuation = false;

            while (TryReadChar() && !end)
            {
                switch (CurrentChar)
                {
                    //
                    // Comments
                    //
                    case VBSymbol.BeginComment:
                        CommentElement commentLine = ParseCommentLine();
                        string commentDirectiveRegionName =
                                GetCommentDirectiveText(commentLine, Configuration.Formatting.Regions.CommentDirectiveBeginPattern, "Name");

                        if (commentDirectiveRegionName != null)
                        {
                            PushComments(codeElements, comments);

                            RegionElement regionElement = new RegionElement();
                            regionElement.Name = commentDirectiveRegionName;
                            enclosingElementStack.Push(regionElement);
                        }
                        else
                        {
                            commentDirectiveRegionName = GetCommentDirectiveText(commentLine, Configuration.Formatting.Regions.CommentDirectiveEndPattern, "Name");
                            if (commentDirectiveRegionName != null)
                            {
                                if (enclosingElementStack.Count == 0 ||
                                enclosingElementStack.Peek().ElementType != ElementType.Region)
                                {
                                    this.OnParseError("Unmatched end region directive");
                                }

                                ICodeElement enclosingElement = enclosingElementStack.Pop();

                                //
                                // Add any processed comments to the region or condition directive.
                                //
                                if (comments.Count > 0)
                                {
                                    foreach (ICommentElement commentElement in comments)
                                    {
                                        enclosingElement.AddChild(commentElement);
                                    }
                                    comments.Clear();
                                }

                                //
                                // Are we processing a nested region or condition directive?
                                //
                                if (enclosingElementStack.Count > 0)
                                {
                                    enclosingElementStack.Peek().AddChild(enclosingElement);
                                }
                                else
                                {
                                    codeElements.Add(enclosingElement);
                                }
                            }
                            else
                            {
                                comments.Add(commentLine);
                            }
                        }
                        break;

                    //
                    // Preprocessor
                    //
                    case VBSymbol.Preprocessor:
                        //
                        // TODO: Parse additional preprocessor directives.
                        //
                        string line = ReadLine().Trim();
                        string[] words = line.Split(WhiteSpaceCharacters, StringSplitOptions.RemoveEmptyEntries);
                        if (words.Length > 0 && VBKeyword.Normalize(words[0]) == VBKeyword.Region)
                        {
                            PushComments(codeElements, comments);

                            RegionElement regionElement = ParseRegion(line);
                            enclosingElementStack.Push(regionElement);
                        }
                        else if (words.Length > 0 &&
                            (VBKeyword.Normalize(words[0]) == VBKeyword.If ||
                            VBKeyword.Normalize(words[0]) == VBKeyword.Else ||
                            VBKeyword.Normalize(words[0]) == VBKeyword.ElseIf))
                        {
                            bool isIf;
                            ConditionDirectiveElement conditionDirective = ParseConditionDirective(line.Trim(), out isIf);

                            if (isIf)
                            {
                                enclosingElementStack.Push(conditionDirective);
                            }
                            else
                            {
                                if (enclosingElementStack.Count == 0 ||
                                    enclosingElementStack.Peek().ElementType != ElementType.ConditionDirective)
                                {
                                    this.OnParseError("Expected 'If' preprocessor directive.");
                                }
                                else
                                {
                                    ConditionDirectiveElement previousCondition =
                                        enclosingElementStack.Peek() as ConditionDirectiveElement;
                                    while (previousCondition.ElseCondition != null)
                                    {
                                        previousCondition = previousCondition.ElseCondition;
                                    }

                                    // Add the condition to the end of the condition linked list
                                    previousCondition.ElseCondition = conditionDirective;
                                }
                            }
                        }
                        else if (words.Length > 1 &&
                            VBKeyword.Normalize(words[0]) == VBKeyword.End &&
                            (VBKeyword.Normalize(words[1]) == VBKeyword.Region ||
                            VBKeyword.Normalize(words[1]) == VBKeyword.If))
                        {
                            ICodeElement enclosingElement = null;

                            if (VBKeyword.Normalize(words[1]) == VBKeyword.Region)
                            {
                                if (enclosingElementStack.Count == 0 ||
                                    enclosingElementStack.Peek().ElementType != ElementType.Region)
                                {
                                    this.OnParseError("Unmatched end region directive");
                                }
                            }
                            else if (enclosingElementStack.Count == 0 ||
                                enclosingElementStack.Peek().ElementType != ElementType.ConditionDirective)
                            {
                                this.OnParseError("Unmatched #End If");
                            }

                            enclosingElement = enclosingElementStack.Pop();

                            //
                            // If there are any attributes not associated with an element (e.g.
                            // a condition directive containing only an attribute, then
                            // throw an error as this is currently not supported.
                            //
                            if (enclosingElement.ElementType == ElementType.ConditionDirective &&
                                attributes.Count > 0)
                            {
                                this.OnParseError("Cannot arrange files with preprocessor directives containing attributes unassociated to an element");
                            }

                            if (comments.Count > 0)
                            {
                                foreach (ICommentElement commentElement in comments)
                                {
                                    enclosingElement.AddChild(commentElement);
                                }
                                comments.Clear();
                            }

                            if (enclosingElementStack.Count > 0)
                            {
                                enclosingElementStack.Peek().AddChild(enclosingElement);
                            }
                            else
                            {
                                codeElements.Add(enclosingElement);
                            }
                        }
                        else
                        {
                            this.OnParseError(
                                "Cannot arrange files with preprocessor directives " +
                                "other than #Region, #End Region and conditional compilation directives");
                        }
                        break;

                    //
                    // Attribute
                    //
                    case VBSymbol.BeginAttribute:
                        nextChar = NextChar;

                        //
                        // Parse attribute
                        //
                        AttributeElement attributeElement = ParseAttribute(comments.AsReadOnly());

                        attributes.Add(attributeElement);
                        codeElements.Add(attributeElement);
                        comments.Clear();
                        break;

                    case VBSymbol.LineContinuation:
                        if (IsWhiteSpace(PreviousChar) && IsWhiteSpace(NextChar))
                        {
                            lineContinuation = true;
                        }
                        else
                        {
                            elementBuilder.Append(CurrentChar);
                        }
                        break;

                    // Eat any unneeded whitespace
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                    case ':':
                        if (elementBuilder.Length > 0)
                        {
                            string processedText = elementBuilder.ToString().Trim();
                            if (CurrentChar == '\n')
                            {
                                if (!lineContinuation)
                                {
                                    this.OnParseError(
                                        string.Format(
                                        Thread.CurrentThread.CurrentCulture,
                                        "Unhandled element text '{0}'",
                                        processedText));
                                }
                                else
                                {
                                    lineContinuation = false;
                                }
                            }

                            if (elementBuilder[elementBuilder.Length - 1] != ' ')
                            {
                                elementBuilder.Append(' ');
                            }
                        }
                        break;

                    default:
                        elementBuilder.Append(CurrentChar);

                        string upperElementText = elementBuilder.ToString().ToUpperInvariant();
                        nextChar = NextChar;

                        if (upperElementText == VBKeyword.End.ToUpperInvariant())
                        {
                            end = true;
                            elementBuilder = new StringBuilder(DefaultBlockLength);
                        }
                        else if (upperElementText == VBKeyword.Rem.ToUpperInvariant() &&
                            nextChar == ' ')
                        {
                            CommentElement remCommentLine = ParseCommentLine();
                            comments.Add(remCommentLine);
                            elementBuilder = new StringBuilder(DefaultBlockLength);
                        }
                        else if (upperElementText == VBKeyword.Option.ToUpperInvariant() &&
                            IsWhiteSpace(nextChar))
                        {
                            ICodeElement optionElement = ParseOption(comments.AsReadOnly());
                            comments.Clear();
                            codeElements.Add(optionElement);
                            elementBuilder = new StringBuilder(DefaultBlockLength);
                        }
                        else
                        {
                            if (char.IsWhiteSpace(nextChar) || VBSymbol.IsVBSymbol(CurrentChar))
                            {
                                string elementText = VBKeyword.Normalize(elementBuilder.ToString());
                                bool isImplements = elementText.StartsWith(
                                    VBKeyword.Implements, StringComparison.OrdinalIgnoreCase);
                                bool isInherits = !isImplements && elementText.StartsWith(
                                    VBKeyword.Inherits, StringComparison.OrdinalIgnoreCase);
                                TypeElement typeElement = parentElement as TypeElement;
                                if ((isImplements || isInherits) && typeElement != null)
                                {
                                    InterfaceReferenceType referenceType = InterfaceReferenceType.None;
                                    if (isInherits)
                                    {
                                        referenceType = InterfaceReferenceType.Class;
                                    }
                                    else if (isImplements)
                                    {
                                        referenceType = InterfaceReferenceType.Interface;
                                    }

                                    do
                                    {
                                        EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);
                                        if (NextChar == VBSymbol.AliasSeparator)
                                        {
                                            EatChar(VBSymbol.AliasSeparator);
                                        }

                                        string typeName = CaptureTypeName();
                                        InterfaceReference interfaceReference =
                                            new InterfaceReference(typeName, referenceType);
                                        typeElement.AddInterface(interfaceReference);
                                        EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);
                                    }
                                    while (NextChar == VBSymbol.AliasSeparator);

                                    elementBuilder = new StringBuilder(DefaultBlockLength);
                                }
                                else
                                {
                                    //
                                    // Try to parse a code element
                                    //
                                    ICodeElement element = TryParseElement(parentElement,
                                        elementBuilder, comments.AsReadOnly(), attributes.AsReadOnly());
                                    if (element != null)
                                    {
                                        // Since more than one field can be declared per line, we need special
                                        // handling here.
                                        FieldElement fieldElement = element as FieldElement;
                                        if (fieldElement != null)
                                        {
                                            if (elementBuilder[0] == VBSymbol.AliasSeparator
                                                && codeElements[codeElements.Count - 1] is FieldElement)
                                            {
                                                FieldElement previousFieldElement = codeElements[codeElements.Count - 1] as FieldElement;
                                                FieldElement currentField = element as FieldElement;

                                                if (currentField.Access == CodeAccess.None)
                                                {
                                                    currentField.Access = previousFieldElement.Access;
                                                }
                                            }
                                            else if (fieldElement.Name.Contains(VBSymbol.AliasSeparator.ToString()))
                                            {
                                                string[] fieldNames = fieldElement.Name.Split(
                                                    new char[]{VBSymbol.AliasSeparator}, StringSplitOptions.RemoveEmptyEntries);
                                                for(int fieldIndex = 0; fieldIndex < fieldNames.Length; fieldIndex++)
                                                {
                                                    if (fieldIndex == 0)
                                                    {
                                                        fieldElement.Name = fieldNames[0];
                                                    }
                                                    else
                                                    {
                                                        FieldElement siblingFieldElement = fieldElement.Clone() as FieldElement;
                                                        siblingFieldElement.Name = fieldNames[fieldIndex].Trim();
                                                        siblingElements.Add(siblingFieldElement);
                                                    }
                                                }
                                            }
                                        }

                                        if (element is CommentedElement)
                                        {
                                            UsingElement usingElement = element as UsingElement;

                                            //
                                            // If this is the first using statement, then don't attach
                                            // header comments to the element.
                                            //
                                            if (usingElement != null && parentElement == null && codeElements.Count == 0)
                                            {
                                                foreach (ICommentElement commentElement in usingElement.HeaderComments)
                                                {
                                                    if (enclosingElementStack.Count > 0)
                                                    {
                                                        enclosingElementStack.Peek().AddChild(commentElement);
                                                    }
                                                    else
                                                    {
                                                        codeElements.Add(commentElement);
                                                    }
                                                }
                                                usingElement.ClearHeaderCommentLines();
                                            }
                                            comments.Clear();
                                        }

                                        if (enclosingElementStack.Count > 0)
                                        {
                                            ICodeElement enclosingElement = enclosingElementStack.Peek();

                                            if (enclosingElement.ElementType == ElementType.ConditionDirective)
                                            {
                                                ConditionDirectiveElement conditionDirective = enclosingElement as ConditionDirectiveElement;
                                                while (conditionDirective.ElseCondition != null)
                                                {
                                                    conditionDirective = conditionDirective.ElseCondition;
                                                }

                                                enclosingElement = conditionDirective;
                                            }

                                            enclosingElement.AddChild(element);
                                            foreach (ICodeElement additionalElement in siblingElements)
                                            {
                                                enclosingElement.AddChild(additionalElement);
                                            }
                                        }
                                        else
                                        {
                                            codeElements.Add(element);
                                            foreach (ICodeElement additionalElement in siblingElements)
                                            {
                                                codeElements.Add(additionalElement);
                                            }
                                        }
                                        siblingElements.Clear();

                                        elementBuilder = new StringBuilder(DefaultBlockLength);

                                        if (element is IAttributedElement)
                                        {
                                            foreach (AttributeElement attribute in attributes)
                                            {
                                                codeElements.Remove(attribute);
                                            }

                                            attributes = new List<AttributeElement>();
                                        }
                                    }
                                }
                            }
                        }

                        break;
                }

                char nextCh = NextChar;
            }

            if (comments.Count > 0)
            {
                for (int commentIndex = 0; commentIndex < comments.Count; commentIndex++)
                {
                    ICommentElement comment = comments[commentIndex];
                    codeElements.Insert(commentIndex, comment);
                }
            }

            //
            // Make sure that all region elements have been closed
            //
            if (enclosingElementStack.Count > 0)
            {
                if (enclosingElementStack.Peek().ElementType == ElementType.Region)
                {
                    this.OnParseError(
                        string.Format(
                        CultureInfo.InvariantCulture,
                        "Missing end region directive for '{0}'",
                        enclosingElementStack.Peek().Name));
                }
                else
                {
                    this.OnParseError("Expected #End If");
                }
            }

            if (elementBuilder.Length > 0)
            {
                this.OnParseError(
                    string.Format(
                    Thread.CurrentThread.CurrentCulture,
                    "Unhandled element text '{0}'",
                    elementBuilder));
            }

            return codeElements;
        }

        /// <summary>
        /// Parses an event.
        /// </summary>
        /// <param name="access">The event access.</param>
        /// <param name="memberAttributes">The member attributes.</param>
        /// <param name="isCustom">Whether or not the event is custom.</param>
        /// <returns>An event code element.</returns>
        private EventElement ParseEvent(
            CodeAccess access, MemberModifiers memberAttributes, bool isCustom)
        {
            EventElement eventElement = new EventElement();
            string name = CaptureWord();
            eventElement.Name = name;
            eventElement.Access = access;
            eventElement.MemberModifiers = memberAttributes;

            EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);
            if (NextChar == VBSymbol.BeginParameterList)
            {
                eventElement.Parameters = ParseNestedText(
                    VBSymbol.BeginParameterList, VBSymbol.EndParameterList, true, true);
            }
            else
            {
                EatWord(VBKeyword.As);

                string eventType = CaptureTypeName();
                if (string.IsNullOrEmpty(eventType))
                {
                    this.OnParseError("Expected type identifier");
                }
                eventElement.Type = eventType;
            }

            string[] implements;
            string blockTemp = TryParseImplements(out implements);
            foreach (string implementation in implements)
            {
                InterfaceReference interfaceReference =
                    new InterfaceReference(implementation, InterfaceReferenceType.Interface);
                eventElement.AddImplementation(interfaceReference);
            }

            if (isCustom)
            {
                eventElement.BodyText = (blockTemp + this.ParseBlock(VBKeyword.Event)).Trim();
            }

            return eventElement;
        }

        /// <summary>
        /// Parses a field.
        /// </summary>
        /// <param name="wordList">The word list.</param>
        /// <param name="access">The field access.</param>
        /// <param name="memberAttributes">The member attributes.</param>
        /// <param name="untypedAssignment">Whether or not the field is untyped.</param>
        /// <returns>A field code element.</returns>
        private FieldElement ParseField(
            StringCollection wordList,
            CodeAccess access,
            MemberModifiers memberAttributes,
            bool untypedAssignment)
        {
            FieldElement field = new FieldElement();

            StringBuilder nameBuilder = new StringBuilder(DefaultWordLength);

            foreach (string word in wordList)
            {
                string trimmedWord = word.Trim(' ', VBSymbol.AliasSeparator);

                string upperWord = trimmedWord.ToUpperInvariant();
                if ((!VBKeyword.IsVBKeyword(trimmedWord) ||
                    upperWord == VBKeyword.Custom.ToUpperInvariant() ||
                    upperWord == VBKeyword.Ansi.ToUpperInvariant() ||
                    upperWord == VBKeyword.Unicode.ToUpperInvariant() ||
                    upperWord == VBKeyword.Auto.ToUpperInvariant()) &&
                    trimmedWord.Length > 0)
                {
                    nameBuilder.Append(trimmedWord);
                    nameBuilder.Append(VBSymbol.AliasSeparator);
                    nameBuilder.Append(' ');
                }
            }

            field.Name = nameBuilder.ToString().TrimEnd(VBSymbol.AliasSeparator, ' ');

            EatWhiteSpace();

            if (!untypedAssignment)
            {
                string returnType = CaptureTypeName();
                if (returnType.ToUpperInvariant() == VBKeyword.New.ToUpperInvariant())
                {
                    EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);
                    field.InitialValue = VBKeyword.New + " " + ReadCodeLine().Trim();
                }
                else
                {
                    field.Type = returnType;
                }
            }

            field.Access = access;
            field.MemberModifiers = memberAttributes;

            EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);

            bool isAssignment = NextChar == VBSymbol.Assignment || untypedAssignment;
            if (isAssignment)
            {
                if (!untypedAssignment)
                {
                    EatChar(VBSymbol.Assignment);
                }
                string initialValue = ParseInitialValue();

                field.InitialValue = initialValue;
            }

            EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);

            if (NextChar == VBSymbol.BeginComment)
            {
                EatChar(VBSymbol.BeginComment);

                string commentText = ReadLine().Trim();
                if (commentText.Length > 0)
                {
                    CommentElement comment = new CommentElement(commentText);
                    field.TrailingComment = comment;
                }
            }

            return field;
        }

        /// <summary>
        /// Parses an import directive.
        /// </summary>
        /// <returns>Using directive code element.</returns>
        private UsingElement ParseImport()
        {
            UsingElement usingElement = new UsingElement();
            string alias = CaptureWord();
            if (string.IsNullOrEmpty(alias))
            {
                this.OnParseError("Expected a namepace name");
            }

            EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);

            bool endOfStatement =
                TryReadChar(Environment.NewLine[0]) ||
                TryReadChar('\n') ||
                TryReadChar(VBSymbol.LineDelimiter);
            if (endOfStatement || NextChar == EmptyChar)
            {
                usingElement.Name = alias;
            }
            else
            {
                EatLineContinuation();
                bool assign = TryReadChar(VBSymbol.Assignment);
                if (!assign)
                {
                    this.OnParseError(
                        string.Format(
                        Thread.CurrentThread.CurrentCulture,
                        "Expected {0} or end of statement.",
                        VBSymbol.Assignment));
                }
                else
                {
                    string name = CaptureWord();
                    if (string.IsNullOrEmpty(name))
                    {
                        this.OnParseError("Expected a type or namepace name");
                    }
                    else
                    {
                        usingElement.Name = alias;
                        usingElement.Redefine = name;
                        TryReadChar(Environment.NewLine[0]);
                    }
                }
            }

            return usingElement;
        }

        /// <summary>
        /// Parses an initial value, such as for a field.
        /// </summary>
        /// <returns>Initial value text.</returns>
        private string ParseInitialValue()
        {
            EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);

            string initialValue = ReadCodeLine().Trim();

            if (string.IsNullOrEmpty(initialValue))
            {
                this.OnParseError("Expected an initial value");
            }
            return initialValue;
        }

        /// <summary>
        /// Parses a method.
        /// </summary>
        /// <param name="access">The member access.</param>
        /// <param name="memberAttributes">The member attributes.</param>
        /// <param name="isFunction">Whether or not the method is a function.</param>
        /// <param name="isDelegate">Whether or not the method is a delegate.</param>
        /// <param name="isOperator">Whether or not the method is an operator.</param>
        /// <param name="operatorType">Type of the operator.</param>
        /// <param name="inInterface">Whether or not the method is in an interface.</param>
        /// <param name="isExternal">Whether or not the method is external.</param>
        /// <param name="externalModifier">The external modifier.</param>
        /// <returns>A method element.</returns>
        private MethodElement ParseMethod(
            CodeAccess access,
            MemberModifiers memberAttributes,
            bool isFunction,
            bool isDelegate,
            bool isOperator,
            OperatorType operatorType,
            bool inInterface,
            bool isExternal,
            string externalModifier)
        {
            MethodElement method = new MethodElement();

            method.Name = CaptureWord();
            if (isOperator)
            {
                // Handle greater than/less than for the method name since these will get
                // excluded with < and > being alias breaks (needed by attributes).
                while (NextChar == VBSymbol.BeginAttribute || NextChar == VBSymbol.EndAttribute)
                {
                    TryReadChar();
                    method.Name += CurrentChar + CaptureWord();
                }
            }

            method.Access = access;
            method.MemberModifiers = memberAttributes;

            method.IsOperator = isOperator;
            method.OperatorType = operatorType;
            method[VBExtendedProperties.ExternalModifier] = externalModifier;

            if (isExternal)
            {
                method.MemberModifiers = method.MemberModifiers | MemberModifiers.External;

                EatLineContinuation();

                EatWord(VBKeyword.Lib);

                EatLineContinuation();

                EatChar(VBSymbol.BeginString);
                string library = CaptureWord().TrimEnd(VBSymbol.BeginString);

                method[VBExtendedProperties.ExternalLibrary] = library;

                EatLineContinuation();

                if (NextChar != VBSymbol.BeginParameterList)
                {
                    EatLineContinuation();

                    EatWord(VBKeyword.Alias);

                    EatLineContinuation();

                    EatChar(VBSymbol.BeginString);
                    string alias = CaptureWord().TrimEnd(VBSymbol.BeginString);

                    method[VBExtendedProperties.ExternalAlias] = alias;
                }
            }

            EatLineContinuation();

            EatChar(VBSymbol.BeginParameterList);
            EatWhiteSpace();
            string paramsTemp = string.Empty;

            if (char.ToLower(NextChar) == char.ToLower(VBKeyword.Of[0]))
            {
                TryReadChar();
                paramsTemp += CurrentChar;

                if (char.ToLower(NextChar) == char.ToLower(VBKeyword.Of[1]))
                {
                    TryReadChar();
                    paramsTemp = string.Empty;

                    this.ParseTypeParameters(method);

                    EatChar(VBSymbol.BeginParameterList);
                    EatWhiteSpace();
                }
            }

            method.Parameters = paramsTemp + ParseNestedText(
                VBSymbol.BeginParameterList, VBSymbol.EndParameterList, false, false);

            if (isFunction || isOperator)
            {
                EatLineContinuation();
                if (char.ToUpper(NextChar) == VBKeyword.As[0])
                {
                    EatWord(VBKeyword.As);
                    method.Type = CaptureTypeName();
                }
                else
                {
                    method.Type = string.Empty;
                }
            }

            EatWhiteSpace();

            string[] implements;
            string[] handles;
            bool parseHandles = !(isFunction || isOperator || isDelegate);
            string blockTemp = TryParseImplementsOrHandles(out implements, out handles, parseHandles);
            if (parseHandles)
            {
                method[VBExtendedProperties.Handles] = handles;
            }

            foreach (string implementation in implements)
            {
                InterfaceReference interfaceReference =
                     new InterfaceReference(implementation, InterfaceReferenceType.Interface);
                method.AddImplementation(interfaceReference);
            }

            if ((memberAttributes & MemberModifiers.Abstract) != MemberModifiers.Abstract &&
                !inInterface && !isExternal)
            {
                if (isFunction || isOperator)
                {
                    if (isOperator)
                    {
                        method.BodyText = (blockTemp + this.ParseBlock(VBKeyword.Operator)).Trim();
                    }
                    else if (!isDelegate)
                    {
                        method.BodyText = (blockTemp + this.ParseBlock(VBKeyword.Function)).Trim();
                    }
                }
                else if (!isDelegate)
                {
                    method.BodyText = (blockTemp + this.ParseBlock(VBKeyword.Sub)).Trim();
                }
            }

            return method;
        }

        /// <summary>
        /// Parses a namespace definition.
        /// </summary>
        /// <returns>A namespace code element.</returns>
        private NamespaceElement ParseNamespace()
        {
            NamespaceElement namespaceElement = new NamespaceElement();
            string namepaceName = CaptureWord();
            namespaceElement.Name = namepaceName;

            //
            // Parse child elements
            //
            List<ICodeElement> childElements = ParseElements(namespaceElement);
            foreach (ICodeElement childElement in childElements)
            {
                namespaceElement.AddChild(childElement);
            }

            EatWhiteSpace();
            EatWord(VBKeyword.Namespace, "Expected End Namespace");

            return namespaceElement;
        }

        /// <summary>
        /// Parses nested text.
        /// </summary>
        /// <param name="beginChar">The begin char.</param>
        /// <param name="endChar">The end char.</param>
        /// <param name="beginExpected">Whether or not the begin char is expected.</param>
        /// <param name="trim">Whether or not the text should be trimmed.</param>
        /// <returns>The parsed nested text.</returns>
        private string ParseNestedText(char beginChar, char endChar, bool beginExpected, bool trim)
        {
            StringBuilder blockText = new StringBuilder(DefaultBlockLength);

            if (beginChar != EmptyChar && beginExpected)
            {
                while (IsWhiteSpace(NextChar))
                {
                    TryReadChar();
                    if (!trim)
                    {
                        blockText.Append(CurrentChar);
                    }
                }

                EatChar(beginChar);
            }

            int depth = 1;
            char nextChar = NextChar;

            if (nextChar == EmptyChar)
            {
                this.OnParseError("Unexpected end of file. Expected " + endChar);
            }
            else if (nextChar == endChar)
            {
                TryReadChar();
            }
            else
            {
                bool inString = false;

                while (depth > 0)
                {
                    bool charRead = TryReadChar();
                    if (!charRead)
                    {
                        this.OnParseError("Unexpected end of file. Expected " + endChar);
                    }

                    nextChar = NextChar;

                    if (CurrentChar == VBSymbol.BeginString)
                    {
                        inString = !inString;
                    }

                    if (beginChar != EmptyChar && CurrentChar == beginChar &&
                        !inString)
                    {
                        blockText.Append(CurrentChar);
                        depth++;
                    }
                    else
                    {
                        blockText.Append(CurrentChar);
                    }

                    if (nextChar == endChar && !inString)
                    {
                        if (depth == 1)
                        {
                            EatChar(endChar);
                            break;
                        }
                        else
                        {
                            depth--;
                        }
                    }
                }
            }

            if (trim)
            {
                return blockText.ToString().Trim();
            }
            else
            {
                return blockText.ToString();
            }
        }

        /// <summary>
        /// Parses a VB Option directive from comments.
        /// </summary>
        /// <param name="comments">Comments to analyze.</param>
        /// <returns>The code element.</returns>
        private ICodeElement ParseOption(ReadOnlyCollection<ICommentElement> comments)
        {
            // HACK: Create an explicit element type for Option (or compiler directive)
            string option = VBKeyword.Option + ReadCodeLine();
            AttributeElement optionElement = new AttributeElement();
            optionElement.BodyText = option;
            optionElement[VBExtendedProperties.Option] = true;

            foreach (ICommentElement comment in comments)
            {
                optionElement.AddHeaderComment(comment);
            }

            return optionElement;
        }

        /// <summary>
        /// Parses a parameter list to a comma-separated string.
        /// </summary>
        /// <returns>Comma-separated string of parameters.</returns>
        private string ParseParams()
        {
            EatLineContinuation();

            return ParseNestedText(VBSymbol.BeginParameterList, VBSymbol.EndParameterList, true, false);
        }

        /// <summary>
        /// Parses a property.
        /// </summary>
        /// <param name="access">The access.</param>
        /// <param name="memberAttributes">The member attributes.</param>
        /// <param name="isDefault">Whether or not the property is a default property</param>
        /// <param name="modifyAccess">The modify access.</param>
        /// <param name="inInterface">Whether or not the property is part of an interface.</param>
        /// <returns>A property code element.</returns>
        private PropertyElement ParseProperty(
            CodeAccess access,
            MemberModifiers memberAttributes,
            bool isDefault,
            string modifyAccess,
            bool inInterface)
        {
            PropertyElement property = new PropertyElement();
            property.Name = CaptureWord();
            property.Access = access;
            property.MemberModifiers = memberAttributes;
            property[VBExtendedProperties.Default] = isDefault;
            property[VBExtendedProperties.AccessModifier] = modifyAccess;

            EatLineContinuation();

            if (NextChar == VBSymbol.BeginParameterList)
            {
                string indexParam = this.ParseParams();
                if (indexParam.Length > 0)
                {
                    property.IndexParameter = indexParam;
                }
            }

            EatWord(VBKeyword.As, "Expected As");

            string type = CaptureTypeName();
            if (string.IsNullOrEmpty(type))
            {
                this.OnParseError("Expected return type");
            }

            property.Type = type;

            string[] implements;
            string blockTemp = TryParseImplements(out implements);
            foreach (string implementation in implements)
            {
                InterfaceReference interfaceReference =
                    new InterfaceReference(implementation, InterfaceReferenceType.Interface);
                property.AddImplementation(interfaceReference);
            }

            if ((memberAttributes & MemberModifiers.Abstract) != MemberModifiers.Abstract &&
                !inInterface)
            {
                property.BodyText = (blockTemp + this.ParseBlock(VBKeyword.Property)).Trim();
            }

            return property;
        }

        /// <summary>
        /// Parses a region from the preprocessor line.
        /// </summary>
        /// <param name="line">Region start line.</param>
        /// <returns>A region element for the line.</returns>
        private RegionElement ParseRegion(string line)
        {
            RegionElement regionElement;
            string regionName = line.Substring(VBKeyword.Region.Length).Trim(' ', '"');

            if (string.IsNullOrEmpty(regionName))
            {
                this.OnParseError("Expected region name");
            }

            regionElement = new RegionElement();
            regionElement.Name = regionName;
            return regionElement;
        }

        /// <summary>
        /// Parses a type definition.
        /// </summary>
        /// <param name="access">The access.</param>
        /// <param name="typeAttributes">The type attributes.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>A type code element.</returns>
        private TypeElement ParseType(
            CodeAccess access, TypeModifiers typeAttributes, TypeElementType elementType)
        {
            TypeElement typeElement = new TypeElement();

            EatWhiteSpace();
            string className = CaptureWord();
            typeElement.Name = className;
            typeElement.Access = access;
            typeElement.Type = elementType;
            typeElement.TypeModifiers = typeAttributes;

            if (elementType == TypeElementType.Enum)
            {
                EatLineContinuation();

                if (NextChar == VBKeyword.As[0])
                {
                    EatWord(VBKeyword.As);
                    string interfaceName = CaptureTypeName();
                    InterfaceReference interfaceReference =
                        new InterfaceReference(interfaceName, InterfaceReferenceType.None);
                    typeElement.AddInterface(interfaceReference);
                }

                string enumText = ParseBlock(VBKeyword.Enumeration);

                // TODO: Parse enum values as fields
                typeElement.BodyText = enumText;
            }
            else
            {
                EatWhiteSpace();
                bool isGeneric = TryReadChar(VBSymbol.BeginParameterList);
                if (isGeneric)
                {
                    EatWord(VBKeyword.Of, "Expected Of");

                    this.ParseTypeParameters(typeElement);
                }

                EatWhiteSpace();

                //
                // Parse child elements
                //
                List<ICodeElement> childElements = ParseElements(typeElement);
                foreach (ICodeElement childElement in childElements)
                {
                    typeElement.AddChild(childElement);
                }

                EatWhiteSpace();
                string elementTypeString = EnumUtilities.ToString(elementType);
                EatWord(elementTypeString, "Expected End " + elementTypeString);
            }

            return typeElement;
        }

        /// <summary>
        /// Parses a type parameter constraint.
        /// </summary>
        /// <returns>The type parameter constraint text.</returns>
        private string ParseTypeParameterConstraint()
        {
            string typeParameterConstraint;
            typeParameterConstraint = CaptureTypeName();

            if (VBKeyword.Normalize(typeParameterConstraint) == VBKeyword.As)
            {
                this.OnParseError("Invalid identifier");
            }

            EatWhiteSpace();
            return typeParameterConstraint;
        }

        /// <summary>
        /// Parses type parameters.
        /// </summary>
        /// <param name="genericElement">The generic element.</param>
        private void ParseTypeParameters(IGenericElement genericElement)
        {
            EatWhiteSpace();

            if (NextChar == VBSymbol.EndParameterList ||
                NextChar == EmptyChar)
            {
                this.OnParseError("Expected type parameter");
            }

            do
            {
                if (genericElement.TypeParameters.Count > 0)
                {
                    if (NextChar == VBSymbol.AliasSeparator)
                    {
                        TryReadChar();
                    }
                    else
                    {
                        this.OnParseError("Expected , or )");
                    }
                }

                string typeParameterName = CaptureWord();

                EatWhiteSpace();

                if (NextChar == EmptyChar)
                {
                    break;
                }

                TypeParameter typeParameter = new TypeParameter();
                typeParameter.Name = typeParameterName;

                if (NextChar != VBSymbol.AliasSeparator &&
                    NextChar != VBSymbol.EndParameterList)
                {
                    if (char.ToLower(NextChar) == char.ToLower(VBKeyword.As[0]))
                    {
                        TryReadChar();

                        if (char.ToLower(NextChar) == char.ToLower(VBKeyword.As[1]))
                        {
                            TryReadChar();

                            EatWhiteSpace();

                            if (NextChar == VBSymbol.EndParameterList)
                            {
                                this.OnParseError("Expected type parameter constraint");
                            }

                            if (NextChar == VBSymbol.BeginTypeConstraintList)
                            {
                                TryReadChar();

                                while (NextChar != VBSymbol.EndTypeConstraintList &&
                                    NextChar != EmptyChar)
                                {
                                    string typeParameterConstraint;
                                    typeParameterConstraint = ParseTypeParameterConstraint();
                                    typeParameter.AddConstraint(typeParameterConstraint);

                                    if (NextChar != VBSymbol.EndTypeConstraintList)
                                    {
                                        if (NextChar == VBSymbol.AliasSeparator)
                                        {
                                            TryReadChar();
                                        }
                                        else
                                        {
                                            this.OnParseError("Expected , or }");
                                        }
                                    }
                                }

                                EatChar(VBSymbol.EndTypeConstraintList);
                            }
                            else
                            {
                                string typeParameterConstraint;
                                typeParameterConstraint = ParseTypeParameterConstraint();
                                typeParameter.AddConstraint(typeParameterConstraint);
                            }
                        }
                    }
                }

                genericElement.AddTypeParameter(typeParameter);

                EatWhiteSpace();
            }
            while (NextChar != VBSymbol.EndParameterList && NextChar != EmptyChar);

            EatChar(VBSymbol.EndParameterList);
        }

        /// <summary>
        /// Reads a code line.
        /// </summary>
        /// <returns>The code line.</returns>
        private string ReadCodeLine()
        {
            StringBuilder lineBuilder = new StringBuilder();
            char nextChar = NextChar;
            bool inString = false;
            while (!(nextChar == EmptyChar ||
                nextChar == Environment.NewLine[0] ||
                nextChar == '\n' ||
                (!inString && nextChar == VBSymbol.LineDelimiter)))
            {
                TryReadChar();
                lineBuilder.Append(CurrentChar);
                if (CurrentChar == VBSymbol.BeginString)
                {
                    inString = !inString;
                }

                nextChar = NextChar;
                if (nextChar == VBSymbol.BeginComment && !inString)
                {
                    break;
                }
            }

            string line = lineBuilder.ToString();

            if (line != null && line.Trim().Length > 0)
            {
                string startTrimmedLine = line.TrimStart();
                string trimmedLine = startTrimmedLine.TrimEnd();

                bool isComment =
                    startTrimmedLine.StartsWith(VBSymbol.BeginComment.ToString(), StringComparison.OrdinalIgnoreCase) ||
                    (startTrimmedLine.StartsWith(VBKeyword.Rem, StringComparison.OrdinalIgnoreCase) &&
                    (startTrimmedLine.Length == trimmedLine.Length || startTrimmedLine[VBKeyword.Rem.Length] == ' '));

                if (!isComment &&
                    (trimmedLine == VBSymbol.LineContinuation.ToString() ||
                    (trimmedLine[trimmedLine.Length - 1] == VBSymbol.LineContinuation &&
                    IsWhiteSpace(trimmedLine[trimmedLine.Length - 2]))))
                {
                    ReadLine();
                    line = line.TrimEnd(VBSymbol.LineContinuation).TrimEnd(WhiteSpaceCharacters) + " " +
                         ReadCodeLine();
                }
            }

            return line;
        }

        /// <summary>
        /// Tries to parse a code element.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="elementBuilder">The element builder.</param>
        /// <param name="comments">The comments.</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns>Code element is succesful, otherwise null.</returns>
        private ICodeElement TryParseElement(
            ICodeElement parentElement,
            StringBuilder elementBuilder,
            ReadOnlyCollection<ICommentElement> comments,
            ReadOnlyCollection<AttributeElement> attributes)
        {
            CodeElement codeElement = null;

            string processedElementText =
                elementBuilder.ToString().Trim();

            switch (VBKeyword.Normalize(processedElementText))
            {
                case VBKeyword.Namespace:
                    codeElement = ParseNamespace();
                    break;

                case VBKeyword.Imports:
                    codeElement = ParseImport();
                    break;
            }

            if (codeElement == null)
            {
                string[] words = processedElementText.TrimEnd(
                    VBSymbol.Assignment,
                    VBSymbol.BeginParameterList).Split(
                    WhiteSpaceCharacters,
                    StringSplitOptions.RemoveEmptyEntries);

                if (words.Length > 0)
                {
                    string normalizedKeyWord = VBKeyword.Normalize(words[0]);

                    if (words.Length > 1 ||
                        normalizedKeyWord == VBKeyword.Class ||
                        normalizedKeyWord == VBKeyword.Structure ||
                        normalizedKeyWord == VBKeyword.Interface ||
                        normalizedKeyWord == VBKeyword.Enumeration ||
                        normalizedKeyWord == VBKeyword.Module ||
                        normalizedKeyWord == VBKeyword.Sub ||
                        normalizedKeyWord == VBKeyword.Function ||
                        normalizedKeyWord == VBKeyword.Property ||
                        normalizedKeyWord == VBKeyword.Delegate ||
                        normalizedKeyWord == VBKeyword.Event)
                    {
                        StringCollection wordList = new StringCollection();
                        wordList.AddRange(words);

                        StringCollection normalizedWordList = new StringCollection();
                        foreach (string word in wordList)
                        {
                            normalizedWordList.Add(VBKeyword.Normalize(word));
                        }

                        string name = string.Empty;
                        ElementType elementType;
                        CodeAccess access = CodeAccess.None;
                        MemberModifiers memberAttributes = MemberModifiers.None;
                        TypeElementType? typeElementType = null;

                        bool isAssignment = processedElementText[processedElementText.Length - 1] == VBSymbol.Assignment;
                        bool isField = normalizedWordList[normalizedWordList.Count - 1] == VBKeyword.As ||
                            isAssignment;
                        if (isField)
                        {
                            elementType = ElementType.Field;
                        }
                        else
                        {
                            GetElementType(normalizedWordList, out elementType, out typeElementType);
                        }

                        if (elementType == ElementType.Method ||
                            elementType == ElementType.Property ||
                            elementType == ElementType.Event ||
                            elementType == ElementType.Delegate ||
                            elementType == ElementType.Type ||
                            elementType == ElementType.Field)
                        {
                            access = GetAccess(normalizedWordList);
                            memberAttributes = GetMemberAttributes(normalizedWordList);
                        }

                        TypeElement parentTypeElement = parentElement as TypeElement;
                        bool inInterface = parentTypeElement != null &&
                            parentTypeElement.Type == TypeElementType.Interface;

                        switch (elementType)
                        {
                            case ElementType.Type:
                                TypeModifiers typeAttributes = (TypeModifiers)memberAttributes;
                                if (normalizedWordList.Contains(VBKeyword.Partial))
                                {
                                    typeAttributes |= TypeModifiers.Partial;
                                }

                                codeElement = ParseType(access, typeAttributes, typeElementType.Value);
                                break;

                            case ElementType.Event:
                                codeElement = ParseEvent(
                                    access, memberAttributes, normalizedWordList.Contains(VBKeyword.Custom));
                                break;

                            case ElementType.Field:
                                FieldElement fieldElement = ParseField(wordList, access, memberAttributes, isAssignment);
                                fieldElement[VBExtendedProperties.WithEvents] =
                                    normalizedWordList.Contains(VBKeyword.WithEvents);
                                fieldElement[VBExtendedProperties.Dim] =
                                    normalizedWordList.Contains(VBKeyword.Dim);
                                codeElement = fieldElement;
                                break;

                            case ElementType.Property:
                                string modifyAccess = null;
                                if (normalizedWordList.Contains(VBKeyword.ReadOnly))
                                {
                                    modifyAccess = VBKeyword.ReadOnly;
                                }
                                else if (normalizedWordList.Contains(VBKeyword.ReadWrite))
                                {
                                    modifyAccess = VBKeyword.ReadWrite;
                                }
                                else if (normalizedWordList.Contains(VBKeyword.WriteOnly))
                                {
                                    modifyAccess = VBKeyword.WriteOnly;
                                }

                                bool isDefault = normalizedWordList.Contains(VBKeyword.Default);
                                codeElement = ParseProperty(access, memberAttributes, isDefault, modifyAccess, inInterface);
                                break;

                            case ElementType.Delegate:
                                codeElement = ParseDelegate(access, memberAttributes);
                                break;

                            case ElementType.Method:
                                bool isOperator = normalizedWordList.Contains(VBKeyword.Operator);
                                OperatorType operatorType = OperatorType.None;
                                if (isOperator)
                                {
                                    operatorType = GetOperatorType(wordList);
                                }

                                bool external = false;
                                string externalModifier = null;
                                if (normalizedWordList.Contains(VBKeyword.Declare))
                                {
                                    external = true;
                                    if (normalizedWordList.Contains(VBKeyword.Ansi))
                                    {
                                        externalModifier = VBKeyword.Ansi;
                                    }
                                    else if (normalizedWordList.Contains(VBKeyword.Unicode))
                                    {
                                        externalModifier = VBKeyword.Unicode;
                                    }
                                    else if (normalizedWordList.Contains(VBKeyword.Auto))
                                    {
                                        externalModifier = VBKeyword.Auto;
                                    }
                                }

                                //
                                // Method
                                //
                                MethodElement methodElement = ParseMethod(
                                        access,
                                        memberAttributes,
                                        normalizedWordList.Contains(VBKeyword.Function),
                                        false,
                                        isOperator,
                                        operatorType,
                                        inInterface,
                                        external,
                                        externalModifier);
                                methodElement[VBExtendedProperties.Overloads] =
                                    normalizedWordList.Contains(VBKeyword.Overloads);
                                if (VBKeyword.Normalize(methodElement.Name) == VBKeyword.New)
                                {
                                    codeElement = CreateConstructor(methodElement);
                                }
                                else
                                {
                                    codeElement = methodElement;
                                }
                                break;
                        }

                        if (codeElement != null)
                        {
                            codeElement[VBExtendedProperties.Overloads] =
                                normalizedWordList.Contains(VBKeyword.Overloads);
                        }
                    }
                }
            }

            if (codeElement != null)
            {
                ApplyCommentsAndAttributes(codeElement, comments, attributes);
            }

            return codeElement;
        }

        /// <summary>
        /// Tries to parse an implements clause.
        /// </summary>
        /// <param name="implements">The implement clauses.</param>
        /// <returns>Unhandled text.</returns>
        private string TryParseImplements(out string[] implements)
        {
            string[] handles;
            return TryParseImplementsOrHandles(out implements, out handles, false);
        }

        /// <summary>
        /// Tries to parse implements and handles clauses.
        /// </summary>
        /// <param name="implements">Parsed implements clauses.</param>
        /// <param name="handles">Parsed handles claues.</param>
        /// <param name="parseHandles">Whether or not handles should be parsed.</param>
        /// <returns>Unhandled text.</returns>
        private string TryParseImplementsOrHandles(
            out string[] implements, out string[] handles, bool parseHandles)
        {
            EatWhiteSpace();

            List<string> aliasList = new List<string>();

            StringBuilder read = new StringBuilder(DefaultBlockLength);

            EatLineContinuation(read);

            bool implementsRead = false;
            bool handlesRead = false;
            if (char.ToLower(NextChar) == char.ToLower(VBKeyword.Implements[0]))
            {
                foreach (char ch in VBKeyword.Implements.ToCharArray())
                {
                    if (char.ToLower(NextChar) == char.ToLower(ch))
                    {
                        TryReadChar();
                        read.Append(CurrentChar);
                        implementsRead = true;
                    }
                    else
                    {
                        implementsRead = false;
                        break;
                    }
                }
            }
            else if (parseHandles && char.ToLower(NextChar) == char.ToLower(VBKeyword.Handles[0]))
            {
                foreach (char ch in VBKeyword.Handles.ToCharArray())
                {
                    if (char.ToLower(NextChar) == char.ToLower(ch))
                    {
                        TryReadChar();
                        read.Append(CurrentChar);
                        handlesRead = true;
                    }
                    else
                    {
                        handlesRead = false;
                        break;
                    }
                }
            }

            if (implementsRead || handlesRead)
            {
                do
                {
                    EatLineContinuation(read);

                    if (aliasList.Count > 0 && NextChar == VBSymbol.AliasSeparator)
                    {
                        TryReadChar();
                    }

                    string alias = CaptureTypeName();
                    if (string.IsNullOrEmpty(alias))
                    {
                        if (implementsRead)
                        {
                            this.OnParseError("Expected an interface member name.");
                        }
                        else
                        {
                            this.OnParseError("Expected an event name.");
                        }
                    }
                    else if (read.Length > 0 && read[read.Length - 1] == VBSymbol.LineContinuation)
                    {
                        //
                        // This is the scenario where the identifier starts
                        // with an underscore.
                        //
                        alias = VBSymbol.LineContinuation.ToString() + alias;
                    }

                    aliasList.Add(alias);
                    read = new StringBuilder(DefaultBlockLength);
                    EatLineContinuation(read);
                }
                while (NextChar == VBSymbol.AliasSeparator);
            }

            implements = new string[] { };
            handles = new string[] { };
            if (implementsRead)
            {
                implements = aliasList.ToArray();
            }
            else if (handlesRead)
            {
                handles = aliasList.ToArray();
            }

            return read.ToString();
        }

        #endregion Methods
    }
}