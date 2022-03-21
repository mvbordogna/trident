using Trident.Data;
using Trident.Data.Contracts;
using Trident.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;

namespace Trident.EFCore
{
    /// <summary>
    /// Class EFCoreRepository.
    /// Implements the <see cref="TridentOptionsBuilder.Data.RepositoryBase{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.RepositoryBase{TEntity}" />
    public abstract class EFCoreRepository<TEntity> : RepositoryBase<TEntity>
        where TEntity : Entity
    {
        private readonly IQueryableHelper queryableHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        protected EFCoreRepository(IAbstractContextFactory abstractContextFactory, IQueryableHelper queryableHelper) :
            base(new Lazy<IContext>(() => abstractContextFactory.Create<IContext>(typeof(TEntity))))
        {
            this.queryableHelper = queryableHelper;
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
            var q = base.Context.Query<TEntity>(detach);
            var result = await queryableHelper.FirstOrDefaultAsync(Context, q, idExpression, detach);
           return result;
		
		}

        public override TEntity GetByIdSync(object id, bool detach = false)
        {
            var idExpression = TypeExtensions.CreateTypedCompareExpression<TEntity>(nameof(Entity.Id), id);
            var q = base.Context.Query<TEntity>(detach);
            return q.FirstOrDefault(idExpression);
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
            var q = base.Context.Query<TEntity>(detach)
                 .Where(x => ids.Contains((TEntityId)x.Id));

            return await queryableHelper.ToListAsync(Context, q, detach);            
        }

        public override IEnumerable<TEntity> GetByIdsSync<TEntityId>(IEnumerable<TEntityId> ids, bool detach = false)
        {
              var q = base.Context.Query<TEntity>(detach)
                 .Where(x => ids.Contains((TEntityId)x.Id));

            return queryableHelper.ToList(Context, q, detach);
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
                    .Aggregate(query, (current, includeProperty) => queryableHelper.Include(current, includeProperty));
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

             return await queryableHelper.ToListAsync(Context, query, noTracking);
        }


        public override IEnumerable<TEntity> GetSync(Expression<Func<TEntity, bool>> filter = null,
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
                    .Aggregate(query, (current, includeProperty) => queryableHelper.Include(current, includeProperty));
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return queryableHelper.ToList(Context, query, noTracking);
        }

        /// <summary>
        /// Returns a value indicating if any entity exists matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public override async Task<bool> Exists(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = base.Context.Query<TEntity>();
            return await queryableHelper.AnyAsync(query, filter);
        }

        public override bool ExistSync(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = base.Context.Query<TEntity>();
            return query.Any(filter);
        }

        /// <summary>
        /// Executes the procedure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;T&gt;&gt;.</returns>
        protected async Task<IEnumerable<T>> ExecuteProcedure<T>(string procedureName, params IDbDataParameter[] parameters)
            where T : class
        {
            return await base.Context.ExecuteProcedureAsync<T>(procedureName, false, parameters);
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

        protected IContext GetCurrentContext(IQueryable queryable)
        {
            var contextField = queryable.GetType().GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (IContext) contextField.GetValue(queryable);
            return result;
        }
    }

}
