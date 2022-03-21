using System.Threading.Tasks;
using Trident.Rest.Contracts;

namespace Trident.Rest
{
    /// <summary>
    /// Class RestContext.
    /// Implements the <see cref="TridentOptionsBuilder.Rest.Contracts.IRestContext" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Rest.Contracts.IRestContext" />
    public class RestContext : IRestContext
    {
        /// <summary>
        /// The rest connection
        /// </summary>
        private readonly IRestConnection _restConnection;
        /// <summary>
        /// The rest client
        /// </summary>
        private readonly IRestClient _restClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestContext"/> class.
        /// </summary>
        /// <param name="restConnection">The rest connection.</param>
        /// <param name="restClient">The rest client.</param>
        public RestContext(IRestConnection restConnection, IRestClient restClient)
        {
            _restConnection = restConnection;
            _restClient = restClient;
        }

        /// <summary>
        /// Executes the message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;RestResponse&gt;.</returns>
        public async Task<RestResponse> ExecuteMessage(RestRequest request)
        {
            return await _restClient.Execute(request, _restConnection);
        }

        /// <summary>
        /// Executes the message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;RestResponse&gt;.</returns>
        public RestResponse ExecuteMessageSync(RestRequest request)
        {
            return _restClient.ExecuteSync(request, _restConnection);
        }

        /// <summary>
        /// Executes the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;RestResponse&lt;T&gt;&gt;.</returns>
        public async Task<RestResponse<T>> ExecuteMessage<T>(RestRequest request)
        {
            return await _restClient.Execute<T>(request, _restConnection);
        }

        public RestResponse<T> ExecuteMessageSync<T>(RestRequest request)
               where T : class, new()
        {
            return _restClient.ExecuteSync<T>(request, _restConnection);
        }


    }
}
