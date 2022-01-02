using SimpleSave.Models.Serializables;

namespace SimpleSave.Services
{
    /// <summary>
    /// Controller to get and load member with the <see cref="SaveVar"/> attribute.
    /// </summary>
    public interface ISaveVarController
    {
        /// <summary>
        /// Gets all member as <see cref="SerializableSaveVar"/>.
        /// </summary>
        /// <param name="saveGameSettings">Settings to use.</param>
        /// <returns>All serializable SaveVars.</returns>
        public SerializableSaveVar[] GetSaveVars(SaveGameSettings saveGameSettings);

        /// <summary>
        /// Loads the <see cref="SerializableSaveVar"/>.
        /// </summary>
        /// <param name="serializableSaveVars">Serializable SaveVar to load.</param>
        /// <param name="saveGameSettings">Settings to use.</param>
        public void LoadSaveVars(SerializableSaveVar[] serializableSaveVars, SaveGameSettings saveGameSettings);
    }
}