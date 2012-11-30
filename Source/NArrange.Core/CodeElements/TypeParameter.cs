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
    /// Type parameter definition for generic types.
    /// </summary>
    public sealed class TypeParameter : ICloneable
    {
        #region Fields

        /// <summary>
        /// Synchronization lock for the type parameter constraints.
        /// </summary>
        private readonly object _constraintsLock = new object();

        /// <summary>
        /// Type parameter constraints.
        /// </summary>
        private List<string> _constraints;

        /// <summary>
        /// Type parameter name.
        /// </summary>
        private string _name;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new TypeParameter.
        /// </summary>
        public TypeParameter()
        {
            _name = string.Empty;
        }

        /// <summary>
        /// Creates a new TypeParameter.
        /// </summary>
        /// <param name="name">The type parameter name.</param>
        /// <param name="constraints">The constraints.</param>
        public TypeParameter(string name, params string[] constraints)
            : this()
        {
            _name = name;
            foreach (string constraint in constraints)
            {
                AddConstraint(constraint);
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the collection of constraints for this type parameter.
        /// </summary>
        public ReadOnlyCollection<string> Constraints
        {
            get
            {
                return BaseConstraints.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets or sets the type parameter name.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets the list of parameter constraints.
        /// </summary>
        private List<string> BaseConstraints
        {
            get
            {
                if (_constraints == null)
                {
                    lock (_constraintsLock)
                    {
                        if (_constraints == null)
                        {
                            _constraints = new List<string>();
                        }
                    }
                }

                return _constraints;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds a constraint for the type parameter.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        public void AddConstraint(string constraint)
        {
            BaseConstraints.Add(constraint);
        }

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            TypeParameter clone = new TypeParameter();

            //
            // Copy state
            //
            clone._name = _name;
            foreach (string constraint in Constraints)
            {
                clone.AddConstraint(constraint);
            }

            return clone;
        }

        #endregion Methods
    }
}