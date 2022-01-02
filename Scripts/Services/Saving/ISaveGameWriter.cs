namespace SimpleSave.Services
{
    /// <summary>
    /// Writes serialized save games into files.
    /// </summary>
    /// <remarks>
    /// You can create your own implementations of this interface.<br/>
    /// If you want to use your own implementation, you need to add it to the <see cref="SaveGameSettings"/> when creating or loading save games from <see cref="Simple"/>.
    /// </remarks>
    public interface ISaveGameWriter
    {
        /// <summary>
        /// Writes the serialized save game.
        /// </summary>
        /// <param name="serializedSaveGame">Serialized save game.</param>
        /// <param name="fileName">Name of the file to write into.</param>
        /// <param name="location">Location to write the file.</param>
        public void Write(string serializedSaveGame, string fileName, string location);

        /// <summary>
        /// Writes the serialized save game into the player prefs.
        /// </summary>
        /// <param name="serializedSaveGame">Serialized save game.</param>
        /// <param name="fileName">Name of the file to write into.</param>
        public void WriteToPlayerPrefs(string serializedSaveGame, string fileName);

        /// <summary>
        /// Deletes the save game file.
        /// </summary>
        /// <param name="fileName">Name of the file to delete.</param>
        /// <param name="location">Path to delete the file from.</param>
        /// <returns>True if the save game has successfully been written.</returns>
        public void Delete(string fileName, string location);

        /// <summary>
        /// Deletes the save game file from the player prefs.
        /// </summary>
        /// <param name="fileName">Name of the file to delete.</param>
        /// <param name="location">Path to delete the file from.</param>
        /// <returns>True if the save game has successfully been written.</returns>
        public void DeleteFromPlayerPrefs(string fileName);
    }
}