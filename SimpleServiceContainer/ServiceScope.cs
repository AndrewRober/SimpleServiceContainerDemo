using System;
using System.Collections.Generic;

namespace SSC
{
    /// <summary>
    /// Represents a service scope, which is responsible for resolving scoped services and managing their lifetimes.
    /// </summary>
    public class ServiceScope : IServiceScope
    {
        private readonly SimpleServiceContainer _container;
        private readonly IDictionary<Type, object> _scopedInstances;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceScope"/> class.
        /// </summary>
        /// <param name="container">The container used for resolving services.</param>
        public ServiceScope(SimpleServiceContainer container)
        {
            _container = container;
            _scopedInstances = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Gets an instance of the specified service type.
        /// </summary>
        /// <param name="serviceType">The type of the service to resolve.</param>
        /// <returns>An instance of the resolved service type.</returns>
        public object GetService(Type serviceType)
        {
            var serviceDescriptor = _container.GetServiceDescriptor(serviceType);

            if (serviceDescriptor.Lifetime == SimpleServiceContainer.ServiceLifetime.Singleton)
            {
                return _container.GetService(serviceType);
            }
            else if (serviceDescriptor.Lifetime == SimpleServiceContainer.ServiceLifetime.Scoped)
            {
                if (!_scopedInstances.TryGetValue(serviceType, out var instance))
                {
                    instance = _container.CreateScopedInstance(serviceType);
                    _scopedInstances[serviceType] = instance;
                }

                return instance;
            }
            else
            {
                return _container.GetService(serviceType);
            }
        }

        /// <summary>
        /// Disposes the scoped instances managed by the service scope.
        /// </summary>
        public void Dispose()
        {
            foreach (var scopedInstance in _scopedInstances.Values)
            {
                if (scopedInstance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _scopedInstances.Clear();
        }
    }
}
