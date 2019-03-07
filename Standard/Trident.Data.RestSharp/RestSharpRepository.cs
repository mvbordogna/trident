using Trident.Contracts;
using Trident.Data.Contracts;
using Trident.Mapper;
using Trident.Rest.Contracts;
using Trident.Search;

namespace Trident.Data.RestSharp
{
    public abstract class RestSharpRepository<TEntity, TTargetEntity, TEntityId, TTargetEntityId>
        : Rest.RestRepositoryBase<TEntity, TTargetEntity, TEntityId, TTargetEntityId>
          where TEntity : class, IHaveId<TEntityId>
          where TTargetEntity : class, IHaveId<TTargetEntityId>, new()
    {
        protected RestSharpRepository(ISearchResultsBuilder resultsBuilder,
            IAbstractContextFactory contextFactory,
            IMapperRegistry mapperRegistry,
            IRestRequestBuilder<TTargetEntity, TTargetEntityId> restRequestBuilder)
            : base(resultsBuilder, contextFactory, mapperRegistry, restRequestBuilder)
        {
        }
    }
}
