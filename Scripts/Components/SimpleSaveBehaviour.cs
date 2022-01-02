using SimpleSave.Services;
using UnityEngine;

namespace SimpleSave
{
    /// <summary>
    /// Provides <see cref="MonoBehaviour"/> methods based on the save game state: Awake(), Start(), Update(), OnDestroy()
    /// </summary>
    /// <remarks>
    /// You can override these methods with the "Default" postfix to only be called outside of a save game <br/>
    /// or with the "SaveGame" postfix to only be called inside of a save game. <br/>
    /// Methods with the "Always" postfix are called no matter if its a save game or not. <br/>
    /// <br/>
    /// DO NOT implement the default <see cref="MonoBehaviour"/> methods Start(), Awake(), Update() or OnDestroy()!
    /// </remarks>
    /// <example>
    /// <code>
    /// public class ExampleClass : SimpleSaveBehaviour
    /// {
    ///     // this will only be called inside a save game
    ///     protected override void UpdateSaveGame()
    ///     {
    ///         SomeSaveGameBehaviour(); 
    ///     }
    ///
    ///     // this will only be called outside a save game
    ///     protected override void UpdateNormal()
    ///     {
    ///         SomeNormalBehaviour(); 
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class SimpleSaveBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Is this currently a save game?
        /// </summary>
        protected bool IsSaveGame => SaveGameLoader.IsSaveGame;

        private static ISaveGameLoader SaveGameLoader;
        private bool _calledStart;

        #region Awake

        private void Awake()
        {
            SaveGameLoader = ServiceWrapper.GetService<ISaveGameLoader>();

            if (IsSaveGame)
            {
                AwakeSaveGame();
                SaveGameLoader.OnSaveGameEnded += ListenOnSaveGamEnded;
            }
            else
            {
                AwakeNormal();   
            }

            AwakeAlways();
        }

        /// <summary>
        /// Will only be called outside a save game.
        /// </summary>
        protected virtual void AwakeNormal()
        {
        }

        /// <summary>
        /// Will always be called.
        /// </summary>
        protected virtual void AwakeAlways()
        {

        }

        /// <summary>
        /// Will only be called inside a save game.
        /// </summary>
        protected virtual void AwakeSaveGame()
        {

        }

        #endregion

        #region Start

        private void Start()
        {
            if (IsSaveGame)
            {
                if (SaveGameLoader.IsLoading)
                {
                    SaveGameLoader.OnSaveGameLoaded += CallStartSaveGame;
                }
                else
                {
                    CallStartSaveGame(SaveGameInfo.Invalid);
                }
            }
            else
            {
                StartNormal();
                _calledStart = true;
            }

            StartAlways();
        }

        private void CallStartSaveGame(SaveGameInfo saveGameInfo)
        {
            SaveGameLoader.OnSaveGameLoaded -= CallStartSaveGame;
            StartSaveGame();
            _calledStart = true;
        }

        /// <summary>
        /// Will only be called outside a save game.
        /// </summary>
        protected virtual void StartNormal()
        {
        }

        /// <summary>
        /// Will always be called.
        /// </summary>
        protected virtual void StartAlways()
        {

        }

        /// <summary>
        /// Will only be called inside a save game.
        /// </summary>
        protected virtual void StartSaveGame()
        {

        }

        #endregion

        #region Update

        private void Update()
        {
            if (IsSaveGame)
            {
                if (!SaveGameLoader.IsLoading && 
                    _calledStart)
                {
                    UpdateSaveGame();
                }
            }
            else
            {
                UpdateNormal();
            }

            UpdateAlways();
        }

        /// <summary>
        /// Will only be called outside a save game.
        /// </summary>
        protected virtual void UpdateNormal()
        {
        }

        /// <summary>
        /// Will always be called.
        /// </summary>
        protected virtual void UpdateAlways()
        {

        }

        /// <summary>
        /// Will only be called inside a save game.
        /// </summary>
        protected virtual void UpdateSaveGame()
        {
        }

        #endregion

        #region Destroy

        private void OnDestroy()
        {
            if (IsSaveGame)
            {
                OnDestroySaveGame();
            }
            else
            {
                OnDestroyNormal();
            }

            OnDestroyAlways();
        }

        /// <summary>
        /// Will only be called outside a save game.
        /// </summary>
        protected virtual void OnDestroyNormal()
        {
        }

        /// <summary>
        /// Will always be called.
        /// </summary>
        protected virtual void OnDestroyAlways()
        {
        }

        /// <summary>
        /// Will only be called inside a save game.
        /// </summary>
        protected virtual void OnDestroySaveGame()
        {

        }

        #endregion

        #region Save Game End

        private void ListenOnSaveGamEnded()
        {
            SaveGameLoader.OnSaveGameEnded -= ListenOnSaveGamEnded;
            OnSaveGameEnded();
        }

        /// <summary>
        /// Will be called when the save game ended.
        /// </summary>
        protected virtual void OnSaveGameEnded()
        {
        }

        #endregion
    }
}