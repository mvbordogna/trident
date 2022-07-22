using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Trident.Caching
{
    public class InMemoryCachingManager : IDisposable, ICachingManager
    {
        private readonly ConcurrentDictionary<string, CacheItem> _cache = new ConcurrentDictionary<string, CacheItem>();
        private readonly CancellationToken _watcherCanelToken;
        private Dictionary<ExpriryOptions, Func<CacheItem, bool>> expireStategies =
            new Dictionary<ExpriryOptions, Func<CacheItem, bool>>()
            {
                { ExpriryOptions.Never, (x)=> false },
                { ExpriryOptions.AbsoluteTime, (x) => x.ExpireAt <= DateTimeOffset.Now},
                { ExpriryOptions.ExplicitTimespan,(x) => x.EntryTimestamp.Add(x.ExpireAfter) <= DateTimeOffset.Now},
                { ExpriryOptions.AutoRefreshOnExpire,(x) => x.EntryTimestamp.Add(x.ExpireAfter) <= DateTimeOffset.Now},
                { ExpriryOptions.RollingTimespan, (x) => x.LastRetrivedTimestamp.Add(x.ExpireAfter) <= DateTimeOffset.Now}
            };


        public InMemoryCachingManager()
        {
            _watcherCanelToken = new CancellationToken();
        }

        public T Get<T>(string key)
        {
            CacheItem item;
            if (_cache.TryGetValue(key, out item) && item != null)
            {
                item.LastRetrivedTimestamp = DateTimeOffset.Now;
                return (T)item.Value;
            }

            return default(T);
        }

        public object Get(string key)
        {
            CacheItem item;
            if (_cache.TryGetValue(key, out item) && item != null)
            {
                item.LastRetrivedTimestamp = DateTimeOffset.Now;
                return item.Value;
            }

            return null;
        }

        public void Set<T>(string key, T value, DateTimeOffset absoluteExpiry)
        {
            Func<string, CacheItem, CacheItem> SetItem = (string k, CacheItem x) =>
            {
                x.ExpireAt = absoluteExpiry;
                x.ExpriryOptions = ExpriryOptions.AbsoluteTime;
                x.LastRetrivedTimestamp = DateTimeOffset.Now;
                x.Value = value;
                return x;
            };

            this._cache.AddOrUpdate(key, SetItem(key, new CacheItem()
            {
                EntryTimestamp = DateTimeOffset.Now
            }), SetItem);
        }

        public void Set<T>(string key, T value, TimeSpan after,
            ExpriryOptions option = ExpriryOptions.RollingTimespan)
        {
            Func<string, CacheItem, CacheItem> SetItem = (string k, CacheItem x) =>
            {
                x.ExpireAfter = after;
                x.ExpriryOptions = option;
                x.LastRetrivedTimestamp = DateTimeOffset.Now;
                x.Value = value;
                return x;
            };

            this._cache.AddOrUpdate(key, SetItem(key, new CacheItem()
            {
                EntryTimestamp = DateTimeOffset.Now
            }), SetItem);
        }


        public void Set<T>(string key, T value)
        {
            Func<string, CacheItem, CacheItem> SetItem = (string k, CacheItem x) =>
            {
                x.ExpriryOptions = ExpriryOptions.Never;
                x.LastRetrivedTimestamp = DateTimeOffset.Now;
                x.Value = value;
                return x;
            };

            this._cache.AddOrUpdate(key, SetItem(key, new CacheItem()
            {
                EntryTimestamp = DateTimeOffset.Now
            }), SetItem);
        }


        public async Task Set<T>(string key, TimeSpan after, Func<Task<T>> refreshFunc)
        {
            var val = await refreshFunc();

            Func<string, CacheItem, CacheItem> SetItem = (string k, CacheItem x) =>
            {
                x.ExpireAfter = after;
                x.ExpriryOptions = ExpriryOptions.AutoRefreshOnExpire;
                x.LastRetrivedTimestamp = DateTimeOffset.Now;
                ((CacheItem<T>)x).RefreshFunc = refreshFunc;
                x.Value = val;
                return x;
            };

            this._cache.AddOrUpdate(key, SetItem(key, new CacheItem<T>()
            {
                EntryTimestamp = DateTimeOffset.Now
            }), SetItem);
        }

        public T Remove<T>(string key)
        {
            CacheItem item;

            if (_cache.TryRemove(key, out item) && item != null)
            {
                return (T)item.Value;
            }

            return default(T);
        }

        public object Remove(string key)
        {
            CacheItem item;
            if (_cache.TryRemove(key, out item) && item != null)
            {
                item.LastRetrivedTimestamp = DateTimeOffset.Now;
                return item.Value;
            }

            return null;
        }


        public void Clear()
        {
            _cache.Clear();
        }


        private void StartExpirationWatcher()
        {
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(10000, _watcherCanelToken);
                        await RemoveExpiredItems();
                    }
                }, _watcherCanelToken);
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception)
            {
                
            }
        }

        private async Task RemoveExpiredItems()
        {
            var itemsToRemove = _cache.Where(x => expireStategies[x.Value.ExpriryOptions](x.Value)).ToList();

            foreach (var item in itemsToRemove)
            {
                if (item.Value.ExpriryOptions != ExpriryOptions.AutoRefreshOnExpire)
                {
                    _cache.TryRemove(item.Key, out var cItem);
                }
                else
                {
                    await item.Value.RefreshValue();
                }
            }

        }


        private class CacheItem
        {
            public object Value { get; set; }

            public DateTimeOffset EntryTimestamp { get; set; }

            public DateTimeOffset LastRetrivedTimestamp { get; set; }

            public TimeSpan ExpireAfter { get; set; }

            public ExpriryOptions ExpriryOptions { get; set; }
            public DateTimeOffset ExpireAt { get; set; }

            public virtual Task RefreshValue()
            {
                return Task.CompletedTask;
            }
        }

        private class CacheItem<T> : CacheItem
        {
            public Func<Task<T>> RefreshFunc { get; set; }

            public override async Task RefreshValue()
            {
                this.EntryTimestamp = DateTimeOffset.Now;
                this.Value = await RefreshFunc();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._watcherCanelToken.ThrowIfCancellationRequested();
                    this._cache.Clear();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion


    }

    public enum ExpriryOptions
    {
        Never = 0,
        RollingTimespan = 1,
        ExplicitTimespan = 2,
        AbsoluteTime = 3,
        AutoRefreshOnExpire = 4
    }

}
