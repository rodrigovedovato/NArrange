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
 *      Everton Elvio Koser
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Core.CodeElements
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Class/struct code element.
    /// </summary>
    public sealed class TypeElement : AttributedElement, IGenericElement
    {
        #region Fields

        /// <summary>
        /// Synchronization lock for the interfaces collection.
        /// </summary>
        private readonly object _interacesLock = new object();

        /// <summary>
        /// Synchronization lock for the type parameters collection.
        /// </summary>
        private readonly object _typeParametersLock = new object();

        /// <summary>
        /// Collection of interface references.
        /// </summary>
        private List<InterfaceReference> _interfaces;

        /// <summary>
        /// Type of the type element.
        /// </summary>
        private TypeElementType _type = TypeElementType.Class;

        /// <summary>
        /// Type modifiers.
        /// </summary>
        private TypeModifiers _typeModifiers;

        /// <summary>
        /// Generic type parameters.
        /// </summary>
        private List<TypeParameter> _typeParameters;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the element type.
        /// </summary>
        public override ElementType ElementType
        {
            get
            {
                return ElementType.Type;
            }
        }

        /// <summary>
        /// Gets the collection of implemented interface names for the type
        /// definition.
        /// </summary>
        public ReadOnlyCollection<InterfaceReference> Interfaces
        {
            get
            {
                return BaseInterfaces.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the type is abstract.
        /// </summary>
        public bool IsAbstract
        {
            get
            {
                return (_typeModifiers & TypeModifiers.Abstract) == TypeModifiers.Abstract;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the type is new.
        /// </summary>
        public bool IsNew
        {
            get
            {
                return (_typeModifiers & TypeModifiers.New) == TypeModifiers.New;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the type is a partial class.
        /// </summary>
        public bool IsPartial
        {
            get
            {
                return (_typeModifiers & TypeModifiers.Partial) == TypeModifiers.Partial;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the type is sealed.
        /// </summary>
        public bool IsSealed
        {
            get
            {
                return (_typeModifiers & TypeModifiers.Sealed) == TypeModifiers.Sealed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the type is static.
        /// </summary>
        public bool IsStatic
        {
            get
            {
                return (_typeModifiers & TypeModifiers.Static) == TypeModifiers.Static;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the type is unsafe.
        /// </summary>
        public bool IsUnsafe
        {
            get
            {
                return (_typeModifiers & TypeModifiers.Unsafe) == TypeModifiers.Unsafe;
            }
        }

        /// <summary>
        /// Gets or sets the type of the type element.
        /// </summary>
        public TypeElementType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// Gets or sets the type attributes.
        /// </summary>
        public TypeModifiers TypeModifiers
        {
            get
            {
                return _typeModifiers;
            }
            set
            {
                _typeModifiers = value;
            }
        }

        /// <summary>
        /// Gets the list of type parameters.
        /// </summary>
        public ReadOnlyCollection<TypeParameter> TypeParameters
        {
            get
            {
                return TypeParametersBase.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the list of implemented interface names.
        /// </summary>
        private List<InterfaceReference> BaseInterfaces
        {
            get
            {
                if (_interfaces == null)
                {
                    lock (_interacesLock)
                    {
                        if (_interfaces == null)
                        {
                            _interfaces = new List<InterfaceReference>();
                        }
                    }
                }

                return _interfaces;
            }
        }

        /// <summary>
        /// Gets the list of type parameters.
        /// </summary>
        private List<TypeParameter> TypeParametersBase
        {
            get
            {
                if (_typeParameters == null)
                {
                    lock (_typeParametersLock)
                    {
                        if (_typeParameters == null)
                        {
                            _typeParameters = new List<TypeParameter>();
                        }
                    }
                }

                return _typeParameters;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Allows an ICodeElementVisitor to process (or visit) this element.
        /// </summary>
        /// <param name="visitor">Visitor to accept the code element.</param>
        /// <remarks>See the Gang of Four Visitor design pattern.</remarks>
        public override void Accept(ICodeElementVisitor visitor)
        {
            visitor.VisitTypeElement(this);
        }

        /// <summary>
        /// Adds an interface implementation to the type definition.
        /// </summary>
        /// <param name="interfaceReference">The interface reference.</param>
        public void AddInterface(InterfaceReference interfaceReference)
        {
            if (interfaceReference != null)
            {
                BaseInterfaces.Add(interfaceReference);
            }
        }

        /// <summary>
        /// Adds a type parameter to the type parameter list.
        /// </summary>
        /// <param name="typeParameter">The type parameter to add.</param>
        public void AddTypeParameter(TypeParameter typeParameter)
        {
            if (typeParameter == null)
            {
                throw new ArgumentNullException("typeParameter");
            }

            TypeParametersBase.Add(typeParameter);
        }

        /// <summary>
        /// Clones an attributed element.
        /// </summary>
        /// <returns>Cloned attribute element state.</returns>
        protected override AttributedElement DoAttributedClone()
        {
            TypeElement clone = new TypeElement();

            //
            // Copy state
            //
            clone._typeModifiers = _typeModifiers;
            clone._type = _type;
            foreach (InterfaceReference interfaceReference in Interfaces)
            {
                InterfaceReference referenceClone = interfaceReference.Clone() as InterfaceReference;
                clone.AddInterface(referenceClone);
            }
            foreach (TypeParameter typeParam in TypeParameters)
            {
                TypeParameter typeParamClone = typeParam.Clone() as TypeParameter;
                clone.TypeParametersBase.Add(typeParamClone);
            }

            return clone;
        }

        #endregion Methods
    }
}