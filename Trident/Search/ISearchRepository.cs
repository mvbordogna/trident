using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trident.Data.Contracts;

namespace Trident.Search
{
    /// <summary>
    /// Interface ISearchRepositoryBase
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <typeparam name="TCriteria">The type of the t criteria.</typeparam>
    /// <seealso cref="Trident.Data.Contracts.IRepository{TEntity}" />
    public interface ISearchRepository<TEntity, TSummary, TCriteria> : IRepository<TEntity>
        where TEntity : class
        where TSummary : class
        where TCriteria : SearchCriteria
    {

        /// <summary>
        /// Searches the specified search criteria.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        Task<SearchResults<TSummary, TCriteria>> Search(TCriteria searchCriteria, IEnumerable<string> includedProperties = null);
    }
    
    /// <summary>
    /// Interface ISearchRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <seealso cref="Trident.Search.ISearchRepository{TEntity, TSummary, TCriteria}" />
    public interface ISearchRepository<TEntity, TSummary> : ISearchRepository<TEntity, TSummary, SearchCriteria>
       where TEntity : class
       where TSummary : class
    { }

    /// <summary>
    /// Interface ISearchRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="Trident.Search.ISearchRepository{TEntity, TEntity}" />
    public interface ISearchRepository<TEntity> : ISearchRepository<TEntity, TEntity>
      where TEntity : class      
    { }
}
