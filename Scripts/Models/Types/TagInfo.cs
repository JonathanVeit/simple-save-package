using System;

namespace SimpleSave.Models
{
    /// <summary>
    /// Information about a tag.
    /// </summary>
    [Serializable]
    internal struct TagInfo
    {
        /// <summary>
        /// Unique identifier of the tag.
        /// </summary>
        public TagId Id;

        /// <summary>
        /// Unique name of the tag.
        /// </summary>
        public string Name;

        /// <summary>
        /// Description of the tag.
        /// </summary>
        public string Description;

        /// <summary>
        /// Bitwise shift value of the tag.
        /// </summary>
        public uint Flag;
    }
}