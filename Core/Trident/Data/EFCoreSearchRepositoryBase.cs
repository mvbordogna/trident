using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident.Data.Contracts;
using Trident.Search;
using Trident.Contracts.Enums;
using Trident.Domain;
using System;
using System.Linq.Expressions;
using Trident.Extensions;

namespace Trident.EFCore
{
    /// <summary>
    /// Abstract Class SearchRepositoryBase.
    /// Implements the <see cref="TridentOptionsBuilder.EFCore.EFCoreRepository{TEntity}" />
    /// Implements the <see cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity, TSummery, TCriteria}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummery">The type of the t summery.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.EFCore.EFCoreRepository{TEntity}" />
    /// <seealso cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity, TSummery, TCriteria}" />
    /// <seealso cref="TridentOptionsBuilder.Data.EntityFramework.EFRepository{TEntity}" />
    /// <seealso cref="TridentOptionsBuilder.TimeSummit.Repositories.Contracts.ISearchRepositoryBase{TEntity, TSummery, TCriteria}" />
    public abstract class EFCoreSearchRepositoryBase<TEntity, TLookup, TSummery, TCriteria> : EFCoreRepository<TEntity>, ISearchRepository<TEntity, TLookup, TSummery, TCriteria>
        where TEntity : Entity
        where TLookup : Domain.Lookup, new()
        where TSummery : Entity
        where TCriteria : SearchCriteria
    {
        /// <summary>
        /// The results builder
        /// </summary>
        private readonly ISearchResultsBuilder _resultsBuilder;
        /// <summary>
        /// The query builder
        /// </summary>
        private readonly ISearchQueryBuilder _queryBuilder;
        private readonly IQueryableHelper queryableHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFSearchRepositoryBase{TEntity, TSummery, TCriteria}" /> class.
        /// </summary>
        /// <param name="resultsBuilder">The results builder.</param>
        /// <param name="queryBuilder">The query builder.</param>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        public EFCoreSearchRepositoryBase(
            ISearchResultsBuilder resultsBuilder,
            ISearchQueryBuilder queryBuilder,
            IAbstractContextFactory abstractContextFactory,
            IQueryableHelper queryableHelper) : base(abstractContextFactory, queryableHelper)
        {
            resultsBuilder.GuardIsNotNull(nameof(resultsBuilder));
            queryBuilder.GuardIsNotNull(nameof(queryBuilder));
            abstractContextFactory.GuardIsNotNull(nameof(abstractContextFactory));
            _resultsBuilder = resultsBuilder;
            _queryBuilder = queryBuilder;
            this.queryableHelper = queryableHelper;
        }

        /// <summary>
        /// Searches the specified search criteria.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="includedProperties">The included properties.</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        public virtual async Task<SearchResults<TSummery, TCriteria>> Search(TCriteria searchCriteria, IEnumerable<String> includedProperties = null)
        {
            var query = BuildQuery(searchCriteria, includedProperties);

            // Get total Records before returning results
            var totalRecords = await queryableHelper.CountAsync(query);

            //apply paging
            query = ApplyPaging(query, searchCriteria);

            var results = await queryableHelper.ToListAsync(Context, query, false);

            return SearchResultContent(results, searchCriteria, totalRecords);
        }

        public SearchResults<TSummery, TCriteria> SearchSync(TCriteria searchCriteria, IEnumerable<string> includedProperties = null)
        {
            var query = BuildQuery(searchCriteria, includedProperties);

            // Get total Records before returning results
            var totalRecords = query.Count();

            //apply paging
            query = ApplyPaging(query, searchCriteria);

            var results = query.ToList();

            return SearchResultContent(results, searchCriteria, totalRecords);
        }

        protected virtual Expression<Func<TSummery, string>> EntityDisplayConverter { get; } = (TSummery e) => e.Id.ToString();



        public SearchResults<TLookup, TCriteria> SearchLookupsSync(TCriteria criteria, IEnumerable<string> includedProperties = null)
        {
            var query = BuildQuery(criteria, includedProperties);
            query = queryableHelper.AsNoTracking(query);

            // Get total Records before returning results
            var totalRecords = query.Count();

            //apply paging
            query = ApplyPaging(query, criteria);

            var FullLookupSelector =
            EntityDisplayConverter.Use((TSummery entity, Func<TSummery, string> selector) => new TLookup
            {
                Id = entity.Id,
                Display = selector(entity)
            });

            var results = (query
                .Select(FullLookupSelector))
                .ToList();

            return SearchLookupsResultContent(results, criteria, totalRecords);
        }

     

