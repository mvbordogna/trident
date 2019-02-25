using Microsoft.EntityFrameworkCore;
using Trident;
using Trident.Data.Contracts;
using Trident.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Trident.Data.EntityFramework.EFCore.AsyncWorkaround
{
    /// <summary>
    /// Class EFCoreAsyncCosmosWorkaroundRepository.
    /// Implements the <see cref="Trident.Data.EntityFramework.EFCore.EFCoreRepository{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Data.EntityFramework.EFCore.EFCoreRepository{TEntity}" />
    public abstract class EFCoreAsyncCosmosWorkaroundRepository<TEntity> : EFCoreRepository<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreAsyncCosmosWorkaroundRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        protected EFCoreAsyncCosmosWorkaroundRepository(IAbstractContextFactory abstractContextFactory) :
            base(abstractContextFactory)
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
            var idExpression = TypeExtensions.CreateTypedCompareExpression<TEntity>(nameof(Entity.Id), id);           
            var result = base.Context.Query<TEntity>(detach)
                .FirstOrDefault(idExpression);
            return await Task.FromResult(result);
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
            var result = base.Context.Query<TEntity>(detach)
                 .Where(x => ids.Contains((TEntityId)x.Id))
                 .ToList();

            return await Task.FromResult(result);
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
            IEnumerable<string> includeProperties = null, bool noTracking = false)
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

            var result = query.ToList();
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Returns a value indicating if any entity exists matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public override async Task<bool> Exists(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = base.Context.Query<TEntity>();
            var result = query.Any(filter);
            return await Task.FromResult(result);
        }
    }
}
