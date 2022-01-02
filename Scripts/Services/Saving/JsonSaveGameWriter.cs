using System;
using System.Collections.Generic;
using System.IO;
using SimpleSave.Extensions;
using UnityEngine;


namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveGameWriter"/>
    internal class JsonSaveGameWriter : BaseService, ISaveGameWriter
    {
        private const string PlayerPrefsNameListKey = "SaveGameList";

        /// <inheritdoc/>
        public void Write(string serializedSaveGame, string fileName, string location)
        {
            CheckOrCreateDictionary(location);

            var filePath = CreateFilePath(fileName, location);
            if (WouldOverrideButNotAllowed(filePath))
            { 
                throw new ArgumentException($"The save game \"{filePath}\" already exists. Overriding is disabled.");
            }

            var sw = new StreamWriter(filePath);
            sw.Write(serializedSaveGame);
            sw.Close();
        }

        /// <inheritdoc/>
        public void WriteToPlayerPrefs(string serializedSaveGame, string fileName)
        {
            string saveGameName = CreatePlayerPrefsName(fileName);

            var nameList = LoadPlayerPrefsNameCollection();
            nameList.Add(saveGameName);
            SavePlayerPrefsNameCollection(nameList);

            PlayerPrefs.SetString(saveGameName, serializedSaveGame);
            PlayerPrefs.Save();
        }

        /// <inheritdoc/>
        public void Delete(string fileName, string location)
        {
            CheckOrCreateDictionary(location);

            var path = CreateFilePath(fileName, location);
            File.Delete(path);
        }

        /// <inheritdoc/>
        public void DeleteFromPlayerPrefs(string fileName)
        {
            string saveGameName = CreatePlayerPrefsName(fileName);

            PlayerPrefs.DeleteKey(saveGameName);
            
            var nameList = LoadPlayerPrefsNameCollection();
            nameList.Remove(saveGameName);
            SavePlayerPrefsNameCollection(nameList);

            PlayerPrefs.Save();
        }

        #region Helper
        
        private static string CreateFilePath(string name, string location)
        {
            return $"{location}/{name}.json";
        }

        private static string CreatePlayerPrefsName(string fileName)
        {
            return $"SaveGame_{fileName}";
        }

        private static void CheckOrCreateDictionary(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            } 
            
            if (Settings.AutoCreateDirectory)
            {
                Logger.LogDebug($"Created directory at path \"{path}\".");
                Directory.CreateDirectory(path);
                return;
            }

            throw new DirectoryNotFoundException($"The directory \"{path}\" could not be found.");
        }

        private static bool WouldOverrideButNotAllowed(string filePath)
        {
            if (Settings.OverrideSaveGames)
            {
                return false;
            }

            return File.Exists(filePath);
        }

        private SaveGameNameCollection LoadPlayerPrefsNameCollection()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsNameListKey))
            {
                var serializedList = PlayerPrefs.GetString(PlayerPrefsNameListKey);
                var saveGameList = JsonUtility.FromJson<SaveGameNameCollection>(serializedList);

                return saveGameList;
            }

            return new SaveGameNameCollection();
        }

        private void SavePlayerPrefsNameCollection(SaveGameNameCollection saveGameNameCollection)
        {
            var serialized = JsonUtility.ToJson(saveGameNameCollection);
            PlayerPrefs.SetString(PlayerPrefsNameListKey, serialized);
        }

        private struct SaveGameNameCollection
        {
             [SerializeField] private List<string> _saveGameNames;

            public void Add(string saveGameName)
            {
                _saveGameNames ??= new List<string>();

                if (!_saveGameNames.Contains(saveGameName))
                {
                    _saveGameNames.Add(saveGameName);
                }
            }

            public void Remove(string saveGameName)
            {
                _saveGameNames ??= new List<string>();

                _saveGameNames.Remove(saveGameName);
            }
        }

        #endregion

    }
}