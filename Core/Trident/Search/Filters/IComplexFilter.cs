using Trident.Data.Contracts;
using System.Linq;

namespace Trident.Search
{
    /// <summary>
    /// Interface IComplexFilter
    /// </summary>
    public interface IComplexFilter
    {
        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        /// <value>The name of the filter.</value>
        string FilterName { get; }
    }

    /// <summary>
    /// Interface IComplexFilter
    /// Implements the <see cref="TridentOptionsBuilder.Search.IComplexFilter" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TridentOptionsBuilder.Search.IComplexFilter" />
    public interface IComplexFilter<T> : IComplexFilter
        where T : class
    {
        /// <summary>
        /// Applies the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="context">The context.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        IQueryable<T> Apply(IQueryable<T> source, object filterValue, IContext context);
    }

}
