namespace Trident.Rest
{
    /// <summary>
    /// REST Parameters locations
    /// </summary>
    public enum RestParameterType
    {
        /// <summary>
        /// The cookie
        /// </summary>
        Cookie,

        /// <summary>
        /// The get or post
        /// </summary>
        GetOrPost,

        /// <summary>
        /// The URL segment
        /// </summary>
        UrlSegment,

        /// <summary>
        /// The HTTP header
        /// </summary>
        HttpHeader,

        /// <summary>
        /// The request body
        /// </summary>
        RequestBody,

        /// <summary>
        /// The query string
        /// </summary>
        QueryString
    }
}