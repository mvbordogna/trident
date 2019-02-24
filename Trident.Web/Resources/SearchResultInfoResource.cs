namespace Trident.Web.Resources
{
    public class SearchResultInfoResource<T> : SearchCriteriaResource where T : SearchCriteriaResource
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

        public T PreviousPageCriteria { get; set; }

        public T NextPageCriteria { get; set; }
    }
}
