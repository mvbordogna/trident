using System.Threading.Tasks;

namespace Trident.Rest.Contracts
{
    /// <summary>
    /// Interface IRestClient
    /// </summary>
    public interface IRestClient
    {
        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestResponse&gt;.</returns>
        Task<RestResponse> Execute(RestRequest request, IRestConnection restConnection);

        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestResponse&lt;T&gt;&gt;.</returns>
        Task<RestResponse<T>> Execute<T>(RestRequest request, IRestConnection restConnection);
        
        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestResponse&gt;.</returns>
        RestResponse ExecuteSync(RestRequest request, IRestConnection restConnection);

        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <param name="restConnection">The rest connection.</param>
        /// <returns>Task&lt;RestResponse&lt;T&gt;&gt;.</returns>
        RestResponse<T> ExecuteSync<T>(RestRequest request, IRestConnection restConnection)
               where T : class, new();
    }
}
