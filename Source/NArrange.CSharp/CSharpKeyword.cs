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
	/// <summary>
	/// C# keyword constants.
	/// </summary>
	public static class CSharpKeyword
	{
		#region Fields

		/// <summary>
		/// "abstract" keyword.
		/// </summary>
		public const string Abstract = "abstract";

		/// <summary>
		/// "as" keyword.
		/// </summary>
		public const string As = "as";

		/// <summary>
		/// "class" keyword.
		/// </summary>
		public const string Class = "class";

		/// <summary>
		/// "const" keyword.
		/// </summary>
		public const string Constant = "const";

		/// <summary>
		/// "delegate" keyword.
		/// </summary>
		public const string Delegate = "delegate";

		/// <summary>
		/// "elif" keyword.
		/// </summary>
		public const string Elif = "elif";

		/// <summary>
		/// "else" keyword.
		/// </summary>
		public const string Else = "else";

		/// <summary>
		/// "endif" keyword.
		/// </summary>
		public const string EndIf = "endif";

		/// <summary>
		/// "endregion" keyword.
		/// </summary>
		public const string EndRegion = "endregion";

		/// <summary>
		/// "enum" keyword.
		/// </summary>
		public const string Enumeration = "enum";

		/// <summary>
		/// "event" keyword.
		/// </summary>
		public const string Event = "event";

		/// <summary>
		/// "explicit" keyword.
		/// </summary>
		public const string Explicit = "explicit";

		/// <summary>
		/// "extern" keyword.
		/// </summary>
		public const string External = "extern";

		/// <summary>
		/// "fixed" keyword.
		/// </summary>
		public const string Fixed = "fixed";

		/// <summary>
		/// "global" keyword.
		/// </summary>
		public const string Global = "global";

		/// <summary>
		/// "if" keyword.
		/// </summary>
		public const string If = "if";

		/// <summary>
		/// "implicit" keyword.
		/// </summary>
		public const string Implicit = "implicit";

		/// <summary>
		/// "interface" keyword.
		/// </summary>
		public const string Interface = "interface";

		/// <summary>
		/// "internal" keyword.
		/// </summary>
		public const string Internal = "internal";

		/// <summary>
		/// "namespace" keyword.
		/// </summary>
		public const string Namespace = "namespace";

		/// <summary>
		/// "new" keyword.
		/// </summary>
		public const string New = "new";

		/// <summary>
		/// "operator" keyword.
		/// </summary>
		public const string Operator = "operator";

		/// <summary>
		/// "override" keyword.
		/// </summary>
		public const string Override = "override";

		/// <summary>
		/// "partial" keyword.
		/// </summary>
		public const string Partial = "partial";

		/// <summary>
		/// "private" keyword.
		/// </summary>
		public const string Private = "private";

		/// <summary>
		/// "protected" keyword.
		/// </summary>
		public const string Protected = "protected";

		/// <summary>
		/// "public" keyword.
		/// </summary>
		public const string Public = "public";

		/// <summary>
		/// "readonly" keyword.
		/// </summary>
		public const string ReadOnly = "readonly";

		/// <summary>
		/// "region" keyword.
		/// </summary>
		public const string Region = "region";

		/// <summary>
		/// "sealed" keyword.
		/// </summary>
		public const string Sealed = "sealed";

		/// <summary>
		/// "static" keyword.
		/// </summary>
		public const string Static = "static";

		/// <summary>
		/// "struct" keyword.
		/// </summary>
		public const string Structure = "struct";

		/// <summary>
		/// "unsafe" keyword.
		/// </summary>
		public const string Unsafe = "unsafe";

		/// <summary>
		/// "using" keyword.
		/// </summary>
		public const string Using = "using";

		/// <summary>
		/// "virtual" keyword.
		/// </summary>
		public const string Virtual = "virtual";

		/// <summary>
		/// "void" keyword.
		/// </summary>
		public const string Void = "void";

		/// <summary>
		/// "volatile" keyword.
		/// </summary>
		public const string Volatile = "volatile";

		/// <summary>
		/// "where" keyword.
		/// </summary>
		public const string Where = "where";

		/// <summary>
		/// Async keyword
		/// </summary>
		public const string Async = "async";

		/// <summary>
		/// Type parameter new() constraint.
		/// </summary>
		public static readonly string NewConstraint = New +
			CSharpSymbol.BeginParameterList + CSharpSymbol.EndParameterList;

		#endregion Fields
	}
}