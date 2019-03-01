using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Trident.Common
{
    /// <summary>
    /// Class AppSettings.
    /// Implements the <see cref="Trident.Common.IConnectionStringSettings" />
    /// </summary>
    /// <seealso cref="Trident.Common.IConnectionStringSettings" />
    public class JsonConnectionStringSettings : IConnectionStringSettings
    {
        private readonly IConfigurationRoot configurationRoot;
        private List<ConnectionStringSettings> connStrings;

        public JsonConnectionStringSettings(IConfigurationRoot configurationRoot)
        {
            this.configurationRoot = configurationRoot;
            connStrings = configurationRoot.GetSection("ConnectionStrings")
                       .GetChildren()
                       .Select(x=> (!string.IsNullOrWhiteSpace(x.Value)  && !string.IsNullOrWhiteSpace(x.Key))
                        ? new System.Configuration.ConnectionStringSettings(x.Key, x.Value)
                        : null)
                        .Where(x=> x != null)
                       .ToList();
        }

        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public System.Configuration.ConnectionStringSettings this[string key]
        {
            get
            {
                var val = configurationRoot.GetConnectionString(key);
                if (!string.IsNullOrWhiteSpace(val))
                {
                    return new System.Configuration.ConnectionStringSettings(key, val);
                }

                return null;
            }
        }
        /// <summary>
        /// Gets the <see cref="System.String" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        public System.Configuration.ConnectionStringSettings this[int index]
        {
            get
            {
                return (connStrings.Count > index) 
                    ? connStrings[index] 
                    : null;
            }

        }  

        public IEnumerator<ConnectionStringSettings> GetEnumerator()
        {
            return connStrings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return connStrings.GetEnumerator();
        }
    }
}
