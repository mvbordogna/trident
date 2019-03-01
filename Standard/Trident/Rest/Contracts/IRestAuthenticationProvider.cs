using System.Threading.Tasks;

namespace Trident.Rest.Contracts
{
    /// <summary>
    /// Interface IRestAuthenticationProvider
    /// </summary>
    public interface IRestAuthenticationProvider
    {
        /// <summary>
        /// Authenticates the specified connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> Authenticate<T>(RestConnectionString connection);

        /// <summary>
        /// Authenticates the specified connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        T AuthenticateSync<T>(RestConnectionString connection);

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets the authentication context.
        /// </summary>
        /// <value>The authentication context.</value>
        object AuthenticationContext { get; }
    }
}