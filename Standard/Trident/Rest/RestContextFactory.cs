using System;
using Trident.Contracts.Enums;
using Trident.Data.Contracts;
using Trident.IoC;
using Trident.Rest.Contracts;

namespace Trident.Rest
{
    /// <summary>
    /// Class RestContextFactory.
    /// Implements the <see cref="Trident.Data.Contracts.ISharedContextFactory{Trident.Rest.Contracts.IRestContext}" />
    /// </summary>
    /// <seealso cref="Trident.Data.Contracts.ISharedContextFactory{Trident.Rest.Contracts.IRestContext}" />
    public class RestContextFactory : ISharedContextFactory<IRestContext>
    {
        /// <summary>
        /// The rest connection string resolver
        /// </summary>
        private readonly IRestConnectionStringResolver _restConnectionStringResolver;
        /// <summary>
        /// The ioc service locator
        /// </summary>
        private readonly IIoCServiceLocator _iocServiceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestContextFactory"/> class.
        /// </summary>
        /// <param name="restConnectionStringResolver">The rest connection string resolver.</param>
        /// <param name="iocServiceLocator">The ioc service locator.</param>
        public RestContextFactory(IRestConnectionStringResolver restConnectionStringResolver,
            IIoCServiceLocator iocServiceLocator)
        {
            _restConnectionStringResolver = restConnectionStringResolver;
            _iocServiceLocator = iocServiceLocator;
        }

        /// <summary>
        /// Gets the specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="dataSource">The data source.</param>
        /// <returns>IRestContext.</returns>
        public IRestContext Get(Type entityType, string dataSource)
        {
            var connection = _restConnectionStringResolver.GetConnection(dataSource);

            var args = new[]
            {
                new Parameter(typeof (IRestConnection), connection)
            };

            return _iocServiceLocator.GetNamed<IRestContext>(dataSource.ToString(), args);
        }
    }
}
