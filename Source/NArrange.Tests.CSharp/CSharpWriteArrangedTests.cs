namespace NArrange.Tests.CSharp
{
    using System.CodeDom.Compiler;

    using NArrange.CSharp;
    using NArrange.Tests.Core;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for parsing, arranging and writing C# test source code files.
    /// </summary>
    [TestFixture]
    public class CSharpWriteArrangedTests : WriteArrangedTests<CSharpParser, CSharpWriter>
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
                    CSharpTestUtilities.GetAssemblyAttributesFile(),
                    CSharpTestUtilities.GetClassAttributesFile(),
                    CSharpTestUtilities.GetClassDefinitionFile(),
                    CSharpTestUtilities.GetClassMembersFile(),
                    CSharpTestUtilities.GetInterfaceDefinitionFile(),
                    CSharpTestUtilities.GetMultiClassDefinitionFile(),
                    CSharpTestUtilities.GetMultipleNamespaceFile(),
                    CSharpTestUtilities.GetOperatorsFile(),
                    CSharpTestUtilities.GetSingleNamespaceFile(),
                    CSharpTestUtilities.GetStructDefinitionFile(),
                    CSharpTestUtilities.GetUTF8File()
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
            return CSharpTestFile.Compile(text, assemblyName);
        }

        #endregion Methods
    }
}