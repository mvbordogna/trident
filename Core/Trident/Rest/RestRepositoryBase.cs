using Trident.Contracts;
using Trident.Data.Contracts;
using Trident.Rest.Contracts;
using Trident.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Trident.Domain;
using Trident.Mapper;

namespace Trident.Rest
{
    /// <summary>
    /// Class RestRepositoryBase.
    /// Implements the <see cref="TridentOptionsBuilder.Data.Contracts.IRepositoryBase{TEntity}" />
    /// Implements the <see cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TTargetEntity">The type of the t target entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the t entity identifier.</typeparam>
    /// <typeparam name="TTargetEntityId">The type of the t target entity identifier.</typeparam>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.IRepositoryBase{TEntity}" />
    /// <seealso cref="TridentOptionsBuilder.Search.ISearchRepository{TEntity}" />
    public abstract class RestRepositoryBase<TEntity,TTargetEntity, TEntityId, TTargetEntityId> : IRepositoryBase<TEntity>, ISearchRepository<TEntity>
        where TEntity : class, IHaveId<TEntityId>
        where TTargetEntity : class, IHaveId<TTargetEntityId>, new()
    {
        /// <summary>
        /// The results builder
        /// </summary>
        private readonly ISearchResultsBuilder _resultsBuilder;

        /// <summary>
        /// Gets the rest request builder.
        /// </summary>
        /// <value>The rest request builder.</value>
        protected IRestRequestBuilder<TTargetEntity, TTargetEntityId> RestRequestBuilder { get; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected IRestContext Context { get; }

        /// <summary>
        /// Gets the mapper registry.
        /// </summary>
        /// <value>The mapper registry.</value>
        protected IMapperRegistry MapperRegistry { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestRepositoryBase{TEntity, TTargetEntity, TEntityId, TTargetEntityId}"/> class.
        /// </summary>
        /// <param name="resultsBuilder">The results builder.</param>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="mapperRegistry">The mapper registry.</param>
        /// <param name="restRequestBuilder">The rest request builder.</param>
        protected RestRepositoryBase(ISearchResultsBuilder resultsBuilder,
            IAbstractContextFactory contextFactory,
            IMapperRegistry mapperRegistry,
            IRestRequestBuilder<TTargetEntity, TTargetEntityId> restRequestBuilder)
        {
            Context = contextFactory.Create<IRestContext>(typeof(TTargetEntity));
            _resultsBuilder = resultsBuilder;
            MapperRegistry = mapperRegistry;
            RestRequestBuilder = restRequestBuilder;
        }

        /// <summary>
        /// Gets the map by local identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;IExternalReference&lt;TEntityId, TTargetEntityId&gt;&gt;.</returns>
        public abstract Task<IExternalReference<TEntityId, TTargetEntityId>> GetMapByLocalId(TEntityId id);


        /// <summary>
        /// Gets the map by local identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;IExternalReference&lt;TEntityId, TTargetEntityId&gt;&gt;.</returns>
        public abstract IExternalReference<TEntityId, TTargetEntityId> GetMapByLocalIdSync(TEntityId id);

        /// <summary>
        /// Deletes the map by local identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task.</returns>
        public abstract Task DeleteMapByLocalId(TEntityId id);


        /// <summary>
        /// Deletes the map by local identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task.</returns>
        public abstract Task DeleteMapByLocalIdSync(TEntityId id);

        /// <summary>
        /// Gets the map by local ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>Task&lt;IEnumerable&lt;IExternalReference&lt;TEntityId, TTargetEntityId&gt;&gt;&gt;.</returns>
        public abstract Task<IEnumerable<IExternalReference<TEntityId, TTargetEntityId>>> GetMapByLocalIds(IEnumerable<TEntityId> ids);

        /// <summary>
        /// Gets the map by remote ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>Task&lt;IEnumerable&lt;IExternalReference&lt;TEntityId, TTargetEntityId&gt;&gt;&gt;.</returns>
        public abstract Task<IEnumerable<IExternalReference<TEntityId, TTargetEntityId>>> GetMapByRemoteIds(IEnumerable<TTargetEntityId> ids);


        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        public virtual async Task Delete(TEntity entity, bool deferCommit = false)
        {
            var idMap = await GetMapByLocalId(entity.GetId());
            if (idMap == null)
            {
                // nothign to delete
                //TODO: or do we want to just continue (skipping the idMap) and try REST delete?
                return;
            }

            var targetEntity = MapperRegistry.Map<TTargetEntity>(entity);
            MapperRegistry.Map(idMap, targetEntity);

            var response = await ExecuteDelete(targetEntity.GetId());          
        }


        public void DeleteSync(TEntity entity, bool deferCommit = false)
        {
            var idMap = GetMapByLocalIdSync(entity.GetId());
            if (idMap == null)
            {
                // nothign to delete
                //TODO: or do we want to just continue (skipping the idMap) and try REST delete?
                return;
            }

            var targetEntity = MapperRegistry.Map<TTargetEntity>(entity);
            MapperRegistry.Map(idMap, targetEntity);

            var response = ExecuteDeleteSync(targetEntity.GetId()); 
        }

        /// <summary>
        /// Executes the delete.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;RestResponse&lt;dynamic&gt;&gt;.</returns>
        /// <exception cref="System.ApplicationException">Delete was not successfull</exception>
        protected async Task<RestResponse<dynamic>> ExecuteDelete(TTargetEntityId id)
        {
            var request = RestRequestBuilder.BuildDelete(id);

            //TODO: deputy returns string, but this code should be Deputy-agnostic
            RestResponse<dynamic> response = null;
            try
            {
                response = await Context.ExecuteMessage<dynamic>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("Delete was not successfull");
                }
            }
            catch (RestException<dynamic> restEx)
            {
                // absorb not found errors...that is what delete is trying to accomplish
                // re-throw all other errors
                if (restEx.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    return restEx.Response;
                }
                throw;
            }

            return response;
        }


        /// <summary>
        /// Executes the delete.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;RestResponse&lt;dynamic&gt;&gt;.</returns>
        /// <exception cref="System.ApplicationException">Delete was not successfull</exception>
        protected RestResponse<dynamic> ExecuteDeleteSync(TTargetEntityId id)
        {
            var request = RestRequestBuilder.BuildDelete(id);

            //TODO: deputy returns string, but this code should be Deputy-agnostic
            RestResponse<dynamic> response = null;
            try
            {
                response = Context.ExecuteMessageSync<dynamic>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("Delete was not successfull");
                }
            }
            catch (RestException<dynamic> restEx)
            {
                // absorb not found errors...that is what delete is trying to accomplish
                // re-throw all other errors
                if (restEx.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    return restEx.Response;
                }
                throw;
            }

            return response;
        }

        /// <summary>
        /// Existses the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> Exists(Expression<Func<TEntity, bool>> filter)
        {
            var request = RestRequestBuilder.BuildExists();

            var response = await Context.ExecuteMessage<List<TTargetEntity>>(request);

            //TODO: use MapTargetToLocal methods
            var results = MapperRegistry.Map<List<TEntity>>(response.Data);
            var query = results.AsQueryable();

            return query.Any(filter);
        }

        /// <summary>
        /// Gets the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="noTracking">if set to <c>true</c> [no tracking].</param>
        /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
        public virtual async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            IEnumerable<string> includeProperties = null, bool noTracking = false)
        {
            var request = RestRequestBuilder.BuildGetAll(includeProperties);

            var response = await Context.ExecuteMessage<List<TTargetEntity>>(request);

            var results = await MapTargetToLocal(response.Data);


            //
            // apply in memory filters
            //
            var query = results.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query.ToList();
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        /// <exception cref="System.NotImplementedException">Only {typeof(TEntityId).Name}</exception>
        public virtual async Task<TEntity> GetById(object id, bool detach = false)
        {
            if (id?.GetType() != typeof(TEntityId))
            {
                throw new NotImplementedException($"Only {typeof(TEntityId).Name} IDs are supported at this time.");
            }

            var idMap = await GetMapByLocalId((TEntityId)id);
            if (idMap == null)
            {
                return null;
            }

            try
            {
                var response = await ExecuteGetById(idMap);

                var entity = await MapTargetToLocal(response.Data, idMap);

                return entity;
            }
            catch (RestException<TTargetEntity> ex)
            {
                // turn 404 errors into nulls
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    // we got out of sync with deputy
                    // clean up the errant ExternalReference on our end
                    await DeleteMapByLocalId(idMap.Id);

                    return null;
                }
                throw;
            }
        }

