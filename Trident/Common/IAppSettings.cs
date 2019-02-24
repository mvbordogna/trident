namespace Trident.Common
{
    /// <summary>
    /// Provides unit testable injection of the AppSettings Interface
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Gets the <see cref="System.String" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        string this[int index] { get; }

        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        string this[string key] { get; }
    }
}
