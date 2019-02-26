using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trident;
using Trident.Data;
using Trident.Data.Contracts;
using Trident.Domain;

namespace Trident.EF6
{
    /// <summary>
    /// Class EFRepository.
    /// Implements the <see cref="Trident.Data.RepositoryBase{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Data.RepositoryBase{TEntity}" />
    public abstract class EFRepository<TEntity> : RepositoryBase<TEntity> 
        where TEntity: Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EFRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        protected EFRepository(IAbstractContextFactory abstractContextFactory):base(new System.Lazy<IContext>(()=> abstractContextFactory.Create<IContext>(typeof(TEntity))))
        {
           
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        public override async Task<TEntity> GetById(object id, bool detach = false)
        {           
            return await base.Context.Query<TEntity>(detach)
                .FirstOrDefaultAsync(TypeExtensions.CreateTypedCompareExpression<TEntity>(nameof(Entity.Id), id));
        }

        /// <summary>
        /// Gets the by ids.
        /// </summary>
        /// <typeparam name="TEntityId">The type of the t entity identifier.</typeparam>
        /// <param name="ids">The ids.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
        public override async Task<IEnumerable<TEntity>> GetByIds<TEntityId>(IEnumerable<TEntityId> ids, bool detach = false)
        {
           return await base.Context.Query<TEntity>(detach)
                .Where(x => ids.Contains((TEntityId)x.Id))
                .ToListAsync();
        }

        /// <summary>
        /// Gets the entities matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
        public override async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            IEnumerable<string> includeProperties = null, bool noTracking=false)
        {
            IQueryable<TEntity> query = base.Context.Query<TEntity>(noTracking);

            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            if (includeProperties != null)
            {
                query = includeProperties
                    .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Returns a value indicating if any entity exists matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public override async Task<bool> Exists(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = base.Context.Query<TEntity>();            
            return await query.AnyAsync(filter);
        }

        /// <summary>
        /// Executes the procedure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;T&gt;&gt;.</returns>
        protected async Task<IEnumerable<T>> ExecuteProcedure<T>(string procedureName, bool noTracking = false, params IDbDataParameter[] parameters)
            where T: class
        {
            return await base.Context.ExecuteProcedureAsync<T>(procedureName, noTracking, parameters);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        protected async Task<int> ExecuteNonQuery(string command, params object[] parameters)
        {
            return await base.Context.ExecuteNonQueryAsync(command, parameters);
        }


    }
}
