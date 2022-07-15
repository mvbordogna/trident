using Microsoft.Extensions.Logging;
using System.Reflection;
using Trident.Common;
using Trident.IoC;
using Trident.Samples.AzureFunctions.Configuration;
using Trident.Azure.IoC;
using Trident.Extensions;
using Trident.Contracts;
using System;
using System.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;
using Trident.Azure;
using Trident.Contracts.Configuration;

namespace Trident.Samples.AzureFunctions
{
    public class PackageModule : IoCModule
    {
        public override void Configure(IIoCProvider builder)
        {
            RegisterDefaultAssemblyScans(builder);
            builder.RegisterSelf();
            var config = new FunctionAppSettings();
            FunctionAppSettings.CoalesceEnvironmentVariables = true;
            builder.RegisterInstance<IAppSettings>(config);
            builder.RegisterInstance<IConnectionStringSettings>(config.ConnectionStrings);

            RegisterDataProviderPackages(builder,
                new Assembly[] {
                    GetType().Assembly,
                    typeof(Trident.Samples.Domain.PackageModule).Assembly
                },
                config.ConnectionStrings);

            
            //RegisterServicesForInterface(builder, typeof(ILogger), LifeSpan.InstancePerLifetimeScope);
            //builder.Register<ILogger>()
            

            builder.RegisterBehavior(() =>
            {
                return Options.Create(new WorkerOptions()
                {
                    Serializer = new JsonNetObjectSerializer()
                });
            });

            builder.RegisterAzureFunctionTypeFactory(this.TargetAssemblies);

        }


        private static void RegisterDataProviderPackages(IIoCProvider ioc, Assembly[] targetAssemblies, IConnectionStringSettings connStringManager)
        {
            connStringManager.GuardIsNotNull(nameof(connStringManager));
            IDataExtender extender = new Trident.EFCore.DataExtender();
            extender.RegisterSupportedConnections(targetAssemblies, connStringManager, ioc);
        }

        //once the Azure.DI.ServiceColectionIoCProvider is fully implemented, all the below methods can go away
        private static void RegisterServicesForInterface(IIoCProvider provider, Type interfaceType, LifeSpan lifetime)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && !p.IsAbstract && !p.IsNested & !p.IsGenericType &&
                            interfaceType.IsAssignableFrom(p));
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces().Where(p => !p.IsNested && p == interfaceType).ToList();
                foreach (var i in interfaces)
                {
                    provider.Register(type, i, lifetime);
                }
            }
        }

    }
}
