using System.Collections.Generic;
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
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IRepository{TEntity}" />
    public interface ISearchRepository<TEntity, TLookup, TSummary, TCriteria> : IRepository<TEntity>
        where TEntity : class
        where TLookup : Domain.Lookup,new()
        where TSummary : class
        where TCriteria : SearchCriteria
    {

        /// <summary>
        /// Searches the specified search criteria.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        Task<SearchResults<TSummary, TCriteria>> Search(TCriteria searchCriteria, IEnumerable<string> includedProperties = null);

        /// <summary>
        /// Searches the specified search criteria.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns>Task&lt;SearchResults&lt;TSummary, TCriteria&gt;&gt;.</returns>
        SearchResults<TSummary, TCriteria> SearchSync(TCriteria searchCriteria, IEnumerable<string> includedProperties = null);

        Task<SearchResults<TLookup, TCriteria>> SearchLookups(TCriteria criteria, IEnumerable<string> defaultIncludedProperties = null);

        SearchResults<TLookup, TCriteria> SearchLookupsSync(TCriteria criteria, IEnumerable<string> defaultIncludedProperties = null);

    }
    
    /// <summary>
    /// Interface ISearchRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSummary">The type of the t summary.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity, TSummary, TCriteria}" />
    public interface ISearchRepository<TEntity, TLookup, TSummary> : ISearchRepository<TEntity, TLookup, TSummary, SearchCriteria>
       where TEntity : class
       where TLookup : Domain.Lookup,new()
       where TSummary : class
    { }

    /// <summary>
    /// Interface ISearchRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity, TEntity}" />
    public interface ISearchRepository<TEntity, TLookup> : ISearchRepository<TEntity, TLookup, TEntity>
      where TEntity : class
      where TLookup : Domain.Lookup, new()
    { }

    /// <summary>
    /// Interface ISearchRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity, TEntity}" />
    public interface ISearchRepository<TEntity> : ISearchRepository<TEntity, Domain.Lookup>
      where TEntity : class      
    { }
}
