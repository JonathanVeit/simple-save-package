using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimpleSave.Samples.Example1
{
    public class UiManager : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private GameObject menuRoot;
        [SerializeField] private string sceneName;

        public static UiManager Instance { get; private set; }
        public bool MenuIsOpen => menuRoot.activeSelf || menuTimer < 0.1f;
        private float menuTimer;


        private void Awake()
        {
            Instance = this;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
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