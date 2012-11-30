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

namespace NArrange.VisualBasic
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;

    using NArrange.Core;
    using NArrange.Core.CodeElements;
    using NArrange.Core.Configuration;

    /// <summary>
    /// Visits a tree of code elements for writing VB code. 
    /// </summary>
    internal sealed class VBWriteVisitor : CodeWriteVisitor
    {
        #region Constructors

        /// <summary>
        /// Creates a new VBWriteVisitor.
        /// </summary>
        /// <param name="writer">Text writer.</param>
        /// <param name="configuration">Code configuration.</param>
        public VBWriteVisitor(TextWriter writer, CodeConfiguration configuration)
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

            // HACK: Create an explicit element type for Option (or compiler directive)
            if (element[VBExtendedProperties.Option] is bool &&
                (bool)element[VBExtendedProperties.Option])
            {
                WriteIndented(element.BodyText);
            }
            else
            {
                bool nested = element.Parent is AttributeElement;
                if (!nested)
                {
                    WriteIndented(VBSymbol.BeginAttribute.ToString());
                }

                if (!string.IsNullOrEmpty(element.Target))
                {
                    Writer.Write(element.Target);
                    Writer.Write(VBSymbol.LineDelimiter);
                    Writer.Write(' ');
                }

                Writer.Write(element.Name);
                if (!string.IsNullOrEmpty(element.BodyText))
                {
                    Writer.Write(VBSymbol.BeginParameterList);
                    Writer.Write(element.BodyText);
                    Writer.Write(VBSymbol.EndParameterList);
                }

                //
                // Nested list of attributes?
                //
                foreach (ICodeElement childElement in element.Children)
                {
                    AttributeElement childAttribute = childElement as AttributeElement;
                    if (childAttribute != null)
                    {
                        Writer.Write(", _");
                        Writer.WriteLine();
                        WriteIndented(string.Empty);
                        childAttribute.Accept(this);
                    }
                }

                if (!nested)
                {
                    Writer.Write(VBSymbol.EndAttribute);

                    if (element.Parent is TextCodeElement)
                    {
                        Writer.Write(" _");
                    }
                }

                if (!nested && element.Parent != null)
                {
                    Writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// Writes a comment line.
        /// </summary>
        /// <param name="comment">The comment.</param>
        public override void VisitCommentElement(CommentElement comment)
        {
            if (comment.Type == CommentType.Block)
            {
                throw new InvalidOperationException("Block comments are not supported by VB.");
            }
            else
            {
                StringBuilder builder = new StringBuilder(DefaultBlockLength);

                if (comment.Type == CommentType.XmlLine)
                {
                    builder.Append("'''");
                }
                else
                {
                    builder.Append("'");
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

            this.WriteIndentedLine(
                string.Format(
                CultureInfo.InvariantCulture,
                ConditionFormat,
                VBSymbol.Preprocessor,
                VBKeyword.If,
                element.ConditionExpression));

            if (element.Children.Count == 0)
            {
                Writer.WriteLine();
            }

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
                    this.WriteIndentedLine(VBSymbol.Preprocessor + VBKeyword.Else);
                }
                else
                {
                    this.WriteIndentedLine(
                        string.Format(
                        CultureInfo.InvariantCulture,
                        ConditionFormat,
                        VBSymbol.Preprocessor,
                        VBKeyword.ElseIf,
                        conditionDirective.ConditionExpression));
                }

                if (conditionDirective.Children.Count == 0)
                {
                    Writer.WriteLine();
                }

                WriteChildren(conditionDirective);

                if (conditionDirective.Children.Count > 0)
                {
                    Writer.WriteLine();
                }
            }

            this.WriteIndented(VBSymbol.Preprocessor + VBKeyword.End + " " + VBKeyword.If);
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

            WriteMemberAttributes(
                element.MemberModifiers,
                element[VBExtendedProperties.Overloads] is bool && (bool)element[VBExtendedProperties.Overloads]);

            Writer.Write(VBKeyword.Sub);
            Writer.Write(' ');

            Writer.Write(element.Name);

            WriteParameterList(element.Parameters);

            if (!string.IsNullOrEmpty(element.Reference))
            {
                TabCount++;
                Writer.WriteLine();
                WriteIndented(element.Reference);
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

            WriteMemberAttributes(
                element.MemberModifiers,
                element[VBExtendedProperties.Overloads] is bool && (bool)element[VBExtendedProperties.Overloads]);

            Writer.Write(VBKeyword.Delegate);
            Writer.Write(' ');

            WriteMethodType(element.Type);

            Writer.Write(element.Name);

            WriteTypeParameters(element);
            WriteParameterList(element.Parameters);

            WriteReturnType(element.Type);
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

            WriteMemberAttributes(
                element.MemberModifiers,
                element[VBExtendedProperties.Overloads] is bool && (bool)element[VBExtendedProperties.Overloads]);

            bool isCustom = false;
            if (element.BodyText != null && element.BodyText.Trim().Length > 0)
            {
                isCustom = true;
                Writer.Write(VBKeyword.Custom);
                Writer.Write(' ');
            }

            Writer.Write(VBKeyword.Event);
            Writer.Write(' ');

            Writer.Write(element.Name);

            if (element.Parameters != null)
            {
                WriteParameterList(element.Parameters);
            }
            WriteReturnType(element.Type);
            WriteImplements(element.Implements);

            if (isCustom)
            {
                WriteBody(element);
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

            WriteMemberAttributes(
                element.MemberModifiers,
                element[VBExtendedProperties.Overloads] is bool && (bool)element[VBExtendedProperties.Overloads]);

            if (element[VBExtendedProperties.Dim] is bool &&
                (bool)element[VBExtendedProperties.Dim])
            {
                if (element.Access != CodeAccess.None)
                {
                    Writer.Write(' ');
                }
                Writer.Write(VBKeyword.Dim);
                Writer.Write(' ');
            }

            if (element[VBExtendedProperties.WithEvents] is bool &&
                (bool)element[VBExtendedProperties.WithEvents])
            {
                Writer.Write(' ');
                Writer.Write(VBKeyword.WithEvents);
                Writer.Write(' ');
            }

            Writer.Write(element.Name);

            if (!string.IsNullOrEmpty(element.Type))
            {
                WriteReturnType(element.Type);

                if (!string.IsNullOrEmpty(element.InitialValue))
                {
                    Writer.Write(' ');
                    Writer.Write(VBSymbol.Assignment);
                    Writer.Write(' ');
                    Writer.Write(element.InitialValue);
                }
            }
            else if (!string.IsNullOrEmpty(element.InitialValue))
            {
                Writer.Write(' ');
                if (element.InitialValue.StartsWith(VBKeyword.New + " "))
                {
                    Writer.Write(VBKeyword.As);
                }
                else
                {
                    Writer.Write(VBSymbol.Assignment);
                }
                Writer.Write(' ');
                Writer.Write(element.InitialValue);
            }

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
                Writer.Write(VBKeyword.Partial);
                Writer.Write(' ');
            }

            WriteAccess(element.Access);

            if (element.IsExternal)
            {
                Writer.Write(VBKeyword.Declare);
                Writer.Write(' ');
            }

            if (element[VBExtendedProperties.ExternalModifier] != null)
            {
                Writer.Write(element[VBExtendedProperties.ExternalModifier].ToString());
                Writer.Write(' ');
            }

            WriteMemberAttributes(
                element.MemberModifiers,
                element[VBExtendedProperties.Overloads] is bool && (bool)element[VBExtendedProperties.Overloads]);

            if (element.IsOperator)
            {
                if (element.OperatorType == OperatorType.Explicit)
                {
                    Writer.Write(VBKeyword.Narrowing);
                    Writer.Write(' ');
                }
                else if (element.OperatorType == OperatorType.Implicit)
                {
                    Writer.Write(VBKeyword.Widening);
                    Writer.Write(' ');
                }

                Writer.Write(VBKeyword.Operator);
                Writer.Write(' ');
            }
            else
            {
                WriteMethodType(element.Type);
            }

            Writer.Write(element.Name);

            WriteTypeParameters(element);

            if (element[VBExtendedProperties.ExternalLibrary] != null)
            {
                Writer.Write(' ');
                Writer.Write(VBKeyword.Lib);
                Writer.Write(' ');

                Writer.Write(VBSymbol.BeginString);
                Writer.Write(element[VBExtendedProperties.ExternalLibrary].ToString());
                Writer.Write(VBSymbol.BeginString);
                Writer.Write(' ');
            }

            if (element[VBExtendedProperties.ExternalAlias] != null)
            {
                Writer.Write(VBKeyword.Alias);
                Writer.Write(' ');

                Writer.Write(VBSymbol.BeginString);
                Writer.Write(element[VBExtendedProperties.ExternalAlias].ToString());
                Writer.Write(VBSymbol.BeginString);
                Writer.Write(' ');
            }

            WriteParameterList(element.Parameters);

            WriteReturnType(element.Type);
            WriteImplements(element.Implements);

            string[] handles = element[VBExtendedProperties.Handles] as string[];
            if (handles != null && handles.Length > 0)
            {
                Writer.Write(' ');
                Writer.Write(VBKeyword.Handles);
                for (int handleIndex = 0; handleIndex < handles.Length; handleIndex++)
                {
                    string handleReference = handles[handleIndex];
                    Writer.Write(' ');
                    Writer.Write(handleReference);
                    if (handleIndex < handles.Length - 1)
                    {
                        Writer.Write(VBSymbol.AliasSeparator);
                    }
                }
            }

            if (!element.IsExternal)
            {
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
            builder.Append(VBKeyword.Namespace);
            builder.Append(' ');
            builder.Append(element.Name);

            WriteIndented(builder.ToString());
            WriteBeginBlock();
            WriteBlockChildren(element);
            WriteEndBlock(element);
            Writer.WriteLine();
            Writer.WriteLine();
        }

        /// <summary>
        /// Processes a property element.
        /// </summary>
        /// <param name="element">Property code element.</param>
        public override void VisitPropertyElement(PropertyElement element)
        {
            this.WriteComments(element.HeaderComments);
            this.WriteAttributes(element);

            WriteAccess(element.Access);

            WriteMemberAttributes(
                element.MemberModifiers, element[VBExtendedProperties.Overloads] is bool && (bool)element[VBExtendedProperties.Overloads]);

            if (element[VBExtendedProperties.Default] is bool &&
                (bool)element[VBExtendedProperties.Default])
            {
                Writer.Write(VBKeyword.Default);
                Writer.Write(' ');
            }

            if (element[VBExtendedProperties.AccessModifier] != null &&
                element[VBExtendedProperties.AccessModifier].ToString() != VBKeyword.ReadOnly)
            {
                Writer.Write(element[VBExtendedProperties.AccessModifier]);
                Writer.Write(' ');
            }

            Writer.Write(VBKeyword.Property);
            Writer.Write(' ');

            Writer.Write(element.Name);
            Writer.Write(VBSymbol.BeginParameterList);
            if (element.IndexParameter != null)
            {
                Writer.Write(element.IndexParameter.Trim());
            }
            Writer.Write(VBSymbol.EndParameterList);

            WriteReturnType(element.Type);
            WriteImplements(element.Implements);

            WriteBody(element);
        }

        /// <summary>
        /// Processes a type element.
        /// </summary>
        /// <param name="element">Type code element.</param>
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
                Writer.Write(VBKeyword.Shadows);
                Writer.Write(' ');
            }

            if (element.IsSealed)
            {
                Writer.Write(VBKeyword.NotInheritable);
                Writer.Write(' ');
            }

            if (element.IsAbstract)
            {
                Writer.Write(VBKeyword.MustInherit);
                Writer.Write(' ');
            }

            if (element.IsPartial)
            {
                Writer.Write(VBKeyword.Partial);
                Writer.Write(' ');
            }

            StringBuilder builder = new StringBuilder(DefaultBlockLength);

            switch (element.Type)
            {
                case TypeElementType.Class:
                    builder.Append(VBKeyword.Class);
                    break;

                case TypeElementType.Enum:
                    builder.Append(VBKeyword.Enumeration);
                    break;

                case TypeElementType.Interface:
                    builder.Append(VBKeyword.Interface);
                    break;

                case TypeElementType.Structure:
                    builder.Append(VBKeyword.Structure);
                    break;

                case TypeElementType.Module:
                    builder.Append(VBKeyword.Module);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        string.Format(
                        Thread.CurrentThread.CurrentCulture,
                        "Unrecognized type element type {0}",
                        element.Type));
            }

            builder.Append(' ');
            builder.Append(element.Name);

            Writer.Write(builder.ToString());

            WriteTypeParameters(element);

            if (element.Interfaces.Count > 0)
            {
                if (element.Type == TypeElementType.Enum)
                {
                    Writer.Write(' ');
                    Writer.Write(VBKeyword.As);
                    Writer.Write(' ');
                    Writer.Write(element.Interfaces[0].Name);
                }
                else
                {
                    TabCount++;
                    Writer.WriteLine();

                    for (int interfaceIndex = 0; interfaceIndex < element.Interfaces.Count; interfaceIndex++)
                    {
                        InterfaceReference interfaceReference = element.Interfaces[interfaceIndex];

                        builder = new StringBuilder();
                        if (interfaceReference.ReferenceType == InterfaceReferenceType.Class)
                        {
                            builder.Append(VBKeyword.Inherits);
                        }
                        else
                        {
                            builder.Append(VBKeyword.Implements);
                        }
                        builder.Append(' ');
                        builder.Append(interfaceReference);
                        WriteIndented(builder.ToString());
                        if (interfaceIndex < element.Interfaces.Count - 1)
                        {
                            Writer.WriteLine();
                        }
                    }

                    TabCount--;
                }
            }

            if (element.Type == TypeElementType.Enum)
            {
                WriteBody(element);
            }
            else
            {
                WriteBeginBlock();
                WriteBlockChildren(element);
                WriteEndBlock(element);

                WriteClosingComment(element, VBSymbol.BeginComment.ToString());
            }
        }

        /// <summary>
        /// Processes a using element.
        /// </summary>
        /// <param name="element">Using/Import directive code element.</param>
        public override void VisitUsingElement(UsingElement element)
        {
            this.WriteComments(element.HeaderComments);

            StringBuilder builder = new StringBuilder(DefaultBlockLength);
            builder.Append(VBKeyword.Imports);
            builder.Append(' ');
            builder.Append(element.Name);
            if (!string.IsNullOrEmpty(element.Redefine))
            {
                builder.Append(" " + VBSymbol.Assignment.ToString() + " ");
                builder.Append(element.Redefine);
            }

            WriteIndented(builder.ToString());
        }

        /// <summary>
        /// Writes children for a block element.
        /// </summary>
        /// <param name="element">Block code element.</param>
        protected override void WriteBlockChildren(ICodeElement element)
        {
            if (element.Children.Count > 0)
            {
                Writer.WriteLine();
            }

            base.WriteBlockChildren(element);

            if (element.Children.Count > 0)
            {
                Writer.WriteLine();
            }
        }

        /// <summary>
        /// Writes a starting region directive.
        /// </summary>
        /// <param name="element">Region element.</param>
        protected override void WriteRegionBeginDirective(RegionElement element)
        {
            StringBuilder builder = new StringBuilder(DefaultBlockLength);
            builder.Append(VBSymbol.Preprocessor);
            builder.Append(VBKeyword.Region);
            builder.Append(" \"");
            builder.Append(element.Name);
            builder.Append('"');

            WriteIndented(builder.ToString());
        }

        /// <summary>
        /// Writes an ending region directive.
        /// </summary>
        /// <param name="element">Region element.</param>
        protected override void WriteRegionEndDirective(RegionElement element)
        {
            StringBuilder builder = new StringBuilder(DefaultBlockLength);
            builder.Append(VBSymbol.Preprocessor);
            builder.Append(VBKeyword.End);
            builder.Append(' ');
            builder.Append(VBKeyword.Region);
            if (Configuration.Formatting.Regions.EndRegionNameEnabled)
            {
                builder.Append(" '");
                builder.Append(element.Name);
            }

            WriteIndented(builder.ToString());
        }

        /// <summary>
        /// Gets the type parent.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The Type parent.</returns>
        private static TypeElement GetTypeParent(ICodeElement element)
        {
            TypeElement parentTypeElement = element.Parent as TypeElement;

            if (parentTypeElement == null &&
                (element.Parent is GroupElement || element.Parent is RegionElement))
            {
                parentTypeElement = GetTypeParent(element.Parent);
            }

            return parentTypeElement;
        }

        /// <summary>
        /// Writes the member or type access.
        /// </summary>
        /// <param name="codeAccess">The code access.</param>
        private void WriteAccess(CodeAccess codeAccess)
        {
            string accessString = string.Empty;
            if (codeAccess != CodeAccess.None)
            {
                accessString = EnumUtilities.ToString(codeAccess).Replace(",", string.Empty) + " ";
                accessString = accessString.Replace(
                    EnumUtilities.ToString(CodeAccess.Internal), VBKeyword.Friend);
            }

            WriteIndented(accessString);
        }

        /// <summary>
        /// Writes a collection of element attributes.
        /// </summary>
        /// <param name="element">The element.</param>
        private void WriteAttributes(AttributedElement element)
        {
            foreach (IAttributeElement attribute in element.Attributes)
            {
                attribute.Accept(this);
            }
        }

        /// <summary>
        /// Writes the begin block.
        /// </summary>
        private void WriteBeginBlock()
        {
            TabCount++;
        }

        /// <summary>
        /// Writes the code element body text.
        /// </summary>
        /// <param name="element">The element.</param>
        private void WriteBody(TextCodeElement element)
        {
            MemberElement memberElement = element as MemberElement;
            TypeElement parentTypeElement = GetTypeParent(element);

            bool isAbstract = memberElement != null &&
                (memberElement.MemberModifiers & MemberModifiers.Abstract) == MemberModifiers.Abstract;
            bool inInterface = memberElement != null &&
                parentTypeElement != null && parentTypeElement.Type == TypeElementType.Interface;

            if (!(isAbstract || inInterface))
            {
                WriteBeginBlock();

                Writer.WriteLine();
                if (element.BodyText != null && element.BodyText.Trim().Length > 0)
                {
                    WriteTextBlock(element.BodyText);
                    Writer.WriteLine();
                    WriteEndBlock(element);
                    WriteClosingComment(element, VBSymbol.BeginComment.ToString());
                }
                else
                {
                    WriteEndBlock(element);
                }
            }
        }

        /// <summary>
        /// Writes the end block for an element.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        private void WriteEndBlock(CodeElement codeElement)
        {
            TabCount--;

            MemberElement memberElement = codeElement as MemberElement;
            string blockName = string.Empty;
            if (memberElement != null)
            {
                if (memberElement.ElementType == ElementType.Method ||
                    memberElement.ElementType == ElementType.Constructor)
                {
                    MethodElement methodElement = memberElement as MethodElement;
                    if (methodElement != null && methodElement.IsOperator)
                    {
                        blockName = VBKeyword.Operator;
                    }
                    else if (memberElement.Type != null)
                    {
                        blockName = VBKeyword.Function;
                    }
                    else
                    {
                        blockName = VBKeyword.Sub;
                    }
                }
            }

            if (string.IsNullOrEmpty(blockName))
            {
                TypeElement typeElement = codeElement as TypeElement;
                if (typeElement != null)
                {
                    blockName = EnumUtilities.ToString(typeElement.Type);
                }

                if (string.IsNullOrEmpty(blockName))
                {
                    blockName = EnumUtilities.ToString(codeElement.ElementType);
                }
            }

            WriteIndented(VBKeyword.End + ' ' + blockName);
        }

        /// <summary>
        /// Writes implements clauses.
        /// </summary>
        /// <param name="interfaceReferences">The interface references.</param>
        private void WriteImplements(ReadOnlyCollection<InterfaceReference> interfaceReferences)
        {
            if (interfaceReferences.Count > 0)
            {
                Writer.Write(' ');
                Writer.Write(VBKeyword.Implements);

                for (int index = 0; index < interfaceReferences.Count; index++)
                {
                    InterfaceReference interfaceReference = interfaceReferences[index];

                    Writer.Write(' ');
                    Writer.Write(interfaceReference.Name);

                    if (interfaceReferences.Count > 1 && index < interfaceReferences.Count - 1)
                    {
                        Writer.Write(VBSymbol.AliasSeparator);
                    }
                }
            }
        }

        /// <summary>
        /// Writes the member attributes.
        /// </summary>
        /// <param name="memberAttributes">The member attributes.</param>
        /// <param name="overloads">Whether or not the member is overloaded.</param>
        private void WriteMemberAttributes(MemberModifiers memberAttributes, bool overloads)
        {
            if ((memberAttributes & MemberModifiers.New) == MemberModifiers.New)
            {
                Writer.Write(VBKeyword.Shadows);
                Writer.Write(' ');
            }

            if ((memberAttributes & MemberModifiers.Constant) == MemberModifiers.Constant)
            {
                Writer.Write(VBKeyword.Constant);
                Writer.Write(' ');
            }

            if ((memberAttributes & MemberModifiers.Static) == MemberModifiers.Static)
            {
                Writer.Write(VBKeyword.Shared);
                Writer.Write(' ');
            }

            if ((memberAttributes & MemberModifiers.Abstract) == MemberModifiers.Abstract)
            {
                Writer.Write(VBKeyword.MustOverride);
                Writer.Write(' ');
            }

            if (overloads)
            {
                Writer.Write(VBKeyword.Overloads);
                Writer.Write(' ');
            }

            if ((memberAttributes & MemberModifiers.Override) == MemberModifiers.Override)
            {
                Writer.Write(VBKeyword.Overrides);
                Writer.Write(' ');
            }

            if ((memberAttributes & MemberModifiers.ReadOnly) == MemberModifiers.ReadOnly)
            {
                Writer.Write(VBKeyword.ReadOnly);
                Writer.Write(' ');
            }

            if ((memberAttributes & MemberModifiers.Sealed) == MemberModifiers.Sealed)
            {
                Writer.Write(VBKeyword.NotOverridable);
                Writer.Write(' ');
            }

            if ((memberAttributes & MemberModifiers.Virtual) == MemberModifiers.Virtual)
            {
                Writer.Write(VBKeyword.Overridable);
                Writer.Write(' ');
            }
        }

        /// <summary>
        /// Writes the type of the method.
        /// </summary>
        /// <param name="returnType">The method return type.</param>
        private void WriteMethodType(string returnType)
        {
            if (returnType == null)
            {
                Writer.Write(VBKeyword.Sub);
            }
            else
            {
                Writer.Write(VBKeyword.Function);
            }
            Writer.Write(' ');
        }

        /// <summary>
        /// Writes the parameter list.
        /// </summary>
        /// <param name="paramList">The param list.</param>
        private void WriteParameterList(string paramList)
        {
            Writer.Write(VBSymbol.BeginParameterList);
            TabCount++;

            if (paramList != null)
            {
                if (paramList.Length > 0 && paramList[0] == VBSymbol.LineContinuation)
                {
                    Writer.Write(' ');
                }

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

            Writer.Write(VBSymbol.EndParameterList);
            TabCount--;
        }

        /// <summary>
        /// Writes a member return type.
        /// </summary>
        /// <param name="returnType">The member return type.</param>
        private void WriteReturnType(string returnType)
        {
            if (!string.IsNullOrEmpty(returnType))
            {
                Writer.Write(' ');
                Writer.Write(VBKeyword.As);
                Writer.Write(' ');
                Writer.Write(returnType);
            }
        }

        /// <summary>
        /// Writes the type parameter constraints.
        /// </summary>
        /// <param name="typeParameter">The type parameter.</param>
        private void WriteTypeParameterConstraints(TypeParameter typeParameter)
        {
            if (typeParameter.Constraints.Count > 0)
            {
                Writer.Write(' ');
                Writer.Write(VBKeyword.As);
                Writer.Write(' ');

                if (typeParameter.Constraints.Count > 1)
                {
                    Writer.Write(VBSymbol.BeginTypeConstraintList);
                }

                for (int constraintIndex = 0; constraintIndex < typeParameter.Constraints.Count;
                    constraintIndex++)
                {
                    string constraint = typeParameter.Constraints[constraintIndex];
                    Writer.Write(constraint);

                    if (constraintIndex < typeParameter.Constraints.Count - 1)
                    {
                        Writer.Write(VBSymbol.AliasSeparator);
                        Writer.Write(' ');
                    }
                }

                if (typeParameter.Constraints.Count > 1)
                {
                    Writer.Write(VBSymbol.EndTypeConstraintList);
                }
            }
        }

        /// <summary>
        /// Writes the type parameters.
        /// </summary>
        /// <param name="genericElement">The generic element.</param>
        private void WriteTypeParameters(IGenericElement genericElement)
        {
            if (genericElement.TypeParameters.Count > 0)
            {
                Writer.Write(VBSymbol.BeginParameterList);

                Writer.Write(VBKeyword.Of);
                Writer.Write(' ');

                for (int parameterIndex = 0; parameterIndex < genericElement.TypeParameters.Count; parameterIndex++)
                {
                    TypeParameter typeParameter = genericElement.TypeParameters[parameterIndex];
                    Writer.Write(typeParameter.Name);

                    WriteTypeParameterConstraints(typeParameter);

                    if (parameterIndex < genericElement.TypeParameters.Count - 1)
                    {
                        Writer.Write(VBSymbol.AliasSeparator);
                        Writer.Write(' ');
                    }
                }

                Writer.Write(VBSymbol.EndParameterList);
            }
        }

        #endregion Methods
    }
}