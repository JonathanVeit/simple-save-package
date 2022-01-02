using System.Collections.Generic;
using SimpleSave.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveItemContainerManager"/>
    internal class SaveItemContainerManager : BaseService, ISaveItemContainerManager
    {
        private readonly Dictionary<string, SaveItemContainer> _loadedContainers;

        public SaveItemContainerManager()
        {
            _loadedContainers = new Dictionary<string, SaveItemContainer>(); 
        }

        /// <inheritdoc />>
        public ISaveItemContainer GetContainerFor(SaveItem item)
        {
            var scene = item.gameObject.scene;
            
            if (!HasValidContainer(scene))
            {
                LoadOrCreateContainer(scene);
            }

            return _loadedContainers[scene.path];
        }

        private bool HasValidContainer(Scene forScene)
        {
            return _loadedContainers.ContainsKey(forScene.path) && _loadedContainers[forScene.path] != null;
        }

        private void LoadOrCreateContainer(Scene forScene)
        {
            var allContainer = GameObject.FindObjectsOfType<SaveItemContainer>();
            bool foundContainer = false;

            for (int i = 0; i < allContainer.Length; i++)
            {
                var scene = allContainer[i].gameObject.scene;
                LoadContainer(scene, allContainer[i]);

                if (scene.path == forScene.path)
                {
                    foundContainer = true;
                }
            }

            if(!foundContainer)
            {
                CreateContainer(forScene);
            }
        }

        private void LoadContainer(Scene forScene, SaveItemContainer container)
        {
            if (_loadedContainers.ContainsKey(forScene.path))
            {
                if (_loadedContainers[forScene.path] == null)
                {
                    _loadedContainers[forScene.path] = container;
                }
            }
            else
            {
                _loadedContainers.Add(forScene.path, container);
            }
        }

        private void CreateContainer(Scene forScene)
        {
            var go = CreateHiddenGameObject(forScene);
            var cmp = go.AddComponent<SaveItemContainer>();

            _loadedContainers.Add(forScene.path, cmp);

            Logger.LogDebug($"Created {nameof(SaveItemContainer)} in scene \"{forScene.name}\".");
        }

        private GameObject CreateHiddenGameObject(Scene inScene)
        {
            var go = new GameObject($"{nameof(SaveItemContainer)}_{inScene.name} (DON'T DESTROY)")
            {
                hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable
            };

            SceneManager.MoveGameObjectToScene(go, inScene);
            return go;
        }
    }
}