namespace NArrange.Tests.VisualBasic
{
    using System.CodeDom.Compiler;

    using NArrange.Tests.Core;
    using NArrange.VisualBasic;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for parsing, arranging and writing VB test source code files.
    /// </summary>
    [TestFixture]
    public class VBWriteArrangedTests : WriteArrangedTests<VBParser, VBWriter>
    {
        #region Properties

        /// <summary>
        /// Gets a list of valid test files.
        /// </summary>
        public override ISourceCodeTestFile[] ValidTestFiles
        {
            get
            {
                return new ISourceCodeTestFile[]
                {
                    VBTestUtilities.GetAssemblyAttributesFile(),
                    VBTestUtilities.GetClassAttributesFile(),
                    VBTestUtilities.GetClassDefinitionFile(),
                    VBTestUtilities.GetClassMembersFile(),
                    VBTestUtilities.GetInterfaceDefinitionFile(),
                    VBTestUtilities.GetMultiClassDefinitionFile(),
                    VBTestUtilities.GetMultipleNamespaceFile(),
                    VBTestUtilities.GetOperatorsFile(),
                    VBTestUtilities.GetSingleNamespaceFile(),
                    VBTestUtilities.GetStructDefinitionFile(),
                    VBTestUtilities.GetModuleDefinitionFile(),
                };
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Compiles source code text to the specified assembly.
        /// </summary>
        /// <param name="text">Text to compile.</param>
        /// <param name="assemblyName">Output assembly name.</param>
        /// <returns>Compiler results.</returns>
        protected override CompilerResults Compile(string text, string assemblyName)
        {
            return VBTestFile.Compile(text, assemblyName);
        }

        #endregion Methods
    }
}