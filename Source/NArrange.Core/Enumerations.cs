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
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Core
{
    using System;

    #region Enumerations

    /// <summary>
    /// Binary expression operator.
    /// </summary>
    public enum BinaryExpressionOperator
    {
        /// <summary>
        /// Equality operator.
        /// </summary>
        Equal,

        /// <summary>
        /// Not equal operator.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Contains a substring.
        /// </summary>
        Contains,

        /// <summary>
        /// Matches a substring.
        /// </summary>
        Matches,

        /// <summary>
        /// Logical And.
        /// </summary>
        And,

        /// <summary>
        /// Logical Or.
        /// </summary>
        Or
    }

    /// <summary>
    /// Code access level.
    /// </summary>
    [Flags]
    public enum CodeAccess
    {
        /// <summary>
        /// None/Not specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Private accessibility.
        /// </summary>
        Private = 1,

        /// <summary>
        /// Protected/family accessibility.
        /// </summary>
        Protected = 2,

        /// <summary>
        /// Internal/assembly accessibility.
        /// </summary>
        Internal = 4,

        /// <summary>
        /// Public accessibility.
        /// </summary>
        Public = 8
    }

    /// <summary>
    /// Represents a level in a code file.
    /// </summary>
    public enum CodeLevel
    {
        /// <summary>
        /// None, not specified.
        /// </summary>
        None,

        /// <summary>
        /// File level.
        /// </summary>
        File,

        /// <summary>
        /// Namespace level.
        /// </summary>
        Namespace
    }

    /// <summary>
    /// Comment type.
    /// </summary>
    public enum CommentType
    {
        /// <summary>
        /// Single line comment.
        /// </summary>
        Line,

        /// <summary>
        /// Single line XML comment.
        /// </summary>
        XmlLine,

        /// <summary>
        /// Block comment.
        /// </summary>
        Block
    }

    /// <summary>
    /// Element attribute scope.
    /// </summary>
    public enum ElementAttributeScope
    {
        /// <summary>
        /// Element scope.
        /// </summary>
        Element,

        /// <summary>
        /// Parent scope.
        /// </summary>
        Parent
    }

    /// <summary>
    /// Element attribute.
    /// </summary>
    public enum ElementAttributeType
    {
        /// <summary>
        /// None/Not specified.
        /// </summary>
        None,

        /// <summary>
        /// Name attribute.
        /// </summary>
        Name,

        /// <summary>
        /// Access attribute.
        /// </summary>
        Access,

        /// <summary>
        /// Modifier attribute.
        /// </summary>
        Modifier,

        /// <summary>
        /// Element Type attribute.
        /// </summary>
        ElementType,

        /// <summary>
        /// Type attribute.
        /// </summary>
        Type,

        /// <summary>
        /// Attributes attribute.
        /// </summary>
        Attributes
    }

    /// <summary>
    /// Element type.
    /// </summary>
    public enum ElementType
    {
        /// <summary>
        /// Not specified.
        /// </summary>
        NotSpecified,

        /// <summary>
        /// Comment element type.
        /// </summary>
        Comment,

        /// <summary>
        /// Attribute element type.
        /// </summary>
        Attribute,

        /// <summary>
        /// Using statement element type.
        /// </summary>
        Using,

        /// <summary>
        /// Namespace element type.
        /// </summary>
        Namespace,

        /// <summary>
        /// Region element type.
        /// </summary>
        Region,

        /// <summary>
        /// Condition directive element type.
        /// </summary>
        ConditionDirective,

        /// <summary>
        /// Field element type.
        /// </summary>
        Field,

        /// <summary>
        /// Constructor element type.
        /// </summary>
        Constructor,

        /// <summary>
        /// Property element type.
        /// </summary>
        Property,

        /// <summary>
        /// Method element type.
        /// </summary>
        Method,

        /// <summary>
        /// Event element type.
        /// </summary>
        Event,

        /// <summary>
        /// Delegate element type.
        /// </summary>
        Delegate,

        /// <summary>
        /// Type element type.
        /// </summary>
        Type,
    }

    /// <summary>
    /// File attribute.
    /// </summary>
    public enum FileAttributeType
    {
        /// <summary>
        /// None/Not specified.
        /// </summary>
        None,

        /// <summary>
        /// File name.
        /// </summary>
        Name,

        /// <summary>
        /// File path.
        /// </summary>
        Path,

        /// <summary>
        /// Attributes for the file.
        /// </summary>
        Attributes
    }

    /// <summary>
    /// Grouping separator type.
    /// </summary>
    public enum GroupSeparatorType
    {
        /// <summary>
        /// New line separator.
        /// </summary>
        NewLine,

        /// <summary>
        /// Custom separator string.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Handler type.
    /// </summary>
    public enum HandlerType
    {
        /// <summary>
        /// Source handler.
        /// </summary>
        Source,

        /// <summary>
        /// Project handler.
        /// </summary>
        Project
    }

    /// <summary>
    /// Enumeration for interface impelementation types.
    /// </summary>
    public enum InterfaceReferenceType
    {
        /// <summary>
        /// None/Unknown reference type.
        /// </summary>
        None,

        /// <summary>
        /// Base class implementation.
        /// </summary>
        Class,

        /// <summary>
        /// Interface implementation.
        /// </summary>
        Interface
    }

    /// <summary>
    /// Log level.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Error message log level.
        /// </summary>
        Error,

        /// <summary>
        /// Warning message log level.
        /// </summary>
        Warning,

        /// <summary>
        /// Informational message log level.
        /// </summary>
        Info,

        /// <summary>
        /// Verbose log level.
        /// </summary>
        Verbose,

        /// <summary>
        /// Trace log level.
        /// </summary>
        Trace
    }

    /// <summary>
    /// Member attributes.
    /// </summary>
    [Flags]
    public enum MemberModifiers
    {
        /// <summary>
        /// None/Not specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Abstract member.
        /// </summary>
        Abstract = 1,

        /// <summary>
        /// Sealed member.
        /// </summary>
        Sealed = 2,

        /// <summary>
        /// Static member.
        /// </summary>
        Static = 4,

        /// <summary>
        /// Unsafe member.
        /// </summary>
        Unsafe = 8,

        /// <summary>
        /// Virtual member.
        /// </summary>
        Virtual = 16,

        /// <summary>
        /// Override member.
        /// </summary>
        Override = 32,

        /// <summary>
        /// New member.
        /// </summary>
        New = 64,

        /// <summary>
        /// ReadOnly member.
        /// </summary>
        ReadOnly = 128,

        /// <summary>
        /// Constant member.
        /// </summary>
        Constant = 256,

        /// <summary>
        /// External member.
        /// </summary>
        External = 512,

        /// <summary>
        /// Partial member.
        /// </summary>
        Partial = 1024
    }

    /// <summary>
    /// Operator type.
    /// </summary>
    public enum OperatorType
    {
        /// <summary>
        /// None/Not specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Explicit operator.
        /// </summary>
        Explicit = 1,

        /// <summary>
        /// Implicit operator.
        /// </summary>
        Implicit = 2
    }

    /// <summary>
    /// Region style.
    /// </summary>
    public enum RegionStyle
    {
        /// <summary>
        /// Default region style.
        /// </summary>
        Default,

        /// <summary>
        /// Use region directives around region members.
        /// </summary>
        Directive,

        /// <summary>
        /// Commented directives around region members.
        /// </summary>
        CommentDirective,

        /// <summary>
        /// No directives around region members, just group.
        /// </summary>
        NoDirective
    }

    /// <summary>
    /// Sort direction.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// None, do not sort.
        /// </summary>
        None,

        /// <summary>
        /// Sort in ascending order.
        /// </summary>
        Ascending,

        /// <summary>
        /// Sort in descending order.
        /// </summary>
        Descending
    }

    /// <summary>
    /// Tabbing style.
    /// </summary>
    public enum TabStyle
    {
        /// <summary>
        /// Uses spaces when writing elements.
        /// </summary>
        Spaces,

        /// <summary>
        /// Use tabs when writing elements.
        /// </summary>
        Tabs
    }

    /// <summary>
    /// Type element type.
    /// </summary>
    public enum TypeElementType
    {
        /// <summary>
        /// Module element type.
        /// </summary>
        Module,

        /// <summary>
        /// Class element type.
        /// </summary>
        Class,

        /// <summary>
        /// Structure element type.
        /// </summary>
        Structure,

        /// <summary>
        /// Interface element type.
        /// </summary>
        Interface,

        /// <summary>
        /// Enumeration element type.
        /// </summary>
        Enum
    }

    /// <summary>
    /// Type attributes.
    /// </summary>
    /// <remarks>This is a subset of member attributes that apply to types.</remarks>
    [Flags]
    public enum TypeModifiers
    {
        /// <summary>
        /// None, no modifiers specified.
        /// </summary>
        None = MemberModifiers.None,

        /// <summary>
        /// Abstract type declaration.
        /// </summary>
        Abstract = MemberModifiers.Abstract,

        /// <summary>
        /// Sealed type declaration.
        /// </summary>
        Sealed = MemberModifiers.Sealed,

        /// <summary>
        /// Static type declaration.
        /// </summary>
        Static = MemberModifiers.Static,

        /// <summary>
        /// Unsafe type declaration.
        /// </summary>
        Unsafe = MemberModifiers.Unsafe,

        /// <summary>
        /// Partial type declaration.
        /// </summary>
        Partial = MemberModifiers.Partial,

        /// <summary>
        /// New type declaration.
        /// </summary>
        New = MemberModifiers.New
    }

    /// <summary>
    /// Unary expression operator.
    /// </summary>
    public enum UnaryExpressionOperator
    {
        /// <summary>
        /// Logical negate.
        /// </summary>
        Negate
    }

    /// <summary>
    /// Using type.
    /// </summary>
    public enum UsingType
    {
        /// <summary>
        /// Namespace.
        /// </summary>
        Namespace,

        /// <summary>
        /// Redefinition of namespace or type.
        /// </summary>
        Alias
    }

    /// <summary>
    /// Whitespace character types.
    /// </summary>
    [Flags]
    public enum WhiteSpaceTypes
    {
        /// <summary>
        /// None, do not include any whitespace.
        /// </summary>
        None = 0,

        /// <summary>
        /// Include spaces.
        /// </summary>
        Space = 1,

        /// <summary>
        /// Include tabs.
        /// </summary>
        Tab = 2,

        /// <summary>
        /// Include carriage returns.
        /// </summary>
        CarriageReturn = 4,

        /// <summary>
        /// Include line feeds.
        /// </summary>
        Linefeed = 8,

        /// <summary>
        /// Include spaces and tabs.
        /// </summary>
        SpaceAndTab = Space | Tab,

        /// <summary>
        /// Include carriage returns and line feeds.
        /// </summary>
        CarriageReturnAndLinefeed = CarriageReturn | Linefeed,

        /// <summary>
        /// Include all whitespace characters.
        /// </summary>
        All = SpaceAndTab | CarriageReturnAndLinefeed
    }

    #endregion Enumerations
}