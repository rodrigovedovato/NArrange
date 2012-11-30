namespace NArrange.Core.CodeElements
{
    using System;

    /// <summary>
    /// Interface implementation definition for type and member references to interfaces and 
    /// interface members.
    /// </summary>
    public sealed class InterfaceReference : ICloneable
    {
        #region Fields

        /// <summary>
        /// Referenced interface name.
        /// </summary>
        private string _name;

        /// <summary>
        /// Reference type.
        /// </summary>
        private InterfaceReferenceType _referenceType;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new InterfaceImplementation.
        /// </summary>
        public InterfaceReference()
        {
        }

        /// <summary>
        /// Creates a new InterfaceImplementation with the specified name and reference type.
        /// </summary>
        /// <param name="name">The interface name.</param>
        /// <param name="referenceType">Type of the reference.</param>
        public InterfaceReference(string name, InterfaceReferenceType referenceType)
        {
            _name = name;
            _referenceType = referenceType;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the referenced interface name.
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
        /// Gets or sets the interface reference type.
        /// </summary>
        public InterfaceReferenceType ReferenceType
        {
            get
            {
                return _referenceType;
            }
            set
            {
                _referenceType = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates a clone of this instance
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            InterfaceReference clone = new InterfaceReference();

            //
            // Copy state
            //
            clone._name = _name;
            clone._referenceType = _referenceType;

            return clone;
        }

        /// <summary>
        /// Gets the string representation of this object.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return _name;
        }

        #endregion Methods
    }
}