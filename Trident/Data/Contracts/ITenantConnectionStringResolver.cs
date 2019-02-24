using System.Data;
using System.Data.Common;

namespace Trident.Data.Contracts
{
    public interface ITenantConnectionStringResolver
    {
        string GetConnectionString(string key);

        IDbConnection GetConnection(string key);
    }
}