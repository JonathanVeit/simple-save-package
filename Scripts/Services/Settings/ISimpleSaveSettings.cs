using SimpleSave.Models;

namespace SimpleSave.Services
{
    /// <summary>
    /// Provides read access to the settings.
    /// </summary>
    internal interface ISimpleSaveSettings 
    {
        /// <summary>
        /// Log type for none-critical errors during the saving and loading process.
        /// </summary>
        InternalLogType LogType { get; }
        
        /// <summary>
        /// The current version string of the save games.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Location to save and load the save games.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Use PlayerPrefs to save and load save games.
        /// </summary>
        bool UsePlayerPrefs { get; }

        /// <summary>
        /// Should the the target directory be created if necessary?
        /// </summary>
        bool AutoCreateDirectory { get; }

        /// <summary>
        /// Should existing save games be overriden?
        /// </summary>
        bool OverrideSaveGames { get; }

        /// <summary>
        /// SceneLoading type.
        /// </summary>
        SceneLoading SceneLoading { get; }

        /// <summary>
        /// Save game end condition.
        /// </summary>
        SaveGameEndCondition SaveGameEndCondition { get; }

        /// <summary>
        /// Should <see cref="SaveItem"/> and <see cref="SaveVar"/> that do not have any tags defined always be saved or loaded?
        /// </summary>
        bool AlwaysSaveTagless { get; }

        /// <summary>
        /// Should <see cref="SaveItem"/>s be allowed to save components that are not placed on their GameObject nor a child.
        /// </summary>
        bool AllowCrossComponentSaving { get; }

        /// <summary>
        /// Assemblies to scan for <see cref="SaveVar"/> and <see cref="SaveRef"/> attributes.
        /// </summary>
        string[] AssembliesToScan { get; }

        /// <summary>
        /// Enables hotkeys in the editor.
        /// </summary>
        bool HotKeysEnabled { get; }

        /// <summary>
        /// Log debug information in the editor.
        /// </summary>
        bool DebugLogging { get; }
    }
}