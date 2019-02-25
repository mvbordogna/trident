using System.Collections.Generic;

namespace Trident.Search
{
    /// <summary>
    /// Interface ISearchResultsBuilder
    /// </summary>
    public interface ISearchResultsBuilder
    {
        /// <summary>
        /// Builds the specified results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="results">The results.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns>SearchResults&lt;T, C&gt;.</returns>
        SearchResults<T, C> Build<T, C>(IEnumerable<T> results, C criteria, int totalRecords)
           where C : SearchCriteria;
    }
}