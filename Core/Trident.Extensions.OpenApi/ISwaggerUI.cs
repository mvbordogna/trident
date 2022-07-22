using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.OpenApi.Models;

namespace Trident.Extensions.OpenApi.Abstractions
{
    /// <summary>
    /// This provides interfaces to the <see cref="SwaggerUI"/> class.
    /// </summary>
    public interface ISwaggerUI
    {
        /// <summary>
        /// Adds metadata to build Open API document.
        /// </summary>
        /// <param name="info"><see cref="OpenApiInfo"/> instance.</param>
        /// <returns><see cref="IDocument"/> instance.</returns>
        ISwaggerUI AddMetadata(OpenApiInfo info);

        /// <summary>
        /// Adds server details.
        /// </summary>
        /// <param name="req"><see cref="HttpRequestData"/> instance.</param>
        /// <param name="routePrefix">Route prefix value.</param>
        /// <returns><see cref="IDocument"/> instance.</returns>
        ISwaggerUI AddServer(HttpRequestData req, string routePrefix);

        /// <summary>
        /// Builds Open API document.
        /// </summary>
        /// <returns><see cref="IDocument"/> instance.</returns>
        Task<ISwaggerUI> BuildAsync();

        /// <summary>
        /// Renders Open API UI in HTML.
        /// </summary>
        /// <param name="endpoint">The endpoint of the Swagger document.</param>
        /// <param name="authKey">API key of the HTTP endpoint to render Open API document.</param>
        /// <returns>Open API UI in HTML.</returns>
        Task<string> RenderAsync(string endpoint, string authKey = null);
    }
}
