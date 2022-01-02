using System.Collections.Generic;
using SimpleSave.Models;
using UnityEngine.SceneManagement;

namespace SimpleSave.Services
{
    /// <summary>
    /// Keeps track of all <see cref="SaveItem"/> instances and initializes them based on their <see cref="SaveItemState"/>.
    /// </summary>
    internal interface ISaveItemController
    {
        /// <summary>
        /// Registers a <see cref="SaveItem"/>.
        /// </summary>
        /// <param name="item">The item to register.</param>
        /// <remarks>
        /// The item will automatically be initialized if necessary and <br/>
        /// state mismatching and duplication will automatically be handled.
        /// </remarks>
        void RegisterItem(SaveItem item);

        /// <summary>
        /// Returns all currently registered items.
        /// </summary>
        /// <returns>Items in as dictionary by their id.</returns>
        Dictionary<SaveItemId, SaveItem> GetRegisteredItems();

        /// <summary>
        /// Returns all currently registered items sorted by their scene.
        /// </summary>
        /// <returns>Items as as dictionary by their scene.</returns>
        Dictionary<Scene, Dictionary<SaveItemId, SaveItem>> GetRegisteredItemsByScene();

        /// <summary>
        /// Tries to get the <see cref="SaveItem"/> for the given <see cref="SaveItemId"/>.
        /// </summary>
        /// <param name="id">Id of the item.</param>
        /// <param name="saveItem">Item to get.</param>
        /// <returns>True if an item for the given id was registered.</returns>
        public bool TryGetItem(SaveItemId id, out SaveItem saveItem);

        /// <summary>
        /// Unregisters a item.
        /// </summary>
        /// <param name="item">The item to unregister.</param>
        void UnregisterItem(SaveItem item);
    }
}