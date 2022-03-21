using Trident.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Data
{
    /// <summary>
    /// Provides an implemenaiton of CosmosDB Connection
    /// </summary>
    public class CosmosDbConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public CosmosDbConnection(string connectionString)
        {
            connectionString.GuardIsNotNull(nameof(connectionString));
            parse(connectionString);
        }

        /// <summary>
        /// Gets the account key.
        /// </summary>
        /// <value>The account key.</value>
        public SecureString AccountKey { get; private set; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets the account endpoint.
        /// </summary>
        /// <value>The account endpoint.</value>
        public string AccountEndpoint { get; private set; }

        /// <summary>
        /// Parses the specified connection string.
        /// </summary>
        /// <param name="connStr">The connection string.</param>
        private void parse(string connStr)
        {
            var dict = connStr.ToDictionary(';', '=');    
            AccountKey = dict.GetValueOrDefault(nameof(AccountKey))?.ToSecureString(); 
            AccountEndpoint = dict.GetValueOrDefault(nameof(AccountEndpoint));
            DatabaseName = dict.GetValueOrDefault(nameof(DatabaseName));
        }
    }
}
