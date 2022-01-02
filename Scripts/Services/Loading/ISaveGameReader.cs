namespace SimpleSave.Services
{
    /// <summary>
    /// Reads serialized save games.
    /// </summary>
    /// <remarks>
    /// You can create your own implementations of this interface.<br/>
    /// If you want to use your own implementation, you need to add it to the <see cref="SaveGameSettings"/> when creating or loading save games from <see cref="Simple"/>.
    /// </remarks>
    public interface ISaveGameReader
    {
        /// <summary>
        /// Reads the serialized save game.
        /// </summary>
        /// <param name="fileName">Name of the file to write into.</param>
        /// <param name="location">Location to write the file into.</param>
        /// <returns>The serialized save game.</returns>
        string Read(string fileName, string location);

        /// <summary>
        /// Reads the serialized save game from PlayerPrefs.
        /// </summary>
        /// <param name="fileName">Name of the file to write into.</param>
        /// <returns>The serialized save game.</returns>
        string ReadFromPlayerPrefs(string fileName);

        /// <summary>
        /// Gets all available save games.
        /// </summary>
        /// <param name="location">Location of the save games.</param>
        /// <returns>Information about the save games.</returns>
        SaveGameInfo[] GetAll(string location);

        /// <summary>
        /// Gets all available save games from the PlayerPrefs.
        /// </summary>
        /// <param name="location">Location of the save games.</param>
        /// <returns>Information about the save games.</returns>
        SaveGameInfo[] GetAllFromPlayerPrefs();
    }
}