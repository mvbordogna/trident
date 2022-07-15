using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using Trident.Contracts.Configuration;
using Trident.Extensions;

namespace Trident.Common
{
    /// <summary>
    /// Class AppSettings.
    /// Implements the <see cref="TridentOptionsBuilder.Common.IAppSettings" />
    /// </summary>
    /// <seealso cref="TridentOptionsBuilder.Common.IAppSettings" />
    public class JsonAppSettings : IAppSettings, IDisposable
    {
        /*
         * Settings in "local.settings.json" are stored in the "Values" section, meaning it must be referred to as "Values:SettingName" instead of "SettingName".
         * To synchronize how settings are retrieved (via indexer method) in Azure App Config Service and locally, flip this to true
         * to have the value accessor coalesce to "Environment.GetEnvironmentVariable" if the json provider returns null.
         */
        public static bool CoalesceEnvironmentVariables { get; set; } = false;

        public Contracts.Configuration.IConnectionStringSettings ConnectionStrings => throw new NotImplementedException();

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
                var result = settings.GetValueOrDefault(key);

                if (CoalesceEnvironmentVariables && result == null)
                    result = Environment.GetEnvironmentVariable(key);

                return result;
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
            T section = configurationRoot.Get<T>();
            configurationRoot.Bind(sectionName ?? typeof(T).Name, section);
            return section;
        }

        public string GetKeyOrDefault(string key, string defaultValue = default)
        {
            var value = DictionaryExtensions.GetValueOrDefault(settings, key, defaultValue);
            if (CoalesceEnvironmentVariables && value == default)
                value = Environment.GetEnvironmentVariable(key);
            return value;
        }
    }
}
