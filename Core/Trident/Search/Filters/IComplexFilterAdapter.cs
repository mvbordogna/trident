using Trident.Data.Contracts;
using System.Linq;

namespace Trident.Search
{
    /// <summary>
    /// Interface IComplexFilterAdapter
    /// </summary>
    public interface IComplexFilterAdapter { }

    /// <summary>
    /// Interface IComplexFilterAdapter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSearchCriteria">The type of the t search criteria.</typeparam>
    public interface IComplexFilterAdapter<T, TSearchCriteria>
       where T : class    
       where TSearchCriteria : SearchCriteria
    {
        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="context">The context.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        IQueryable<T> ApplyFilter(IQueryable<T> source, TSearchCriteria criteria, IContext context);
    }

    /// <summary>
    /// Interface IComplexFilterAdapter
    /// Implements the <see cref="TridentOptionsBuilder.Search.IComplexFilterAdapter{T, TSearchCriteria}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSearchCriteria">The type of the t search criteria.</typeparam>
    /// <typeparam name="TCustomFilter">The type of the t custom filter.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Search.IComplexFilterAdapter{T, TSearchCriteria}" />
    public interface IComplexFilterAdapter<T, TSearchCriteria, TCustomFilter>: IComplexFilterAdapter<T, TSearchCriteria>
        where T : class
        where TCustomFilter : class
        where TSearchCriteria : SearchCriteria<TCustomFilter>
    {  
    }
}


