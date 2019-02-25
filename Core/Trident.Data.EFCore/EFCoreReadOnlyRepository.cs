using Trident.Data.Contracts;
using System;

namespace Trident.Data.EntityFramework.EFCore
{
    /// <summary>
    /// Class EFCoreReadOnlyRepository.
    /// Implements the <see cref="Trident.Data.ReadOnlyRepositoryBase{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Data.ReadOnlyRepositoryBase{TEntity}" />
    public abstract class EFCoreReadOnlyRepository<TEntity> : ReadOnlyRepositoryBase<TEntity>
        where TEntity : class
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EFCoreReadOnlyRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="abstractContextFactory">The abstract context factory.</param>
        protected EFCoreReadOnlyRepository(IAbstractContextFactory abstractContextFactory)
             : base(new Lazy<IContext>(()=> abstractContextFactory.Create<IContext>(typeof(TEntity))))
        { }
    }
}
