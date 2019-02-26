using Trident.Data;
using Trident.EF6.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trident.Data.EntityFramework.Contracts;

namespace Trident.EF6
{
    /// <summary>
    /// Class EFDataContext.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    /// <seealso cref="Trident.EF6.Contracts.IEFDbContext" />
    /// <seealso cref="Trident.Data.IConnectionManager" />
    [DbConfigurationType(typeof(DynamicExecutionStrategyDbConfiguration))]
    public class EFDataContext : DbContext, IEFDbContext, IConnectionManager
    {
        /// <summary>
        /// The database model builder
        /// </summary>
        private readonly IDbModelBuilder _dbModelBuilder;

        /// <summary>
        /// Initializes static members of the <see cref="EFDataContext"/> class.
        /// </summary>
        static EFDataContext()
        {
            //Database.SetInitializer<EFDataContext>(null);
            //this sets the initializers, current one stops ef from altering the db on startup. 
            //Database.SetInitializer(new ContextInitializerNone<EFDataContext>());

            //this is the one that is currently being used by Trident
            // Database.SetInitializer(new CreateDatabaseIfNotExists<EFDataContext>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EFDataContext" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="dbModelBuilder">The database model builder.</param>
        public EFDataContext(IDbConnection connection,
            IDbModelBuilder dbModelBuilder) : base(connection.ConnectionString, dbModelBuilder.GetCompiled(connection))
        {
            //_dbModelBuilder = dbModelBuilder;

        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        public void Add<T>(T entity) where T : class
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
        public void Update<T>(T entity) where T : class
        {
            Console.WriteLine($"{entity.GetType().Name} {Entry(entity).State} ");
            if (base.Entry(entity).State == EntityState.Detached)
            {
                Set<T>().Attach(entity);
                Entry(entity).State = EntityState.Modified;
            }

        }

        /// <summary>
        /// find as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public async Task<T> FindAsync<T>(object id, bool detach=false) where T : class
        {
            Console.WriteLine($"Get: {typeof(T).Name}");
            var entity = await Set<T>().FindAsync(id);

            if (entity != null && detach)
            {
                base.Entry(entity).State = EntityState.Detached;
            }

            return entity;

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
            if(noTracking)
               query = query.AsNoTracking();

            return query;
        }

        /// <summary>
        /// Executes the procedure asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;IQueryable&lt;T&gt;&gt;.</returns>
        public Task<IQueryable<T>> ExecuteProcedureAsync<T>(string procedureName, bool noTracking = false, params IDbDataParameter[]  parameters)
             where T : class
        {
            var paramsString = String.Join(",", parameters.Select(x => x.ParameterName));

            var result = this.Database
                .SqlQuery<T>($"exec {procedureName} {paramsString}", parameters).AsQueryable();

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
        public override async Task<int> SaveChangesAsync()
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
            this.Database.Connection.Open();
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public void Close()
        {
            if (this.Database.Connection.State == ConnectionState.Open)
                this.Database.Connection.Close();
        }

        /// <summary>
        /// execute non query as an asynchronous operation.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async Task<int> ExecuteNonQueryAsync(string command, params object[] parameters)
        {
            return await this.Database.ExecuteSqlCommandAsync(command, parameters);
        }
    }
}
