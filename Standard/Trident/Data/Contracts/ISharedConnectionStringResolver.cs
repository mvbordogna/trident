using System.Data;
using Trident.Contracts.Enums;

namespace Trident.Data.Contracts
{
    /// <summary>
    /// Interface ISharedConnectionStringResolver
    /// </summary>
    public interface ISharedConnectionStringResolver
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
        /// <returns>IDbConnection.</returns>
        IDbConnection GetConnection(string dataSource);
    }
}