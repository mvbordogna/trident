using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Trident.Contracts;
using C = Trident.Contracts.Configuration;
using Trident.Data;
using Trident.Data.Contracts;
using Trident.EFCore.Contracts;
using Trident.IoC;

namespace Trident.EFCore
{
    public class DataExtender : IDataExtender
    {
        private static class Providers
        {
            public const string SQL = "system.data.sqlclient";
            internal const string CosmosDB = "cosmosdb";
        }

        private Dictionary<string, Action<IIoCProvider, Assembly[], C.ConnectionStringSettings>> ProviderSetups
            = new Dictionary<string, Action<IIoCProvider, Assembly[], C.ConnectionStringSettings>>()
            {
                {    Providers.SQL , (p, targetAssemblies, connStr)=>{

                     p.RegisterBehavior<IEFCoreModelBuilder>(() =>
                        {
                            return new EFCoreSqlDBAutoModelBuilder(
                                DataSourceType.Shared, connStr.Name,
                               assembliesToScan: targetAssemblies);
                        }, connStr.Name, LifeSpan.SingleInstance);

                       p.RegisterNamed<SqlServerOptionsBuilder, IOptionsBuilder>(connStr.Name, LifeSpan.SingleInstance);
                       p.RegisterNamed<EFCoreDataContext, IEFDbContext>(connStr.Name);
                       p.RegisterNamed<EFCoreSharedContextFactory, ISharedContextFactory>(connStr.Name);

                    }
                },
                { Providers.CosmosDB, (p, targetAssemblies, connStr)=>{

                     p.RegisterBehavior<IEFCoreModelBuilder>(() =>
                        {

                            return new CosmosDBAutoModelBuilder(
                                DataSourceType.Shared, connStr.Name,
                                assembliesToScan: targetAssemblies);

                        }, connStr.Name, LifeSpan.SingleInstance);


                       p.RegisterNamed<CosmosDbOptionsBuilder, IOptionsBuilder>(connStr.Name, LifeSpan.SingleInstance);
                       p.RegisterNamed<EFCoreCosmosDataContext, IEFDbContext>(connStr.Name);
                       p.RegisterNamed<EFCoreSharedContextFactory, ISharedContextFactory>(connStr.Name);
                    }
                }
            };


        public void RegisterSupportedConnections(Assembly[] targetAssemblies, C.IConnectionStringSettings connStringManager, IIoCProvider provider)
        {
            provider.Register<OptionsFactory, IOptionsFactory>(LifeSpan.SingleInstance);
            provider.Register<EFCoreModelBuilderFactory, IEFCoreModelBuilderFactory>(LifeSpan.SingleInstance);
            provider.Register<DBProviderAbstractFactory, IDBProviderAbstractFactory>(LifeSpan.SingleInstance);
            provider.Register<EntityMapFactory, IEntityMapFactory>(LifeSpan.SingleInstance);
            provider.RegisterAll<IEntityMapper>(targetAssemblies, LifeSpan.SingleInstance);

            //this is so context objects dataSourceName does blowup if verify is used
            provider.RegisterBehavior<string>(() => "Test container Verify String", LifeSpan.SingleInstance);
            provider.RegisterBehavior<DbContextOptions>(() => new InjectionVerifyDbContextOptions());

            if (connStringManager != null && connStringManager?.Count() > 0)
            {
                foreach (var conn in connStringManager)
                {
                    if (conn.ProviderName != null)
                    {
                        var pn = conn.ProviderName.ToLower();

                        if (ProviderSetups.ContainsKey(pn))
                        {
                            ProviderSetups[pn](provider, targetAssemblies, conn);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine($"Provider name not found in connection string \"{conn.Name}\".");
                    }
                }
            }
            else
            {
                if (!AllowNoDatasources)
                {
                    //add warning
                    var msg = "No Connection Strings Configured";
                    System.Diagnostics.Trace.WriteLine(msg);
                    throw new Exception(msg);
                }
            }
        }

        public static bool AllowNoDatasources { get; set; } = false; // force field function does not use connection strings.

        private class InjectionVerifyDbContextOptions : DbContextOptions
        {
            public InjectionVerifyDbContextOptions() : base(new Dictionary<Type, IDbContextOptionsExtension>())
            {
            }

            public override Type ContextType => typeof(EFCoreDataContext);

            public override DbContextOptions WithExtension<TExtension>(TExtension extension)
            {
                return this;
            }
        }

    }
}