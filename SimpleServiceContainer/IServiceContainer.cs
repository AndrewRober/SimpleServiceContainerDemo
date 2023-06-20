using System;

using static SSC.SimpleServiceContainer;

namespace SSC
{
    /// <summary>
    /// Represents a simple dependency injection container that supports registering singleton and transient services.
    /// </summary>
    public interface IServiceContainer : IServiceProvider, IDisposable
    {
        #region RegisterSingleton
        /// <summary>
        /// Registers a singleton service with an existing instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The existing instance of the service.</param>
        void RegisterSingleton<TService>(TService instance);

        /// <summary>
        /// Registers a singleton service with an implementation factory.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationFactory">The factory function that creates instances of the service.</param>
        void RegisterSingleton<TService>(Func<TService> implementationFactory);

        /// <summary>
        /// Registers a singleton service with the specified implementation.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void RegisterSingleton<TService, TImplementation>() where TImplementation : TService;

        /// <summary>
        /// Registers a singleton service with the specified implementation.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <remarks>
        /// Singleton services are created only once and the same instance is returned for all subsequent requests.
        /// </remarks>
        void RegisterSingleton<TImplementation>() where TImplementation : class;
        #endregion

        #region RegisterTransient
        /// <summary>
        /// Registers a transient service with an implementation factory.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationFactory">The factory function that creates instances of the service.</param>
        void RegisterTransient<TService>(Func<TService> implementationFactory);

        /// <summary>
        /// Registers a transient service with the specified implementation.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void RegisterTransient<TService, TImplementation>() where TImplementation : TService;

        /// <summary>
        /// Registers a transient service with the specified implementation.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <remarks>
        /// Transient services are created each time they are requested.
        /// </remarks>
        void RegisterTransient<TImplementation>() where TImplementation : class;
        #endregion

        #region CreateScoped

        /// <summary>
        /// Registers a scoped service with a specific implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void RegisterScoped<TService, TImplementation>();

        /// <summary>
        /// Registers a scoped service with the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void RegisterScoped<TImplementation>() where TImplementation : class;

        /// <summary>
        /// Creates a new service scope.
        /// </summary>
        /// <returns>A new IServiceScope.</returns>
        IServiceScope CreateScope();

        /// <summary>
        /// Retrieves a registered service descriptor.
        /// </summary>
        /// <param name="serviceType">The type of the service to retrieve.</param>
        /// <returns>A ServiceDescriptor instance if the service is registered; otherwise, null.</returns>
        ServiceDescriptor GetServiceDescriptor(Type serviceType);

        /// <summary>
        /// Creates a scoped instance of the specified service.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns>An instance of the specified service if it is registered as scoped; otherwise, null.</returns>
        object CreateScopedInstance(Type serviceType);

        /// <summary>
        /// Gets an instance of the specified service.
        /// </summary>
        /// <param name="serviceType">The type of the service to retrieve.</param>
        /// <returns>An instance of the specified service if it is registered; otherwise, null.</returns>
        object GetService(Type serviceType);

        #endregion

        #region CreateInstance
        /// <summary>
        /// Creates an instance of the specified service type with its dependencies resolved.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>An instance of the service with its dependencies resolved.</returns>
        TService CreateInstance<TService>();

        /// <summary>
        /// Creates an instance of the specified service type with its dependencies resolved.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns>An instance of the service with its dependencies resolved.</returns>
        object CreateInstance(Type serviceType);

        /// <summary>
        /// Creates an instance of the specified implementation type by resolving its constructor dependencies.
        /// </summary>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <returns>The created implementation instance with resolved dependencies.</returns>
        object CreateInstanceWithDependencies(Type implementationType);
        #endregion
    }
}
