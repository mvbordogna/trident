using System;
using System.Collections.Generic;
using System.Configuration;
using Trident.Common;
using Trident.Contracts;
using Trident.Data.Contracts;
using Trident.IoC;
using Trident.Rest.Contracts;
using Trident.Data.RestSharp;

namespace Trident.Rest
{
    public class RestDataExtender : IDataExtender
    {

        private static class Providers
        {
            public const string REST = "Rest";
        }

        private Dictionary<string, Action<IIoCProvider, TridentConfigurationOptions, ConnectionStringSettings>> ProviderSetups
            = new Dictionary<string, Action<IIoCProvider, TridentConfigurationOptions, ConnectionStringSettings>>()
            {
                {    Providers.REST , (p, config, connStr)=>{

                    p.RegisterNamed<RestContextFactory, ISharedContextFactory>(connStr.Name, LifeSpan.SingleInstance);
                    p.RegisterNamed<RestSharpAuthenticationProvider, IRestAuthenticationProvider>(connStr.Name);
                    p.RegisterNamed<RestContext, IRestContext>(connStr.Name);


                    }
                }
            };


        public void RegisterSupportedConnections(TridentConfigurationOptions config, IConnectionStringSettings connStringManager, IIoCProvider provider)
        {
            provider.Register<RestConnectionStringResolver, IRestConnectionStringResolver>(LifeSpan.SingleInstance);
            provider.Register<RestSharpClient, IRestClient>( LifeSpan.SingleInstance);

            //this is so context objects dataSourceName does blowup if verify is used
            provider.RegisterBehavior<string>(() => "Test container Verify String", LifeSpan.SingleInstance);
      
            foreach (var conn in connStringManager)
            {
                if (ProviderSetups.ContainsKey(conn.ProviderName))
                {
                    ProviderSetups[conn.ProviderName](provider, config, conn);
                }
            }
        }
    }
}
