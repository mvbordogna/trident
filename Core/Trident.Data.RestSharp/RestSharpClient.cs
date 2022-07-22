using System;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Trident.Mapper;
using Trident.Rest;
using Trident.Rest.Contracts;
using IRestClient = Trident.Rest.Contracts.IRestClient;
using RestRequest = Trident.Rest.RestRequest;
using RestResponse = Trident.Rest.RestResponse;
using RestSharpRestClient = RestSharp.RestClient;
using RestSharpRestRequest = RestSharp.RestRequest;

namespace Trident.Data.RestSharp
{
    /// <summary>
    /// Class RestSharpClient.
    /// Implements the <see cref="TridentOptionsBuilder.Rest.Contracts.IRestClient" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Rest.Contracts.IRestClient" />
    /// <summary>
    /// Class RestSharpClient.
    /// Implements the <see cref="TridentOptionsBuilder.Rest.Contracts.IRestClient" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Rest.Contracts.IRestClient" />
    public class RestSharpClient : IRestClient
    {
        /// <summary>
        /// The mapper
        /// </summary>
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapperRegistry _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestSharpClient"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <summary>
        /// Initializes a new instance of the <see cref="RestSharpClient"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        public RestSharpClient(IMapperRegistry mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestResponse&gt;.</returns>
        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestResponse&gt;.</returns>
        public async Task<RestResponse> Execute(RestRequest request, IRestConnection restConnection)
        {
            var client = await BuildRestClient(restConnection);
            var restRequest = _mapper.Map<RestSharpRestRequest>(request);
            var restResponse = await client.ExecuteAsync(restRequest);
            var response = _mapper.Map<Rest.RestResponse>(restResponse);
            NormalizeErrorsToExceptions(restResponse, response);
            return response;
        }

        public RestResponse ExecuteSync(RestRequest request, IRestConnection restConnection)
        {
            var client = BuildRestClientSync(restConnection);
            var restRequest = _mapper.Map<RestSharpRestRequest>(request);
            var restResponse = client.Execute(restRequest);
            var response = _mapper.Map<Rest.RestResponse>(restResponse);
            NormalizeErrorsToExceptions(restResponse, response);
            return response;
        }


        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestResponse&lt;T&gt;&gt;.</returns>
        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestResponse&lt;T&gt;&gt;.</returns>
        public async Task<Rest.RestResponse<T>> Execute<T>(RestRequest request, IRestConnection restConnection)
        {
            var client = await BuildRestClient(restConnection);
            var restRequest = _mapper.Map<RestSharpRestRequest>(request);


            //
            // make the REST request and wait for the response
            //
            var restResponse = await client.ExecuteAsync<T>(restRequest);


            var response = _mapper.Map<Rest.RestResponse<T>>(restResponse);
            NormalizeErrorsToExceptions(restResponse, response);
            return response;
        }

        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestResponse&lt;T&gt;&gt;.</returns>
        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestResponse&lt;T&gt;&gt;.</returns>
        public Rest.RestResponse<T> ExecuteSync<T>(RestRequest request, IRestConnection restConnection)
            where T:class, new()
        {
            var client = BuildRestClientSync(restConnection);
            var restRequest = _mapper.Map<RestSharpRestRequest>(request);


            //
            // make the REST request and wait for the response
            //
            var restResponse = client.Execute<T>(restRequest, (Method)Enum.Parse(typeof(Method), request.Method.ToString()));


            var response = _mapper.Map<Rest.RestResponse<T>>(restResponse);
            NormalizeErrorsToExceptions(restResponse, response);
            return response;
        }


        /// <summary>
        /// Builds the rest client.
        /// </summary>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestSharpRestClient&gt;.</returns>
        /// <summary>
        /// Builds the rest client.
        /// </summary>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestSharpRestClient&gt;.</returns>
        private static async Task<RestSharpRestClient> BuildRestClient(IRestConnection restConnection)
        {
            var client = new RestSharpRestClient
            {
                BaseUrl = restConnection.BaseUrl,
                Timeout = restConnection.Timeout,
                Authenticator = await restConnection.Authenticate<IAuthenticator>()
            };

            // use Newtonsoft's for a) proper handling of DateTimeOffset types, b) performance and c) extensability
            client.AddHandler("application/json", ()=> NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/json", () => NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/x-json", () => NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/javascript", () => NewtonsoftJsonSerializer.Default);
            client.AddHandler("*+json", () => NewtonsoftJsonSerializer.Default);

            return client;
        }


        private static RestSharpRestClient BuildRestClientSync(IRestConnection restConnection)
        {
            var client = new RestSharpRestClient
            {
                BaseUrl = restConnection.BaseUrl,
                Timeout = restConnection.Timeout,
                Authenticator = restConnection.AuthenticateSync<IAuthenticator>()
            };

            // use Newtonsoft's for a) proper handling of DateTimeOffset types, b) performance and c) extensability
            client.AddHandler("application/json", () => NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/json", () => NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/x-json", () => NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/javascript", () => NewtonsoftJsonSerializer.Default);
            client.AddHandler("*+json", () => NewtonsoftJsonSerializer.Default);

            return client;
        }

        /// <summary>
        /// Converts to exceptions.
        /// </summary>
        /// <param name="restResponse">The rest response.</param>
        /// <param name="response">The response.</param>
        /// <exception cref="TridentOptionsBuilder.Rest.RestException">
        /// Error calling REST API.  {restResponse.ErrorMessage}
        /// or
        /// Error calling REST API. {restResponse.Content}
        /// </exception>
        /// <summary>
        /// Converts to exceptions.
        /// </summary>
        /// <param name="restResponse">The rest response.</param>
        /// <param name="response">The response.</param>
        /// <exception cref="RestException">
        /// Error calling REST API.  {restResponse.ErrorMessage}
        /// or
        /// Error calling REST API. {restResponse.Content}
        /// </exception>
        private static void NormalizeErrorsToExceptions(IRestResponse restResponse, RestResponse response)
        {
            if (restResponse.ErrorException != null)
            {
                throw new RestException(response, $"Error calling REST API.  {restResponse.ErrorMessage}.  Check inner exception for details.", restResponse.ErrorException);
            }

            if ((int)restResponse.StatusCode >= 300 || (int)restResponse.StatusCode < 200)
            {
                throw new RestException(response, $"Error calling REST API. {restResponse.Content}");
            }
        }

        /// <summary>
        /// Converts to exceptions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restResponse">The rest response.</param>
        /// <param name="response">The response.</param>
        /// <exception cref="RestException">Error calling REST API.  {restResponse.ErrorMessage}</exception>
        /// <exception cref="RestException{T}">Error calling REST API. {restResponse.Content}</exception>
        /// <exception cref="TridentOptionsBuilder.Rest.RestException">Error calling REST API.  {restResponse.ErrorMessage}</exception>
        private static void NormalizeErrorsToExceptions<T>(IRestResponse restResponse, Rest.RestResponse<T> response)
        {
            if (restResponse.ErrorException != null)
            {
                throw new RestException(response, $"Error calling REST API.  {restResponse.ErrorMessage}.  Check inner exception for details.", restResponse.ErrorException);
            }

            if ((int)restResponse.StatusCode >= 300 || (int)restResponse.StatusCode < 200)
            {
                throw new RestException<T>(response, $"Error calling REST API. {restResponse.Content}");
            }
        }
    }
}
