using System;
using System.Threading.Tasks;
using Trident.Extensions;
using Trident.Rest.Contracts;

namespace Trident.Rest
{
    /// <summary>
    /// Class RestConnection.
    /// Implements the <see cref="TridentOptionsBuilder.Rest.Contracts.IRestConnection" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Rest.Contracts.IRestConnection" />
    public class RestConnection : IRestConnection
    {
        /// <summary>
        /// The authentication provider
        /// </summary>
        private readonly IRestAuthenticationProvider _authProvider;
        /// <summary>
        /// The connection string
        /// </summary>
        private readonly RestConnectionString _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestConnection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="authProvider">The authentication provider.</param>
        public RestConnection(string connection, IRestAuthenticationProvider authProvider)
        {
            _authProvider = authProvider;
            _connectionString = connection.FromJson<RestConnectionString>();
        }

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <value>The base URL.</value>
        public Uri BaseUrl => _connectionString.BaseUri;

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout => _connectionString.Timeout;

        /// <summary>
        /// Authenticates this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Task&lt;T&gt;.</returns>
        public async Task<T> Authenticate<T>()
        {
            return await _authProvider.Authenticate<T>(_connectionString);
        }

        /// <summary>
        /// Authenticates this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Task&lt;T&gt;.</returns>
        public T AuthenticateSync<T>()
        {
            return _authProvider.AuthenticateSync<T>(_connectionString);
        }
    }
}
