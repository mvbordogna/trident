using Trident.Contracts;
using Trident.Data.Contracts;
using Trident.Search;
using Trident.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Trident.Business
{
    /// <summary>
    /// Provides an abstract read-only implementation of a manager class.
    /// Does not implement write behavior to the underlying persistence medium
    /// Implements the <see cref="Trident.Contracts.IReadOnlyManager{TEntity, TSummary, TCriteria}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="Trident.Contracts.IReadOnlyManager{TEntity, TSummary, TCriteria}" />
    /// <seealso cref="Trident.Contracts.IManager{TId, TEntity, TSummary, TCriteria}" />
    /// <seealso cref="Trident.Contracts.IManager{TEntity, TSummary, TCriteria}" />
    public abstract class ReadOnlyManagerBase<TEntity, TSummary, TCriteria> : IReadOnlyManager<TEntity, TSummary, TCriteria>
        where TEntity : class
        where TSummary : class
        where TCriteria : SearchCriteria
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerBase{TEntity,TSummary,TCriteria}" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        protected ReadOnlyManagerBase(IReadOnlyProvider<TEntity, TSummary, TCriteria> provider)
        {
            Provider = provider;
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>The provider.</value>
        protected IReadOnlyProvider<TEntity, TSummary, TCriteria> Provider { get; }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>System.Threading.Tasks.Task&lt;TEntity&gt;.</returns>
        [NonTransactional]
        public async Task<TEntity> GetById(object id, bool loadChildren = false)
        {
            return await Provider.GetById(id, loadChildren: loadChildren);
        }

        /// <summary>
        /// Gets the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Collections.Generic.IEnumerable&lt;TEntity&gt;&gt;.</returns>
        [NonTransactional]
        public async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null,
                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                    List<string> includeProperties = null,
                    bool loadChildren = false)
        {
            return await Provider.Get(filter, orderBy, includeProperties, loadChildren: loadChildren);
        }

        /// <summary>
        /// Returns a value indicating where any entities exist matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [NonTransactional]
        public async Task<bool> Exists(Expression<Func<TEntity, bool>> filter)
        {
            return await Provider.Exists(filter);
        }

        /// <summary>
        /// Searches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>System.Threading.Tasks.Task&lt;Trident.Search.SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        [NonTransactional]
        public async Task<SearchResults<TSummary, TCriteria>> Search(TCriteria criteria, bool loadChildren = false)
        {
            return await Provider.Search(criteria, loadChildren);
        }
    }

    /// <summary>
    /// Class ReadOnlyManagerBase.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <seealso cref="Trident.Business.ReadOnlyManagerBase{TEntity, TSummary, Trident.Search.SearchCriteria}" />
    /// <seealso cref="Trident.Contracts.IReadOnlyManager{TEntity, TSummary, Trident.Search.SearchCriteria}" />
    public abstract class ReadOnlyManagerBase<TEntity, TSummary>
        : ReadOnlyManagerBase<TEntity, TSummary, SearchCriteria>,
        IReadOnlyManager<TEntity, TSummary, SearchCriteria>
       where TEntity : class
       where TSummary : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyManagerBase{TEntity, TSummary}" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        protected ReadOnlyManagerBase(IReadOnlyProvider<TEntity, TSummary> provider) : base(provider)
        {
        }
    }

    /// <summary>
    /// Class ReadOnlyManagerBase.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Business.ReadOnlyManagerBase{TEntity, TEntity}" />
    /// <seealso cref="Trident.Contracts.IReadOnlyManager{TEntity, TEntity}" />
    public abstract class ReadOnlyManagerBase<TEntity>
        : ReadOnlyManagerBase<TEntity, TEntity>,
        IReadOnlyManager<TEntity, TEntity>
       where TEntity : class

    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyManagerBase{TEntity}" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        protected ReadOnlyManagerBase(IReadOnlyProvider<TEntity> provider) : base(provider)
        {
        }
    }
}



