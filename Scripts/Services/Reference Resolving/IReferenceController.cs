using SimpleSave.Models.Serializables;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <summary>
    /// Controller to get and load references from <see cref="Component"/> instances.
    /// </summary>
    internal interface IReferenceController
    {
        /// <summary>
        /// Gets all serializable references of the given component.
        /// </summary>
        /// <param name="component">Component to get the references from.</param>
        /// <param name="savingItem">Item which saves the component.</param>
        /// <param name="referenceResolver">Resolver to use.</param>
        /// <returns>All serializable references.</returns>
        SerializableReference[] GetReferences(Component component, SaveItem savingItem,
            IReferenceResolver referenceResolver);

        /// <summary>
        /// Loads all references to the given component.
        /// </summary>
        /// <param name="serializableReferences">References to load.</param>
        /// <param name="component">Component to load references to.</param>
        /// <param name="referenceResolver">Resolver to use.</param>
        void LoadReferences(SerializableReference[] serializableReferences, Component component,
            IReferenceResolver referenceResolver);
    }
}