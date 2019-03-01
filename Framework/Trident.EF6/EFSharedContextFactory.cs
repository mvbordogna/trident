using System;
using System.Data;
using Trident.Contracts.Enums;
using Trident.Data.Contracts;
using Trident.IoC;
using Trident.EF6.Contracts;
using Trident.Data.EntityFramework.Contracts;

namespace Trident.EF6
{
    /// <summary>
    /// Class EFSharedContextFactory.
    /// Implements the <see cref="Trident.Data.Contracts.ISharedContextFactory{Trident.Data.Contracts.IContext}" />
    /// </summary>
    /// <seealso cref="Trident.Data.Contracts.ISharedContextFactory{Trident.Data.Contracts.IContext}" />
    public class EFSharedContextFactory : ISharedContextFactory<IContext>
    {
        /// <summary>
        /// The shared connection string resolver
        /// </summary>
        private readonly ISharedConnectionStringResolver _sharedConnectionStringResolver;
        /// <summary>
        /// The model builder factory
        /// </summary>
        private readonly IModelBuilderFactory _modelBuilderFactory;
        /// <summary>
        /// The ioc service locator
        /// </summary>
        private readonly IIoCServiceLocator _iocServiceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFSharedContextFactory"/> class.
        /// </summary>
        /// <param name="sharedConnectionStringResolver">The shared connection string resolver.</param>
        /// <param name="modelBuilderFactory">The model builder factory.</param>
        /// <param name="iocServiceLocator">The ioc service locator.</param>
        public EFSharedContextFactory(ISharedConnectionStringResolver sharedConnectionStringResolver,
            IModelBuilderFactory modelBuilderFactory,
            IIoCServiceLocator iocServiceLocator)
        {
            _sharedConnectionStringResolver = sharedConnectionStringResolver;
            _modelBuilderFactory = modelBuilderFactory;
            _iocServiceLocator = iocServiceLocator;
        }

        /// <summary>
        /// Gets the specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="dataSource">The data source.</param>
        /// <returns>IContext.</returns>
        public IContext Get(Type entityType, string dataSource)
        {
            var connection = _sharedConnectionStringResolver.GetConnection(dataSource);
            var builder = _modelBuilderFactory.Get(dataSource.ToString());
            var args = new []
            {
                new Parameter(typeof (IDbConnection), connection),
                new Parameter(typeof (IDbModelBuilder), builder)
            };
            return _iocServiceLocator.GetNamed<IEFDbContext>(dataSource.ToString(), args);
        }
    }
}
