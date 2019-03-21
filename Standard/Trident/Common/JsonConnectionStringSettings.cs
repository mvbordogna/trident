using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trident.Extensions;

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
                        ? new ConnectionStringSettings(x.Key, x.Value)
                        : null)
                        .Where(x=> x != null)
                       .ToList();
            connStrings.ForEach(x => SetProviderName(x));          

        }

        private void SetProviderName(ConnectionStringSettings x)
        {
            var dict = x.ConnectionString.ToLower().ToDictionary(';', '=');
            x.ProviderName = dict.GetValueOrDefault("providername");          
        }

        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public ConnectionStringSettings this[string key]
        {
            get
            {
                var val = configurationRoot.GetConnectionString(key);
                if (!string.IsNullOrWhiteSpace(val))
                {
                    return new ConnectionStringSettings(key, val);
                }

                return null;
            }
        }
        /// <summary>
        /// Gets the <see cref="System.String" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        public ConnectionStringSettings this[int index]
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
