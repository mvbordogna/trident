namespace Trident.Common
{
    /// <summary>
    /// Provides unit testable injection of the AppSettings Interface
    /// </summary>
    public interface IAppSettings
    {
        

        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        string this[string key] { get; }

        /// <summary>
        /// Returns the key value if exists or the default the key does not exist.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        string GetKeyOrDefault(string key, string defaultValue = null);

        T GetSection<T>(string sectionName = null) where T : class;
    }
}
