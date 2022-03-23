using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Trident.Contracts.Api.Client;
using Trident.UI.Blazor.Contracts.Services;


namespace Trident.UI.Client
{

    public class HttpGenericService : HttpServiceBase<HttpGenericService>, IHttpGenericService
    {
        public HttpGenericService(
           ILogger<HttpGenericService> logger,
           IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory) { }

        public new async Task<Response<T>> SendRequest<T>(string service, string method, string route, object data = null) where T : class
        {
            return await base.SendRequest<T>(service, method, route, data);
        }
    }
}
