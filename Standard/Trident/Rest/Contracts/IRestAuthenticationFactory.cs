using Trident.Contracts.Enums;

namespace Trident.Rest.Contracts
{
    /// <summary>
    /// Interface IRestAuthenticationFactory
    /// </summary>
    public interface IRestAuthenticationFactory
    {
        /// <summary>
        /// Gets the authentication provider.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>IRestAuthenticationProvider.</returns>
        IRestAuthenticationProvider GetAuthenticationProvider(string dataSource);
    }
}