using Trident.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Search
{
    /// <summary>
    /// Class ComplexFilterFactory.
    /// Implements the <see cref="TridentOptionsBuilder.Search.IComplexFilterFactory" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Search.IComplexFilterFactory" />
    public class ComplexFilterFactory : IComplexFilterFactory
    {
        /// <summary>
        /// The registered filters
        /// </summary>
        private Dictionary<Type, Dictionary<string, IComplexFilter>> RegisteredFilters = null;
        /// <summary>
        /// The registered adapters
        /// </summary>
        private Dictionary<Type, IEnumerable<IComplexFilterAdapter>> RegisteredAdapters = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexFilterFactory"/> class.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <param name="filterAdapters">The filter adapters.</param>
        public ComplexFilterFactory(IEnumerable<IComplexFilter> filters, IEnumerable<IComplexFilterAdapter> filterAdapters)
        {
            RegisteredFilters = filters.GroupBy(x => {

                var tempType = x.GetType();
                while (tempType != null && !tempType.IsGenericType || (tempType.IsGenericType && tempType.GetGenericTypeDefinition() != typeof(ComplexFilterBase<,>)))
                {
                    tempType = tempType.BaseType;
                }

                if (tempType == null)
                {
                    throw new InvalidOperationException($"{tempType.FullName} if used as an IComplexFilter type must inherit from ComplexFilterBase<,>");
                }

                return tempType.GetGenericArguments().First();
            })
                 .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.FilterName));

            RegisteredAdapters = filterAdapters.GroupBy(x => {

                var tempType = x.GetType();
                while (tempType != null && !tempType.IsGenericType || (tempType.IsGenericType && tempType.GetGenericTypeDefinition() != typeof(ComplexFilterAdapterBase<,,>)))
                {
                    tempType = tempType.BaseType;
                }

                if (tempType == null)
                {
                    throw new InvalidOperationException($"{tempType.FullName} if used as an IComplexFilterAdapter type must inherit from ComplexFilterAdapterBase<,,>");
                }

                return tempType.GetGenericArguments().First();
            })
                 .ToDictionary(x => x.Key, x => x.AsEnumerable());
        }

        /// <summary>
        /// Applies the adapter filters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="context">The context.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        public IQueryable<T> ApplyAdapterFilters<T, TCriteria>(IQueryable<T> source, TCriteria criteria, IContext context)
            where T : class
            where TCriteria : SearchCriteria
        {
            if (RegisteredAdapters.Keys.Contains(typeof(T)))
            {
                if (typeof(SearchCriteria<>).IsAssignableFrom(typeof(TCriteria)))
                {
                    var typedFilterAdapters = RegisteredAdapters[typeof(T)];

                    foreach (var adapter in typedFilterAdapters)
                    {
                        var typedAdapted = adapter as IComplexFilterAdapter<T, TCriteria>;
                        if (typedAdapted != null)
                        {
                            source = typedAdapted.ApplyFilter(source, criteria, context);
                        }
                    }
                }
                else
                {
                    var errMsg = $"To use ComplexFilterAdapterBase, SearchCriteria of {typeof(TCriteria).Name} must inherit or be an instance of SearchCriteria<T>";
                    throw new System.InvalidCastException(errMsg);
                }
            }

            return source;
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>IComplexFilter&lt;T&gt;.</returns>
        public IComplexFilter<T> GetFilter<T>(string key)
            where T : class
        {
            if (RegisteredFilters.Keys.Contains(typeof(T)))
            {
                var typedFilterDict = RegisteredFilters[typeof(T)];
                if (typedFilterDict.Keys.Contains(key))
                {
                    return typedFilterDict[key] as IComplexFilter<T>;
                }
            }

            return null;
        }
    }
}
