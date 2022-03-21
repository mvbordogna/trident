using System;
using System.Collections.Generic;
using System.Net;

namespace Trident.Rest
{
    /// <summary>
    /// Class RestResponse.
    /// Implements the <see cref="TridentOptionsBuilder.Rest.RestResponse" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TridentOptionsBuilder.Rest.RestResponse" />
    public class RestResponse<T> : RestResponse
    {
        /// <summary>
        /// Deserialized entity data.
        /// </summary>
        /// <value>The data.</value>
        public T Data { get; set; }
    }


    /// <summary>
    /// Class RestResponse.
    /// Implements the <see cref="TridentOptionsBuilder.Rest.RestResponse" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Rest.RestResponse" />
    public class RestResponse
    {
        /// <summary>
        /// The RestRequest that was made to get this RestResponse
        /// </summary>
        /// <value>The request.</value>
        /// <remarks>Mainly for debugging if ResponseStatus is not OK</remarks>
        public RestRequest Request { get; set; }

        /// <summary>
        /// MIME content type of response
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>
        /// Length in bytes of the response content
        /// </summary>
        /// <value>The length of the content.</value>
        public long ContentLength { get; set; }

        /// <summary>
        /// Encoding of the response content
        /// </summary>
        /// <value>The content encoding.</value>
        public string ContentEncoding { get; set; }

        /// <summary>
        /// String representation of response content
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; set; }

        /// <summary>
        /// HTTP response status code
        /// </summary>
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Description of HTTP status returned
        /// </summary>
        /// <value>The status description.</value>
        public string StatusDescription { get; set; }

        /// <summary>
        /// Response content
        /// </summary>
        /// <value>The raw bytes.</value>
        public byte[] RawBytes { get; set; }

        /// <summary>
        /// The URL that actually responded to the content (different from request if redirected)
        /// </summary>
        /// <value>The response URI.</value>
        public Uri ResponseUri { get; set; }

        /// <summary>
        /// HttpWebResponse.Server
        /// </summary>
        /// <value>The server.</value>
        public string Server { get; set; }

        ///// <summary>
        ///// Cookies returned by server with the response
        ///// </summary>
        //public IList<RestResponseCookie> Cookies { get; }

        /// <summary>
        /// Headers returned by server with the response
        /// </summary>
        /// <value>The headers.</value>
        public IList<RestParameter> Headers { get; }

        ///// <summary>
        ///// Status of the request. Will return Error for transport errors.
        ///// HTTP errors will still return ResponseStatus.Completed, check StatusCode instead
        ///// </summary>
        //public ResponseStatus ResponseStatus { get; set; }

        /// <summary>
        /// Transport or other non-HTTP error generated while attempting request
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Exceptions thrown during the request, if any.
        /// </summary>
        /// <value>The error exception.</value>
        /// <remarks>Will contain only network transport or framework exceptions thrown during the request.
        /// HTTP protocol errors are handled by RestSharp and will not appear here.</remarks>
        public Exception ErrorException { get; set; }

    }
}