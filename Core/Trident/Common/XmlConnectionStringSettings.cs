using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trident.Contracts.Configuration;

namespace Trident.Common
{
    /// <summary>
    /// Class AppSettings.
    /// Implements the <see cref="TridentOptionsBuilder.Common.IConnectionStringSettings" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Common.IConnectionStringSettings" />
    public class XmlConnectionStringSettings : IConnectionStringSettings
    {
        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public ConnectionStringSettings this[string key]
        {
            get
            {
                return null;
                //return System.Configuration.ConfigurationManager.ConnectionStrings[key];
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
                try
                {
                    return null;
                    //return System.Configuration.ConfigurationManager.ConnectionStrings[index];
                }
                catch
                {
                    return null;
                }
            }
        }

        public void Add(string key, string connectionString, string provider)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ConnectionStringSettings> GetEnumerator()
        {
            return null; 
            //return System.Configuration.ConfigurationManager.ConnectionStrings
            //    .OfType<System.Configuration.ConnectionStringSettings>()
            //    .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
            //return System.Configuration.ConfigurationManager.ConnectionStrings
            //    .GetEnumerator();
        }
    }
}
