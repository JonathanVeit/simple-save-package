using System;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable variable with the <see cref="SaveVar"/> attribute.
    /// </summary>
    [Serializable]
    public struct SerializableSaveVar
    {
        /// <summary>
        /// Unique id of the variable.
        /// </summary>
        public int Id;

        /// <summary>
        /// Serialized value.
        /// </summary>
        public string Value;
    }
}