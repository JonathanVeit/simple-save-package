using System;
using System.Collections.Generic;
using System.Linq;
using SimpleSave.Models;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <summary>
    /// Container to store <see cref="SaveVarInfo"/>.
    /// </summary>
    internal sealed class TagContainer : BaseContainer<TagContainer>
    {
        [SerializeField] private TagInfo[] _tags;

        public void OverrideTags(IEnumerable<TagInfo> tags)
        {
            _tags = tags.ToArray();
        }

        public List<TagInfo> GetTags()
        {
            _tags ??= Array.Empty<TagInfo>();
            return new List<TagInfo>(_tags);
        }
    }
}