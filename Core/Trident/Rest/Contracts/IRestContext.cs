using System.Threading.Tasks;

namespace Trident.Rest.Contracts
{
    /// <summary>
    /// Interface IRestContext
    /// </summary>
    public interface IRestContext
    {
        /// <summary>
        /// Executes the message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;RestResponse&gt;.</returns>
        Task<RestResponse> ExecuteMessage(RestRequest request);

        /// <summary>
        /// Executes the message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;RestResponse&gt;.</returns>

        RestResponse ExecuteMessageSync(RestRequest request);

        /// <summary>
        /// Executes the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;RestResponse&lt;T&gt;&gt;.</returns>
        Task<RestResponse<T>> ExecuteMessage<T>(RestRequest request);


        /// <summary>
        /// Executes the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;RestResponse&lt;T&gt;&gt;.</returns>
        RestResponse<T> ExecuteMessageSync<T>(RestRequest request)
               where T : class, new();
    }
}
