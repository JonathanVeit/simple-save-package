using System.Collections.Generic;
using SimpleSave.Extensions;
using UnityEngine;
using SimpleSave.Models;
using SimpleSave.Services;

namespace SimpleSave
{
    /// <summary>
    /// Identifies a <see cref="GameObject"/> in a scene to be saved and loaded.
    /// </summary>
    [AddComponentMenu("Simple Save/Save Item")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class SaveItem : MonoBehaviour
    {
        [SerializeField] private SaveItemId _id;
        [SerializeField] private SaveItemState _state;
        [SerializeField] private List<TagId> _tags 
            = new List<TagId>();
        [SerializeField] private GameObjectProperty _properties;
        [SerializeField] private string _prefabId;
        [SerializeField] private List<ComponentInfo> _components 
            = new List<ComponentInfo>();

        /// <summary>
        /// Unique identifier of the item.
        /// </summary>
        public SaveItemId Id => _id;

        /// <summary>
        /// State of the item.
        /// </summary>
        public SaveItemState State => _state;

        /// <summary>
        /// Properties to save.
        /// </summary>
        public GameObjectProperty Properties => _properties;

        /// <summary>
        /// Identifier of the prefab. Only required for items that are used as prefabs.
        /// </summary>
        public string PrefabId => _prefabId;

        private DoubleSideDictionary<ComponentId, Component> _savedComponentsDictionary;

        #region Services

        private static ILogger Logger => ServiceWrapper.GetService<ILogger>();

        private static ISaveItemController Controller => ServiceWrapper.GetService<ISaveItemController>();

        private static ISimpleSaveSettings Settings => ServiceWrapper.GetService<ISimpleSaveSettings>();

        private static ISaveItemHelper SaveItemHelper => ServiceWrapper.GetService<ISaveItemHelper>();

        #endregion

        #region Internal

        internal void SetId(SaveItemId id)
        {
            _id = id;
        }

        internal void SetState(SaveItemState state)
        {
            _state = state;
        }

        internal bool HasTag(TagId tagId)
        {
            return _tags.Contains(tagId);
        }

        internal void AddTag(TagId tagId)
        {
            _tags.Add(tagId);
        }

        internal void RemoveTag(TagId tagId)
        {
            _tags.Remove(tagId);
        }

        internal TagId[] GetTags()
        {
            return _tags.ToArray();
        }

        internal void SetProperties(GameObjectProperty properties)
        {
            _properties = properties;
        }

        internal void SetPrefabId(string prefabId)
        {
            _prefabId = prefabId;
        }

        internal void AddComponent(Component component)
        {
            var id = ServiceWrapper.GetService<IIdProvider>().GetNewComponentId();
            _components.Add(new ComponentInfo(id, component));
        }
        
        internal List<ComponentInfo> GetComponents()
        {
            return _components;
        }

        internal bool TryGetComponent(ComponentId id, out Component component)
        {
            if (_savedComponentsDictionary == null)
            {
                ConvertComponentsToDictionary();
            }

            return _savedComponentsDictionary.TryGetValue(id, out component);
        }

        internal bool TryGetComponentId(Component component, out ComponentId componentId)
        {
            if (_savedComponentsDictionary == null)
            {
                ConvertComponentsToDictionary();
            }

            return _savedComponentsDictionary.TryGetValue(component, out componentId);
        }

        #endregion

        #region Public

        /// <summary>
        /// Finds the <see cref="SaveItem"/> with the given id.
        /// </summary>
        /// <param name="id">Id of the item.</param>
        /// <returns>The <see cref="SaveItem"/> with the given id. If no item was found, it will return null.</returns>
        /// <remarks>Only items that are currently loaded in any scene can be found.</remarks>
        public static SaveItem Find(SaveItemId id)
        {
            return ServiceWrapper.GetService<ISaveItemController>().TryGetItem(id, out var saveItem) ? saveItem : null;
        }

        #endregion

        #region Initialization

        private void Awake()
        {
            if (CanRegister())
            {
                Controller.RegisterItem(this);
            }

            if (Application.isPlaying)
            {
                CleanupComponents();
            }
        }

        private bool CanRegister()
        {
            return !SaveItemHelper.IsInPrefabStage(this);
        }

        private void OnDestroy()
        {
            if (CanUnregister())
            {
                Controller.UnregisterItem(this);
            }
        }

        private bool CanUnregister()
        {
            if (SaveItemHelper.IsInPrefabStage(this))
            {
                return false;
            }

            return Application.isPlaying || 
                   !Application.isPlaying && gameObject.scene.isLoaded;
        }

        #endregion

        #region Helper

        private void ConvertComponentsToDictionary()
        {
            _savedComponentsDictionary = new DoubleSideDictionary<ComponentId, Component>();

            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i].Component == null)
                {
                    continue;
                }

                _savedComponentsDictionary.Add(_components[i].Id, _components[i].Component);
            }
        }

        internal void ValidateComponents()
        {
            var foundComponents = new HashSet<Component>();

            for (int i = 0; i < _components.Count; i++)
            {
                var componentInfo = _components[i];

                if (componentInfo.Component == null)
                {
                    continue;
                }

                if (foundComponents.Contains(componentInfo.Component))
                {
                    componentInfo.SetComponent(null);
                    Logger.LogInternal($"The component is already added.");
                    continue;
                }

                if (_components[i].Component is SaveItem)
                {
                    componentInfo.SetComponent(null);
                    Logger.LogInternal($"You cannot save the {nameof(SaveItem)} component.");
                    continue;
                }

                if (!Settings.AllowCrossComponentSaving &&
                    !_components[i].Component.transform.IsChildOf(transform))
                {
                    componentInfo.SetComponent(null);
                    Logger.LogInternal($"The component is not on a child of the {nameof(SaveItem)}.");
                    continue;
                }

                foundComponents.Add(componentInfo.Component);
            }
        }

        private void CleanupComponents()
        {
            for (int i = _components.Count - 1; i > 0; i--)
            {
                if (_components[i].Component == null)
                {
                    _components.RemoveAt(i);
                }
            }
        }

        #endregion
    }
}