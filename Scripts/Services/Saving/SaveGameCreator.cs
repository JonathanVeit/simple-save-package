using System;
using System.Collections.Generic;
using System.Linq;
using SimpleSave.Extensions;
using SimpleSave.Models;
using SimpleSave.Models.Serializables;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveGameCreator"/>
    internal class SaveGameCreator : BaseService, ISaveGameCreator
    {
        /// <inheritdoc/>
        public event Action<SaveGameInfo> OnSaveGameCreated;

        /// <inheritdoc/>
        public event Action OnCreatingSaveGameFailed;

        #region Services

        private static ISaveItemConverter ItemConverter => ServiceWrapper.GetService<ISaveItemConverter>();

        private static ISaveItemController SaveItemController => ServiceWrapper.GetService<ISaveItemController>();

        private static ISaveVarController SaveVarController => ServiceWrapper.GetService<ISaveVarController>();

        private static ITagManager TagManager => ServiceWrapper.GetService<ITagManager>();

        private static IIdProvider IdProvider => ServiceWrapper.GetService<IIdProvider>();

        #endregion

        #region Synchronous

        /// <inheritdoc/>
        public SaveGameInfo CreateSaveGame(string name, SaveGameSettings saveGameSettings, Dictionary<string, string> customData)
        {
            try
            {
                DebugHelper.StartTimer("SaveGameCreator.Save");

                var sSaveGame = CreateSerializableSaveGame(name, saveGameSettings, customData);
                WriteSaveGame(sSaveGame, name, saveGameSettings);

                Logger.LogDebug(
                    $"Created save game \"{name}\" at location \"{saveGameSettings.Location}\" in {DebugHelper.StopTimer("SaveGameCreator.Save")} ms.");

                var saveGameInfo = new SaveGameInfo(sSaveGame.Info);
                OnSaveGameCreated?.Invoke(saveGameInfo);

                return saveGameInfo;
            }
            catch (Exception exception)
            {
                Logger.Log(LogType.Error, "Exception when trying to create the save game.");
                Logger.LogException(exception);

                OnCreatingSaveGameFailed?.Invoke();
                return SaveGameInfo.Invalid;
            }
        }

        private SerializableSaveGame CreateSerializableSaveGame(string name, SaveGameSettings saveGameSettings, Dictionary<string, string> customData)
        {
            return new SerializableSaveGame 
            {
                Info = CreateSaveGameInfo(name, saveGameSettings, customData),
                SaveVars = CreateSerializableSaveVars(saveGameSettings.SaveVarSerializer, saveGameSettings),
                Scenes = CreateSerializableScenes(saveGameSettings),
            };
        }

        private SerializableSaveGameInfo CreateSaveGameInfo(string name, SaveGameSettings saveGameSettings, Dictionary<string, string> customData)
        {
            var sCustomData = ConvertCustomData(customData);
            var info = new SerializableSaveGameInfo
            {
                Id = IdProvider.GetNewSaveGameId(),
                Name = name, 
                Created = DateTime.UtcNow.ToString(),
                Version = saveGameSettings.Version,
                Location = saveGameSettings.Location,
                CustomData = sCustomData,
            };

            return info;
        }

        private SerializableCustomDataEntry[] ConvertCustomData(Dictionary<string, string> customData)
        {
            if (customData == null)
            {
                return Array.Empty<SerializableCustomDataEntry>();
            }

            var result = new SerializableCustomDataEntry[customData.Count];

            int index = 0;
            foreach (var entry in customData)
            {
                result[index] = new SerializableCustomDataEntry()
                {
                    Key = entry.Key,
                    Value = entry.Value
                };

                index++;
            }

            return result;
        }

        private SerializableSaveVar[] CreateSerializableSaveVars(ISaveVarSerializer saveVarSerializer, SaveGameSettings saveGameSettings)
        {
            return SaveVarController.GetSaveVars(saveGameSettings);
        }

        private SerializableScene[] CreateSerializableScenes(SaveGameSettings saveGameSettings)
        {
            var totalSceneCount = SceneManager.sceneCount;
            var itemsByScene = SaveItemController.GetRegisteredItemsByScene();
            var result = new SerializableScene[totalSceneCount];

            for (int i = 0; i < totalSceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);

                if (itemsByScene.ContainsKey(scene))
                {
                    result[i] = new SerializableScene
                    {
                        Index = i,
                        Path = scene.path,
                        Items = ConvertToSerializableItems(itemsByScene[scene], saveGameSettings),
                    };

                    continue;
                }

                result[i] = new SerializableScene
                {
                    Index = i,
                    Path = scene.path,
                    Items = null,
                };
            }

            return result;
        }

        private SerializableSaveItem[] ConvertToSerializableItems(Dictionary<SaveItemId, SaveItem> items,
            SaveGameSettings saveGameSettings)
        {
            RemoveInvalidItems(items, saveGameSettings);
            var result = new SerializableSaveItem[items.Count];

            int index = 0;
            foreach (var entry in items)
            {
                if (ItemIsInvalid(entry.Value))
                {
                    Logger.LogInternal(
                        $"{nameof(SaveItem)} with id \"{entry.Key}\" seems to be destroyed and therefore wont be saved.");
                    continue;
                }
                
                if (saveGameSettings.Tags != null &&
                    !TagManager.DoAnyTagsMatch(entry.Value.GetTags(), saveGameSettings.Tags))
                {
                    continue;
                }

                result[index] = ItemConverter.ToSerializable(entry.Value, saveGameSettings);
                index++;
            }

            return result;
        }

        private static void RemoveInvalidItems(Dictionary<SaveItemId, SaveItem> items, SaveGameSettings saveGameSettings)
        {
            var keys = items.Keys.ToArray();

            foreach (var key in keys)
            {
                if (ItemIsInvalid(items[key]))
                {
                    items.Remove(key);
                }

                if (!TagManager.DoAnyTagsMatch(items[key].GetTags(), saveGameSettings.Tags))
                {
                    items.Remove(key);
                }
            }
        }

        private static bool ItemIsInvalid(SaveItem item)
        {
            return item.gameObject == null;
        }

        private static void WriteSaveGame(SerializableSaveGame sSaveGame, string fileName,
            SaveGameSettings saveGameSettings)
        {
            string serializedSaveGame = saveGameSettings.SaveGameSerializer.Serialize(sSaveGame);

            if (saveGameSettings.UsePlayerPrefs)
            {
                saveGameSettings.SaveGameWriter.WriteToPlayerPrefs(serializedSaveGame, fileName);
            }
            else
            {
                saveGameSettings.SaveGameWriter.Write(serializedSaveGame, fileName, saveGameSettings.Location);
            }
        }

        #endregion
    }
}