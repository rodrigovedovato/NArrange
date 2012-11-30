namespace NArrange.Tests.Core.CodeElements
{
    using NArrange.Core.CodeElements;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the TypeParameter class.
    /// </summary>
    [TestFixture]
    public class TypeParameterTests
    {
        #region Methods

        /// <summary>
        /// Tests the ICloneable implementation.
        /// </summary>
        [Test]
        public void CloneTest()
        {
            TypeParameter typeParameter = new TypeParameter();
            typeParameter.Name = "T";
            typeParameter.AddConstraint("IDisposable");
            typeParameter.AddConstraint("new()");

            TypeParameter clone = typeParameter.Clone() as TypeParameter;
            Assert.IsNotNull(clone, "Clone should return a TypeParameter instance.");

            Assert.AreEqual(typeParameter.Name, clone.Name, "Name property was not copied correctly.");
            Assert.AreEqual(typeParameter.Constraints.Count, clone.Constraints.Count, "Constraints property was not copied correctly.");
            Assert.AreEqual(typeParameter.Constraints[0], clone.Constraints[0], "Constraints property was not copied correctly.");
            Assert.AreEqual(typeParameter.Constraints[1], clone.Constraints[1], "Constraints property was not copied correctly.");
        }

        /// <summary>
        /// Tests the construction of a type parameter.
        /// </summary>
        [Test]
        public void CreateTest()
        {
            TypeParameter typeParameter = new TypeParameter();

            //
            // Verify default values.
            //
            Assert.AreEqual(string.Empty, typeParameter.Name, "Unexpected default value for name.");
            Assert.IsNotNull(typeParameter.Constraints, "Constraints collection should be instantiated.");
            Assert.AreEqual(0, typeParameter.Constraints.Count, "Constraints collection should be empty.");
        }

        #endregion Methods
    }
}