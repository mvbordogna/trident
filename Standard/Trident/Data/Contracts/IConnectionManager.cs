namespace Trident.Data
{
    /// <summary>
    /// Interface IConnectionManager
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// Opens this instance.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

    }
}