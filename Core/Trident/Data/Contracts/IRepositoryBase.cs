using System.Threading.Tasks;

namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface IReadOnlyResponsitory
    /// Implements the <see cref="TridentOptionsBuilder.Data.Contracts.IRepositoryBase" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IRepositoryBase" />
    public interface IReadOnlyRepository<TEntity>: IRepository
        where TEntity : class
    {

    }

    /// <summary>
    /// Interface IRepositoryBase
    /// Implements the <see cref="TridentOptionsBuilder.Data.Contracts.IReadOnlyResponsitory{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IReadOnlyResponsitory{TEntity}" />
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IRepositoryBase" />
    public interface IRepositoryBase<TEntity>: IReadOnlyRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        Task<TEntity> GetById(object id, bool detach=false);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        Task Insert(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        Task Delete(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        Task Update(TEntity entity, bool deferCommit = false);


        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        TEntity GetByIdSync(object id, bool detach = false);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        void InsertSync(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        void DeleteSync(TEntity entity, bool deferCommit = false);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        void UpdateSync(TEntity entity, bool deferCommit = false);
    }   
}