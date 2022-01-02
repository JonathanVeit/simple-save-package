using System;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable unity scene.
    /// </summary>
    [Serializable]
    public struct SerializableScene
    {
        /// <summary>
        /// Identifier to load the scene with.
        /// </summary>
        public string Path;

        /// <summary>
        /// Index of the scene in the save game.
        /// </summary>
        /// <remarks>Index 0 is the active scene. All other indices are additive scenes.</remarks>
        public int Index;

        /// <summary>
        /// All items saved in the scene. 
        /// </summary>
        public SerializableSaveItem[] Items;
    }
}