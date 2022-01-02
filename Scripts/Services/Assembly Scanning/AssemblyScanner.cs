using System;
using System.Linq;
using System.Reflection;
using SimpleSave.Extensions;
using SimpleSave.Models;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="IAssemblyScanner"/>
    internal class AssemblyScanner : BaseService, IAssemblyScanner
    {
        private const BindingFlags scanningFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        /// <inheritdoc />
        public BindingFlags ScanningFlags => scanningFlags;

        #region Services

        private static readonly IIdProvider IdProvider = ServiceWrapper.GetService<IIdProvider>();

        #endregion

#if UNITY_EDITOR

        [UnityEditor.InitializeOnLoadMethod]
        private static void StartScan()
        {
            if (CannotScan())
            {
                return;
            }

            DebugHelper.StartTimer("AssemblyScanner");

            var saveVarContainer = SaveVarInfoContainer.Instance;
            var referenceContainer = ReferenceInfoContainer.Instance;

            saveVarContainer.Clear();
            referenceContainer.Clear();

            var assemblies = Settings.AssembliesToScan;
            int validAssemblies = 0;
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    var assembly = Assembly.Load(assemblies[i]);
                    ScanAssembly(assembly);
                    validAssemblies++;
                }
                catch (Exception e)
                {
                    Logger.Log(LogType.Error, $"Unable to load Assembly \"{assemblies[i]}\".\n{e.Message}");
                }
            }

            Logger.LogDebug(
                $"Scanned {validAssemblies} assemblie(s) in {DebugHelper.StopTimer("AssemblyScanner")} ms.\n" +
                $"Found {saveVarContainer.Count} [SaveVar] attributes and {referenceContainer.Count} [SaveRef] attributes.");

            UnityEditor.EditorUtility.SetDirty(saveVarContainer);
            UnityEditor.EditorUtility.SetDirty(referenceContainer);

            UnityEditor.AssetDatabase.SaveAssets();
        }

        private static bool CannotScan()
        {
            return UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode ||
                   Application.isPlaying ||
                   UnityEditor.BuildPipeline.isBuildingPlayer ||
                   UnityEditor.EditorApplication.isCompiling;
        }

        private static void ScanAssembly(Assembly assembly)
        {
            var typesInAssembly = assembly.GetTypes();

            for (var index = 0; index < typesInAssembly.Length; index++)
            {
                ScanType(typesInAssembly[index]);
            }
        }

        private static void ScanType(Type type)
        {
            var memberInfos = type.GetMembers(scanningFlags)
                .Where(field => field.IsDefined(typeof(SaveVar)) || field.IsDefined(typeof(SaveRef)));

            foreach (var memberInfo in memberInfos)
            {
                AddMember(type, memberInfo);
            }

            ScanBaseClasses(type, type.BaseType);
        }

        private static void ScanBaseClasses(Type type, Type baseType)
        {
            if (baseType == null ||
                baseType == typeof(MonoBehaviour) ||
                baseType == typeof(Component))
            {
                return;
            }

            var memberInfos = baseType.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Where(field => field.IsDefined(typeof(SaveRef)));

            foreach (var memberInfo in memberInfos)
            {
                AddMember(type, memberInfo);
            }

            ScanBaseClasses(type, baseType.BaseType);
        }

        private static void AddMember(Type owningType, MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    AddField(owningType, (FieldInfo)memberInfo);
                    break;

                case MemberTypes.Property:
                    AddProperty(owningType, (PropertyInfo)memberInfo);
                    break;

                default:
                    return;
            }
        }

        private static void AddField(Type owningType, FieldInfo fieldInfo)
        {
            if (fieldInfo.IsDefined(typeof(SaveVar)))
            {
                if (!SaveVarAttributeIsValid(fieldInfo))
                {
                    return;
                }

                var saveVarInfo = new SaveVarInfo
                {
                    MemberName = fieldInfo.Name,
                    MemberCategory = MemberCategory.Field,
                    MemberType = fieldInfo.FieldType,
                    DeclaringType = fieldInfo.DeclaringType,
                    Tags = fieldInfo.GetCustomAttribute<SaveVar>().Tags
                };

                saveVarInfo.Id = IdProvider.GetSaveVarId(saveVarInfo);
                SaveVarInfoContainer.Instance.Add(saveVarInfo);
            }

            if (fieldInfo.IsDefined(typeof(SaveRef)))
            {
                if (!SaveRefAttributeIsValid(fieldInfo))
                {
                    return;
                }

                var referenceInfo = new ReferenceInfo
                {
                    MemberName = fieldInfo.Name,
                    MemberCategory = MemberCategory.Field,
                    MemberType = fieldInfo.FieldType,
                    DeclaringType = fieldInfo.DeclaringType,
                    OwningType = owningType,
                };

                referenceInfo.Id = IdProvider.GetReferenceId(referenceInfo);
                ReferenceInfoContainer.Instance.Add(referenceInfo);
            }
        }

        private static void AddProperty(Type owningType, PropertyInfo propertyInfo)
        {
            if (!PropertyDeclaringIsValid(propertyInfo))
            {
                return;
            }

            if (propertyInfo.IsDefined(typeof(SaveVar)))
            {
                if (!SaveVarAttributeIsValid(propertyInfo))
                {
                    return;
                }

                var saveVarInfo = new SaveVarInfo
                {
                    MemberName = propertyInfo.Name,
                    MemberCategory = MemberCategory.Property,
                    MemberType = propertyInfo.PropertyType,
                    DeclaringType = propertyInfo.DeclaringType,
                    Tags = propertyInfo.GetCustomAttribute<SaveVar>().Tags
                };

                saveVarInfo.Id = IdProvider.GetSaveVarId(saveVarInfo);
                SaveVarInfoContainer.Instance.Add(saveVarInfo);
            }

            if (propertyInfo.IsDefined(typeof(SaveRef)))
            {
                if (!SaveRefAttributeIsValid(propertyInfo))
                {
                    return;
                }

                var referenceInfo = new ReferenceInfo
                {
                    MemberName = propertyInfo.Name,
                    MemberCategory = MemberCategory.Property,
                    MemberType = propertyInfo.PropertyType,
                    DeclaringType = propertyInfo.DeclaringType,
                    OwningType = owningType,
                };

                referenceInfo.Id = IdProvider.GetReferenceId(referenceInfo);
                ReferenceInfoContainer.Instance.Add(referenceInfo);
            }
        }

