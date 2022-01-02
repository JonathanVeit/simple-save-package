namespace SimpleSave.Models
{
    /// <summary>
    /// Determines how scenes will be handled during the save game loading process.
    /// </summary>
    public enum SceneLoading
    {
        /// <summary>
        /// All scenes defined in the save game will be loaded.
        /// </summary>
        LoadAll = 1,

        /// <summary>
        /// No scenes will be loaded.
        /// </summary>
        SkipLoading = 2,
    }
}
