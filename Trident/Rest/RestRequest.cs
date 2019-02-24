using System.Collections.Generic;

namespace Trident.Rest
{
    /// <summary>
    /// Class RestRequest.
    /// </summary>
    public class RestRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestRequest"/> class.
        /// </summary>
        public RestRequest()
        {
            Parameters = new List<RestParameter>();
        }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public RestMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public List<RestParameter> Parameters { get; set; }
    }
}