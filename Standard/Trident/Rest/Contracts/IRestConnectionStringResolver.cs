using Trident.Contracts.Enums;

namespace Trident.Rest.Contracts
{
    /// <summary>
    /// Interface IRestConnectionStringResolver
    /// </summary>
    public interface IRestConnectionStringResolver
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>System.String.</returns>
        string GetConnectionString(string dataSource);

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>IRestConnection.</returns>
        IRestConnection GetConnection(string dataSource);
    }
}