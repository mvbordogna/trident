using Trident.Data.Contracts;
using Trident.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Trident.Business
{
    /// <summary>
    /// Provides an abstract read-only implementation of a provider class.
    /// Does not implement write behavior to the underlying persistence medium
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IReadOnlyProvider{TEntity, TSummary, TCriteria}" />
    public abstract class ReadOnlyProviderBase<TEntity, TLookup, TSummary, TCriteria> : IReadOnlyProvider<TEntity, TLookup, TSummary, TCriteria>
        where TEntity : class
        where TLookup : Domain.Lookup, new()
        where TSummary : class
        where TCriteria : SearchCriteria
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly ISearchRepository<TEntity, TLookup, TSummary, TCriteria> _repository;
        /// <summary>
        /// The combinded default filters
        /// </summary>
        private Expression<Func<TEntity, bool>> _combindedDefaultFilters;

        /// <summary>
        /// When Overridden in derived classes, gets the default included properties.
        /// </summary>
        /// <value>The default included properties.</value>
        public virtual IReadOnlyList<string> DefaultIncludedProperties { get; } = new List<string>().AsReadOnly();

        /// <summary>
        /// Gets the default filters that are applied to all queries, unless specified otherwise.
        /// </summary>
        /// <value>The default filters.</value>
        public virtual IReadOnlyList<Expression<Func<TEntity, bool>>> DefaultFilters =>
             new List<Expression<Func<TEntity, bool>>>().AsReadOnly();

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <value>The repository.</value>
        protected ISearchRepository<TEntity, TLookup, TSummary, TCriteria> Repository => _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyProviderBase{TEntity, TSummary, TCriteria}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        protected ReadOnlyProviderBase(ISearchRepository<TEntity, TLookup, TSummary, TCriteria> repository)
        {

            _repository = repository;
        }

        /// <summary>
        /// Determines of any entities exist matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="applyDefaultFilters">if set to <c>true</c> [apply default filters].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> Exists(Expression<Func<TEntity, bool>> filter, bool applyDefaultFilters = true)
        {
            if (applyDefaultFilters)
            {
                filter = ApplyDefaultFilters(filter);
            }

            return await _repository.Exists(filter);
        }

        /// <summary>
        /// Gets entities matching the specified filter.
        /// NOTE:
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <param name="applyDefaultfilters">if set to <c>true</c> [apply defaultfilters].</param>
        /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
        public virtual async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            IEnumerable<string> includeProperties = null, bool noTracking = false, bool loadChildren = false,
            bool applyDefaultFilters = true)
        {
            var propertyIncluded = includeProperties ?? DefaultIncludedProperties;
            filter = (applyDefaultFilters) ? ApplyDefaultFilters(filter) : filter;
            var results = await _repository.Get(filter, orderBy, propertyIncluded, noTracking);

            if (loadChildren)
            {
                foreach (var entity in results)
                {
                    await LoadChildren(entity, noTracking);
                }
            }

            return results;
        }

        public IEnumerable<TEntity> GetSync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, 
            IOrderedQueryable<TEntity>> orderBy = null, IEnumerable<string> includeProperties = null, 
            bool noTracking = false, bool loadChildren = false, bool applyDefaultFilters = true)
        {
            var propertyIncluded = includeProperties ?? DefaultIncludedProperties;
            filter = (applyDefaultFilters) ? ApplyDefaultFilters(filter) : filter;
            var results =  _repository.GetSync(filter, orderBy, propertyIncluded, noTracking);

            if (loadChildren)
            {
                foreach (var entity in results)
                {
                    LoadChildrenSync(entity, noTracking);
                }
            }

            return results;
        }

        public bool ExistsSync(Expression<Func<TEntity, bool>> filter, bool applyDefaultFilters = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets entity matching the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <param name="applyDefaultFilters">if set to <c>true</c> [apply default filters].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        public virtual async Task<TEntity> GetById(object id, bool detach = false, bool loadChildren = false, bool applyDefaultFilters = true)
        {
            var obj = await _repository.GetById(id, detach);
            if (applyDefaultFilters && obj != null)
            {
                var filter = GetCombinedDefaultFilters()?.Compile();
                obj = (filter == null || filter(obj)) ? obj : null;
            }

            if (obj != null && loadChildren)
            {
                await LoadChildren(obj, detach);
            }

            return obj;
        }


        public TEntity GetByIdSync(object id, bool detach = false, bool loadChildren = false, bool applyDefaultFilters = true)
        {
            var obj = _repository.GetByIdSync(id, detach);
            if (applyDefaultFilters && obj != null)
            {
                var filter = GetCombinedDefaultFilters()?.Compile();
                obj = (filter == null || filter(obj)) ? obj : null;
            }

            if (obj != null && loadChildren)
            {
                LoadChildrenSync(obj, detach);
            }

            return obj;
        }
             

        /// <summary>
        /// Loads the children.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <returns>Task.</returns>
        protected virtual Task LoadChildren(TEntity entity, bool noTracking = false)
        {
            return Task.CompletedTask;
        }

        protected virtual void LoadChildrenSync(TEntity entity, bool noTracking = false)
        {
            
        }

        /// <summary>
        /// Loads the children.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <returns>Task.</returns>
        protected virtual Task LoadChildren(IEnumerable<TSummary> entity, bool noTracking = false)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Loads the children.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <returns>Task.</returns>
        protected virtual void LoadChildrenSync(IEnumerable<TSummary> entity, bool noTracking = false)
        {
          
        }

        /// <summary>
        /// Searches the repository for entities given the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        public virtual async Task<SearchResults<TLookup, TCriteria>> SearchLookups(TCriteria criteria)
        {
            ApplyDefaultFilters(criteria);
            var searchResult = await _repository.SearchLookups(criteria, new List<string>());

            return searchResult;
        }

        public virtual SearchResults<TLookup, TCriteria> SearchLookupsSync(TCriteria criteria)
        {
            ApplyDefaultFilters(criteria);
            var searchResult = _repository.SearchLookupsSync(criteria, new List<string>());

            return searchResult;
        }


        /// <summary>
        /// Searches the repository for entities given the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        public virtual async Task<SearchResults<TSummary, TCriteria>> Search(TCriteria criteria, bool loadChildren = false)
        {
            ApplyDefaultFilters(criteria);
            var searchResult = await _repository.Search(criteria, DefaultIncludedProperties);

            if (loadChildren)
            {
                await this.LoadChildren(searchResult.Results);
            }

            return searchResult;
        }



        /// <summary>
        /// Searches the repository for entities given the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        public virtual SearchResults<TSummary, TCriteria> SearchSync(TCriteria criteria, bool loadChildren = false)
        {
            ApplyDefaultFilters(criteria);
            var searchResult = _repository.SearchSync(criteria, DefaultIncludedProperties);

            if (loadChildren)
            {
                this.LoadChildren(searchResult.Results);
            }

            return searchResult;
        }

        /// <summary>
        /// Gets the combined default filters.
        /// </summary>
        /// <returns>Expression&lt;Func&lt;TEntity, System.Boolean&gt;&gt;.</returns>
        protected Expression<Func<TEntity, bool>> GetCombinedDefaultFilters()
        {
            if (_combindedDefaultFilters == null && DefaultFilters.Any())
            {
                if (DefaultFilters.Count == 1)
                    return _combindedDefaultFilters = DefaultFilters.First();

                Expression<Func<TEntity, bool>> cumulator = null;
                if (DefaultFilters.Count > 1)
                    cumulator = DefaultFilters.First();

                for (int i = 1; i < DefaultFilters.Count; i++)
                {
                    cumulator = cumulator.AndAlso(DefaultFilters[i]);
                }

                _combindedDefaultFilters = cumulator;
            }

            return _combindedDefaultFilters;
        }

        /// <summary>
        /// Applies the default filters.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Expression&lt;Func&lt;TEntity, System.Boolean&gt;&gt;.</returns>
        protected Expression<Func<TEntity, bool>> ApplyDefaultFilters(Expression<Func<TEntity, bool>> filter)
        {
            var defaultFilter = GetCombinedDefaultFilters();
            if (defaultFilter != null && filter != null)
            {
                filter = filter.AndAlso(defaultFilter);
            }
            else if (defaultFilter != null)
            {
                return defaultFilter;
            }

            return filter;
        }

        /// <summary>
        /// Applies the default filters.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        private void ApplyDefaultFilters(TCriteria criteria)
        {
            if (criteria != null && criteria.ApplyDefaultFilters)
            {
                var defaultFilter = GetCombinedDefaultFilters();
                if (defaultFilter != null)
                {
                    criteria.DefaultFilterBag = defaultFilter;
                }
            }
        }

    }



    /// <summary>
    /// Provides an abstract read-only implementation of a provider class.
    /// Does not implement write behavior to the underlying persistence medium
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IReadOnlyProvider{TEntity, TSummary, TCriteria}" />
    public abstract class ReadOnlyProviderBase<TEntity, TLookup, TSummary> : ReadOnlyProviderBase<TEntity, TLookup, TSummary, SearchCriteria>,
        IReadOnlyProvider<TEntity, TLookup, TSummary>
        where TEntity : class
        where TLookup : Domain.Lookup, new()
        where TSummary : class
    {
        protected ReadOnlyProviderBase(ISearchRepository<TEntity, TLookup, TSummary, SearchCriteria> repository) : base(repository)
        {
        }
    }

    /// <summary>
    /// Provides an abstract read-only implementation of a provider class.
    /// Does not implement write behavior to the underlying persistence medium
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IReadOnlyProvider{TEntity, TSummary, TCriteria}" />
    public abstract class ReadOnlyProviderBase<TEntity, TLookup> : ReadOnlyProviderBase<TEntity, TLookup, TEntity>,
        IReadOnlyProvider<TEntity, TLookup>
        where TEntity : class
        where TLookup : Domain.Lookup, new()

    {
        protected ReadOnlyProviderBase(ISearchRepository<TEntity, TLookup> repository) : base(repository)
        {
        }
    }

    /// <summary>
    /// Provides an abstract read-only implementation of a provider class.
    /// Does not implement write behavior to the underlying persistence medium
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IReadOnlyProvider{TEntity, TSummary, TCriteria}" />
    public abstract class ReadOnlyProviderBase<TEntity> : ReadOnlyProviderBase<TEntity, Domain.Lookup>,
        IReadOnlyProvider<TEntity>
        where TEntity : class
    {
        protected ReadOnlyProviderBase(ISearchRepository<TEntity> repository) : base(repository)
        {
        }
    }

}
