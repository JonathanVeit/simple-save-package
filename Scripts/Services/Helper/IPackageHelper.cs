using SimpleSave.Models;

namespace SimpleSave.Services
{
    /// <summary>
    /// Helper for package related methods.
    /// </summary>
    internal interface IPackageHelper
    {
        /// <summary>
        /// Is SimpleSave installed as a package?
        /// </summary>
        bool IsPackage { get; }

        /// <summary>
        /// Gets the valid path in the resources.
        /// </summary>
        /// <param name="pathInResources">Path inside the resources root folder.</param>
        /// <returns>The valid path.</returns>
        /// <remarks>
        /// The directory will be created if necessary.
        /// </remarks>
        string GetValidResourcePath(string pathInResources);

        /// <summary>
        /// Gets the valid path in the package.
        /// </summary>
        /// <param name="pathInPackage">Path inside the package root folder.</param>
        /// <returns>The valid path.</returns>
        /// <remarks>
        /// Inside a package, the path and all files will be readonly.
        /// </remarks>
        string GetValidPackagePath(string pathInPackage);

        /// <summary>
        /// Gets the package information.
        /// </summary>
        /// <returns>The package information.</returns>
        PackageInformation GetPackageInformation();
    }
}
