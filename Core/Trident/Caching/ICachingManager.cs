using System;
using System.Threading.Tasks;

namespace Trident.Caching
{
    public interface ICachingManager
    {
        void Clear();
        object Get(string key);
        T Get<T>(string key);
        object Remove(string key);
        T Remove<T>(string key);
        void Set<T>(string key, T value);
        void Set<T>(string key, T value, DateTimeOffset absoluteExpiry);
        void Set<T>(string key, T value, TimeSpan after, ExpriryOptions option = ExpriryOptions.RollingTimespan);
        Task Set<T>(string key, TimeSpan timeSpan, Func<Task<T>> refreshFunc);  
    }
}
