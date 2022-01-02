using System;
using UnityEngine;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable <see cref="Animator"/>.
    /// </summary>
    [Serializable]
    public struct SerializableAnimator
    {
        /// <summary>
        /// Layer of the animator.
        /// </summary>
        public SerializableAnimatorLayer[] Layers;

        /// <summary>
        /// Parameters of the animator.
        /// </summary>
        public SerializableAnimatorParameter[] Parameters;
    }
}