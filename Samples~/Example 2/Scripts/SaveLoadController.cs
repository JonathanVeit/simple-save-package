using SimpleSave.Models;
using UnityEngine;

namespace SimpleSave.Samples.Example2
{
    public class SaveLoadController : MonoBehaviour
    {
        public static SaveLoadController Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        public void Load(SaveGameInfo saveGameInfo)
        {
            var saveGameSettings = new SaveGameSettings
            {
                Location = Application.dataPath + "/Simple Save/Samples/Example 1/Savegames/",
                SceneLoading = SceneLoading.LoadAll
            };

            Simple.Load(saveGameInfo, saveGameSettings);
        }

        public void Save(string saveGameName)
        {
            var saveGameSettings = new SaveGameSettings
            {
                Location = Application.dataPath + "/Simple Save/Samples/Example 1/Savegames/",
                SceneLoading = SceneLoading.LoadAll
            };

            Simple.Save(saveGameName, saveGameSettings);
        }
    }
}