using System;
using UnityEngine;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable <see cref="SaveItem"/>.
    /// </summary>
    [Serializable]
    public struct SerializableSaveItem
    {
        /// <summary>
        /// Unique identifier of the item.
        /// </summary>
        public string Id;

        /// <summary>
        /// State of the item.
        /// </summary>
        public SaveItemState State;

        /// <summary>
        /// Tags of the item.
        /// </summary>
        public TagId[] Tags;

        /// <summary>
        /// Unique identifier of the prefab.
        /// </summary>
        public string PrefabId;

        /// <summary>
        /// Serializable properties of the items <see cref="GameObject"/>.
        /// </summary>
        public SerializableGameObjectProperty[] Properties;

        /// <summary>
        /// Serializable <see cref="Component"/> of the item.
        /// </summary>
        public SerializableComponent[] Components;
    }
}