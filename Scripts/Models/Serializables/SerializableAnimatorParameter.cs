using System;
using UnityEngine;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable parameter of an <see cref="Animator"/>.
    /// </summary>
    [Serializable]
    public struct SerializableAnimatorParameter
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name;

        /// <summary>
        /// Type of the parameter.
        /// </summary>
        public AnimatorControllerParameterType Type;

        /// <summary>
        /// Value of the parameter.
        /// </summary>
        public string Value;
    }
}