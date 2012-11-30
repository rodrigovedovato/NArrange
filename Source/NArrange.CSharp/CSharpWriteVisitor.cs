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
 *<contributor>Everton Elvio Koser</contributor>
 *<contributor>Justin Dearing</contributor>
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.CSharp
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using System.Threading;

	using NArrange.Core;
	using NArrange.Core.CodeElements;
	using NArrange.Core.Configuration;

	/// <summary>
	/// Visits a tree of code elements for writing C# code 
	/// </summary>
	internal sealed class CSharpWriteVisitor : CodeWriteVisitor
	{
		#region Fields

		/// <summary>
		/// Closing comment prefix.
		/// </summary>
		private const string ClosingCommentPrefix = "// ";

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new CSharpWriteVisitor.
		/// </summary>
		/// <param name="writer">Text writer to write code elements to.</param>
		/// <param name="configuration">Code configuration.</param>
		public CSharpWriteVisitor(TextWriter writer, CodeConfiguration configuration)
			: base(writer, configuration)
		{
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Processes an attribute element.
		/// </summary>
		/// <param name="element">Attribute code element.</param>
		public override void VisitAttributeElement(AttributeElement element)
		{
			this.WriteComments(element.HeaderComments);

			bool nested = element.Parent is AttributeElement;

			if (!nested)
			{
				WriteIndented(CSharpSymbol.BeginAttribute.ToString());
				if (!string.IsNullOrEmpty(element.Target))
				{
					Writer.Write(element.Target);
					Writer.Write(CSharpSymbol.TypeImplements);
					Writer.Write(' ');
				}
			}

			Writer.Write(element.Name);
			if (!string.IsNullOrEmpty(element.BodyText))
			{
				Writer.Write(CSharpSymbol.BeginParameterList);
				Writer.Write(element.BodyText);
				Writer.Write(CSharpSymbol.EndParameterList);
			}

			//
			// Nested list of attributes?
			//
			foreach (ICodeElement childElement in element.Children)
			{
				AttributeElement childAttribute = childElement as AttributeElement;
				if (childAttribute != null)
				{
					Writer.Write(',');
					Writer.WriteLine();
					WriteIndented(string.Empty);
					childAttribute.Accept(this);
				}
			}

			if (!nested)
			{
				Writer.Write(CSharpSymbol.EndAttribute);
			}

			if (!nested && element.Parent != null)
			{
				Writer.WriteLine();
			}
		}

		/// <summary>
		/// Writes a comment line.
		/// </summary>
		/// <param name="comment">Comment code element.</param>
		public override void VisitCommentElement(CommentElement comment)
		{
			StringBuilder builder = new StringBuilder(DefaultBlockLength);

			if (comment.Type == CommentType.Block)
			{
				builder.Append("/*");
				builder.Append(FormatCommentText(comment));
				builder.Append("*/");

				WriteTextBlock(builder.ToString());
			}
			else
			{
				if (comment.Type == CommentType.XmlLine)
				{
					builder.Append("///");
				}
				else
				{
					builder.Append("//");
				}

				builder.Append(FormatCommentText(comment));
				WriteIndented(builder.ToString());
			}
		}

		/// <summary>
		/// Writes a condition directive element.
		/// </summary>
		/// <param name="element">Condition directive code element.</param>
		public override void VisitConditionDirectiveElement(ConditionDirectiveElement element)
		{
			const string ConditionFormat = "{0}{1} {2}";
			ConditionDirectiveElement conditionDirective = element;

			this.WriteIndented(
				string.Format(
				CultureInfo.InvariantCulture,
				ConditionFormat,
				CSharpSymbol.Preprocessor,
				CSharpKeyword.If,
				element.ConditionExpression));

			Writer.WriteLine();
			WriteChildren(element);
			if (element.Children.Count > 0)
			{
				Writer.WriteLine();
			}

			while (conditionDirective.ElseCondition != null)
			{
				conditionDirective = conditionDirective.ElseCondition;

				if (conditionDirective.ElseCondition == null)
				{
					this.WriteIndented(CSharpSymbol.Preprocessor + CSharpKeyword.Else);
				}
				else
				{
					this.WriteIndented(
						string.Format(
						CultureInfo.InvariantCulture,
						ConditionFormat,
						CSharpSymbol.Preprocessor,
						CSharpKeyword.Elif,
						conditionDirective.ConditionExpression));
				}

				Writer.WriteLine();
				WriteChildren(conditionDirective);
				if (element.Children.Count > 0)
				{
					Writer.WriteLine();
				}
			}

			this.WriteIndented(CSharpSymbol.Preprocessor + CSharpKeyword.EndIf);
		}

		/// <summary>
		/// Processes a constructor element.
		/// </summary>
		/// <param name="element">Constructor code element.</param>
		public override void VisitConstructorElement(ConstructorElement element)
		{
			this.WriteComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			Writer.Write(element.Name);

			WriteParameterList(element.Parameters);
			Writer.WriteLine();

			if (element.Reference != null)
			{
				TabCount++;
				WriteIndentedLine(
					string.Format(
					CultureInfo.InvariantCulture,
					"{0} {1}",
					CSharpSymbol.TypeImplements,
					element.Reference));
				TabCount--;
			}

			WriteBody(element);
		}

		/// <summary>
		/// Processes a delegate element.
		/// </summary>
		/// <param name="element">Delegate code element.</param>
		public override void VisitDelegateElement(DelegateElement element)
		{
			this.WriteComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			Writer.Write(CSharpKeyword.Delegate);
			Writer.Write(' ');

			Writer.Write(element.Type);
			Writer.Write(' ');

			Writer.Write(element.Name);

			WriteTypeParameters(element);
			WriteParameterList(element.Parameters);
			WriteTypeParameterConstraints(element);
			Writer.Write(CSharpSymbol.EndOfStatement);
		}

		/// <summary>
		/// Processes an event element.
		/// </summary>
		/// <param name="element">Event code element.</param>
		public override void VisitEventElement(EventElement element)
		{
			this.WriteComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			Writer.Write(CSharpKeyword.Event);
			Writer.Write(' ');

			Writer.Write(element.Type);
			Writer.Write(' ');

			Writer.Write(element.Name);

			if (element.BodyText != null && element.BodyText.Length > 0)
			{
				Writer.WriteLine();
				WriteBody(element);
			}
			else
			{
				Writer.Write(CSharpSymbol.EndOfStatement);
			}
		}

		/// <summary>
		/// Processes a field element.
		/// </summary>
		/// <param name="element">Field code element.</param>
		public override void VisitFieldElement(FieldElement element)
		{
			this.WriteComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			if (element.IsVolatile)
			{
				Writer.Write(CSharpKeyword.Volatile);
				Writer.Write(' ');
			}

			if (element[CSharpExtendedProperties.Fixed] is bool &&
				(bool)element[CSharpExtendedProperties.Fixed])
			{
				Writer.Write(CSharpKeyword.Fixed);
				Writer.Write(' ');
			}

			Writer.Write(element.Type);
			Writer.Write(' ');

			Writer.Write(element.Name);

			if (!string.IsNullOrEmpty(element.InitialValue))
			{
				Writer.Write(' ');
				Writer.Write(CSharpSymbol.Assignment);
				Writer.Write(' ');
				if (element.InitialValue.IndexOf("\n") >= 0)
				{
					string initialValue = element.InitialValue;
					int lineFeedIndex = initialValue.IndexOf('\n');
					if (lineFeedIndex > 0)
					{
						string initialValueFirstLine = initialValue.Substring(0, lineFeedIndex + 1);
						initialValue = initialValue.Substring(lineFeedIndex + 1);

						Writer.Write(initialValueFirstLine);
					}

					WriteTextBlock(initialValue);
				}
				else
				{
					Writer.Write(element.InitialValue);
				}
			}

			Writer.Write(CSharpSymbol.EndOfStatement);

			if (element.TrailingComment != null)
			{
				Writer.Write(' ');
				int tabCountTemp = this.TabCount;
				this.TabCount = 0;
				element.TrailingComment.Accept(this);
				this.TabCount = tabCountTemp;
			}
		}

		/// <summary>
		/// Processes a method element.
		/// </summary>
		/// <param name="element">Method code element.</param>
		public override void VisitMethodElement(MethodElement element)
		{
			this.WriteComments(element.HeaderComments);
			this.WriteAttributes(element);

			if (element.IsPartial)
			{
				Writer.Write(CSharpKeyword.Partial);
				Writer.Write(' ');
			}

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			if (element.OperatorType == OperatorType.None)
			{
				if (element.IsAsync)
				{
					Writer.Write(CSharpKeyword.Async);
					Writer.Write(' ');
				}

				Writer.Write(element.Type);
				Writer.Write(' ');

				if (element.IsOperator)
				{
					Writer.Write(CSharpKeyword.Operator);
					Writer.Write(' ');
				}

				Writer.Write(element.Name);
			}
			else if (element.IsOperator)
			{
				if (element.OperatorType == OperatorType.Explicit)
				{
					Writer.Write(CSharpKeyword.Explicit);
				}
				else if (element.OperatorType == OperatorType.Implicit)
				{
					Writer.Write(CSharpKeyword.Implicit);
				}
				Writer.Write(' ');

				Writer.Write(CSharpKeyword.Operator);
				Writer.Write(' ');
				Writer.Write(element.Type);
			}

			WriteTypeParameters(element);
			WriteParameterList(element.Parameters);
			WriteTypeParameterConstraints(element);

			if (element.BodyText == null)
			{
				Writer.Write(CSharpSymbol.EndOfStatement);
			}
			else
			{
				Writer.WriteLine();
				WriteBody(element);
			}
		}

		/// <summary>
		/// Processes a namespace element.
		/// </summary>
		/// <param name="element">Namespace code element.</param>
		public override void VisitNamespaceElement(NamespaceElement element)
		{
			this.WriteComments(element.HeaderComments);

			StringBuilder builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpKeyword.Namespace);
			builder.Append(' ');
			builder.Append(element.Name);

			WriteIndentedLine(builder.ToString());
			WriteBeginBlock();

			//
			// Process all children
			//
			WriteBlockChildren(element);

			WriteEndBlock();
		}

		/// <summary>
		/// Processes a property element.
		/// </summary>
		/// <param name="element">Property element to process.</param>
		public override void VisitPropertyElement(PropertyElement element)
		{
			this.WriteComments(element.HeaderComments);
			this.WriteAttributes(element);

			WriteAccess(element.Access);

			WriteMemberAttributes(element.MemberModifiers);

			Writer.Write(element.Type);
			Writer.Write(' ');

			Writer.Write(element.Name);
			if (element.IndexParameter != null)
			{
				Writer.Write(CSharpSymbol.BeginAttribute);
				Writer.Write(element.IndexParameter);
				Writer.Write(CSharpSymbol.EndAttribute);
			}

			Writer.WriteLine();

			WriteBody(element);
		}

		/// <summary>
		/// Processes a type element.
		/// </summary>
		/// <param name="element">Type elemenent to process.</param>
		public override void VisitTypeElement(TypeElement element)
		{
			this.WriteComments(element.HeaderComments);
			this.WriteAttributes(element);

			if (element.Access != CodeAccess.None)
			{
				WriteAccess(element.Access);
			}
			else
			{
				WriteIndented(string.Empty);
			}

			if (element.IsNew)
			{
				Writer.Write(CSharpKeyword.New);
				Writer.Write(' ');
			}

			if (element.IsUnsafe)
			{
				Writer.Write(CSharpKeyword.Unsafe);
				Writer.Write(' ');
			}

			if (element.IsStatic)
			{
				Writer.Write(CSharpKeyword.Static);
				Writer.Write(' ');
			}

			if (element.IsSealed)
			{
				Writer.Write(CSharpKeyword.Sealed);
				Writer.Write(' ');
			}

			if (element.IsAbstract)
			{
				Writer.Write(CSharpKeyword.Abstract);
				Writer.Write(' ');
			}

			if (element.IsPartial)
			{
				Writer.Write(CSharpKeyword.Partial);
				Writer.Write(' ');
			}

			StringBuilder builder = new StringBuilder(DefaultBlockLength);

			switch (element.Type)
			{
				case TypeElementType.Class:
					builder.Append(CSharpKeyword.Class);
					break;

				case TypeElementType.Enum:
					builder.Append(CSharpKeyword.Enumeration);
					break;

				case TypeElementType.Interface:
					builder.Append(CSharpKeyword.Interface);
					break;

				case TypeElementType.Structure:
					builder.Append(CSharpKeyword.Structure);
					break;

				default:
					throw new ArgumentOutOfRangeException(
						string.Format(
						Thread.CurrentThread.CurrentCulture,
						"Unhandled type element type {0}",
						element.Type));
			}

			builder.Append(' ');
			builder.Append(element.Name);

			Writer.Write(builder.ToString());

			WriteTypeParameters(element);

			if (element.Interfaces.Count > 0)
			{
				builder = new StringBuilder(DefaultBlockLength);
				builder.Append(' ');
				builder.Append(CSharpSymbol.TypeImplements);
				builder.Append(' ');

				for (int interfaceIndex = 0; interfaceIndex < element.Interfaces.Count; interfaceIndex++)
				{
					InterfaceReference interfaceReference = element.Interfaces[interfaceIndex];
					builder.Append(interfaceReference.Name);

					if (interfaceIndex < element.Interfaces.Count - 1)
					{
						builder.Append(CSharpSymbol.AliasSeparator);
						builder.Append(' ');
					}
				}

				Writer.Write(builder.ToString());
			}

			WriteTypeParameterConstraints(element);
			Writer.WriteLine();

			if (element.Type == TypeElementType.Enum)
			{
				WriteBody(element);
			}
			else
			{
				WriteBeginBlock();
				WriteBlockChildren(element);
				WriteEndBlock();

				WriteClosingComment(element, ClosingCommentPrefix);
			}
		}

		/// <summary>
		/// Processes a using element.
		/// </summary>
		/// <param name="element">Using element to process.</param>
		public override void VisitUsingElement(UsingElement element)
		{
			this.WriteComments(element.HeaderComments);

			StringBuilder builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpKeyword.Using);
			builder.Append(' ');
			builder.Append(element.Name);
			if (!string.IsNullOrEmpty(element.Redefine))
			{
				builder.Append(" " + CSharpSymbol.Assignment.ToString() + " ");
				builder.Append(element.Redefine);
			}
			builder.Append(CSharpSymbol.EndOfStatement);

			WriteIndented(builder.ToString());
		}

		/// <summary>
		/// Writes a begin region directive.
		/// </summary>
		/// <param name="element">Region element.</param>
		protected override void WriteRegionBeginDirective(RegionElement element)
		{
			StringBuilder builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpSymbol.Preprocessor);
			builder.Append(CSharpKeyword.Region);
			builder.Append(' ');
			builder.Append(element.Name);

			WriteIndented(builder.ToString());
		}

		/// <summary>
		/// Writes an end region directive.
		/// </summary>
		/// <param name="element">Region element.</param>
		protected override void WriteRegionEndDirective(RegionElement element)
		{
			StringBuilder builder = new StringBuilder(DefaultBlockLength);
			builder.Append(CSharpSymbol.Preprocessor);
			builder.Append(CSharpKeyword.EndRegion);
			if (Configuration.Formatting.Regions.EndRegionNameEnabled)
			{
				builder.Append(' ');
				builder.Append(element.Name);
			}

			WriteIndented(builder.ToString());
		}

		/// <summary>
		/// Writes the accessibility for a member or type.
		/// </summary>
		/// <param name="codeAccess">Code member or type accessibility.</param>
		[SuppressMessage("Microsoft.Globalization", "CA1308", Justification = "C# keywords are lowercase.")]
		private void WriteAccess(CodeAccess codeAccess)
		{
			string accessString = string.Empty;
			if (codeAccess != CodeAccess.None)
			{
				accessString = EnumUtilities.ToString(codeAccess).ToLowerInvariant().Replace(",", string.Empty) + " ";
			}

			WriteIndented(accessString);
		}

		/// <summary>
		/// Writes a collection of element attributes.
		/// </summary>
		/// <param name="element">Attributed code element.</param>
		private void WriteAttributes(AttributedElement element)
		{
			foreach (IAttributeElement attribute in element.Attributes)
			{
				attribute.Accept(this);
			}
		}

		/// <summary>
		/// Writes the begin block for a member body.
		/// </summary>
		private void WriteBeginBlock()
		{
			WriteIndented(CSharpSymbol.BeginBlock.ToString());
			TabCount++;
		}

		/// <summary>
		/// Writes a member body.
		/// </summary>
		/// <param name="element">The element.</param>
		private void WriteBody(TextCodeElement element)
		{
			WriteBeginBlock();
			Writer.WriteLine();
			if (element.BodyText != null && element.BodyText.Trim().Length > 0)
			{
				WriteTextBlock(element.BodyText);
				Writer.WriteLine();
				WriteEndBlock();
				WriteClosingComment(element, ClosingCommentPrefix);
			}
			else
			{
				TabCount--;
				WriteIndented(CSharpSymbol.EndBlock.ToString());
			}
		}

		/// <summary>
		/// Writes the end block for a member body.
		/// </summary>
		private void WriteEndBlock()
		{
			TabCount--;
			WriteIndented(CSharpSymbol.EndBlock.ToString());
		}

		/// <summary>
		/// Writes the modifiers for a member.
		/// </summary>
		/// <param name="memberAttributes">Member modifiers.</param>
		private void WriteMemberAttributes(MemberModifiers memberAttributes)
		{
			if ((memberAttributes & MemberModifiers.Static) == MemberModifiers.Static)
			{
				Writer.Write(CSharpKeyword.Static);
				Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Unsafe) == MemberModifiers.Unsafe)
			{
				Writer.Write(CSharpKeyword.Unsafe);
				Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.New) == MemberModifiers.New)
			{
				Writer.Write(CSharpKeyword.New);
				Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Constant) == MemberModifiers.Constant)
			{
				Writer.Write(CSharpKeyword.Constant);
				Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Abstract) == MemberModifiers.Abstract)
			{
				Writer.Write(CSharpKeyword.Abstract);
				Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.External) == MemberModifiers.External)
			{
				Writer.Write(CSharpKeyword.External);
				Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Override) == MemberModifiers.Override)
			{
				Writer.Write(CSharpKeyword.Override);
				Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.ReadOnly) == MemberModifiers.ReadOnly)
			{
				Writer.Write(CSharpKeyword.ReadOnly);
				Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Sealed) == MemberModifiers.Sealed)
			{
				Writer.Write(CSharpKeyword.Sealed);
				Writer.Write(' ');
			}

			if ((memberAttributes & MemberModifiers.Virtual) == MemberModifiers.Virtual)
			{
				Writer.Write(CSharpKeyword.Virtual);
				Writer.Write(' ');
			}
		}

		/// <summary>
		/// Writes a parameter list.
		/// </summary>
		/// <param name="paramList">String of comma-separated parameters.</param>
		private void WriteParameterList(string paramList)
		{
			// TODO: Treat parameters as code elements.
			Writer.Write(CSharpSymbol.BeginParameterList);
			TabCount++;

			if (paramList != null)
			{
				string[] paramLines = paramList.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
				for (int paramLineIndex = 0; paramLineIndex < paramLines.Length; paramLineIndex++)
				{
					string paramLine = paramLines[paramLineIndex];
					if (paramLineIndex > 0)
					{
						Writer.WriteLine();
						WriteIndented(paramLine.Trim());
					}
					else
					{
						Writer.Write(paramLine);
					}
				}
			}
			Writer.Write(CSharpSymbol.EndParameterList);
			TabCount--;
		}

		/// <summary>
		/// Writes the type parameter constraints for a generic code element.
		/// </summary>
		/// <param name="genericElement">Generic code element.</param>
		private void WriteTypeParameterConstraints(IGenericElement genericElement)
		{
			if (genericElement.TypeParameters.Count > 0)
			{
				foreach (TypeParameter typeParameter in genericElement.TypeParameters)
				{
					if (typeParameter.Constraints.Count > 0)
					{
						Writer.WriteLine();
						if (Configuration.Formatting.Tabs.TabStyle == TabStyle.Tabs)
						{
							WriteIndented("\t");
						}
						else
						{
							WriteIndented(new string(' ', Configuration.Formatting.Tabs.SpacesPerTab));
						}

						Writer.Write(CSharpKeyword.Where);
						Writer.Write(' ');
						Writer.Write(typeParameter.Name);
						Writer.Write(' ');
						Writer.Write(CSharpSymbol.TypeImplements);
						Writer.Write(' ');
						for (int constraintIndex = 0; constraintIndex < typeParameter.Constraints.Count;
							constraintIndex++)
						{
							string constraint = typeParameter.Constraints[constraintIndex];
							Writer.Write(constraint);

							if (constraintIndex < typeParameter.Constraints.Count - 1)
							{
								Writer.Write(CSharpSymbol.AliasSeparator);
								Writer.Write(' ');
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Writes the list of type parameters for a generic code element.
		/// </summary>
		/// <param name="genericElement">Generic code element.</param>
		private void WriteTypeParameters(IGenericElement genericElement)
		{
			if (genericElement.TypeParameters.Count > 0)
			{
				Writer.Write(CSharpSymbol.BeginGeneric);

				for (int parameterIndex = 0; parameterIndex < genericElement.TypeParameters.Count; parameterIndex++)
				{
					TypeParameter typeParameter = genericElement.TypeParameters[parameterIndex];
					Writer.Write(typeParameter.Name);
					if (parameterIndex < genericElement.TypeParameters.Count - 1)
					{
						Writer.Write(CSharpSymbol.AliasSeparator);
						Writer.Write(' ');
					}
				}

				Writer.Write(CSharpSymbol.EndGeneric);
			}
		}

		#endregion Methods
	}
}