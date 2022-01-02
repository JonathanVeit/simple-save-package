using SimpleSave.Models;
using UnityEngine.SceneManagement;

namespace SimpleSave.Services
{
    /// <inheritdoc/>
    internal class SaveItemHelper : ISaveItemHelper
    {
        /// <inheritdoc/>
        public bool IsInPrefabStage(SaveItem item)
        {
            // There is currently no better way to check this.
            // Alternatively the UnityEditor.Experimental.PrefabStage class could return the scene 
            // but since its experimental, I decided to not use it.
            var itemScene = item.gameObject.scene;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i) == itemScene)
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public SaveItemState GetItemState(SaveItem item)
        {
            if (string.IsNullOrEmpty(item.gameObject.scene.name) ||
                IsInPrefabStage(item))
            {
                return SaveItemState.Prefab;
            }

#if UNITY_EDITOR

            if (UnityEditor.PrefabUtility.GetPrefabInstanceStatus(item.gameObject) == UnityEditor.PrefabInstanceStatus.NotAPrefab)
            {
                return SaveItemState.Scene;
            }
#endif

            return SaveItemState.PrefabInstance;
        }
    }
}
