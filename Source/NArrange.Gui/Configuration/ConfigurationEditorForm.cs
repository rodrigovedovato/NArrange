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
    using System.Globalization;
    using System.Text;
    using System.Windows.Forms;

    using NArrange.Core.Configuration;

    /// <summary>
    /// Form for editing a code configuration.
    /// </summary>
    public partial class ConfigurationEditorForm : BaseForm
    {
        #region Fields

        /// <summary>
        /// Whether or not a configuration can be selected for editing.
        /// </summary>
        private bool _canSelectConfig = true;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new ConfigurationEditorForm.
        /// </summary>
        public ConfigurationEditorForm()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether a configuration can currently be selected.
        /// </summary>
        private bool CanSelectConfig
        {
            get
            {
                return _canSelectConfig;
            }
            set
            {
                _canSelectConfig = value;

                _buttonCancel.Enabled = !value;
                _buttonSave.Enabled = !value;
                _configurationEditorControl.Enabled = !value;
                _configurationPicker.Enabled = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates a new configuration using the specified filename and updates
        /// the UI.
        /// </summary>
        /// <param name="filename">The filename.</param>
        private void CreateConfiguration(string filename)
        {
            try
            {
                CodeConfiguration configuration = CodeConfiguration.Default.Clone() as CodeConfiguration;
                configuration.Save(filename);

                _configurationEditorControl.Configuration = configuration;
                this.CanSelectConfig = false;
            }
            catch
            {
                string message = string.Format(
                    CultureInfo.CurrentUICulture,
                    "Unable to create configuration file {0}.",
                    filename);

                MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler for the Cancel button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleButtonCancelClick(object sender, EventArgs e)
        {
            this.CanSelectConfig = true;
            this._configurationEditorControl.Configuration = null;
            this._configurationPicker.Refresh();
        }

        /// <summary>
        /// Event handler for the Create button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleButtonCreateClick(object sender, EventArgs e)
        {
            this.CreateConfiguration(_configurationPicker.SelectedFile);
        }

        /// <summary>
        /// Event handler for the Save button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleButtonSaveClick(object sender, EventArgs e)
        {
            this.CanSelectConfig = true;
            this.SaveConfiguration(_configurationPicker.SelectedFile);
            this._configurationPicker.Refresh();
        }

        /// <summary>
        /// Event handler for the configuration picker EditClick event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleConfigurationPickerEditClick(object sender, EventArgs e)
        {
            this.LoadConfiguration(_configurationPicker.SelectedFile);
        }

        /// <summary>
        /// Loads the configuration file into the editor and updates the UI state.
        /// </summary>
        /// <param name="filename">The filename.</param>
        private void LoadConfiguration(string filename)
        {
            if (filename.Length == 0)
            {
                MessageBox.Show(this, "Please select a configuration to load.", this.Text);
            }
            else
            {
                try
                {
                    CodeConfiguration configuration = CodeConfiguration.Load(filename, false);
                    _configurationEditorControl.Configuration = configuration;
                    this.CanSelectConfig = false;
                }
                catch (Exception ex)
                {
                    StringBuilder messageBuilder = new StringBuilder(
                         string.Format(
                         CultureInfo.CurrentUICulture,
                        "Unable to load configuration file {0}: {1}",
                        filename,
                        ex.Message));
                    if (ex.InnerException != null)
                    {
                        messageBuilder.AppendFormat(" {0}", ex.InnerException.Message);
                    }

                    MessageBox.Show(
                        this,
                        messageBuilder.ToString(),
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Saves the current configuration to the specified file and
        /// updates the UI.
        /// </summary>
        /// <param name="filename">The filename.</param>
        private void SaveConfiguration(string filename)
        {
            try
            {
                _configurationEditorControl.Configuration.Save(filename);
                this.CanSelectConfig = true;
                this._configurationEditorControl.Configuration = null;
            }
            catch (Exception ex)
            {
                StringBuilder messageBuilder = new StringBuilder(
                        string.Format(
                        CultureInfo.CurrentUICulture,
                        "Unable to save configuration file {0}: {1}",
                        filename,
                        ex.Message));
                if (ex.InnerException != null)
                {
                    messageBuilder.AppendFormat(" {0}", ex.InnerException.Message);
                }

                MessageBox.Show(
                    this,
                    messageBuilder.ToString(),
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #endregion Methods
    }
}