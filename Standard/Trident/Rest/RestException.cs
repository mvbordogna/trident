using System;

namespace Trident.Rest
{
    /// <summary>
    /// Class RestException.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class RestException : Exception
    {
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>The response.</value>
        public RestResponse Response { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestException"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="message">The message.</param>
        public RestException(RestResponse response, string message) : base(message)
        {
            Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestException"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RestException(RestResponse response, string message, Exception innerException) : base(message, innerException)
        {
            Response = response;
        }
    }

    /// <summary>
    /// Class RestException.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Exception" />
    public class RestException<T> : Exception
    {
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>The response.</value>
        public RestResponse<T> Response { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestException{T}"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="message">The message.</param>
        public RestException(RestResponse<T> response, string message) : base(message)
        {
            Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestException{T}"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RestException(RestResponse<T> response, string message, Exception innerException) : base(message, innerException)
        {
            Response = response;
        }
    }
}