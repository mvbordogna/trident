using System;
using System.Collections.Generic;

namespace Trident.IoC
{
    /// <summary>
    /// Interface IBootstrapScanner
    /// </summary>
    public interface IBootstrapScanner
    {
        /// <summary>
        /// Gets the bootstrappers.
        /// </summary>
        /// <returns>List&lt;Type&gt;.</returns>
        List<Type> GetBootstrappers();
    }
}
