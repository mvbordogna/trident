using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using Trident.Common;
using Trident.Contracts.Configuration;
using Trident.Extensions;

namespace Trident.UI.Blazor
{
    public class AppSettings : IAppSettings
    {
        public static bool CoalesceEnvironmentVariables { get; set; } = false;
        protected IConfigurationRoot _appSettings;
        private Dictionary<string, string> settings;
        private IDisposable disposibleRefHandled;
        private IConfigurationSection appSettingsSection;
        private IChangeToken reloadToken;
        private List<string> settingsIndex;


        public AppSettings(IConfigurationBuilder configbuilder)
        {
            Init(configbuilder);
        }


        protected virtual void Init(IConfigurationBuilder configbuilder)
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = configbuilder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{env}.json", optional: true)
                ;

            _appSettings = builder.Build();
            ConfigChangedCallback(null);
            this.ConnectionStrings = new JsonConnectionStringSettings(_appSettings);
        }


        protected void ConfigChangedCallback(object state)
        {
            using (disposibleRefHandled) { }
            appSettingsSection = _appSettings.GetSection("AppSettings");
            reloadToken = appSettingsSection.GetReloadToken();
            disposibleRefHandled = reloadToken.RegisterChangeCallback(ConfigChangedCallback, null);
            settings = appSettingsSection.GetChildren().ToDictionary(x => x.Key, x => x.Value);
            settingsIndex = settings.Keys.ToList();
        }


        public string this[string key] => _appSettings[key];


        public T GetSection<T>(string sectionName = null)
              where T : class
        {
            T section = _appSettings.GetSection(sectionName).Get<T>();

           // _appSettings.Bind(sectionName ?? typeof(T).Name, section);
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

    }



}
