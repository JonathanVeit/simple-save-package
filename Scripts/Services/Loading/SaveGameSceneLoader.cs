using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SimpleSave.Models;
using SimpleSave.Models.Serializables;

namespace SimpleSave.Services
{
    /// <summary>
    /// Takes care of loading all scenes defined in a <see cref="SerializableSaveGame"/>
    /// </summary>
    internal class SaveGameSceneLoader
    {
        internal enum LoadingState
        {
            BeforeLoading = 0,
            IsLoading = 1,
            FinishedLoading = 3,
        }

        private List<string> _scenesToLoad;

        private SerializableSaveGame _serializableSaveGame;
        private SaveGameSettings _saveGameSettings;

        public LoadingState State { get; private set; }
        public event Action<SerializableSaveGame, SaveGameSettings> OnLoadingCompleted;

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="saveGame">Save game to load scenes from.</param>
        public void Initialize(SerializableSaveGame saveGame, SaveGameSettings saveGameSettings)
        {
            State = LoadingState.BeforeLoading;
            SceneManager.sceneLoaded += OnSceneLoaded;

            StoreScenesToLoad(saveGame);

            _serializableSaveGame = saveGame;
            _saveGameSettings = saveGameSettings;
        }

        private void StoreScenesToLoad(SerializableSaveGame saveGame)
        {
            if (_scenesToLoad is null)
            {
                _scenesToLoad = new List<string>();
            }
            else
            {
                _scenesToLoad.Clear();
            }

            for (int i = 0; i < saveGame.Scenes.Length; i++)
            {
                if (saveGame.Scenes[i].Index == 0)
                {
                    _scenesToLoad.Insert(0, saveGame.Scenes[i].Path);
                    continue;
                }

                _scenesToLoad.Add(saveGame.Scenes[i].Path);
            }
        }

        #endregion

        /// <summary>
        /// Load all scenes combined in a synchronous way.
        /// </summary>
        public void LoadScenesCombined()
        {
            State = LoadingState.IsLoading;
            for (int i = 0; i < _scenesToLoad.Count; i++)
            {
                if (_saveGameSettings.SceneLoading != SceneLoading.LoadAll &&
                    i == 0)
                {
                    continue;
                }

                var mode = i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive;
                SceneManager.LoadScene(_scenesToLoad[i], mode);
            }
        }

        /// <summary>
        /// Load all scenes combined in an asynchronous way.
        /// </summary>
        public void LoadScenesCombinedAsync()
        {
            State = LoadingState.IsLoading;
            for (int i = 0; i < _scenesToLoad.Count; i++)
            {
                if (_saveGameSettings.SceneLoading != SceneLoading.LoadAll &&
                    i == 0)
                {
                    continue;
                }

                var mode = i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive;
                SceneManager.LoadSceneAsync(_scenesToLoad[i], mode);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            _scenesToLoad.Remove(scene.path);

            if (_scenesToLoad.Count == 0)
            {
                State = LoadingState.FinishedLoading;
                OnLoadingCompleted?.Invoke(_serializableSaveGame, _saveGameSettings);
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
    }
}