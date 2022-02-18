using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SimpleSave.Samples.Example1
{
    public class UiManager : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private GameObject menuRoot;
        [SerializeField] private string sceneName;
        [SerializeField] private Text durationText;

        public static UiManager Instance { get; private set; }

        public bool MenuIsOpen => menuRoot.activeSelf || menuTimer < 0.1f;
        private float menuTimer;

        private void Awake()
        {
            Instance = this;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;

            GlobalVariables.Duration = 0;
        }

        private void Update()
        {
            GlobalVariables.Duration += Time.deltaTime;
            durationText.text = TimeSpan.FromSeconds(GlobalVariables.Duration).ToString(@"mm\:ss");
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenu();
            }

            menuTimer += Time.deltaTime;
        }

        private void ToggleMenu()
        {
            menuRoot.SetActive(!menuRoot.activeSelf);
            menuTimer = 0;
            Cursor.lockState = menuRoot.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = menuRoot.activeSelf;
        }

        public void Save()
        {
            SaveLoadController.Instance.Save();
            ToggleMenu();
        }

        public void Load()
        {
            SaveLoadController.Instance.Load();
        }

        public void Restart()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}