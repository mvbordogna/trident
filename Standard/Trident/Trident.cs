﻿using System;
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

        public static TridentApplicationContext Initialize(TridentOptions options, Action<IConfigurationBuilder> configMethod = null)
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
           

            p.UsingTridentData();
            p.UsingTridentTransactions();
            if (options.EnableFileStorage) p.UsingTridentFileStorage();

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

        private static void RegisterDataProviderPackages(IIoCProvider ioc, TridentOptions config, IConnectionStringSettings connStringManager)
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

        private static void SetupConfiguration(TridentOptions config, Action<IConfigurationBuilder> configMethod = null)
        {
            if(config.AppConfiguration == null)
            {
                var builder = new ConfigurationBuilder();

                if (config.AutoDetectConfigFiles)
                {
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


                    System.Diagnostics.Debug.WriteLine($"App base Directory: {binDir}");
                    System.Diagnostics.Debug.WriteLine($"json app settings Path: {jsonConfg}");
                    System.Diagnostics.Debug.WriteLine($"xml config Path: {xmlConfig}");

                    System.Diagnostics.Debug.WriteLine($"JSON EXISTS: {hasJson}");
                    System.Diagnostics.Debug.WriteLine($"XML EXISTS: {hasXml}");
                    System.Diagnostics.Debug.WriteLine($"AutoDetectConfigFiles Enabled: {config.AutoDetectConfigFiles}");

                    if (hasJson)
                    {
                        System.Diagnostics.Debug.WriteLine($"Setting up Configuration JSON File USING {jsonConfg}");
                        builder.SetBasePath(binDir)
                               .AddJsonFile(config.JsonConfigFileName, optional: true, reloadOnChange: true);
                        config.UsingJsonConfig = true;
                        config.UsingXmlConfig = false;

                    }
                    else if (hasXml)
                    {
                        config.UsingJsonConfig = false;
                        config.UsingXmlConfig = true;
                    }
                    else if (!(hasXml || hasJson))
                    {
                        System.Diagnostics.Debug.WriteLine($"No Configuration Files Found or registered. if its is unexpected try using the configuration builder method overload parameter Action<IConfigurationBuilder> configMethod ");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"AutoDetectConfigFiles Enabled {config.AutoDetectConfigFiles}");
                    System.Diagnostics.Debug.WriteLine($"User the Action<IConfigurationBuilder> configMethod overload parameter to configure using the builder");
                }

               
                configMethod?.Invoke(builder);
                config.AppConfiguration = builder.Build();
            }
        }
    }


    public class TridentApplicationContext
    {

        internal TridentApplicationContext(IIoCServiceLocator ioc, TridentOptions config)
        {
            this.ServiceLocator = ioc;
            Configuration = config;
        }

       public  IIoCServiceLocator ServiceLocator { get; }

       public  TridentOptions Configuration { get; }

    }


    public class TridentOptions
    {

        internal string BinDir { get; set; }

        public bool EnableTransactions { get; set; } = true;

        public Assembly[] TargetAssemblies { get; set; }

        public Type[] ModuleTypes { get; set; }

        public bool ScanForModules { get; set; } = true;
    
        public bool UsingJsonConfig { get; internal set; }

        public bool UsingXmlConfig { get; internal set; }

        public bool EnableFileStorage {get; set; }

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
