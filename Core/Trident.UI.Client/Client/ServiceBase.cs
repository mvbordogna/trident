using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Trident.Contracts.Api.Client;
using Trident.UI.Client.Contracts.Models;

namespace Trident.UI.Client
{
    public abstract class ServiceBase<TThis, TModel, TId> : ReadOnlyServiceBase<TThis, TModel, TId>,
        Trident.Contracts.Api.Client.IServiceProxyBase<TModel, TId>
        where TModel : class, Trident.Contracts.Api.Client.IGuidModelBase
        where TThis : Trident.Contracts.Api.Client.IServiceProxy
    {
        protected virtual string UpdateRoute => $"{ResourceName}/{{id}}";
        protected virtual string CreateRoute => $"{ResourceName}";
        protected virtual string DeleteRoute => $"{ResourceName}/{{id}}";
        protected virtual string PatchRoute => $"{ResourceName}/{{id}}";

        protected virtual string UpdateMethod => HttpMethod.Put.Method;
        protected virtual string CreateMethod => HttpMethod.Post.Method;
        protected virtual string DeleteMethod => HttpMethod.Delete.Method;
        protected virtual string PatchMethod => HttpMethod.Patch.Method;

        protected ServiceBase(
            ILogger<TThis> logger,
            IHttpClientFactory httpClientFactory)
            : base(logger, httpClientFactory)
        {
        }

        public async Task<Response<TModel>> Update(TModel model)
        {
            var response = await SendRequest<TModel>(UpdateMethod, UpdateRoute.Replace("{id}", model.Id.ToString()), model);
            return response;
        }

        public virtual async Task<Response<TModel>> Delete(TModel model)
        {
            var response = await SendRequest<TModel>(DeleteMethod, DeleteRoute.Replace("{id}", model.Id.ToString()), model);
            return response;
        }

        public async Task<Response<TModel>> Create(TModel model)
        {
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }
            var response = await SendRequest<TModel>(CreateMethod, CreateRoute, model);
            return response;
        }

        public async Task<Response<TModel>> Patch(Guid id, TModel model)
        {
            
            var response = await SendRequest<TModel>(PatchMethod, PatchRoute.Replace("{id}", id.ToString()), model);
            return response;
        }

        public async Task<Response<TModel>> Patch(TId id, IDictionary<string, object> patches)
        {
            
            var response = await SendRequest<TModel>(PatchMethod, PatchRoute.Replace("{id}", id.ToString()), patches);
            return response;
        }
    }
}
