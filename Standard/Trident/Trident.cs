using System;
using System.IO;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Trident.Common;
using Trident.IoC;
using System.Linq;
using Trident.Contracts;
using System.Collections.Generic;

namespace Trident
{
    public static class Trident
    {

        public static TridentApplicationContext Initialize(TridentConfigurationOptions config, Action<IConfigurationBuilder> configMethod = null)
        {
            var targetAssemblies = config.TargetAssemblies;
            var p = new IoC.AutofacIoCProvider();
            p.RegisterSelf();
            var builder = p.Builder;
            SetupConfiguration(config, configMethod);
            Common.IConnectionStringSettings connStringManager = null;


            if (config.UsingJsonConfig)
            {
                builder.UsingTridentAppSettingsJsonManager();
                builder.UsingTridentConnectionStringJsonManager();
                connStringManager = new Common.JsonConnectionStringSettings(config.AppConfiguration);
            }
            else if (config.UsingXmlConfig)
            {
                builder.UsingTridentAppSettingsXmlManager();
                builder.UsingTridentConnectionStringXmlManager();
                connStringManager = new Common.XmlConnectionStringSettings();
            }
            else throw new System.Configuration.ConfigurationErrorsException("Primary configuration type can be only one json (new) or xml (legecy)");
            
        
            builder.UsingTridentSearch();
            builder.UsingTridentTransactions();
            builder.UsingTridentRepositories(targetAssemblies);
            builder.UsingTridentProviders(targetAssemblies);
            builder.UsingTridentManagers(targetAssemblies);
            builder.UsingTridentValidationManagers(targetAssemblies);
            builder.UsingTridentValidationRules(targetAssemblies);
            builder.UsingTridentWorkflowManagers(targetAssemblies);
            builder.UsingTridentWorkflowTasks(targetAssemblies);
            builder.UsingTridentFactories(targetAssemblies);
            builder.UsingTridentResolvers(targetAssemblies);
            builder.UsingTridentMapperProfiles(targetAssemblies);
            builder.UsingTridentFileStorage();     
            builder.UsingTridentData();
            RegisterDataProviderPackages(p, config, connStringManager);
                      
            foreach (var t in config.ModuleTypes)
            {
                p.RegisterModule(t);
            }

            p.Build();

            return new TridentApplicationContext(p, config);
        }

        private static void RegisterDataProviderPackages(IIoCProvider ioc, TridentConfigurationOptions config, IConnectionStringSettings connStringManager)
        {
            var assList = new List<Assembly>();
            assList.AddRange(config.TargetAssemblies);
            assList.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.StartsWith("Trident.")));
            var extenders = assList.SelectMany(x => x.GetTypes())
                   .Where(x => typeof(IDataExtender).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                   .Select(x => Activator.CreateInstance(x) as IDataExtender)
                   .ToList();
                   
            foreach (var x in extenders)
            {
                x.RegisterSupportedConnections(config, connStringManager, ioc);
            }
        }

        private static void SetupConfiguration(TridentConfigurationOptions config, Action<IConfigurationBuilder> configMethod = null) {

            var entryAssembly = Assembly.GetEntryAssembly();
            var tridentAssebmly = Assembly.GetExecutingAssembly();
            var binDir = Path.GetDirectoryName((entryAssembly ?? tridentAssebmly).Location);
            config.BinDir = binDir;
            var xmlConfigName = (entryAssembly != null)
                ? $"{Path.GetFileName(entryAssembly.Location) }.config"
                : "web.config";

            config.JsonConfigFileName = config.JsonConfigFileName ?? "appsettings.json";
            var jsonConfg = Path.Combine(binDir, config.JsonConfigFileName);
            var xmlConfig = Path.Combine(binDir, xmlConfigName);
       
            var hasXml = File.Exists(xmlConfig);
            var hasJson = File.Exists(jsonConfg);

            if (config.AutoDetectConfigFiles)
            {
                if (!hasXml && hasJson && config.AppConfiguration == null)
                {                    
                    var builder = new ConfigurationBuilder()
                            .SetBasePath(binDir)
                            .AddJsonFile(config.JsonConfigFileName, optional: true, reloadOnChange: true);

                    configMethod?.Invoke(builder);
                    config.AppConfiguration = builder.Build();
                    config.UsingJsonConfig = true;
                    config.UsingXmlConfig = false;
                }
                else
                {
                    config.UsingJsonConfig = false;
                    config.UsingXmlConfig = true;
                }
            }
            else if( config.AppConfiguration != null && hasJson)
            {
                config.UsingJsonConfig = true;
                config.UsingXmlConfig = false;
            }
            else
            {
                config.UsingJsonConfig = false;
                config.UsingXmlConfig = true;
            }
          
        }
    }


    public class TridentApplicationContext
    {

        internal TridentApplicationContext(IIoCProvider ioc, TridentConfigurationOptions config)
        {
            this.IocProvider = ioc;
            Configuration = config;
        }

       public  IIoCProvider IocProvider { get; }

       public  TridentConfigurationOptions Configuration { get; }

    }


    public class TridentConfigurationOptions
    {

        internal string BinDir { get; set; }

        public bool EnableTransactions { get; set; } = true;

        public Assembly[] TargetAssemblies { get; set; }

        public Type[] ModuleTypes { get; set; }

        public bool ScanForModules { get; set; } = true;
    
        public bool UsingJsonConfig { get; internal set; }

        public bool UsingXmlConfig { get; internal set; }


        /// <summary>
        /// When True Trident will look for app.config or appsettings.config files to load for the configuration
        /// this options is available for .Net Core and Frameowrk
        /// </summary>
        public bool AutoDetectConfigFiles { get; set; }
        public IConfigurationRoot AppConfiguration { get; set; }
        public string JsonConfigFileName { get; set; }

    }





}
