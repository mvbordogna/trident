using System.Collections.Generic;
using Trident.Common;
using Trident.Configuration;
using Trident.Contracts.Configuration;
using Trident.Contracts.Enums;
using Trident.Rest.Contracts;

namespace Trident.Rest
{
    /// <summary>
    /// Class RestConnectionStringResolver.
    /// Implements the <see cref="TridentOptionsBuilder.Rest.Contracts.IRestConnectionStringResolver" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Rest.Contracts.IRestConnectionStringResolver" />
    public class RestConnectionStringResolver : IRestConnectionStringResolver
    {
        /// <summary>
        /// The application settings
        /// </summary>
        private readonly IAppSettings _appSettings;
        /// <summary>
        /// The rest data source mappings
        /// </summary>
        private readonly Dictionary<string, string> _restDataSourceMappings;
        /// <summary>
        /// The authentication factory
        /// </summary>
        private readonly IRestAuthenticationFactory _authFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestConnectionStringResolver"/> class.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <param name="authFactory">The authentication factory.</param>
        public RestConnectionStringResolver(IAppSettings appSettings, IRestAuthenticationFactory authFactory)
        {
            _appSettings = appSettings;
            _authFactory = authFactory;
            _restDataSourceMappings = new Dictionary<string, string>();

            //this is ok for now but just like the other one we should move this registration to a config            
            //_restDataSourceMappings.Add(SharedDataSource.RestMicroServiceData, "MicroServiceDataSource");
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>System.String.</returns>
        public string GetConnectionString(string dataSource)
        {
            GuardDataBaseConnectionKeyConfigured(dataSource);
            return _appSettings[_restDataSourceMappings[dataSource]];
        }

        /// <summary>
        /// Guards the data base connection key configured.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        private void GuardDataBaseConnectionKeyConfigured(string dataSource)
        {
            if (!_restDataSourceMappings.ContainsKey(dataSource))
            {
                throw new ConfigurationErrorsException($"{nameof(SharedDataSource)}.{dataSource} REST connection string key is not configured.");
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns>IRestConnection.</returns>
        public IRestConnection GetConnection(string dataSource)
        {
            var conn = GetConnectionString(dataSource);
            if (conn == null)
            {
                return null;
            }

            var authProvider = _authFactory.GetAuthenticationProvider(dataSource);
            return new RestConnection(conn, authProvider);
        }
    }
}