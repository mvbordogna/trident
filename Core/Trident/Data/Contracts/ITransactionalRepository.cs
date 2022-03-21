using System.Threading.Tasks;

namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface ITransactionalRepository
    /// Implements the <see cref="TridentOptionsBuilder.Data.Contracts.IRepository{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IRepository{TEntity}" />
    public interface ITransactionalRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Commits this instance.
        /// </summary>
        /// <returns>Task.</returns>
        Task Commit();

        /// <summary>
        /// Commits this instance.
        /// </summary>
        /// <returns>Task.</returns>
        void CommitSync();
    }
}
