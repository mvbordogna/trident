using System;
using Trident.Contracts.Configuration;
using Trident.Extensions;

namespace Trident.Common
{
    /// <summary>
    /// Class AppSettings.
    /// Implements the <see cref="TridentOptionsBuilder.Common.IAppSettings" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Common.IAppSettings" />
    public class XmlAppSettings : IAppSettings
    {
        public Contracts.Configuration.IConnectionStringSettings ConnectionStrings => throw new NotImplementedException();

        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string this[string key]  
        {
            get
            {
                return null;
                //return System.Configuration.ConfigurationManager.AppSettings[key];               
            }            
        }
        /// <summary>
        /// Gets the <see cref="System.String" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        public string this[int index]
        {
            get
            {
                try
                {
                    return null;
                   // return System.Configuration.ConfigurationManager.AppSettings[index];
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }
            }
        }

        public T GetSection<T>(string sectionName = null)
            where T : class
        {
            return null;
            //return System.Configuration.ConfigurationManager.GetSection(sectionName ?? typeof(T).Name) as T;
        }

        public string GetKeyOrDefault(string key, string defaultValue = default)
        {
            return defaultValue;
        }
    }
}
