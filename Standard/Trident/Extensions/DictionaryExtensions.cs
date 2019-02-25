using System.Collections.Generic;

namespace Trident.Extensions
{
    /// <summary>
    /// Class DictionaryExtensions.
    /// </summary>
    public static class DictionaryExtensions
    {

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>TValue.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> target, TKey key, TValue defaultValue = default(TValue))
        {
            var defaultVal = !object.Equals(defaultValue, default(TValue)) ? defaultValue : default(TValue);
            if (target == null) return defaultVal;
            return (target.ContainsKey(key))
                ? target[key]
                : defaultVal;
        }


    }
}
