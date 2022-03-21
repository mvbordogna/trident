using System;
using System.Threading.Tasks;
using RestSharp.Authenticators;
using Trident.Rest;
using Trident.Rest.Contracts;

namespace Trident.Data.RestSharp
{
    /// <summary>
    /// Class RestSharpAuthenticationProvider.
    /// Implements the <see cref="TridentOptionsBuilder.Rest.RestAuthenticationProvider" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Rest.RestAuthenticationProvider" />
    /// <summary>
    /// Class RestSharpAuthenticationProvider.
    /// Implements the <see cref="TridentOptionsBuilder.Rest.RestAuthenticationProvider" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Rest.RestAuthenticationProvider" />
    public class RestSharpAuthenticationProvider : RestAuthenticationProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestSharpAuthenticationProvider" /> class.
        /// </summary>
        /// <param name="restClient">The rest client.</param>
        /// <summary>
        /// Initializes a new instance of the <see cref="RestSharpAuthenticationProvider"/> class.
        /// </summary>
        /// <param name="restClient">The rest client.</param>
        public RestSharpAuthenticationProvider(IRestClient restClient) : base(restClient)
        {
        }

        /// <summary>
        /// Performs the authenticate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        public override Task<T> PerformAuthenticate<T>(RestConnectionString connection)
        {
            return Task.FromResult(PerformAuthenticateSync<T>(connection));
        }

        public override T PerformAuthenticateSync<T>(RestConnectionString connection)
        {
            // HACK: there has to be a better way to do this that allows callers (e.g. RestSharpClient) to be able to rely on T at compile time
            if (!string.IsNullOrWhiteSpace(connection.ClientSecret))
            {
                var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(connection.ClientSecret);
                return (T)(object)authenticator;
            }

            if (!string.IsNullOrWhiteSpace(connection.CertificateThumbprint))
            {
                //TODO: add support for more auth types, e.g. OAuth2 JWT
                throw new NotSupportedException();
            }

            // anonymous authentication
            return (T)(object)null;
        }
    }
}
