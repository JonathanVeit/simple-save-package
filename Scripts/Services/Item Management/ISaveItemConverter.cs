using SimpleSave.Models.Serializables;

namespace SimpleSave.Services
{
    /// <summary>
    /// Converts <see cref="SaveItem"/> into <see cref="SerializableSaveGame"/>.
    /// </summary>
    internal interface ISaveItemConverter
    {
        /// <summary>
        /// Converts the given <see cref="SaveItem"/> into a <see cref="SerializableSaveItem"/>.
        /// </summary>
        /// <param name="saveItem">Item to serialize.</param>
        /// <param name="saveGameSettings">Settings to use.</param>
        /// <returns>The serializable item</returns>
        SerializableSaveItem ToSerializable(SaveItem saveItem, SaveGameSettings saveGameSettings);

        /// <summary>
        /// Converts the given <see cref="SerializableSaveItem"/> onto a <see cref="SaveItem"/>.
        /// </summary>
        /// <param name="serializableSaveItem">Serializable item.</param>
        /// <param name="saveItem">Item to convert onto</param>
        /// <param name="saveGameSettings">Settings to use.</param>
        void ToItem(SerializableSaveItem serializableSaveItem, SaveItem saveItem, SaveGameSettings saveGameSettings);
    }
}