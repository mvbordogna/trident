using System.Collections;
using System.Collections.Generic;

namespace Trident.Contracts.Configuration
{
    /// <summary>
    /// Provides a unit testable interface for the injection of the ConfigurationManager ConnectionStrings interface
    /// </summary>
    public interface IConnectionStringSettings : IEnumerable, IEnumerable<ConnectionStringSettings>
    {
        /// <summary>
        /// Gets the <see cref="ConnectionStringSettings" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.Configuration.ConnectionStringSettings.</returns>
        ConnectionStringSettings this[int index] { get; }

        /// <summary>
        /// Gets the <see cref="ConnectionStringSettings" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Configuration.ConnectionStringSettings.</returns>
        ConnectionStringSettings this[string key] { get; }
    }
}
