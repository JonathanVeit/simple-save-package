using SimpleSave.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSave.Samples.Example2
{
    public class SaveMenu : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private InputField saveGameName;

        public void Save()
        {
            if (string.IsNullOrEmpty(saveGameName.text))
            {
                return;
            }
            var saveGameSettings = new SaveGameSettings
            {
                Location = Application.dataPath + "/Simple Save/Samples/Example 2/Savegames/",
                SceneLoading = SceneLoading.LoadAll
            };

            Simple.Save(saveGameName.text, saveGameSettings);
            Return();
        }

        public void Return()
        {
            UiManager.Instance.OpenPauseMenu();
            this.gameObject.SetActive(false);
        }
    }
}