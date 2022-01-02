using System.Collections.Generic;
using SimpleSave.Extensions;
using SimpleSave.Models;
using UnityEngine.SceneManagement;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveItemController"/>
    internal class RuntimeSaveItemController : BaseService, ISaveItemController
    {
        private readonly Dictionary<Scene, Dictionary<SaveItemId, SaveItem>> _loadedItems;

        #region Services

        private static IIdProvider IdProvider => ServiceWrapper.GetService<IIdProvider>();
        private static ISaveItemContainerManager SaveItemContainerManager => ServiceWrapper.GetService<ISaveItemContainerManager>();
        private static ISaveGameLoader SaveGameLoader => ServiceWrapper.GetService<ISaveGameLoader>();

        #endregion

        public RuntimeSaveItemController()
        {
            _loadedItems = new Dictionary<Scene, Dictionary<SaveItemId, SaveItem>>();
        }

        #region Register/Unregister

        /// <inheritdoc/>
        public void RegisterItem(SaveItem item)
        {
            var scene = item.gameObject.scene;
            if (!_loadedItems.ContainsKey(scene))
            {
                _loadedItems.Add(scene, new Dictionary<SaveItemId, SaveItem>());
            }

            if (InitializeItem(item))
            {
                _loadedItems[scene].Add(item.Id, item);
            }
        }

        private bool InitializeItem(SaveItem item)
        {
            // the item is not in the container -> it has been instantiated at runtime
            if (!SaveItemContainerManager.GetContainerFor(item).ContainsItem(item))
            {
                // instantiated during loading process
                if (SaveGameLoader.IsLoading)
                {
                    // uninitialized? -> item registered itself, is skipped
                    // initialized?   -> registered by controller
                    return item.State != SaveItemState.Uninitialized;
                }

                return HandlePrefabItem(item);
            }
            
            return HandleSceneItem(item);
        }

        private bool HandleSceneItem(SaveItem item)
        {
            var id = SaveItemContainerManager.GetContainerFor(item).GetIdFor(item);

            if (IdIsAlreadyRegistered(id, out string registeredItemScene))
            {
                Logger.LogInternal($"Tried to register {nameof(SaveItem)} with id \"{id}\" from scene \"{item.gameObject.scene.name}\" " +
                                                    $"but another {nameof(SaveItem)} from scene \"{registeredItemScene}\" with the same id has already been registered.\n" +
                                                    $"The Item wont be saved. Please check whether or not there are {nameof(SaveItem)}s with the same Id in those scenes.");
                return false;
            }

            item.SetId(id);
            item.SetState(SaveItemState.Scene);
            return true;
        }

        private bool IdIsAlreadyRegistered(SaveItemId id, out string registeredItemScene)
        {
            foreach (var entry in _loadedItems)
            {
                if (entry.Value.TryGetValue(id, out var registeredItem))
                {
                    registeredItemScene = registeredItem.gameObject.scene.name;
                    return true;
                }
            }

            registeredItemScene = null;
            return false;
        }

        private bool HandlePrefabItem(SaveItem item)
        {
            var id = IdProvider.GetNewItemId();
            item.SetId(id);
            item.SetState(SaveItemState.PrefabInstance);

            return true;
        }

        /// <inheritdoc/>
        public Dictionary<SaveItemId, SaveItem> GetRegisteredItems()
        {
            var result = new Dictionary<SaveItemId, SaveItem>();
            foreach (var entry in _loadedItems)
            {
                foreach (var item in entry.Value)
                {
                    result.Add(item.Key, item.Value);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public Dictionary<Scene, Dictionary<SaveItemId, SaveItem>> GetRegisteredItemsByScene()
        {
            return new Dictionary<Scene, Dictionary<SaveItemId, SaveItem>>(_loadedItems);
        }

        /// <inheritdoc/>
        public bool TryGetItem(SaveItemId id, out SaveItem saveItem)
        {
            foreach (var entry in _loadedItems)
            {
                if (entry.Value.TryGetValue(id, out saveItem))
                {
                    return true;
                }
            }

            saveItem = null;
            return false;
        }

        /// <inheritdoc/>
        public void UnregisterItem(SaveItem item)
        {
            if (_loadedItems.TryGetValue(item.gameObject.scene, out var entry))
            {
                entry.Remove(item.Id);
            }
        }

        #endregion
    }
}