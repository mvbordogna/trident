using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Trident.Data.Contracts;

namespace Trident.Data
{
    /// <summary>
    /// Base impelenation of a CRUD operations Repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Data.Contracts.ITransactionalRepository{TEntity}" />
    public abstract class RepositoryBase<TEntity> : ReadOnlyRepositoryBase<TEntity>, ITransactionalRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected RepositoryBase(Lazy<IContext> context) : base(context) { }

        /// <summary>
        /// Returns a value indicating if any entity exists matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public abstract Task<bool> Exists(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Gets the entities matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
        public abstract Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, IEnumerable<string> includeProperties = null, bool noTracking = false);

        public abstract Task<IEnumerable<TEntity>> GetByIds<TEntityId>(IEnumerable<TEntityId> ids, bool detach = false);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        public virtual async Task<TEntity> GetById(object id, bool detach = false)
        {
            return await Context.FindAsync<TEntity>(id, detach);
        }

        public TEntity GetByIdSync(object id, bool detach = false)
        {
            return Context.Find<TEntity>(id, detach);
        }


        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        public virtual async Task Insert(TEntity entity, bool deferCommit = false)
        {
            Context.Add(entity);
            if (!deferCommit)
            {
                await Context.SaveChangesAsync();
            }
        }
     
        public void InsertSync(TEntity entity, bool deferCommit = false)
        {
            Context.Add(entity);
            if (!deferCommit)
            {
                Context.SaveChanges();
            }
        }


        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        public virtual async Task Delete(TEntity entity, bool deferCommit = false)
        {
            Context.Delete(entity);
            if (!deferCommit)
            {
                await Context.SaveChangesAsync();
            }
        }


        public void DeleteSync(TEntity entity, bool deferCommit = false)
        {
            Context.Delete(entity);
            if (!deferCommit)
            {
                Context.SaveChanges();
            }
        }



        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        public virtual async Task Update(TEntity entity, bool deferCommit = false)
        {
            Context.Update(entity);
            if (!deferCommit)
            {
                await Context.SaveChangesAsync();
            }
        }

        public void UpdateSync(TEntity entity, bool deferCommit = false)
        {
            Context.Update(entity);
            if (!deferCommit)
            {
                Context.SaveChanges();
            }
        }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task Commit()
        {
            await Context.SaveChangesAsync();
        }

        public void CommitSync()
        {
            Context.SaveChanges();
        }
    }
}
