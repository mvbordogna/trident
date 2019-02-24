using Newtonsoft.Json;
using Trident.Contracts.Enums;
using System.Collections.Generic;

namespace Trident.Search
{

    /// <summary>
    /// Class SearchCriteria.
    /// </summary>
    public class SearchCriteria
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCriteria" /> class.
        /// </summary>
        public SearchCriteria()
        {
            MultiOrderBy = new Dictionary<string, SortOrder>();
            Filters = new Dictionary<string, object>();
            DefaultFilterBag = null;
            ContextBag = new Dictionary<string, object>();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public SearchCriteria(SearchCriteria criteria)
        {
            Keywords = criteria.Keywords;
            CurrentPage = criteria.CurrentPage;
            PageSize = criteria.PageSize;
            OrderBy = criteria.OrderBy;
            SortOrder = criteria.SortOrder;
            MultiOrderBy = new Dictionary<string, SortOrder>(criteria.MultiOrderBy);
            Filters = new Dictionary<string, object>(criteria.Filters);
            DefaultFilterBag = criteria.DefaultFilterBag;
            ApplyDefaultFilters = criteria.ApplyDefaultFilters;
            ContextBag = new Dictionary<string, object>(criteria.ContextBag);
        }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        public string Keywords { get; set; }
        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        public int? CurrentPage { get; set; }
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int? PageSize { get; set; }
        /// <summary>
        /// Gets or sets the order by.
        /// </summary>
        /// <value>The order by.</value>
        public string OrderBy { get; set; }
        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public SortOrder SortOrder { get; set; }

        /// <summary>
        /// Gets the multi order by.
        /// </summary>
        /// <value>The multi order by.</value>
        public Dictionary<string, SortOrder> MultiOrderBy { get; set; }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public Dictionary<string, object> Filters { get; set; }

        /// <summary>
        /// Gets or sets the default filter bag.
        /// </summary>
        /// <value>The default filter bag.</value>
        [JsonIgnore]
        internal object DefaultFilterBag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [apply default filters].
        /// </summary>
        /// <value><c>true</c> if [apply default filters]; otherwise, <c>false</c>.</value>
        public bool ApplyDefaultFilters { get; set; } = true;

        /// <summary>
        /// Gets or sets the context bag.
        /// </summary>
        /// <value>The context bag.</value>
        public Dictionary<string, object> ContextBag { get; set; }

    }

    /// <summary>
    /// Generically Typed Search Criteria that provides a Custom Filter Object for use with Unmanaged Complex Fitlers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Trident.Search.SearchCriteria" />
    public class SearchCriteria<T> : SearchCriteria
    {
        /// <summary>
        /// Gets or sets the custom filter object to be used with custom implemented filters.
        /// </summary>
        /// <value>The custom filter.</value>
        public T CustomFilter { get; set; }
    }

}
