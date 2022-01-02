using System;

namespace SimpleSave.Models
{
    /// <summary>
    /// Conditions to determine whether a save game has ended.
    /// </summary>
    [Flags]
    public enum SaveGameEndCondition
    {
        /// <summary>
        /// The active scene has changed.
        /// </summary>
        ActiveSceneChanged = 1 << 0,

        /// <summary>
        /// Any scene was unloaded.
        /// </summary>
        SceneUnloaded = 1 << 1,

        /// <summary>
        /// Any scene was loaded.
        /// </summary>
        SceneLoadedAdditive = 1 << 2,
    }
}
