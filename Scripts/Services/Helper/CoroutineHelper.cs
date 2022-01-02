using System.Collections;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <inheritdoc />
    internal class CoroutineHelper : ICoroutineHelper
    {
        private CoroutineRunner _coroutineRunner;

        /// <inheritdoc />
        public Coroutine RunCoroutine(IEnumerator enumerator)
        {
            if (_coroutineRunner is null)
            {
                CreateRunner();
            }

            return _coroutineRunner.StartCoroutine(enumerator);
        }

        private void CreateRunner()
        {
            var go = new GameObject("SimpleSave_CoroutineRunner");
            go.hideFlags = HideFlags.HideInHierarchy;
            Object.DontDestroyOnLoad(go);
            _coroutineRunner = go.AddComponent<CoroutineRunner>();
        }

        private class CoroutineRunner : MonoBehaviour
        {
        }
    }
}