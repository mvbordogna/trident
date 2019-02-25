using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Trident.Web.Contracts
{
    /// <summary>
    /// Interface ISecurityTransitionManager
    /// </summary>
    public interface ISecurityTransitionManager
    {
        /// <summary>
        /// Attempts to transition from a legacy cookie to the RA managed cookie
        /// To avoid a use forced logout.
        /// </summary>
        /// <returns>Task&lt;Tuple&lt;IPrincipal, System.Boolean&gt;&gt;.</returns>
        Task<Tuple<IPrincipal, bool>> TryLegacySecurityTransistion();
    }
}
