using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSC
{

    /// <summary>
    /// A simple implementation of the IServiceContainer interface.
    /// </summary>
    public class SimpleServiceContainer : IServiceContainer
    {
        private readonly IDictionary<Type, ServiceDescriptor> _services;
        private readonly IDictionary<Type, object> _singletonInstances;
        private readonly IDictionary<Type, object> _scopedServices;
        private readonly object _syncRoot = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleServiceContainer"/> class.
        /// </summary>
        public SimpleServiceContainer()
        {
            _services = new Dictionary<Type, ServiceDescriptor>();
            _singletonInstances = new Dictionary<Type, object>();
            _scopedServices = new Dictionary<Type, object>();
        }

        #region RegisterSingleton
        /// <summary>
        /// Registers a singleton service with an existing instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The existing instance of the service.</param>
        public void RegisterSingleton<TService>(TService instance)
        {
            lock (_syncRoot)
            {
                if (_services.ContainsKey(typeof(TService)))
                {
                    throw new InvalidOperationException($"Service of type {typeof(TService)} is already registered.");
                }
                _services[typeof(TService)] = new ServiceDescriptor(typeof(TService),
                    () => instance, ServiceLifetime.Singleton);
            }
        }

        /// <summary>
        /// Registers a singleton service with an implementation factory.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationFactory">The factory function that creates instances of the service.</param>
        public void RegisterSingleton<TService>(Func<TService> implementationFactory)
        {
            lock (_syncRoot)
            {
                if (_services.ContainsKey(typeof(TService)))
                {
                    throw new InvalidOperationException($"Service of type {typeof(TService)} is already registered.");
                }
                _services[typeof(TService)] = new ServiceDescriptor(typeof(TService),
                    () => implementationFactory(), ServiceLifetime.Singleton);
            }
        }

        /// <summary>
        /// Registers a singleton service with the specified implementation.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void RegisterSingleton<TService, TImplementation>() where TImplementation : TService
        {
            lock (_syncRoot)
            {
                _services[typeof(TService)] = new ServiceDescriptor(typeof(TService),
                    () => CreateInstanceWithDependencies(typeof(TImplementation)), ServiceLifetime.Singleton);
            }
        }

        /// <summary>
        /// Registers a singleton service with the specified implementation.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <remarks>
        /// Singleton services are created only once and the same instance is returned for all subsequent requests.
        /// </remarks>
        public void RegisterSingleton<TImplementation>() where TImplementation : class
        {
            lock (_syncRoot)
            {
                _services[typeof(TImplementation)] = new ServiceDescriptor(typeof(TImplementation),
                    () => CreateInstanceWithDependencies(typeof(TImplementation)), ServiceLifetime.Singleton);
            }
        }
        #endregion

        #region RegisterTransient
        /// <summary>
        /// Registers a transient service with an implementation factory.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationFactory">The factory function that creates instances of the service.</param>
        public void RegisterTransient<TService>(Func<TService> implementationFactory)
        {
            lock (_syncRoot)
            {
                _services[typeof(TService)] = new ServiceDescriptor(typeof(TService), 
                    () => implementationFactory(), ServiceLifetime.Transient);
            }
        }

        /// <summary>
        /// Registers a transient service with the specified implementation.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void RegisterTransient<TService, TImplementation>() where TImplementation : TService
        {
            lock (_syncRoot)
            {
                _services[typeof(TService)] = new ServiceDescriptor(typeof(TService),
                    () => CreateInstanceWithDependencies(typeof(TImplementation)), ServiceLifetime.Transient);
            }
        }

        /// <summary>
        /// Registers a transient service with the specified implementation.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <remarks>
        /// Transient services are created each time they are requested.
        /// </remarks>
        public void RegisterTransient<TImplementation>() where TImplementation : class
        {
            lock (_syncRoot)
            {
                _services[typeof(TImplementation)] = new ServiceDescriptor(typeof(TImplementation),
                    () => CreateInstanceWithDependencies(typeof(TImplementation)), ServiceLifetime.Transient);
            }
        }
        #endregion

        #region CreateScoped

        /// <summary>
        /// Registers a scoped service with a specific implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void RegisterScoped<TService, TImplementation>()
        {
            var serviceType = typeof(TService);
            var implementationType = typeof(TImplementation);

            lock (_syncRoot)
            {
                _services[serviceType] = new ServiceDescriptor(serviceType, 
                    () => CreateInstanceWithDependencies(implementationType), ServiceLifetime.Scoped);
            }
        }

        /// <summary>
        /// Registers a scoped service with the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void RegisterScoped<TImplementation>() where TImplementation : class
        {
            var serviceType = typeof(TImplementation);

            if (_services.ContainsKey(serviceType))
            {
                throw new InvalidOperationException($"Service {serviceType} is already registered.");
            }

            _services[serviceType] = new ServiceDescriptor(serviceType, () => Activator.CreateInstance<TImplementation>(), ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Creates a new service scope.
        /// </summary>
        /// <returns>A new IServiceScope.</returns>
        public IServiceScope CreateScope()
        {
            return new ServiceScope(this);
        }

        /// <summary>
        /// Retrieves a registered service descriptor.
        /// </summary>
        /// <param name="serviceType">The type of the service to retrieve.</param>
        /// <returns>A ServiceDescriptor instance if the service is registered; otherwise, null.</returns>
        public ServiceDescriptor GetServiceDescriptor(Type serviceType)
        {
            return _services.TryGetValue(serviceType, out var serviceDescriptor) ? serviceDescriptor : null;
        }

        /// <summary>
        /// Creates a scoped instance of the specified service.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns>An instance of the specified service if it is registered as scoped; otherwise, null.</returns>
        public object CreateScopedInstance(Type serviceType)
        {
            var serviceDescriptor = GetServiceDescriptor(serviceType);

            if (serviceDescriptor.Lifetime != ServiceLifetime.Scoped)
            {
                throw new InvalidOperationException($"No scoped service is registered for type {serviceType}.");
            }

            return serviceDescriptor.ImplementationFactory();
        }

        /// <summary>
        /// Gets an instance of the specified service.
        /// </summary>
        /// <param name="serviceType">The type of the service to retrieve.</param>
        /// <returns>An instance of the specified service if it is registered; otherwise, null.</returns>
        public object GetService(Type serviceType)
        {
            var serviceDescriptor = GetServiceDescriptor(serviceType);

            if (serviceDescriptor == null)
            {
                throw new InvalidOperationException($"No service registered for type '{serviceType.FullName}'.");
            }

            if (serviceDescriptor.Lifetime == ServiceLifetime.Singleton)
            {
                lock (_syncRoot)
                {
                    if (!_singletonInstances.TryGetValue(serviceType, out var instance))
                    {
                        instance = serviceDescriptor.ImplementationFactory();
                        _singletonInstances[serviceType] = instance;
                    }

                    return instance;
                }
            }
            else
            {
                return serviceDescriptor.ImplementationFactory();
            }
        }

        #endregion

        #region CreateInstance
        /// <summary>
        /// Creates an instance of the specified service type with its dependencies resolved.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>An instance of the service with its dependencies resolved.</returns>
        public TService CreateInstance<TService>()
        {
            return (TService)CreateInstance(typeof(TService));
        }

        /// <summary>
        /// Creates an instance of the specified service type with its dependencies resolved.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns>An instance of the service with its dependencies resolved.</returns>
        public object CreateInstance(Type serviceType)
        {
            ServiceDescriptor serviceDescriptor;

            lock (_syncRoot)
            {
                if (!_services.TryGetValue(serviceType, out serviceDescriptor))
                {
                    throw new InvalidOperationException($"Service of type {serviceType} is not registered.");
                }
            }

            if (serviceDescriptor.Lifetime == ServiceLifetime.Singleton)
            {
                lock (_syncRoot)
                {
                    if (_singletonInstances.TryGetValue(serviceType, out var instance))
                    {
                        return instance;
                    }
                    else
                    {
                        instance = serviceDescriptor.ImplementationFactory();
                        _singletonInstances[serviceType] = instance;
                        return instance;
                    }
                }
            }
            else
            {
                return serviceDescriptor.ImplementationFactory();
            }
        }

        /// <summary>
        /// Creates an instance of the specified implementation type by resolving its constructor dependencies.
        /// </summary>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <returns>The created implementation instance with resolved dependencies.</returns>
        public object CreateInstanceWithDependencies(Type implementationType)
        {
            var constructor = implementationType.GetConstructors().First();
            var constructorParameters = constructor.GetParameters();
            var parameterInstances = new object[constructorParameters.Length];

            for (int i = 0; i < constructorParameters.Length; i++)
            {
                parameterInstances[i] = CreateInstance(constructorParameters[i].ParameterType);
            }

            return constructor.Invoke(parameterInstances);
        }
        #endregion

        /// <summary>
        /// Releases all resources used by the SimpleServiceContainer.
        /// </summary>
        public void Dispose()
        {
            lock (_syncRoot)
            {
                foreach (var singletonInstance in _singletonInstances.Values)
                {
                    try
                    {
                        if (singletonInstance is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                    catch (Exception e)
                    {

                        throw;
                    }
                }

                _singletonInstances.Clear();
            }
        }

        /// <summary>
        /// Represents a service descriptor with information about service registration.
        /// </summary>
        public class ServiceDescriptor
        {
            public Type ServiceType { get; }
            public Func<object> ImplementationFactory { get; }
            public ServiceLifetime Lifetime { get; }

            /// <summary>
            /// Initializes a new instance of the ServiceDescriptor class.
            /// </summary>
            /// <param name="serviceType">The type of the service.</param>
            /// <param name="implementationFactory">The factory function to create the service instance.</param>
            /// <param name="lifetime">The service's lifetime (singleton, transient, or scoped).</param>
            public ServiceDescriptor(Type serviceType, Func<object> implementationFactory, ServiceLifetime lifetime)
            {
                ServiceType = serviceType;
                ImplementationFactory = implementationFactory;
                Lifetime = lifetime;
            }
        }

        /// <summary>
        /// An enumeration of service lifetimes.
        /// </summary>
        public enum ServiceLifetime
        {
            Singleton,
            Transient,
            Scoped
        }
    }
}
