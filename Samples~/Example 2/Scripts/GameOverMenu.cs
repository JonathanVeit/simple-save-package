using UnityEngine;

namespace SimpleSave.Samples.Example2
{
    public class GameOverMenu : MonoBehaviour
    {
        public void Restart()
        {
            GameManager.Instance.Restart();
        }
    }
}