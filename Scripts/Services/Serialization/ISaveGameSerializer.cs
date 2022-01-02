using SimpleSave.Models.Serializables;

namespace SimpleSave.Services
{
    /// <summary>
    /// Serializer for <see cref="SerializableSaveGame"/>.
    /// </summary>
    /// <remarks>
    /// You can create your own implementations of this interface.<br/>
    /// If you want to use your own implementation, you need to add it to the <see cref="SaveGameSettings"/> when creating or loading save games from <see cref="Simple"/>.
    /// </remarks>
    public interface ISaveGameSerializer
    {
        /// <summary>
        /// Serializes a save game.
        /// </summary> component into a string.
        /// <param name="serializableSaveGame">Save game to serialize.</param>
        /// <returns>Serialized save game.</returns>
        public string Serialize(SerializableSaveGame serializableSaveGame);

        /// <summary>
        /// Deserializes into a serializable save game.
        /// </summary>
        /// <param name="serializedSaveGame">The serialized save game.</param>
        /// <returns>The serializable save game.</returns>
        public SerializableSaveGame Deserialize(string serializedSaveGame);
    }
}