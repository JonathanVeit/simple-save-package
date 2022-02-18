using SimpleSave.Models;

namespace SimpleSave.Services
{
    /// <summary>
    /// Manages, created and deletes tags.
    /// </summary>
    internal interface ITagManager
    {
        /// <summary>
        /// The maximum amount of tags.
        /// </summary>
        public int MaxTagCount { get; }

        /// <summary>
        /// Determines whether a tag with the given name exists.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>True if a tag with the given name exists.</returns>
        bool TagExists(string tagName);

        /// <summary>
        /// Determines whether a tag with the given id exists.
        /// </summary>
        /// <param name="tagId">Id of the tag.</param>
        /// <returns>True if a tag with the id tag exists.</returns>
        bool TagExists(TagId tagId);

        /// <summary>
        /// Gets the <see cref="TagInfo"/> associated with the specified key.
        /// </summary>
        /// <param name="tagId">Id of the tag to get.</param>
        /// <param name="tagInfo">Tag to get.</param>
        /// <returns>True if a tag with the id exists.</returns>
        bool TryGetTagInfo(TagId tagId, out TagInfo tagInfo);

        /// <summary>
        /// Gets the <see cref="TagInfo"/> associated with the specified name.
        /// </summary>
        /// <param name="tagName">Name of the tag to get.</param>
        /// <param name="tagInfo">Tag to get.</param>
        /// <returns>True if a tag with the name exists.</returns>
        bool TryGetTagInfo(string tagName, out TagInfo tagInfo);

        /// <summary>
        /// Gets all existing <see cref="TagInfo"/>.
        /// </summary>
        /// <returns>All existing <see cref="TagInfo"/>.</returns>
        TagInfo[] GetAllTagInfos();

        /// <summary>
        /// Updates the name of the tag associated with the specified id.
        /// </summary>
        /// <param name="tagId">Id of the tag.</param>
        /// <param name="newName">New name of the tag.</param>
        void UpdateTagName(TagId tagId, string newName);

        /// <summary>
        /// Updates the description of the tag associated with the specified id.
        /// </summary>
        /// <param name="tagId">Id of the tag.</param>
        /// <param name="newDescription">New description of the tag.</param>
        void UpdateTagDescription(TagId tagId, string newDescription);

        /// <summary>
        /// Tries to create a new tag with the given name and description.
        /// </summary>
        /// <param name="tagName">Name of the new tag.</param>
        /// <param name="description">Description of the new tag.</param>
        /// <param name="createdTag">The <see cref="TagInfo"/> of the new tag.</param>
        /// <returns>True if the tag was created successful.</returns>
        bool TryCreateTag(string tagName, string description, out TagInfo createdTag);

        /// <summary>
        /// Deletes the tag associated with the specified id.
        /// </summary>
        /// <param name="tagId">Id of the tag.</param>
        void DeleteTag(TagId tagId);

        /// <summary>
        /// Determines whether any tag associated with the given ids is included in the given collection of tag names.
        /// </summary>
        /// <param name="itemTags">Id to check.</param>
        /// <param name="tagCollection">Collection of tags to check against.</param>
        bool DoAnyTagsMatch(TagId[] itemTags, TagCollection tagCollection);

        /// <summary>
        /// Determines whether any given tag name is included in the given collection of tag names.
        /// </summary>
        /// <param name="tagNames">Id to check.</param>
        /// <param name="tagCollection">Collection of tags to check against.</param>
        bool DoAnyTagsMatch(string[] tagNames, TagCollection tagCollection);
    }
}