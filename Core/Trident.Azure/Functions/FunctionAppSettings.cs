using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using Trident.Common;
using Trident.Extensions;
using Az = Azure;

namespace Trident.Azure.Functions
{
    public abstract class FunctionAppSettingsBase : IAppSettings
    {

        /*
           * Settings in "local.settings.json" are stored in the "Values" section, meaning it must be referred to as "Values:SettingName" instead of "SettingName".
           * To synchronize how settings are retrieved (via indexer method) in Azure App Config Service and locally, flip this to true
           * to have the value accessor coalesce to "Environment.GetEnvironmentVariable" if the json provider returns null.
           */
        public static bool CoalesceEnvironmentVariables { get; set; } = false;

        protected IConfigurationRoot _appSettings;
        private Dictionary<string, string> settings;
        private IDisposable disposibleRefHandled;
        private IConfigurationSection appSettingsSection;
        private IChangeToken reloadToken;
        private List<string> settingsIndex;


        public FunctionAppSettingsBase()
        {
            Init();
        }


        protected virtual void Init()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("KeyVaultUri")))
            {
                builder.AddAzureKeyVault(new Uri(Environment.GetEnvironmentVariable("KeyVaultUri")), new DefaultAzureCredential());
            }

            _appSettings = builder.Build();
            ConnectionStrings = new JsonConnectionStringSettings(_appSettings);
            ConfigChangedCallback(null);
        }


        protected void ConfigChangedCallback(object state)
        {
            using (disposibleRefHandled) { }
            appSettingsSection = this._appSettings.GetSection("AppSettings");
            reloadToken = appSettingsSection.GetReloadToken();
            disposibleRefHandled = reloadToken.RegisterChangeCallback(this.ConfigChangedCallback, null);
            settings = appSettingsSection.GetChildren().ToDictionary(x => x.Key, x => x.Value);
            settingsIndex = settings.Keys.ToList();
        }


        public string this[string key] => _appSettings[key];


        public T GetSection<T>(string sectionName = null)
              where T : class
        {
            T section = _appSettings.Get<T>();
            _appSettings.Bind(sectionName ?? typeof(T).Name, section);
            return section;
        }

        public string GetKeyOrDefault(string key, string defaultValue = default)
        {
            var value = DictionaryExtensions.GetValueOrDefault(settings, key, defaultValue);
            if (CoalesceEnvironmentVariables && value == default)
                value = Environment.GetEnvironmentVariable(key);
            return value;
        }


        public IConnectionStringSettings ConnectionStrings { get; private set; }

        public string this[int index] => settings.Values.ToList()[index];
    }



}
