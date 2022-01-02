using SimpleSave.Models;
using SimpleSave.Services;

namespace SimpleSave
{
    /// <summary>
    /// Specifies the settings on how to create and load a save game.
    /// </summary>
    /// <remarks>
    /// Default settings can be set in the settings menu.
    /// </remarks>
    /// <example>
    /// <code>
    /// // this will create a save game at a specified location and use a custom implementation of the ISaveGameWriter
    /// public void SaveCustom()
    /// {
    ///     var settings = new SaveGameSettings
    ///     {
    ///         Location = "Example/Location",
    ///         SaveGameWriter = new CustomSaveGameWriter(),
    ///     };
    /// 
    ///     Simple.Save("ExampleSaveGame", settings);
    /// }
    /// </code>
    /// </example>
public sealed class SaveGameSettings
    {
        private string _location;
        private bool? _usePlayerPrefs;
        private string _version;
        private TagCollection _tags;

        private SceneLoading? _sceneLoading;
        private IComponentSerializer _componentSerializer;
        private ISaveGameSerializer _saveGameSerializer;
        private ISaveVarSerializer _saveVarSerializer;
        private ISaveGameWriter _saveGameWriter;
        private ISaveGameReader _saveGameReader;
        private IPrefabInstanceProvider _prefabInstanceProvider;
        private IReferenceResolver _referenceResolver;

        #region Services

        private static ISimpleSaveSettings Settings => ServiceWrapper.GetService<ISimpleSaveSettings>();

        #endregion
        
        /// <summary>
        /// Location for saving or loading the save game.
        /// </summary>
        public string Location
        {
            get => _location ?? Settings.Location;
            set => _location = value;
        }

        /// <summary>
        /// Use PlayerPrefs to save and load save games.
        /// </summary>
        public bool UsePlayerPrefs
        {
            get => _usePlayerPrefs ?? Settings.UsePlayerPrefs;
            set => _usePlayerPrefs = value;
        }

        /// <summary>
        /// Version of the save game.
        /// </summary>
        public string Version
        {
            get => _version ?? Settings.Version;
            set => _version = value;
        }

        /// <summary>
        /// Tags to use.
        /// </summary>
        public TagCollection Tags
        {
            get => _tags;
            set => _tags = value;
        }

        /// <summary>
        /// Scene loading to use.
        /// </summary>
        public SceneLoading SceneLoading
        {
            get => _sceneLoading ?? Settings.SceneLoading;
            set => _sceneLoading = value;
        }

        /// <summary>
        /// Serializer for components to use.
        /// </summary>
        public IComponentSerializer ComponentSerializer {
            get => _componentSerializer ?? ServiceWrapper.GetService<IComponentSerializer>();
            set => _componentSerializer = value;
        }

        /// <summary>
        /// Serializer for the save game to use.
        /// </summary>
        public ISaveGameSerializer SaveGameSerializer
        {
            get => _saveGameSerializer ?? ServiceWrapper.GetService<ISaveGameSerializer>();
            set => _saveGameSerializer = value;
        }

        /// <summary>
        /// Serializer for the save vars to use.
        /// </summary>
        public ISaveVarSerializer SaveVarSerializer
        {
            get => _saveVarSerializer ?? ServiceWrapper.GetService<ISaveVarSerializer>();
            set => _saveVarSerializer = value;
        }

        /// <summary>
        /// Writer for the save game to use.
        /// </summary>
        public ISaveGameWriter SaveGameWriter
        {
            get => _saveGameWriter ?? ServiceWrapper.GetService<ISaveGameWriter>();
            set => _saveGameWriter = value;
        }

        /// <summary>
        /// Reader for the save game to use
        /// </summary>
        public ISaveGameReader SaveGameReader
        {
            get => _saveGameReader ?? ServiceWrapper.GetService<ISaveGameReader>();
            set => _saveGameReader = value;
        }

        /// <summary>
        /// Prefab instance provider to use.
        /// </summary>
        public IPrefabInstanceProvider PrefabInstanceProvider
        {
            get => _prefabInstanceProvider ?? ServiceWrapper.GetService<IPrefabInstanceProvider>();
            set => _prefabInstanceProvider = value;
        }

        /// <summary>
        /// Resolver to use.
        /// </summary>
        public IReferenceResolver ReferenceResolver
        {
            get => _referenceResolver ?? ServiceWrapper.GetService<IReferenceResolver>();
            set => _referenceResolver = value;
        }

        /// <summary>
        /// Default settings as defined in the settings menu.
        /// </summary>
        public static SaveGameSettings Default => new SaveGameSettings();
    }
}