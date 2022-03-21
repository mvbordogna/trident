using Trident.Contracts;
using Trident.Data.Contracts;
using Trident.Domain;
using System;
using System.Threading.Tasks;

namespace Trident.Business
{
    /// <summary>
    /// Provids an implemantion for storing and retriving files.
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Contracts.IFileStorageManager" />
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IFileStorageProvider" />
    public class DefaultFileStorageManager : IFileStorageManager
    {
        private readonly IFileStorageProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFileStorageManager"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public DefaultFileStorageManager(IFileStorageProvider repository)
        {
            _provider = repository;
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task.</returns>
        public virtual async Task Delete(FileStorageEntity entity)
        {
            await  _provider.Delete(entity);
        }

        /// <summary>
        /// Existses the specified storage key.
        /// </summary>
        /// <param name="storageKey">The storage key.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> Exists(string storageKey)
        {
            return await _provider.Exists(storageKey);
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;FileStorageEntity&gt;.</returns>
        public virtual async Task<FileStorageEntity> GetById(string id)
        {
            return await _provider.GetById(id);
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task&lt;FileStorageEntity&gt;.</returns>
        public virtual async Task<FileStorageEntity> Insert(FileStorageEntity entity)
        {
            await FormatStorageKey(entity);
            await _provider.Insert(entity);
            return entity;
        }

        protected virtual Task FormatStorageKey(FileStorageEntity entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task&lt;FileStorageEntity&gt;.</returns>
        public virtual async Task<FileStorageEntity> Save(FileStorageEntity entity)
        {
            await FormatStorageKey(entity);
            await _provider.Update(entity);
            return entity;
        }


        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task&lt;FileStorageEntity&gt;.</returns>
        public virtual async Task<FileStorageEntity> Update(FileStorageEntity entity)
        {
            await FormatStorageKey(entity);
            await _provider.Update(entity);
            return entity;
        }      
    }
}
