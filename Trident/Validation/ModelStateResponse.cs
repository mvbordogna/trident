using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trident.Validation
{
    /// <summary>
    /// Class ModelStateResponse.
    /// Excluded from code coverage because this class is used for
    /// JSON deserialization for controller Tests
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ModelStateResponse
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the state of the model.
        /// </summary>
        /// <value>The state of the model.</value>
        public Dictionary<string, string[]> ModelState { get; set; }

    }
}
