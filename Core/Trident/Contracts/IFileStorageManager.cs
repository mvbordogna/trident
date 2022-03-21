using System.Threading.Tasks;
using Trident.Domain;

namespace Trident.Contracts
{
    /// <summary>
    /// Interface IFileStorageManager
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IManager" />
    public interface IFileStorageManager : IManager
    {

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task&lt;FileStorageEntity&gt;.</returns>
        Task<FileStorageEntity> Save(FileStorageEntity entity);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;FileStorageEntity&gt;.</returns>
        Task<FileStorageEntity> GetById(string id);

        /// <summary>
        /// Existses the specified storage key.
        /// </summary>
        /// <param name="storageKey">The storage key.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> Exists(string storageKey);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task&lt;FileStorageEntity&gt;.</returns>
        Task<FileStorageEntity> Insert(FileStorageEntity entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task&lt;FileStorageEntity&gt;.</returns>
        Task<FileStorageEntity> Update(FileStorageEntity entity);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task.</returns>
        Task Delete(FileStorageEntity entity);
    }
}