        /// <summary>
        /// Executes the get by identifier.
        /// </summary>
        /// <param name="idMap">The identifier map.</param>
        /// <returns>Task&lt;RestResponse&lt;TTargetEntity&gt;&gt;.</returns>
        protected virtual async Task<RestResponse<TTargetEntity>> ExecuteGetById(IExternalReference<TEntityId, TTargetEntityId> idMap)
        {
            var request = RestRequestBuilder.BuildGetById(idMap.ExternalId);
            return await Context.ExecuteMessage<TTargetEntity>(request);
        }

        /// <summary>
        /// Executes the get by identifier.
        /// </summary>
        /// <param name="idMap">The identifier map.</param>
        /// <returns>Task&lt;RestResponse&lt;TTargetEntity&gt;&gt;.</returns>
        protected virtual RestResponse<TTargetEntity> ExecuteGetByIdSync(IExternalReference<TEntityId, TTargetEntityId> idMap)
        {
            var request = RestRequestBuilder.BuildGetById(idMap.ExternalId);
            return Context.ExecuteMessageSync<TTargetEntity>(request);
        }

        /// <summary>
        /// Gets the by ids.
        /// </summary>
        /// <typeparam name="TId">The type of the t identifier.</typeparam>
        /// <param name="ids">The ids.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
        /// <exception cref="System.NotImplementedException">Only {typeof(TEntityId).Name}</exception>
        public virtual async Task<IEnumerable<TEntity>> GetByIds<TId>(IEnumerable<TId> ids, bool detach = false)
        {
            if (typeof(TId) != typeof(TEntityId))
            {
                throw new NotImplementedException($"Only {typeof(TEntityId).Name} IDs are supported at this time.");
            }

            var maps = await GetMapByLocalIds(ids.Cast<TEntityId>());
            var externalIdLookup = maps.ToDictionary(x => x.ExternalId);

            var request = RestRequestBuilder.BuildGetByIds(externalIdLookup.Keys);

            var response = await Context.ExecuteMessage<List<TTargetEntity>>(request);
            return await MapTargetToLocal(response.Data, externalIdLookup);
        }
        
