namespace NArrange.Tests.Gui.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;

    using NArrange.Core.Configuration;
    using NArrange.Gui.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the ConfigurationElementTypeDescriptionProvider
    /// class.
    /// </summary>
    [TestFixture]
    public class ConfigurationElementTypeDescriptionProviderTests
    {
        #region Methods

        /// <summary>
        /// Tests the GetTypeDescriptor method.
        /// </summary>
        [Test]
        public void GetTypeDescriptorTest()
        {
            Type configType = typeof(ElementConfiguration);

            ConfigurationElementTypeDescriptionProvider typeDescriptionProvider =
                new ConfigurationElementTypeDescriptionProvider(configType);

            ICustomTypeDescriptor typeDescriptor =
                typeDescriptionProvider.GetTypeDescriptor(configType);

            Assert.IsNotNull(
                typeDescriptor,
                "Expected a valid type descriptor instance to be returned from GetTypeDescriptor().");

            PropertyDescriptorCollection properties = typeDescriptor.GetProperties();

            PropertyDescriptor elementsProperty = properties["Elements"];
            Assert.IsNotNull(elementsProperty, "Expected property 'Elements' to be present.");

            object editor = elementsProperty.GetEditor(typeof(UITypeEditor));
            Assert.IsNotNull(editor, "Expected an editor instance.");
            Assert.AreEqual(
                typeof(ConfigurationElementCollectionEditor),
                editor.GetType(),
                "Unexpected editor type for the Elements property.");
        }

        #endregion Methods
    }
}