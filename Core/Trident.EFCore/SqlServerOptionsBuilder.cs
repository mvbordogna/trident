using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Trident.Data.Contracts;
using Trident.EFCore.Contracts;

namespace Trident.EFCore
{
    /// <summary>
    /// Class SqlServerOptionsBuilder.
    /// Implements the <see cref="TridentOptionsBuilder.EFCore.Contracts.IOptionsBuilder" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.EFCore.Contracts.IOptionsBuilder" />
    public class SqlServerOptionsBuilder : IOptionsBuilder
    {
#if DEBUG
        //static LoggerFactory object
        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
            //builder.AddConsole();
        });
#endif

        /// <summary>
        /// The shared connection string resolver
        /// </summary>
        private readonly ISharedConnectionStringResolver _sharedConnectionStringResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerOptionsBuilder" /> class.
        /// </summary>
        /// <param name="sharedConnectionStringResolver">The shared connection string resolver.</param>
        public SqlServerOptionsBuilder(ISharedConnectionStringResolver sharedConnectionStringResolver)
        {
            _sharedConnectionStringResolver = sharedConnectionStringResolver;
        }
        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <param name="sharedDataSource">The shared data source.</param>
        /// <returns>DbContextOptions.</returns>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">SharedDataSource Type not found</exception>
        public DbContextOptions GetOptions(string dataSource)
        {
            var connectionString = _sharedConnectionStringResolver.GetConnectionString(dataSource);

            var builder = new DbContextOptionsBuilder<EFCoreDataContext>()
                    .UseSqlServer(connectionString, (t) =>
                    {
                        t.EnableRetryOnFailure();
                    })
#if DEBUG
                    .UseLoggerFactory(loggerFactory);
#else
                    ;
#endif

            return builder.Options;
        }
    }
}
