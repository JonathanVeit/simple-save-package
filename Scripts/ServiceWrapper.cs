using UnityEngine;

namespace SimpleSave
{
    /// <summary>
    /// Wrapper for all services of SimpleSave. Services are divided into runtime and editor specific.
    /// </summary>
    internal static class ServiceWrapper
    {
        private static readonly ServiceProvider EditorServiceProvider;
        private static readonly ServiceProvider RuntimeServiceProvider;
        
        static ServiceWrapper()
        {
            RuntimeServiceProvider = new ServiceProvider();
            EditorServiceProvider = new ServiceProvider();
        }

        #region Register Service

        /// <summary>
        /// Registers a new service to be used in the editor.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TImplementation">Service implementation.</typeparam>
        public static void RegisterEditorService<TService, TImplementation>()
            where TImplementation : class, TService
        {
            EditorServiceProvider.RegisterService<TService, TImplementation>();
        }

        /// <summary>
        /// Registers a new service to be used at runtime.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TImplementation">Service implementation.</typeparam>
        public static void RegisterRuntimeService<TService, TImplementation>()
            where TImplementation : class, TService
        {
            RuntimeServiceProvider.RegisterService<TService, TImplementation>();
        }

        #endregion

        #region Get Service

        /// <summary>
        /// Gets the current implementation registered for a certain service type.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <returns>The registered implementation.</returns>
        public static TService GetService<TService>()
        {
            if (Application.isPlaying)
            {
                return RuntimeServiceProvider.GetService<TService>();
            }
            else
            {
                return EditorServiceProvider.GetService<TService>();
            }
        }

        #endregion

        #region Unregister Service

        /// <summary>
        /// Unregisters editor service of a certain type.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        public static void UnregisterEditorService<TService>()
        {
            EditorServiceProvider.UnregisterService<TService>();
        }

        /// <summary>
        /// Unregisters runtime service of a certain type.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        public static void UnregisterRuntimeService<TService>()
        {
            RuntimeServiceProvider.UnregisterService<TService>();
        }

        /// <summary>
        /// Unregisters all current services for editor and runtime.
        /// </summary>
        public static void Clear()
        {
            EditorServiceProvider.Clear();
            RuntimeServiceProvider.Clear();
        }

        #endregion
    }
}