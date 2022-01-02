using System.Reflection;

namespace SimpleSave.Services
{
    /// <summary>
    /// Scans assemblies for <see cref="SaveVar"/> and <see cref="SaveRef"/>.
    /// </summary>
    internal interface IAssemblyScanner
    {
        /// <summary>
        /// BindingFlags to search members.
        /// </summary>
        BindingFlags ScanningFlags { get; }
    }
}