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
    using System.Collections.Generic;

    /// <summary>
    /// Visual Basic keyword constants.
    /// </summary>
    public static class VBKeyword
    {
        #region Fields

        /// <summary>
        /// "Alias" keyword.
        /// </summary>
        public const string Alias = "Alias";

        /// <summary>
        /// "Ansi" keyword.
        /// </summary>
        public const string Ansi = "Ansi";

        /// <summary>
        /// "As" keyword.
        /// </summary>
        public const string As = "As";

        /// <summary>
        /// "Auto" keyword.
        /// </summary>
        public const string Auto = "Auto";

        /// <summary>
        /// "Begin" keyword.
        /// </summary>
        public const string Begin = "Begin";

        /// <summary>
        /// "Class" keyword.
        /// </summary>
        public const string Class = "Class";

        /// <summary>
        /// "Const" keyword.
        /// </summary>
        public const string Constant = "Const";

        /// <summary>
        /// "Custom" keyword.
        /// </summary>
        public const string Custom = "Custom";

        /// <summary>
        /// "Declare" keyword.
        /// </summary>
        public const string Declare = "Declare";

        /// <summary>
        /// "Default" keyword.
        /// </summary>
        public const string Default = "Default";

        /// <summary>
        /// "Delegate" keyword.
        /// </summary>
        public const string Delegate = "Delegate";

        /// <summary>
        /// "Dim" keyword.
        /// </summary>
        public const string Dim = "Dim";

        /// <summary>
        /// "Else" keyword.
        /// </summary>
        public const string Else = "Else";

        /// <summary>
        /// "ElseIf" keyword.
        /// </summary>
        public const string ElseIf = "ElseIf";

        /// <summary>
        /// "End" keyword.
        /// </summary>
        public const string End = "End";

        /// <summary>
        /// "Enum" keyword.
        /// </summary>
        public const string Enumeration = "Enum";

        /// <summary>
        /// "Event" keyword.
        /// </summary>
        public const string Event = "Event";

        /// <summary>
        /// "Friend" keyword.
        /// </summary>
        public const string Friend = "Friend";

        /// <summary>
        /// "Function" keyword.
        /// </summary>
        public const string Function = "Function";

        /// <summary>
        /// "Handles" keyword.
        /// </summary>
        public const string Handles = "Handles";

        /// <summary>
        /// "If" keyword.
        /// </summary>
        public const string If = "If";

        /// <summary>
        /// "Implements" keyword.
        /// </summary>
        public const string Implements = "Implements";

        /// <summary>
        /// "Imports" keyword.
        /// </summary>
        public const string Imports = "Imports";

        /// <summary>
        /// "Inherits" keyword.
        /// </summary>
        public const string Inherits = "Inherits";

        /// <summary>
        /// "Interface" keyword.
        /// </summary>
        public const string Interface = "Interface";

        /// <summary>
        /// "Lib" keyword.
        /// </summary>
        public const string Lib = "Lib";

        /// <summary>
        /// "Module" keyword.
        /// </summary>
        public const string Module = "Module";

        /// <summary>
        /// "MustInherit" keyword.
        /// </summary>
        public const string MustInherit = "MustInherit";

        /// <summary>
        /// "MustOverride" keyword.
        /// </summary>
        public const string MustOverride = "MustOverride";

        /// <summary>
        /// "Namespace" keyword.
        /// </summary>
        public const string Namespace = "Namespace";

        /// <summary>
        /// "Narrowing" keyword.
        /// </summary>
        public const string Narrowing = "Narrowing";

        /// <summary>
        /// "New" keyword.
        /// </summary>
        public const string New = "New";

        /// <summary>
        /// "NotInheritable" keyword.
        /// </summary>
        public const string NotInheritable = "NotInheritable";

        /// <summary>
        /// "NotOverridable" keyword.
        /// </summary>
        public const string NotOverridable = "NotOverridable";

        /// <summary>
        /// "Of" keyword.
        /// </summary>
        public const string Of = "Of";

        /// <summary>
        /// "Operator" keyword.
        /// </summary>
        public const string Operator = "Operator";

        /// <summary>
        /// "Option" keyword.
        /// </summary>
        public const string Option = "Option";

        /// <summary>
        /// "Overloads" keyword.
        /// </summary>
        public const string Overloads = "Overloads";

        /// <summary>
        /// "Overridable" keyword.
        /// </summary>
        public const string Overridable = "Overridable";

        /// <summary>
        /// "Overrides" keyword.
        /// </summary>
        public const string Overrides = "Overrides";

        /// <summary>
        /// "Partial" keyword.
        /// </summary>
        public const string Partial = "Partial";

        /// <summary>
        /// "Private" keyword.
        /// </summary>
        public const string Private = "Private";

        /// <summary>
        /// "Property" keyword.
        /// </summary>
        public const string Property = "Property";

        /// <summary>
        /// "Protected" keyword.
        /// </summary>
        public const string Protected = "Protected";

        /// <summary>
        /// "Public" keyword.
        /// </summary>
        public const string Public = "Public";

        /// <summary>
        /// "ReadOnly" keyword.
        /// </summary>
        public const string ReadOnly = "ReadOnly";

        /// <summary>
        /// "ReadWrite" keyword.
        /// </summary>
        public const string ReadWrite = "ReadWrite";

        /// <summary>
        /// "Region" keyword.
        /// </summary>
        public const string Region = "Region";

        /// <summary>
        /// REM (line comment)
        /// </summary>
        public const string Rem = "REM";

        /// <summary>
        /// "Shadows" keyword.
        /// </summary>
        public const string Shadows = "Shadows";

        /// <summary>
        /// "Shared" keyword.
        /// </summary>
        public const string Shared = "Shared";

        /// <summary>
        /// "Structure" keyword.
        /// </summary>
        public const string Structure = "Structure";

        /// <summary>
        /// "Sub" keyword.
        /// </summary>
        public const string Sub = "Sub";

        /// <summary>
        /// "Unicode" keyword.
        /// </summary>
        public const string Unicode = "Unicode";

        /// <summary>
        /// "Widening" keyword.
        /// </summary>
        public const string Widening = "Widening";

        /// <summary>
        /// "WithEvents" keyword.
        /// </summary>
        public const string WithEvents = "WithEvents";

        /// <summary>
        /// "WriteOnly" keyword.
        /// </summary>
        public const string WriteOnly = "WriteOnly";

        /// <summary>
        /// A collection of keywords that have specialized normalization rules.
        /// </summary>
        private static Dictionary<string, string> _specialNormalizedKeywords = 
            CreateNormalizedKeywordDictionary();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Determines whether or not the specifed word is a VB keyword.
        /// </summary>
        /// <param name="word">The word to test.</param>
        /// <returns>Whether or not the word is a VB keyword.</returns>
        public static bool IsVBKeyword(string word)
        {
            bool isKeyword = false;

            if (!string.IsNullOrEmpty(word))
            {
                string normalized = Normalize(word.Trim());

                isKeyword =
                    normalized == Alias ||
                    normalized == Ansi ||
                    normalized == Auto ||
                    normalized == As ||
                    normalized == Begin ||
                    normalized == Class ||
                    normalized == Constant ||
                    normalized == Custom ||
                    normalized == Default ||
                    normalized == Delegate ||
                    normalized == Dim ||
                    normalized == Else ||
                    normalized == ElseIf ||
                    normalized == End ||
                    normalized == Enumeration ||
                    normalized == Event ||
                    normalized == Friend ||
                    normalized == Function ||
                    normalized == Handles ||
                    normalized == If ||
                    normalized == Imports ||
                    normalized == Implements ||
                    normalized == Inherits ||
                    normalized == Interface ||
                    normalized == Lib ||
                    normalized == Module ||
                    normalized == MustInherit ||
                    normalized == MustOverride ||
                    normalized == Namespace ||
                    normalized == Narrowing ||
                    normalized == New ||
                    normalized == NotInheritable ||
                    normalized == NotOverridable ||
                    normalized == Of ||
                    normalized == Operator ||
                    normalized == Option ||
                    normalized == Overloads ||
                    normalized == Overridable ||
                    normalized == Overrides ||
                    normalized == Partial ||
                    normalized == Private ||
                    normalized == Property ||
                    normalized == Protected ||
                    normalized == Public ||
                    normalized == ReadOnly ||
                    normalized == ReadWrite ||
                    normalized == Region ||
                    normalized == Rem ||
                    normalized == Shadows ||
                    normalized == Shared ||
                    normalized == Structure ||
                    normalized == Sub ||
                    normalized == Unicode ||
                    normalized == Widening ||
                    normalized == WithEvents ||
                    normalized == WriteOnly;
            }

            return isKeyword;
        }

        /// <summary>
        /// Normalizes a keyword for standard casing.
        /// </summary>
        /// <param name="keyword">VB keyword.</param>
        /// <returns>The normalized keyword.</returns>
        public static string Normalize(string keyword)
        {
            string normalized = null;

            if (keyword != null)
            {
                if (keyword.Length > 0)
                {
                    if (!(_specialNormalizedKeywords.TryGetValue(keyword, out normalized)))
                    {
                        normalized = char.ToUpper(keyword[0]).ToString();
                        if (keyword.Length > 1)
                        {
                            normalized += keyword.Substring(1);
                        }
                    }
                }
            }

            return normalized;
        }

        /// <summary>
        /// Creates a dictionary of special cases when normalizing keywords.
        /// </summary>
        /// <returns>Normalized keyword dictionary.</returns>
        private static Dictionary<string, string> CreateNormalizedKeywordDictionary()
        {
            Dictionary<string, string> _specialNormalizedKeywords;
            _specialNormalizedKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            _specialNormalizedKeywords[ElseIf] = ElseIf;
            _specialNormalizedKeywords[ReadOnly] = ReadOnly;
            _specialNormalizedKeywords[ReadWrite] = ReadWrite;
            _specialNormalizedKeywords[WriteOnly] = WriteOnly;
            _specialNormalizedKeywords[MustOverride] = MustOverride;
            _specialNormalizedKeywords[MustInherit] = MustInherit;
            _specialNormalizedKeywords[NotInheritable] = NotInheritable;
            _specialNormalizedKeywords[NotOverridable] = NotOverridable;
            _specialNormalizedKeywords[WithEvents] = WithEvents;
            _specialNormalizedKeywords[Rem] = Rem.ToUpper();
            return _specialNormalizedKeywords;
        }

        #endregion Methods
    }
}