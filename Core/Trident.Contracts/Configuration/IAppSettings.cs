using System.Collections;
using System.Collections.Generic;

namespace Trident.Contracts.Configuration
{
    public interface IAppSettings
    {
        string this[string key] { get; }

        /// <summary>
        /// Returns the key value if exists or the default the key does not exist.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        string GetKeyOrDefault(string key, string defaultValue = null);

        /// <summary>
        /// Gets an application section of the specified name and returns the section serialized as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        T GetSection<T>(string sectionName = null) where T : class;

        /// <summary>
        /// Returns the Connections strings in the connection string section
        /// </summary>
        IConnectionStringSettings ConnectionStrings { get; }
    }
}
