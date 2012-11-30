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

namespace NArrange.CSharp
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
	/// NArrange CSharp parser implementation.
	/// </summary>
	public sealed class CSharpParser : CodeParser
	{
		#region Fields

		/// <summary>
		/// Escape character.
		/// </summary>
		private const char EscapeChar = '\\';

		#endregion Fields

		#region Methods

		/// <summary>
		/// Parses elements from the current point in the stream.
		/// </summary>
		/// <returns>Collection of parsed code elements.</returns>
		protected override List<ICodeElement> DoParseElements()
		{
			return ParseElements(null);
		}

		/// <summary>
		/// Gets the member or type access.
		/// </summary>
		/// <param name="wordList">The word list.</param>
		/// <returns>Member or type access.</returns>
		private static CodeAccess GetAccess(StringCollection wordList)
		{
			CodeAccess access = CodeAccess.None;

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Public))
			{
				access = CodeAccess.Public;
			}
			else if (TryFindAndRemoveWord(wordList, CSharpKeyword.Private))
			{
				access = CodeAccess.Private;
			}
			else
			{
				if (TryFindAndRemoveWord(wordList, CSharpKeyword.Protected))
				{
					access |= CodeAccess.Protected;
				}

				if (TryFindAndRemoveWord(wordList, CSharpKeyword.Internal))
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
			StringCollection wordList, out ElementType elementType, out TypeElementType? typeElementType)
		{
			elementType = ElementType.NotSpecified;
			typeElementType = null;

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Class))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Class;
				return;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Structure))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Structure;
				return;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Enumeration))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Enum;
				return;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Interface))
			{
				elementType = ElementType.Type;
				typeElementType = TypeElementType.Interface;
				return;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Event))
			{
				elementType = ElementType.Event;
				return;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Delegate))
			{
				elementType = ElementType.Delegate;
				return;
			}
		}

		/// <summary>
		/// Gets a member modifiers flags enum from the modifer word list.
		/// </summary>
		/// <param name="wordList">Modifiers word list.</param>
		/// <returns>Member modifier flags enumeration.</returns>
		private static MemberModifiers GetMemberAttributes(StringCollection wordList)
		{
			MemberModifiers memberAttributes;
			memberAttributes = MemberModifiers.None;

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Sealed))
			{
				memberAttributes |= MemberModifiers.Sealed;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Abstract))
			{
				memberAttributes |= MemberModifiers.Abstract;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Static))
			{
				memberAttributes |= MemberModifiers.Static;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Unsafe))
			{
				memberAttributes |= MemberModifiers.Unsafe;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Virtual))
			{
				memberAttributes |= MemberModifiers.Virtual;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Override))
			{
				memberAttributes |= MemberModifiers.Override;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.New))
			{
				memberAttributes |= MemberModifiers.New;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Constant))
			{
				memberAttributes |= MemberModifiers.Constant;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.ReadOnly))
			{
				memberAttributes |= MemberModifiers.ReadOnly;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.External))
			{
				memberAttributes |= MemberModifiers.External;
			}

			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Partial))
			{
				memberAttributes |= MemberModifiers.Partial;
			}

			return memberAttributes;
		}

		/// <summary>
		/// Extracts a member name.
		/// </summary>
		/// <param name="words">The words to extract the member name and type from.</param>
		/// <param name="name">The member name.</param>
		/// <param name="returnType">The meber return type.</param>
		private static void GetMemberNameAndType(
			StringCollection words, out string name, out string returnType)
		{
			name = null;
			returnType = null;

			for (int wordIndex = 0; wordIndex < words.Count; wordIndex++)
			{
				string wordGroup = words[wordIndex];
				int separatorIndex = wordGroup.IndexOf(CSharpSymbol.AliasSeparator);
				if (separatorIndex >= 0 && wordGroup[wordGroup.Length - 1] != CSharpSymbol.EndAttribute)
				{
					if (separatorIndex < wordGroup.Length - 1)
					{
						//
						// Format words with commas to have a space after comma
						//
						string[] aliases = wordGroup.Split(CSharpSymbol.AliasSeparator);
						wordGroup = string.Empty;
						for (int aliasIndex = 0; aliasIndex < aliases.Length; aliasIndex++)
						{
							string alias = aliases[aliasIndex];
							wordGroup += alias.Trim();
							if (aliasIndex < aliases.Length - 1)
							{
								wordGroup += ", ";
							}
						}

						wordGroup = wordGroup.TrimEnd();
						words[wordIndex] = wordGroup;
					}

					//
					// Concatenate comma separated values into logical groups
					//
					if (wordGroup[0] == CSharpSymbol.AliasSeparator && wordIndex > 0)
					{
						if (wordGroup.Length == 1 && wordIndex < words.Count - 1)
						{
							words[wordIndex - 1] = words[wordIndex - 1] +
								CSharpSymbol.AliasSeparator + " " +
								words[wordIndex + 1];
							words.RemoveAt(wordIndex);
							words.RemoveAt(wordIndex);
							wordIndex--;
							wordIndex--;
						}
						else
						{
							words[wordIndex - 1] = words[wordIndex - 1] + wordGroup;
							words.RemoveAt(wordIndex);
							wordIndex--;
						}
					}
					else if (wordIndex < words.Count &&
						wordGroup[wordGroup.Length - 1] == CSharpSymbol.AliasSeparator)
					{
						wordGroup = wordGroup + " " + words[wordIndex + 1];
						words[wordIndex] = wordGroup;
						words.RemoveAt(wordIndex + 1);
						wordIndex--;
					}
				}
			}

			if (words.Count > 1)
			{
				int nameIndex = words.Count - 1;
				name = words[nameIndex];
				words.RemoveAt(nameIndex);

				int typeIndex = nameIndex;
				string typeCandidate;

				do
				{
					typeIndex--;
					typeCandidate = words[typeIndex];
					words.RemoveAt(typeIndex);
				}
				while (words.Count > 0 &&
					(typeCandidate == CSharpKeyword.Operator ||
					typeCandidate == CSharpKeyword.Implicit ||
					typeCandidate == CSharpKeyword.Explicit));

				if (name[name.Length - 1] == CSharpSymbol.EndAttribute && words.Count > 0)
				{
					//
					// Property indexer
					//
					while (typeIndex > 0 && name.IndexOf(CSharpSymbol.BeginAttribute) < 0)
					{
						name = typeCandidate + " " + name;
						typeIndex--;
						typeCandidate = words[typeIndex];
						words.RemoveAt(typeIndex);
					}

					if (name[0] == CSharpSymbol.BeginAttribute)
					{
						name = typeCandidate + name;
						typeIndex--;
						typeCandidate = words[typeIndex];
						words.RemoveAt(typeIndex);
					}
				}

				//
				// Array return type with spaces?
				//
				while (typeCandidate[typeCandidate.Length - 1] == CSharpSymbol.EndAttribute &&
					typeCandidate[0] == CSharpSymbol.BeginAttribute)
				{
					typeIndex--;
					typeCandidate = words[typeIndex] + typeCandidate;
					words.RemoveAt(typeIndex);
				}

				if (typeCandidate != CSharpKeyword.Abstract &&
					typeCandidate != CSharpKeyword.Constant &&
					typeCandidate != CSharpKeyword.Internal &&
					typeCandidate != CSharpKeyword.New &&
					typeCandidate != CSharpKeyword.Override &&
					typeCandidate != CSharpKeyword.Private &&
					typeCandidate != CSharpKeyword.Protected &&
					typeCandidate != CSharpKeyword.Public &&
					typeCandidate != CSharpKeyword.ReadOnly &&
					typeCandidate != CSharpKeyword.Sealed &&
					typeCandidate != CSharpKeyword.Static &&
					typeCandidate != CSharpKeyword.Virtual &&
					typeCandidate != CSharpKeyword.Operator &&
					typeCandidate != CSharpKeyword.Implicit &&
					typeCandidate != CSharpKeyword.Explicit &&
					typeCandidate != CSharpKeyword.Async)
				{
					returnType = typeCandidate;
				}
			}
			else
			{
				name = words[0];
				words.RemoveAt(0);
			}
		}

		/// <summary>
		/// Gets the operator type from the modifier list.
		/// </summary>
		/// <param name="wordList">Modifier words.</param>
		/// <returns>Operator type.</returns>
		private static OperatorType GetOperatorType(StringCollection wordList)
		{
			OperatorType operatorType = OperatorType.None;
			if (TryFindAndRemoveWord(wordList, CSharpKeyword.Explicit))
			{
				operatorType = OperatorType.Explicit;
			}
			else if (TryFindAndRemoveWord(wordList, CSharpKeyword.Implicit))
			{
				operatorType = OperatorType.Implicit;
			}

			return operatorType;
		}

		/// <summary>
		/// Determines whether or not the specified char is a C# special character
		/// that signals a break in an alias.
		/// </summary>
		/// <param name="ch">Character to test.</param>
		/// <returns>True if the character is an alias break, otherwise false.</returns>
		private static bool IsAliasBreak(char ch)
		{
			return ch == CSharpSymbol.BeginParameterList ||
					ch == CSharpSymbol.EndParameterList ||
					ch == CSharpSymbol.EndOfStatement ||
					ch == CSharpSymbol.AliasSeparator ||
					ch == CSharpSymbol.TypeImplements ||
					ch == CSharpSymbol.BeginBlock ||
					ch == CSharpSymbol.EndBlock ||
					ch == CSharpSymbol.Negate ||
					ch == CSharpSymbol.Assignment ||
					ch == CSharpSymbol.BeginAttribute ||
					ch == CSharpSymbol.EndAttribute;
		}

		/// <summary>
		/// Parses a region from the preprocessor line.
		/// </summary>
		/// <param name="line">Line of text containing the region to parse.</param>
		/// <returns>A region element.</returns>
		private static RegionElement ParseRegion(string line)
		{
			RegionElement regionElement;
			string regionName = line.Substring(CSharpKeyword.Region.Length).Trim();

			// A region name is not required, so allow empty string
			regionElement = new RegionElement();
			regionElement.Name = regionName;
			return regionElement;
		}

		/// <summary>
		/// Pushes the comments to the code elements collection.
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
		/// Trims the trailing white space.
		/// </summary>
		/// <param name="elementBuilder">The element builder.</param>
		private static void TrimTrailingWhiteSpace(StringBuilder elementBuilder)
		{
			while (elementBuilder.Length > 0 &&
									 IsWhiteSpace(elementBuilder[elementBuilder.Length - 1]))
			{
				elementBuilder.Remove(elementBuilder.Length - 1, 1);
			}
		}

		/// <summary>
		/// Tries to find and remove a word from the word list.
		/// </summary>
		/// <param name="wordList">The word list.</param>
		/// <param name="word">The word to find and remove.</param>
		/// <returns>Whether or not the word was found and removed.</returns>
		private static bool TryFindAndRemoveWord(StringCollection wordList, string word)
		{
			bool removed = false;

			int wordIndex = wordList.IndexOf(word);
			if (wordIndex >= 0)
			{
				wordList.RemoveAt(wordIndex);
				removed = true;
			}

			return removed;
		}

		/// <summary>
		/// Captures a type name alias from the stream.
		/// </summary>
		/// <returns>Captured type name.</returns>
		private string CaptureTypeName()
		{
			return CaptureTypeName(true);
		}

		/// <summary>
		/// Captures a type name alias from the stream.
		/// </summary>
		/// <param name="captureGeneric">
		/// Whether or not to capture generic symbols.
		/// </param>
		/// <returns>Captured type name.</returns>
		private string CaptureTypeName(bool captureGeneric)
		{
			string typeName = CaptureWord(captureGeneric);
			EatWhiteSpace();

			//
			// Array with space in between?
			//
			if (CurrentChar == CSharpSymbol.BeginAttribute)
			{
				EatWhiteSpace();
				EatChar(CSharpSymbol.EndAttribute);

				typeName += CSharpSymbol.BeginAttribute.ToString() + CSharpSymbol.EndAttribute.ToString();
			}

			return typeName;
		}

		/// <summary>
		/// Captures an alias or keyword from the stream.
		/// </summary>
		/// <returns>Captured word.</returns>
		private string CaptureWord()
		{
			return CaptureWord(false);
		}

		/// <summary>
		/// Captures an alias or keyword from the stream.
		/// </summary>
		/// <param name="captureGeneric">
		/// Whether or not to capture generic symbols, such as in type names.
		/// </param>
		/// <returns>Captured word.</returns>
		private string CaptureWord(bool captureGeneric)
		{
			EatWhiteSpace();

			StringBuilder word = new StringBuilder(DefaultWordLength);

			char nextChar = NextChar;
			while (nextChar != EmptyChar)
			{
				if (IsWhiteSpace(nextChar) ||
					(IsAliasBreak(nextChar) &&
					!(nextChar == CSharpSymbol.TypeImplements && (word.ToString() == CSharpKeyword.Global || word.ToString() == CSharpKeyword.Global + CSharpSymbol.TypeImplements.ToString()))) ||
					(!captureGeneric &&
					(nextChar == CSharpSymbol.BeginGeneric ||
					nextChar == CSharpSymbol.EndGeneric)))
				{
					break;
				}
				else
				{
					TryReadChar();
					word.Append(CurrentChar);
					nextChar = NextChar;
				}
			}

			return word.ToString();
		}

		/// <summary>
		/// Eats a trailing end of statement.
		/// </summary>
		private void EatTrailingEndOfStatement()
		{
			EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);
			if (NextChar == CSharpSymbol.EndOfStatement)
			{
				EatChar(CSharpSymbol.EndOfStatement);
			}
		}

		/// <summary>
		/// Parses an alias list, such as for parameters.
		/// </summary>
		/// <returns>Alias array.</returns>
		private string[] ParseAliasList()
		{
			List<string> aliases = new List<string>();

			EatWhiteSpace();

			char nextChar = NextChar;
			if (nextChar == CSharpSymbol.BeginBlock)
			{
				this.OnParseError("Expected a class or interface name");
			}
			else
			{
				while (nextChar != EmptyChar && nextChar != CSharpSymbol.BeginBlock)
				{
					string alias = CaptureWord(false);

					EatWhiteSpace();

					nextChar = NextChar;
					if (nextChar == CSharpSymbol.BeginGeneric)
					{
						alias += CSharpSymbol.BeginGeneric.ToString() +
							ParseNestedText(CSharpSymbol.BeginGeneric, CSharpSymbol.EndGeneric, true, true) +
							CSharpSymbol.EndGeneric.ToString();
					}

					if (alias == CSharpKeyword.New)
					{
						// new(), for type parameter constraint lists
						if (TryReadChar(CSharpSymbol.BeginParameterList) &&
							TryReadChar(CSharpSymbol.EndParameterList))
						{
							alias = CSharpKeyword.NewConstraint;
						}
						else
						{
							this.OnParseError("Invalid new constraint, use new()");
						}
					}

					aliases.Add(alias);

					EatWhiteSpace();

					nextChar = NextChar;
					if (nextChar != CSharpSymbol.AliasSeparator)
					{
						break;
					}
					else
					{
						TryReadChar();
					}
				}
			}

			return aliases.ToArray();
		}

		/// <summary>
		/// Parses an attribute.
		/// </summary>
		/// <param name="comments">Comments to apply.</param>
		/// <returns>Attribute code element.</returns>
		private AttributeElement ParseAttribute(ReadOnlyCollection<ICommentElement> comments)
		{
			return ParseAttribute(comments, false);
		}

		/// <summary>
		/// Parses an attribute.
		/// </summary>
		/// <param name="comments">Comments to apply.</param>
		/// <param name="nested">Whether or not this a a nested/chained attribute.</param>
		/// <returns>Attribute element.</returns>
		private AttributeElement ParseAttribute(ReadOnlyCollection<ICommentElement> comments, bool nested)
		{
			AttributeElement attributeElement = new AttributeElement();

			string typeName = CaptureTypeName(false);
			EatWhiteSpace();

			//
			// Check for an attribute target
			//
			if (TryReadChar(CSharpSymbol.TypeImplements))
			{
				attributeElement.Target = typeName;
				typeName = CaptureTypeName(false);
				EatWhiteSpace();
			}

			attributeElement.Name = typeName;

			if (NextChar == CSharpSymbol.BeginParameterList)
			{
				string attributeText = ParseNestedText(
					CSharpSymbol.BeginParameterList, CSharpSymbol.EndParameterList, true, false);
				attributeElement.BodyText = attributeText;
			}

			EatWhiteSpace();

			while (!nested && TryReadChar(CSharpSymbol.AliasSeparator))
			{
				if (NextChar != CSharpSymbol.AliasSeparator)
				{
					AttributeElement childAttributeElement = ParseAttribute(null, true);
					if (string.IsNullOrEmpty(childAttributeElement.Target))
					{
						childAttributeElement.Target = attributeElement.Target;
					}
					attributeElement.AddChild(childAttributeElement);
				}
			}

			EatWhiteSpace();

			if (!nested)
			{
				EatChar(CSharpSymbol.EndAttribute);
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
		/// Parses a member block.
		/// </summary>
		/// <param name="beginExpected">Begin block character expected.</param>
		/// <param name="parentElement">Element to parse the block for.</param>
		/// <returns>Block text.</returns>
		private string ParseBlock(bool beginExpected, CommentedElement parentElement)
		{
			List<ICommentElement> extraComments = new List<ICommentElement>();

			if (beginExpected)
			{
				// TODO: Assign any parsed comments to the parent element
				extraComments.AddRange(ParseComments());

				if (parentElement != null)
				{
					foreach (ICommentElement comment in extraComments)
					{
						parentElement.AddHeaderComment(comment);
					}
				}
			}

			return ParseNestedText(CSharpSymbol.BeginBlock, CSharpSymbol.EndBlock, beginExpected, true);
		}

		/// <summary>
		/// Parses a comment block.
		/// </summary>
		/// <returns>Comment code element.</returns>
		private CommentElement ParseCommentBlock()
		{
			TryReadChar();
			TryReadChar();
			TryReadChar();

			StringBuilder blockComment = new StringBuilder(DefaultBlockLength);

			while (!(PreviousChar == CSharpSymbol.BlockCommentModifier &&
				CurrentChar == CSharpSymbol.BeginComment))
			{
				blockComment.Append(PreviousChar);
				TryReadChar();
			}

			return new CommentElement(blockComment.ToString(), CommentType.Block);
		}

		/// <summary>
		/// Parses a comment line.
		/// </summary>
		/// <returns>Comment code element.</returns>
		private CommentElement ParseCommentLine()
		{
			CommentElement commentLine;
			TryReadChar();

			CommentType commentType = CommentType.Line;
			if (NextChar == CSharpSymbol.BeginComment)
			{
				commentType = CommentType.XmlLine;
				TryReadChar();
			}

			string commentText = ReadLine();
			commentLine = new CommentElement(commentText, commentType);
			return commentLine;
		}

		/// <summary>
		/// Parses comments from the current position in the reader.
		/// </summary>
		/// <returns>Collection of comments.</returns>
		private ReadOnlyCollection<ICommentElement> ParseComments()
		{
			EatWhiteSpace();

			List<ICommentElement> comments = new List<ICommentElement>();

			char nextChar = NextChar;
			while (nextChar == CSharpSymbol.BeginComment)
			{
				TryReadChar();

				nextChar = NextChar;
				if (nextChar == CSharpSymbol.BeginComment)
				{
					CommentElement commentLine = ParseCommentLine();
					comments.Add(commentLine);
				}
				else if (nextChar == CSharpSymbol.BlockCommentModifier)
				{
					CommentElement commentBlock = ParseCommentBlock();
					comments.Add(commentBlock);
				}
				else
				{
					this.OnParseError(
						string.Format(
						Thread.CurrentThread.CurrentCulture,
						"Invalid character '{0}'",
						CSharpSymbol.BeginComment));
				}

				EatWhiteSpace();

				nextChar = NextChar;
			}

			EatWhiteSpace();

			return comments.AsReadOnly();
		}

		/// <summary>
		/// Parses a condition directive.
		/// </summary>
		/// <param name="line">Directive line text.</param>
		/// <param name="isIf">Whether or not the condition directive is an if directive.</param>
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

			string condition = null;
			if (separatorIndex > 0)
			{
				condition = line.Substring(separatorIndex + 1).Trim();
			}

			isIf = directive == CSharpKeyword.If;

			switch (directive)
			{
				case CSharpKeyword.If:
				case CSharpKeyword.Elif:
					if (string.IsNullOrEmpty(condition))
					{
						this.OnParseError("Expected a condition expression");
					}
					break;

				case CSharpKeyword.Else:
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
		/// Parses a constructor.
		/// </summary>
		/// <param name="memberName">Member name.</param>
		/// <param name="access">Member accessibility.</param>
		/// <param name="memberAttributes">Member attributes.</param>
		/// <returns>Constructor code element.</returns>
		private ConstructorElement ParseConstructor(string memberName, CodeAccess access, MemberModifiers memberAttributes)
		{
			ConstructorElement constructor = new ConstructorElement();
			constructor.Name = memberName;
			constructor.Access = access;
			constructor.MemberModifiers = memberAttributes;

			constructor.Parameters = this.ParseParameters();

			EatWhiteSpace();

			List<ICommentElement> extraComments = new List<ICommentElement>();
			extraComments.AddRange(ParseComments());

			EatWhiteSpace();

			bool hasReference = TryReadChar(CSharpSymbol.TypeImplements);
			if (hasReference)
			{
				EatWhiteSpace();

				extraComments.AddRange(ParseComments());

				StringBuilder referenceBuilder = new StringBuilder(DefaultWordLength);

				EatWhiteSpace();
				referenceBuilder.Append(CaptureWord());

				EatWhiteSpace();
				string referenceParams =
					ParseNestedText(CSharpSymbol.BeginParameterList, CSharpSymbol.EndParameterList, true, true);
				referenceBuilder.Append(CSharpSymbol.BeginParameterList);
				referenceBuilder.Append(referenceParams);
				referenceBuilder.Append(CSharpSymbol.EndParameterList);

				constructor.Reference = referenceBuilder.ToString();
			}

			constructor.BodyText = this.ParseBlock(true, constructor);

			foreach (ICommentElement comment in extraComments)
			{
				constructor.AddHeaderComment(comment);
			}

			return constructor;
		}

		/// <summary>
		/// Parses a delegate.
		/// </summary>
		/// <param name="memberName">Member name</param>
		/// <param name="access">Code access</param>
		/// <param name="memberAttributes">Member attributes</param>
		/// <param name="returnType">Return type</param>
		/// <returns>A delegate code element.</returns>
		private DelegateElement ParseDelegate(
			string memberName, CodeAccess access, MemberModifiers memberAttributes, string returnType)
		{
			DelegateElement delegateElement = new DelegateElement();
			delegateElement.Name = memberName;
			delegateElement.Access = access;
			delegateElement.Type = returnType;
			delegateElement.MemberModifiers = memberAttributes;

			int genericIndex = memberName.IndexOf(CSharpSymbol.BeginGeneric);
			bool isGeneric = genericIndex >= 0 && genericIndex < memberName.Length - 1;
			if (isGeneric)
			{
				delegateElement.Name = memberName.Substring(0, genericIndex);
				string typeParameterString = memberName.TrimEnd(CSharpSymbol.EndGeneric).Substring(
					genericIndex + 1);

				string[] typeParameterNames = typeParameterString.Split(
					new char[] { CSharpSymbol.AliasSeparator, ' ' },
					StringSplitOptions.RemoveEmptyEntries);
				foreach (string typeParameterName in typeParameterNames)
				{
					TypeParameter typeParameter = new TypeParameter();
					typeParameter.Name = typeParameterName;
					delegateElement.AddTypeParameter(typeParameter);
				}
			}

			delegateElement.Parameters = this.ParseParameters();

			if (isGeneric)
			{
				ParseTypeParameterConstraints(delegateElement);
			}

			EatChar(CSharpSymbol.EndOfStatement);

			return delegateElement;
		}

		/// <summary>
		/// Parses a collection of elements.
		/// </summary>
		/// <param name="parentElement">Parent element for context.</param>
		/// <returns>Parsed code elements.</returns>
		private List<ICodeElement> ParseElements(ICodeElement parentElement)
		{
			List<ICodeElement> codeElements = new List<ICodeElement>();
			List<ICodeElement> siblingElements = new List<ICodeElement>();
			List<ICommentElement> comments = new List<ICommentElement>();
			List<AttributeElement> attributes = new List<AttributeElement>();
			Stack<ICodeElement> enclosingElementStack = new Stack<ICodeElement>();

			StringBuilder elementBuilder = new StringBuilder(DefaultBlockLength);

			char nextChar;

			while (TryReadChar())
			{
				switch (CurrentChar)
				{
					//
					// Comments
					//
					case CSharpSymbol.BeginComment:
						nextChar = NextChar;
						if (nextChar == CSharpSymbol.BeginComment)
						{
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
						}
						else if (nextChar == CSharpSymbol.BlockCommentModifier)
						{
							CommentElement commentBlock = ParseCommentBlock();
							comments.Add(commentBlock);
						}
						else
						{
							elementBuilder.Append(CurrentChar);
						}
						break;

					//
					// Preprocessor
					//
					case CSharpSymbol.Preprocessor:
						//
						// TODO: Parse pragma directives
						//
						string line = ReadLine().Trim();
						if (line.StartsWith(CSharpKeyword.Region, StringComparison.Ordinal))
						{
							PushComments(codeElements, comments);

							RegionElement regionElement = ParseRegion(line);
							enclosingElementStack.Push(regionElement);
						}
						else if (line.StartsWith(CSharpKeyword.If, StringComparison.Ordinal) ||
							line.StartsWith(CSharpKeyword.Else, StringComparison.Ordinal) ||
							line.StartsWith(CSharpKeyword.Elif, StringComparison.Ordinal))
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
									this.OnParseError("Expected 'if' preprocessor directive.");
								}
								else
								{
									ConditionDirectiveElement previousCondition =
										enclosingElementStack.Peek() as ConditionDirectiveElement;
									while (previousCondition.ElseCondition != null)
									{
										previousCondition = previousCondition.ElseCondition;
									}

									//
									// Add the condition to the end of the condition linked list
									//
									previousCondition.ElseCondition = conditionDirective;
								}
							}
						}
						else if (line.StartsWith(CSharpKeyword.EndRegion, StringComparison.Ordinal) ||
							line.StartsWith(CSharpKeyword.EndIf, StringComparison.Ordinal))
						{
							ICodeElement enclosingElement = null;

							//
							// If we don't have an element of the same type on the stack, then
							// we've got a mismatch for the closing directive.
							//
							if (line.StartsWith(CSharpKeyword.EndRegion, StringComparison.Ordinal))
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
								this.OnParseError("Unmatched #endif");
							}

							enclosingElement = enclosingElementStack.Pop();

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
							// If there are any attributes not associated with an element (e.g.
							// a condition directive containing only an attribute, then
							// throw an error as this is currently not supported.
							//
							if (enclosingElement.ElementType == ElementType.ConditionDirective &&
								attributes.Count > 0)
							{
								this.OnParseError("Cannot arrange files with preprocessor directives containing attributes unassociated to an element");
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
							this.OnParseError(
								"Cannot arrange files with preprocessor directives " +
								"other than #region, #endregion and conditional compilation directives");
						}
						break;

					//
					// Attribute
					//
					case CSharpSymbol.BeginAttribute:
						nextChar = NextChar;

						//
						// Parse array definition
						//
						if (elementBuilder.Length > 0)
						{
							EatWhiteSpace();
							nextChar = NextChar;

							if (nextChar == CSharpSymbol.EndAttribute)
							{
								// Array type
								EatChar(CSharpSymbol.EndAttribute);

								elementBuilder.Append(CSharpSymbol.BeginAttribute);
								elementBuilder.Append(CSharpSymbol.EndAttribute);
								elementBuilder.Append(' ');
							}
							else
							{
								string nestedText = ParseNestedText(
									CSharpSymbol.BeginAttribute,
									CSharpSymbol.EndAttribute,
									false,
									true);

								elementBuilder.Append(CSharpSymbol.BeginAttribute);
								elementBuilder.Append(nestedText);
								elementBuilder.Append(CSharpSymbol.EndAttribute);
							}
						}
						else
						{
							//
							// Parse attribute
							//
							AttributeElement attributeElement = ParseAttribute(comments.AsReadOnly());

							attributes.Add(attributeElement);
							codeElements.Add(attributeElement);
							comments.Clear();
						}
						break;

					//
					// Trim generics
					//
					case CSharpSymbol.BeginGeneric:
						string elementText = elementBuilder.ToString();
						if (elementBuilder.Length > 0 &&
							!(elementText.Trim().EndsWith(CSharpKeyword.Operator, StringComparison.Ordinal)))
						{
							string nestedText = ParseNestedText(
								CSharpSymbol.BeginGeneric,
								CSharpSymbol.EndGeneric,
								false,
								true);

							//
							// Trim whitespace preceding type parameters
							//
							TrimTrailingWhiteSpace(elementBuilder);

							elementBuilder.Append(CSharpSymbol.BeginGeneric);
							elementBuilder.Append(nestedText);
							elementBuilder.Append(CSharpSymbol.EndGeneric);
						}
						else
						{
							elementBuilder.Append(CurrentChar);
						}
						break;

					case CSharpSymbol.Nullable:
						TrimTrailingWhiteSpace(elementBuilder);
						elementBuilder.Append(CurrentChar);
						break;

					// Eat any unneeded whitespace
					case ' ':
					case '\n':
					case '\r':
					case '\t':
						if (elementBuilder.Length > 0 &&
							elementBuilder[elementBuilder.Length - 1] != ' ')
						{
							elementBuilder.Append(' ');
						}
						break;

					default:
						elementBuilder.Append(CurrentChar);
						nextChar = NextChar;

						if (char.IsWhiteSpace(nextChar) || CSharpSymbol.IsCSharpSymbol(CurrentChar))
						{
							//
							// Try to parse a code element
							//
							ICodeElement element = TryParseElement(
								parentElement,
								elementBuilder,
								comments.AsReadOnly(),
								attributes.AsReadOnly());
							if (element != null)
							{
								// Since more than one field can be declared per line, we need special
								// handling here.
								FieldElement fieldElement = element as FieldElement;
								if (fieldElement != null)
								{
									if (fieldElement.Name.Contains(CSharpSymbol.AliasSeparator.ToString()))
									{
										string[] fieldNames = fieldElement.Name.Split(
											new char[] { CSharpSymbol.AliasSeparator }, StringSplitOptions.RemoveEmptyEntries);
										for (int fieldIndex = 0; fieldIndex < fieldNames.Length; fieldIndex++)
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

						break;
				}

				char nextCh = NextChar;

				//
				// Elements should capture closing block characters
				//
				if (nextCh == CSharpSymbol.EndBlock)
				{
					break;
				}
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
			// Make sure that all region elements and preprocessor directives have been closed
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
					this.OnParseError("Expected #endif");
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
		/// <param name="access">Member accessibility.</param>
		/// <param name="memberAttributes">Member modifiers.</param>
		/// <returns>Event code element.</returns>
		private EventElement ParseEvent(CodeAccess access, MemberModifiers memberAttributes)
		{
			EventElement eventElement = new EventElement();

			StringBuilder eventSignature = new StringBuilder();
			while (NextChar != CSharpSymbol.EndOfStatement &&
				NextChar != CSharpSymbol.BeginBlock)
			{
				if (TryReadChar())
				{
					eventSignature.Append(CurrentChar);
				}
			}

			string[] words = eventSignature.ToString().Split(WhiteSpaceCharacters, StringSplitOptions.RemoveEmptyEntries);
			StringCollection wordList = new StringCollection();
			wordList.AddRange(words);
			string name = null;
			string type = null;

			GetMemberNameAndType(wordList, out name, out type);

			eventElement.Type = type;
			eventElement.Name = name;
			eventElement.Access = access;
			eventElement.MemberModifiers = memberAttributes;

			EatWhiteSpace();

			char nextChar = NextChar;
			if (nextChar == CSharpSymbol.EndOfStatement)
			{
				EatChar(CSharpSymbol.EndOfStatement);
			}
			else
			{
				eventElement.BodyText = this.ParseBlock(true, eventElement);
			}

			return eventElement;
		}

		/// <summary>
		/// Parses a field.
		/// </summary>
		/// <param name="isAssignment">Has field assignment.</param>
		/// <param name="access">Field access.</param>
		/// <param name="memberAttributes">The member attributes.</param>
		/// <param name="memberName">Name of the member.</param>
		/// <param name="returnType">Return type.</param>
		/// <param name="isVolatile">Whether or not the field is volatile.</param>
		/// <param name="isFixed">Whether or not the field is fixed.</param>
		/// <returns>A field code element.</returns>
		private FieldElement ParseField(
			bool isAssignment,
			CodeAccess access,
			MemberModifiers memberAttributes,
			string memberName,
			string returnType,
			bool isVolatile,
			bool isFixed)
		{
			FieldElement field = new FieldElement();
			field.Name = memberName;
			field.Type = returnType;
			field.Access = access;
			field.MemberModifiers = memberAttributes;
			field.IsVolatile = isVolatile;
			field[CSharpExtendedProperties.Fixed] = isFixed;

			if (isAssignment)
			{
				string initialValue = ParseInitialValue();
				field.InitialValue = initialValue;
			}

			EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);
			if (NextChar == CSharpSymbol.BeginComment)
			{
				EatChar(CSharpSymbol.BeginComment);
				if (NextChar == CSharpSymbol.BeginComment)
				{
					field.TrailingComment = ParseCommentLine();
				}
				else if (NextChar == CSharpSymbol.BlockCommentModifier)
				{
					field.TrailingComment = ParseCommentBlock();
				}
			}
			return field;
		}

		/// <summary>
		/// Parses an initial value, such as for a field.
		/// </summary>
		/// <returns>Initial value text.</returns>
		private string ParseInitialValue()
		{
			EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);

			string initialValue = ParseNestedText(EmptyChar, CSharpSymbol.EndOfStatement, false, false);

			if (string.IsNullOrEmpty(initialValue))
			{
				this.OnParseError("Expected an initial value");
			}

			return initialValue;
		}

		/// <summary>
		/// Parses a method.
		/// </summary>
		/// <param name="memberName">Member name.</param>
		/// <param name="access">Code access.</param>
		/// <param name="memberAttributes">Member attributes.</param>
		/// <param name="returnType">Return type.</param>
		/// <param name="isOperator">Whether or not the method is an operator.</param>
		/// <param name="operatorType">Type of the operator.</param>
		/// <returns>Method code element.</returns>
		private MethodElement ParseMethod(
			string memberName,
			CodeAccess access,
			MemberModifiers memberAttributes,
			string returnType,
			bool isOperator,
			OperatorType operatorType,
			bool isAsync)
		{
			MethodElement method = new MethodElement();
			method.Name = memberName;
			method.Access = access;
			method.Type = returnType;
			method.MemberModifiers = memberAttributes;
			method.IsOperator = isOperator;
			method.OperatorType = operatorType;
			method.IsAsync = isAsync;
			if (isOperator &&
				(operatorType == OperatorType.Implicit || operatorType == OperatorType.Explicit))
			{
				method.Type = memberName;
				method.Name = null;
			}

			int genericIndex = memberName.LastIndexOf(CSharpSymbol.BeginGeneric);
			int lastQualifierIndex = memberName.LastIndexOf(CSharpSymbol.AliasQualifier);
			bool isGeneric = !isOperator &&
				(genericIndex >= 0 && genericIndex < memberName.Length - 1 &&
				(lastQualifierIndex < 0 || lastQualifierIndex < genericIndex));
			if (isGeneric)
			{
				method.Name = memberName.Substring(0, genericIndex);
				string typeParameterString = memberName.TrimEnd(CSharpSymbol.EndGeneric).Substring(
					genericIndex + 1);

				string[] typeParameterNames = typeParameterString.Split(
					new char[] { CSharpSymbol.AliasSeparator, ' ' },
					StringSplitOptions.RemoveEmptyEntries);
				foreach (string typeParameterName in typeParameterNames)
				{
					TypeParameter typeParameter = new TypeParameter();
					typeParameter.Name = typeParameterName;
					method.AddTypeParameter(typeParameter);
				}
			}

			method.Parameters = this.ParseParameters();

			if (isGeneric)
			{
				ParseTypeParameterConstraints(method);
			}

			EatWhiteSpace();
			bool endOfStatement = NextChar == CSharpSymbol.EndOfStatement;
			if (endOfStatement)
			{
				TryReadChar();
				method.BodyText = null;
			}
			else
			{
				method.BodyText = this.ParseBlock(true, method);
			}

			return method;
		}

		/// <summary>
		/// Parses a namespace definition.
		/// </summary>
		/// <returns>Namespace code element.</returns>
		private NamespaceElement ParseNamespace()
		{
			NamespaceElement namespaceElement = new NamespaceElement();
			string namepaceName = CaptureWord();
			namespaceElement.Name = namepaceName;

			EatChar(CSharpSymbol.BeginBlock);

			EatWhiteSpace();

			if (NextChar != CSharpSymbol.EndBlock)
			{
				//
				// Parse child elements
				//
				List<ICodeElement> childElements = ParseElements(namespaceElement);
				foreach (ICodeElement childElement in childElements)
				{
					namespaceElement.AddChild(childElement);
				}
			}

			EatChar(CSharpSymbol.EndBlock);

			//
			// Namespaces allow a trailing semi-colon
			//
			EatTrailingEndOfStatement();

			return namespaceElement;
		}

		/// <summary>
		/// Parses nested text.
		/// </summary>
		/// <param name="beginChar">The begin char.</param>
		/// <param name="endChar">The end char.</param>
		/// <param name="beginExpected">Whether or not the begin char is expected.</param>
		/// <param name="trim">Whether or not the parsed text should be trimmed.</param>
		/// <returns>The parsed text.</returns>
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
				bool inCharLiteral = false;
				bool inLineComment = false;
				bool inBlockComment = false;
				bool inVerbatimString = false;
				bool escaped = false;

				while (depth > 0)
				{
					char previousPreviousChar = PreviousChar;

					bool charRead = TryReadChar();
					if (!charRead)
					{
						this.OnParseError("Unexpected end of file. Expected " + endChar);
					}

					nextChar = NextChar;

					bool inComment = inBlockComment || inLineComment;

					if (!inComment)
					{
						escaped = !escaped && PreviousChar == EscapeChar;

						if (!inCharLiteral && CurrentChar == CSharpSymbol.BeginString
							&& (inVerbatimString || !escaped))
						{
							inString = !inString;
							if (!inString)
							{
								escaped = false;
							}
							inVerbatimString = inString && PreviousChar == CSharpSymbol.BeginVerbatimString;
						}
						else if (!inString && CurrentChar == CSharpSymbol.BeginCharLiteral
							&& !escaped)
						{
							inCharLiteral = !inCharLiteral;
							if (!inCharLiteral)
							{
								escaped = false;
							}
						}
					}

					if (!inCharLiteral && !inString)
					{
						if (!inBlockComment && CurrentChar == CSharpSymbol.BeginComment &&
							nextChar == CSharpSymbol.BeginComment)
						{
							inLineComment = true;
						}
						else if (inLineComment && ((CurrentChar == Environment.NewLine[0] &&
							(Environment.NewLine.Length == 1 || nextChar == Environment.NewLine[1])) ||
							CurrentChar == '\n'))
						{
							inLineComment = false;
						}
						else if (!inLineComment && !inBlockComment &&
							CurrentChar == CSharpSymbol.BeginComment &&
							nextChar == CSharpSymbol.BlockCommentModifier)
						{
							inBlockComment = true;
						}
						else if (inBlockComment &&
							CurrentChar == CSharpSymbol.BlockCommentModifier &&
							nextChar == CSharpSymbol.BeginComment)
						{
							inBlockComment = false;
						}
					}

					inComment = inBlockComment || inLineComment;
					if (beginChar != EmptyChar && CurrentChar == beginChar &&
						!inCharLiteral && !inString && !inComment)
					{
						blockText.Append(CurrentChar);
						depth++;
					}
					else
					{
						blockText.Append(CurrentChar);
					}

					if (nextChar == endChar && !inString && !inCharLiteral && !inComment)
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
		/// Parses a parameter list.
		/// </summary>
		/// <returns>A comma-separated string of parameters.</returns>
		private string ParseParameters()
		{
			return ParseNestedText(CSharpSymbol.BeginParameterList, CSharpSymbol.EndParameterList, false, false);
		}

		/// <summary>
		/// Parses a property.
		/// </summary>
		/// <param name="memberName">Name of the member.</param>
		/// <param name="returnType">Type of the return.</param>
		/// <param name="access">The access.</param>
		/// <param name="memberAttributes">The member attributes.</param>
		/// <returns>A property code element.</returns>
		private PropertyElement ParseProperty(string memberName, string returnType, CodeAccess access, MemberModifiers memberAttributes)
		{
			PropertyElement property = new PropertyElement();

			int indexStart = memberName.IndexOf(CSharpSymbol.BeginAttribute);
			if (indexStart >= 0)
			{
				string indexParameter = memberName.Substring(indexStart).Trim().Trim(
					CSharpSymbol.BeginAttribute, CSharpSymbol.EndAttribute).Trim();
				property.IndexParameter = indexParameter;
				memberName = memberName.Substring(0, indexStart);
			}

			property.Name = memberName;
			property.Access = access;
			property.Type = returnType;
			property.MemberModifiers = memberAttributes;

			property.BodyText = this.ParseBlock(false, property);

			return property;
		}

		/// <summary>
		/// Parses a type definition.
		/// </summary>
		/// <param name="access">Type accessibility.</param>
		/// <param name="typeAttributes">Type modifiers.</param>
		/// <param name="elementType">Type element type.</param>
		/// <returns>Type code element.</returns>
		private TypeElement ParseType(
			CodeAccess access,
			TypeModifiers typeAttributes,
			TypeElementType elementType)
		{
			TypeElement typeElement = new TypeElement();

			EatWhiteSpace();
			string className = CaptureWord();
			typeElement.Name = className;
			typeElement.Access = access;
			typeElement.Type = elementType;
			typeElement.TypeModifiers = typeAttributes;

			EatWhiteSpace();

			if (elementType == TypeElementType.Enum)
			{
				EatWhiteSpace();

				if (NextChar == CSharpSymbol.TypeImplements)
				{
					TryReadChar();
					string interfaceName = CaptureTypeName();
					InterfaceReference interfaceReference =
						new InterfaceReference(interfaceName, InterfaceReferenceType.None);
					typeElement.AddInterface(interfaceReference);
				}

				string enumText = ParseBlock(true, typeElement);

				// TODO: Parse enum values as fields
				typeElement.BodyText = enumText;
			}
			else
			{
				bool isGeneric = TryReadChar(CSharpSymbol.BeginGeneric);
				if (isGeneric)
				{
					string[] typeParameterNames = ParseAliasList();
					foreach (string typeParameterName in typeParameterNames)
					{
						TypeParameter typeParameter = new TypeParameter();
						typeParameter.Name = typeParameterName;
						typeElement.AddTypeParameter(typeParameter);
					}

					EatWhiteSpace();

					if (!TryReadChar(CSharpSymbol.EndGeneric))
					{
						this.OnParseError("Expected " + CSharpSymbol.EndGeneric);
					}
				}

				EatWhiteSpace();

				bool implements = TryReadChar(CSharpSymbol.TypeImplements);

				if (implements)
				{
					string[] typeList = ParseAliasList();
					foreach (string type in typeList)
					{
						InterfaceReference interfaceReference =
							new InterfaceReference(type, InterfaceReferenceType.None);
						typeElement.AddInterface(interfaceReference);
					}
				}

				EatWhiteSpace();

				ParseTypeParameterConstraints(typeElement);

				// Associate any additional comments in the type definition with the type.
				ReadOnlyCollection<ICommentElement> extraComments = ParseComments();
				foreach (ICommentElement comment in extraComments)
				{
					typeElement.AddHeaderComment(comment);
				}

				EatChar(CSharpSymbol.BeginBlock);

				EatWhiteSpace();

				if (NextChar != CSharpSymbol.EndBlock)
				{
					//
					// Parse child elements
					//
					List<ICodeElement> childElements = ParseElements(typeElement);
					foreach (ICodeElement childElement in childElements)
					{
						typeElement.AddChild(childElement);
					}
				}

				EatChar(CSharpSymbol.EndBlock);
			}

			//
			// Types allow a trailing semi-colon
			//
			EatTrailingEndOfStatement();

			return typeElement;
		}

		/// <summary>
		/// Parses type parameter constraints and adds them to the
		/// generic element.
		/// </summary>
		/// <param name="genericElement">The generic element.</param>
		private void ParseTypeParameterConstraints(IGenericElement genericElement)
		{
			EatWhiteSpace();

			if (NextChar == CSharpKeyword.Where[0])
			{
				List<ICommentElement> extraComments = new List<ICommentElement>();

				while (genericElement.TypeParameters.Count > 0 &&
					NextChar != CSharpSymbol.BeginBlock &&
					NextChar != CSharpSymbol.EndOfStatement)
				{
					//
					// Parse type parameter constraints
					//
					string keyWord = CaptureWord();
					if (keyWord == CSharpKeyword.Where)
					{
						extraComments.AddRange(ParseComments());

						string parameterName = CaptureWord();

						TypeParameter parameter = null;
						foreach (TypeParameter typeParameter in genericElement.TypeParameters)
						{
							if (typeParameter.Name == parameterName)
							{
								parameter = typeParameter;
								break;
							}
						}

						if (parameter == null)
						{
							this.OnParseError("Unknown type parameter '" + parameterName + "'");
						}

						extraComments.AddRange(ParseComments());

						bool separatorFound = TryReadChar(CSharpSymbol.TypeImplements);
						if (!separatorFound)
						{
							this.OnParseError("Expected " + CSharpSymbol.TypeImplements);
						}

						string[] typeList = ParseAliasList();
						foreach (string type in typeList)
						{
							parameter.AddConstraint(type);
						}

						int newIndex = parameter.Constraints.IndexOf(
							CSharpKeyword.NewConstraint);
						if (newIndex >= 0 && newIndex + 1 != parameter.Constraints.Count)
						{
							this.OnParseError("The " + CSharpKeyword.NewConstraint +
								" must be the last declared type parameter constraint");
						}
					}
					else
					{
						this.OnParseError("Expected type parameter constraint");
					}

					extraComments.AddRange(ParseComments());
				}

				CommentedElement commentedElement = genericElement as CommentedElement;
				if (commentedElement != null)
				{
					foreach (ICommentElement comment in extraComments)
					{
						commentedElement.AddHeaderComment(comment);
					}
				}
			}
		}

		/// <summary>
		/// Parses a using directive code element.
		/// </summary>
		/// <returns>Using directive code element.</returns>
		private UsingElement ParseUsing()
		{
			UsingElement usingElement = new UsingElement();
			string alias = CaptureWord();
			if (string.IsNullOrEmpty(alias))
			{
				this.OnParseError("Expected a namepace name");
			}

			EatWhiteSpace();

			bool endOfStatement = TryReadChar(CSharpSymbol.EndOfStatement);
			if (endOfStatement)
			{
				usingElement.Name = alias;
			}
			else
			{
				bool assign = TryReadChar(CSharpSymbol.Assignment);
				if (!assign)
				{
					this.OnParseError(
						string.Format(
						Thread.CurrentThread.CurrentCulture,
						"Expected {0} or {1}.",
						CSharpSymbol.Assignment,
						CSharpSymbol.EndOfStatement));
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
						EatWhiteSpace();

						char nextChar = NextChar;
						if (nextChar == CSharpSymbol.BeginGeneric)
						{
							name += CSharpSymbol.BeginGeneric.ToString() +
								ParseNestedText(CSharpSymbol.BeginGeneric, CSharpSymbol.EndGeneric, true, true) +
								CSharpSymbol.EndGeneric.ToString();
						}
						usingElement.Name = alias;
						usingElement.Redefine = name;
						EatChar(CSharpSymbol.EndOfStatement);
					}
				}
			}

			// C# supports moving of using elements from file to namespace level
			// and vice versa
			usingElement.IsMovable = true;

			return usingElement;
		}

		/// <summary>
		/// Tries to parse a code element.
		/// </summary>
		/// <param name="parentElement">Parent element for context.</param>
		/// <param name="elementBuilder">StringBuilder containing the unprocessed text.</param>
		/// <param name="comments">Comments to apply to the element.</param>
		/// <param name="attributes">Attributes to apply to the element.</param>
		/// <returns>Code element if succesful, otherwise null.</returns>
		private ICodeElement TryParseElement(
			ICodeElement parentElement,
			StringBuilder elementBuilder,
			ReadOnlyCollection<ICommentElement> comments,
			ReadOnlyCollection<AttributeElement> attributes)
		{
			CodeElement codeElement = null;

			string processedElementText =
				elementBuilder.ToString().Trim();

			switch (processedElementText)
			{
				case CSharpKeyword.Namespace:
					codeElement = ParseNamespace();
					break;

				case CSharpKeyword.Using:
					codeElement = ParseUsing();
					break;
			}

			if (codeElement == null)
			{
				string tempElementText = processedElementText;
				bool isOperator = processedElementText.Contains(' ' + CSharpKeyword.Operator);
				if (!isOperator)
				{
					tempElementText = processedElementText.TrimEnd(CSharpSymbol.Assignment);
				}

				string[] words = tempElementText.TrimEnd(
					CSharpSymbol.EndOfStatement,
					CSharpSymbol.BeginParameterList,
					CSharpSymbol.BeginBlock).Split(
					WhiteSpaceCharacters,
					StringSplitOptions.RemoveEmptyEntries);

				char lastChar = processedElementText[processedElementText.Length - 1];
				bool isStatement = lastChar == CSharpSymbol.EndOfStatement;
				bool hasParams = lastChar == CSharpSymbol.BeginParameterList;
				bool isProperty = lastChar == CSharpSymbol.BeginBlock;

				if (words.Length > 0 &&
					(words.Length > 1 ||
					words[0] == CSharpKeyword.Class ||
					words[0] == CSharpKeyword.Structure ||
					words[0] == CSharpKeyword.Interface ||
					words[0] == CSharpKeyword.Enumeration ||
					words[0] == CSharpKeyword.Event ||
					words[0][0] == CSharpSymbol.BeginFinalizer ||
					isStatement || hasParams || isProperty))
				{
					bool isAssignment = !isOperator &&
						lastChar == CSharpSymbol.Assignment &&
						NextChar != CSharpSymbol.Assignment &&
						PreviousChar != CSharpSymbol.Assignment &&
						PreviousChar != CSharpSymbol.Negate;

					StringCollection wordList = new StringCollection();
					int operatorLength = CSharpKeyword.Operator.Length;
					foreach (string word in words)
					{
						if (word.Length > operatorLength &&
							word.StartsWith(CSharpKeyword.Operator, StringComparison.Ordinal) &&
							!char.IsLetterOrDigit(word[operatorLength]))
						{
							wordList.Add(CSharpKeyword.Operator);
							wordList.Add(word.Substring(operatorLength));
						}
						else
						{
							wordList.Add(word);
						}
					}

					ElementType elementType;
					TypeElementType? typeElementType = null;
					if (isProperty)
					{
						elementType = ElementType.Property;
					}
					else
					{
						GetElementType(wordList, out elementType, out typeElementType);
					}

					CodeAccess access = CodeAccess.None;
					MemberModifiers memberAttributes = MemberModifiers.None;
					OperatorType operatorType = OperatorType.None;

					if (isStatement || isAssignment || hasParams ||
						elementType == ElementType.Property ||
						elementType == ElementType.Event ||
						elementType == ElementType.Delegate ||
						elementType == ElementType.Type)
					{
						access = GetAccess(wordList);
						memberAttributes = GetMemberAttributes(wordList);
						operatorType = GetOperatorType(wordList);
					}

					//
					// Type definition?
					//
					if (elementType == ElementType.Type)
					{
						TypeModifiers typeAttributes = (TypeModifiers)memberAttributes;

						//
						// Parse a type definition
						//
						codeElement = ParseType(access, typeAttributes, typeElementType.Value);
					}
					else if (elementType == ElementType.Event)
					{
						codeElement = ParseEvent(access, memberAttributes);
					}

					if (codeElement == null)
					{
						string memberName = null;
						string returnType = null;

						if (isStatement || isAssignment || hasParams || isProperty)
						{
							GetMemberNameAndType(wordList, out memberName, out returnType);
						}

						if (hasParams)
						{
							if (elementType == ElementType.Delegate)
							{
								codeElement = ParseDelegate(memberName, access, memberAttributes, returnType);
							}
							else
							{
								if (returnType == null && operatorType == OperatorType.None)
								{
									//
									// Constructor/finalizer
									//
									codeElement = ParseConstructor(memberName, access, memberAttributes);
								}
								else
								{
									bool isAsync = TryFindAndRemoveWord(wordList, CSharpKeyword.Async);

									//
									// Method
									//
									codeElement = ParseMethod(
										memberName,
										access,
										memberAttributes,
										returnType,
										isOperator,
										operatorType,
										isAsync);
								}
							}
						}
						else if (isStatement || isAssignment)
						{
							//
							// Field
							//
							bool isVolatile = TryFindAndRemoveWord(wordList, CSharpKeyword.Volatile);
							bool isFixed = TryFindAndRemoveWord(wordList, CSharpKeyword.Fixed);
							FieldElement field = ParseField(
								isAssignment,
								access,
								memberAttributes,
								memberName,
								returnType,
								isVolatile,
								isFixed);

							codeElement = field;
						}
						else if (elementType == ElementType.Property)
						{
							codeElement = ParseProperty(memberName, returnType, access, memberAttributes);
						}
					}

					// Check for any unhandled element text
					if (codeElement != null && wordList.Count > 0)
					{
						StringBuilder remainingWords = new StringBuilder();
						foreach (string word in wordList)
						{
							remainingWords.Append(word + " ");
						}

						this.OnParseError(
							string.Format(
							Thread.CurrentThread.CurrentCulture,
							"Unhandled element text '{0}'",
							remainingWords.ToString().Trim()));
					}
				}
			}

			if (codeElement is InterfaceMemberElement || codeElement is TypeElement ||
				codeElement is ConstructorElement || codeElement is NamespaceElement)
			{
				//
				// Eat closing comments
				//
				if (NextChar != EmptyChar)
				{
					EatWhiteSpace(WhiteSpaceTypes.SpaceAndTab);
					if (NextChar == CSharpSymbol.BeginComment)
					{
						EatChar(CSharpSymbol.BeginComment);
						ReadLine();
					}
				}
			}

			if (codeElement != null)
			{
				ApplyCommentsAndAttributes(codeElement, comments, attributes);
			}

			return codeElement;
		}

		#endregion Methods
	}
}