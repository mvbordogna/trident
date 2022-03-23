using System.Threading.Tasks;

namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface IQueueRepository
    /// Implements the <see cref="TridentOptionsBuilder.Data.Contracts.IRepositoryBase" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IRepositoryBase" />
    public interface IQueueRepository<TEntity>: IRepository
    {
        /// <summary>
        /// Checks for the existance of the queue.
        /// </summary>
        /// <returns>true if the queue exists; otherwise false.</returns>
        Task<bool> Exists();

        /// <summary>
        /// Creates the queue if it doesn't already exists.
        /// </summary>
        /// <returns>true if the queue did not alreay exist and was created; otherwise false.</returns>
        Task<bool> CreateIfNotExists();

        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        /// <returns>TEntity.</returns>
        Task<TEntity> Dequeue();

        /// <summary>
        /// Enqueues the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task.</returns>
        Task Enqueue(TEntity entity);

        /// <summary>
        /// Enqueues the specified entity. Creates the queue if it does not exist.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="createQueueIfNotExists">if set to <c>true</c> [create queue if not exists].</param>
        /// <returns>Task.</returns>
        Task Enqueue(TEntity entity, bool createQueueIfNotExists);
    }
}
