using System;
using System.Collections;
using System.Collections.Generic;

namespace Trident.IoC
{
    /// <summary>
    /// Interface IIoCServiceLocator
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IIoCServiceLocator : IServiceProvider, IDisposable
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        T Get<T>();

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        object Get(Type type);

        /// <summary>
        /// Gets the specified parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns>T.</returns>
        T Get<T>(params Parameter[] parameters);

        bool TryGet<T>(out T service);
      
        /// <summary>
        /// Resolves all keyed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        IEnumerable<T> ResolveAllKeyed<T>();

        /// <summary>
        /// Resolves all typed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        IEnumerable<T> ResolveAllTyped<T>();

        IEnumerable ResolveAllTyped(Type ofType);

        /// <summary>
        /// Gets the named.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>T.</returns>
        T GetNamed<T>(string serviceName);

        /// <summary>
        /// Gets the named.
        /// </summary>
        /// <param name="serviceRegistrationType">Type of the service registration.</param>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>System.Object.</returns>
        object GetNamed(Type serviceRegistrationType, string serviceName);

        /// <summary>
        /// Gets the named.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>T.</returns>
        T GetNamed<T>(string serviceName, params Parameter[] parameters);

        /// <summary>
        /// Creates the child lifetime scope.
        /// </summary>
        /// <returns>IIoCServiceLocator.</returns>
        IIoCServiceLocator CreateChildLifetimeScope();

        /// <summary>
        /// Creates the child lifetime scope.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        /// <returns>IIoCServiceLocator.</returns>
        IIoCServiceLocator CreateChildLifetimeScope(string scopeName);

        /// <summary>
        /// Gets the name of the scope.
        /// </summary>
        /// <value>The name of the scope.</value>
        string ScopeName { get; }
    }
}
