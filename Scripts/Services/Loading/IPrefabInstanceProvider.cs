namespace SimpleSave.Services
{
    /// <summary>
    /// Provides prefab instances required to load a save game.
    /// </summary>
    /// <remarks>
    /// You can create your own implementations of this interface.<br/>
    /// If you want to use your own implementation, you need to add it to the <see cref="SaveGameSettings"/> when creating or loading save games from <see cref="Simple"/>.
    /// </remarks>
    public interface IPrefabInstanceProvider
    {
        /// <summary>
        /// Creates and returns an instance of the given prefab id. 
        /// </summary>
        /// <param name="prefabId">Id of the prefab to create.</param>
        /// <returns>The <see cref="SaveItem"/> on the prefab instance.</returns>
        public SaveItem GetInstance(string prefabId);
    }
}