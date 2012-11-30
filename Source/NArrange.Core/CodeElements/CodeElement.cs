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
    /// Code element base class.
    /// </summary>
    public abstract class CodeElement : ICodeElement
    {
        #region Fields

        /// <summary>
        /// Children synch lock.
        /// </summary>
        private readonly object _childrenLock = new object();

        /// <summary>
        /// Extened property dictionary.
        /// </summary>
        private readonly Dictionary<string, object> _extendedProperties;

        /// <summary>
        /// Collection of chile code elements.
        /// </summary>
        private List<ICodeElement> _children;

        /// <summary>
        /// Element name.
        /// </summary>
        private string _name;

        /// <summary>
        /// Parent code element.
        /// </summary>
        private ICodeElement _parent;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected CodeElement()
        {
            // Default property values
            _name = string.Empty;
            _extendedProperties = new Dictionary<string, object>(5);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the collection of children for this element.
        /// </summary>
        public ReadOnlyCollection<ICodeElement> Children
        {
            get
            {
                return BaseChildren.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the element type.
        /// </summary>
        public abstract ElementType ElementType
        {
            get;
        }

        /// <summary>
        /// Gets or sets the code element name.
        /// </summary>
        public virtual string Name
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
        /// Gets or sets the parent element.
        /// </summary>
        public virtual ICodeElement Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (value != _parent)
                {
                    if (_parent != null)
                    {
                        _parent.RemoveChild(this);
                    }

                    _parent = value;
                    if (_parent != null && !_parent.Children.Contains(this))
                    {
                        _parent.AddChild(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the base child collection.
        /// </summary>
        protected List<ICodeElement> BaseChildren
        {
            get
            {
                if (_children == null)
                {
                    lock (_childrenLock)
                    {
                        if (_children == null)
                        {
                            _children = new List<ICodeElement>();
                        }
                    }
                }

                return _children;
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets or sets an extended property.
        /// </summary>
        /// <param name="key">Extended property name/key.</param>
        /// <returns></returns>
        public virtual object this[string key]
        {
            get
            {
                object value = null;
                _extendedProperties.TryGetValue(key, out value);

                return value;
            }
            set
            {
                if (value == null)
                {
                    _extendedProperties.Remove(key);
                }
                else
                {
                    _extendedProperties[key] = value;
                }
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Allows an ICodeElementVisitor to process (or visit) this element.
        /// </summary>
        /// <remarks>See the Gang of Four Visitor design pattern.</remarks>
        /// <param name="visitor">Visitor to accept the code element.</param>
        public abstract void Accept(ICodeElementVisitor visitor);

        /// <summary>
        /// Adds a child to this element.
        /// </summary>
        /// <param name="childElement">Child to add.</param>
        public virtual void AddChild(ICodeElement childElement)
        {
            if (childElement != null)
            {
                lock (_childrenLock)
                {
                    if (childElement != null && !BaseChildren.Contains(childElement))
                    {
                        BaseChildren.Add(childElement);
                        childElement.Parent = this;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all child elements.
        /// </summary>
        public virtual void ClearChildren()
        {
            lock (_childrenLock)
            {
                for (int childIndex = 0; childIndex < BaseChildren.Count; childIndex++)
                {
                    ICodeElement child = BaseChildren[childIndex];
                    if (child != null && child.Parent != null)
                    {
                        child.Parent = null;
                        childIndex--;
                    }
                }

                BaseChildren.Clear();
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            CodeElement clone = DoClone();

            // Copy base state
            clone._name = _name;
            lock (_childrenLock)
            {
                for (int childIndex = 0; childIndex < BaseChildren.Count; childIndex++)
                {
                    ICodeElement child = BaseChildren[childIndex];
                    ICodeElement childClone = child.Clone() as ICodeElement;

                    childClone.Parent = clone;
                }
            }

            foreach (string key in _extendedProperties.Keys)
            {
                clone[key] = _extendedProperties[key];
            }

            return clone;
        }

        /// <summary>
        /// Inserts a child element at the specified index.
        /// </summary>
        /// <param name="index">Index to insert at.</param>
        /// <param name="childElement">Element to insert.</param>
        public virtual void InsertChild(int index, ICodeElement childElement)
        {
            if (childElement != null)
            {
                lock (_childrenLock)
                {
                    if (BaseChildren.Contains(childElement))
                    {
                        BaseChildren.Remove(childElement);
                    }

                    BaseChildren.Insert(index, childElement);
                    childElement.Parent = this;
                }
            }
        }

        /// <summary>
        /// Removes a child from this element.
        /// </summary>
        /// <param name="childElement">Child element to remove.</param>
        public virtual void RemoveChild(ICodeElement childElement)
        {
            if (childElement != null && BaseChildren.Contains(childElement))
            {
                lock (_childrenLock)
                {
                    if (childElement != null && BaseChildren.Contains(childElement))
                    {
                        BaseChildren.Remove(childElement);
                        childElement.Parent = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the string representation of this object.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return _name;
        }

        /// <summary>
        /// Gets a string representation of this object using the specified format string.
        /// </summary>
        /// <param name="format">Code element format string.</param>
        /// <returns>String representation.</returns>
        public virtual string ToString(string format)
        {
            return ElementUtilities.Format(format, this);
        }

        /// <summary>
        /// Creates a clone of the instance and assigns any state.
        /// </summary>
        /// <returns>Clone of the code element.</returns>
        protected abstract CodeElement DoClone();

        #endregion Methods
    }
}