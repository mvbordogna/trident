namespace Trident.Contracts.Enums
/// <summary>
/// Enum SharedDataSource
/// </summary>
{
    public enum SharedDataSource
    {
        /// <summary>
        /// The undefined
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// The default database
        /// </summary>
        DefaultDB = 1,       
        /// The trident cosmos database
        /// </summary>
        DefaultCosmosDB = 3,
        /// <summary>
        /// The default EFCore database
        /// </summary>
        DefualtEFCoreDb = 4,
        /// <summary>
        /// The azure storage queues
        /// </summary>
        AzureStorageQueues = 5
    }
}
