using System.Collections.Generic;
using Trident.Contracts.Enums;

namespace Trident.Web.Resources
{
    public class SearchCriteriaResource
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCriteria"/> class.
        /// </summary>
        public SearchCriteriaResource()
        {
            MultiOrderBy = new Dictionary<string, SortOrder>();
            Filters = new Dictionary<string, string>();
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
        public Dictionary<string, string> Filters { get; set; }

        public bool ApplyDefaultFilters { get; set; } = true;

        public bool LoadChildren { get; set; } = false;
    }
}
