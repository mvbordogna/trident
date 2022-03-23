using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trident.Contracts.Api.Client;
using Trident.Contracts.Enums;
using Trident.Extensions;
using ApiSearch = Trident.Api.Search;

namespace Trident.UI.Client
{
    public abstract class ReadOnlyServiceBase<TThis, TModel, TId> : HttpNamedServiceBase<TThis>, IReadOnlyServiceBase<TModel, TId>
       where TModel : class, IGuidModelBase
       where TThis : IServiceProxy
    {
        protected virtual string SearchRoute => $"{ResourceName}/search";
        protected virtual string GetByIdRoute => $"{ResourceName}/{{id}}";

        protected virtual string SearchMethod => HttpMethod.Post.Method;
        protected virtual string GetByIdMethod => HttpMethod.Get.Method;
        protected virtual string ResourceName { get; } = typeof(TModel).Name.ToLower();


        protected ReadOnlyServiceBase(
            ILogger<TThis> logger,
            IHttpClientFactory httpClientFactory)
            : base(logger, httpClientFactory)
        {
            var serviceAttr = this.GetType().GetCustomAttribute<ServiceAttribute>();
            serviceAttr.GuardIsNotNull("ServiceAttribute");
            this.ResourceName = serviceAttr.ResourceName ?? this.ResourceName;
        }
        public async Task<Response<ApiSearch.SearchResultsModel<TLookup, ApiSearch.SearchCriteriaModel>>> SearchLookupsResponse<TLookup>(ApiSearch.SearchCriteriaModel criteria)
        {
            var response = await SendRequest<ApiSearch.SearchResultsModel<TLookup, ApiSearch.SearchCriteriaModel>>(SearchMethod, SearchRoute, criteria);
            return response;
        }
      
        public async Task<Response<ApiSearch.SearchResultsModel<TModel, ApiSearch.SearchCriteriaModel>>> SearchResponse(ApiSearch.SearchCriteriaModel criteria)
        {
            var response = await SendRequest<ApiSearch.SearchResultsModel<TModel, ApiSearch.SearchCriteriaModel>>(SearchMethod, SearchRoute, criteria);
            return response;
        }


        public async Task<Response<ApiSearch.SearchResultsModel<TModel, ApiSearch.SearchCriteriaModel>>> GetAllResponse()
        {
            var criteria = new ApiSearch.SearchCriteriaModel()
            {
                CurrentPage = 0,
                PageSize = int.MaxValue,
                Keywords = "",
                OrderBy = "",
                SortOrder = SortOrder.Asc
            };

            var response = await SearchResponse(criteria);
            return response;
        }

        public async Task<Response<TModel>> GetByIdResponse(TId id)
        {
            var response = await SendRequest<TModel>(GetByIdMethod, GetByIdRoute.Replace("{id}", id.ToString()));
            return response;
        }

        public async Task<TModel> GetById(TId id)
        {
            var response = await GetByIdResponse(id);
            return response?.Model ?? default;
        }

        public async Task<ApiSearch.SearchResultsModel<TLookup, ApiSearch.SearchCriteriaModel>> SearchLookups<TLookup>(ApiSearch.SearchCriteriaModel criteria)
        {
            var response = await SendRequest<ApiSearch.SearchResultsModel<TLookup, ApiSearch.SearchCriteriaModel>>(SearchMethod, SearchRoute, criteria);
            return response?.Model ?? default;
        }

        public async Task<ApiSearch.SearchResultsModel<TModel, ApiSearch.SearchCriteriaModel>> Search(ApiSearch.SearchCriteriaModel criteria)
        {
            var response = await SendRequest<ApiSearch.SearchResultsModel<TModel, ApiSearch.SearchCriteriaModel>>(SearchMethod, SearchRoute, criteria);
            return response?.Model ?? default;
        }


        public async Task<IEnumerable<TModel>> GetAll()
        {
            var response = await GetAllResponse();
            return response?.Model?.Results ?? default;
        }

    }
}
