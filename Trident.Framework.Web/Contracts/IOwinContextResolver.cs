using Microsoft.Owin;
using Trident.Contracts;

namespace Trident.Web.Contracts
{
    /// <summary>
    /// Interface IOwinContextResolver
    /// Implements the <see cref="Trident.Contracts.IResolver" />
    /// </summary>
    /// <seealso cref="Trident.Contracts.IResolver" />
    public interface IOwinContextResolver : IResolver
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <returns>IOwinContext.</returns>
        IOwinContext GetContext();
    }
}
