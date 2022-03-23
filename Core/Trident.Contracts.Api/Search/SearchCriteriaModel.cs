using Trident.Contracts.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trident.Api.Search
{
    /// <summary>
    /// Class SearchCriteria.
    /// </summary>
    [JsonConverter(typeof(SearchCriteriaModelConverter))]
    public class SearchCriteriaModel 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCriteria" /> class.
        /// </summary>
        public SearchCriteriaModel()
        {
            MultiOrderBy = new Dictionary<string, SortOrder>();
            Filters = new Dictionary<string, object>();        
        }

        ///// <summary>
        ///// Copy constructor.
        ///// </summary>
        ///// <param name="criteria">The criteria.</param>
        //public SearchCriteriaModel(SearchCriteria criteria)
        //{
        //    Keywords = criteria.Keywords;
        //    CurrentPage = criteria.CurrentPage;
        //    PageSize = criteria.PageSize;
        //    OrderBy = criteria.OrderBy;
        //    SortOrder = criteria.SortOrder;
        //    MultiOrderBy = new Dictionary<string, SortOrder>(criteria.MultiOrderBy);
        //    Filters = new Dictionary<string, object>(criteria.Filters);       
        //}

        public bool SkipCount { get; set; }

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

        public SearchCriteriaModel AddFilter(string name, object value)
        {
            if (Filters == null)
                Filters = new Dictionary<string, object>();
            Filters[name] = value;
            return this;
        }

        public SearchCriteriaModel AddFilterIf(bool condition, string name, object value)
        {
            if (condition)
                AddFilter(name, value);
            return this;
        }


    }

    /// <summary>
    /// Generically Typed Search Criteria that provides a Custom Filter Object for use with Unmanaged Complex Fitlers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TridentOptionsBuilder.Search.SearchCriteria" />
    public class SearchCriteriaModel<T> : SearchCriteriaModel
    {
        /// <summary>
        /// Gets or sets the custom filter object to be used with custom implemented filters.
        /// </summary>
        /// <value>The custom filter.</value>
        public T CustomFilter { get; set; }
    }

}
