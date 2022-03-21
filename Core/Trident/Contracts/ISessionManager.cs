namespace Trident.Contracts
{
    /// <summary>
    /// When implemented in derived classes provides an interface for a session manager
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void SetItem<T>(string key, T value);

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        T GetItem<T>(string key);

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="key">The key.</param>
        void DeleteItem(string key);

        /// <summary>
        /// Clears all.
        /// </summary>
        void ClearAll();
    }
}
