using Trident.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Trident.Data.Contracts
{

    /// <summary>
    /// Interface IReadOnlyProvider
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IProvider" />
    public interface IReadOnlyProvider<TEntity, TLookup, TSummary, TCriteria> : IProvider
        where TEntity : class
        where TLookup : Domain.Lookup,new()
        where TSummary : class
        where TCriteria : SearchCriteria
    {
        /// <summary>
        /// Gets the Read Only default included properties List.
        /// </summary>
        /// <value>The default included properties.</value>
        IReadOnlyList<string> DefaultIncludedProperties { get; }

        /// <summary>
        /// Gets entity matching the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <param name="applyDefaultFilters">if set to <c>true</c> [apply default filters].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> GetById(object id, bool detach = false, bool loadChildren = false, bool applyDefaultFilters = true);

        TEntity GetByIdSync(object id, bool detach = false, bool loadChildren = false, bool applyDefaultFilters = true);

        /// <summary>
        /// Gets entities matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <param name="applyDefaultFilters">if set to <c>true</c> [apply default filters].</param>
        /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
        Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            IEnumerable<string> includeProperties = null, bool noTracking = false, bool loadChildren = false, bool applyDefaultFilters = true);

        IEnumerable<TEntity> GetSync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           IEnumerable<string> includeProperties = null, bool noTracking = false, bool loadChildren = false, bool applyDefaultFilters = true);


        /// <summary>
        /// Determines of any entities exist matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="applyDefaultFilters">if set to <c>true</c> [apply default filters].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> Exists(Expression<Func<TEntity, bool>> filter, bool applyDefaultFilters = true);

        bool ExistsSync(Expression<Func<TEntity, bool>> filter, bool applyDefaultFilters = true);

        /// <summary>
        /// Searches the repository for entities given the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        Task<SearchResults<TSummary, TCriteria>> Search(TCriteria criteria, bool loadChildren = false);


        /// <summary>
        /// Searches the repository for entities given the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        SearchResults<TSummary, TCriteria> SearchSync(TCriteria criteria, bool loadChildren = false);

        Task<SearchResults<TLookup, TCriteria>> SearchLookups(TCriteria criteria);

        SearchResults<TLookup, TCriteria> SearchLookupsSync(TCriteria criteria);
    }

    /// <summary>
    /// Interface IReadOnlyProvider
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IReadOnlyProvider{TEntity, TLookup, TSummary, TridentOptionsBuilder.Search.SearchCriteria}" />
    public interface IReadOnlyProvider<TEntity, TLookup, TSummary> : IReadOnlyProvider<TEntity, TLookup, TSummary, SearchCriteria>
       where TEntity : class
       where TLookup : Domain.Lookup,new()
       where TSummary : class
    { }

    /// <summary>
    /// Interface IReadOnlyProvider
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IReadOnlyProvider{TEntity, TEntity}" />
    public interface IReadOnlyProvider<TEntity, TLookup> : IReadOnlyProvider<TEntity, TLookup, TEntity>
       where TEntity : class
       where TLookup : Domain.Lookup,new()
    { }

    /// <summary>
    /// Interface IReadOnlyProvider
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IReadOnlyProvider{TEntity, Domain.Lookup{object} TEntity}" />
    public interface IReadOnlyProvider<TEntity> : IReadOnlyProvider<TEntity, Domain.Lookup>
       where TEntity : class
    { }


    /// <summary>
    /// Interface IProvider
    /// Implements the <see cref="TridentOptionsBuilder.Data.Contracts.IReadOnlyProvider{TEntity, TSummary, TCriteria}" />
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the  entity.</typeparam>
    /// <typeparam name="TSummary">The type of the summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the criteria.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IReadOnlyProvider{TEntity, TSummary, TCriteria}" />
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IProvider" />
    public interface IProvider<TId, TEntity, TLookup, TSummary, TCriteria> : IReadOnlyProvider<TEntity, TLookup, TSummary, TCriteria>
        where TEntity : Domain.EntityBase<TId>
        where TLookup : Domain.Lookup,new()
        where TSummary : Domain.Entity
        where TCriteria : SearchCriteria
    {

        /// <summary>
        /// Gets the by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <param name="applyDefaultFilters">if set to <c>true</c> [apply default filters].</param>
        /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
        Task<IEnumerable<TEntity>> GetByIds(IEnumerable<TId> ids, bool detach = false, bool loadChildren = false, bool applyDefaultFilters = true);

        IEnumerable<TEntity> GetByIdsSync(IEnumerable<TId> ids, bool detach = false, bool loadChildren = false, bool applyDefaultFilters = true);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        Task Insert(TEntity entity, bool deferCommit = false);

        void InsertSync(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        Task Update(TEntity entity, bool deferCommit = false);

        void UpdateSync(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        Task Delete(TEntity entity, bool deferCommit = false);

        void DeleteSync(TEntity entity, bool deferCommit = false);

    }


    /// <summary>
    /// Interface IProvider
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IProvider{TId, TEntity, TSummary, TridentOptionsBuilder.Search.SearchCriteria}" />
    public interface IProvider<TId, TEntity, TLookup, TSummary> : IProvider<TId, TEntity, TLookup, TSummary, SearchCriteria>
        where TEntity : Domain.EntityBase<TId>
        where TLookup : Domain.Lookup,new()
        where TSummary : Domain.Entity
    { }

    /// <summary>
    /// Interface IProvider
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IProvider{TId, TEntity, TLookup, TEntity}" />
    public interface IProvider<TId, TEntity, TLookup> : IProvider<TId, TEntity, TLookup, TEntity>
        where TEntity : Domain.EntityBase<TId>
        where TLookup : Domain.Lookup,new()
    { }

    /// <summary>
    /// Interface IProvider
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IProvider{TId, TEntity, TLookup, TEntity}" />
    public interface IProvider<TId, TEntity> : IProvider<TId, TEntity, Domain.Lookup>
        where TEntity : Domain.EntityBase<TId>
    { }


}
