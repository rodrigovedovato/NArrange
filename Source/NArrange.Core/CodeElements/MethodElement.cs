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
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	/// <summary>
	/// Method element.
	/// </summary>
	public sealed class MethodElement : InterfaceMemberElement, IGenericElement
	{
		#region Fields

		/// <summary>
		/// Synchronization lock for the type paramaters list.
		/// </summary>
		private readonly object _typeParametersLock = new object();

		/// <summary>
		/// Whether or not the method is an operator.
		/// </summary>
		private bool _isOperator;

		/// <summary>
		/// Operator type, if applicable.
		/// </summary>
		private OperatorType _operatorType;

		/// <summary>
		/// Method parameters as a comma-separated list (including any attributes).
		/// </summary>
		private string _params;

		/// <summary>
		/// Generic type parameters.
		/// </summary>
		private List<TypeParameter> _typeParameters;

		/// <summary>
		/// Whether or not the method is async
		/// </summary>
		private bool _isAsync;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Gets the element type.
		/// </summary>
		public override ElementType ElementType
		{
			get
			{
				return ElementType.Method;
			}
		}

		/// <summary>
		/// Gets a value indicating whether or not the member is external.
		/// </summary>
		public bool IsExternal
		{
			get
			{
				return (MemberModifiers & MemberModifiers.External) == MemberModifiers.External;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the method is an operator.
		/// </summary>
		public bool IsOperator
		{
			get
			{
				return _isOperator;
			}
			set
			{
				_isOperator = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether or not the method is a partial method.
		/// </summary>
		public bool IsPartial
		{
			get
			{
				return (MemberModifiers & MemberModifiers.Partial) == MemberModifiers.Partial;
			}
		}

		/// <summary>
		/// Gets or sets the operator type.
		/// </summary>
		public OperatorType OperatorType
		{
			get
			{
				return _operatorType;
			}
			set
			{
				_operatorType = value;
			}
		}

		/// <summary>
		/// Gets or sets the parameter list 
		/// </summary>
		public string Parameters
		{
			get
			{
				return _params;
			}
			set
			{
				_params = value;
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

		/// <summary>
		/// Whether or not the method is async
		/// </summary>
		public bool IsAsync
		{
			get { return _isAsync; }
			set { _isAsync = value; }
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
			visitor.VisitMethodElement(this);
		}

		/// <summary>
		/// Adds a type parameter to the type parameter list.
		/// </summary>
		/// <param name="typeParameter">Type paramater to add.</param>
		public void AddTypeParameter(TypeParameter typeParameter)
		{
			if (typeParameter == null)
			{
				throw new ArgumentNullException("typeParameter");
			}

			TypeParametersBase.Add(typeParameter);
		}

		/// <summary>
		/// Creates a clone of this instance.
		/// </summary>
		/// <returns>
		/// Clone of the instance with the interface member element state copied.
		/// </returns>
		protected override InterfaceMemberElement DoInterfaceMemberClone()
		{
			MethodElement clone = new MethodElement();

			//
			// Copy state
			//
			clone._params = _params;
			clone._isOperator = _isOperator;
			clone._operatorType = _operatorType;
			clone._isAsync = _isAsync;

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