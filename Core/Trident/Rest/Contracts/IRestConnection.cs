using System;
using System.Threading.Tasks;

namespace Trident.Rest.Contracts
{
    /// <summary>
    /// Interface IRestConnection
    /// </summary>
    public interface IRestConnection
    {
        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <value>The base URL.</value>
        Uri BaseUrl { get; }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        int Timeout { get; }

        /// <summary>
        /// Authenticates this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> Authenticate<T>();

        /// <summary>
        /// Authenticates this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Task&lt;T&gt;.</returns>
        T AuthenticateSync<T>();
    }
}
