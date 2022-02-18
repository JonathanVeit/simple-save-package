using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SimpleSave.Extensions;
using SimpleSave.Models.Serializables;
using UnityEngine;


namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveGameReader"/>
    internal class JsonSaveGameReader : BaseService, ISaveGameReader
    {
        private const string PlayerPrefsNameListKey = "SaveGameList";

        private const string InfoFieldIdentifier = "Info";
        private const string InfoIdIdentifier = "Id";
        private const string InfoNameIdentifier = "Name";
        private const string InfoCreatedIdentifier = "Created";
        private const string InfoVersionIdentifier = "Version";
        private const string InfoLocationIdentifier = "Location";
        private const string InfoCustomDataIdentifier = "CustomData";

        /// <inheritdoc/>
        public string Read(string fileName, string location)
        {
            CheckOrCreateDictionary(location);

            var filePath = CreateFilePath(fileName, location);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Save game file \"{filePath}\" not found.");
            }

            StreamReader sr = new StreamReader(filePath);
            var result = sr.ReadToEnd();
            sr.Close();

            return result;
        }

        /// <inheritdoc/>
        public string ReadFromPlayerPrefs(string fileName)
        {
            string saveGameName = CreatePlayerPrefsName(fileName);

            if (!PlayerPrefs.HasKey(saveGameName))
            {
                throw new FileNotFoundException($"Save game \"{fileName}\" could not be found in PlayerPrefs.");
            }
          
            return PlayerPrefs.GetString(saveGameName);
        }

        /// <inheritdoc/>
        public SaveGameInfo[] GetAll(string location)
        {
            CheckOrCreateDictionary(location);

            var directoryInfo = new DirectoryInfo(location);
            var files = directoryInfo.GetFiles();
            List<SaveGameInfo> result = new List<SaveGameInfo>();

            for (int i = 0; i < files.Length; i++)
            {
                if (TryCreateInfo(files[i], out var info))
                {
                    result.Add(info);
                }
            }

            return result.ToArray();
        }

        /// <inheritdoc/>
        public SaveGameInfo[] GetAllFromPlayerPrefs()
        {
            var nameList = LoadPlayerPrefsNameCollection();
            var allNames = nameList.GetAllNames();

            var result = new List<SaveGameInfo> ();

            for (int i = 0; i < allNames.Length; i++)
            {
                var stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(PlayerPrefs.GetString(allNames[i])));

                if (TryCreateInfo(stream, "PlayerPrefs", out SaveGameInfo saveGameInfo))
                {
                    result.Add(saveGameInfo);
                }
            }

            return result.ToArray();
        }

        #region Helper

        private static bool TryCreateInfo(FileInfo file, out SaveGameInfo info)
        {
            if (file.Extension != ".json")
            {
                info = default;
                return false;
            }

            using FileStream stream = file.OpenRead();
            return TryCreateInfo(stream, file.DirectoryName, out info);
        }

        private static bool TryCreateInfo(Stream stream, string directory, out SaveGameInfo info)
        {
            bool reachedInfoField = false;
            bool foundInfo = false;

            string id = string.Empty;
            string name = string.Empty;
            string created = string.Empty;
            string version = string.Empty;
            string location = string.Empty;
            SerializableCustomDataEntry[] customData = null;

            using (var sr = new StreamReader(stream))
            using (var reader = new JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    if (!reachedInfoField)
                    {
                        if (reader.Depth == 1 &&
                            reader.TokenType == JsonToken.PropertyName &&
                            reader.Value?.ToString() == InfoFieldIdentifier)
                        {
                            reachedInfoField = true;
                        }

                        continue;
                    }

                    if (reader.TokenType != JsonToken.PropertyName)
                    {
                        continue;
                    }

                    var propertyIdentifier = reader.Value?.ToString();
                    reader.Read();
                    var propertyValue = reader.Value?.ToString();

                    switch (propertyIdentifier)
                    {
                        case InfoIdIdentifier:
                            // we read until we find the id
                            reader.Read(); // -> "_value"
                            reader.Read(); // -> id
                            id = reader.Value?.ToString();
                            break;
                        case InfoNameIdentifier:
                            name = propertyValue;
                            break;
                        case InfoCreatedIdentifier:
                            created = propertyValue;
                            break;
                        case InfoVersionIdentifier:
                            version = propertyValue;
                            break;
                        case InfoLocationIdentifier:
                            location = directory;
                            break;
                        case InfoCustomDataIdentifier:
                            customData = ReadCustomData(reader);
                            break;

                        default:
                            Logger.Log(LogType.Warning, $"Found unexpected property {propertyIdentifier} when trying to read the {nameof(SaveGameInfo)} " +
                                                        $"from save game {directory}.");
                            break;
                    }

                    if (!string.IsNullOrEmpty(name) &&
                        !string.IsNullOrEmpty(created) &&
                        !string.IsNullOrEmpty(version) &&
                        !string.IsNullOrEmpty(location) &&
                        customData != null)
                    {
                        stream.Close();
                        sr.Close();
                        reader.Close();

                        foundInfo = true;
                        break;
                    }
                }
            }

            if (!foundInfo)
            {
                info = SaveGameInfo.Invalid;
                return false;
            }

            info = new SaveGameInfo(id, name, created, version, location, customData);
            return true;
        }

        private static SerializableCustomDataEntry[] ReadCustomData(JsonTextReader reader)
        {
            var startDepth = reader.Depth + 1;
            var result = new List<SerializableCustomDataEntry>();

            while (reader.Read())
            {
                if (reader.Depth < startDepth)
                {
                    break;
                }

                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                reader.Read();
                string key = reader.Value?.ToString();
                reader.Read();
                reader.Read();
                string value = reader.Value?.ToString();

                result.Add(new SerializableCustomDataEntry
                {
                    Key = key,
                    Value = value,
                });
            }

            return result.ToArray();
        }

        private static string CreateFilePath(string fileName, string location)
        {
            return $"{location}/{fileName}.json";
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

        private struct SaveGameNameCollection
        {
            [SerializeField] private List<string> _saveGameNames;

            public string[] GetAllNames()
            {
                return _saveGameNames.ToArray();
            }
        }

        #endregion
    }
}