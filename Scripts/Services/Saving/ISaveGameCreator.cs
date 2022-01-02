using System;
using System.Collections.Generic;

namespace SimpleSave.Services
{
    /// <summary>
    /// Takes care of creating save games.
    /// </summary>
    internal interface ISaveGameCreator
    {
        /// <summary>
        /// Called right after a save game was created.
        /// </summary>
        public event Action<SaveGameInfo> OnSaveGameCreated;

        /// <summary>
        /// Called right after creating a save game failed.
        /// </summary>
        public event Action OnCreatingSaveGameFailed;

        /// <summary>
        /// Creates a new save game based on the current game.
        /// </summary>
        /// <param name="name">Name of the save game.</param>
        /// <param name="saveGameSettings">Settings to use.</param>
        /// <param name="customData">Custom data to store in the save game.</param>
        /// <returns>The <see cref="SaveGameInfo"/> of the save game.</returns>
        SaveGameInfo CreateSaveGame(string name, SaveGameSettings saveGameSettings, Dictionary<string, string> customData);
    }
}