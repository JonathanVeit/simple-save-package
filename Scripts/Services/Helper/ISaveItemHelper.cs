using SimpleSave.Models;

namespace SimpleSave.Services
{
    /// <summary>
    /// Helper class for <see cref="SaveItem"/>.
    /// </summary>
    internal interface ISaveItemHelper
    {
        /// <summary>
        /// Determines if the given item is inside the prefab stage and therefore currently edited as a prefab.
        /// </summary>
        /// <param name="item">Item to determine</param>
        /// <returns>True if the item is inside the prefab stage.</returns>
        bool IsInPrefabStage(SaveItem item);

        /// <summary>
        /// Determines the <see cref="SaveItemState"/> of the given item.
        /// </summary>
        /// <param name="item">Item to determine.</param>
        /// <returns>State of the item.</returns>
        SaveItemState GetItemState(SaveItem item);
    }
}