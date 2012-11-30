namespace NArrange.Tests.VisualBasic
{
    /// <summary>
    /// Visual Basic test utilities.
    /// </summary>
    public static class VBTestUtilities
    {
        #region Methods

        /// <summary>
        /// Assembly attributes test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetAssemblyAttributesFile()
        {
            return new VBTestFile("AssemblyAttributes.vb");
        }

        /// <summary>
        /// Class attributes test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetClassAttributesFile()
        {
            return new VBTestFile("ClassAttributes.vb");
        }

        /// <summary>
        /// Class definition test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetClassDefinitionFile()
        {
            return new VBTestFile("ClassDefinition.vb");
        }

        /// <summary>
        /// Class members test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetClassMembersFile()
        {
            return new VBTestFile("ClassMembers.vb");
        }

        /// <summary>
        /// Interface definition test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetInterfaceDefinitionFile()
        {
            return new VBTestFile("InterfaceDefinition.vb");
        }

        /// <summary>
        /// Interface implementation test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetInterfaceImplementationFile()
        {
            return new VBTestFile("InterfaceImplementation.vb");
        }

        /// <summary>
        /// Module definition test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetModuleDefinitionFile()
        {
            return new VBTestFile("ModuleDefinition.vb");
        }

        /// <summary>
        /// Multiple class definition test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetMultiClassDefinitionFile()
        {
            return new VBTestFile("MultiClassDefinition.vb");
        }

        /// <summary>
        /// Multiple namespace test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetMultipleNamespaceFile()
        {
            return new VBTestFile("MultipleNamespace.vb");
        }

        /// <summary>
        /// Operators test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetOperatorsFile()
        {
            return new VBTestFile("Operators.vb");
        }

        /// <summary>
        /// Single namespace test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetSingleNamespaceFile()
        {
            return new VBTestFile("SingleNamespace.vb");
        }

        /// <summary>
        /// Structure definition test file
        /// </summary>
        /// <returns>The test file.</returns>
        public static VBTestFile GetStructDefinitionFile()
        {
            return new VBTestFile("StructDefinition.vb");
        }

        #endregion Methods
    }
}