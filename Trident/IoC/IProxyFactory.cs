using System;

namespace Trident.IoC
{
    /// <summary>
    /// Interface IProxyFactory
    /// </summary>
    public interface IProxyFactory
    {
        /// <summary>
        /// Creates the proxy generic. This method requires that T is known generically.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Target">The target.</param>
        /// <returns>T.</returns>
        T CreateProxyGeneric<T>(T Target) where T : class;

        /// <summary>
        /// Creates the proxy. This method allows use of reflection to register and apply the proxy.
        /// </summary>
        /// <param name="limitType">Type of the limit.</param>
        /// <param name="target">The target.</param>
        /// <returns>System.Object.</returns>
        object CreateProxy(Type limitType, object target);
    }
}
