namespace NArrange.Tests.Gui
{
    using NArrange.Gui;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the BaseForm form.
    /// </summary>
    [TestFixture]
    public class BaseFormTests
    {
        #region Methods

        /// <summary>
        /// Verify that a BaseForm can be created without throwing an exception.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            BaseForm form = new BaseForm();
            Assert.IsNotNull(form, "Expected a valid base form instance.");
        }

        #endregion Methods
    }
}