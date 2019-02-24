using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Trident.Security
{
    /// <summary>
    /// Interface IPrincipalResolver
    /// </summary>
    public interface IPrincipalResolver
    {
        /// <summary>
        /// Gets the user principal.
        /// </summary>
        /// <returns>IPrincipal.</returns>
        IPrincipal GetUserPrincipal();
    }
}