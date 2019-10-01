using Microsoft.EntityFrameworkCore;
using Trident.EFCore.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Trident.EFCore
{

    /// <summary>
    /// Class EFCoreDataContext.
    /// Implements the <see cref="Microsoft.EntityFrameworkCore.DbContext" />
    /// Implements the <see cref="Trident.Data.EntityFramework.Contracts.IEFDbContext" />
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    /// <seealso cref="Trident.Data.EntityFramework.Contracts.IEFDbContext" />
    public class EFCoreDataContext : DbContext, IEFDbContext
    {
        /// <summary>
        /// The model builder factory
        /// </summary>
        private readonly IEFCoreModelBuilderFactory _modelBuilderFactory;
        private readonly IEntityMapFactory mapFactory;

        /// <summary>
        /// The data source
        /// </summary>
        private readonly string _dataSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreDataContext"/> class.
        /// </summary>
        /// <param name="modelBuilderFactory">The model builder factory.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="options">The options.</param>
        public EFCoreDataContext(IEFCoreModelBuilderFactory modelBuilderFactory, IEntityMapFactory mapFactory, string dataSource, DbContextOptions options)
            : base(options)
        {
            _modelBuilderFactory = modelBuilderFactory;
            this.mapFactory = mapFactory;
            _dataSource = dataSource;
        }

        /// <summary>
        /// <para>
        /// Override this method to configure the database (and other options) to be used for this context.
        /// This method is called for each instance of the context that is created.
        /// The base implementation does nothing.
        /// </para>
        /// <para>
        /// In situations where an instance of <see cref="T:Microsoft.EntityFrameworkCore.DbContextOptions" /> may or may not have been passed
        /// to the constructor, you can use <see cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured" /> to determine if
        /// the options have already been set, and skip some or all of the logic in
        /// <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" />.
        /// </para>
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context. Databases (and other extensions)
        /// typically define extension methods on this object that allow you to configure the context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run.</remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var modelAppender =  _modelBuilderFactory.GetBuilder(_dataSource);
            modelAppender.AppendModelMappings(modelBuilder, mapFactory);           
            base.OnModelCreating(modelBuilder);
        }
        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        public new void Add<T>(T entity) where T : class
        {
            Console.WriteLine($"{entity.GetType().Name} {Entry(entity).State} ");
            Set<T>().Add(entity);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        public void Delete<T>(T entity) where T : class
        {
            Console.WriteLine($"{entity.GetType().Name} {Entry(entity).State} ");
            if (base.Entry(entity).State == EntityState.Detached)
            {
                Set<T>().Attach(entity);
            }
            Set<T>().Remove(entity);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        public new void Update<T>(T entity) where T : class
        {
            Console.WriteLine($"{entity.GetType().Name} {Entry(entity).State} ");
            if (base.Entry(entity).State == EntityState.Detached)
            {
                Set<T>().Update(entity);
                //Entry(entity).State = EntityState.Modified;
            }
        }

        public T Find<T>(object id, bool detach = false) where T : class
        {
            Console.WriteLine($"Get: {typeof(T).Name}");
            var entity = base.Find<T>(id);

            if (entity != null && detach)
            {
                base.Entry(entity).State = EntityState.Detached;
            }

            return entity;
        }

        /// <summary>
        /// find as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public virtual async Task<T> FindAsync<T>(object id, bool detach = false) where T : class
        {
            Console.WriteLine($"Get: {typeof(T).Name}");
            var entity = await base.FindAsync<T>(id);

            if (entity != null && detach)
            {
                base.Entry(entity).State = EntityState.Detached;
            }

            return await Task.FromResult(entity);

        }

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        public IQueryable<T> Query<T>(bool noTracking = false) where T : class
        {
            Console.WriteLine($"Query: {typeof(T).Name}");
            IQueryable<T> query = Set<T>();
            if (noTracking) 
                query = query.AsNoTracking();
              

            return query;
        }

        public IQueryable<T> ExecuteProcedure<T>(string procedureName, bool noTracking = false, params IDbDataParameter[] parameters)
           where T : class
        {
            var paramsString = String.Join(",", parameters.Select(x => x.ParameterName));
            var result = Set<T>().FromSqlRaw($"exec {procedureName} {paramsString}", parameters);

            if (!noTracking)
            {
                foreach (var item in result)
                {
                    this.Set<T>().Attach(item);
                }
            }

            return result;
        }


        /// <summary>
        /// Executes the procedure asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;IQueryable&lt;T&gt;&gt;.</returns>
        public Task<IQueryable<T>> ExecuteProcedureAsync<T>(string procedureName, bool noTracking = false, params IDbDataParameter[] parameters)
             where T : class
        {
            var paramsString = String.Join(",", parameters.Select(x => x.ParameterName));

            var result = Set<T>().FromSqlRaw($"exec {procedureName} {paramsString}", parameters);

            if (!noTracking)
            {
                foreach (var item in result)
                {
                    this.Set<T>().Attach(item);
                }
            }

            return Task.FromResult(result);
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
        public virtual async Task<int> SaveChangesAsync()
        {
            if (ChangeTracker.HasChanges())
            {
                var result = await SaveChangesAsync(CancellationToken.None);
                Console.WriteLine(result);
                return result;
            }
            return 0;

        }

        /// <summary>
        /// Opens a connection to the database.
        /// </summary>
        public void Open()
        {
            this.Database.OpenConnection();
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public void Close()
        {
            this.Database.CloseConnection();
        }

        /// <summary>
        /// execute non query as an asynchronous operation.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public virtual async Task<int> ExecuteNonQueryAsync(string command, params object[] parameters)
        {
            return await this.Database.ExecuteSqlCommandAsync(command, parameters);
        }

        public int ExecuteNonQuery(string command, params object[] parameters)
        {
            return this.Database.ExecuteSqlCommand(command, parameters);
        }
    }

    
}
