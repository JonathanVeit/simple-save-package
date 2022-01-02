using System;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable version of <see cref="SaveGameInfo"/>.
    /// </summary>
    [Serializable]
    public struct SerializableSaveGameInfo
    {
        /// <summary>
        /// Unique id of the save game.
        /// </summary>
        public SaveGameId Id;

        /// <summary>
        /// Custom name of the save game.
        /// </summary>
        public string Name;

        /// <summary>
        /// Date when the save game has been created.
        /// </summary>
        public string Created;

        /// <summary>
        /// Version of the save game.
        /// </summary>
        public string Version;

        /// <summary>
        /// Location of the save game file.
        /// </summary>
        public string Location;

        /// <summary>
        /// Custom data of the save game.
        /// </summary>
        public SerializableCustomDataEntry[] CustomData;
    }
}