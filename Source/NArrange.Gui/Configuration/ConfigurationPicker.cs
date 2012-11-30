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
//      - Fixed the shifting of the Edit button under Mono
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Gui.Configuration
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using NArrange.Core;

    /// <summary>
    /// Control that allows the user to select, create or edit a configuration.
    /// </summary>
    public partial class ConfigurationPicker : UserControl
    {
        #region Constructors

        /// <summary>
        /// Creates a new ConfigurationPicker.
        /// </summary>
        public ConfigurationPicker()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when the create button is clicked.
        /// </summary>
        public event EventHandler CreateClick;

        /// <summary>
        /// Occurs when the edit button is clicked.
        /// </summary>
        public event EventHandler EditClick;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the selected configuration file.
        /// </summary>
        public string SelectedFile
        {
            get
            {
                return this._textBoxFile.Text;
            }
            set
            {
                this._textBoxFile.Text = value;
                this.UpdateButtons();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Refreshes this control.
        /// </summary>
        public override void Refresh()
        {
            this.UpdateButtons();
            base.Refresh();
        }

        /// <summary>
        /// Event handler for the Browse button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleButtonBrowseClick(object sender, EventArgs e)
        {
            _openFileDialog.FileName = _textBoxFile.Text;
            DialogResult result = _openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = _openFileDialog.FileName;
                _textBoxFile.Text = filename;

                this.OnEditClick();
            }
        }

        /// <summary>
        /// Event handler for the Create button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleButtonCreateClick(object sender, EventArgs e)
        {
            this.OnCreateClick();
        }

        /// <summary>
        /// Event handler for the Edit button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleButtonEditClick(object sender, EventArgs e)
        {
            this.OnEditClick();
        }

        /// <summary>
        /// Event handler for the file textbox TextChanged event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleTextBoxFileTextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        /// <summary>
        /// Called when the Create button is clicked.
        /// </summary>
        private void OnCreateClick()
        {
            EventHandler temp = CreateClick;
            if (temp != null)
            {
                temp(this, new EventArgs());
            }
        }

        /// <summary>
        /// Called when the Edit button is clicked.
        /// </summary>
        private void OnEditClick()
        {
            EventHandler temp = EditClick;
            if (temp != null)
            {
                temp(this, new EventArgs());
            }
        }

        /// <summary>
        /// Updates the button state when the selected file changes.
        /// </summary>
        private void UpdateButtons()
        {
            string filename = _textBoxFile.Text.Trim();
            bool fileEntered = filename.Length > 0;
            _buttonEdit.Enabled = fileEntered;
            _buttonCreate.Enabled = fileEntered;

            bool fileExists = fileEntered && File.Exists(filename);
            _buttonCreate.Visible = !fileExists;
            _buttonEdit.Visible = fileExists;

            if (MonoUtilities.IsMonoRuntime)
            {
                // Not sure why in Mono, but the edit button shifts when it becomes
                // visible.  Let's just reallign it to the position of the hidden
                // create button.
                _buttonEdit.Location = _buttonCreate.Location;
            }
        }

        #endregion Methods
    }
}