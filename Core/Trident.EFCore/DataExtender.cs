using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using Trident.Common;
using Trident.Contracts;
using Trident.Core;
using Trident.Core.Data;
using Trident.Data;
using Trident.Data.Contracts;
using Trident.Data.EntityFramework.Contracts;
using Trident.EFCore.Contracts;
using Trident.IoC;

namespace Trident.EFCore
{
    public class DataExtender : IDataExtender
    {      

        private static class Providers
        {
            public const string SQL = "System.Data.SqlClient";
            internal const string CosmosDB = "CosmosDB";
        }

        private Dictionary<string, Action<IIoCProvider, TridentConfigurationOptions, ConnectionStringSettings>> ProviderSetups
            = new Dictionary<string, Action<IIoCProvider, TridentConfigurationOptions, ConnectionStringSettings>>()
            {
                {    Providers.SQL , (p, config, connStr)=>{

                     p.RegisterBehavior<IEFCoreModelBuilder>(() =>
                        {
                            return new EFCoreSqlDBAutoModelBuilder(
                                DataSourceType.Shared, connStr.Name,
                               assembliesToScan: config.TargetAssemblies);
                        }, connStr.Name, LifeSpan.SingleInstance);

                       p.RegisterNamed<SqlServerOptionsBuilder, IOptionsBuilder>(connStr.Name, LifeSpan.SingleInstance);
                       p.RegisterNamed<EFCoreDataContext, IEFDbContext>(connStr.Name);
                       p.RegisterNamed<EFCoreSharedContextFactory, ISharedContextFactory>(connStr.Name);

                    }
                },
                { Providers.CosmosDB, (p, config, connStr)=>{

                     p.RegisterBehavior<IEFCoreModelBuilder>(() =>
                        {
                            return new CosmosDBAutoModelBuilder(
                                DataSourceType.Shared, connStr.Name,
                               assembliesToScan: config.TargetAssemblies);
                        }, connStr.Name, LifeSpan.SingleInstance);

                       p.RegisterNamed<CosmosDbOptionsBuilder, IOptionsBuilder>(connStr.Name, LifeSpan.SingleInstance);
                       p.RegisterNamed<EFCoreDataContext, IEFDbContext>(connStr.Name);
                       p.RegisterNamed<EFCoreSharedContextFactory, ISharedContextFactory>(connStr.Name);

                    }
                }
            };


        public void RegisterSupportedConnections(TridentConfigurationOptions config, IConnectionStringSettings connStringManager, IIoCProvider provider)
        {  
            provider.Register<OptionsFactory, IOptionsFactory>(LifeSpan.SingleInstance);
            provider.Register<EFCoreModelBuilderFactory, IEFCoreModelBuilderFactory>(LifeSpan.SingleInstance);
            provider.Register<DBProviderAbstractFactory, IDBProviderAbstractFactory>(LifeSpan.SingleInstance);

           //this is so context objects dataSourceName does blowup if verify is used
            provider.RegisterBehavior<string>(() => "Test container Verify String", LifeSpan.SingleInstance);
            provider.RegisterBehavior<DbContextOptions>(() => new InjectionVerifyDbContextOptions());
  
           

            foreach (var conn in connStringManager)
            {
                if (ProviderSetups.ContainsKey(conn.ProviderName))
                {
                    ProviderSetups[conn.ProviderName](provider, config, conn);
                }

            }
        }


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