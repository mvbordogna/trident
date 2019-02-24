using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Trident.Security.Managers
{
    /// <summary>
    /// Interface ISignInManager
    /// </summary>
    public interface ISignInManager
    {
        /// <summary>
        /// Logins the asynchronous.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>Task&lt;IPrincipal&gt;.</returns>
        Task<IPrincipal> LoginAsync(Guid identifier);
        /// <summary>
        /// Logins the exists.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> LoginExists(Guid identifier);

        /// <summary>
        /// Refreshes the login.
        /// </summary>
        /// <returns>Task.</returns>
        Task RefreshLogin();
        /// <summary>
        /// Logouts this instance.
        /// </summary>
        void Logout();
    }
}