using Trident.Web.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Trident.Web.Security
{
    /// <summary>
    /// This handles cookies that are limited by per cookie length. It breaks down long cookies for responses, and reassembles them
    /// from requests.
    /// Implements the <see cref="Trident.Web.Contracts.ICookieManager" />
    /// </summary>
    /// <seealso cref="Trident.Web.Contracts.ICookieManager" />
    public class HttpContextChunkingCookieManager : ICookieManager
    {
        /// <summary>
        /// Creates a new instance of ChunkingCookieManager.
        /// </summary>
        public HttpContextChunkingCookieManager()
        {
            ChunkSize = 4070;
            ThrowForPartialCookies = true;
        }

        /// <summary>
        /// The maximum size of cookie to send back to the client. If a cookie exceeds this size it will be broken down into multiple
        /// cookies. Set this value to null to disable this behavior. The default is 4070 characters, which is supported by all
        /// common browsers.
        /// Note that browsers may also have limits on the total size of all cookies per domain, and on the number of cookies per domain.
        /// </summary>
        /// <value>The size of the chunk.</value>
        public int? ChunkSize { get; set; }

        /// <summary>
        /// Throw if not all chunks of a cookie are available on a request for re-assembly.
        /// </summary>
        /// <value><c>true</c> if [throw for partial cookies]; otherwise, <c>false</c>.</value>
        public bool ThrowForPartialCookies { get; set; }

        // Parse the "chunks:XX" to determine how many chunks there should be.
        /// <summary>
        /// Parses the chunks count.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        private static int ParseChunksCount(string value)
        {
            if (value != null && value.StartsWith("chunks:", StringComparison.Ordinal))
            {
                var chunksCountString = value.Substring("chunks:".Length);
                int chunksCount;
                if (int.TryParse(chunksCountString, NumberStyles.None, CultureInfo.InvariantCulture, out chunksCount))
                {
                    return chunksCount;
                }
            }
            return 0;
        }

        /// <summary>
        /// Get the reassembled cookie. Non chunked cookies are returned normally.
        /// Cookies with missing chunks just have their "chunks:XX" header returned.
        /// </summary>
        /// <param name="requestCookies">The request cookies.</param>
        /// <param name="key">The key.</param>
        /// <returns>The reassembled cookie, if any, or null.</returns>
        /// <exception cref="System.ArgumentNullException">requestCookies</exception>
        /// <exception cref="System.FormatException"></exception>
        /// <exception cref="ArgumentNullException">requestCookies</exception>
        /// <exception cref="FormatException"></exception>
        public string GetRequestCookie(IEnumerable<KeyValuePair<string, string>> requestCookies, string key)
        {
            if (requestCookies == null)
            {
                throw new ArgumentNullException(nameof(requestCookies));
            }
            var cookieDefault = default(KeyValuePair<string, string>);
            var escapedKey = Uri.EscapeDataString(key);
            var chunkCookie = requestCookies.FirstOrDefault(t => t.Key == escapedKey);
            if (chunkCookie.Equals(cookieDefault)) return null;

            var chunksCount = ParseChunksCount(chunkCookie.Value);
            if (chunksCount > 0)
            {
                var quoted = false;
                var chunks = new string[chunksCount];
                for (var chunkId = 1; chunkId <= chunksCount; chunkId++)
                {
                    var chunkKey = string.Concat(
                                            escapedKey,
                                            "C",
                                            chunkId.ToString(CultureInfo.InvariantCulture));
                    var chunk = requestCookies.FirstOrDefault(t => t.Key == chunkKey);
                    if (chunk.Equals(cookieDefault))
                    {
                        if (ThrowForPartialCookies)
                        {
                            var totalSize = 0;
                            for (var i = 0; i < chunkId - 1; i++)
                            {
                                totalSize += chunks[i].Length;
                            }
                            throw new FormatException(
                                string.Format(CultureInfo.CurrentCulture, "Cookie is incomplete {0}, ", chunkId - 1, chunksCount, totalSize));
                        }
                        // Missing chunk, abort by returning the original cookie value. It may have been a false positive?
                        return chunkCookie.Value;
                    }
                    if (IsQuoted(chunk.Value))
                    {
                        // Note: Since we assume these cookies were generated by our code, then we can assume that if one cookie has quotes then they all do.
                        quoted = true;
                        chunk = new KeyValuePair<string, string>(chunk.Key, RemoveQuotes(chunk.Value));
                    }
                    chunks[chunkId - 1] = chunk.Value;
                }
                var merged = string.Join(string.Empty, chunks);
                if (quoted)
                {
                    merged = Quote(merged);
                }
                return merged;
            }
            return chunkCookie.Value;
        }

        /// <summary>
        /// Appends a new response cookie to the Set-Cookie header. If the cookie is larger than the given size limit
        /// then it will be broken down into multiple cookies as follows:
        /// Set-Cookie: CookieName=chunks:3; path=/
        /// Set-Cookie: CookieNameC1=Segment1; path=/
        /// Set-Cookie: CookieNameC2=Segment2; path=/
        /// Set-Cookie: CookieNameC3=Segment3; path=/
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="httpOnly">if set to <c>true</c> [HTTP only].</param>
        /// <param name="secure">if set to <c>true</c> [secure].</param>
        /// <param name="path">The path.</param>
        /// <param name="expires">The expires.</param>
        /// <param name="domain">The domain.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        /// <exception cref="System.InvalidOperationException">Cookie Limit too small</exception>
        /// <exception cref="ArgumentNullException">context</exception>
        /// <exception cref="InvalidOperationException">Cookie Limit too small</exception>
        public void AppendResponseCookie(HttpContext context, string key, string value,
            bool httpOnly = true,
            bool secure = false,
            string path = null,
            DateTime? expires = null,
            string domain = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            value = value ?? string.Empty;
            var quoted = false;
            if (IsQuoted(value))
            {
                quoted = true;
                value = RemoveQuotes(value);
            }
            var escapedValue = Uri.EscapeDataString(value);
            var cookie = new HttpCookie(key, escapedValue)
            {
                Domain = domain,
                Path = path,
                HttpOnly = httpOnly,
                Secure = secure
            };
            if (expires.HasValue)
                cookie.Expires = expires.Value;

            var escapedKey = Uri.EscapeDataString(key);
            var prefix = escapedKey + "=";
            // Normal cookie
            if (!ChunkSize.HasValue || ChunkSize.Value > prefix.Length + escapedValue.Length + (quoted ? 2 : 0))
            {
                var setCookieValue = quoted ? Quote(escapedValue) : escapedValue;
                cookie.Value = setCookieValue;
                context.Response.Cookies.Add(cookie);
            }
            else if (ChunkSize.Value < prefix.Length + (quoted ? 2 : 0) + 10)
            {
                // 10 is the minimum data we want to put in an individual cookie, including the cookie chunk identifier "CXX".
                // No room for data, we can't chunk the options and name
                throw new InvalidOperationException("Cookie Limit too small");
            }
            else
            {
                // Break the cookie down into multiple cookies.
                // Key = CookieName, value = "Segment1Segment2Segment2"
                // Set-Cookie: CookieName=chunks:3; path=/
                // Set-Cookie: CookieNameC1="Segment1"; path=/
                // Set-Cookie: CookieNameC2="Segment2"; path=/
                // Set-Cookie: CookieNameC3="Segment3"; path=/
                var dataSizePerCookie = ChunkSize.Value - prefix.Length - (quoted ? 2 : 0) - 3; // Budget 3 chars for the chunkid.
                var cookieChunkCount = (int)Math.Ceiling(escapedValue.Length * 1.0 / dataSizePerCookie);

                var chunksCookie = new HttpCookie(escapedKey, "chunks:" + cookieChunkCount.ToString(CultureInfo.InvariantCulture))
                {
                    Domain = domain,
                    Path = path,
                    HttpOnly = httpOnly,
                    Secure = secure
                };
                if (expires.HasValue)
                {
                    chunksCookie.Expires = expires.Value;
                }

                context.Response.Cookies.Add(chunksCookie);

                var offset = 0;
                for (var chunkId = 1; chunkId <= cookieChunkCount; chunkId++)
                {
                    var remainingLength = escapedValue.Length - offset;
                    var length = Math.Min(dataSizePerCookie, remainingLength);
                    var segment = escapedValue.Substring(offset, length);
                    offset += length;

                    var name = string.Concat(escapedKey, "C", chunkId.ToString(CultureInfo.InvariantCulture));

                    var cookieValue = string.Concat(quoted ? "\"" : string.Empty, segment, quoted ? "\"" : string.Empty);
                    var chunkCookie = new HttpCookie(name, cookieValue)
                    {
                        Domain = domain,
                        Path = path,
                        HttpOnly = httpOnly,
                        Secure = secure
                    };
                    if (expires.HasValue)
                    {
                        chunkCookie.Expires = expires.Value;
                    }

                    context.Response.Cookies.Add(chunkCookie);
                }
            }
        }

        /// <summary>
        /// Deletes the cookie with the given key by setting an expired state. If a matching chunked cookie exists on
        /// the request, delete each chunk.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        /// <exception cref="ArgumentNullException">context</exception>
        public void DeleteCookie(HttpContext context, string key)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var escapedKey = Uri.EscapeDataString(key);
            var firstCookieValue = context.Request.Cookies[escapedKey];
            if (firstCookieValue != null)
            {
                // delete the primary cookie
                DeleteCookieWithName(context, escapedKey);

                // delete any chunk cookies too
                var chunksCount = ParseChunksCount(firstCookieValue.Value);
                for(var chunkId = 1; chunkId <= chunksCount; chunkId++)
                {
                    var name = string.Concat(escapedKey, "C", chunkId.ToString(CultureInfo.InvariantCulture));
                    DeleteCookieWithName(context, name);
                }
            }
        }

        /// <summary>
        /// Deletes the name of the cookie with.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="escapedKey">The escaped key.</param>
        private static void DeleteCookieWithName(HttpContext context, string escapedKey)
        {
            // it should not be present in Response yet, but if so, delete
            context.Response.Cookies.Remove(escapedKey);

            // delete a cookie by adding an expired cookie of the same name to the Response
            context.Response.Cookies.Add(new HttpCookie(escapedKey)
            {
                Expires = DateTime.Now.AddDays(-1)
            });
        }

        /// <summary>
        /// Determines whether the specified value is quoted.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is quoted; otherwise, <c>false</c>.</returns>
        private static bool IsQuoted(string value)
        {
            return value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"';
        }

        /// <summary>
        /// Removes the quotes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        private static string RemoveQuotes(string value)
        {
            return value.Substring(1, value.Length - 2);
        }

        /// <summary>
        /// Quotes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        private static string Quote(string value)
        {
            return '"' + value + '"';
        }
    }
}
