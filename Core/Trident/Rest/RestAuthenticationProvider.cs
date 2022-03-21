using System;
using System.Threading.Tasks;
using Trident.Rest.Contracts;

namespace Trident.Rest
{
    /// <summary>
    /// Class RestAuthenticationProvider.
    /// Implements the <see cref="TridentOptionsBuilder.Rest.Contracts.IRestAuthenticationProvider" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Rest.Contracts.IRestAuthenticationProvider" />
    public abstract class RestAuthenticationProvider : IRestAuthenticationProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestAuthenticationProvider"/> class.
        /// </summary>
        /// <param name="restClient">The rest client.</param>
        protected RestAuthenticationProvider(IRestClient restClient)
        {
            RestClient = restClient;
        }

        /// <summary>
        /// Gets the rest client.
        /// </summary>
        /// <value>The rest client.</value>
        protected IRestClient RestClient { get; }

        /// <summary>
        /// Authenticates the specified connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public async Task<T> Authenticate<T>(RestConnectionString connection)
        {
            if (!IsAuthenticated)
            {
                AuthenticationContext = await PerformAuthenticate<T>(connection);
                IsAuthenticated = true;
            }

            return (T)AuthenticationContext;
        }



        /// <summary>
        /// Authenticates the specified connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public T AuthenticateSync<T>(RestConnectionString connection)
        {
            if (!IsAuthenticated)
            {
                AuthenticationContext = PerformAuthenticateSync<T>(connection);
                IsAuthenticated = true;
            }

            return (T)AuthenticationContext;
        }


        /// <summary>
        /// Performs the authenticate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public abstract Task<T> PerformAuthenticate<T>(RestConnectionString connection);

        /// <summary>
        /// Performs the authenticate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns>Task&lt;T&gt;.</returns>

        public abstract T PerformAuthenticateSync<T>(RestConnectionString connection);

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Gets the authentication context.
        /// </summary>
        /// <value>The authentication context.</value>
        public object AuthenticationContext { get; private set; }
    }
}
