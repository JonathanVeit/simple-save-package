using SimpleSave.Services;
using UnityEngine;

namespace SimpleSave
{
    /// <summary>
    /// Startup for SimpleSave to register required services.
    /// </summary>
    internal static class Startup
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void SetupInEditor()
        {
            RegisterSharedServices();
            RegisterEditorServices();
            RegisterRuntimeServices();
        }
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void SetupInBuild()
        {
            RegisterSharedServices();
            RegisterRuntimeServices();
        }
#endif

        private static void RegisterSharedServices()
        {
            RegisterServiceAtRuntimeAndEditor<ILogger, SimpleSaveLogger>();
            RegisterServiceAtRuntimeAndEditor<IDebugHelper, DebugHelper>();
            RegisterServiceAtRuntimeAndEditor<IIdProvider, IdProvider>();
            RegisterServiceAtRuntimeAndEditor<ISaveItemHelper, SaveItemHelper>();
            RegisterServiceAtRuntimeAndEditor<ICoroutineHelper, CoroutineHelper>();

            RegisterServiceAtRuntimeAndEditor<ISaveGameCreator, SaveGameCreator>();
            RegisterServiceAtRuntimeAndEditor<ISaveItemConverter, SaveItemConverter>();

            RegisterServiceAtRuntimeAndEditor<ISaveGameLoader, SaveGameLoader>();
            RegisterServiceAtRuntimeAndEditor<IPrefabInstanceProvider, ResourcePrefabInstanceProvider>();

            RegisterServiceAtRuntimeAndEditor<IComponentSerializer, JsonComponentSerializer>();
            RegisterServiceAtRuntimeAndEditor<ISaveGameSerializer, JsonSaveGameSerializer>();
            RegisterServiceAtRuntimeAndEditor<ISaveGameWriter, JsonSaveGameWriter>();
            RegisterServiceAtRuntimeAndEditor<ISaveGameReader, JsonSaveGameReader>();

            RegisterServiceAtRuntimeAndEditor<ISaveItemContainerManager, SaveItemContainerManager>();

            RegisterServiceAtRuntimeAndEditor<ISaveVarController, SaveVarController>();
            RegisterServiceAtRuntimeAndEditor<ISaveVarSerializer, JsonSaveVarSerializer>();

            RegisterServiceAtRuntimeAndEditor<IAssemblyScanner, AssemblyScanner>();
            RegisterServiceAtRuntimeAndEditor<IReferenceController, ReferenceController>();
            RegisterServiceAtRuntimeAndEditor<IReferenceResolver, ReferenceResolver>();

            RegisterServiceAtRuntimeAndEditor<ISimpleSaveSettings, SimpleSaveSettings>();

            RegisterServiceAtRuntimeAndEditor<ITagManager, TagManager>();

            RegisterServiceAtRuntimeAndEditor<IPackageHelper, PackageHelper>();
        }

        private static void RegisterServiceAtRuntimeAndEditor<TService, TImplementation>()
            where TImplementation : class, TService
        {
            ServiceWrapper.RegisterRuntimeService<TService, TImplementation>();
            ServiceWrapper.RegisterEditorService<TService, TImplementation>();
        }

        private static void RegisterEditorServices()
        {
            ServiceWrapper.RegisterEditorService<ISaveItemController, EditorSaveItemController>();
        }

        private static void RegisterRuntimeServices()
        {
            ServiceWrapper.RegisterRuntimeService<ISaveItemController, RuntimeSaveItemController>();
        }
    }
}