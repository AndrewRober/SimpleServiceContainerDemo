using System;

namespace SSC
{
    /// <summary>
    /// Defines the interface for a service scope, which is responsible for resolving scoped services and managing their lifetimes.
    /// </summary>
    public interface IServiceScope : IDisposable
    {
        /// <summary>
        /// Gets an instance of the specified service type.
        /// </summary>
        /// <param name="serviceType">The type of the service to resolve.</param>
        /// <returns>An instance of the resolved service type.</returns>
        object GetService(Type serviceType);
    }
}
