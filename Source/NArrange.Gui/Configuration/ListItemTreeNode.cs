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
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Gui.Configuration
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Tree node for a list item.
    /// </summary>
    public sealed class ListItemTreeNode : TreeNode
    {
        #region Fields

        /// <summary>
        /// Component being edited.
        /// </summary>
        private object _component;

        /// <summary>
        /// Context menu.
        /// </summary>
        private ContextMenuStrip _contextMenu;

        /// <summary>
        /// Item in the list associated with the node.
        /// </summary>
        private object _listItem;

        /// <summary>
        /// Parent list property.
        /// </summary>
        private PropertyDescriptor _listProperty;

        /// <summary>
        /// Move down menu item.
        /// </summary>
        private ToolStripMenuItem _moveDownMenuItem;

        /// <summary>
        /// Move up menu item.
        /// </summary>
        private ToolStripMenuItem _moveUpMenuItem;

        /// <summary>
        /// Remove menu item.
        /// </summary>
        private ToolStripMenuItem _removeMenuItem;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new ListItemTreeNode.
        /// </summary>
        /// <param name="listProperty">The list property.</param>
        /// <param name="component">The component.</param>
        /// <param name="listItem">The list item.</param>
        public ListItemTreeNode(PropertyDescriptor listProperty, object component, object listItem)
        {
            _listProperty = listProperty;
            _component = component;
            _listItem = listItem;

            this.Tag = _listItem;

            Initialize();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the list item associated with this node.
        /// </summary>
        public object ListItem
        {
            get
            {
                return _listItem;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Moves this list item node down in the collection.
        /// </summary>
        public void MoveDown()
        {
            IList list = this._listProperty.GetValue(_component) as IList;
            if (list != null && list.Contains(_listItem))
            {
                int index = list.IndexOf(_listItem);
                if (index < list.Count - 1)
                {
                    TreeNode parent = this.Parent;
                    if (parent != null)
                    {
                        parent.Nodes.Remove(this);
                    }
                    list.Remove(_listItem);

                    int newIndex = ++index;
                    if (parent != null)
                    {
                        parent.Nodes.Insert(newIndex, this);
                    }
                    list.Insert(newIndex, _listItem);
                }

                this.Select();
                this.UpdateMenu();
            }
        }

        /// <summary>
        /// Moves this list item node down in the collection.
        /// </summary>
        public void MoveUp()
        {
            IList list = this._listProperty.GetValue(_component) as IList;
            if (list != null && list.Contains(_listItem))
            {
                int index = list.IndexOf(_listItem);
                if (index > 0)
                {
                    TreeNode parent = this.Parent;
                    if (parent != null)
                    {
                        parent.Nodes.Remove(this);
                    }
                    list.Remove(_listItem);

                    int newIndex = --index;
                    if (parent != null)
                    {
                        parent.Nodes.Insert(newIndex, this);
                    }
                    list.Insert(newIndex, _listItem);
                }

                this.Select();
                this.UpdateMenu();
            }
        }

        /// <summary>
        /// Removes the item from the collection.
        /// </summary>
        public void RemoveItem()
        {
            IList list = this._listProperty.GetValue(_component) as IList;
            if (list != null && list.Contains(_listItem))
            {
                if (this.Parent != null)
                {
                    this.Parent.TreeView.SelectedNode = this.Parent;
                }

                list.Remove(_listItem);
            }
        }

        /// <summary>
        /// Updates the context menu for the tree node.
        /// </summary>
        public void UpdateMenu()
        {
            IList list = this._listProperty.GetValue(_component) as IList;
            if (list != null && list.Contains(_listItem))
            {
                int index = list.IndexOf(_listItem);
                this._moveUpMenuItem.Enabled = index > 0;
                this._moveDownMenuItem.Enabled = index < list.Count - 1;
            }
        }

        /// <summary>
        /// Updates the display text.
        /// </summary>
        public void UpdateText()
        {
            this.Text = _listItem.ToString();
        }

        /// <summary>
        /// Event handler for the Move Down menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleMoveDownMenuItemClick(object sender, EventArgs e)
        {
            MoveDown();
        }

        /// <summary>
        /// Event handler for the Move Up menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleMoveUpMenuItemClick(object sender, EventArgs e)
        {
            MoveUp();
        }

        /// <summary>
        /// Event handler for the Remove menu item click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleRemoveMenuItemClick(object sender, EventArgs e)
        {
            this.RemoveItem();
        }

        /// <summary>
        /// Initializes this tree node.
        /// </summary>
        private void Initialize()
        {
            this.UpdateText();

            _contextMenu = new ContextMenuStrip();

            _removeMenuItem = new ToolStripMenuItem("&Remove");
            _removeMenuItem.Click += new EventHandler(HandleRemoveMenuItemClick);
            _removeMenuItem.ShortcutKeys = Keys.Delete;
            _contextMenu.Items.Add(_removeMenuItem);

            _moveUpMenuItem = new ToolStripMenuItem("Move &Up");
            _moveUpMenuItem.Click += new EventHandler(HandleMoveUpMenuItemClick);
            _moveUpMenuItem.ShortcutKeys = Keys.Control | Keys.Up;
            _contextMenu.Items.Add(_moveUpMenuItem);

            _moveDownMenuItem = new ToolStripMenuItem("Move &Down");
            _moveDownMenuItem.Click += new EventHandler(HandleMoveDownMenuItemClick);
            _moveDownMenuItem.ShortcutKeys = Keys.Control | Keys.Down;
            _contextMenu.Items.Add(_moveDownMenuItem);

            this.UpdateMenu();

            this.ContextMenuStrip = _contextMenu;
        }

        /// <summary>
        /// Sets this node as the selected node in the tree view.
        /// </summary>
        private void Select()
        {
            if (this.TreeView != null)
            {
                this.TreeView.SelectedNode = this;
            }
        }

        #endregion Methods
    }
}