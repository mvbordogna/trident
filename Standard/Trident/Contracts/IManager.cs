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
    /// Implements the <see cref="Trident.Contracts.IManager" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="Trident.Contracts.IManager" />
    public interface IReadOnlyManager<TEntity, TSummary, TCriteria>:IManager
        where TEntity : class
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

        /// <summary>
        /// Determines if any entities exists given the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> Exists(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Searches given the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        Task<SearchResults<TSummary, TCriteria>> Search(TCriteria criteria, bool loadChildren = false);
    }

    /// <summary>
    /// Interface IReadOnlyManager
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <seealso cref="Trident.Contracts.IReadOnlyManager{TEntity, TSummary, Trident.Search.SearchCriteria}" />
    public interface IReadOnlyManager<TEntity, TSummary> : IReadOnlyManager<TEntity, TSummary, SearchCriteria>
        where TEntity : class
        where TSummary : class      
    { }

    /// <summary>
    /// Interface IReadOnlyManager
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Contracts.IReadOnlyManager{TEntity, TEntity}" />
    public interface IReadOnlyManager<TEntity> : IReadOnlyManager<TEntity, TEntity>
        where TEntity : class
    { }

    /// <summary>
    /// Interface IManager
    /// Implements the <see cref="Trident.Contracts.IReadOnlyManager{TEntity, TSummary, TCriteria}" />
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="Trident.Contracts.IReadOnlyManager{TEntity, TSummary, TCriteria}" />
    /// <seealso cref="Trident.Contracts.IManager" />
    public interface IManager<TId, TEntity, TSummary, TCriteria>: IReadOnlyManager<TEntity, TSummary, TCriteria>
        where TEntity : Domain.EntityBase<TId>
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

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> Save(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> Insert(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> Update(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> Delete(TEntity entity, bool deferCommit = false);
        /// <summary>
        /// Bulks the save.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns>Task&lt;List&lt;TEntity&gt;&gt;.</returns>
        Task<IEnumerable<TEntity>> BulkSave(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes all specified Entities
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> BulkDelete(IEnumerable<TEntity> entities);


        /// <summary>
        /// Deletes all entities matching the specified Ids.
        /// </summary>
        /// <param name="entityIds">The entity ids.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> BulkDelete(IEnumerable<TId> entityIds);

        /// <summary>
        /// Patches the entity matching specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <param name="patches">The patches.</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> Patch(TId id, bool deferCommit = false, params Action<TEntity>[] patches);

        /// <summary>
        /// Patches the entity matching specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="patches">The patches.</param>
        /// <param name="overridePatches">The override patches.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> Patch(TId id, Dictionary<string, object> patches, IDictionary<string, Action<TEntity>> overridePatches = null, bool deferCommit = false);
    }


    /// <summary>
    /// Interface IManager
    /// Implements the <see cref="Trident.Contracts.IManager{TId, TEntity, TSummary, Trident.Search.SearchCriteria}" />
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <seealso cref="Trident.Contracts.IManager{TId, TEntity, TSummary, Trident.Search.SearchCriteria}" />
    public interface IManager<TId, TEntity, TSummary> : IManager<TId, TEntity, TSummary, SearchCriteria>
       where TEntity : Domain.EntityBase<TId>
       where TSummary : Domain.Entity
    { }

    /// <summary>
    /// Interface IManager
    /// Implements the <see cref="Trident.Contracts.IManager{TId, TEntity, TEntity, Trident.Search.SearchCriteria}" />
    /// </summary>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Contracts.IManager{TId, TEntity, TEntity, Trident.Search.SearchCriteria}" />
    public interface IManager<TId, TEntity> : IManager<TId, TEntity, TEntity, SearchCriteria>
     where TEntity : Domain.EntityBase<TId>    
    { }

}
