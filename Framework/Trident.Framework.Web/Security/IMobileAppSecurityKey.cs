using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Web.Security
{
    /// <summary>
    /// Interface IMobileAppSecurityKey
    /// </summary>
    public interface IMobileAppSecurityKey
    {
        /// <summary>
        /// Gets the API key.
        /// </summary>
        /// <value>The API key.</value>
        Guid ApiKey { get; }
        /// <summary>
        /// Gets the user key.
        /// </summary>
        /// <value>The user key.</value>
        Guid UserKey { get; }
    }
}
