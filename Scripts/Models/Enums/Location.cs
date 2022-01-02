using UnityEngine;

namespace SimpleSave.Models
{
    /// <summary>
    /// Locations to save into and load from.
    /// </summary>
    internal enum Location
    {
        /// <summary>
        /// Equivalent to <see cref="Application.dataPath"/>.
        /// </summary>
        DataPath = 0,

        /// <summary>
        /// Equivalent to <see cref="Application.persistentDataPath"/>.
        /// </summary>
        PersistentDataPath = 1,

        /// <summary>
        /// Equivalent to <see cref="Application.streamingAssetsPath"/>.
        /// </summary>
        StreamingAssetsPath = 2,

        /// <summary>
        /// Custom absolute path.
        /// </summary>
        Custom = 3,

        /// <summary>
        /// Saves directly to the PlayerPrefs.
        /// </summary>
        PlayerPrefs = 4,
    }
}
