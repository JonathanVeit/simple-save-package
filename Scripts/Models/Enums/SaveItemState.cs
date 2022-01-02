namespace SimpleSave.Models
{
    /// <summary>
    /// All possible states of a <see cref="SaveItem"/>.
    /// </summary>
    public enum SaveItemState
    {
        /// <summary>I
        /// Default state of the item. It wont be saved or loaded and has to be initialized first.
        /// </summary>
        Uninitialized   = 0,

        /// <summary>
        /// Item located in a scene.
        /// </summary>
        Scene           = 1,

        /// <summary>
        /// Item is a prefab in the project.
        /// </summary>
        Prefab          = 2,

        /// <summary>
        /// Item is an instance of a prefab in a scene.
        /// </summary>
        PrefabInstance  = 3,
    }
}
