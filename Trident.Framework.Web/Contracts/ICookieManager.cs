using System;
using System.Collections.Generic;
using System.Web;

namespace Trident.Web.Contracts
{
    /// <summary>
    /// Interface ICookieManager
    /// </summary>
    public interface ICookieManager
    {
        /// <summary>
        /// Gets or sets the size of the chunk.
        /// </summary>
        /// <value>The size of the chunk.</value>
        int? ChunkSize { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [throw for partial cookies].
        /// </summary>
        /// <value><c>true</c> if [throw for partial cookies]; otherwise, <c>false</c>.</value>
        bool ThrowForPartialCookies { get; set; }

        /// <summary>
        /// Appends the response cookie.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="HttpOnly">if set to <c>true</c> [HTTP only].</param>
        /// <param name="Secure">if set to <c>true</c> [secure].</param>
        /// <param name="Path">The path.</param>
        /// <param name="Expires">The expires.</param>
        /// <param name="Domain">The domain.</param>
        void AppendResponseCookie(HttpContext context, string key, string value, bool HttpOnly = true, bool Secure = false, string Path = null, DateTime? Expires = null, string Domain = null);
        /// <summary>
        /// Deletes the cookie.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        void DeleteCookie(HttpContext context, string key);
        /// <summary>
        /// Gets the request cookie.
        /// </summary>
        /// <param name="requestCookies">The request cookies.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        string GetRequestCookie(IEnumerable<KeyValuePair<string, string>> requestCookies, string key);
    }
}