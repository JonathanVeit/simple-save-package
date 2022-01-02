using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleSave
{
    /// <summary>
    /// Simple service container class.
    /// Services are created with constructor dependency injection which automatically creates and injects the services into the constructor.
    /// </summary>
    /// <remarks>
    /// Each service will created as singleton.<br />
    /// When creating the service, the very first constructor that accepts the registered services will be used.<br />
    /// Empty constructors are supported as long as they are the only available constructor.
    /// </remarks>
    internal class ServiceProvider
    {
        private readonly Dictionary<Type, Type> RegisteredServices;
        private readonly Dictionary<Type, object> ServiceInstances;
        private readonly HashSet<Type> serviceCache;

        public ServiceProvider()
        {
            RegisteredServices = new Dictionary<Type, Type>();
            ServiceInstances = new Dictionary<Type, object>();

            serviceCache = new HashSet<Type>();
        }

        #region Register Service

        /// <summary>
        /// Registers a new service.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TImplementation">Service implementation.</typeparam>
        public void RegisterService<TService, TImplementation>()
            where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            var implementationType = typeof(TImplementation);

            if (RegisteredServices.ContainsKey(serviceType))
            {
                throw new ArgumentException($"There is already a service registered for type {serviceType.Name}.");
            }

            RegisteredServices.Add(serviceType, implementationType);
        }

        #endregion

        #region Get Service

        /// <summary>
        /// Gets the current implementation registered for a certain service type.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <returns>The registered implementation.</returns>
        public TService GetService<TService>()
        {
            return LoadOrCreateService<TService>();
        }

        private TService LoadOrCreateService<TService>()
        {
            var serviceType = typeof(TService);

            if (ServiceInstances.TryGetValue(serviceType, out var instance))
            {
                return (TService)instance;
            }
    
            serviceCache.Clear();
            var serviceInstance = CreateInstanceWithDependencies(serviceType);

            return (TService)serviceInstance;
        }

        #endregion

        #region Service Creation

        private object CreateInstanceWithDependencies(Type serviceType)
        {
            if (!serviceCache.Contains(serviceType))
            {
                serviceCache.Add(serviceType);
            }
            else
            {
                //throw new ArgumentException($"Circular dependency detected for service type {serviceType.Name}.");
            }

            if (!RegisteredServices.TryGetValue(serviceType, out var implementationType))
            {
                throw new NotImplementedException($"No service implemented for type {serviceType.Name}.");
            }

            var constructors = implementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < constructors.Length; i++)
            {
                var constructor = constructors[i];
                var parameters = constructor.GetParameters();

                // only one constructor without any arguments
                if (constructors.Length == 1 &&
                    parameters.Length == 0 ||
                    ContainsConstructorParameters(parameters))
                {
                    var serviceInstance = CreateInstanceByParameters(implementationType, parameters);
                    ServiceInstances.Add(serviceType, serviceInstance);

                    return serviceInstance;
                }
            }

            throw new ArgumentException($"No suitable constructor found to create instance of type {implementationType.Name}.");
        }

        private bool ContainsConstructorParameters(ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (!RegisteredServices.ContainsKey(parameters[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

        private object CreateInstanceByParameters(Type type, ParameterInfo[] parameters)
        {
            object[] services = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var serviceType = parameters[i].ParameterType;
                if (ServiceInstances.TryGetValue(serviceType, out var serviceInstance))
                {
                    services[i] = serviceInstance;
                    continue;
                }

                var newService = CreateInstanceWithDependencies(serviceType);

                services[i] = newService;
            }

            return Activator.CreateInstance(type, services);
        }

        #endregion
        
        #region Unegister Service

        /// <summary>
        /// UnRegisters editor service of a certain type.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        public void UnregisterService<TService>()
        {
            var serviceType = typeof(TService);

            if (RegisteredServices.ContainsKey(serviceType))
            {
                RegisteredServices.Remove(serviceType);
            }
        }

        /// <summary>
        /// UnRegisters all current services.
        /// </summary>
        public void Clear()
        {
            RegisteredServices.Clear();
            ServiceInstances.Clear();
        }

        #endregion
    }
}