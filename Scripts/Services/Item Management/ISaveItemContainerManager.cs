namespace SimpleSave.Services
{
    /// <summary>
    /// Manages the <see cref="ISaveItemContainer"/> instances.
    /// </summary>
    /// <remarks>
    /// Container will be created automatically if needed.
    /// </remarks>
    internal interface ISaveItemContainerManager
    {
        /// <summary>
        /// Returns container responsible for storing the given item.
        /// </summary>
        /// <param name="item">Item to get the container for.</param>
        /// <returns>The container.</returns>
        /// <remarks>Will create a new container if necessary.</remarks>
        ISaveItemContainer GetContainerFor(SaveItem item);
    }
}