        public TEntity GetByIdSync(object id, bool detach = false)
        {
            if (id?.GetType() != typeof(TEntityId))
            {
                throw new NotImplementedException($"Only {typeof(TEntityId).Name} IDs are supported at this time.");
            }

            var idMap = GetMapByLocalIdSync((TEntityId)id);
            if (idMap == null)
            {
                return null;
            }

            try
            {
                var response = ExecuteGetByIdSync(idMap);

                var entity = MapTargetToLocalSync(response.Data, idMap);

                return entity;
            }
            catch (RestException<TTargetEntity> ex)
            {
                // turn 404 errors into nulls
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                {  
                    DeleteMapByLocalIdSync(idMap.Id);
                    return null;
                }
                throw;
            }
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        public virtual async Task Insert(TEntity entity, bool deferCommit = false)
        {
            var targetentity = MapperRegistry.Map<TTargetEntity>(entity);
            var response = await ExecuteInsert(entity, targetentity);
            MapperRegistry.Map(response.Data, entity);
        }

        public void InsertSync(TEntity entity, bool deferCommit = false)
        {
            var targetentity = MapperRegistry.Map<TTargetEntity>(entity);
            var response = ExecuteInsertSync(entity, targetentity);
            MapperRegistry.Map(response.Data, entity);
        }


        /// <summary>
        /// Executes the insert.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="target">The target.</param>
        /// <returns>Task&lt;RestResponse&lt;TTargetEntity&gt;&gt;.</returns>
        protected virtual async Task<RestResponse<TTargetEntity>> ExecuteInsert(TEntity entity, TTargetEntity target)
        {
            var request = RestRequestBuilder.BuildInsert(target);

            return await Context.ExecuteMessage<TTargetEntity>(request);
        }

        /// <summary>
        /// Executes the insert.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="target">The target.</param>
        /// <returns>Task&lt;RestResponse&lt;TTargetEntity&gt;&gt;.</returns>
        protected virtual RestResponse<TTargetEntity> ExecuteInsertSync(TEntity entity, TTargetEntity target)
        {
            var request = RestRequestBuilder.BuildInsert(target);

            return Context.ExecuteMessageSync<TTargetEntity>(request);
        }


        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deferCommit">if set to <c>true</c> [defer commit].</param>
        /// <returns>Task.</returns>
        public virtual async Task Update(TEntity entity, bool deferCommit = false)
        {
            var idMap = await GetMapByLocalId(entity.GetId());
            var targetentity = MapperRegistry.Map<TTargetEntity>(entity);
            MapperRegistry.Map(idMap, targetentity);

            var response = await ExecuteUpdate(entity, targetentity);

            MapperRegistry.Map(response.Data, entity);//TODO: or start clean with a new object?
            MapperRegistry.Map(idMap, entity);
        }
               
        public void UpdateSync(TEntity entity, bool deferCommit = false)
        {
            var idMap = GetMapByLocalIdSync(entity.GetId());
            var targetentity = MapperRegistry.Map<TTargetEntity>(entity);
            MapperRegistry.Map(idMap, targetentity);

            var response = ExecuteUpdateSync(entity, targetentity);

            MapperRegistry.Map(response.Data, entity);//TODO: or start clean with a new object?
            MapperRegistry.Map(idMap, entity);
        }



        /// <summary>
        /// Executes the update.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="target">The target.</param>
        /// <returns>Task&lt;RestResponse&lt;TTargetEntity&gt;&gt;.</returns>
        protected virtual async Task<RestResponse<TTargetEntity>> ExecuteUpdate(TEntity entity, TTargetEntity target)
        {
            var request = RestRequestBuilder.BuildUpdate(target, target.GetId());
            return await Context.ExecuteMessage<TTargetEntity>(request);
        }

        /// <summary>
        /// Executes the update.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="target">The target.</param>
        /// <returns>Task&lt;RestResponse&lt;TTargetEntity&gt;&gt;.</returns>
        protected virtual RestResponse<TTargetEntity> ExecuteUpdateSync(TEntity entity, TTargetEntity target)
        {
            var request = RestRequestBuilder.BuildUpdate(target, target.GetId());
            return Context.ExecuteMessageSync<TTargetEntity>(request);
        }

        /// <summary>
        /// Searches the specified search criteria.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="includedProperties">The included properties.</param>
        /// <returns>Task&lt;SearchResults&lt;TEntity, SearchCriteria&gt;&gt;.</returns>
        public virtual async Task<SearchResults<TEntity, SearchCriteria>> Search(SearchCriteria searchCriteria, IEnumerable<string> includedProperties = null)
        {
            var response = await ExecuteSearch(searchCriteria, includedProperties);

            // map to local model & saturate with local ids
            var results = await MapTargetToLocal(response.Data);

            //TODO: search does not return metadata....what should we do here?
            // if we got back less than we asked for, then there are no more results, otherwise, pretend there is one more
            var totalRecords = results.Count < searchCriteria.PageSize
                ? results.Count
                : results.Count + 1;

            return SearchResultContent(results, searchCriteria, totalRecords);
        }
    
        public SearchResults<TEntity, SearchCriteria> SearchSync(SearchCriteria searchCriteria, IEnumerable<string> includedProperties = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the source.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="includedProperties">The included properties.</param>
        /// <returns>Task&lt;SearchResults&lt;TTargetEntity, SearchCriteria&gt;&gt;.</returns>
        public virtual async Task<SearchResults<TTargetEntity, SearchCriteria>> SearchSource(SearchCriteria searchCriteria, IEnumerable<string> includedProperties = null)
        {
            searchCriteria.ContextBag["HA"] = false;
             var response = await ExecuteSearch(searchCriteria, includedProperties);
            var results = response?.Data ?? new List<TTargetEntity>();
            
            //TODO: search does not return metadata....what should we do here?
            // if we got back less than we asked for, then there are no more results, otherwise, pretend there is one more
            var totalRecords = results.Count < searchCriteria.PageSize
                ? results.Count
                : results.Count + 1;

            return _resultsBuilder.Build(results, searchCriteria, totalRecords);    
        }

        /// <summary>
        /// Executes the search.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="includedProperties">The included properties.</param>
        /// <returns>Task&lt;RestResponse&lt;List&lt;TTargetEntity&gt;&gt;&gt;.</returns>
        protected virtual async Task<RestResponse<List<TTargetEntity>>> ExecuteSearch(SearchCriteria searchCriteria, IEnumerable<string> includedProperties = null)
        {
            //TODO: currently we expect TTargetEntity property names to be in SearchCriteria.  Consider if we should expect/map from TEntity?!?
            var request = RestRequestBuilder.BuildSearch(searchCriteria, includedProperties);

            return await Context.ExecuteMessage<List<TTargetEntity>>(request);
        }

        /// <summary>
        /// Searches the content of the result.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns>SearchResults&lt;TEntity, SearchCriteria&gt;.</returns>
        protected virtual SearchResults<TEntity, SearchCriteria> SearchResultContent(List<TEntity> results, SearchCriteria criteria, int totalRecords)
        {
            return _resultsBuilder.Build(results, criteria, totalRecords);
        }





        /// <summary>
        /// Converts to local.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="idMap">The identifier map.</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        protected virtual Task<TEntity> MapTargetToLocal(TTargetEntity target, IExternalReference<TEntityId, TTargetEntityId> idMap)
        {
            return Task.FromResult(MapTargetToLocalSync(target, idMap));
        }



        /// <summary>
        /// Converts to local.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="idMap">The identifier map.</param>
        /// <returns>Task&lt;TEntity&gt;.</returns>
        protected virtual TEntity MapTargetToLocalSync(TTargetEntity target, IExternalReference<TEntityId, TTargetEntityId> idMap)
        {
            var entity = MapperRegistry.Map<TEntity>(target);

            // if data gets out of sync & we dont have the id, just dont map (instead of error)
            if (idMap != null)
            {
                MapperRegistry.Map(idMap, entity);
            }

            return entity;
        }

        /// <summary>
        /// Converts to local.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Task&lt;List&lt;TEntity&gt;&gt;.</returns>
        private async Task<List<TEntity>> MapTargetToLocal(List<TTargetEntity> response)
        {
            // get id map for all ids returned
            var remoteIds = response.Select(x => x.GetId());
            var maps = await GetMapByRemoteIds(remoteIds);
            var targetIdLookup = maps.ToDictionary(x => x.ExternalId);

            // map to local model & saturate with local ids
            return await MapTargetToLocal(response, targetIdLookup);
        }

        /// <summary>
        /// Converts to local.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="targetIdLookup">The target identifier lookup.</param>
        /// <returns>Task&lt;List&lt;TEntity&gt;&gt;.</returns>
        private async Task<List<TEntity>> MapTargetToLocal(List<TTargetEntity> response, Dictionary<TTargetEntityId, IExternalReference<TEntityId, TTargetEntityId>> targetIdLookup)
        {
            // map to local model & saturate with local ids
            var results = new List<TEntity>(response.Count);
            foreach (var result in response)
            {
                targetIdLookup.TryGetValue(result.GetId(), out var map);
                var entity = await MapTargetToLocal(result, map);

                results.Add(entity);
            }

            return results;
        }
             

        public IEnumerable<TEntity> GetSync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, IEnumerable<string> includeProperties = null, bool noTracking = false)
        {
            throw new NotImplementedException();
        }

        public Task<SearchResults<Lookup, SearchCriteria>> SearchLookups(SearchCriteria criteria, IEnumerable<string> defaultIncludedProperties = null)
        {
            throw new NotImplementedException();
        }

        public SearchResults<Lookup, SearchCriteria> SearchLookupsSync(SearchCriteria criteria, IEnumerable<string> defaultIncludedProperties = null)
        {
            throw new NotImplementedException();
        }
    }
}
