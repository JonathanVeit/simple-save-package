using System;

namespace SimpleSave.Services
{
    /// <summary>
    /// Takes care of loading save games.
    /// </summary>
    internal interface ISaveGameLoader
    {
        /// <summary>
        /// Called right after a save game was loaded.
        /// </summary>
        public event Action<SaveGameInfo> OnSaveGameLoaded;

        /// <summary>
        /// Called right after loading a save game failed.
        /// </summary>
        public event Action<SaveGameInfo> OnLoadingSaveGameFailed;

        /// <summary>
        /// Called right after a save game was ended.
        /// </summary>
        public event Action OnSaveGameEnded;

        /// <summary>
        /// Is this is currently a save game?
        /// </summary>
        bool IsSaveGame { get; }

        /// <summary>
        /// True if a save game is currently being loaded.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Loads the given save game.
        /// </summary>
        /// <param name="saveGameInfo">Save game to load.</param>
        /// <param name="saveGameSettings">Settings to use.</param>
        void Load(SaveGameInfo saveGameInfo, SaveGameSettings saveGameSettings);
    }
}