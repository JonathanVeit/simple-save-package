using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimpleSave.Samples.Example2
{
    public class GameManager : SimpleSaveBehaviour
    {
        [Header("settings")] 
        [SerializeField] private string sceneName;

        [SaveVar] public static int Score { get; private set; }
        public bool IsPaused => IsGameOver || IsMenu;

        private bool IsGameOver;
        private bool IsMenu;

        public static GameManager Instance { get; private set; }

        protected override void AwakeAlways()
        {
            Instance = this;
            Cursor.visible = false;
        }

        protected override void StartNormal()
        {
            Score = 0;
            UiManager.Instance.SetScore(Score);
            MapGenerator.Instance.GenerateMap();
        }

        protected override void StartSaveGame()
        {
            UiManager.Instance.SetScore(Score);
        }

        protected override void UpdateAlways()
        {
            if (IsGameOver)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (IsMenu)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Pause()
        {
            UiManager.Instance.OpenPauseMenu();
            IsMenu = true;
        }

        public void Resume()
        {
            UiManager.Instance.ClosePauseMenu();
            IsMenu = false;
        }

        public void GameOver()
        {
            IsGameOver = true;
            Cursor.visible = true;
            UiManager.Instance.OpenGameOverMenu();
        }

        public void Restart()
        {
            SceneManager.LoadScene(sceneName);
        }

        public void BlockDestroyed()
        {
            Score += 100;
            UiManager.Instance.SetScore(Score);
        }
    }
}