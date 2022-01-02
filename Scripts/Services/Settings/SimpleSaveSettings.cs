using System;
using SimpleSave.Models;
using UnityEngine;


namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISimpleSaveSettings"/>
    internal class SimpleSaveSettings : BaseService, ISimpleSaveSettings
    {
        private const string PlayerPrefsLocation = "PlayerPrefs";

        private static SettingsContainer Container => SettingsContainer.Instance;

        /// <inheritdoc />
        public InternalLogType LogType => Container.LogType;

        /// <inheritdoc />
        public string Location
        {
            get
            {
                switch (Container.Location)
                {
                    case Models.Location.DataPath:
                        return Application.dataPath + Container.SpecifiedLocation;
                    case Models.Location.PersistentDataPath:
                        return Application.persistentDataPath + Container.SpecifiedLocation;
                    case Models.Location.StreamingAssetsPath:
                        return Application.streamingAssetsPath + Container.SpecifiedLocation;
                    case Models.Location.Custom:
                        return Container.CustomLocation;
                    case Models.Location.PlayerPrefs:
                        return PlayerPrefsLocation;
                    
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <inheritdoc />
        public bool UsePlayerPrefs => Container.Location == Models.Location.PlayerPrefs;

        /// <inheritdoc />
        public string Version => Container.Version;

        /// <inheritdoc />
        public bool AutoCreateDirectory => Container.AutoCreateDirectory;

        /// <inheritdoc />
        public bool OverrideSaveGames => Container.OverrideSaveGames;

        /// <inheritdoc />
        public SceneLoading SceneLoading => Container.SceneLoading;

        /// <inheritdoc />
        public SaveGameEndCondition SaveGameEndCondition => Container.SaveGameEndCondition;

        /// <inheritdoc />
        public bool AlwaysSaveTagless => Container.AlwaysSaveTagless;

        /// <inheritdoc />
        public bool AllowCrossComponentSaving => Container.AllowCrossComponentSaving;

        /// <inheritdoc />
        public string[] AssembliesToScan => Container.AssembliesToScan.ToArray();

        /// <inheritdoc />
        public bool HotKeysEnabled => Container.HotKeysEnabled;

        /// <inheritdoc />
        public bool DebugLogging => Container.DebugLogging;
    }
}