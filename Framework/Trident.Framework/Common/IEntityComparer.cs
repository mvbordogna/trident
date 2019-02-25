using Trident.Domain;
using System.Collections.Generic;

namespace Trident.Common
{
    /// <summary>
    /// Interface for entity comparison.
    /// </summary>
    public interface IEntityComparer
    {
        /// <summary>
        /// When implemented in derivied classes, provides the abilty to compare the entity to another entity of the same type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="original">The original.</param>
        /// <param name="target">The target.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>IEnumerable&lt;EntityCompareResult&gt;.</returns>
        IEnumerable<EntityCompareResult> Compare<TEntity>(TEntity original, TEntity target, EntityCompareConfig config) where TEntity : class;
    }
}
