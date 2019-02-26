using Microsoft.EntityFrameworkCore;
using Trident.Contracts.Enums;
using Trident.Data.Contracts;
using Trident.IoC;
using Trident.Data.EntityFramework.Contracts;
using Trident.EFCore.Contracts;
using System;

namespace Trident.EFCore
{
    /// <summary>
    /// Class EFCoreTenantContextFactory.
    /// Implements the <see cref="Trident.Data.Contracts.ISharedContextFactory{Trident.Data.Contracts.IContext}" />
    /// </summary>
    /// <seealso cref="Trident.Data.Contracts.ISharedContextFactory{Trident.Data.Contracts.IContext}" />
    /// <seealso cref="ExakTime.SaaS.Core.Data.Contracts.ITenantContextFactory" />
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
        public IContext Get(Type entityType, SharedDataSource dataSource)
        {           
            var optionsBuilderfactory = _serviceLocater.Get<IOptionsFactory>();
            var options  = optionsBuilderfactory.GetOptions(dataSource.ToString());

            var args = new[]
            {
                new Parameter(typeof (IEFCoreModelBuilderFactory), _modelBuidlerFactory),
                new Parameter(typeof (string), dataSource.ToString()),
                new Parameter(typeof (DbContextOptions), options)            
            };
            return _serviceLocater.GetNamed<IEFDbContext>(dataSource.ToString(), args);
        }
    }
}
