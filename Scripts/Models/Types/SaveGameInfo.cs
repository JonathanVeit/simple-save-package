using System;
using System.Collections.Generic;
using SimpleSave.Models;
using SimpleSave.Models.Serializables;

namespace SimpleSave
{
    /// <summary>
    /// Information about a save game.
    /// </summary>
    [Serializable]
    public struct SaveGameInfo
    {
        /// <summary>
        /// Unique id of the save game.
        /// </summary>
        internal SaveGameId Id { get; }

        /// <summary>
        /// Custom name of the save game.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Date when the save game has been created.
        /// </summary>
        public DateTime Created { get; }

        /// <summary>
        /// Version of the save game.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Location of the save game file.
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// Custom data of the save game.
        /// </summary>
        public Dictionary<string, string> CustomData { get; }

        /// <summary>
        /// Returns true if the save game is valid and can be loaded.
        /// </summary>
        public bool IsValid { get; }

        internal SaveGameInfo(SerializableSaveGameInfo sSaveGameInfo) : this(sSaveGameInfo.Id, sSaveGameInfo.Name,
            sSaveGameInfo.Created, sSaveGameInfo.Version, sSaveGameInfo.Location, sSaveGameInfo.CustomData)
        {
        }

        internal SaveGameInfo(SaveGameId id, string name, string created, string version, string location,
            SerializableCustomDataEntry[] customData)
        {
            Id = id;
            Name = name;
            Created = DateTime.Parse(created);
            Version = version;
            Location = location;
            CustomData = ConvertCustomData(customData);

            IsValid = true;
        }

        /// <summary>
        /// Invalid save game that cannot be loaded.
        /// </summary>
        internal static SaveGameInfo Invalid => new SaveGameInfo();

        #region Helper

        private static Dictionary<string, string> ConvertCustomData(
            SerializableCustomDataEntry[] serializableCustomDataEntries)
        {
            var result = new Dictionary<string, string>();
            for (int i = 0; i < serializableCustomDataEntries.Length; i++)
            {
                result.Add(serializableCustomDataEntries[i].Key, serializableCustomDataEntries[i].Value);
            }

            return result;
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}