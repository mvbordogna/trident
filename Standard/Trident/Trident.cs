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

        public static TridentApplicationContext Initialize(TridentConfigurationOptions options, Action<IConfigurationBuilder> configMethod = null)
        {
            var targetAssemblies = options.TargetAssemblies;
            var p = Activator.CreateInstance(options.IoCProviderType = typeof(IoC.AutofacIoCProvider)) as IIoCProvider;
            p.RegisterSelf();
            
            SetupConfiguration(options, configMethod);
            Common.IConnectionStringSettings connStringManager = null;


            if (options.UsingJsonConfig)
            {
                p.UsingTridentAppSettingsJsonManager();
                p.UsingTridentConnectionStringJsonManager();
                p.RegisterInstance<IConfigurationRoot>(options.AppConfiguration);
                connStringManager = new Common.JsonConnectionStringSettings(options.AppConfiguration);
            }
            else if (options.UsingXmlConfig)
            {
                p.UsingTridentAppSettingsXmlManager();
                p.UsingTridentConnectionStringXmlManager();
                connStringManager = new Common.XmlConnectionStringSettings();
            }
            else throw new System.Configuration.ConfigurationErrorsException("Primary configuration type can be only one json (new) or xml (legecy)");

            p.UsingTridentData();
            p.UsingTridentTransactions();
            p.UsingTridentFileStorage();

            p.UsingTridentSearch(targetAssemblies);           
            p.UsingTridentRepositories(targetAssemblies);
            p.UsingTridentProviders(targetAssemblies);
            p.UsingTridentManagers(targetAssemblies);
            p.UsingTridentValidationManagers(targetAssemblies);
            p.UsingTridentValidationRules(targetAssemblies);
            p.UsingTridentWorkflowManagers(targetAssemblies);
            p.UsingTridentWorkflowTasks(targetAssemblies);
            p.UsingTridentFactories(targetAssemblies);
            p.UsingTridentResolvers(targetAssemblies);
            p.UsingTridentMapperProfiles(targetAssemblies);

            RegisterDataProviderPackages(p, options, connStringManager);
                      
            foreach (var t in options.ModuleTypes)
            {
                p.RegisterModule(t);
            }

            p.Build();

            if (options.ValidateInitialization)
                p.VerifyAndThrow();

            return new TridentApplicationContext(p.Get<IIoCServiceLocator>(), options);
        }

        private static void RegisterDataProviderPackages(IIoCProvider ioc, TridentConfigurationOptions config, IConnectionStringSettings connStringManager)
        {
            //need a fix that works for everything or multiple fixes.... dll are lazy JIT and somethimes there are on in the bin either, wtf msft
            var assList = new List<Assembly>();
            assList.AddRange(config.TargetAssemblies);
            assList.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.StartsWith("Trident.")));            
            var extenders = assList.Distinct().SelectMany(x => x.GetTypes())
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

        internal TridentApplicationContext(IIoCServiceLocator ioc, TridentConfigurationOptions config)
        {
            this.ServiceLocator = ioc;
            Configuration = config;
        }

       public  IIoCServiceLocator ServiceLocator { get; }

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
        public Type IoCProviderType { get; internal set; }
        public bool ValidateInitialization { get; set; }
    }





}
