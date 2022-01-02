using System;
using System.Collections.Generic;
using SimpleSave.Extensions;
using SimpleSave.Models;
using SimpleSave.Models.Serializables;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="IReferenceController"/>
    internal sealed class ReferenceController : BaseService, IReferenceController
    {
        #region Services

        private static IAssemblyScanner AssemblyScanner => ServiceWrapper.GetService<IAssemblyScanner>();

        #endregion

        #region To Serializable

        /// <inheritdoc />
        public SerializableReference[] GetReferences(Component component, SaveItem savingItem,
            IReferenceResolver referenceResolver)
        {
            var componentType = component.GetType();
            if (!ReferenceInfoContainer.Instance.TryGet(componentType, out ReferenceInfo[] referenceInfos))
            {
                return null;
            }

            var result = new List<SerializableReference>();

            for (int i = 0; i < referenceInfos.Length; i++)
            {
                if (TryGetReference(component, referenceInfos[i], referenceResolver,
                    out SerializableReference serializableReference))
                {
                    result.Add(serializableReference);
                }
            }

            return result.ToArray();
        }

        private bool TryGetReference(Component component, ReferenceInfo referenceInfo,
            IReferenceResolver referenceResolver, out SerializableReference serializableReference)
        {
            switch (referenceInfo.MemberCategory)
            {
                case MemberCategory.Field:
                    return TryGetFieldReference(component, referenceInfo, referenceResolver,
                        out serializableReference);
                case MemberCategory.Property:
                    return TryGetPropertyReference(component, referenceInfo, referenceResolver,
                        out serializableReference);

                default:
                    Logger.LogInternal($"{nameof(ReferenceController)} has no implementation to get references of category \"{referenceInfo.MemberCategory}\".");
                    serializableReference = default;
                    return false;
            }
        }

        private bool TryGetFieldReference(Component component, ReferenceInfo referenceInfo,
            IReferenceResolver referenceResolver, out SerializableReference serializableReference)
        {
            var fieldInfo = referenceInfo.DeclaringType.GetField(referenceInfo.MemberName, AssemblyScanner.ScanningFlags);
            if (fieldInfo is null)
            {
                Logger.LogInternal($"Unable to find field \"{referenceInfo.MemberType}\" on component \"{referenceInfo.DeclaringType.Name}\".");
                serializableReference = default;
                return false;
            }

            var fieldValue = fieldInfo.GetValue(component);

            return referenceResolver.TryConvertReference(fieldValue, referenceInfo, out serializableReference);
        }

        private bool TryGetPropertyReference(Component component, ReferenceInfo referenceInfo,
            IReferenceResolver referenceResolver, out SerializableReference serializableReference)
        {
            var propertyInfo =
                referenceInfo.DeclaringType.GetProperty(referenceInfo.MemberName, AssemblyScanner.ScanningFlags);
            if (propertyInfo is null)
            {
                Logger.LogInternal($"Unable to find property \"{referenceInfo.MemberType}\" on component \"{referenceInfo.DeclaringType.Name}\".");
                serializableReference = default;
                return false;
            }

            var propertyValue = propertyInfo.GetValue(component);

            return referenceResolver.TryConvertReference(propertyValue, referenceInfo, out serializableReference);
        }

        #endregion

        #region Resolve

        /// <inheritdoc />
        public void LoadReferences(SerializableReference[] serializableReferences, Component component,
            IReferenceResolver referenceResolver)
        {
            for (int i = 0; i < serializableReferences.Length; i++)
            {
                ResolveReference(serializableReferences[i], component, referenceResolver);
            }
        }

        private void ResolveReference(SerializableReference serializableReference, Component component,
            IReferenceResolver referenceResolver)
        {
            if (!ReferenceInfoContainer.Instance.TryGet(new ReferenceId(serializableReference.Id),
                out ReferenceInfo saveRefInfo))
            {
                return;
            }

            switch (saveRefInfo.MemberCategory)
            {
                case MemberCategory.Field:
                    ResolveAsField(serializableReference, saveRefInfo, component, referenceResolver);
                    break;
                case MemberCategory.Property:
                    ResolveAsProperty(serializableReference, saveRefInfo, component, referenceResolver);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ResolveAsField(SerializableReference serializableReference, ReferenceInfo referenceInfo,
            Component component, IReferenceResolver referenceResolver)
        {
            var field = referenceInfo.DeclaringType.GetField(referenceInfo.MemberName, AssemblyScanner.ScanningFlags);
            if (field is null)
            {
                Logger.LogInternal($"Unable to find field \"{referenceInfo.MemberType}\" on component \"{referenceInfo.DeclaringType.Name}\".");
                return;
            }

            if (referenceResolver.TryResolveReference(serializableReference, referenceInfo, out object value))
            {
                field.SetValue(component, value);
            }
        }

        private void ResolveAsProperty(SerializableReference serializableReference, ReferenceInfo referenceInfo,
            Component component, IReferenceResolver referenceResolver)
        {
            var property = referenceInfo.DeclaringType.GetProperty(referenceInfo.MemberName, AssemblyScanner.ScanningFlags);
            if (property is null)
            {
                Logger.LogInternal($"Unable to find field \"{referenceInfo.MemberType}\" on component \"{referenceInfo.DeclaringType.Name}\".");
                return;
            }

            if (referenceResolver.TryResolveReference(serializableReference, referenceInfo, out object value))
            {
                property.SetValue(component, value);
            }
        }

        #endregion
    }
}