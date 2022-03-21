using Trident.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Trident.Data.Contracts;

namespace Trident.Search
{
    /// <summary>
    /// Class that provides generic query building capabilities between SearchCriteria and IQueryable&lt;T&gt;
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Search.ISearchQueryBuilder" />
    public class SearchQueryBuilder : ISearchQueryBuilder
    {
        static Dictionary<CompareOperators, Func<Expression, Expression, BinaryExpression>> _operatorDict
             = new Dictionary<CompareOperators, Func<Expression, Expression, BinaryExpression>>()
             {
                { CompareOperators.eq,  Expression.Equal },
                { CompareOperators.ne,  Expression.NotEqual },
                { CompareOperators.gt,  Expression.GreaterThan },
                { CompareOperators.gte,  Expression.GreaterThanOrEqual },
                { CompareOperators.lt,  Expression.LessThan },
                { CompareOperators.lte,  Expression.LessThanOrEqual },
                { CompareOperators.contains,  (a,b) => TypeExtensions. GetStringEvalOperationExpression(nameof(String.Contains), a, b) },
                { CompareOperators.startsWith,  (a,b) => TypeExtensions. GetStringEvalOperationExpression(nameof(String.StartsWith), a, b) },
                { CompareOperators.endWith,  (a,b) => TypeExtensions. GetStringEvalOperationExpression(nameof(String.EndsWith), a, b) }
             };



        /// <summary>
        /// The complex filter factory
        /// </summary>
        private readonly IComplexFilterFactory _complexFilterFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchQueryBuilder"/> class.
        /// </summary>
        /// <param name="complexFilterFactory">The complex filter factory.</param>
        public SearchQueryBuilder(IComplexFilterFactory complexFilterFactory)
        {
            _complexFilterFactory = complexFilterFactory;
        }

        /// <summary>
        /// Applies paging to the specified query given the search criteria current page and page size.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        public IQueryable<T> ApplyPaging<T, TCriteria>(IQueryable<T> source, TCriteria searchCriteria)
            where T : class
            where TCriteria : SearchCriteria
        {
            if (searchCriteria.PageSize.GetValueOrDefault() != 0 && searchCriteria.PageSize.GetValueOrDefault() != int.MaxValue)
            {
                if (searchCriteria.CurrentPage.GetValueOrDefault() > 0)
                {
                    source = source.Skip(searchCriteria.CurrentPage.GetValueOrDefault() * searchCriteria.PageSize.GetValueOrDefault());
                }

                source = source.Take(searchCriteria.PageSize.GetValueOrDefault());
            }

            return source;
        }

        private static LambdaExpression CreateExpression(Type type, string propertyName)
        {
            var param = Expression.Parameter(type, "x");
            Expression body = param;
            foreach (var member in propertyName.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }
            return Expression.Lambda(body, param);
        }




        /// <summary>
        /// Applies the order by clauses to the IQueryable given the specified dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="orderBy">The filter by.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public IQueryable<T> ApplyOrderBy<T>(IQueryable<T> source, Dictionary<string, SortOrder> orderBy)
        {
            var i = 0;
            string orderByMethod = string.Empty;

            foreach (var key in orderBy.Keys)
            {
                var type = typeof(T);
                var property = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(x => string.Compare( x.Name, key, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (property == null)
                    throw new ArgumentException($"{key} is not a valid member of {type.FullName}");

                var orderByExp = CreateExpression(type, key);
                var propertyType = orderByExp.Body.Type;

                if (i == 0)
                {
                    orderByMethod = orderBy[key] == SortOrder.Asc
                            ? "OrderBy"
                            : "OrderByDescending";
                }
                else
                {
                    orderByMethod = orderBy[key] == SortOrder.Asc
                           ? "ThenBy"
                           : "ThenByDescending";
                }

                MethodCallExpression resultExp = Expression.Call(typeof(Queryable), orderByMethod,
                    new Type[] { type, property.PropertyType },
                    source.Expression, Expression.Quote(orderByExp));

                source = source.Provider.CreateQuery<T>(resultExp);

                i++;
            }

            return source;
        }

        /// <summary>
        /// Applies filtering to the specified query given the dictionary of filter information.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="context">The context.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.NotSupportedException">Complex filter values are only supported using ComplexFilterStrategies.</exception>
        /// <exception cref="Exception">Unknown type : " + keyPropertyExpression.Type.ToString()</exception>
        public IQueryable<T> ApplyFilter<T, TCriteria>(IQueryable<T> source, TCriteria criteria, IContext context = null)
          where T : class
          where TCriteria : SearchCriteria
        {
            var filterBy = criteria.Filters;

            foreach (var key in filterBy.Keys)
            {
                var filter = _complexFilterFactory.GetFilter<T>(key);
                if (filter != null)
                {
                    source = filter.Apply(source, filterBy[key], context);
                }
                else
                {
                    Expression<Func<T, bool>> ce = null;
                    var filterValueType = filterBy[key]?.GetType();
                    if (filterValueType == typeof(AxiomFilter))
                    {
                        var af = (AxiomFilter)filterBy[key];
                        ce = af.ToExpression<T>();                       
                    }                  
                    else if (typeof(Axiom).IsAssignableFrom(filterValueType) )
                    {                       
                        var ax = (Axiom)filterBy[key];
                        ce = ax.ToExpression<T>();
                    }
                    else if (typeof(Compare).IsAssignableFrom(filterValueType))
                    {
                        var comparer = (Compare)filterBy[key];
                        var ax = new Axiom(comparer, key);
                        ce = ax.ToExpression<T>();
                    }
                    else
                    {
                        ce = new Axiom()
                        {
                            Field = key,
                            Value = filterBy[key],
                            Operator = CompareOperators.eq
                        }.ToExpression<T>();                       
                    }

                    source = source.Where(ce);
                }
            }

            return source = _complexFilterFactory.ApplyAdapterFilters(source, criteria, context);
        }

      

        /// <summary>
        /// Applies any filters found in the criteria Filterbag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>IQueryable&lt;T&gt;.</returns>
        public IQueryable<T> ApplyFilterBag<T>(IQueryable<T> source, SearchCriteria criteria)
        {
            var filterFunc = criteria.DefaultFilterBag as Expression<Func<T, bool>>;
            if (filterFunc != null)
            {
                source = source.Where(filterFunc);
            }

            return source;
        }

    }
}
