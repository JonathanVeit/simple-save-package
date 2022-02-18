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
    internal sealed class SaveVarInfoContainer : BaseContainer<SaveVarInfoContainer>
    {
        [SerializeField] private List<SerializableSaveVarInfo> saveVars;

        private Dictionary<int, SaveVarInfo> _saveVarInfos;

        /// <summary>
        /// How many member are stored inside the container.
        /// </summary>
        public int Count => saveVars?.Count ?? 0;

        #region Access

        /// <summary>
        /// Adds a new <see cref="SaveVarInfo"/> to the container.
        /// </summary>
        /// <param name="saveVarInfo">SaveVarInfo to add.</param>
        public void Add(SaveVarInfo saveVarInfo)
        {
            saveVars ??= new List<SerializableSaveVarInfo>();

            saveVars.Add(new SerializableSaveVarInfo
            {
                Id = saveVarInfo.Id,
                DeclaringType = saveVarInfo.DeclaringType.AssemblyQualifiedName,
                MemberType = saveVarInfo.MemberType.AssemblyQualifiedName,
                MemberCategory = saveVarInfo.MemberCategory,
                MemberName = saveVarInfo.MemberName,
                Tags = saveVarInfo.Tags
            });
        }

        /// <summary>
        /// Returns all stored <see cref="SaveVarInfo"/>.
        /// </summary>
        /// <returns>All registered SaveVarInfos.</returns>
        public SaveVarInfo[] GetAll()
        {
            if (_saveVarInfos is null)
            {
                LoadSaveVarInfos();
            }

            return _saveVarInfos.Values.ToArray();
        }

        /// <summary>
        /// Tries to get the registered <see cref="SaveVarInfo"/> for id <see cref="SaveVarId"/>.
        /// </summary>
        /// <param name="id">Id of the SaveVarInfo.</param>
        /// <param name="saveVarInfo">The stored SaveVarInfo.</param>
        /// <returns>True of there was a SaveVarInfo stored of the given id.</returns>
        public bool TryGet(SaveVarId id, out SaveVarInfo saveVarInfo)
        {
            if (_saveVarInfos is null)
            {
                LoadSaveVarInfos();
            }

            return _saveVarInfos.TryGetValue(id, out saveVarInfo);
        }

        private void LoadSaveVarInfos()
        {
            _saveVarInfos ??= new Dictionary<int, SaveVarInfo>();

            for (int i = 0; i < saveVars.Count; i++)
            {
                if (_saveVarInfos.ContainsKey(saveVars[i].Id))
                {
                    continue;
                }

                _saveVarInfos.Add(saveVars[i].Id, new SaveVarInfo
                {
                    Id = saveVars[i].Id,
                    DeclaringType = Type.GetType(saveVars[i].DeclaringType),
                    MemberType = Type.GetType(saveVars[i].MemberType),
                    MemberCategory = saveVars[i].MemberCategory,
                    MemberName = saveVars[i].MemberName,
                    Tags = saveVars[i].Tags
                });
            }
        }

        #endregion

        /// <summary>
        /// Deletes all stored <see cref="SaveVarInfo"/>.
        /// </summary>
        public void Clear()
        {
            saveVars?.Clear();
            _saveVarInfos?.Clear();
        }

        [Serializable]
        private struct SerializableSaveVarInfo
        {
            public SaveVarId Id;
            public string DeclaringType;
            public MemberCategory MemberCategory;
            public string MemberName;
            public string MemberType;
            [HideInInspector] public string[] Tags;
        }
    }
}