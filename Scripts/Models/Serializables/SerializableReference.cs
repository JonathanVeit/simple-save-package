using System;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable reference of a member with the <see cref="SaveRef"/> attribute.
    /// </summary>
    [Serializable]
    public struct SerializableReference
    {
        /// <summary>
        /// Unique id of the associated <see cref="ReferenceInfo"/>.
        /// </summary>
        public int Id;

        /// <summary>
        /// Id of the target <see cref="SaveItem"/> associated with the referenced object. 
        /// </summary>
        public string ItemId;

        /// <summary>
        /// Value of the serialized reference.
        /// </summary>
        public string Value;
    }
}