namespace NArrange.Tests.Core.Configuration
{
    using NArrange.Core;
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the InterfaceReference class.
    /// </summary>
    [TestFixture]
    public class InterfaceReferenceTests
    {
        #region Methods

        /// <summary>
        /// Tests creation of an InterfaceReference instance.
        /// </summary>
        [Test]
        public void CreateDefaultTest()
        {
            InterfaceReference reference1 = new InterfaceReference();
            Assert.IsNull(reference1.Name, "Unexpected default value for Name.");
            Assert.AreEqual(InterfaceReferenceType.None, reference1.ReferenceType, "Unexpected default value for ReferenceType.");
        }

        /// <summary>
        /// Tests creation of an InterfaceReference instance.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            InterfaceReference reference1 = new InterfaceReference("IDisposable", InterfaceReferenceType.Interface);
            Assert.AreEqual("IDisposable", reference1.Name, "Unexpected value for Name.");
            Assert.AreEqual(InterfaceReferenceType.Interface, reference1.ReferenceType, "Unexpected value for ReferenceType.");
        }

        /// <summary>
        /// Tests the Name property.
        /// </summary>
        [Test]
        public void NameTest()
        {
            InterfaceReference reference = new InterfaceReference();
            reference.Name = "Test";
            Assert.AreEqual("Test", reference.Name, "Unexpected value for Name.");
        }

        /// <summary>
        /// Tests the ReferenceType property.
        /// </summary>
        [Test]
        public void ReferenceTypeTest()
        {
            InterfaceReference reference = new InterfaceReference();
            reference.ReferenceType = InterfaceReferenceType.Class;
            Assert.AreEqual(InterfaceReferenceType.Class, reference.ReferenceType, "Unexpected value for ReferenceType.");
        }

        #endregion Methods
    }
}