using Trident.Contracts;
using Trident.Domain;
using System.Threading.Tasks;

namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface IFileStorageProvider
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IProvider" />
    public interface IFileStorageProvider : IProvider
    {
        /// <summary>
        /// Gets entity matching the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<FileStorageEntity> GetById(string id);

        /// <summary>
        /// Determines of any entities exist matching the specified filter.
        /// </summary>
        /// <param name="storageKey">The storage key.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> Exists(string storageKey);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task.</returns>
        Task Insert(FileStorageEntity entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task.</returns>
        Task Update(FileStorageEntity entity);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task.</returns>
        Task Delete(FileStorageEntity entity);
    }
}
