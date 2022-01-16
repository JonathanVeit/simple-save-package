using System;
using System.Collections.Generic;
using System.Linq;
using SimpleSave.Models;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <summary>
    /// Container to store <see cref="ReferenceInfo"/>.
    /// </summary>
    internal sealed class ReferenceInfoContainer : BaseContainer<ReferenceInfoContainer>
    {
        [SerializeField] private List<SerializableReferenceInfo> references;

        private Dictionary<Type, Dictionary<int, ReferenceInfo>> _loadedReferenceInfos;

        /// <summary>
        /// How many member are stored in the container.
        /// </summary>
        public int Count => references?.Count ?? 0;

        #region Access

        /// <summary>
        /// Adds a new <see cref="ReferenceInfo"/> to the container.
        /// </summary>
        /// <param name="referenceInfo">ReferenceInfo to add.</param>
        public void Add(ReferenceInfo referenceInfo)
        {
            references ??= new List<SerializableReferenceInfo>();

            for (int i = 0; i < references.Count; i++)
            {
                if (references[i].OwningType == referenceInfo.OwningType.AssemblyQualifiedName)
                {
                    references[i].Members.Add(new SerializableReferenceMember
                    {
                        Id = referenceInfo.Id,
                        MemberCategory = referenceInfo.MemberCategory,
                        DeclaringType = referenceInfo.DeclaringType.AssemblyQualifiedName,
                        MemberName = referenceInfo.MemberName,
                        MemberType = referenceInfo.MemberType.AssemblyQualifiedName
                    });
                    return;
                }
            }

            references.Add(new SerializableReferenceInfo
            {
                OwningType = referenceInfo.OwningType.AssemblyQualifiedName,
                Members = new List<SerializableReferenceMember>
                {
                    new SerializableReferenceMember
                    {
                        Id = referenceInfo.Id,
                        MemberCategory = referenceInfo.MemberCategory,
                        DeclaringType = referenceInfo.DeclaringType.AssemblyQualifiedName,
                        MemberName = referenceInfo.MemberName,
                        MemberType = referenceInfo.MemberType.AssemblyQualifiedName
                    }
                }
            });
        }

        public bool TryGet(Type declaring, out ReferenceInfo[] referenceInfos)
        {
            if (_loadedReferenceInfos is null)
            {
                LoadReferenceInfos();
            }

            if (!_loadedReferenceInfos.TryGetValue(declaring, out var entry))
            {
                referenceInfos = Array.Empty<ReferenceInfo>();
                return false;
            }

            referenceInfos = entry.Values.ToArray();
            return true;
        }

        public bool TryGet(ReferenceId id, out ReferenceInfo referenceInfo)
        {
            if (_loadedReferenceInfos is null)
            {
                LoadReferenceInfos();
            }

            foreach (var entry in _loadedReferenceInfos)
            {
                if (entry.Value.TryGetValue(id, out referenceInfo))
                {
                    return true;
                }
            }

            referenceInfo = default;
            return false;
        }

        /// <summary>
        /// Determines if the given type has any fields or properties containing the <see cref="SaveRef"/> attribute.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if the type has fields or properties with the attribute.</returns>
        public bool TypeHasReferences(Type type)
        {
            if (_loadedReferenceInfos is null)
            {
                LoadReferenceInfos();
            }

            return _loadedReferenceInfos.ContainsKey(type);
        }

        private void LoadReferenceInfos()
        {
            _loadedReferenceInfos = new Dictionary<Type, Dictionary<int, ReferenceInfo>>();

            for (int i = 0; i < references.Count; i++)
            {
                var owningType = Type.GetType(references[i].OwningType);
                if (owningType is null)
                {
                    continue;
                }

                if (!_loadedReferenceInfos.ContainsKey(owningType))
                {
                    _loadedReferenceInfos.Add(owningType, new Dictionary<int, ReferenceInfo>());
                }

                for (int j = 0; j < references[i].Members.Count; j++)
                {
                    var serializableMember = references[i].Members[j];
                    if (_loadedReferenceInfos[owningType].ContainsKey(serializableMember.Id))
                    {
                        continue;
                    }

                    _loadedReferenceInfos[owningType].Add(serializableMember.Id, new ReferenceInfo()
                    {
                        Id = serializableMember.Id,
                        MemberCategory = serializableMember.MemberCategory,
                        MemberName = serializableMember.MemberName,
                        MemberType = Type.GetType(serializableMember.MemberType),
                        DeclaringType = Type.GetType(serializableMember.DeclaringType),
                    });
                }
            }
        }

        #endregion

        /// <summary>
        /// Deletes all stored <see cref="SaveVarInfo"/>.
        /// </summary>
        public void Clear()
        {
            references?.Clear();
            _loadedReferenceInfos?.Clear();
        }

        [Serializable]
        private struct SerializableReferenceInfo
        {
            public string OwningType;
            public List<SerializableReferenceMember> Members;
        }

        [Serializable]
        private struct SerializableReferenceMember
        {
            public ReferenceId Id;
            public string DeclaringType;
            public MemberCategory MemberCategory;
            public string MemberName;
            public string MemberType;
        }
    }
}