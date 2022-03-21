using Microsoft.EntityFrameworkCore;
using System;
using Trident.Data.Contracts;
using Trident.EFCore.Contracts;
using Trident.IoC;

namespace Trident.EFCore
{
    /// <summary>
    /// Class EFCoreTenantContextFactory.
    /// Implements the <see cref="TridentOptionsBuilder.Data.Contracts.ISharedContextFactory{TridentOptionsBuilder.Data.Contracts.IContext}" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.ISharedContextFactory{TridentOptionsBuilder.Data.Contracts.IContext}" />
    public class EFCoreSharedContextFactory : ISharedContextFactory<IContext>
    {
        /// <summary>
        /// The service locater
        /// </summary>
        private readonly IIoCServiceLocator _serviceLocater;
        /// <summary>
        /// The model buidler factory
        /// </summary>
        private readonly IEFCoreModelBuilderFactory _modelBuidlerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreSharedContextFactory"/> class.
        /// </summary>
        /// <param name="serviceLocater">The service locater.</param>
        /// <param name="modelBuidlerFactory">The model buidler factory.</param>
        public EFCoreSharedContextFactory(IIoCServiceLocator serviceLocater, IEFCoreModelBuilderFactory modelBuidlerFactory)
        {
            _serviceLocater = serviceLocater;
            _modelBuidlerFactory = modelBuidlerFactory;
        }

        /// <summary>
        /// Gets the specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="dataSource">The data source.</param>
        /// <returns>IContext.</returns>
        public IContext Get(Type entityType, string dataSource)
        {
            var optionsBuilderfactory = _serviceLocater.Get<IOptionsFactory>();
            var options = optionsBuilderfactory.GetOptions(dataSource);

            var args = new[]
            {
                new Parameter(typeof (IEFCoreModelBuilderFactory), _modelBuidlerFactory),
                new Parameter(typeof (string), dataSource),
                new Parameter(typeof (DbContextOptions), options)
            };
            return _serviceLocater.GetNamed<IEFDbContext>(dataSource, args);
            /*  return (IContext) Activator.CreateInstance(options.ContextType, new object[] {
                  _modelBuidlerFactory,
                   _serviceLocater.Get<IEntityMapFactory>(),
                   dataSource,
                   options,
                   _serviceLocater.Get<ILoggerAdapter>(),
                  _serviceLocater.Get<Microsoft.Extensions.Logging.ILoggerFactory>(),
                   _serviceLocater.Get<IAppSettings>(),
                   _serviceLocater
              }); */
        }
    }
}
