using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using Trident.Azure;

namespace Trident.Testing.TestScopes
{
    public class HttpTestScope<T> : ITestScope<T> where T : class
    {

        public T InstanceUnderTest { get; set; }
        public Mock<ILogger> LoggerMock { get; } = new Mock<ILogger>();
        public Mock<FunctionContext> FunctionContextMock { get; } = new Mock<FunctionContext>();

        public HttpRequestData CreateHttpRequest(Dictionary<string, StringValues> query, string body)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<ILoggerFactory, LoggerFactory>();
            serviceCollection.AddScoped<IOptions<WorkerOptions>>((p) =>
            {
                return Microsoft.Extensions.Options.Options.Create<WorkerOptions>(new WorkerOptions()
                {
                    Serializer = new JsonNetObjectSerializer()
                });
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            FunctionContextMock.SetupProperty(c => c.InstanceServices, serviceProvider);

            var reqMock = new Mock<HttpRequestData>(FunctionContextMock.Object);
            var headersMock = new HttpHeadersCollection();
            if (query != null && query.Any())
            {
                var queryString = query.Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}").ToArray();
                var completeQueryString = "?" + string.Join("&", queryString);
                reqMock.Setup(req => req.Url.Query).Returns(completeQueryString);
            }

            reqMock.Setup(req => req.Headers).Returns(headersMock);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(body);
            writer.Flush();
            stream.Position = 0;
            reqMock.Setup(req => req.Body).Returns(stream);

            reqMock.Setup(r => r.CreateResponse()).Returns(() =>
            {
                var response = new Mock<HttpResponseData>(FunctionContextMock.Object);
                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                response.SetupProperty(r => r.StatusCode, HttpStatusCode.Accepted);
                response.SetupProperty(r => r.Body, new MemoryStream());
                return response.Object;
            });


            return reqMock.Object;
        }
    }

    public static class HttpTestHelperExtensions
    {
        public static async Task<T> ReadJsonToObject<T>(this HttpResponseData response)
        {
            response.Body.Position = 0;
            var bodyStr = await new StreamReader(response.Body).ReadToEndAsync();

            if (!(typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>)))
                bodyStr = bodyStr.TrimStart('[').TrimEnd(']');

            return JsonConvert.DeserializeObject<T>(bodyStr);
        }
    }

}
