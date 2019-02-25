using Trident.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace BlueChilli.Trident.Web.Resolvers
{
    /// <summary>
    /// Class HttpContextPrincipalResolver.
    /// Implements the <see cref="Trident.Security.IPrincipalResolver" />
    /// </summary>
    /// <seealso cref="Trident.Security.IPrincipalResolver" />
    public class HttpContextPrincipalResolver : IPrincipalResolver
    {
        /// <summary>
        /// Gets the user principal.
        /// </summary>
        /// <returns>IPrincipal.</returns>
        public IPrincipal GetUserPrincipal()
        {
            return HttpContext.Current?.User;
        }
    }
}