using System;
using System.Collections.Generic;
using SimpleSave.Services;
using UnityEngine;
using UnityEngine.Events;

namespace SimpleSave
{
    /// <summary>
    /// The main class to use SimpleSave.
    /// </summary>
    public static class Simple
    {
        /// <summary>
        /// Is there currently a save game loading?
        /// </summary>
        public static bool IsLoadingSaveGame => SaveGameLoader.IsLoading;

        /// <summary>
        /// Will be invoked right after a save game was created.
        /// </summary>
        public static UnityEvent<SaveGameInfo> OnSaveGameCreated = new UnityEvent<SaveGameInfo>();

        /// <summary>
        /// Will be invoked right after creating a save game failed.
        /// </summary>
        public static UnityEvent OnCreatingSaveGameFailed = new UnityEvent();

        /// <summary>
        /// Will be invoked right after a save game was loaded;
        /// </summary>
        public static UnityEvent<SaveGameInfo> OnSaveGameLoaded = new UnityEvent<SaveGameInfo>();

        /// <summary>
        /// Will be invoked right after loading a save game failed.
        /// </summary>
        public static UnityEvent<SaveGameInfo> OnLoadingSaveGameFailed = new UnityEvent<SaveGameInfo>();

        #region Services

        private static ISaveGameCreator SaveGameCreator => ServiceWrapper.GetService<ISaveGameCreator>();
        private static ISaveGameLoader SaveGameLoader => ServiceWrapper.GetService<ISaveGameLoader>();

        #endregion

        #region Events

        static Simple()
        {
            SaveGameCreator.OnSaveGameCreated += CallOnSaveGameCreated;
            SaveGameCreator.OnCreatingSaveGameFailed += CallOnCreatingSaveGameFailed;

            SaveGameLoader.OnSaveGameLoaded += CallOnSaveGameLoaded;
            SaveGameLoader.OnLoadingSaveGameFailed += CallOnLoadingSaveGameFailed;
        }

        private static void CallOnSaveGameCreated(SaveGameInfo saveGameInfo)
        {
            OnSaveGameCreated?.Invoke(saveGameInfo);
        }

        private static void CallOnCreatingSaveGameFailed()
        {
            OnCreatingSaveGameFailed?.Invoke();
        }

        private static void CallOnSaveGameLoaded(SaveGameInfo saveGameInfo)
        {
            OnSaveGameLoaded?.Invoke(saveGameInfo);
        }

        private static void CallOnLoadingSaveGameFailed(SaveGameInfo saveGameInfo)
        {
            OnLoadingSaveGameFailed?.Invoke(saveGameInfo);
        }

        #endregion

        #region Save

        /// <summary>
        /// Creates a new save game.
        /// </summary>
        /// <param name="saveGameName">Name of the save game.</param>
        /// <returns>Information about the created save game.</returns>
        /// <remarks>
        /// Default settings as defined in the settings window will be used.
        /// </remarks>
        public static SaveGameInfo Save(string saveGameName)
        {
            return Save(saveGameName, SaveGameSettings.Default, null);
        }

        /// <summary>
        /// Creates a new save game.
        /// </summary>
        /// <param name="saveGameName">Name of the save game.</param>
        /// <param name="customData">Custom data to save.</param>
        /// <returns>Information about the created save game.</returns>
        /// <remarks>
        /// Default settings as defined in the settings window will be used.
        /// </remarks>
        public static SaveGameInfo Save(string saveGameName, Dictionary<string, string> customData)
        {
            return Save(saveGameName, SaveGameSettings.Default, customData);
        }

        /// <summary>
        /// Creates a new save game.
        /// </summary>
        /// <param name="saveGameName">Name of the save game.</param>
        /// <param name="settings">Settings to use.</param>
        /// <returns>Information about the created save game.</returns>
        public static SaveGameInfo Save(string saveGameName, SaveGameSettings settings)
        {
            return Save(saveGameName, settings, null);
        }

        /// <summary>
        /// Creates a new save game.
        /// </summary>
        /// <param name="saveGameName">Name of the save game.</param>
        /// <param name="settings">Settings to use.</param>
        /// <param name="customData">Custom data to save.</param>
        /// <returns>Information about the created save game.</returns>
        public static SaveGameInfo Save(string saveGameName, SaveGameSettings settings,
            Dictionary<string, string> customData)
        {
            ValidateRuntime();

            return SaveGameCreator.CreateSaveGame(saveGameName, settings, customData);
        }

        #endregion

        #region Load

        /// <summary>
        /// Loads a new save game.
        /// </summary>
        /// <param name="saveGame">Information about the save game to load.</param>
        /// <remarks>
        /// Default settings as defined in the settings window will be used.
        /// </remarks>
        public static void Load(SaveGameInfo saveGame)
        {
            Load(saveGame, SaveGameSettings.Default);
        }

        /// <summary>
        /// Loads a new save game.
        /// </summary>
        /// <param name="saveGame">Information about the save game to load.</param>
        /// <param name="settings">Settings to use.</param>
        public static void Load(SaveGameInfo saveGame, SaveGameSettings settings)
        {
            ValidateRuntime();

            SaveGameLoader.Load(saveGame, settings);
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Gets all available save games.
        /// </summary>
        /// <returns>Information about the save games.</returns>
        /// <remarks>
        /// Default settings as defined in the settings window will be used.
        /// </remarks>
        public static SaveGameInfo[] GetAllSaveGames()
        {
            return GetAllSaveGames(SaveGameSettings.Default);
        }

        /// <summary>
        /// Gets all available save games.
        /// </summary>
        /// <param name="settings">Settings to use.</param>
        /// <returns>Information about the save games.</returns>
        public static SaveGameInfo[] GetAllSaveGames(SaveGameSettings settings)
        {
            ValidateRuntime();

            if (settings.UsePlayerPrefs)
            {
                return settings.SaveGameReader.GetAllFromPlayerPrefs();
            }

            return settings.SaveGameReader.GetAll(settings.Location);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the save game.
        /// </summary>
        /// <param name="saveGame">Information about the save game to delete.</param>
        public static void Delete(SaveGameInfo saveGame)
        {
            Delete(saveGame, SaveGameSettings.Default);
        }

        /// <summary>
        /// Deletes the save game.
        /// </summary>
        /// <param name="saveGame">Information about the save game to delete.</param>
        /// <param name="settings">Settings to use.</param>
        public static void Delete(SaveGameInfo saveGame, SaveGameSettings settings)
        {
            ValidateRuntime();

            if (settings.UsePlayerPrefs)
            {
                settings.SaveGameWriter.DeleteFromPlayerPrefs(saveGame.Name);
            }
            else
            {
                settings.SaveGameWriter.Delete(saveGame.Name, saveGame.Location);
            }
        }

        #endregion

        private static void ValidateRuntime()
        {
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("You can only use Simple Save at runtime.");
            }
        }
    }
}