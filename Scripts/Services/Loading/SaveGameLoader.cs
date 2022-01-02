using System;
using System.Collections;
using System.Collections.Generic;
using SimpleSave.Extensions;
using SimpleSave.Models;
using SimpleSave.Models.Serializables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveGameLoader"/>
    internal class SaveGameLoader : BaseService, ISaveGameLoader
    {
        /// <inheritdoc />
        public event Action<SaveGameInfo> OnSaveGameLoaded;

        /// <inheritdoc />
        public event Action<SaveGameInfo> OnLoadingSaveGameFailed;

        /// <inheritdoc />
        public event Action OnSaveGameEnded;

        /// <inheritdoc />
        public bool IsSaveGame { get; private set; }

        /// <inheritdoc />
        public bool IsLoading { get; private set; }

        private readonly SaveGameSceneLoader _saveGameSceneLoader;
        private readonly Dictionary<SaveGameId, SerializableSaveGame> _saveGameCache;

        public SaveGameLoader()
        {
            _saveGameSceneLoader = new SaveGameSceneLoader();
            _saveGameCache = new Dictionary<SaveGameId, SerializableSaveGame>();

            SceneManager.sceneUnloaded += ListenSceneUnloaded;
            SceneManager.sceneLoaded += ListenSceneLoaded;
        }

        #region Services

        private static ISaveItemConverter SaveItemConverter => ServiceWrapper.GetService<ISaveItemConverter>();

        private static ISaveItemController SaveItemController => ServiceWrapper.GetService<ISaveItemController>();

        private static ISaveVarController SaveVarController => ServiceWrapper.GetService<ISaveVarController>();

        private static ITagManager TagManager => ServiceWrapper.GetService<ITagManager>();

        #endregion

        #region Synchronous

        /// <inheritdoc/>
        public void Load(SaveGameInfo saveGameInfo, SaveGameSettings saveGameSettings)
        {
            if (IsLoading)
            {
                Logger.Log(LogType.Error, "There is already a save game loading. Please wait until the loading process was finished.");
                return;
            }

            if (!saveGameInfo.IsValid)
            {
                Logger.Log(LogType.Error, "The given save game is invalid and cannot be loaded.");
                return;
            }

           LoadInternal(saveGameInfo, saveGameSettings);
        }

        private void LoadInternal(SaveGameInfo saveGameInfo, SaveGameSettings saveGameSettings)
        {
            try
            {
                DebugHelper.StartTimer("SaveGameLoader.Load");

                IsLoading = true;
                SetSaveGameState(true);

                SerializableSaveGame sSaveGame;
                if (_saveGameCache.TryGetValue(saveGameInfo.Id, out var cachedSaveGame))
                {
                    sSaveGame = cachedSaveGame;
                }
                else
                {
                    sSaveGame = ReadSerializableSaveGame(saveGameInfo, saveGameSettings);
                    _saveGameCache.Add(sSaveGame.Info.Id, sSaveGame);
                }

                if (saveGameSettings.SceneLoading == SceneLoading.SkipLoading)
                {
                    LoadToCurrentScenes(sSaveGame, saveGameSettings);
                    return;
                }

                if (!ValidateScenes(sSaveGame))
                {
                    return;
                }

                LoadWithScenes(sSaveGame, saveGameSettings);
            }
            catch (Exception exception)
            {
                Logger.Log(LogType.Error, $"Exception when trying to load the save game.");
                Logger.LogException(exception);

                IsLoading = false;
                IsSaveGame = false;

                OnLoadingSaveGameFailed?.Invoke(saveGameInfo);
            }
        }

        private SerializableSaveGame ReadSerializableSaveGame(SaveGameInfo saveGameInfo,
            SaveGameSettings saveGameSettings)
        {
            string serializedSaveGame = string.Empty;

            if (saveGameSettings.UsePlayerPrefs)
            {
                saveGameSettings.SaveGameReader.ReadFromPlayerPrefs(saveGameInfo.Name);
            }
            else
            {
                serializedSaveGame = saveGameSettings.SaveGameReader.Read(saveGameInfo.Name, saveGameInfo.Location);
            }

            return saveGameSettings.SaveGameSerializer.Deserialize(serializedSaveGame);
        }

        private void LoadToCurrentScenes(SerializableSaveGame sSaveGame, SaveGameSettings saveGameSettings)
        {
            try
            {
                var sceneLoadingDuration = DebugHelper.GetTimer("SaveGameLoader.Load");

                _saveGameSceneLoader.OnLoadingCompleted -= LoadToCurrentScenes;
                LoadSaveItems(sSaveGame, saveGameSettings);
                LoadSaveVars(sSaveGame, saveGameSettings);

                IsLoading = false;
                OnSaveGameLoaded?.Invoke(new SaveGameInfo(sSaveGame.Info));

                Logger.LogDebug(
                    $"Loaded save game \"{sSaveGame.Info.Name}\" from \"{sSaveGame.Info.Location}\" in {DebugHelper.StopTimer("SaveGameLoader.Load")} ms ({sceneLoadingDuration} ms for scenes).");
            }
            catch (Exception exception)
            {
                Logger.Log(LogType.Error, $"Exception when trying to load the save game.");
                Logger.LogException(exception);

                IsLoading = false;
                IsSaveGame = false;

                OnLoadingSaveGameFailed?.Invoke(new SaveGameInfo(sSaveGame.Info));
            }
        }
        
        private void LoadWithScenes(SerializableSaveGame sSaveGame, SaveGameSettings saveGameSettings)
        {
            _saveGameSceneLoader.Initialize(sSaveGame, saveGameSettings);
            _saveGameSceneLoader.OnLoadingCompleted += LoadToCurrentScenes;

            _saveGameSceneLoader.LoadScenesCombined();
        }

        private bool ValidateScenes(SerializableSaveGame sSaveGame)
        {
            for (int i = 0; i < sSaveGame.Scenes.Length; i++)
            {
                var path = sSaveGame.Scenes[i].Path;
                if (SceneUtility.GetBuildIndexByScenePath(path) < 0)
                {
                    throw new ArgumentException($"Scene \"{path}\" cannot be found");
                }
            }

            return true;
        }

        private void LoadSaveVars(SerializableSaveGame sSaveGame, SaveGameSettings saveGameSettings)
        {
            SaveVarController.LoadSaveVars(sSaveGame.SaveVars, saveGameSettings);
        }

        private void LoadSaveItems(SerializableSaveGame saveGame, SaveGameSettings saveGameSettings)
        {
            CreatePrefabInstances(saveGame, saveGameSettings);

            for (int i = 0; i < saveGame.Scenes.Length; i++)
            {
                LoadItemsOfScene(saveGame.Scenes[i], saveGameSettings);
            }
        }

        private void CreatePrefabInstances(SerializableSaveGame saveGame, SaveGameSettings saveGameSettings)
        {
            for (int i = 0; i < saveGame.Scenes.Length; i++)
            {
                for (int j = 0; j < saveGame.Scenes[i].Items.Length; j++)
                {
                    var sItem = saveGame.Scenes[i].Items[j];

                    if (sItem.State != SaveItemState.PrefabInstance ||
                        saveGameSettings.SceneLoading != SceneLoading.LoadAll)
                    {
                        continue;
                    }

                    if (saveGameSettings.Tags != null &&
                        !TagManager.DoAnyTagsMatch(sItem.Tags, saveGameSettings.Tags))
                    {
                        continue;
                    }

                    CreatePrefabInstance(sItem, saveGame.Scenes[i].Path, saveGameSettings.PrefabInstanceProvider);
                }
            }
        }

        private static void CreatePrefabInstance(SerializableSaveItem sItem, string targetScenePath,
            IPrefabInstanceProvider prefabInstanceProvider)
        {
            var instance = prefabInstanceProvider.GetInstance(sItem.PrefabId);

            instance.SetId(sItem.Id);
            instance.SetState(sItem.State);

            if (instance.gameObject.scene.path != targetScenePath)
            {
                var scene = SceneManager.GetSceneByPath(targetScenePath);
                SceneManager.MoveGameObjectToScene(instance.gameObject, scene);
            }

            SaveItemController.RegisterItem(instance);
        }

        private void LoadItemsOfScene(SerializableScene serializableScene, SaveGameSettings saveGameSettings)
        {
            var itemsInScene = SaveItemController.GetRegisteredItems();

            for (int i = 0; i < serializableScene.Items.Length; i++)
            {
                var sItem = serializableScene.Items[i];

                if (!TagManager.DoAnyTagsMatch(sItem.Tags, saveGameSettings.Tags))
                {
                    continue;
                }

                if (itemsInScene.TryGetValue(sItem.Id, out var saveItemInScene))
                {
                    SaveItemConverter.ToItem(sItem, saveItemInScene, saveGameSettings);
                    continue;
                }

                Logger.LogInternal(
                    $"{nameof(SaveItem)} with id \"{sItem.Id}\" could not be found in any scene and wont be loaded.");
            }
        }

        #endregion

        #region Helper

        private static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return CoroutineHelper.RunCoroutine(coroutine);
        }

        private void ListenSceneUnloaded(Scene unloadedScene)
        {
            if (IsLoading)
            {
                return;
            }

            if (Settings.SaveGameEndCondition.HasFlag(SaveGameEndCondition.SceneUnloaded) ||
                Settings.SaveGameEndCondition.HasFlag(SaveGameEndCondition.ActiveSceneChanged) &&
                unloadedScene == SceneManager.GetActiveScene())
            {
                SetSaveGameState(false);
            }
        }

        private void ListenSceneLoaded(Scene loadedScene, LoadSceneMode mode)
        {
            if (IsLoading)
            {
                return;
            }
            
            if (mode == LoadSceneMode.Additive && 
                Settings.SaveGameEndCondition.HasFlag(SaveGameEndCondition.SceneLoadedAdditive))
            {
                SetSaveGameState(false);
            }
        }

        private void SetSaveGameState(bool isSaveGame)
        {
            var wasSaveGame = IsSaveGame;
            IsSaveGame = isSaveGame;

            if (wasSaveGame &&
                !IsSaveGame)
            {
                OnSaveGameEnded?.Invoke();
            }
        }

        #endregion
    }
}