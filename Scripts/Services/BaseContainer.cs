using System;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <summary>
    /// Base for all container handled as <see cref="ScriptableObject"/>.
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    internal abstract class BaseContainer<TContainer> : ScriptableObject
        where TContainer : BaseContainer<TContainer>
    {
        private const string ContainerResourcePath = "Container";

        private static TContainer _instance;

        /// <summary>
        /// Instance of the container in the project.
        /// </summary>
        public static TContainer Instance => GetOrCreateContainer();

        /// <summary>
        /// Will be called right after the instance was created.
        /// </summary>

        protected static event Action<TContainer> OnInstanceCreated;

        private static TContainer GetOrCreateContainer()
        {
            if (_instance != null)
            {
                return _instance;
            }

            var loadedContainer = Resources.LoadAll<TContainer>($"{ContainerResourcePath}/{typeof(TContainer).Name}");

            if (loadedContainer != null &&
                loadedContainer.Length > 0)
            {
                _instance = loadedContainer[0];
                return _instance;
            }

#if UNITY_EDITOR
            _instance = CreateContainer();
            OnInstanceCreated?.Invoke(_instance);
            return _instance;
#else
            throw new System.Exception($"Unable to get or create {typeof(TContainer).Name} at path \"{ContainerResourcePath}\".");
#endif
        }

#if UNITY_EDITOR
        private static TContainer CreateContainer()
        {
            TContainer container = ScriptableObject.CreateInstance<TContainer>();

            var path = ServiceWrapper.GetService<IPackageHelper>().GetValidResourcePath(ContainerResourcePath) + $"/{typeof(TContainer).Name}.asset";
            UnityEditor.AssetDatabase.CreateAsset(container, path);
            UnityEditor.AssetDatabase.SaveAssets();

            return container;
        }
#endif
    }
}