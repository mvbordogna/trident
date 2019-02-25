using Microsoft.EntityFrameworkCore;
using Trident.Contracts.Enums;
using Trident.Data.Contracts;
using Trident.Data.EntityFramework.EFCore.Contracts;
using System;

namespace Trident.Data.EntityFramework.EFCore
{
    /// <summary>
    /// Class SqlServerOptionsBuilder.
    /// Implements the <see cref="Trident.Data.EntityFramework.EFCore.Contracts.IOptionsBuilder" />
    /// </summary>
    /// <seealso cref="Trident.Data.EntityFramework.EFCore.Contracts.IOptionsBuilder" />
    public class SqlServerOptionsBuilder : IOptionsBuilder
    {
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
        public DbContextOptions GetOptions(string sharedDataSource)
        {
            if (Enum.TryParse<SharedDataSource>(sharedDataSource, out var dataSource))
            {
                var connectionString = _sharedConnectionStringResolver.GetConnectionString(dataSource);              

                var builder = new DbContextOptionsBuilder<EFCoreDataContext>()
                        .UseSqlServer(connectionString, (t) =>
                        {
                            t.EnableRetryOnFailure();
                        });

                return builder.Options;
            }

            throw new System.Configuration.ConfigurationErrorsException("SharedDataSource Type not found");
        }
    }
}
