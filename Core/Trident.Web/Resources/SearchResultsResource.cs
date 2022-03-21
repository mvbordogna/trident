using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Web.Resources
{
    public class SearchResultsResource<T, C>
        where C : SearchCriteriaResource
    {

        public SearchResultsResource(IEnumerable<T> results) : this()
        {
            this.Results = results;
            Info.PageSize = results.Count();
        }

        public SearchResultsResource()
        {
            Info = new SearchResultInfoResource<C>();
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
        public SearchResultInfoResource<C> Info { get; set; }
    }
}
