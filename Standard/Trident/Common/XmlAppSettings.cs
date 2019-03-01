using System;
using System.Configuration;

namespace Trident.Common
{
    /// <summary>
    /// Class AppSettings.
    /// Implements the <see cref="Trident.Common.IAppSettings" />
    /// </summary>
    /// <seealso cref="Trident.Common.IAppSettings" />
    public class XmlAppSettings : IAppSettings
    {
        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string this[string key]  
        {
            get
            {
                return ConfigurationManager.AppSettings[key];               
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
                    return ConfigurationManager.AppSettings[index];
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
            return ConfigurationManager.GetSection(sectionName ?? typeof(T).Name) as T;
        }
    }
}
