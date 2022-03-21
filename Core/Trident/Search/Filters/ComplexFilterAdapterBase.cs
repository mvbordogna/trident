using System.Linq;
using Trident.Data.Contracts;

namespace Trident.Search
{

    /// <summary>
    /// Class ComplexFilterAdapterBase.
    /// Implements the <see cref="TridentOptionsBuilder.Search.IComplexFilterAdapter{T, TSearchCriteria, TCustomFilter}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSearchCriteria">The type of the t search criteria.</typeparam>
    /// <typeparam name="TCustomFilter">The type of the t custom filter.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Search.IComplexFilterAdapter{T, TSearchCriteria, TCustomFilter}" />
    public abstract class ComplexFilterAdapterBase<T, TSearchCriteria, TCustomFilter> : IComplexFilterAdapter<T, TSearchCriteria, TCustomFilter>
       where T : class
       where TCustomFilter : class
       where TSearchCriteria : SearchCriteria<TCustomFilter>
    {
        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="customFilter">The custom filter.</param>
        /// <param name="context">The context.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        protected abstract IQueryable<T> ApplyFilter(IQueryable<T> source, TCustomFilter customFilter, IContext context);

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="context">The context.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        public virtual IQueryable<T> ApplyFilter(IQueryable<T> source, TSearchCriteria criteria, IContext context)
        {
           
                var typedCriteria = criteria as SearchCriteria<TCustomFilter>;
                if (typedCriteria != null)
                {
                    return ApplyFilter(source, typedCriteria.CustomFilter, context);
                }
                return source;           
        }
    }

}
