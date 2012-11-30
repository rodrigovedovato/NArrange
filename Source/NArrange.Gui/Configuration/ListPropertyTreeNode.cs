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
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Tree node for a list component property.
    /// </summary>
    public sealed class ListPropertyTreeNode : PropertyTreeNode
    {
        #region Fields

        /// <summary>
        /// Add menu item.
        /// </summary>
        private ToolStripMenuItem _addMenuItem;

        /// <summary>
        /// Context menu.
        /// </summary>
        private ContextMenuStrip _contextMenu;

        /// <summary>
        /// Types valid for the list.
        /// </summary>
        private Type[] _newItemTypes;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new ListPropertyTreeNode.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="component">The component.</param>
        /// <param name="newItemTypes">The new item types.</param>
        public ListPropertyTreeNode(
            PropertyDescriptor property, object component, Type[] newItemTypes)
            : base(property, component)
        {
            _newItemTypes = newItemTypes;

            Initialize();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the display name for a type.
        /// </summary>
        /// <param name="type">The type to get the display name for.</param>
        /// <returns>The dispaly name.</returns>
        private static string GetDisplayName(Type type)
        {
            string displayName = type.Name;

            object[] displayNameAttributes = type.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (displayNameAttributes.Length > 0)
            {
                DisplayNameAttribute displayNameAttribute = (DisplayNameAttribute)displayNameAttributes[0];
                displayName = displayNameAttribute.DisplayName;
            }

            return displayName;
        }

        /// <summary>
        /// Adds a new item to the list of the specified type.
        /// </summary>
        /// <param name="type">The type to add.</param>
        private void AddItem(Type type)
        {
            object instance = Activator.CreateInstance(type);
            IList list = this.PropertyValue as IList;
            if (list != null)
            {
                list.Add(instance);
            }
        }

        /// <summary>
        /// Initializes this tree node.
        /// </summary>
        private void Initialize()
        {
            _contextMenu = new ContextMenuStrip();

            if (_newItemTypes != null && _newItemTypes.Length > 0)
            {
                _addMenuItem = new ToolStripMenuItem("&Add");

                if (_newItemTypes.Length == 1)
                {
                    _addMenuItem.Click += delegate(object sender, EventArgs e)
                    {
                        this.AddItem(_newItemTypes[0]);
                    };
                }
                else
                {
                    for (int typeIndex = 0; typeIndex < _newItemTypes.Length; typeIndex++)
                    {
                        Type type = _newItemTypes[typeIndex];
                        string typeName = GetDisplayName(type);
                        ToolStripMenuItem addTypeMenuItem = new ToolStripMenuItem(typeName);
                        addTypeMenuItem.Click += delegate(object sender, EventArgs e)
                       {
                           this.AddItem(type);
                       };

                        _addMenuItem.DropDownItems.Add(addTypeMenuItem);
                    }
                }

                _contextMenu.Items.Add(_addMenuItem);
            }

            this.ContextMenuStrip = _contextMenu;
        }

        #endregion Methods
    }
}