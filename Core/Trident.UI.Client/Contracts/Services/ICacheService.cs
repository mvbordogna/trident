using System.Threading.Tasks;
using Trident.UI.Client.Contracts.Models;

namespace Trident.UI.Blazor.Contracts.Services
{
    public interface ICacheService
    {
        public Task<TItem> GetCachedValue<TItem>(string identifier, CacheKey key);
        public Task<bool> ClearCache(string identifier);
        public Task<bool> ClearCacheKey(string identifier, CacheKey key);
        public Task<bool> SetCachedValue<TItem>(string idenfitier, CacheKey key, TItem value);
    }
}
