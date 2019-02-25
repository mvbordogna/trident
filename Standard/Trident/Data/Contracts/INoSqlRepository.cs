namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface INoSqlRepository
    /// Implements the <see cref="Trident.Data.Contracts.IRepositoryBase{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Data.Contracts.IRepositoryBase{TEntity}" />
    public interface INoSqlRepository<TEntity>: IRepositoryBase<TEntity> where TEntity: class
    {
    }
}
