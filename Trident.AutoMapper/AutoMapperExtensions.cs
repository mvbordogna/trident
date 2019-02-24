using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trident.Mapper
{

    /// <summary>
    /// Adds Extension methods to the public accessible types for AutoMapper.
    /// </summary>
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Use this method before any forMember invocation mapping definions It will clear all mappings,
        /// NOTE: Property names that match between two types will automatically have a map create for them,
        /// this is a way to clear all mappings, the only add the ones you want.
        /// if use
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TDest">The type of the t dest.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>IMappingExpression&lt;TSource, TDest&gt;.</returns>
        public static IMappingExpression<TSource, TDest> ClearAllMappings<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        {
            expression.ForAllMembers(opt => opt.Ignore());
            return expression;
        }

        /// <summary>
        /// Ignores all non existing.
        /// This is used to support legacy mappings that were not explicitly ignoring unmatched properies.
        /// NOTE: **this should not be used for new code
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TDestination">The type of the t destination.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>IMappingExpression&lt;TSource, TDestination&gt;.</returns>
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var sourceProperties = new HashSet<string>(typeof(TSource).GetProperties(flags)
                .Select(x => x.Name.ToLower()).Distinct());
            var destinationProperties = typeof(TDestination).GetProperties(flags);

            foreach (var property in destinationProperties)
            {
                if (!sourceProperties.Contains(property.Name.ToLower()))
                {
                    expression.ForMember(property.Name, opt => opt.Ignore());
                }
            }
            return expression;
        }
    }
}
