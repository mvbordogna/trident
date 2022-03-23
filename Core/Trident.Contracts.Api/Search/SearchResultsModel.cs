using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Api.Search
{
    /// <summary>
    /// Class SearchResults.
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    /// <typeparam name="C">Entity Search Critiera Type</typeparam>
    public class SearchResultsModel<T, C> where C : SearchCriteriaModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResults{T, C}"/> class.
        /// </summary>
        /// <param name="results">The results.</param>
        public SearchResultsModel(IEnumerable<T> results) : this()
        {
            this.Results = results;
            Info.PageSize = results.Count();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResults{T, C}"/> class.
        /// </summary>
        public SearchResultsModel()
        {
            Info = new SearchResultInfoModel<C>();
            Results = new List<T>();
        }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public IEnumerable<T> Results { get; set; }

        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>The information.</value>
        public SearchResultInfoModel<C> Info { get; set; }
    }
}
