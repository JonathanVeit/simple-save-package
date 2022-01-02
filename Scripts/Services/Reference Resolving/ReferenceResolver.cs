using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleSave.Extensions;
using SimpleSave.Models;
using SimpleSave.Models.Serializables;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="IReferenceResolver"/>
    internal sealed class ReferenceResolver : BaseService, IReferenceResolver
    {
        private static readonly Type ComponentType = typeof(Component);
        private static readonly Type GameObjectType = typeof(GameObject);
        private static readonly Type GenericListType = typeof(List<>);

        private const char EnumerationSeparator = ':';
        private const char ChildIndexSeparator = '.';

        #region Services

        private static ISaveItemController SaveItemController => ServiceWrapper.GetService<ISaveItemController>();

        #endregion

        #region Save

        /// <inheritdoc />
        public bool TryConvertReference(object referenceValue, ReferenceInfo referenceInfo,
            out SerializableReference serializableReference)
        {
            // Component
            if (IsTypeOrSubclass(referenceInfo.MemberType, ComponentType))
            {
                serializableReference = CreateComponentReference(referenceValue, referenceInfo);
                return true;
            }

            // Component list
            if (IsListOf(referenceInfo.MemberType, ComponentType))
            {
                serializableReference = CreateComponentListReference(referenceValue, referenceInfo);
                return true;
            }

            // Component array
            if (IsArrayOf(referenceInfo.MemberType, ComponentType))
            {
                serializableReference = CreateComponentArrayReference(referenceValue, referenceInfo);
                return true;
            }

            // GameObject
            if (IsTypeOrSubclass(referenceInfo.MemberType, GameObjectType))
            {
                serializableReference = CreateGameObjectReference(referenceValue, referenceInfo);
                return true;
            }

            // GameObject list 
            if (IsListOf(referenceInfo.MemberType, GameObjectType))
            {
                serializableReference = CreateGameObjectListReference(referenceValue, referenceInfo);
                return true;
            }

            // GameObject array 
            if (IsArrayOf(referenceInfo.MemberType, GameObjectType))
            {
                serializableReference = CreateGameObjectArrayReference(referenceValue, referenceInfo);
                return true;
            }

            Logger.LogInternal($"{nameof(ReferenceResolver)} has no implementation to save references of type \"{referenceInfo.MemberType.FullName}\".");
            serializableReference = default;
            return false;
        }

        #region Component

        private SerializableReference CreateComponentReference(object memberValue, 
            ReferenceInfo referenceInfo)
        {
            FindComponentReferenceValues(memberValue, referenceInfo, out var saveItemId, out var value);
            return new SerializableReference
            {
                Id = referenceInfo.Id,
                ItemId = saveItemId,
                Value = value,
            };
        }

        private SerializableReference CreateComponentListReference(object memberValue,
            ReferenceInfo referenceInfo)
        {
            IList asList = memberValue as IList;
            if (asList == null)
            {
                return new SerializableReference
                {
                    Id = referenceInfo.Id,
                    ItemId = null,
                    Value = null,
                };
            }

            StringBuilder itemIdBuilder = new StringBuilder();
            StringBuilder valueBuilder = new StringBuilder();

            for (int i = 0; i < asList.Count; i++)
            {
                var objectAtIndex = asList[i];
                FindComponentReferenceValues(objectAtIndex, referenceInfo, out var saveItemId,
                    out var value);

                itemIdBuilder.Append(saveItemId);
                valueBuilder.Append(value);

                if (i < asList.Count - 1)
                {
                    itemIdBuilder.Append(EnumerationSeparator);
                    valueBuilder.Append(EnumerationSeparator);
                }
            }

            return new SerializableReference
            {
                Id = referenceInfo.Id,
                ItemId = itemIdBuilder.ToString(),
                Value = valueBuilder.ToString(),
            };
        }

        private SerializableReference CreateComponentArrayReference(object memberValue,
            ReferenceInfo referenceInfo)
        {
            Component[] asArray = memberValue as Component[];
            if (asArray == null)
            {
                return new SerializableReference
                {
                    Id = referenceInfo.Id,
                    ItemId = null,
                    Value = null,
                };
            }

            StringBuilder itemIdBuilder = new StringBuilder();
            StringBuilder valueBuilder = new StringBuilder();

            for (int i = 0; i < asArray.Length; i++)
            {
                var objectAtIndex = asArray[i];
                FindComponentReferenceValues(objectAtIndex, referenceInfo, out var saveItemId,
                    out var value);

                itemIdBuilder.Append(saveItemId);
                valueBuilder.Append(value);

                if (i < asArray.Length - 1)
                {
                    itemIdBuilder.Append(EnumerationSeparator);
                    valueBuilder.Append(EnumerationSeparator);
                }
            }

            return new SerializableReference
            {
                Id = referenceInfo.Id,
                ItemId = itemIdBuilder.ToString(),
                Value = valueBuilder.ToString(),
            };
        }

        private void FindComponentReferenceValues(object memberValue, 
            ReferenceInfo referenceInfo, out SaveItemId saveItemId, out string componentId)
        {
            saveItemId = SaveItemId.Invalid;
            componentId = string.Empty;

            var referencedComponent = (Component)memberValue;
            if (referencedComponent == null)
            {
                return;
            }

            if (referencedComponent is SaveItem asSaveItem)
            {
                saveItemId = asSaveItem.Id;
                componentId = asSaveItem.Id;
            }

            var associatedSaveItem = SearchAssociatedSaveItem(referencedComponent.gameObject);
            if (associatedSaveItem == null)
            {
                Logger.LogInternal($"Unable to save reference to \"{referencedComponent}\" in member \"{referenceInfo.MemberName}\" on \"{referenceInfo.DeclaringType.Name}\". " +
                    $"The referenced Component is not being saved by any {nameof(SaveItem)}.");
                return;
            }

            var foundComponentId = SearchReferencedComponentId(associatedSaveItem, referencedComponent);
            if (foundComponentId == ComponentId.Invalid)
            {
                Logger.LogInternal($"Unable to save reference to \"{referencedComponent}\" in member \"{referenceInfo.MemberName}\" on \"{referenceInfo.DeclaringType.Name}\". " +
                    $"The referenced Component \"{referencedComponent}\" is not being saved by the {nameof(SaveItem)}.");
                return;
            }

            saveItemId = associatedSaveItem.Id;
            componentId = foundComponentId;
        }

        private ComponentId SearchReferencedComponentId(SaveItem targetItem, Component referencedComponent)
        {
            targetItem.TryGetComponentId(referencedComponent, out var referencedComponentId);
            return referencedComponentId;
        }

        #endregion

        #region GameObject

        private SerializableReference CreateGameObjectReference(object memberValue,
            ReferenceInfo referenceInfo)
        {
            FindGameObjectReferenceValues(memberValue, referenceInfo, out var saveItemId, out var value);
            return new SerializableReference
            {
                Id = referenceInfo.Id,
                ItemId = saveItemId,
                Value = value,
            };
        }

        private SerializableReference CreateGameObjectListReference(object memberValue,
            ReferenceInfo referenceInfo)
        {
            IList asList = memberValue as IList;
            if (asList == null)
            {
                return new SerializableReference
                {
                    Id = referenceInfo.Id,
                    ItemId = null,
                    Value = null,
                };
            }

            StringBuilder itemIdBuilder = new StringBuilder();
            StringBuilder valueBuilder = new StringBuilder();

            for (int i = 0; i < asList.Count; i++)
            {
                var objectAtIndex = asList[i];
                FindGameObjectReferenceValues(objectAtIndex, referenceInfo, out var saveItemId,
                    out var value);

                itemIdBuilder.Append(saveItemId);
                valueBuilder.Append(value);

                if (i < asList.Count - 1)
                {
                    itemIdBuilder.Append(EnumerationSeparator);
                    valueBuilder.Append(EnumerationSeparator);
                }
            }

            return new SerializableReference
            {
                Id = referenceInfo.Id,
                ItemId = itemIdBuilder.ToString(),
                Value = valueBuilder.ToString()
            };
        }

        private SerializableReference CreateGameObjectArrayReference(object memberValue,
            ReferenceInfo referenceInfo)
        {
            GameObject[] asArray = memberValue as GameObject[];
            if (asArray == null)
            {
                return new SerializableReference
                {
                    Id = referenceInfo.Id,
                    ItemId = null,
                    Value = null,
                };
            }

            StringBuilder itemIdBuilder = new StringBuilder();
            StringBuilder valueBuilder = new StringBuilder();

            for (int i = 0; i < asArray.Length; i++)
            {
                var objectAtIndex = asArray[i];
                FindGameObjectReferenceValues(objectAtIndex, referenceInfo, out var saveItemId,
                    out var value);

                itemIdBuilder.Append(saveItemId);
                valueBuilder.Append(value);

                if (i < asArray.Length - 1)
                {
                    itemIdBuilder.Append(EnumerationSeparator);
                    valueBuilder.Append(EnumerationSeparator);
                }
            }

            return new SerializableReference
            {
                Id = referenceInfo.Id,
                ItemId = itemIdBuilder.ToString(),
                Value = valueBuilder.ToString(),
            };
        }

        private void FindGameObjectReferenceValues(object memberValue,
            ReferenceInfo referenceInfo, out SaveItemId saveItemId, out string gameObjectName)
        {
            saveItemId = SaveItemId.Invalid;
            gameObjectName = string.Empty;

            var referencedGameObject = (GameObject)memberValue;
            if (referencedGameObject == null)
            {
                return;
            }

            var associatedSaveItem = SearchAssociatedSaveItem(referencedGameObject);
            if (associatedSaveItem == null)
            {
                Logger.LogInternal($"Unable to save reference to \"{referencedGameObject.name}\" in member \"{referenceInfo.MemberName}\" on \"{referenceInfo.DeclaringType.Name}\". " +
                    $"The referenced GameObject has no {nameof(SaveItem)} component and is not child of a GameObject with a {nameof(SaveItem)} component.");
                return;
            }

            saveItemId = associatedSaveItem.Id;
            gameObjectName = GetChildIndexString(associatedSaveItem.transform, referencedGameObject.transform);
        }

        private string GetChildIndexString(Transform parent, Transform child)
        {
            if (parent == child)
            {
                return "-1";
            }

            Transform curSubChild = child;
            Transform curSubParent = child.parent;

            var stringBilder = new StringBuilder();

            while (true)
            {
                stringBilder.Append(curSubChild.GetSiblingIndex());

                if (curSubParent == parent)
                {
                    break;
                }

                curSubChild = curSubParent;
                curSubParent = curSubChild.parent;
                stringBilder.Append(ChildIndexSeparator);
            }

            return stringBilder.ToString();
        }

        #endregion

        #endregion

        #region Resolve

        /// <inheritdoc />
        public bool TryResolveReference(SerializableReference serializableReference, ReferenceInfo referenceInfo,
            out object referencedObject)
        {
            if (string.IsNullOrEmpty(serializableReference.Value))
            {
                referencedObject = null;
                return true;
            }

            // Component
            if (IsTypeOrSubclass(referenceInfo.MemberType, ComponentType))
            {
                referencedObject = ResolveComponentReference(serializableReference, referenceInfo);
                return true;
            }

            // Component List
            if (IsListOf(referenceInfo.MemberType, ComponentType))
            {
                referencedObject = ResolveComponentListReference(serializableReference, referenceInfo);
                return true;
            }

            // Component Array
            if (IsArrayOf(referenceInfo.MemberType, ComponentType))
            {
                referencedObject = ResolveComponentArrayReference(serializableReference, referenceInfo);
                return true;
            }

            // GameObject
            if (referenceInfo.MemberType == GameObjectType)
            {
                referencedObject = ResolveGameObjectReference(serializableReference, referenceInfo);
                return true;
            }

            // GameObject List
            if (IsListOf(referenceInfo.MemberType, GameObjectType))
            {
                referencedObject = ResolveGameObjectListReference(serializableReference, referenceInfo);
                return true;
            }

            // GameObject Array
            if (IsArrayOf(referenceInfo.MemberType, GameObjectType))
            {
                referencedObject = ResolveGameObjectArrayReference(serializableReference, referenceInfo);
                return true;
            }

            Logger.LogInternal($"{nameof(ReferenceResolver)} has no implementation to resolve references of type \"{referenceInfo.MemberType.FullName}\".");
            referencedObject = null;
            return false;
        }

        #region Component

        private Component ResolveComponentReference(SerializableReference serializableReference,
            ReferenceInfo referenceInfo)
        {
            return SearchReferencedComponent(serializableReference.Value, serializableReference.ItemId, referenceInfo);
        }

        private IList ResolveComponentListReference(SerializableReference serializableReference,
            ReferenceInfo referenceInfo)
        {
            var itemIds = serializableReference.ItemId.Split(EnumerationSeparator);
            var values = serializableReference.Value.Split(EnumerationSeparator);

            var result = (IList)Activator.CreateInstance(referenceInfo.MemberType);
            for (int i = 0; i < itemIds.Length; i++)
            {
                result.Add(SearchReferencedComponent(values[i], itemIds[i], referenceInfo));
            }

            return result;
        }

        private Array ResolveComponentArrayReference(SerializableReference serializableReference,
            ReferenceInfo referenceInfo)
        {
            var itemIds = serializableReference.ItemId.Split(EnumerationSeparator);
            var values = serializableReference.Value.Split(EnumerationSeparator);

            var result = Array.CreateInstance(referenceInfo.MemberType.GetElementType(), itemIds.Length);
            for (int i = 0; i < itemIds.Length; i++)
            {
                result.SetValue(SearchReferencedComponent(values[i], itemIds[i], referenceInfo), i);
            }

            return result;
        }

        private Component SearchReferencedComponent(string referenceValue, string itemId, ReferenceInfo referenceInfo)
        {
            if (string.IsNullOrEmpty(referenceValue) ||
                string.IsNullOrEmpty(itemId))
            {
                return null;
            }

            if (SaveItemController.TryGetItem(referenceValue, out SaveItem referencedItem))
            {
                return referencedItem;
            }

            if (!SaveItemController.TryGetItem(itemId, out SaveItem associatedItem))
            {
                Logger.LogInternal($"Referenced {nameof(SaveItem)} with Id \"{itemId}\" of member \"{referenceInfo.MemberName}\" " +
                    $"on component \"{referenceInfo.DeclaringType.Name}\" could not be found. The reference cannot be resolved.");

                return null;
            }

            if (!associatedItem.TryGetComponent(referenceValue, out var referencedComponent))
            {
                Logger.LogInternal($"Unable to resolve reference of member \"{referenceInfo.MemberName}\" on component \"{referenceInfo.DeclaringType.Name}\". " +
                    $"The referenced Component with Id \"{referenceValue}\" could not be found.");
            }
           
            return referencedComponent;
        }

        #endregion

        #region GameObject

        private GameObject ResolveGameObjectReference(SerializableReference serializableReference,
            ReferenceInfo referenceInfo)
        {
            return SearchReferencedGameObject(serializableReference.Value, serializableReference.ItemId, referenceInfo);
        }

        private List<GameObject> ResolveGameObjectListReference(SerializableReference serializableReference,
            ReferenceInfo referenceInfo)
        {
            var itemIds = serializableReference.ItemId.Split(EnumerationSeparator);
            var values = serializableReference.Value.Split(EnumerationSeparator);

            var result = new List<GameObject>();
            for (int i = 0; i < itemIds.Length; i++)
            {
                result.Add(SearchReferencedGameObject(values[i], itemIds[i], referenceInfo));
            }

            return result;
        }

        private GameObject[] ResolveGameObjectArrayReference(SerializableReference serializableReference,
            ReferenceInfo referenceInfo)
        {
            var itemIds = serializableReference.ItemId.Split(EnumerationSeparator);
            var values = serializableReference.Value.Split(EnumerationSeparator);

            var result = new GameObject[itemIds.Length];
            for (int i = 0; i < itemIds.Length; i++)
            {
                result[i] = SearchReferencedGameObject(values[i], itemIds[i], referenceInfo);
            }

            return result;
        }

        GameObject SearchReferencedGameObject(string gameObjectName, string itemId, ReferenceInfo referenceInfo)
        {
            if (string.IsNullOrEmpty(gameObjectName) ||
                string.IsNullOrEmpty(itemId))
            {
                return null;
            }

            if (!SaveItemController.TryGetItem(itemId, out SaveItem associatedItem))
            {
                Logger.LogInternal($"Referenced {nameof(SaveItem)} with Id \"{itemId}\" of member \"{referenceInfo.MemberName}\" " +
                    $"on component \"{referenceInfo.DeclaringType.Name}\" could not be found. The reference cannot be resolved.");

                return null;
            }

            var gameObject = SearchGameObjectByIndices(associatedItem.transform, gameObjectName);
            if (gameObject == null)
            {
                Logger.LogInternal($"Unable to resolve reference of member \"{referenceInfo.MemberName}\" on component \"{referenceInfo.DeclaringType.Name}\". " +
                    $"The referenced GameObject \"{gameObjectName}\" could not be found.");
            }

            return gameObject;
        }

        private GameObject SearchGameObjectByIndices(Transform parent, string indexString)
        {
            if (indexString == "-1")
            {
                return parent.gameObject;
            }

            var indices = indexString.Split(ChildIndexSeparator);

            Transform curChild = null;
            Transform curParent = parent;
            for (int i = indices.Length - 1; i >= 0; i--)
            {
                curChild = curParent.GetChild(int.Parse(indices[i]));
                curParent = curChild;
            }

            return curChild.gameObject;
        }

        #endregion

        #endregion

        #region Helper

        private static bool IsTypeOrSubclass(Type typeToCheck, Type parent)
        {
            return typeToCheck == parent ||
                   typeToCheck.IsSubclassOf(parent);
        }

        private static bool IsListOf(Type typeToCheck, Type genericType)
        {
            if (typeToCheck.IsArray ||
                !typeToCheck.IsGenericType ||
                typeToCheck.GetGenericTypeDefinition() != GenericListType)
            {
                return false;
            }

            var genericArgument = typeToCheck.GetGenericArguments()[0];
            return genericArgument == genericType ||
                   genericArgument.IsSubclassOf(genericType);
        }

        private static bool IsArrayOf(Type typeToCheck, Type genericType)
        {
            if (!typeToCheck.IsArray)
            {
                return false;
            }

            var elementType = typeToCheck.GetElementType();

            return elementType == genericType ||
                   elementType.IsSubclassOf(genericType);
        }

        private static SaveItem SearchAssociatedSaveItem(GameObject referencedComponentGameObject)
        {
            var targetItem = referencedComponentGameObject.GetComponentInParent<SaveItem>();
            return targetItem;
        }

        #endregion
    }
}