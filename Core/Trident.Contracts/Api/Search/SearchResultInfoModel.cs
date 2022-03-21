namespace Trident.Api.Search
{
    /// <summary>
    /// Class SearchResultInfo.
    /// Implements the <see cref="TridentOptionsBuilder.Search.SearchCriteria" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TridentOptionsBuilder.Search.SearchCriteria" />
    public class SearchResultInfoModel<T> : SearchCriteriaModel where T : SearchCriteriaModel
    {
        /// <summary>
        /// Gets the page count.
        /// </summary>
        /// <value>The page count.</value>
        public int PageCount { get; set; }

        /// <summary>
        /// Gets or sets the total records.
        /// </summary>
        /// <value>The total records.</value>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Gets or sets the previous page criteria.
        /// </summary>
        /// <value>The previous page criteria.</value>
        public T PreviousPageCriteria { get; set; }

        /// <summary>
        /// Gets or sets the next page criteria.
        /// </summary>
        /// <value>The next page criteria.</value>
        public T NextPageCriteria { get; set; }
    
    }
}
