using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Trident.Contracts.Api.Client;
using Trident.Contracts.Api.Validation;
using Trident.Extensions;

namespace Trident.UI.Client
{
    public abstract class HttpServiceBase<TThis> : IHttpServiceBase
        where TThis : IServiceProxy
    {
        protected static ConcurrentDictionary<string, HttpClient> HttpNamedClients { get; } = new ConcurrentDictionary<string, HttpClient>();
        protected ILogger<TThis> Logger { get; private set; }
        protected IHttpClientFactory HttpClientFactory { get; }
        protected HttpClient NoAuthClient { get; }

        public HttpServiceBase(
             ILogger<TThis> logger,
             IHttpClientFactory httpClientFactory)
        {
            logger.GuardIsNotNull(nameof(logger));
            httpClientFactory.GuardIsNotNull(nameof(httpClientFactory));

            Logger = logger;
            HttpClientFactory = httpClientFactory;
            NoAuthClient = httpClientFactory.CreateClient();
        }

        protected HttpClient GetClientOrDefault(string serviceName = null)
        {
            if (!string.IsNullOrWhiteSpace(serviceName))
            {
                return HttpNamedClients.GetOrAdd(serviceName, (name) => this.HttpClientFactory.CreateClient(name));
            }
            else return NoAuthClient;
        }

        protected void logInfo(string message, Guid requestId, string opereration, string stage, string responseType, string service, string url, string data, HttpStatusCode statusCode = HttpStatusCode.Unused)
        {
            Logger.LogInformation($"Begin Client Send Request", new
            {
                RequestId = requestId,
                Opereration = opereration,
                Stage = stage,
                ResponseType = responseType,
                SendRequest = service,
                Url = url,
                Data = data,
                HttpStatusCode = (statusCode == HttpStatusCode.Unused)
                    ? null
                    : (HttpStatusCode?)statusCode
            });
        }
        protected void logWarn(string message, Guid requestId, string opereration, string stage, string responseType, string service, string url, string data, HttpStatusCode statusCode = HttpStatusCode.Unused)
        {
            Logger.LogWarning($"Begin Client Send Request", new
            {
                RequestId = requestId,
                Opereration = opereration,
                Stage = stage,
                ResponseType = responseType,
                SendRequest = service,
                Url = url,
                Data = data,
                HttpStatusCode = (statusCode == HttpStatusCode.Unused)
                    ? null
                    : (HttpStatusCode?)statusCode
            });
        }



        protected async Task<Response<M>> SendRequest<M>(string service, string method, string route, object data = null)
          where M : class
        {
            return await SendRequest<Response<M>, M, ErrorCodes>(service, method, route, data);
        }

        protected async Task<T> SendRequest<T, M, TErrorCodes>(string service, string method, string route, object data = null)
            where T : Response<M, TErrorCodes>, new()
            where M : class
        where TErrorCodes : struct, Enum
        {
            var output = new T();
            var requestId = Guid.NewGuid();
            string url = string.Empty;
            method = method.ToUpper();

            try
            {
                var dataJson = JsonConvert.SerializeObject(data);
                var client = GetClientOrDefault(service);
                url = $"{client.BaseAddress}/{route}".Replace("//", "/");
                logInfo("Begin Client Send Request", requestId, nameof(SendRequest), "Begin Request",
                    typeof(M).FullName, service, url, dataJson);

                var httpMethod = new HttpMethod(method);
                var request = (httpMethod != HttpMethod.Get)
                    ? new HttpRequestMessage(httpMethod, route)
                    {
                        Content = new StringContent(dataJson)
                    }
                    : new HttpRequestMessage(httpMethod, route);

                request.Headers.Add("Request-Id", requestId.ToString());

                var response = await client.SendAsync(request);

                output.StatusCode = response.StatusCode;
                output.ResponseContent = await response.Content.ReadAsStringAsync();

                logInfo("Client Response Recieved", requestId, nameof(SendRequest), "Response Recieved",
                   typeof(M).FullName, service, url, output.ResponseContent, output.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    output.Model = (typeof(M).IsAssignableTo(typeof(string)))
                        ? (M)(object)output.ResponseContent
                        : JsonConvert.DeserializeObject<M>(output.ResponseContent);

                    logInfo("Model Deserialized Successfully", requestId, nameof(SendRequest),
                        "Deserialized Message", typeof(M).FullName, service, url, output.ResponseContent, output.StatusCode);


                    return output;
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    output = MapValidationResults<T, M, TErrorCodes>(output, response);

                    logInfo("BadRequest Validation Model Deserialized Successfully", requestId, nameof(SendRequest),
                        "Deserialized Message", typeof(M).FullName, service, url, output.ResponseContent, output.StatusCode);
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    output = MapValidationResults<T, M, TErrorCodes>(output, response);

                    logWarn("Resource Not Found", requestId, nameof(SendRequest),
                        "Deserialized Message", typeof(M).FullName, service, url, null, output.StatusCode);
                }

                return output;
            }
            catch (Exception ex)
            {
                output.Exception = ex;

                Logger.LogError(ex, $"Exception occured while processing your request.", new
                {
                    RequestId = requestId,
                    Opereration = nameof(SendRequest),
                    ResponseType = typeof(M).FullName,
                    SendRequest = service,
                    Method = method,
                    Url = url,
                    Exception = ex
                });
            }

            return output;
        }

        private T MapValidationResults<T, M, TErrorCodes>(T output, HttpResponseMessage response)
            where T : Response<M, TErrorCodes>
            where M : class
            where TErrorCodes : struct, Enum
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (output.ResponseContent.Contains(nameof(Trident.Contracts.Api.Validation.ValidationResult<TErrorCodes>.ErrorCode), StringComparison.InvariantCultureIgnoreCase))
                {
                    var validationResults = output.ValidationErrors = JsonConvert.DeserializeObject<IEnumerable<Trident.Contracts.Api.Validation.ValidationResult<TErrorCodes, M>>>(output.ResponseContent);

                    if (validationResults != null && validationResults.Count(r => !string.IsNullOrEmpty(r.Message) && r.ErrorCode != null) > 0)
                    {
                        output.ValidationSummary = ParseValidationResults(validationResults);
                    }
                }
            }

            return output;
        }

        private static string ParseValidationResults(IEnumerable<ValidationResult> validationResults)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");

            foreach (ValidationResult validationResult in validationResults)
                sb.Append("<li>" + validationResult.Message + "</li>");

            sb.Append("</ul>");

            return sb.ToString();
        }
    }
}
