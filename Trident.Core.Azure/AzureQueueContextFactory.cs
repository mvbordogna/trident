using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Trident.Core.Contracts.Enums;
using Trident.Core.Data.Contracts;

namespace Trident.Core.Azure
{
    /// <summary>
    /// Class AzureQueueContextFactory.
    /// Implements the <see cref="Trident.Core.Data.Contracts.ISharedContextFactory{Microsoft.WindowsAzure.Storage.Queue.CloudQueueClient}" />
    /// </summary>
    /// <seealso cref="Trident.Core.Data.Contracts.ISharedContextFactory{Microsoft.WindowsAzure.Storage.Queue.CloudQueueClient}" />
    /// <seealso cref="ExakTime.SaaS.Core.Data.Contracts.ISharedContextFactory{Microsoft.WindowsAzure.Storage.Queue.CloudQueueClient}" />
    public class AzureQueueContextFactory : ISharedContextFactory<CloudQueueClient>
    {
        /// <summary>
        /// The shared connection string resolver
        /// </summary>
        private readonly ISharedConnectionStringResolver _sharedConnectionStringResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureQueueContextFactory" /> class.
        /// </summary>
        /// <param name="sharedConnectionStringResolver">The shared connection string resolver.</param>
        public AzureQueueContextFactory(ISharedConnectionStringResolver sharedConnectionStringResolver)
        {
            _sharedConnectionStringResolver = sharedConnectionStringResolver;
        }

        /// <summary>
        /// Gets the Queue Client Context.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="dataSource">The data source.</param>
        /// <returns>CloudQueueClient.</returns>
        public CloudQueueClient Get(Type entityType, SharedDataSource dataSource)
        {
            var connectionString = _sharedConnectionStringResolver.GetConnectionString(dataSource);
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount.CreateCloudQueueClient();
        }
    }
}
