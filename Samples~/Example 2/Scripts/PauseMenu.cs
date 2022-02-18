using UnityEngine;

namespace SimpleSave.Samples.Example2
{
    public class PauseMenu : MonoBehaviour
    {
        public void Resume()
        {
            GameManager.Instance.Resume();
        }

        public void OpenSaveMenu()
        {
            UiManager.Instance.OpenSaveMenu();
        }

        public void OpenLoadMenu()
        {
            UiManager.Instance.OpenLoadMenu();
        }

        public void Restart()
        {
            GameManager.Instance.Restart();
        }

    }
}