        /// <summary>
        /// Searches the specified search criteria.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="includedProperties">The included properties.</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        public virtual async Task<SearchResults<TLookup, TCriteria>> SearchLookups(TCriteria searchCriteria, IEnumerable<string> includedProperties = null)
        {
            var query = BuildQuery(searchCriteria, includedProperties);
            query = queryableHelper.AsNoTracking(query);

            var totalRecords = 0;
            if (!searchCriteria.SkipCount)
            {
                // Get total Records before returning results
                totalRecords = await queryableHelper.CountAsync(query);
            }

            //apply paging
            query = ApplyPaging(query, searchCriteria);

            var FullLookupSelector =
            EntityDisplayConverter.Use((TSummery entity, Func<TSummery, string> selector) => new TLookup
            {
                Id = entity.Id,
                Display = selector(entity)
            });


            var summaryQuery = query.Select(FullLookupSelector);

            var results = await queryableHelper.ToListAsync(Context, summaryQuery, false);

            return SearchLookupsResultContent(results, searchCriteria, totalRecords);
        }

        private IQueryable<TSummery> BuildQuery(TCriteria searchCriteria, IEnumerable<string> includedProperties = null)
        {
            var query = base.Context.Query<TSummery>(noTracking: searchCriteria.detach);
            var context = GetCurrentContext(query);

            //apply keyword search
            query = this.ApplyKeywordSearch(query, searchCriteria.Keywords);

            //apply filters           
            query = ApplyFilter(query, searchCriteria, context);

            if (includedProperties != null)
                query = includedProperties
                    .Aggregate(query, (current, includeProperty) => queryableHelper.Include(current, includeProperty));

            //apply sorting
            if (searchCriteria.MultiOrderBy.Any())
            {
                query = ApplyOrderBy(query, searchCriteria.MultiOrderBy);
            }
            else if (!string.IsNullOrWhiteSpace(searchCriteria.OrderBy))
            {
                query = ApplyOrderBy(query, new Dictionary<string, SortOrder>()
                {
                    { searchCriteria.OrderBy, searchCriteria.SortOrder }
                });
            }

            return query;
        }




        /// <summary>
        /// Then implemented in a derivied class, applies filters given the specified keywords string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="keywords">The keywords.</param>
        /// <returns>IQueryable&lt;TSummery&gt;.</returns>
        protected virtual IQueryable<T> ApplyKeywordSearch<T>(IQueryable<T> source, string keywords)
            where T : class
        {
            return source;
        }

        /// <summary>
        /// Applies paging to the specified query given the search criteria current page and page size.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        protected virtual IQueryable<T> ApplyPaging<T>(IQueryable<T> source, TCriteria searchCriteria)
            where T : class
        {
            return _queryBuilder.ApplyPaging(source, searchCriteria);
        }

        /// <summary>
        /// Applies the order by clauses to the IQueryable given the specified dictionary.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filterBy">The filter by.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        protected virtual IQueryable<T> ApplyOrderBy<T>(IQueryable<T> source, Dictionary<string, SortOrder> filterBy)
            where T : class
        {
            return _queryBuilder.ApplyOrderBy(source, filterBy);
        }

        /// <summary>
        /// Applies filtering to the specified query given the internal FilterBag Object member of SearchCriteria.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="context"></param>
        /// <returns>IQueryable&lt;TSummery&gt;.</returns>
        protected virtual IQueryable<T> ApplyFilter<T>(IQueryable<T> source, SearchCriteria criteria, IContext context)
            where T : class
        {

            var filters = (criteria?.Filters?.Any() ?? false)
                ? _queryBuilder.ApplyFilter(source, criteria, context)
                : source;
            return _queryBuilder.ApplyFilterBag(filters, criteria);
        }

        /// <summary>
        /// Searches the content of the result.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns>SearchResults&lt;T, C&gt;.</returns>
        protected virtual SearchResults<TSummery, TCriteria> SearchResultContent(List<TSummery> results, TCriteria criteria, int totalRecords)
        {
            return _resultsBuilder.Build(results, criteria, totalRecords);
        }

