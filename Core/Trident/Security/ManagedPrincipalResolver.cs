using System.Security.Claims;
using System.Security.Principal;

namespace Trident.Security
{
    /// <summary>
    /// Class ManagedPrincipalResolver.
    /// Implements the <see cref="TridentOptionsBuilder.Security.IPrincipalResolver" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Security.IPrincipalResolver" />
    public class ManagedPrincipalResolver : IPrincipalResolver
    {

        /// <summary>
        /// The principal
        /// </summary>
        private IPrincipal _principal;
        /// <summary>
        /// The pad lock
        /// </summary>
        private static object pad_lock = new object();

        /// <summary>
        /// Gets the user principal.
        /// </summary>
        /// <returns>IPrincipal.</returns>
        public IPrincipal GetUserPrincipal()
        {
            lock (pad_lock)
            {
                return _principal;
            }
        }


        /// <summary>
        /// Sets the principal.
        /// NOTE: Do not use or expose this method. this is an interim solution until
        /// the Claims Principal can take over HttpContext.Current.User
        /// </summary>
        /// <param name="principal">The principal.</param>
        private void SetPrincipal(IPrincipal principal)
        {
            lock (pad_lock)
            {
                _principal = principal;
            }
        }


    }
}
