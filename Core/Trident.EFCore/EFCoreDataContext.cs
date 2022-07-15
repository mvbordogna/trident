using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trident.Common;
using Trident.Contracts.Configuration;
using Trident.Data;
using Trident.Data.Contracts;
using Trident.Domain;
using Trident.EFCore.Contracts;
using Trident.Extensions;
using Trident.IoC;
using Trident.Logging;

namespace Trident.EFCore
{
    /// <summary>
    /// Class EFCoreDataContext.
    /// Implements the <see cref="Microsoft.EntityFrameworkCore.DbContext" />
    /// Implements the <see cref="IEFDbContext" />
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    /// <seealso cref="IEFDbContext" />
    public class EFCoreDataContext : DbContext, IEFDbContext
    {
        /// <summary>
        /// The model builder factory
        /// </summary>
        protected IEFCoreModelBuilderFactory ModelBuilderFactory { get; }
        private readonly IEntityMapFactory _mapFactory;
        protected readonly ILog _log;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IAppSettings _appSettings;
        protected DbContextOptions Options { get; }
        protected IIoCServiceLocator IoCServiceLocator { get; }

        /// <summary>
        /// The data source
        /// </summary>
        protected string DataSource { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreDataContext"/> class.
        /// </summary>
        /// <param name="modelBuilderFactory">The model builder factory.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="options">The options.</param>
        public EFCoreDataContext(
            IEFCoreModelBuilderFactory modelBuilderFactory,
            IEntityMapFactory mapFactory,
            string dataSource,
            DbContextOptions options, ILog log,
            ILoggerFactory loggerFactory,
            IAppSettings appSettings,
            IIoCServiceLocator ioCServiceLocator)
            : base(options)
        {
            ModelBuilderFactory = modelBuilderFactory;
            _mapFactory = mapFactory;
            DataSource = dataSource;
            _log = log;
            _loggerFactory = loggerFactory;
            _appSettings = appSettings;
            IoCServiceLocator = ioCServiceLocator;
            Options = options;
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
            if (bool.Parse(_appSettings.GetKeyOrDefault("EFLoggingEnabled", "false")))
                optionsBuilder.UseLoggerFactory(this._loggerFactory);

            //optionsBuilder.EnableSensitiveDataLogging();
            //optionsBuilder.EnableDetailedErrors();
            //optionsBuilder.ConfigureWarnings((warnings) =>
            //{
            //    warnings.Default(WarningBehavior.Log)
            //        .Log(CoreEventId.PropertyChangeDetected);
            //});

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
            var modelAppender = ModelBuilderFactory.GetBuilder(DataSource);
            modelAppender.AppendModelMappings(modelBuilder, _mapFactory);
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        public new void Add<T>(T entity) where T : class
        {
            Set<T>().Add(entity);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        public void Delete<T>(T entity) where T : class
        {
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
            var e = base.Entry(entity);
            if (e.State == EntityState.Detached)
            {
                if (entity is Entity e1)
                {
                    try
                    {
                        var entry = ChangeTracker.Entries<T>()
                            .FirstOrDefault(t => t.Entity is Entity t1 && t1.Id.Equals(e1.Id));


                        if (entry != null)
                        {
                            entry.State = EntityState.Detached;
                            Set<T>().Update(entity);
                        }
                        else
                        {
                            Replace(e.Entity, entity);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _log.Error(
                            exception: ex,
                            messageTemplate: String.Join(", ", ex.ExpandException().Select(e => e.Message)));
                        Set<T>().Update(entity);
                    }
                }
                else
                {
                    Set<T>().Update(entity);
                }
            }
        }

        public T Find<T>(object id, bool detach = false) where T : class
        {
            var result = FindImpl<T>(id, detach, false);
            return result;
        }

        private T FindImpl<T>(object id, bool detach, bool isTransient) where T : class
        {
            if (!detach || isTransient)
            {
                var result = base.Find<T>(id);
                MapDynamicObjects(result);
                return result;
            }
            else
            {
                var tmpContext = (EFCoreDataContext)Clone();
                var result = tmpContext.FindImpl<T>(id, false, true);
                return result;
            }
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
            var result = await FindAsyncImpl<T>(id, detach, false);
            return result;
        }

        private async Task<T> FindAsyncImpl<T>(object id, bool detach, bool isTransient) where T : class
        {
            if (!detach || isTransient)
            {
                var result = await base.FindAsync<T>(id);
                MapDynamicObjects(result);
                return result;
            }
            else
            {
                var tmpContext = (EFCoreDataContext)Clone();
                var result = await tmpContext.FindAsyncImpl<T>(id, false, true);
                return result;
            }
        }

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        public IQueryable<T> Query<T>(bool noTracking = false) where T : class
        {
            //IQueryable<T> query = Set<T>();
            //if (noTracking)
            //    query = query.AsNoTracking();

            //return query;

            var result = QueryImpl<T>(noTracking);
            return result;
        }

        private IQueryable<T> QueryImpl<T>(bool noTracking) where T : class
        {
            if (!noTracking)
            {
                var result = Set<T>();
                return result;
            }
            else
            {
                var tmpContext = (EFCoreDataContext)Clone();
                var result = tmpContext.QueryImpl<T>(false);
                return result;
            }
        }

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        public IQueryable<T> QueryRaw<T>(string queryString, bool noTracking = false, params object[] parameters) where T : class
        {
            //IQueryable<T> query = Set<T>().FromSqlRaw(queryString, parameters);
            //if (noTracking)
            //    query = query.AsNoTracking();
            //return query;

            var result = QueryRawImpl<T>(queryString, noTracking, parameters, false);
            return result;
        }

        private IQueryable<T> QueryRawImpl<T>(string queryString, bool noTracking, object[] parameters, bool isTransient) where T : class
        {
            if (!noTracking || isTransient)
            {
                var result = RelationalQueryableExtensions.FromSqlRaw(Set<T>(), queryString, parameters);
                return result;
            }
            else
            {
                var tmpContext = (EFCoreDataContext)Clone();
                var result = tmpContext.QueryRawImpl<T>(queryString, false, parameters, true);
                return result;
            }
        }

        public IQueryable<T> ExecuteProcedure<T>(string procedureName, bool noTracking = false, params IDbDataParameter[] parameters)
           where T : class
        {
            var result = ExecuteProcedureImpl<T>(procedureName, noTracking, parameters);
            return result;
        }

        private IQueryable<T> ExecuteProcedureImpl<T>(
            string procedureName, bool noTracking = false,
            params IDbDataParameter[] parameters) where T : class
        {
            if (!noTracking)
            {
                var paramsString = String.Join(",", parameters.Select(x => x.ParameterName));
                var result = RelationalQueryableExtensions.FromSqlRaw(Set<T>(), $"exec {procedureName} {paramsString}", parameters);

                foreach (var item in result)
                    Set<T>().Attach(item);

                return result;
            }
            else
            {
                var tmpContext = (EFCoreDataContext)Clone();
                var result = tmpContext.ExecuteProcedureImpl<T>(procedureName, false, parameters);
                return result;
            }
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
            var result = ExecuteProcedureAsyncImpl<T>(procedureName, noTracking, parameters);
            return result;
        }

        private Task<IQueryable<T>> ExecuteProcedureAsyncImpl<T>(
            string procedureName, bool noTracking = false,
            params IDbDataParameter[] parameters) where T : class
        {
            if (!noTracking)
            {
                var paramsString = String.Join(",", parameters.Select(x => x.ParameterName));
                var result = RelationalQueryableExtensions.FromSqlRaw(Set<T>(), $"exec {procedureName} {paramsString}", parameters);

                foreach (var item in result)
                    Set<T>().Attach(item);

                return Task.FromResult(result);
            }
            else
            {
                var tmpContext = (EFCoreDataContext)Clone();
                var result = tmpContext.ExecuteProcedureAsyncImpl<T>(procedureName, false, parameters);
                return result;
            }
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
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _log.Debug(messageTemplate: $"Calling {nameof(SaveChangesAsync)} on context:" +
                $" {this.ContextId} with provider: {this.Database.ProviderName}");
            if (ChangeTracker.HasChanges())
            {
                _log.Debug(messageTemplate: $"HasChanges");
                var result = await base.SaveChangesAsync(cancellationToken);
                _log.Debug(messageTemplate: $"SaveChangesResult: {result}");
                return result;
            }
            return 0;

        }
        public virtual async Task<int> SaveChangesAsync<TEntity>(CancellationToken cancellationToken = default)
            where TEntity : class
        {
            _log.Debug(messageTemplate: $"Calling {nameof(SaveChangesAsync)} on context:" +
                $" {this.ContextId} with provider: {this.Database.ProviderName}");
            if (ChangeTracker.HasChanges())
            {
                _log.Debug(messageTemplate: $"HasChanges");

                var result = await base.SaveChangesAsync(cancellationToken);
                _log.Debug(messageTemplate: $"SaveChangesResult: {result}");
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
            return await this.Database.ExecuteSqlRawAsync(command, parameters);
        }

        public int ExecuteNonQuery(string command, params object[] parameters)
        {
            return this.Database.ExecuteSqlRaw(command, parameters);
        }

        public void Replace<TEntity>(TEntity oldEntity, TEntity newEntity) where TEntity : class
        {
            ChangeTracker.TrackGraph(newEntity, e =>
            {
                e.Entry.State = e.Entry.IsKeySet ? EntityState.Modified : EntityState.Added;
            });
        }

        public virtual IDbClient<T> GetDbClient<T>()
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(string command, IDictionary<string, object> parameters) where TEntity : class
        {
            throw new NotImplementedException();
        }

        protected object Clone()
        {
            return (IContext)Activator.CreateInstance(this.GetType(), new object[] {
               ModelBuilderFactory,
               IoCServiceLocator.Get<IEntityMapFactory>(),
               DataSource,
               Options,
               IoCServiceLocator.Get<ILog>(),
               IoCServiceLocator.Get<Microsoft.Extensions.Logging.ILoggerFactory>(),
               IoCServiceLocator.Get<IAppSettings>(),
               IoCServiceLocator
            });
        }

        public virtual void MapDynamicObjects<TEntity>(TEntity result) where TEntity : class { }
    }
}
