using Trident.Web.Contracts;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Web.Security;
using Trident.Contracts;
using Trident.Common;

namespace Trident.Web
{
    /// <summary>
    /// Class CookieSessionManager.
    /// Implements the <see cref="Trident.Contracts.ISessionManager" />
    /// </summary>
    /// <seealso cref="Trident.Contracts.ISessionManager" />
    public class CookieSessionManager : ISessionManager
    {
        /// <summary>
        /// The cookie manager
        /// </summary>
        private readonly ICookieManager _cookieManager;
        /// <summary>
        /// The owin context resolver
        /// </summary>
        private IOwinContextResolver _owinContextResolver;
        /// <summary>
        /// The application setting
        /// </summary>
        private readonly IAppSettings _appSetting;
        /// <summary>
        /// The in memory storage
        /// </summary>
        private readonly ConcurrentDictionary<string, string> _InMemStorage = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieSessionManager"/> class.
        /// </summary>
        /// <param name="cookieManager">The cookie manager.</param>
        /// <param name="owinContextResolver">The owin context resolver.</param>
        /// <param name="appSetting">The application setting.</param>
        public CookieSessionManager(ICookieManager cookieManager,
            IOwinContextResolver owinContextResolver,
            IAppSettings appSetting)
        {
            _cookieManager = cookieManager;
            _owinContextResolver = owinContextResolver;
            _appSetting = appSetting;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        public T GetItem<T>(string key)
        {
            var val = _InMemStorage.GetOrAdd(key, (_key) => GetCookie(_key));
            return val.ChangeType<T>();
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetItem<T>(string key, T value)
        {
            if (value == null) return;
            var storedValue = value.ToString();

            _InMemStorage.AddOrUpdate(key, storedValue, (_key, oldvalue) => storedValue);
            AddOrUpdateCookie(key, storedValue);

        }


        /// <summary>
        /// Gets the cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        private string GetCookie(string key)
        {
            var context = _owinContextResolver.GetContext();
            var val = _cookieManager.GetRequestCookie(context.Request.Cookies, key);
            if(val != null)
            {
                return DecryptValue(val);
            }

            return null;
        }

        /// <summary>
        /// Adds the or update cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void AddOrUpdateCookie(string key, string value)
        {
            double minutes;
            var timeSpan = double.TryParse(_appSetting["CookieSessionTimeout"], out minutes) ?
                TimeSpan.FromMinutes(minutes) : TimeSpan.FromMinutes(525600);
            _cookieManager.AppendResponseCookie(System.Web.HttpContext.Current, key, EncryptValue(value),
                HttpOnly: true,
                Secure: true,
                Path: "/",
                Expires: DateTime.Now.Add(timeSpan)
            );
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="key">The key.</param>
        public void DeleteItem(string key)
        {
            _cookieManager.DeleteCookie(System.Web.HttpContext.Current, key);
            _InMemStorage.TryRemove(key, out var oldVal);
        }

        /// <summary>
        /// Clears all.
        /// </summary>
        public void ClearAll()
        {
            var keys = _InMemStorage.Keys.ToList();
            foreach (var key in keys)
            {
                DeleteItem(key);
            }
        }

        /// <summary>
        /// Encrypts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        private string EncryptValue(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            var encryptedTicket = MachineKey.Protect(bytes);
            return Convert.ToBase64String(encryptedTicket);
        }

        /// <summary>
        /// Decrypts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        private string DecryptValue(string value)
        {
            var encryptedTicket = Convert.FromBase64String(value);
            byte[] bytes = MachineKey.Unprotect(encryptedTicket);
            return Encoding.UTF8.GetString(bytes);

        }
    }
}
