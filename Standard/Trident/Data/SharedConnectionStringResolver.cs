//using Trident.Data.Contracts;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.Common;
//using Trident.Contracts.Enums;
//using Trident.Common;

//namespace Trident.Data
//{
//    /// <summary>
//    /// Class provides an implementation of a common interface for retriving connection strings resovled based on a DataSource name mapping SharedConnectionStringResolver.
//    /// Implements the <see cref="Trident.Data.Contracts.ISharedConnectionStringResolver" />
//    /// </summary>
//    /// <seealso cref="Trident.Data.Contracts.ISharedConnectionStringResolver" />\
//    /// <remarks>Enum to String mappings should be moved to configuration file. As of now, if a new 
//    /// data source is needed, an entry to the dictionary mus be added to the constructor</remarks>
//    public class SharedConnectionStringResolver : ISharedConnectionStringResolver
//    {
//        /// <summary>
//        /// The connection string settings
//        /// </summary>
//        private readonly IConnectionStringSettings _connectionStringSettings;
//        /// <summary>
//        /// The shared data source mappings
//        /// </summary>
//        private Dictionary<SharedDataSource, string> _sharedDataSourceMappings;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="SharedConnectionStringResolver"/> class.
//        /// </summary>
//        /// <param name="connectionStringSettings">The connection string settings.</param>
//        public SharedConnectionStringResolver(IConnectionStringSettings connectionStringSettings)
//        {
//            _connectionStringSettings = connectionStringSettings;
//            _sharedDataSourceMappings = new Dictionary<SharedDataSource, string>();
//            _sharedDataSourceMappings.Add(SharedDataSource.DefaultDB, nameof(SharedDataSource.DefaultDB));
//            _sharedDataSourceMappings.Add(SharedDataSource.DefualtEFCoreDb, nameof(SharedDataSource.DefualtEFCoreDb));
//            _sharedDataSourceMappings.Add(SharedDataSource.DefaultCosmosDB, nameof(SharedDataSource.DefaultCosmosDB));
//            _sharedDataSourceMappings.Add(SharedDataSource.AzureStorageQueues, nameof(SharedDataSource.AzureStorageQueues));           
//        }

//        /// <summary>
//        /// Gets the connection string assocated with the specifid shared data source.
//        /// </summary>
//        /// <param name="dataSource">The data source.</param>
//        /// <returns>System.String.</returns>
//        public string GetConnectionString(SharedDataSource dataSource)
//        {
//            GuardDataBaseConnectionKeyConfigured(dataSource);
//            return _connectionStringSettings[_sharedDataSourceMappings[dataSource]]?.ConnectionString;
//        }

//        /// <summary>
//        /// Guards the data base connection key configured.
//        /// </summary>
//        /// <param name="dataSource">The data source.</param>
//        /// <exception cref="ConfigurationErrorsException"></exception>
//        private void GuardDataBaseConnectionKeyConfigured(SharedDataSource dataSource)
//        {
//            if (!_sharedDataSourceMappings.ContainsKey(dataSource))
//                throw new ConfigurationErrorsException($"{nameof(SharedDataSource)}.{dataSource.ToString()} database connection string key is not configured.");

//        }

//        /// <summary>
//        /// Gets the connection object.
//        /// </summary>
//        /// <param name="dataSource">The data source.</param>
//        /// <returns>IDbConnection.</returns>
//        public IDbConnection GetConnection(SharedDataSource dataSource)
//        {
//            GuardDataBaseConnectionKeyConfigured(dataSource);
//            var conn = _connectionStringSettings[_sharedDataSourceMappings[dataSource]];
//            if (conn == null) return null;

//            var provider = !string.IsNullOrWhiteSpace(conn.ProviderName) ? conn.ProviderName : "System.Data.SqlClient";
//            var connString = conn.ConnectionString;

//            DbProviderFactory factory =  DbProviderFactories.GetFactory(provider);
//            var connection = factory.CreateConnection();
//            connection.ConnectionString = connString;

//            return connection;
//        }
//    }
//}