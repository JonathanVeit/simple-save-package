using System.Collections.Generic;
using SimpleSave.Models;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveGameCreator"/>
    internal sealed class TagManager : BaseService, ITagManager
    {
        private TagContainer TagContainer => TagContainer.Instance;

        private readonly Dictionary<TagId, TagInfo> _tagCacheById;
        private readonly Dictionary<string, TagInfo> _tagCacheByName;
        private readonly HashSet<uint> _tagValues;

        public int MaxTagCount => 32;

        public TagManager()
        {
            _tagCacheById = new Dictionary<TagId, TagInfo>();
            _tagCacheByName = new Dictionary<string, TagInfo>();
            _tagValues = new HashSet<uint>();

            LoadCache();
        }

        #region Services

        private IIdProvider IdProvider => ServiceWrapper.GetService<IIdProvider>();

        #endregion

        /// <inheritdoc />
        public bool TagExists(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                return false;
            }

            return _tagCacheByName.ContainsKey(tagName);
        }

        /// <inheritdoc />
        public bool TagExists(TagId tagId)
        {
            return _tagCacheById.ContainsKey(tagId);
        }

        /// <inheritdoc />
        public bool TryGetTagInfo(TagId tagId, out TagInfo tagInfo)
        { 
            return _tagCacheById.TryGetValue(tagId, out tagInfo);
        }

        /// <inheritdoc />
        public bool TryGetTagInfo(string tagName, out TagInfo tagInfo)
        {
            return _tagCacheByName.TryGetValue(tagName, out tagInfo);
        }

        /// <inheritdoc />
        public TagInfo[] GetAllTagInfos()
        {
            var result = new TagInfo[_tagCacheById.Count];

            int index = 0;
            foreach (var tagInfo in _tagCacheById.Values)
            {
                result[index] = tagInfo;
                index++;
            }

            return result;
        }

        /// <inheritdoc />
        public void UpdateTagName(TagId tagId, string newName)
        {
            var prevName = _tagCacheById[tagId].Name;

            var newTagInfo = _tagCacheById[tagId];
            newTagInfo.Name = newName;

            _tagCacheById[tagId] = newTagInfo;
            _tagCacheByName[prevName] = newTagInfo;

            SaveTagsToContainer();
        }

        /// <inheritdoc />
        public void UpdateTagDescription(TagId tagId, string newDescription)
        {
            var prevName = _tagCacheById[tagId].Name;

            var newTagInfo = _tagCacheById[tagId];
            newTagInfo.Description = newDescription;

            _tagCacheById[tagId] = newTagInfo;
            _tagCacheByName[prevName] = newTagInfo;

            SaveTagsToContainer();
        }

        /// <inheritdoc />
        public bool TryCreateTag(string tagName, string description, out TagInfo createdTag)
        {
            if (TagExists(tagName))
            {
                Logger.Log(LogType.Log, $"Trying to ");
                createdTag = default;
                return false;
            }

            var tagInfo = new TagInfo
            {
                Id = IdProvider.GetNewTagId(),
                Name = tagName,
                Value = GetFreeTagValue(),
                Description = description,
            };

            _tagCacheById.Add(tagInfo.Id, tagInfo);
            _tagCacheByName.Add(tagInfo.Name, tagInfo);
            _tagValues.Add(tagInfo.Value);

            SaveTagsToContainer();
            createdTag = tagInfo;
            return true;
        }

        /// <inheritdoc />
        public void DeleteTag(TagId tagId)
        {
            var tagInfo = _tagCacheById[tagId];

            _tagCacheById.Remove(tagId);
            _tagCacheByName.Remove(tagInfo.Name);
            _tagValues.Remove(tagInfo.Value);
            
            SaveTagsToContainer();
        }

        /// <inheritdoc />
        public bool DoAnyTagsMatch(TagId[] tagIds, TagCollection tagCollection)
        {
            // only save tagless
            if (tagCollection == TagCollection.TaglessOnly)
            {
                return tagIds.Length == 0;
            }

            // no tags defined = save all
            if (tagCollection == null ||
                tagCollection.Tags == null ||
                tagCollection.Tags.Length == 0)
            {
                return true;
            }

            // target has 0 tags but tagless are included
            if (tagIds.Length == 0 &&
                tagCollection.IncludeTagless)
            {
                return true;
            }

            var tagMask = ConvertToBitMask(tagIds);
            var collectionMask = ConvertToBitMask(tagCollection);

            return (tagMask & collectionMask) != 0;
        }

        /// <inheritdoc />
        public bool DoAnyTagsMatch(string[] tagNames, TagCollection tagCollection)
        {
            // only save tagless
            if (tagCollection == TagCollection.TaglessOnly)
            {
                return tagNames.Length == 0;
            }

            // no tags defined = save all
            if (tagCollection == null ||
                tagCollection.Tags == null ||
                tagCollection.Tags.Length == 0)
            {
                return true;
            }

            // target has 0 tags but tagless are included
            if (tagNames.Length == 0 &&
                tagCollection.IncludeTagless)
            {
                return true;
            }

            var tagMask = ConvertToBitMask(tagNames);
            var collectionMask = ConvertToBitMask(tagCollection);

            return (tagMask & collectionMask) != 0;
        }

        private uint ConvertToBitMask(IEnumerable<TagId> tagIds)
        {
            uint bitMask = 0;
            foreach (string tagId in tagIds)
            {
                if (!_tagCacheById.ContainsKey(tagId))
                {
                    continue;
                }

                var tagValue = _tagCacheById[tagId].Value;
                
                bitMask |= tagValue;
            }

            return bitMask; 
        }

        private uint ConvertToBitMask(TagCollection tagCollection)
        {
            return ConvertToBitMask(tagCollection.Tags);
        }
        private uint ConvertToBitMask(IEnumerable<string> tagNames)
        {
            uint bitMask = 0;
            foreach (string tagName in tagNames)
            {
                if (!_tagCacheByName.ContainsKey(tagName))
                {
                    continue;
                }

                var tagValue = _tagCacheByName[tagName].Value;

                bitMask |= tagValue;
            }

            return bitMask;
        }

        #region Helper

        private void LoadCache()
        {
            var tags = TagContainer.GetTags();
            for (var i = 0; i < tags.Count; i++)
            {
                var tagInfo = tags[i];

                _tagCacheById.Add(tagInfo.Id, tagInfo);
                _tagCacheByName.Add(tagInfo.Name, tagInfo);
                _tagValues.Add(tagInfo.Value);
            }
        }

        private uint GetFreeTagValue()
        {
            for (int i = 1; i < int.MaxValue; i++)
            {
                var value = (uint)1 << i;
                if (_tagValues.Contains(value))
                {
                    continue;
                }

                return value;
            }

            return 0;
        }

        private void SaveTagsToContainer()
        {
            TagContainer.OverrideTags(_tagCacheById.Values);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(TagContainer);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        #endregion
    }
}