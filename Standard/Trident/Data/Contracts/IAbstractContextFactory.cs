using System;

namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface IAbstractContextFactory
    /// </summary>
    public interface IAbstractContextFactory
    {
        /// <summary>
        /// Creates the specified entity type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>T.</returns>
        T Create<T>(Type entityType);       
    }
}
