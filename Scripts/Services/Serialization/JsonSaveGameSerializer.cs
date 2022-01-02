using UnityEngine;
using SimpleSave.Models.Serializables;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveGameSerializer"/>
    internal class JsonSaveGameSerializer : BaseService, ISaveGameSerializer
    {
        /// <inheritdoc />
        public string Serialize(SerializableSaveGame serializableSaveGame)
        {
            return JsonUtility.ToJson(serializableSaveGame, true);
        }

        /// <inheritdoc />
        public SerializableSaveGame Deserialize(string serializedSaveGame)
        {
            return JsonUtility.FromJson<SerializableSaveGame>(serializedSaveGame);
        }
    }
}