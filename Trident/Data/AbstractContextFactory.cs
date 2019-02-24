using System;
using System.Reflection;
using Trident.Contracts.Enums;
using Trident.Data.Contracts;
using Trident.IoC;

namespace Trident.Data
{
    /// <summary>
    /// Implementation of AbstractContextFactory that allows for the implemenation of concrete context 
    /// factories that are data provider specific.
    /// Implements the <see cref="Trident.Data.Contracts.IAbstractContextFactory" />
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="Trident.Data.Contracts.IAbstractContextFactory" />
    /// <seealso cref="System.IDisposable" />
    /// <remarks>This class maintains a cache of context objects. Generally, this class is 
    /// registered with the DI container as a per request or per lifetime scoped, so that Context objects 
    /// like EF6/EFCore, which are not thead-safe and may create memory pressure issue are disposed of after
    /// a completed request.</remarks>
    public class AbstractContextFactory : IAbstractContextFactory, IDisposable
    {
        /// <summary>
        /// The service locator
        /// </summary>
        private readonly IIoCServiceLocator _serviceLocator;
        /// <summary>
        /// The context cache
        /// </summary>
        private readonly System.Collections.Concurrent.ConcurrentDictionary<int, object> _contextCache = new System.Collections.Concurrent.ConcurrentDictionary<int, object>();

        /// <summary>
        /// The pad lock
        /// </summary>
        private readonly object padLock = new object();
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractContextFactory"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public AbstractContextFactory(IIoCServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;         
        }


        /// <summary>
        /// Creates the specified Context type based on the entity attribute or default configuration of its data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>T.</returns>
        public T Create<T>(Type entityType)
        {
            T context = default(T);
            var attr = entityType.GetCustomAttribute<UseSharedDataSourceAttribute>();

            var dataSourceName = (attr != null && attr.DataSource != SharedDataSource.Undefined) ? attr.DataSource : SharedDataSource.DefaultDB;
            int contextHash = dataSourceName.GetHashCode();

            context = (T)_contextCache.GetOrAdd(contextHash, (hash) =>
            {
                var factory = (ISharedContextFactory<T>)_serviceLocator.GetNamed<ISharedContextFactory>(dataSourceName.ToString());
                return factory.Get(entityType, dataSourceName);
            });

            return context;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //autofac will dispose the items in the list
            //just want to clear the refs.
            _contextCache?.Clear();
        }
    }
}
