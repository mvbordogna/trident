using System;
using Trident.Contracts.Enums;

namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface ISharedContextFactory
    /// </summary>
    public interface ISharedContextFactory { }
    /// <summary>
    /// Interface ISharedContextFactory
    /// Implements the <see cref="TridentOptionsBuilder.Data.Contracts.ISharedContextFactory" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.ISharedContextFactory" />
    public interface ISharedContextFactory<out T>:ISharedContextFactory
    {
        /// <summary>
        /// Gets the specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="dataSource">The data source.</param>
        /// <returns>T.</returns>
        T Get(Type entityType, string dataSource);
    }
}
