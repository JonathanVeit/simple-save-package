using SimpleSave.Services;
using UnityEditor;
using UnityEngine;

namespace SimpleSave
{
    internal static class Hotkeys
    {
        private const string SaveGameName = "quicksave";
        private static SaveGameSettings SaveGameSettings => new SaveGameSettings
        {
            Location = Application.temporaryCachePath + "/"
        };

        #region Services

        private static ILogger Logger => ServiceWrapper.GetService<ILogger>();
        private static ISimpleSaveSettings Settings => ServiceWrapper.GetService<ISimpleSaveSettings>();

        #endregion

        [MenuItem("Tools/Simple Save/Quick Save %&s", true)]
        public static bool QuickSaveValidation()
        {
            return Application.isPlaying &&
                   Settings.HotKeysEnabled;
        }
        
        [MenuItem("Tools/Simple Save/Quick Save %&s")]
        public static void QuickSave()
        {
            Simple.Save(SaveGameName, SaveGameSettings);
        }

        [MenuItem("Tools/Simple Save/Quick Load %&l", true)]
        public static bool QuickLoadValidation()
        {
            return Application.isPlaying &&
                   Settings.HotKeysEnabled;
        }

        [MenuItem("Tools/Simple Save/Quick Load %&l")]
        public static void QuickLoad()
        {
            foreach (var saveGameInfo in Simple.GetAllSaveGames(SaveGameSettings))
            {
                if (saveGameInfo.Name == SaveGameName)
                {
                    Simple.Load(saveGameInfo, SaveGameSettings);
                    return;
                }
            }

            Logger.Log(LogType.Log, "No quick save game found.");
        }
    }
}
