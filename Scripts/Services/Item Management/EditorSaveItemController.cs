using System;
using System.Collections.Generic;
using SimpleSave.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveItemController"/>
    internal class EditorSaveItemController : BaseService, ISaveItemController
    {
        #region Services

        private static IIdProvider IdProvider => ServiceWrapper.GetService<IIdProvider>();
        private static ISaveItemContainerManager SaveItemContainerManager => ServiceWrapper.GetService<ISaveItemContainerManager>();
        
        #endregion

        #region Register/Unregister

        /// <inheritdoc/>
        public void RegisterItem(SaveItem item)
        {
            if (!SaveItemContainerManager.GetContainerFor(item).ContainsItem(item))
            {
                InitializeItem(item);
            }
        }

        private void InitializeItem(SaveItem item)
        {
            var id = IdProvider.GetNewItemId();
            SaveItemContainerManager.GetContainerFor(item).AddItem(item, id);
            SetDefaultItemValues(item);
        }

        private void SetDefaultItemValues(SaveItem item)
        {
            if (item.GetComponents().Count > 0)
            {
                return;
            }

            item.SetProperties(GameObjectProperty.ActiveSelf | GameObjectProperty.Layer | GameObjectProperty.Tag);
            item.AddComponent(item.transform);
        }

        /// <inheritdoc/>
        public Dictionary<SaveItemId, SaveItem> GetRegisteredItems()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Dictionary<Scene, Dictionary<SaveItemId, SaveItem>> GetRegisteredItemsByScene()
        {
            var result = new Dictionary<Scene, Dictionary<SaveItemId, SaveItem>>();

            foreach (var saveItem in GameObject.FindObjectsOfType<SaveItem>())
            {
                if (!result.ContainsKey(saveItem.gameObject.scene))
                {
                    result.Add(saveItem.gameObject.scene, new Dictionary<SaveItemId, SaveItem>());
                }

                result[saveItem.gameObject.scene].Add(SaveItemContainerManager.GetContainerFor(saveItem).GetIdFor(saveItem), saveItem);
            }

            return result;
        }

        /// <inheritdoc/>
        public bool TryGetItem(SaveItemId id, out SaveItem saveItem)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void UnregisterItem(SaveItem item)
        {
            SaveItemContainerManager.GetContainerFor(item).RemoveItem(item);
        }

        #endregion
    }
}