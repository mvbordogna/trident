﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using Trident.Common;
using Trident.Contracts;
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

        private Dictionary<string, Action<IIoCProvider, TridentOptions, ConnectionStringSettings>> ProviderSetups
            = new Dictionary<string, Action<IIoCProvider, TridentOptions, ConnectionStringSettings>>()
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
                        //TODO: put back to normal - workaround context for efcore issues when runing async still ocurring
                        //p.RegisterNamed<EFCoreDataContext, IEFDbContext>(connStr.Name);
                       p.RegisterNamed<AsyncWorkaround.EFCoreAsyncWorkAroundDbContext, IEFDbContext>(connStr.Name);
                       p.RegisterNamed<EFCoreSharedContextFactory, ISharedContextFactory>(connStr.Name);
                    }
                }
            };


        public void RegisterSupportedConnections(TridentOptions config, IConnectionStringSettings connStringManager, IIoCProvider provider)
        {
            provider.Register<OptionsFactory, IOptionsFactory>(LifeSpan.SingleInstance);
            provider.Register<EFCoreModelBuilderFactory, IEFCoreModelBuilderFactory>(LifeSpan.SingleInstance);
            provider.Register<DBProviderAbstractFactory, IDBProviderAbstractFactory>(LifeSpan.SingleInstance);
            provider.Register<EntityMapFactory, IEntityMapFactory>(LifeSpan.SingleInstance);
            provider.RegisterAll<IEntityMapper>(config.TargetAssemblies, LifeSpan.SingleInstance);

            //this is so context objects dataSourceName does blowup if verify is used
            provider.RegisterBehavior<string>(() => "Test container Verify String", LifeSpan.SingleInstance);
            provider.RegisterBehavior<DbContextOptions>(() => new InjectionVerifyDbContextOptions());

            if (connStringManager.Count() > 0)
            {
                foreach (var conn in connStringManager)
                {
                    var pn = conn.ProviderName.ToLower();

                    if (ProviderSetups.ContainsKey(pn))
                    {
                        ProviderSetups[pn](provider, config, conn);
                    }
                }
            }
            else
            {
                //add warning
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