using Microsoft.EntityFrameworkCore;
using Trident.EFCore.Contracts;
using System;
using System.Threading.Tasks;

namespace Trident.EFCore.AsyncWorkaround
{
    /// <summary>
    /// UPDATE: 3.0.x preview still has issues with the async implemenation
    /// After the next release of EFCore 2.2.0+, this class should be removed
    /// it is only here as an async shim until async works for Cosmos.Sql db Provider of EFCore
    /// note that Async works for Sql Server Provider and this is not needed for thos db contexts
    /// Workaround repo bases have been added as well.
    /// Implements the <see cref="Trident.Data.EntityFramework.EFCore.EFCoreDataContext" />
    /// </summary>
    /// <seealso cref="Trident.Data.EntityFramework.EFCore.EFCoreDataContext" />
    public class EFCoreAsyncWorkAroundDbContext : EFCoreDataContext
    {
        /// <summary>
        /// The model builder factory
        /// </summary>
        private readonly IEFCoreModelBuilderFactory _modelBuilderFactory;
        /// <summary>
        /// The data source
        /// </summary>
        private readonly string _dataSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreAsyncWorkAroundDbContext"/> class.
        /// </summary>
        /// <param name="modelBuilderFactory">The model builder factory.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="options">The options.</param>
        public EFCoreAsyncWorkAroundDbContext(IEFCoreModelBuilderFactory modelBuilderFactory, IEntityMapFactory mapperFactory, string dataSource, DbContextOptions options)
            : base(modelBuilderFactory, mapperFactory, dataSource,  options)
        {
            _modelBuilderFactory = modelBuilderFactory;
            _dataSource = dataSource;
        }


        /// <summary>
        /// find as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public async override Task<T> FindAsync<T>(object id, bool detach = false) 
        {
            Console.WriteLine($"Get: {typeof(T).Name}");
            var entity = base.Find<T>(id);

            if (entity != null && detach)
            {
                base.Entry(entity).State = EntityState.Detached;
            }

            return await Task.FromResult(entity);

        }

        /// <summary>
        /// save changes as an asynchronous operation.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation.
        /// The task result contains the number of state entries written to the underlying database. This can include
        /// state entries for entities and/or relationships. Relationship state entries are created for
        /// many-to-many relationships and relationships where there is no foreign key property
        /// included in the entity class (often referred to as independent associations).</returns>
        /// <remarks>Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        /// that any asynchronous operations have completed before calling another method on this context.</remarks>
        public override async Task<int> SaveChangesAsync()
        {
            if (ChangeTracker.HasChanges())
            {
                var result = base.SaveChanges();
                Console.WriteLine(result);
                return await Task.FromResult(result);
            }
            return await Task.FromResult(0);
        }


        /// <summary>
        /// execute non query as an asynchronous operation.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public override async Task<int> ExecuteNonQueryAsync(string command, params object[] parameters)
        {
            var result = this.Database.ExecuteSqlCommand(command, parameters);
            return await Task.FromResult(result);
        }
    }
}
