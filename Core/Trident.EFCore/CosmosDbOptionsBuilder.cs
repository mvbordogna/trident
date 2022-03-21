using Microsoft.EntityFrameworkCore;
using Trident.Data;
using Trident.Data.Contracts;
using Trident.Extensions;
using Trident.EFCore.Contracts;

namespace Trident.EFCore
{
    /// <summary>
    /// Class CosmosDbOptionsBuilder.
    /// Implements the <see cref="TridentOptionsBuilder.EFCore.Contracts.IOptionsBuilder" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.EFCore.Contracts.IOptionsBuilder" />
    public class CosmosDbOptionsBuilder : IOptionsBuilder
    {
        /// <summary>
        /// The shared connection string resolver
        /// </summary>
        private readonly ISharedConnectionStringResolver _sharedConnectionStringResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerOptionsBuilder" /> class.
        /// </summary>
        /// <param name="sharedConnectionStringResolver">The shared connection string resolver.</param>
        public CosmosDbOptionsBuilder(ISharedConnectionStringResolver sharedConnectionStringResolver)
        {
            _sharedConnectionStringResolver = sharedConnectionStringResolver;
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <param name="sharedDataSource">The shared data source.</param>
        /// <returns>DbContextOptions.</returns>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">SharedDataSource Type not found</exception>
        public DbContextOptions GetOptions(string sharedDataSource)
        {

            var conn = new CosmosDbConnection(_sharedConnectionStringResolver.GetConnectionString(sharedDataSource));
            var builder = new DbContextOptionsBuilder<EFCoreCosmosDataContext>()
            .UseCosmos(conn.AccountEndpoint, conn.AccountKey.ToUnsecureString(), conn.DatabaseName, (t) =>
               { 
                       //when they add some options to the class we may do some stuff here
               });

            return builder.Options;
        }
    }
}
