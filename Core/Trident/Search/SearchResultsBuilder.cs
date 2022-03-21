using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trident.Extensions;

namespace Trident.Search
{
    /// <summary>
    /// Class SearchResultsBuilder.
    /// Implements the <see cref="TridentOptionsBuilder.Search.ISearchResultsBuilder" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Search.ISearchResultsBuilder" />
    public class SearchResultsBuilder : ISearchResultsBuilder
    {


        /// <summary>
        /// Searches the content of the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="results">The results.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns>SearchResults&lt;T, C&gt;.</returns>
        public SearchResults<T, C> Build<T, C>(IEnumerable<T> results, C criteria, int totalRecords)
           where C : SearchCriteria
        {

            return new SearchResults<T, C>(results)
            {
                Info = GetSearchResultInfo(criteria, totalRecords),
            };
        }

        /// <summary>
        /// Gets the search result information.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns>SearchResultInfo&lt;C&gt;.</returns>
        private SearchResultInfo<C> GetSearchResultInfo<C>(C criteria, int totalCount) where C : SearchCriteria
        {
            int pageSize = criteria.PageSize.GetValueOrDefault(SearchConstants.DEFAULT_GRID_PAGE_SIZE);
            var pageCount = pageSize > 0 ? (totalCount / pageSize + (totalCount % pageSize > 0 ? 1 : 0)) : 0;

            var info = new SearchResultInfo<C>()
            {
                PageCount = pageCount,
                TotalRecords = totalCount,
                CurrentPage = criteria.CurrentPage ?? 0,
                PageSize = pageSize,
                Keywords = criteria.Keywords,
                Filters = criteria.Filters,
                MultiOrderBy = criteria.MultiOrderBy,
                OrderBy = criteria.OrderBy,
                SortOrder = criteria.SortOrder,             
                SkipCount = criteria.SkipCount,
                ApplyDefaultFilters = criteria.ApplyDefaultFilters,
                ContextBag = criteria.ContextBag,
                DefaultFilterBag = criteria.DefaultFilterBag,
                detach = criteria.detach,

                PreviousPageCriteria = GetPageCriteria(criteria,              
                       (criteria.CurrentPage ?? 0) - 1,
                    pageCount, totalCount),

                NextPageCriteria = GetPageCriteria(criteria,                   
                        ( criteria.CurrentPage ?? 0) + 1,
                    pageCount, totalCount)
            };


            return info;

        }

        /// <summary>
        /// Gets the page criteria.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="currentCriteria">The current criteria.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageCount">The page count.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns>C.</returns>
        private C GetPageCriteria<C>(C currentCriteria, int page, int pageCount, int totalCount)
            where C : SearchCriteria
        {
            var pageCriteria = currentCriteria.Clone();
            if (0 <= page && page <= pageCount - 1)
            {
                int previousPageNumber = currentCriteria.CurrentPage ?? 0;
                pageCriteria.CurrentPage = page;
                currentCriteria.CurrentPage = previousPageNumber;
            }

            return pageCriteria;
        }

    }
}
