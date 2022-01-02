using System;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable save game.
    /// </summary>
    [Serializable]
    public class SerializableSaveGame
    {
        /// <summary>
        /// Information about the save game.
        /// </summary>
        public SerializableSaveGameInfo Info;

        /// <summary>
        /// Serializable <see cref="SaveVar"/>.
        /// </summary>
        public SerializableSaveVar[] SaveVars;

        /// <summary>
        /// Serializable scenes of the save game.
        /// </summary>
        public SerializableScene[] Scenes;
    }
}