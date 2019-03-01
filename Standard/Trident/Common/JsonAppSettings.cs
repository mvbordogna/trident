using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using Trident.Extensions;

namespace Trident.Common
{
    /// <summary>
    /// Class AppSettings.
    /// Implements the <see cref="Trident.Common.IAppSettings" />
    /// </summary>
    /// <seealso cref="Trident.Common.IAppSettings" />
    public class JsonAppSettings : IAppSettings, IDisposable
    {
        private readonly IConfigurationRoot configurationRoot;
        private IConfigurationSection appSettingsSection;
        private IChangeToken reloadToken;
        private IDisposable disposibleRefHandled;
        private Dictionary<string, string> settings;
        private List<string> settingsIndex;

        public JsonAppSettings(IConfigurationRoot configurationRoot)
        {
            this.configurationRoot = configurationRoot;
            ConfigChangedCallback(null);

        }

        private void ConfigChangedCallback(object state)
        {
            using (disposibleRefHandled) { }
            appSettingsSection = this.configurationRoot.GetSection("AppSettings");
            reloadToken = appSettingsSection.GetReloadToken();
            disposibleRefHandled = reloadToken.RegisterChangeCallback(this.ConfigChangedCallback, null);
            settings = appSettingsSection.GetChildren().ToDictionary(x => x.Key, x => x.Value);
            settingsIndex = settings.Keys.ToList();
        }

        public void Dispose()
        {
            using (disposibleRefHandled) { }
        }

        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string this[string key]
        {
            get
            {
                return settings.GetValueOrDefault(key);
            }
        }
        /// <summary>
        /// Gets the <see cref="System.String" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        public string this[int index]
        {
            get
            {
                if (settingsIndex.Count <= index) return null;
                return settings.GetValueOrDefault(settingsIndex[index]);
            }
        }

        public T GetSection<T>(string sectionName = null)
           where T : class
        {
            return configurationRoot.GetValue(typeof(T), sectionName ?? typeof(T).Name) as T;
        }

    }
}
