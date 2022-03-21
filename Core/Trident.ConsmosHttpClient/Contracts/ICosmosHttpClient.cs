using System.Threading.Tasks;
using System.Collections.Generic;

namespace Trident.Cosmos.Contracts
{
    public interface ICosmosHttpClient
    {
        Task<List<CosmosDatabase>> GetAllDatabases();
        Task<CosmosDatabase> GetDatabaseById(string databaseId);
        Task<List<CosmosCollection>> GetAllCollections(string databaseId);
        Task<CosmosCollection> GetCollectionById(string databaseId, string collectionId);
        Task<List<T>> GetAllDocuments<T>(string databaseId, string collectionId);
        Task<T> GetDocumentById<T>(string databaseId, string collectionId, string partitionKey, string documentId);
        Task<List<T>> ExecuteQuery<T>(string databaseId, string collectionId, string query);
    }
}
