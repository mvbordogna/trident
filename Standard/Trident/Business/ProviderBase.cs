using System;
using System.Linq;
using System.Linq.Expressions;
using Trident.Data.Contracts;
using Trident.Search;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Trident.Business
{
    /// <summary>
    /// Class ProviderBase.
    /// Implements the <see cref="Trident.Business.ReadOnlyProviderBase{TEntity, TSummary, TCriteria}" />
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="Trident.Business.ReadOnlyProviderBase{TEntity, TSummary, TCriteria}" />
    /// <seealso cref="Trident.Data.Contracts.IProvider{TId, TEntity, TSummary, TCriteria}" />
    /// <seealso cref="Trident.Data.Contracts.IProvider{TEntity, TSummary, TCriteria}" />
    public abstract class ProviderBase<TId, TEntity, TSummary, TCriteria> : ReadOnlyProviderBase<TEntity, TSummary, TCriteria>, IProvider<TId, TEntity, TSummary, TCriteria>
        where TEntity : Domain.EntityBase<TId>
        where TSummary : Domain.Entity
        where TCriteria : SearchCriteria
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderBase{TEntity, TSummary, TCriteria}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        protected ProviderBase(ISearchRepository<TEntity, TSummary, TCriteria> repository) : base(repository) { }


        /// <summary>
        /// Gets the identifier compare expression.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Expression&lt;Func&lt;TEntity, System.Boolean&gt;&gt;.</returns>
        protected Expression<Func<TEntity, bool>> GetIdCompareExpression(TId id)
        {
            return TypeExtensions.CreateTypedCompareExpression<TEntity, TId>(nameof(Domain.EntityBase<TId>.Id), id);
        }

        /// <summary>
        /// Gets entity matching the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <param name="applyDefaultFilters">if set to <c>true</c> [apply default filters].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        public override async Task<TEntity> GetById(object id, bool detach = false,
            bool loadChildren = false, bool applyDefaultFilters = true)
        {
            if (!(DefaultIncludedProperties?.Any() ?? false))
            {
                return await base.GetById(id, detach, loadChildren, applyDefaultFilters);
            }

            var filters = (applyDefaultFilters)
              ? ApplyDefaultFilters(GetIdCompareExpression((TId)id))
              : GetIdCompareExpression((TId)id);

            var obj = (await base.Get(filter: filters,
                includeProperties: DefaultIncludedProperties,
                noTracking: detach, loadChildren: loadChildren))
                .FirstOrDefault();

            if (applyDefaultFilters && DefaultFilters.Any() && obj != null)
            {
                var filter = GetCombinedDefaultFilters().Compile();
                obj = (filter(obj)) ? obj : null;
            }

            return obj;
        }


        /// <summary>
        /// Gets the by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <param name="applyDefaultFilters">if set to <c>true</c> [apply default filters].</param>
        /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetByIds(IEnumerable<TId> ids, bool detach = false, bool loadChildren = false, bool applyDefaultFilters = true)
        {
            var filters = (applyDefaultFilters)
              ? ApplyDefaultFilters(x => ids.Contains(x.Id))
              : x => ids.Contains(x.Id);

            var entities = (await base.Get(filter: filters,
                includeProperties: DefaultIncludedProperties,
                noTracking: detach, loadChildren: loadChildren));

            if (loadChildren)
            {
                foreach (var entity in entities)
                {
                    await LoadChildren(entity, detach);
                }
            }

            return entities;
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        public virtual async Task Insert(TEntity entity, bool deferCommit = false)
        {
            await Repository.Insert(entity, deferCommit);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        public virtual async Task Update(TEntity entity, bool deferCommit = false)
        {
            await Repository.Update(entity, deferCommit);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        public virtual async Task Delete(TEntity entity, bool deferCommit = false)
        {
            await Repository.Delete(entity, deferCommit);
        }
    }



    /// <summary>
    /// Class ProviderBase.
    /// Implements the <see cref="Trident.Business.ProviderBase{TId, TEntity, TSummary, Trident.Search.SearchCriteria}" />
    /// Implements the <see cref="Trident.Data.Contracts.IProvider{TId, TEntity, TSummary, Trident.Search.SearchCriteria}" />
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <seealso cref="Trident.Business.ProviderBase{TId, TEntity, TSummary, Trident.Search.SearchCriteria}" />
    /// <seealso cref="Trident.Data.Contracts.IProvider{TId, TEntity, TSummary, Trident.Search.SearchCriteria}" />
    /// <seealso cref="SearchCriteria" />
    /// <seealso cref="SearchCriteria" />
    public abstract class ProviderBase<TId, TEntity, TSummary> : ProviderBase<TId, TEntity, TSummary, SearchCriteria>, IProvider<TId, TEntity, TSummary>
        where TEntity : Domain.EntityBase<TId>
        where TSummary : Domain.Entity
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderBase{TId, TEntity, TSummary}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        protected ProviderBase(ISearchRepository<TEntity, TSummary, SearchCriteria> repository) : base(repository) { }

    }

    /// <summary>
    /// Class ProviderBase.
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Business.ProviderBase{TId, TEntity, TEntity}" />
    /// <seealso cref="Trident.Data.Contracts.IProvider{TId, TEntity, TEntity}" />
    public abstract class ProviderBase<TId, TEntity> : ProviderBase<TId, TEntity, TEntity>, IProvider<TId, TEntity>
        where TEntity : Domain.EntityBase<TId>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderBase{TId, TEntity}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        protected ProviderBase(ISearchRepository<TEntity, TEntity, SearchCriteria> repository) : base(repository) { }

    }
}
