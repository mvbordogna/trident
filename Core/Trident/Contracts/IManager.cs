using Trident.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Trident.Contracts
{
    /// <summary>
    /// Provides a root interface for assembly scanning registration of Managers
    /// </summary>
    public interface IManager { }


    /// <summary>
    /// Interface IReadOnlyManager
    /// Implements the <see cref="TridentOptionsBuilder.Contracts.IManager" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IManager" />
    public interface IReadOnlyManager<TEntity, TLookup, TSummary, TCriteria>:IManager
        where TEntity : class
        where TLookup : Domain.Lookup, new()
        where TSummary : class
        where TCriteria : SearchCriteria
    {
        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> GetById(object id, bool loadChildren = false);

        TEntity GetByIdSync(object id, bool loadChildren = false);

        /// <summary>
        /// Gets the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
        Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
             List<string> includeProperties = null,
                    bool loadChildren = false);

        IEnumerable<TEntity> GetSync(Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         List<string> includeProperties = null,
                bool loadChildren = false);

     
        /// <summary>
        /// Determines if any entities exists given the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> Exists(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Determines if any entities exists given the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        bool ExistsSync(Expression<Func<TEntity, bool>> filter);


        /// <summary>
        /// Searches given the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        Task<SearchResults<TSummary, TCriteria>> Search(TCriteria criteria, bool loadChildren = false);

        SearchResults<TSummary, TCriteria> SearchSync(TCriteria criteria, bool loadChildren = false);


        Task<SearchResults<TLookup, TCriteria>> SearchLookups(TCriteria criteria);

        SearchResults<TLookup, TCriteria> SearchLookupsSync(TCriteria criteria);

    }

    /// <summary>
    /// Interface IReadOnlyManager
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IReadOnlyManager{TEntity, TSummary, TridentOptionsBuilder.Search.SearchCriteria}" />
    public interface IReadOnlyManager<TEntity, TLookup, TSummary> : IReadOnlyManager<TEntity, TLookup, TSummary, SearchCriteria>
        where TEntity : class
        where TLookup : Domain.Lookup, new()
        where TSummary : class      
    { }

    /// <summary>
    /// Interface IReadOnlyManager
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IReadOnlyManager{TEntity, TLookup, TEntity}" />
    public interface IReadOnlyManager<TEntity, TLookup> : IReadOnlyManager<TEntity, TLookup, TEntity>
        where TEntity : class
        where TLookup : Domain.Lookup, new()
    { }


    /// <summary>
    /// Interface IReadOnlyManager
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IReadOnlyManager{TEntity, TLookup, TEntity}" />
    public interface IReadOnlyManager<TEntity> : IReadOnlyManager<TEntity, Domain.Lookup>
        where TEntity : class        
    { }


    /// <summary>
    /// Interface IManager
    /// Implements the <see cref="TridentOptionsBuilder.Contracts.IReadOnlyManager{TEntity, TSummary, TCriteria}" />
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IReadOnlyManager{TEntity, TSummary, TCriteria}" />
    /// <seealso cref="TridentOptionsBuilder.Contracts.IManager" />
    public interface IManager<TId, TEntity, TLookup, TSummary, TCriteria>: IReadOnlyManager<TEntity, TLookup, TSummary, TCriteria>
        where TEntity : Domain.EntityBase<TId>
        where TLookup : Domain.Lookup, new()
        where TSummary :Domain.Entity
        where TCriteria : SearchCriteria
    {
        /// <summary>
        /// Gets the by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
        Task<IEnumerable<TEntity>> GetByIds(IEnumerable<TId> ids, bool loadChildren = false);

        IEnumerable<TEntity> GetByIdsSync(IEnumerable<TId> ids, bool loadChildren = false);

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> Save(TEntity entity, bool deferCommit = false);

        TEntity SaveSync(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> Insert(TEntity entity, bool deferCommit = false);


        TEntity InsertSync(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> Update(TEntity entity, bool deferCommit = false);

        TEntity UpdateSync(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> Delete(TEntity entity, bool deferCommit = false);

        bool DeleteSync(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Bulks the save.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns>Task&lt;List&lt;TEntity&gt;&gt;.</returns>
        Task<IEnumerable<TEntity>> BulkSave(IEnumerable<TEntity> entities);

        IEnumerable<TEntity> BulkSaveSync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes all specified Entities
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> BulkDelete(IEnumerable<TEntity> entities);

        bool BulkDeleteSync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes all entities matching the specified Ids.
        /// </summary>
        /// <param name="entityIds">The entity ids.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> BulkDelete(IEnumerable<TId> entityIds);

        bool BulkDeleteSync(IEnumerable<TId> entityIds);

        /// <summary>
        /// Patches the entity matching specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <param name="patches">The patches.</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> Patch(TId id, bool deferCommit = false, params Action<TEntity>[] patches);

        TEntity PatchSync(TId id, bool deferCommit = false, params Action<TEntity>[] patches);

        /// <summary>
        /// Patches the entity matching specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="patches">The patches.</param>
        /// <param name="overridePatches">The override patches.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> Patch(TId id, Dictionary<string, object> patches, IDictionary<string, Action<TEntity>> overridePatches = null, bool deferCommit = false);

        TEntity PatchSync(TId id, Dictionary<string, object> patches, IDictionary<string, Action<TEntity>> overridePatches = null, bool deferCommit = false);
    }


    /// <summary>
    /// Interface IManager
    /// Implements the <see cref="TridentOptionsBuilder.Contracts.IManager{TId, TEntity, TSummary, TridentOptionsBuilder.Search.SearchCriteria}" />
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IManager{TId, TEntity, TSummary, TridentOptionsBuilder.Search.SearchCriteria}" />
    public interface IManager<TId, TEntity, TLookup, TSummary> : IManager<TId, TEntity, TLookup, TSummary, SearchCriteria>
       where TEntity : Domain.EntityBase<TId>
       where TLookup : Domain.Lookup, new()
       where TSummary : Domain.Entity
    { }

    /// <summary>
    /// Interface IManager
    /// Implements the <see cref="TridentOptionsBuilder.Contracts.IManager{TId, TEntity, TEntity, TridentOptionsBuilder.Search.SearchCriteria}" />
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IManager{TId, TEntity, TEntity, TridentOptionsBuilder.Search.SearchCriteria}" />
    public interface IManager<TId, TEntity, TLookup> : IManager<TId, TEntity, TLookup, TEntity>
     where TEntity : Domain.EntityBase<TId>
     where TLookup : Domain.Lookup, new()
    { }

    /// <summary>
    /// Interface IManager
    /// Implements the <see cref="TridentOptionsBuilder.Contracts.IManager{TId, TEntity, TEntity, TridentOptionsBuilder.Search.SearchCriteria}" />
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IManager{TId, TEntity, TEntity, TridentOptionsBuilder.Search.SearchCriteria}" />
    public interface IManager<TId, TEntity> : IManager<TId, TEntity, Domain.Lookup>
     where TEntity : Domain.EntityBase<TId>
    { }


}
