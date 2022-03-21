using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trident.Data;

namespace Trident.EFCore
{
    public class CosmosDbClient<T> : IDbClient<T>
    {
        private Container _container;
        private DataSourceInfo _dbInfo;       

        public CosmosDbClient(Container container, DataSourceInfo dbInfo)
        {
            _container = container ?? throw new System.ArgumentNullException(nameof(container));
            _dbInfo = dbInfo ?? throw new System.ArgumentNullException(nameof(dbInfo));
        }

        public async Task<IEnumerable<JObject>> ExecuteQueryAsync(string command, IDictionary<string, object> parameters = null)
        {
            var queryDefinition = new QueryDefinition(command);
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    queryDefinition.WithParameter(param.Key, param.Value);
                }
            }
           
            var resultSet = _container.GetItemQueryIterator<JObject>(queryDefinition);
            var results = await resultSet.ReadNextAsync();
            return results.Cast<JObject>();
        }
    }
}
