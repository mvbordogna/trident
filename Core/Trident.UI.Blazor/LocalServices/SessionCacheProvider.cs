using Blazored.SessionStorage;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trident.UI.Blazor.Contracts.Services;
using Trident.UI.Client.Contracts.Models;

namespace Trident.UI.Blazor.Client
{
    public class SessionCacheService : ICacheService
    {
        private readonly ISessionStorageService _cache;
        private readonly Dictionary<CacheKey, string> _cacheKeys;

        public SessionCacheService(ISessionStorageService cache)
        {
            _cache = cache;
            _cacheKeys = new Dictionary<CacheKey, string>() {
                { CacheKey.Account, "" },
                { CacheKey.Messages, "Msg-" },
                { CacheKey.Permissions, "permissions" }
            };
        }

        private string CompositeKey(string identifier, CacheKey key)
        {
            return $"{_cacheKeys[key]}{identifier}";
        }

        public async Task<TItem> GetCachedValue<TItem>(string identifier, CacheKey key)
        {
            return await _cache.GetItemAsync<TItem>(CompositeKey(identifier, key));
        }

        public async Task<bool> ClearCache(string idenfitier)
        {
            await _cache.RemoveItemAsync(CompositeKey(idenfitier, CacheKey.Account));
            await _cache.RemoveItemAsync(CompositeKey(idenfitier, CacheKey.Messages));
            await _cache.RemoveItemAsync(CompositeKey(idenfitier, CacheKey.Permissions));

            return await Task.FromResult(true);
        }

        public async Task<bool> ClearCacheKey(string identifier, CacheKey key)
        {
            await _cache.RemoveItemAsync(CompositeKey(identifier, key));

            return await (Task.FromResult(true));
        }

        public async Task<bool> SetCachedValue<TItem>(string idenfitier, CacheKey key, TItem value)
        {
            try
            {
                await _cache.SetItemAsync(CompositeKey(idenfitier, key), value);
                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }
    }
}
