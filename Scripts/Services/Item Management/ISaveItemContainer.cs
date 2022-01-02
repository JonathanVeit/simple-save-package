using SimpleSave.Models;

namespace SimpleSave.Services
{
    /// <summary>
    /// Container to store <see cref="SaveItemId"/> and their associated <see cref="SaveItem"/> in a unity scene.
    /// </summary>
    /// <remarks>
    /// There is only one container per scene.
    /// </remarks>
    internal interface ISaveItemContainer
    {
        /// <summary>
        /// Adds a new item and its id.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="id">Id to add for the item.</param>
        void AddItem(SaveItem item, SaveItemId id);

        /// <summary>
        /// Determine whether the item is stored in the container.
        /// </summary>
        /// <param name="item">Item to determine.</param>
        /// <returns>True if the container stored the item.</returns>
        bool ContainsItem(SaveItem item);

        /// <summary>
        /// Determine whether an item with the given id is stored in the container.
        /// </summary>
        /// <param name="id">Id to determine.</param>
        /// <returns>True if the container stores and item with the given id.</returns>
        bool ContainsItemWithId(SaveItemId id);

        /// <summary>
        /// Returns the id stored for the given item.
        /// </summary>
        /// <param name="item">The item to get the id for.</param>
        /// <returns>The id stored for the item.</returns>
        /// <remarks></remarks>
        SaveItemId GetIdFor(SaveItem item);

        /// <summary>
        /// Overrides the id for the given item.
        /// </summary>
        /// <param name="item">Item to override the id.</param>
        /// <param name="newId">New id.</param>
        void OverrideId(SaveItem item, SaveItemId newId);

        /// <summary>
        /// Removes the item from the container.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        void RemoveItem(SaveItem item);
    }
}