using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Domain
{
    /// <summary>
    /// A data class for storing entity comparison results
    /// </summary>
    public class EntityCompareResult
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; set; }
        /// <summary>
        /// Gets or sets the original value.
        /// </summary>
        /// <value>The original value.</value>
        public string OriginalValue { get; set; }
        /// <summary>
        /// Creates new value.
        /// </summary>
        /// <value>The new value.</value>
        public string NewValue { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0} Changed from [{1}] to [{2}]", this.PropertyName, this.OriginalValue, this.NewValue);
        }
    }
}
