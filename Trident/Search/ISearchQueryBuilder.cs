using Trident.Contracts.Enums;
using Trident.Data.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Trident.Search
{
    /// <summary>
    /// Interface ISearchQueryBuilder
    /// </summary>
    public interface ISearchQueryBuilder
    {

        /// <summary>
        /// Applies paging to the specified query given the search criteria current page and page size.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        IQueryable<T> ApplyPaging<T, TCriteria>(IQueryable<T> source, TCriteria searchCriteria)
            where T : class
            where TCriteria : SearchCriteria;

        /// <summary>
        /// Applies the order by clauses to the IQueryable given the specified dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filterBy">The filter by.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        IQueryable<T> ApplyOrderBy<T>(IQueryable<T> source, Dictionary<string, SortOrder> filterBy);

        /// <summary>
        /// Applies filtering to the specified query given the dictionary of filter information.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="context">The context.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        IQueryable<T> ApplyFilter<T, TCriteria>(IQueryable<T> source, TCriteria criteria, IContext context = null)
            where T : class
            where TCriteria : SearchCriteria;

        /// <summary>
        /// Applies any filters found in the criteria Filterbag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        IQueryable<T> ApplyFilterBag<T>(IQueryable<T> source, SearchCriteria criteria);
    }
}