namespace NArrange.Tests.Core
{
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Interface for a source code test file.
    /// </summary>
    public interface ISourceCodeTestFile
    {
        #region Properties

        /// <summary>
        /// Gets the assembly for the test file.
        /// </summary>
        Assembly Assembly
        {
            get;
        }

        /// <summary>
        /// Gets the name of the test file.
        /// </summary>
        string Name
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets a TextReader for this test file.
        /// </summary>
        /// <returns>The TextReader.</returns>
        TextReader GetReader();

        #endregion Methods
    }
}