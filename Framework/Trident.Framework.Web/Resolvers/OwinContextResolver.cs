using Microsoft.Owin;
using Trident.Web.Contracts;

namespace Trident.Web.Resolvers
{
    /// <summary>
    /// Class OwinContextResolver.
    /// Implements the <see cref="Trident.Web.Contracts.IOwinContextResolver" />
    /// </summary>
    /// <seealso cref="Trident.Web.Contracts.IOwinContextResolver" />
    public class OwinContextResolver:IOwinContextResolver
    {
        /// <summary>
        /// The owin context
        /// </summary>
        private IOwinContext _owinContext = null;

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <returns>IOwinContext.</returns>
        public IOwinContext GetContext()
        {
            return _owinContext;
        }

        /// <summary>
        /// Initializes the specified owin context.
        /// </summary>
        /// <param name="owinContext">The owin context.</param>
        private void Initialize(IOwinContext owinContext)
        {
            _owinContext = owinContext;
        }
    }
}
