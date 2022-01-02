using System;
using UnityEngine;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable <see cref="Component"/> within a <see cref="ComponentInfo"/>.
    /// </summary>
    [Serializable]
    public class SerializableComponent
    {
        /// <summary>
        /// Unique identifier of the <see cref="ComponentInfo"/>.
        /// </summary>
        public string Id;

        /// <summary>
        /// Serialized component value.
        /// </summary>
        public string Value;

        /// <summary>
        /// Is the component enabled?
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// All references of properties or fields marked with the <see cref="SaveRef"/> attribute.
        /// </summary>
        public SerializableReference[] References;
    }
}