using SimpleSave.Models;
using UnityEngine;

namespace SimpleSave.Samples.Example1
{
    public class SaveLoadController : MonoBehaviour
    {
        public static SaveLoadController Instance { get; private set; }

        private const string saveGameName = "SaveGame";
        private static SaveGameInfo currentGameInfo;

        void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
        }

        public void Load()
        {
            if (currentGameInfo.IsValid)
            {
                var saveGameSettings = new SaveGameSettings
                {
                    Location = Application.dataPath + "/Simple Save/Samples/Example 1/Savegames/",
                    SceneLoading = SceneLoading.LoadAll
                };

                Simple.Load(currentGameInfo, saveGameSettings);
            }
        }

        public void Save()
        {
            var saveGameSettings = new SaveGameSettings
            {
                Location = Application.dataPath + "/Simple Save/Samples/Example 1/Savegames/",
                SceneLoading = SceneLoading.LoadAll
            };

            currentGameInfo = Simple.Save(saveGameName, saveGameSettings);
        }
    }
}