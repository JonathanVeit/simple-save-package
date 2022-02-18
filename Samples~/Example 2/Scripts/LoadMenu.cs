using SimpleSave.Models;
using UnityEngine;

namespace SimpleSave.Samples.Example2
{
    public class LoadMenu : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private Transform grid;
        [SerializeField] private SaveGameEntry entryPrefab;

        private SaveGameEntry selectedEntry;

        private void OnEnable()
        {
            SpawnSaveGames();
        }

        private void SpawnSaveGames()
        {
            foreach (Transform child in grid)
            {
                Destroy(child.gameObject);
            }

            var saveGameSettings = new SaveGameSettings
            {
                Location = Application.dataPath + "/Simple Save/Samples/Example 2/Savegames/",
                SceneLoading = SceneLoading.LoadAll
            };

            foreach (var saveGameInfo in Simple.GetAllSaveGames(saveGameSettings))
            {
                SpawnSaveGame(saveGameInfo);
            }
        }

        private void SpawnSaveGame(SaveGameInfo saveGameInfo)
        {
            var entry = Instantiate(entryPrefab, grid);
            entry.Initialize(saveGameInfo, OnEntrySelected);
        }

        private void OnEntrySelected(SaveGameEntry entry)
        {
            selectedEntry = entry;
        }

        public void Load()
        {
            if (selectedEntry is null)
            {
                return;
            }

            Simple.Load(selectedEntry.SaveGame);
        }

        public void Return()
        {
            UiManager.Instance.OpenPauseMenu();
            this.gameObject.SetActive(false);
        }
    }
}