        /// <summary>
        /// Searches the content of the result.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns>SearchResults&lt;T, C&gt;.</returns>
        protected virtual SearchResults<TLookup, TCriteria> SearchLookupsResultContent(List<TLookup> results, TCriteria criteria, int totalRecords)
        {
            return _resultsBuilder.Build(results, criteria, totalRecords);
        }

    }


    /// <summary>
    /// Class EFSearchRepositoryBase.
    /// Implements the <see cref="TridentOptionsBuilder.EFCore.EFCoreSearchRepositoryBase{TEntity, TSummery, TridentOptionsBuilder.Search.SearchCriteria}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummery">The type of the t summery.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.EFCore.EFCoreSearchRepositoryBase{TEntity, TSummery, TridentOptionsBuilder.Search.SearchCriteria}" />
    /// <seealso cref="TridentOptionsBuilder.Data.EntityFramework.EFSearchRepositoryBase{TEntity, TSummery, TridentOptionsBuilder.Search.SearchCriteria}" />
    /// <seealso cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity, TSummery, TridentOptionsBuilder.Search.SearchCriteria}" />
    public abstract class EFCoreSearchRepositoryBase<TEntity, TLookup, TSummery> : EFCoreSearchRepositoryBase<TEntity, TLookup, TSummery, SearchCriteria>, ISearchRepository<TEntity, TLookup, TSummery>
       where TEntity : Entity
       where TLookup : Domain.Lookup, new()
       where TSummery : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreSearchRepositoryBase{TEntity, TSummery}" /> class.
        /// </summary>
        /// <param name="resultsBuilder">The results builder.</param>
        /// <param name="queryBuilder">The query builder.</param>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        public EFCoreSearchRepositoryBase(
            ISearchResultsBuilder resultsBuilder,
            ISearchQueryBuilder queryBuilder,
            IAbstractContextFactory abstractContextFactory,
            IQueryableHelper queryableHelper)
            : base(resultsBuilder, queryBuilder, abstractContextFactory, queryableHelper)
        {
        }
    }

    /// <summary>
    /// Class EFCoreSearchRepositoryBase.
    /// Implements the <see cref="TridentOptionsBuilder.EFCore.EFCoreSearchRepositoryBase{TEntity, TEntity}" />
    /// Implements the <see cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity, TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.EFCore.EFCoreSearchRepositoryBase{TEntity, TEntity}" />
    /// <seealso cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity, TEntity}" />
    public abstract class EFCoreSearchRepositoryBase<TEntity, TLookup> : EFCoreSearchRepositoryBase<TEntity, TLookup, TEntity>, ISearchRepository<TEntity, TLookup>
     where TEntity : Entity
     where TLookup : Domain.Lookup, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreSearchRepositoryBase{TEntity}" /> class.
        /// </summary>
        /// <param name="resultsBuilder">The results builder.</param>
        /// <param name="queryBuilder">The query builder.</param>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        public EFCoreSearchRepositoryBase(
            ISearchResultsBuilder resultsBuilder,
            ISearchQueryBuilder queryBuilder,
            IAbstractContextFactory abstractContextFactory,
             IQueryableHelper queryableHelper)
            : base(resultsBuilder, queryBuilder, abstractContextFactory, queryableHelper)
        {
        }
    }


    /// <summary>
    /// Class EFCoreSearchRepositoryBase.
    /// Implements the <see cref="TridentOptionsBuilder.EFCore.EFCoreSearchRepositoryBase{TEntity, TEntity}" />
    /// Implements the <see cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity, TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.EFCore.EFCoreSearchRepositoryBase{TEntity, TEntity}" />
    /// <seealso cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity, TEntity}" />
    public abstract class EFCoreSearchRepositoryBase<TEntity> : EFCoreSearchRepositoryBase<TEntity, Domain.Lookup, TEntity>, ISearchRepository<TEntity>
     where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreSearchRepositoryBase{TEntity}" /> class.
        /// </summary>
        /// <param name="resultsBuilder">The results builder.</param>
        /// <param name="queryBuilder">The query builder.</param>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        public EFCoreSearchRepositoryBase(
            ISearchResultsBuilder resultsBuilder,
            ISearchQueryBuilder queryBuilder,
            IAbstractContextFactory abstractContextFactory,
             IQueryableHelper queryableHelper)
            : base(resultsBuilder, queryBuilder, abstractContextFactory, queryableHelper)
        {
        }
    }
}


