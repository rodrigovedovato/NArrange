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
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Core.CodeElements
{
    /// <summary>
    /// Interface for a code element visitor.
    /// </summary>
    public interface ICodeElementVisitor
    {
        #region Methods

        /// <summary>
        /// Visits an AttributeElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitAttributeElement(AttributeElement element);

        /// <summary>
        /// Visits a CommentLineElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitCommentElement(CommentElement element);

        /// <summary>
        /// Visits a ConditionDirectiveElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitConditionDirectiveElement(ConditionDirectiveElement element);

        /// <summary>
        /// Visits a ConstructorElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitConstructorElement(ConstructorElement element);

        /// <summary>
        /// Visits a DelegateElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitDelegateElement(DelegateElement element);

        /// <summary>
        /// Visits an EventElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitEventElement(EventElement element);

        /// <summary>
        /// Visits a FieldElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitFieldElement(FieldElement element);

        /// <summary>
        /// Visits a GroupElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitGroupElement(GroupElement element);

        /// <summary>
        /// Visits a MethodElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitMethodElement(MethodElement element);

        /// <summary>
        /// Visits a NamespaceElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitNamespaceElement(NamespaceElement element);

        /// <summary>
        /// Visits a PropertyElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitPropertyElement(PropertyElement element);

        /// <summary>
        /// Visits a RegionElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitRegionElement(RegionElement element);

        /// <summary>
        /// Visits a TypeElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitTypeElement(TypeElement element);

        /// <summary>
        /// Visits a UsingElement.
        /// </summary>
        /// <param name="element">The element.</param>
        void VisitUsingElement(UsingElement element);

        #endregion Methods
    }
}