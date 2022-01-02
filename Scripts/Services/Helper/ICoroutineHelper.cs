using System.Collections;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <summary>
    /// Helper class to handle coroutines in none-<see cref="MonoBehaviour"/> classes.
    /// </summary>
    internal interface ICoroutineHelper
    { 
        /// <summary>
        /// Starts the coroutine.
        /// </summary>
        /// <param name="enumerator">Coroutine to start.</param>
        /// <returns>The started coroutine.</returns>
        Coroutine RunCoroutine(IEnumerator enumerator);
    }
}