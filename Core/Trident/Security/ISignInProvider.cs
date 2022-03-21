using System.Security.Principal;

namespace Trident.Security
{
    /// <summary>
    /// Interface ISignInProvider
    /// </summary>
    public interface ISignInProvider
    {
        /// <summary>
        /// Logons the specified identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        void Logon(params IIdentity[] identity);
        /// <summary>
        /// Logouts this instance.
        /// </summary>
        void Logout();
    }
}
