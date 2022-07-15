using Trident.Data.Contracts;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Trident.Contracts.Enums;
using Trident.Common;
using Trident.Configuration;
using Trident.Contracts.Configuration;

namespace Trident.Data
{
    /// <summary>
    /// Class provides an implementation of a common interface for retriving connection strings resovled based on a DataSource name mapping SharedConnectionStringResolver.
    /// Implements the <see cref="TridentOptionsBuilder.Data.Contracts.ISharedConnectionStringResolver" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Data.Contracts.ISharedConnectionStringResolver" />\
    /// <remarks>Enum to String mappings should be moved to configuration file. As of now, if a new 
    /// data source is needed, an entry to the dictionary mus be added to the constructor</remarks>
    public class SharedConnectionStringResolver : ISharedConnectionStringResolver
    {
        /// <summary>
        /// The connection string settings
        /// </summary>
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IDBProviderAbstractFactory dBProviderAbstractFactory;

        /// <summary>
        /// The shared data source mappings
        /// </summary>
        private Dictionary<string, string> _sharedDataSourceMappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedConnectionStringResolver"/> class.
        /// </summary>
        /// <param name="connectionStringSettings">The connection string settings.</param>
        public SharedConnectionStringResolver(IConnectionStringSettings connectionStringSettings, IDBProviderAbstractFactory dBProviderAbstractFactory)
        {
            _connectionStringSettings = connectionStringSettings;
            this.dBProviderAbstractFactory = dBProviderAbstractFactory;
            _sharedDataSourceMappings = new Dictionary<string, string>();
            foreach (var conn in connectionStringSettings)
            {
                _sharedDataSourceMappings.Add(conn.Name, conn.ConnectionString);
            }
        }

        /// <summary>
        /// Gets the connection string assocated with the specifid shared data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>System.String.</returns>
        public string GetConnectionString(string dataSource)
        {
            GuardDataBaseConnectionKeyConfigured(dataSource);
            return _sharedDataSourceMappings[dataSource];
        }

        /// <summary>
        /// Guards the data base connection key configured.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        private void GuardDataBaseConnectionKeyConfigured(string dataSource)
        {
            if (!_sharedDataSourceMappings.ContainsKey(dataSource))
                throw new ConfigurationErrorsException($"{nameof(SharedDataSource)}.{dataSource.ToString()} database connection string key is not configured.");

        }

        /// <summary>
        /// Gets the connection object.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>IDbConnection.</returns>
        public IDbConnection GetConnection(string dataSource)
        {
            GuardDataBaseConnectionKeyConfigured(dataSource);
            var conn = _connectionStringSettings[_sharedDataSourceMappings[dataSource]];
            if (conn == null) return null;

            var provider = !string.IsNullOrWhiteSpace(conn.ProviderName) ? conn.ProviderName : "System.Data.SqlClient";
            var connString = conn.ConnectionString;

            DbProviderFactory factory = dBProviderAbstractFactory.GetFactory(provider);
            var connection = factory.CreateConnection();
            connection.ConnectionString = connString;

            return connection;
        }
    }
}