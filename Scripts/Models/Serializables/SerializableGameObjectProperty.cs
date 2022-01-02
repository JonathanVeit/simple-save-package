using System;
using UnityEngine;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable property of a <see cref="GameObject"/>.
    /// </summary>
    [Serializable]
    public struct SerializableGameObjectProperty
    {
        /// <summary>
        /// Type of the property.
        /// </summary>
        public GameObjectProperty Property;

        /// <summary>
        /// Serialized value of the property.
        /// </summary>
        public string Value;
    }
}