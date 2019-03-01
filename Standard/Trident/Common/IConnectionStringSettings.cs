using System.Collections;
using System.Collections.Generic;

namespace Trident.Common
{
    /// <summary>
    /// Provides a unit testable interface for the injection of the ConfigurationManager ConnectionStrings interface
    /// </summary>
    public interface IConnectionStringSettings: IEnumerable, IEnumerable<System.Configuration.ConnectionStringSettings>
    {
        /// <summary>
        /// Gets the <see cref="System.Configuration.ConnectionStringSettings" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.Configuration.ConnectionStringSettings.</returns>
        System.Configuration.ConnectionStringSettings this[int index] { get; }

        /// <summary>
        /// Gets the <see cref="System.Configuration.ConnectionStringSettings" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Configuration.ConnectionStringSettings.</returns>
        System.Configuration.ConnectionStringSettings  this[string key] { get; }
    }
}
