using Trident.Data.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trident.Search
{
    /// <summary>
    /// Class ComplexFilterBase.
    /// Implements the <see cref="TridentOptionsBuilder.Search.IComplexFilter{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TFilterValue">The type of the t filter value.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Search.IComplexFilter{T}" />
    public abstract class ComplexFilterBase<T, TFilterValue> : IComplexFilter<T>
        where T : class
    {
        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        /// <value>The name of the filter.</value>
        public abstract string FilterName { get; }

        /// <summary>
        /// Applies the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="context">The context.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        public IQueryable<T> Apply(IQueryable<T> source, object filterValue, IContext context)
        {  
            TFilterValue filterVal = TypeExtensions.ParseToTypedObject<TFilterValue>(filterValue);               
            return this.ApplyFilter(source, filterVal, context);
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="context">The context.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        protected abstract IQueryable<T> ApplyFilter(IQueryable<T> source, TFilterValue filterValue, IContext context);

    }
}
