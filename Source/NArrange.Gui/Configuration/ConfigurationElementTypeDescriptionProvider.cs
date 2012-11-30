#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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

namespace NArrange.Gui.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using NArrange.Core.Configuration;

    /// <summary>
    /// Custom type descriptor provider for the CodeConfiguration class.
    /// </summary>
    public sealed class ConfigurationElementTypeDescriptionProvider : TypeDescriptionProvider
    {
        #region Fields

        /// <summary>
        /// Base type descriptor provider.
        /// </summary>
        private TypeDescriptionProvider _baseProvider;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new ConfigurationElementTypeDescriptionProvider.
        /// </summary>
        /// <param name="type">Type to get property descriptions for.</param>
        public ConfigurationElementTypeDescriptionProvider(Type type)
        {
            _baseProvider = TypeDescriptor.GetProvider(type);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Create and return the custom type descriptor and chain it with the original
        /// custom type descriptor.
        /// </summary>
        /// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
        /// <param name="instance">An instance of the type. Can be null if no instance was passed to the <see cref="T:System.ComponentModel.TypeDescriptor"></see>.</param>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.ICustomTypeDescriptor"></see> that can provide metadata for the type.
        /// </returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new CodeConfigurationTypeDescriptor(_baseProvider.GetTypeDescriptor(objectType, instance));
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Custom type descriptor. It creates a new property and returns it along
        /// with the original list.
        /// </summary>
        private sealed class CodeConfigurationTypeDescriptor : CustomTypeDescriptor
        {
            #region Constructors

            /// <summary>
            /// Creates a new type descriptor using the specified parent.
            /// </summary>
            /// <param name="parent">The parent custom type descriptor.</param>
            internal CodeConfigurationTypeDescriptor(ICustomTypeDescriptor parent)
                : base(parent)
            {
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Gets the PropertyDescriptors for the Type.
            /// </summary>
            /// <param name="attributes">An array of attributes to use as a filter. This can be null.</param>
            /// <returns>
            /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"></see> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"></see>.
            /// </returns>
            /// <remarks>
            /// All ConfigurationElementCollection properties are overriden to specify the
            /// collection editor.
            /// </remarks>
            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                // Enumerate the original set of properties and create our new set with it
                PropertyDescriptorCollection originalProperties = base.GetProperties(attributes);
                List<PropertyDescriptor> newProperties = new List<PropertyDescriptor>();
                foreach (PropertyDescriptor originalProperty in originalProperties)
                {
                    if (originalProperty.PropertyType == typeof(ConfigurationElementCollection))
                    {
                        newProperties.Add(new ElementCollectionPropertyDescriptor(originalProperty));
                    }
                    else
                    {
                        newProperties.Add(originalProperty);
                    }
                }

                // Finally return the list
                return new PropertyDescriptorCollection(newProperties.ToArray());
            }

            /// <summary>
            /// This method add a new property to the original collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"></see> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"></see>.
            /// </returns>
            public override PropertyDescriptorCollection GetProperties()
            {
                return this.GetProperties(null);
            }

            #endregion Methods
        }

        /// <summary>
        /// Elements property descriptor.
        /// </summary>
        private class ElementCollectionPropertyDescriptor : PropertyDescriptor
        {
            #region Fields

            /// <summary>
            /// Original, reflected property.
            /// </summary>
            private PropertyDescriptor _originalProperty;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Creates a new ElementCollectionPropertyDescriptor.
            /// </summary>
            /// <param name="originalProperty">The original property.</param>
            public ElementCollectionPropertyDescriptor(PropertyDescriptor originalProperty)
                : base(originalProperty, new Attribute[] { })
            {
                _originalProperty = originalProperty;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the type of the component for which this property belongs.
            /// </summary>
            public override Type ComponentType
            {
                get
                {
                    return _originalProperty.ComponentType;
                }
            }

            /// <summary>
            /// Gets a value indicating whether or not this property is read-only.
            /// </summary>
            public override bool IsReadOnly
            {
                get
                {
                    return _originalProperty.IsReadOnly;
                }
            }

            /// <summary>
            /// Gets the Type of this property.
            /// </summary>
            public override Type PropertyType
            {
                get
                {
                    return _originalProperty.PropertyType;
                }
            }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Gets a value indicating whether or not the properties value can be
            /// reset for the specified component.
            /// </summary>
            /// <param name="component">The component to test for reset capability.</param>
            /// <returns>
            /// true if resetting the component changes its value; otherwise, false.
            /// </returns>
            public override bool CanResetValue(object component)
            {
                return _originalProperty.CanResetValue(component);
            }

            /// <summary>
            /// Gets the editor for this property.
            /// </summary>
            /// <param name="editorBaseType">The base type of editor, which is used to differentiate between multiple editors that a property supports.</param>
            /// <returns>
            /// An instance of the requested editor type, or null if an editor cannot be found.
            /// </returns>
            public override object GetEditor(Type editorBaseType)
            {
                return new ConfigurationElementCollectionEditor(_originalProperty.PropertyType);
            }

            /// <summary>
            /// Gets the property value for the specified component.
            /// </summary>
            /// <param name="component">The component with the property for which to retrieve the value.</param>
            /// <returns>
            /// The value of a property for a given component.
            /// </returns>
            public override object GetValue(object component)
            {
                return _originalProperty.GetValue(component);
            }

            /// <summary>
            /// Resets the value for this property.
            /// </summary>
            /// <param name="component">The component with the property value that is to be reset to the default value.</param>
            public override void ResetValue(object component)
            {
                _originalProperty.ResetValue(component);
            }

            /// <summary>
            /// Sets the value for this property.
            /// </summary>
            /// <param name="component">The component with the property value that is to be set.</param>
            /// <param name="value">The new value.</param>
            public override void SetValue(object component, object value)
            {
                _originalProperty.SetValue(component, value);
            }

            /// <summary>
            /// Gets a value indicating whether the property should be
            /// serialized by designers.
            /// </summary>
            /// <param name="component">The component with the property to be examined for persistence.</param>
            /// <returns>
            /// true if the property should be persisted; otherwise, false.
            /// </returns>
            public override bool ShouldSerializeValue(object component)
            {
                return _originalProperty.ShouldSerializeValue(component);
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}