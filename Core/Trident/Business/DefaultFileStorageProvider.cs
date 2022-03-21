using Trident.Data.Contracts;
using Trident.Domain;
using System.Threading.Tasks;

namespace Trident.Business
{
    /// <summary>
    /// Implemenatation of a file storage provider.
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IFileStorageProvider" />
    public class DefaultFileStorageProvider : IFileStorageProvider
    {
        private readonly IFileStorageRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public DefaultFileStorageProvider(IFileStorageRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task.</returns>
        public virtual async Task Delete(FileStorageEntity entity)
        {
            await _repository.Delete(entity);
        }

        /// <summary>
        /// Determines of any entities exist matching the specified filter.
        /// </summary>
        /// <param name="storageKey">The storage key.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> Exists(string storageKey)
        {
            return await _repository.Exists(storageKey);
        }

        /// <summary>
        /// Gets entity matching the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        public virtual async Task<FileStorageEntity> GetById(string id)
        {
            return await _repository.GetById(id);
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task.</returns>
        public virtual async Task Insert(FileStorageEntity entity)
        {
            await _repository.Insert(entity);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task.</returns>
        public virtual async Task Update(FileStorageEntity entity)
        {
            await _repository.Update(entity);
        }
    }
}
