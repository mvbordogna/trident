using System;

namespace Trident.Mapper
{
    /// <summary>
    /// MapperRegistry registry interface.
    /// </summary>
    public interface IMapperRegistry
    {
        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="DESTINATION">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>DESTINATION.</returns>
        DESTINATION Map<DESTINATION>(object source)
            where DESTINATION : class;

        /// <summary>
        /// Maps the specified source to the specified destination.
        /// </summary>
        /// <typeparam name="SOURCE">The type of the SOURCE.</typeparam>
        /// <typeparam name="DESTINATION">The type of the DESTINATION.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        void Map<SOURCE, DESTINATION>(SOURCE source, DESTINATION destination)
            where DESTINATION : class
            where SOURCE : class;

        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>TDestination.</returns>
        TDestination Map<TSource, TDestination>(TSource source);

        /// <summary>
        /// Dynamics the map.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>TDestination.</returns>
        TDestination DynamicMap<TSource, TDestination>(TSource source);
        
        /// <summary>
        /// Asserts the configuration is valid.
        /// </summary>
        void AssertConfigurationIsValid();
    }
}
