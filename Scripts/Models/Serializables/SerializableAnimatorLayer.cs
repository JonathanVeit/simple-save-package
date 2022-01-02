using System;
using UnityEngine;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable layer of an <see cref="Animator"/>.
    /// </summary>
    [Serializable]
    public struct SerializableAnimatorLayer
    {
        /// <summary>
        /// Index of the layer.
        /// </summary>
        public int Index;

        /// <summary>
        /// Hash of the playing state.
        /// </summary>
        public int StateHash;

        /// <summary>
        /// Normalized time of the playing state.
        /// </summary>
        public float NormalizedTime;
    }
}