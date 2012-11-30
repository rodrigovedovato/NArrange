namespace NArrange.Tests.VisualBasic
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using Microsoft.VisualBasic;

    using NArrange.Tests.Core;

    using NUnit.Framework;

    /// <summary>
    /// Visual Basic test file information.
    /// </summary>
    public class VBTestFile : ISourceCodeTestFile
    {
        #region Fields

        /// <summary>
        /// Cache of compiled test source files.
        /// </summary>
        private static Dictionary<string, Assembly> _compiledSourceFiles = new Dictionary<string, Assembly>();

        /// <summary>
        /// Assembly for the test file.
        /// </summary>
        private Assembly _assembly;

        /// <summary>
        /// Test file resource name.
        /// </summary>
        private string _resourceName;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new test file using the specified resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        public VBTestFile(string resourceName)
        {
            _resourceName = resourceName;
            _assembly = GetAssembly(resourceName);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the assembly associated with the test file
        /// </summary>
        public Assembly Assembly
        {
            get
            {
                return _assembly;
            }
        }

        /// <summary>
        /// Gets the name of the test file.
        /// </summary>
        public string Name
        {
            get
            {
                return _resourceName;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Compiles VB source code
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The assembly name.</param>
        /// <returns>Compiler results.</returns>
        public static CompilerResults Compile(string source, string name)
        {
            //
            // Compile the test source file
            //
            CodeDomProvider provider = VBCodeProvider.CreateProvider("VisualBasic");

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;

            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Data.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, source);

            return results;
        }

        /// <summary>
        /// Retrieves a reader for the specified resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>TextReader for the test file resource.</returns>
        public static TextReader GetTestFileReader(string resourceName)
        {
            return new StreamReader(GetTestFileStream(resourceName), Encoding.Default);
        }

        /// <summary>
        /// Opens a test file resource stream.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>Stream for the test file resource.</returns>
        public static Stream GetTestFileStream(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(
               typeof(VBTestUtilities), "TestSourceFiles." + resourceName);

            Assert.IsNotNull(stream, "Test stream could not be retrieved.");

            return stream;
        }

        /// <summary>
        /// Gets a TextReader for this test file
        /// </summary>
        /// <returns>Text reader for the test file.</returns>
        public TextReader GetReader()
        {
            return GetTestFileReader(_resourceName);
        }

        /// <summary>
        /// Gets the assembly for the specified resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>Assembly for the resource.</returns>
        private static Assembly GetAssembly(string resourceName)
        {
            Assembly assembly = null;
            if (!_compiledSourceFiles.TryGetValue(resourceName, out assembly))
            {
                using (TextReader reader = GetTestFileReader(resourceName))
                {
                    string source = reader.ReadToEnd();

                    CompilerResults results = Compile(source, resourceName);

                    if (results.Errors.Count > 0)
                    {
                        CompilerError error = null;

                        error = TestUtilities.GetCompilerError(results);

                        if (error != null)
                        {
                            Assert.Fail(
                                "Test source code should not produce compiler errors. " +
                                "Error: {0} - {1}, line {2}, column {3} ",
                                error.ErrorText,
                                resourceName,
                                error.Line,
                                error.Column);
                        }

                        assembly = results.CompiledAssembly;
                    }
                }

                if (assembly != null)
                {
                    _compiledSourceFiles.Add(resourceName, assembly);
                }
            }

            return assembly;
        }

        #endregion Methods
    }
}