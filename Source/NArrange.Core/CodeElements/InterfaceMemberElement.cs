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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Base class for interface member elements.
    /// </summary>
    public abstract class InterfaceMemberElement : MemberElement
    {
        #region Fields

        /// <summary>
        /// Synchronization lock for the implements collection.
        /// </summary>
        private readonly object _implementsLock = new object();

        /// <summary>
        /// List of interfaces the member implements.
        /// </summary>
        private List<InterfaceReference> _implements;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the list of interface implementations.
        /// </summary>
        public ReadOnlyCollection<InterfaceReference> Implements
        {
            get
            {
                return ImplementsBase.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the list of interface implementations.
        /// </summary>
        protected List<InterfaceReference> ImplementsBase
        {
            get
            {
                if (_implements == null)
                {
                    lock (_implementsLock)
                    {
                        if (_implements == null)
                        {
                            _implements = new List<InterfaceReference>();
                        }
                    }
                }

                return _implements;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds an item to the Implements collection.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        public void AddImplementation(InterfaceReference implementation)
        {
            if (implementation != null)
            {
                ImplementsBase.Add(implementation);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Clone of the instance with the interface member element state copied.</returns>
        protected abstract InterfaceMemberElement DoInterfaceMemberClone();

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>
        /// Clone of the instance with the member element state copied.
        /// </returns>
        protected override sealed MemberElement DoMemberClone()
        {
            InterfaceMemberElement clone = DoInterfaceMemberClone();

            foreach (InterfaceReference implementation in Implements)
            {
                InterfaceReference implementationClone = implementation.Clone() as InterfaceReference;
                clone.ImplementsBase.Add(implementation);
            }

            return clone;
        }

        #endregion Methods
    }
}