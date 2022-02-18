using System;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSave.Samples.Example2
{
    public class SaveGameEntry : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private Text nameTest;

        public SaveGameInfo SaveGame { get; private set; }
        private Action<SaveGameEntry> onSelectCallback;

        public void Initialize(SaveGameInfo saveGame, Action<SaveGameEntry> onSelect)
        {
            this.SaveGame = saveGame;
            onSelectCallback = onSelect;

            nameTest.text = saveGame.Name;
        }

        public void Select()
        {
            onSelectCallback?.Invoke(this);
        }
    }
}