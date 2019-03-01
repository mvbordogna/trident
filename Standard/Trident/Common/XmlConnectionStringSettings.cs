using System;
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
    public class XmlConnectionStringSettings : IConnectionStringSettings
    {
        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public System.Configuration.ConnectionStringSettings this[string key]  
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[key];               
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
                try
                {
                    return ConfigurationManager.ConnectionStrings[index];
                }
                catch (ConfigurationErrorsException)
                {
                    return null;
                }
            }
        }

        public IEnumerator<ConnectionStringSettings> GetEnumerator()
        {
            return ConfigurationManager.ConnectionStrings
                .OfType<ConnectionStringSettings>()
                .GetEnumerator();         
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ConfigurationManager.ConnectionStrings
                .GetEnumerator();
        }
    }
}
