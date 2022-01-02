using System.IO;
using SimpleSave.Models;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <inheritdoc />
    internal sealed class PackageHelper : IPackageHelper
    {
        private const string RelativeAssetPath = "Assets/Simple Save";
        private const string RelativePackagePath = "Packages/com.jonathanveit.simplesave";

        /// <inheritdoc />
        public bool IsPackage
        {
            get
            {
                var packageDirectoryPath = Path.GetFullPath(RelativePackagePath);
                return Directory.Exists(packageDirectoryPath);
            }
        }

        /// <inheritdoc />
        public string GetValidResourcePath(string pathInResources)
        {
            var resourcePath = $"{RelativeAssetPath}/Resources/{pathInResources}";
            if (!Directory.Exists(resourcePath))
            {
                Directory.CreateDirectory(resourcePath);
            }

            return resourcePath;
        }

        /// <inheritdoc />
        public string GetValidPackagePath(string pathInRootDirectory)
        {
            if (IsPackage)
            {
                return $"{RelativePackagePath}/{pathInRootDirectory}";
            }
           
            return $"{RelativeAssetPath}/{pathInRootDirectory}";
        }

        /// <inheritdoc />
        public PackageInformation GetPackageInformation()
        {
#if UNITY_EDITOR
            
            var path = GetValidPackagePath("package.json");
            var packageAsset = (TextAsset)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));

            if (packageAsset != null)
            {
                return JsonUtility.FromJson<PackageInformation>(packageAsset.text);
            }

#endif

            throw new FileNotFoundException("Unable to find the package.json file.");
        }
    }
}
