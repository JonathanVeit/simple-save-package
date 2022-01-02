using System;
using System.Collections.Generic;
using SimpleSave.Extensions;
using SimpleSave.Models;
using SimpleSave.Models.Serializables;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveItemConverter"/>
    internal class SaveItemConverter : BaseService, ISaveItemConverter
    {
        private const string DestroyedComponentMarker = "destroyed";

        #region Services

        private static IReferenceController ReferenceController => ServiceWrapper.GetService<IReferenceController>();

        #endregion

        #region To Serializable

        /// <inheritdoc/>
        public SerializableSaveItem ToSerializable(SaveItem saveItem, SaveGameSettings saveGameSettings)
        {
            var sItem = new SerializableSaveItem
            {
                Id = saveItem.Id,
                State = saveItem.State,
                Tags = saveItem.GetTags(),
                PrefabId = saveItem.PrefabId,
                Properties = ConvertProperties(saveItem),
                Components = ConvertComponents(saveItem.GetComponents(), saveItem, saveGameSettings)
            };

            return sItem;
        }

        private SerializableGameObjectProperty[] ConvertProperties(SaveItem saveItem)
        {
            var flaggedProperties = GetFlaggedProperties(saveItem.Properties);

            var sItemProperties = new SerializableGameObjectProperty[flaggedProperties.Length];
            for (int i = 0; i < flaggedProperties.Length; i++)
            {
                sItemProperties[i] = new SerializableGameObjectProperty
                {
                    Property = flaggedProperties[i],
                    Value = GetSerializedPropertyValue(saveItem, flaggedProperties[i]),
                };
            }

            return sItemProperties;
        }

        private GameObjectProperty[] GetFlaggedProperties(GameObjectProperty property)
        {
            var values = Enum.GetValues(property.GetType());

            int amount = CalculateAmountOfFlaggedProperties(property, values);
            var result = new GameObjectProperty[amount];

            int index = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (property.HasFlag((Enum)values.GetValue(i)))
                {
                    result[index] = (GameObjectProperty)values.GetValue(i);
                    index++;
                }
            }

            return result;
        }

        private int CalculateAmountOfFlaggedProperties(GameObjectProperty property, Array values)
        {
            int amount = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (property.HasFlag((Enum)values.GetValue(i)))
                {
                    amount++;
                }
            }

            return amount;
        }

        private string GetSerializedPropertyValue(SaveItem saveItem, GameObjectProperty property)
        {
            switch (property)
            {
                case GameObjectProperty.ActiveSelf:
                    return saveItem.gameObject.activeSelf.ToString();
                case GameObjectProperty.Layer:
                    return saveItem.gameObject.layer.ToString();
                case GameObjectProperty.Tag:
                    return saveItem.gameObject.tag;
                case GameObjectProperty.Parent:
                    return GetParentSaveItemId(saveItem);

                default:
                    throw new ArgumentOutOfRangeException(nameof(property), property, null);
            }
        }

        private string GetParentSaveItemId(SaveItem saveItem)
        {
            if (saveItem.transform.parent == null)
            {
                return null;
            }

            var itemInParent = saveItem.transform.parent.GetComponent<SaveItem>();
            if (itemInParent == null)
            {
                Logger.LogInternal(
                    $"Unable to save the parent of {nameof(SaveItem)} \"{saveItem.Id}\". The parent is missing a {nameof(SaveItem)} component.");
                return string.Empty;
            }

            return itemInParent.Id;
        }

        private SerializableComponent[] ConvertComponents(List<ComponentInfo> componentInfos, SaveItem saveItem,
            SaveGameSettings saveGameSettings)
        {
            var sComponents = new SerializableComponent[componentInfos.Count];

            for (int i = 0; i < componentInfos.Count; i++)
            {
                var component = componentInfos[i].Component;

                if (component == null)
                {
                    sComponents[i] = new SerializableComponent
                    {
                        Id = componentInfos[i].Id,
                        Value = DestroyedComponentMarker,
                        References = Array.Empty<SerializableReference>()
                    };
                    continue;
                }

                if (component is ISimpleSaveCallbacks asISimpleSaveItemCallbacks)
                {
                    asISimpleSaveItemCallbacks.OnBeforeSaved();
                }

                if (component is ISimpleSaveCustomSerialization asISimpleSaveCustomSerialization)
                {
                    sComponents[i] = new SerializableComponent
                    {
                        Id = componentInfos[i].Id,
                        Value = asISimpleSaveCustomSerialization.Serialize(),
                        Enabled = component is MonoBehaviour { enabled: true },
                        References = ReferenceController.GetReferences(component, saveItem,
                            saveGameSettings.ReferenceResolver)
                    };
                    continue;
                }

                sComponents[i] = new SerializableComponent
                {
                    Id = componentInfos[i].Id,
                    Value = saveGameSettings.ComponentSerializer.Serialize(componentInfos[i].Component),
                    Enabled = component is MonoBehaviour { enabled: true },
                    References = ReferenceController.GetReferences(componentInfos[i].Component, saveItem,
                        saveGameSettings.ReferenceResolver)
                };
            }

            return sComponents;
        }

        #endregion

        #region To Item

        /// <inheritdoc/>
        public void ToItem(SerializableSaveItem serializableSaveItem, SaveItem saveItem,
            SaveGameSettings saveGameSettings)
        {
            LoadGameObjectProperties(serializableSaveItem.Properties, saveItem);
            LoadComponents(serializableSaveItem.Components, saveItem, saveGameSettings);
        }

        private static void LoadGameObjectProperties(
            SerializableGameObjectProperty[] gameObjectProperties, SaveItem saveItem)
        {
            for (int i = 0; i < gameObjectProperties.Length; i++)
            {
                switch (gameObjectProperties[i].Property)
                {
                    case GameObjectProperty.ActiveSelf:
                        saveItem.gameObject.SetActive(bool.Parse(gameObjectProperties[i].Value));
                        break;
                    case GameObjectProperty.Layer:
                        saveItem.gameObject.layer = int.Parse(gameObjectProperties[i].Value);
                        break;
                    case GameObjectProperty.Tag:
                        saveItem.gameObject.tag = gameObjectProperties[i].Value;
                        break;
                    case GameObjectProperty.Parent:
                        TryRecreateParent(saveItem, gameObjectProperties[i]);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(
                            $"{nameof(SaveItemConverter)} has no implementation to load GameObjectProperty of type {gameObjectProperties[i].Property}.");
                }
            }
        }

        private static void TryRecreateParent(SaveItem saveItem, SerializableGameObjectProperty gameObjectProperty)
        {
            if (string.IsNullOrEmpty(gameObjectProperty.Value))
            {
                return;
            }

            var parentItem = SaveItem.Find(gameObjectProperty.Value);
            if (parentItem == null)
            {
                Logger.LogInternal(
                    $"Unable to load the parent of {nameof(SaveItem)} \"{saveItem.Id}\". The parent {nameof(SaveItem)} \"{gameObjectProperty.Value}\" could not be found.");
                return;
            }
            saveItem.transform.SetParent(parentItem.transform);
        }

        private static void LoadComponents(SerializableComponent[] serializableComponents, SaveItem item,
            SaveGameSettings saveGameSettings)
        {
            for (int i = 0; i < serializableComponents.Length; i++)
            {
                if (item.TryGetComponent(serializableComponents[i].Id, out var component))
                {
                    if (serializableComponents[i].Value == DestroyedComponentMarker)
                    {
                        Object.Destroy(component);
                        continue;
                    }

                    if (component is MonoBehaviour asMonoBehaviour)
                    {
                        asMonoBehaviour.enabled = serializableComponents[i].Enabled;
                    }

                    if (component is ISimpleSaveCustomSerialization asISimpleSaveCustomSerialization)
                    {
                        asISimpleSaveCustomSerialization.Populate(serializableComponents[i].Value);
                    }
                    else
                    {
                        saveGameSettings.ComponentSerializer.PopulateTo(serializableComponents[i].Value, component);
                    }

                    ReferenceController.LoadReferences(serializableComponents[i].References, component,
                        saveGameSettings.ReferenceResolver);

                    if (component is ISimpleSaveCallbacks asISimpleSaveItemCallbacks)
                    {
                        asISimpleSaveItemCallbacks.OnAfterLoaded();
                    }
                }
            }
        }

        #endregion
    }
}