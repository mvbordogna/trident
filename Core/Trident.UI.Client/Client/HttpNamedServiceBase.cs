using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trident.Contracts.Api.Client;
using Trident.Extensions;

namespace Trident.UI.Client
{
    public abstract class HttpNamedServiceBase<TThis> : HttpServiceBase<TThis>, IHttpNamedServiceBase
        where TThis : IServiceProxy
    {
        protected HttpClient AuthClient { get; set; }
        public string HttpServiceName { get; protected set; }

        protected HttpNamedServiceBase(
            ILogger<TThis> logger,
            IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory)
        {
            var serviceAttr = this.GetType().GetCustomAttribute<ServiceAttribute>();
            serviceAttr.GuardIsNotNull(nameof(ServiceAttribute));

            this.HttpServiceName = serviceAttr.Name;
            this.AuthClient = this.GetClientOrDefault(this.HttpServiceName);
        }

        protected string BaseUrl
        {
            get
            {
                return this.AuthClient.BaseAddress.ToString();
            }
        }

        protected virtual async Task<Response<T>> SendRequest<T>(string method, string route, object data = null)
            where T : class
        {
            return await SendRequest<T>(this.HttpServiceName, method, route, data);
        }

        protected virtual async Task<T> SendRequest<T, M, TErrorCodes>(string method, string route, object data = null)
           where T : Response<M, TErrorCodes>, new()
           where M : class
           where TErrorCodes : struct, Enum
        {
            return await SendRequest<T, M, TErrorCodes>(this.HttpServiceName, method, route, data);
        }
    }
}
