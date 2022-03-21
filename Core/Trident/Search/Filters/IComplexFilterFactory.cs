using Trident.Data.Contracts;
using System.Linq;

namespace Trident.Search
{
    /// <summary>
    /// Provides an interface for applies complex filters
    /// </summary>
    public interface IComplexFilterFactory
    {
        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>IComplexFilter&lt;T&gt;.</returns>
        IComplexFilter<T> GetFilter<T>(string key)
            where T : class;
        /// <summary>
        /// Applies the adapter filters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="context">The context.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        IQueryable<T> ApplyAdapterFilters<T, TCriteria>(IQueryable<T> source, TCriteria criteria, IContext context)
             where T : class         
             where TCriteria : SearchCriteria;
    }

}
