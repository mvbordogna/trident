using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trident.Data
{
    public interface IDbClient<T>
    {
        Task<IEnumerable<JObject>> ExecuteQueryAsync(string command, IDictionary<string, object> parameters = null);
        
    }
}
