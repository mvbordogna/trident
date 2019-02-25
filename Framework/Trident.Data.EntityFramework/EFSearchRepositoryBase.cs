using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident.Data.Contracts;
using Trident.Search;
using Trident.Contracts.Enums;
using System.Data.Entity;
using Trident.Domain;
using System;

namespace Trident.Data.EntityFramework
{
    /// <summary>
    /// Abstract Class SearchRepositoryBase.
    /// Implements the <see cref="Trident.Search.ISearchRepository{TEntity, TSummery, TCriteria}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummery">The type of the t summery.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="Trident.Search.ISearchRepository{TEntity, TSummery, TCriteria}" />
    /// <seealso cref="Trident.Data.EntityFramework.EFRepository{TEntity}" />
    /// <seealso cref="Trident.TimeSummit.Repositories.Contracts.ISearchRepositoryBase{TEntity, TSummery, TCriteria}" />
    public abstract class EFSearchRepositoryBase<TEntity, TSummery, TCriteria> : EFRepository<TEntity>, ISearchRepository<TEntity, TSummery, TCriteria>
        where TEntity : Entity
        where TSummery : class
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

        /// <summary>
        /// Initializes a new instance of the <see cref="EFSearchRepositoryBase{TEntity, TSummery, TCriteria}" /> class.
        /// </summary>
        /// <param name="resultsBuilder">The results builder.</param>
        /// <param name="queryBuilder">The query builder.</param>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        public EFSearchRepositoryBase(
            ISearchResultsBuilder resultsBuilder,
            ISearchQueryBuilder queryBuilder,
            IAbstractContextFactory abstractContextFactory) : base(abstractContextFactory)
        {
            _resultsBuilder = resultsBuilder;
            _queryBuilder = queryBuilder;
        }

        /// <summary>
        /// Searches the specified search criteria.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="includedProperties">The included properties.</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        /// <exception cref="System.Exception"></exception>
        public virtual async Task<SearchResults<TSummery, TCriteria>> Search(TCriteria searchCriteria, IEnumerable<String> includedProperties = null)
        {
            var query = base.Context.Query<TSummery>();

            //apply keyword search
            query = this.ApplyKeywordSearch(query, searchCriteria.Keywords);

            //apply filters           
            query = ApplyFilter(query, searchCriteria);

            if(includedProperties != null)
            query = includedProperties
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

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

            // Get total Records before returning results
            var totalRecords = await query.CountAsync();

            //apply paging
            query = ApplyPaging(query, searchCriteria);

            try
            {
                var results = await query.ToListAsync();

                return SearchResultContent(results, searchCriteria, totalRecords);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Then implemented in a derivied class, applies filters given the specified keywords string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="keywords">The keywords.</param>
        /// <returns>IQueryable&lt;TSummery&gt;.</returns>
        protected virtual IQueryable<TSummery> ApplyKeywordSearch(IQueryable<TSummery> source,  string keywords)
        {
            return source;
        }

        /// <summary>
        /// Applies paging to the specified query given the search criteria current page and page size.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        protected virtual IQueryable<TSummery> ApplyPaging(IQueryable<TSummery> source, TCriteria searchCriteria)
        {
            return _queryBuilder.ApplyPaging(source, searchCriteria);
        }

        /// <summary>
        /// Applies the order by clauses to the IQueryable given the specified dictionary.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filterBy">The filter by.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        protected virtual IQueryable<TSummery> ApplyOrderBy(IQueryable<TSummery> source, Dictionary<string, SortOrder> filterBy)
        {
            return _queryBuilder.ApplyOrderBy(source, filterBy);
        }

        /// <summary>
        /// Applies filtering to the specified query given the internal FilterBag Object member of SearchCriteria.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>IQueryable&lt;TSummery&gt;.</returns>
        protected virtual IQueryable<TSummery> ApplyFilter(IQueryable<TSummery> source, SearchCriteria criteria)
        {
           
            var filters = (criteria?.Filters?.Any() ?? false) 
                ? _queryBuilder.ApplyFilter(source, criteria, Context) 
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
    }


    /// <summary>
    /// Class EFSearchRepositoryBase.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummery">The type of the t summery.</typeparam>
    /// <seealso cref="Trident.Data.EntityFramework.EFSearchRepositoryBase{TEntity, TSummery, Trident.Search.SearchCriteria}" />
    /// <seealso cref="Trident.Search.ISearchRepository{TEntity, TSummery, Trident.Search.SearchCriteria}" />
    public abstract class EFSearchRepositoryBase<TEntity, TSummery> : EFSearchRepositoryBase<TEntity, TSummery, SearchCriteria>, ISearchRepository<TEntity, TSummery, SearchCriteria>
       where TEntity : Entity
       where TSummery : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EFSearchRepositoryBase{TEntity, TSummery}"/> class.
        /// </summary>
        /// <param name="resultsBuilder">The results builder.</param>
        /// <param name="queryBuilder">The query builder.</param>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        public EFSearchRepositoryBase(
            ISearchResultsBuilder resultsBuilder,
            ISearchQueryBuilder queryBuilder,
            IAbstractContextFactory abstractContextFactory)
            : base(resultsBuilder, queryBuilder, abstractContextFactory)
        {
        }
    }

    /// <summary>
    /// Class EFSearchRepositoryBase.
    /// Implements the <see cref="Trident.Data.EntityFramework.EFSearchRepositoryBase{TEntity, TEntity}" />
    /// Implements the <see cref="Trident.Search.ISearchRepository{TEntity, TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Data.EntityFramework.EFSearchRepositoryBase{TEntity, TEntity}" />
    /// <seealso cref="Trident.Search.ISearchRepository{TEntity, TEntity}" />
    public abstract class EFSearchRepositoryBase<TEntity> : EFSearchRepositoryBase<TEntity, TEntity>, ISearchRepository<TEntity, TEntity>
     where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EFSearchRepositoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="resultsBuilder">The results builder.</param>
        /// <param name="queryBuilder">The query builder.</param>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        public EFSearchRepositoryBase(
            ISearchResultsBuilder resultsBuilder,
            ISearchQueryBuilder queryBuilder,
            IAbstractContextFactory abstractContextFactory)
            : base(resultsBuilder, queryBuilder, abstractContextFactory)
        {
        }
    }

}


