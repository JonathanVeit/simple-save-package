using System;
using UnityEngine;

namespace SimpleSave.Models
{
    /// <summary>
    /// All properties of a <see cref="GameObject"/> that can be saved or loaded.
    /// </summary>
    [Flags]
    public enum GameObjectProperty
    {
        /// <summary>
        /// Active state of the GameObject.
        /// </summary>
        ActiveSelf = 1 << 1,

        /// <summary>
        /// Layer of the GameObject.
        /// </summary>
        Layer = 1 << 2,

        /// <summary>
        /// Tag of the GameObject.
        /// </summary>
        Tag = 1 << 3,

        /// <summary>
        /// The parent of the GameObject.
        /// </summary>
        Parent = 1 << 4,
    }
}
