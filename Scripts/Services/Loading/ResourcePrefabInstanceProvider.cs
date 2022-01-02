using System;
using SimpleSave.Extensions;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="IPrefabInstanceProvider"/>
    internal class ResourcePrefabInstanceProvider : BaseService, IPrefabInstanceProvider
    {
        /// <inheritdoc />>
        public SaveItem GetInstance(string prefabId)
        {
            var assets = Resources.LoadAll<SaveItem>(prefabId);

            if (assets.Length == 0)
            {
                throw new ArgumentException($"No prefab found for id \"{prefabId}\" in resources.");
            }

            if (assets.Length > 1)
            {
                Logger.LogInternal($"Found more than one prefab for id \"{prefabId}\" in resources. The first one will be used.");
            }
            return UnityEngine.Object.Instantiate(assets[0]);
        }
    }
}