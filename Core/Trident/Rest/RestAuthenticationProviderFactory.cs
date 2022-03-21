using Trident.Contracts.Enums;
using Trident.IoC;
using Trident.Rest.Contracts;

namespace Trident.Rest
{
    /// <summary>
    /// Class RestAuthenticationProviderFactory.
    /// Implements the <see cref="TridentOptionsBuilder.Rest.Contracts.IRestAuthenticationFactory" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Rest.Contracts.IRestAuthenticationFactory" />
    public class RestAuthenticationProviderFactory : IRestAuthenticationFactory
    {
        /// <summary>
        /// The service locator
        /// </summary>
        private readonly IIoCServiceLocator _serviceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestAuthenticationProviderFactory"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public RestAuthenticationProviderFactory(IIoCServiceLocator  serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Gets the authentication provider.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>IRestAuthenticationProvider.</returns>
        public IRestAuthenticationProvider GetAuthenticationProvider(string dataSource)
        {
            return _serviceLocator.GetNamed<IRestAuthenticationProvider>(dataSource);
        }
    }
}
