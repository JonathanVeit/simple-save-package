using UnityEngine;
using UnityEngine.UI;

namespace SimpleSave.Samples.Example2
{
    public class UiManager : MonoBehaviour
    {
        [Header("settings")] 
        [SerializeField] private Text scoreText;
        [SerializeField] private GameObject gameOverMenuRoot;
        [SerializeField] private GameObject pauseMenuRoot;
        [SerializeField] private GameObject saveMenuRoot;
        [SerializeField] private GameObject loadMenuRoot;

        public static UiManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void SetScore(int value)
        {
            scoreText.text = value.ToString();
        }

        public void OpenPauseMenu()
        {
            pauseMenuRoot.SetActive(true);
        }

        public void ClosePauseMenu()
        {
            pauseMenuRoot.SetActive(false);
            pauseMenuRoot.SetActive(false);
            saveMenuRoot.SetActive(false);
            loadMenuRoot.SetActive(false);
        }

        public void OpenLoadMenu()
        {
            pauseMenuRoot.SetActive(false);
            loadMenuRoot.SetActive(true);
        }

        public void OpenSaveMenu()
        {
            pauseMenuRoot.SetActive(false);
            saveMenuRoot.SetActive(true);
        }

        public void OpenGameOverMenu()
        {
            gameOverMenuRoot.SetActive(true);
        }

        public void Restart()
        {
            GameManager.Instance.Restart();
        }
    }
}