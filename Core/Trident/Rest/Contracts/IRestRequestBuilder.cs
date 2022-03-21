using Trident.Search;
using System.Collections.Generic;

namespace Trident.Rest.Contracts
{
    /// <summary>
    /// Interface IRestRequestBuilder
    /// Implements the <see cref="TridentOptionsBuilder.Rest.Contracts.IRestRequestBuilder" />
    /// </summary>
    /// <typeparam name="TTargetEntity">The type of the target entity (i.e. the REST service's entity).</typeparam>
    /// <typeparam name="TTargetEntityId">The type of the t target entity identifier.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Rest.Contracts.IRestRequestBuilder" />
    public interface IRestRequestBuilder<TTargetEntity, in TTargetEntityId> : IRestRequestBuilder
    {
        /// <summary>
        /// Builds the get by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>RestRequest.</returns>
        RestRequest BuildGetById(TTargetEntityId id);

        /// <summary>
        /// Builds the get by ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>RestRequest.</returns>
        RestRequest BuildGetByIds(IEnumerable<TTargetEntityId> ids);

        /// <summary>
        /// Builds the get all.
        /// </summary>
        /// <param name="includedProperties">The included properties.</param>
        /// <returns>RestRequest.</returns>
        RestRequest BuildGetAll(IEnumerable<string> includedProperties = null);//TODO: avoid due to expressions and redundancy wtih search

        /// <summary>
        /// Builds the delete.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>RestRequest.</returns>
        RestRequest BuildDelete(TTargetEntityId id);

        /// <summary>
        /// Builds the exists.
        /// </summary>
        /// <returns>RestRequest.</returns>
        RestRequest BuildExists();//TODO: avoid due to expressions

        /// <summary>
        /// Builds the insert.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>RestRequest.</returns>
        RestRequest BuildInsert(TTargetEntity entity);

        /// <summary>
        /// Builds the search.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="includedProperties">The included properties.</param>
        /// <returns>RestRequest.</returns>
        RestRequest BuildSearch(SearchCriteria searchCriteria, IEnumerable<string> includedProperties = null);

        /// <summary>
        /// Builds the update.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>RestRequest.</returns>
        RestRequest BuildUpdate(TTargetEntity entity, TTargetEntityId id);
    }

    /// <summary>
    /// Interface IRestRequestBuilder
    /// Implements the <see cref="TridentOptionsBuilder.Rest.Contracts.IRestRequestBuilder" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Rest.Contracts.IRestRequestBuilder" />
    public interface IRestRequestBuilder
    {

    }
}