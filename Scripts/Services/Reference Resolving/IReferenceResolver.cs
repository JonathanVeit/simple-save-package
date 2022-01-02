using SimpleSave.Models;
using SimpleSave.Models.Serializables;

namespace SimpleSave.Services
{
    /// <summary>
    /// Takes care of getting and resolving references.
    /// </summary>
    /// <remarks>
    /// You can create your own implementations of this interface.<br/>
    /// If you want to use your own implementation, you need to add it to the <see cref="SaveGameSettings"/> when creating or loading save games from <see cref="Simple"/>.
    /// </remarks>
    public interface IReferenceResolver
    {
        /// <summary>
        /// Tries to convert the given reference into a <see cref="SerializableReference"/>.
        /// </summary>
        /// <param name="referenceValue">Reference to save.</param>
        /// <param name="referenceInfo">Information about the reference.</param>
        /// <param name="serializableReference">Serializable reference.</param>
        /// <returns>True if the reference could be converted.</returns>
        bool TryConvertReference(object referenceValue, ReferenceInfo referenceInfo,
            out SerializableReference serializableReference);

        /// <summary>
        /// Tries to resolve the given serializable reference.
        /// </summary>
        /// <param name="serializableReference">Serializable reference to resolve.</param>
        /// <param name="referenceInfo">Information about the reference.</param>
        /// <param name="referencedObject">The references object.</param>
        /// <returns>True if the reference could be resolved.</returns>
        bool TryResolveReference(SerializableReference serializableReference, ReferenceInfo referenceInfo,
            out object referencedObject);
    }
}