using Trident.Contracts;
using Trident.Data.Contracts;
using Trident.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Rest.Contracts
{
    /// <summary>
    /// Interface IRestRepository
    /// Implements the <see cref="TridentOptionsBuilder.Data.Contracts.IRepositoryBase{TEntity}" />
    /// Implements the <see cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TTargetEntity">The type of the t target entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the t entity identifier.</typeparam>
    /// <typeparam name="TTargetEntityId">The type of the t target entity identifier.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IRepositoryBase{TEntity}" />
    /// <seealso cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity}" />
    public interface IRestRepository<TEntity, TTargetEntity, TEntityId, TTargetEntityId>
        : IRepositoryBase<TEntity>, ISearchRepository<TEntity>
           where TEntity : class, IHaveId<TEntityId>
           where TTargetEntity : class, IHaveId<TTargetEntityId>
    {
        /// <summary>
        /// Searches the source.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="includedProperties">The included properties.</param>
        /// <returns>Task&lt;SearchResults&lt;TTargetEntity, SearchCriteria&gt;&gt;.</returns>
        Task<SearchResults<TTargetEntity, SearchCriteria>> SearchSource(SearchCriteria searchCriteria, IEnumerable<string> includedProperties = null);
    }
}
