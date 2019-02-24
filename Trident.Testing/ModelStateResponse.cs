using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHelpers
{
    /// <summary>
    /// Class ModelStateResponse.
    /// </summary>
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
