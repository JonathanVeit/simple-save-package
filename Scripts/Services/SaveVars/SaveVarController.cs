using System;
using SimpleSave.Extensions;
using SimpleSave.Models;
using SimpleSave.Models.Serializables;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveVarController"/>
    internal sealed class SaveVarController : BaseService, ISaveVarController
    {
        private static readonly Type SerializableReferenceType = typeof(SerializableReference);

        #region Services

        private static IAssemblyScanner AssemblyScanner => ServiceWrapper.GetService<IAssemblyScanner>();
        private static ITagManager TagManager => ServiceWrapper.GetService<ITagManager>();

        #endregion

        #region Get

        /// <inheritdoc />
        public SerializableSaveVar[] GetSaveVars(SaveGameSettings saveGameSettings)
        {
            var saveVarInfos = SaveVarInfoContainer.Instance.GetAll();
            var result = new SerializableSaveVar[CalculateTaggedSaveVarCount(saveVarInfos, saveGameSettings)];

            int index = 0;
            for (int i = 0; i < saveVarInfos.Length; i++)
            {
                if (!TagManager.DoAnyTagsMatch(saveVarInfos[i].Tags, saveGameSettings.Tags))
                {
                    continue;
                }

                result[index] = CreateSerializableSaveVar(saveVarInfos[i], saveGameSettings.SaveVarSerializer, saveGameSettings.ReferenceResolver);
                index++;
            }

            return result;
        }

        private int CalculateTaggedSaveVarCount(SaveVarInfo[] saveVarInfos, SaveGameSettings saveGameSettings)
        {
            int result = 0;
            for (int i = 0; i < saveVarInfos.Length; i++)
            {
                if (!TagManager.DoAnyTagsMatch(saveVarInfos[i].Tags, saveGameSettings.Tags))
                {
                    continue;
                }

                result++;
            }

            return result;
        }

        private SerializableSaveVar CreateSerializableSaveVar(SaveVarInfo saveVarInfo,
            ISaveVarSerializer saveVarSerializer, IReferenceResolver referenceResolver)
        {
            switch (saveVarInfo.MemberCategory)
            {
                case MemberCategory.Field:
                    return CreateSerializableSaveVarFromField(saveVarInfo, saveVarSerializer, referenceResolver);
                case MemberCategory.Property:
                    return CreateSerializableSaveVarFromProperty(saveVarInfo, saveVarSerializer, referenceResolver);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static SerializableSaveVar CreateSerializableSaveVarFromField(SaveVarInfo saveVarInfo,
            ISaveVarSerializer saveVarSerializer, IReferenceResolver referenceResolver)
        {
            var fieldInfo = saveVarInfo.DeclaringType.GetField(saveVarInfo.MemberName, AssemblyScanner.ScanningFlags);
            if (fieldInfo is null)
            {
                return new SerializableSaveVar();
            }

            var fieldValue = fieldInfo.GetValue(null);

            if (TrySaveAsReference(fieldValue, saveVarInfo, referenceResolver, out var serializableReference))
            {
                return new SerializableSaveVar
                {
                    Id = saveVarInfo.Id,
                    Value = saveVarSerializer.Serialize(serializableReference)
                };
            }

            return new SerializableSaveVar
            {
                Id = saveVarInfo.Id,
                Value = saveVarSerializer.Serialize(fieldValue)
            };
        }

        private static SerializableSaveVar CreateSerializableSaveVarFromProperty(SaveVarInfo saveVarInfo,
            ISaveVarSerializer saveVarSerializer, IReferenceResolver referenceResolver)
        {
            var propertyInfo =
                saveVarInfo.DeclaringType.GetProperty(saveVarInfo.MemberName, AssemblyScanner.ScanningFlags);
            if (propertyInfo is null)
            {
                return new SerializableSaveVar();
            }

            var propertyValue = propertyInfo.GetValue(null);

            if (TrySaveAsReference(propertyValue, saveVarInfo, referenceResolver, out var serializableReference))
            {
                return new SerializableSaveVar
                {
                    Id = saveVarInfo.Id,
                    Value = saveVarSerializer.Serialize(serializableReference)
                };
            }

            return new SerializableSaveVar
            {
                Id = saveVarInfo.Id,
                Value = saveVarSerializer.Serialize(propertyValue)
            };
        }

        private static bool TrySaveAsReference(object fieldValue, SaveVarInfo saveVarInfo,
            IReferenceResolver referenceResolver, out SerializableReference serializableReference)
        {
            if (ReferenceInfoContainer.Instance.TryGet(saveVarInfo.DeclaringType, out var referenceInfos))
            {
                for (int i = 0; i < referenceInfos.Length; i++)
                {
                    if (referenceInfos[i].MemberName == saveVarInfo.MemberName)
                    {
                        if (referenceResolver.TryConvertReference(fieldValue, referenceInfos[i],
                            out serializableReference))
                        {
                            return true;
                        }

                        return false;
                    }
                }
            }

            serializableReference = default;
            return false;
        }

        #endregion

        #region Load

        /// <inheritdoc />
        public void LoadSaveVars(SerializableSaveVar[] serializableSaveVars, SaveGameSettings saveGameSettings)
        {
            for (int i = 0; i < serializableSaveVars.Length; i++)
            {
                if (SaveVarInfoContainer.Instance.TryGet(new SaveVarId(serializableSaveVars[i].Id), out var saveVarInfo))
                {
                    if (saveGameSettings.Tags != null &&
                        !TagManager.DoAnyTagsMatch(saveVarInfo.Tags, saveGameSettings.Tags))
                    {
                        continue;
                    }

                    switch (saveVarInfo.MemberCategory)
                    {
                        case MemberCategory.Field:
                            LoadField(serializableSaveVars[i].Value, saveVarInfo, saveGameSettings.SaveVarSerializer, saveGameSettings.ReferenceResolver);
                            break;
                        case MemberCategory.Property:
                            LoadProperty(serializableSaveVars[i].Value, saveVarInfo, saveGameSettings.SaveVarSerializer, saveGameSettings.ReferenceResolver);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Logger.LogInternal($"Unable to load {typeof(SaveVar)} with id {serializableSaveVars[i].Id}.");
                }
            }
        }

        private static void LoadField(string value, SaveVarInfo saveVarInfo,
            ISaveVarSerializer saveVarSerializer, IReferenceResolver referenceResolver)
        {
            var field = saveVarInfo.DeclaringType.GetField(saveVarInfo.MemberName, AssemblyScanner.ScanningFlags);
            if (field is null)
            {
                Logger.LogInternal($"Unable to load field \"{saveVarInfo.MemberName}\" of type \"{saveVarInfo.DeclaringType}\". " +
                    $"The field could not be found.");
                return;
            }

            if (TryResolveAsReference(saveVarInfo, value, saveVarSerializer, referenceResolver,
                out var referencedObject))
            {
                field.SetValue(null, referencedObject);
                return;
            }

            field.SetValue(null, saveVarSerializer.Deserialize(value, saveVarInfo.MemberType));
        }

        private static void LoadProperty(string value, SaveVarInfo saveVarInfo,
            ISaveVarSerializer saveVarSerializer, IReferenceResolver referenceResolver)
        {
            var property = saveVarInfo.DeclaringType.GetProperty(saveVarInfo.MemberName, AssemblyScanner.ScanningFlags);
            if (property is null)
            {
                Logger.LogInternal(
                    $"Unable to load property \"{saveVarInfo.MemberName}\" of type \"{saveVarInfo.DeclaringType}\". " +
                    $"The property could not be found.");
                return;
            }

            if (TryResolveAsReference(saveVarInfo, value, saveVarSerializer, referenceResolver,
                out var referencedObject))
            {
                property.SetValue(null, referencedObject);
                return;
            }

            property.SetValue(null, saveVarSerializer.Deserialize(value, saveVarInfo.MemberType));
        }

        private static bool TryResolveAsReference(SaveVarInfo saveVarInfo, string serializedReference, 
            ISaveVarSerializer saveVarSerializer, IReferenceResolver referenceResolver, out object referencedObject)
        {
            if (ReferenceInfoContainer.Instance.TryGet(saveVarInfo.DeclaringType, out var referenceInfos))
            {
                for (int i = 0; i < referenceInfos.Length; i++)
                {
                    if (referenceInfos[i].MemberName != saveVarInfo.MemberName)
                    {
                        continue;
                    }

                    var serializableReference = (SerializableReference) saveVarSerializer.Deserialize(serializedReference, SerializableReferenceType);

                    if (referenceResolver.TryResolveReference(serializableReference, referenceInfos[i],
                        out referencedObject))
                    {
                        return true;
                    }

                    return true;
                }
            }

            referencedObject = default;
            return false;
        }

        #endregion
    }
}