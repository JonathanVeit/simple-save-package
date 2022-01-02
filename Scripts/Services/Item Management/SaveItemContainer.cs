using System;
using System.Collections.Generic;
using System.Linq;
using SimpleSave.Extensions;
using SimpleSave.Models;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveItemContainer"/>
    internal class SaveItemContainer : MonoBehaviour, ISaveItemContainer, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SerializableEntry> loadedItems;

        private readonly Dictionary<SaveItem, SaveItemId> _loadedItems;

        #region Services

        private ISaveItemContainerManager SaveItemContainerManager =>
            ServiceWrapper.GetService<ISaveItemContainerManager>();

        private ILogger Logger => ServiceWrapper.GetService<ILogger>();
        #endregion

        #region Serialization
        public SaveItemContainer()
        {
            loadedItems = new List<SerializableEntry>();
            _loadedItems = new Dictionary<SaveItem, SaveItemId>();
        }

        public void OnBeforeSerialize()
        {
            loadedItems.Clear();

            foreach (var entry in _loadedItems.ToArray())
            {
                if (entry.Key == null)
                {
                    continue;
                }

                if (entry.Key.gameObject.scene != this.gameObject.scene)
                {
                    HandleCrossSceneReference(entry.Key);
                    continue;
                }

                loadedItems.Add(new SerializableEntry()
                {
                    Item = entry.Key,
                    Id = entry.Value,
                });
            }
        }

        private void HandleCrossSceneReference(SaveItem item)
        {
            SaveItemContainerManager.GetContainerFor(item).AddItem(item, GetIdFor(item));
            RemoveItem(item);

            Logger.LogDebug($"Handled scene transition of {nameof(SaveItem)} \"{item.gameObject.name}\" to scene {item.gameObject.scene.name}.");
        }

        public void OnAfterDeserialize()
        {
            _loadedItems.Clear();

            foreach (var entry in loadedItems)
            {
                if (entry.Item is null)
                {
                    continue;
                }

                _loadedItems.Add(entry.Item, entry.Id);
            }
        }

        [Serializable]
        private struct SerializableEntry
        {
            public SaveItem Item;
            public SaveItemId Id;
        }
        #endregion

        /// <inheritdoc />
        public void AddItem(SaveItem item, SaveItemId id)
        {
            if (ContainsItem(item))
            {
                throw new ArgumentException($"{nameof(SaveItemContainer)} in scene {this.gameObject.scene.name} already contains Item entry for {item.gameObject.name}.");
            }

            _loadedItems.Add(item, id);
        }

        /// <inheritdoc />
        public void RemoveItem(SaveItem item)
        {
            _loadedItems.Remove(item);
        }

        /// <inheritdoc />
        public bool ContainsItem(SaveItem item)
        {
            return _loadedItems.ContainsKey(item);
        }

        /// <inheritdoc />
        public bool ContainsItemWithId(SaveItemId id)
        {
            foreach (var entry in _loadedItems)
            {
                if (entry.Value == id)
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        public SaveItemId GetIdFor(SaveItem item)
        {
            if (!ContainsItem(item))
            {
                return SaveItemId.Invalid;
            }

            return _loadedItems[item];
        }

        /// <inheritdoc />
        public void OverrideId(SaveItem item, SaveItemId newId)
        {
            if (!ContainsItem(item))
            {
                throw new ArgumentException($"{nameof(SaveItemContainer)} in scene {this.gameObject.scene.name} does not contain entry for {item.gameObject.name}.");
            }

            _loadedItems[item] = newId;
        }
    }
}