#endif

        #region Helper

        private static bool PropertyDeclaringIsValid(PropertyInfo propertyInfo)
        {
            if (propertyInfo.SetMethod == null)
            {
                Logger.Log(LogType.Warning,
                    $"The property \"{propertyInfo.Name}\" of \"{propertyInfo.DeclaringType.Name}\" has no setter and cannot be saved or loaded.");
                return false;
            }

            if (propertyInfo.GetMethod == null)
            {
                Logger.Log(LogType.Warning,
                    $"The property \"{propertyInfo.Name}\" of \"{propertyInfo.DeclaringType.Name}\" has no getter and cannot be saved or loaded.");
                return false;
            }

            return true;
        }

        private static bool SaveVarAttributeIsValid(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    if (!propertyInfo.SetMethod.IsStatic ||
                        !propertyInfo.GetMethod.IsStatic)
                    {
                        Logger.Log(LogType.Warning,
                            $"The property \"{propertyInfo.Name}\" of \"{propertyInfo.DeclaringType.Name}\" is not declared as static and cannot be saved or loaded.");
                        return false;
                    }

                    if (propertyInfo.PropertyType.IsSubclassOf(typeof(UnityEngine.Object)) ||
                        propertyInfo.PropertyType == typeof(UnityEngine.Object))
                    {
                        if (!propertyInfo.IsDefined(typeof(SaveRef)))
                        {
                            Logger.Log(LogType.Warning,
                                $"The field \"{propertyInfo.Name}\" of \"{propertyInfo.DeclaringType.Name}\" is a reference type but has no {nameof(SaveRef)} attribute. " +
                                "Saving the reference wont work.");

                            return false;
                        }
                    }

                    break;

                case FieldInfo fieldInfo:
                    if (!fieldInfo.IsStatic)
                    {
                        Logger.Log(LogType.Warning,
                            $"The field \"{fieldInfo.Name}\" of \"{fieldInfo.DeclaringType.Name}\" is not declared as static and cannot be saved or loaded.");
                        return false;
                    }

                    if (fieldInfo.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) ||
                        fieldInfo.FieldType == typeof(UnityEngine.Object))
                    {
                        if (!fieldInfo.IsDefined(typeof(SaveRef)))
                        {
                            Logger.Log(LogType.Warning,
                                $"The field \"{fieldInfo.Name}\" of \"{fieldInfo.DeclaringType.Name}\" is a reference type but has no {nameof(SaveRef)} attribute. " +
                                "Saving the reference wont work.");
                            return false;
                        }
                    }

                    break;
            }

            return true;
        }

        private static bool SaveRefAttributeIsValid(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    if (propertyInfo.PropertyType != typeof(Component) &&
                        !propertyInfo.PropertyType.IsSubclassOf(typeof(Component)) &&
                        propertyInfo.PropertyType != typeof(GameObject))
                    {
                        Logger.Log(LogType.Warning,
                            $"The reference of property \"{propertyInfo.Name}\" of \"{propertyInfo.DeclaringType.Name}\" cannot be saved. Only {nameof(Component)} and {nameof(GameObject)} references can be saved.");
                        return false;
                    }

                    break;

                case FieldInfo fieldInfo:
                    if (fieldInfo.FieldType != typeof(Component) &&
                        !fieldInfo.FieldType.IsSubclassOf(typeof(Component)) &&
                        fieldInfo.FieldType != typeof(GameObject))
                    {
                        Logger.Log(LogType.Warning,
                            $"The reference of field \"{fieldInfo.Name}\" of \"{fieldInfo.DeclaringType.Name}\" cannot be saved. Only {nameof(Component)} and {nameof(GameObject)} references can be saved.");
                        return false;
                    }

                    break;
            }

            return true;
        }

        #endregion
    }
}