using System;
using System.Collections.Generic;
using System.Reflection;
using Trident.Contracts;
using Trident.Contracts.Configuration;
using Trident.Data.Contracts;
using Trident.Data.RestSharp;
using Trident.IoC;
using Trident.Rest.Contracts;

namespace Trident.Rest
{
    public class RestDataExtender : IDataExtender
    {

        private static class Providers
        {
            public const string REST = "Rest";
        }

        private Dictionary<string, Action<IIoCProvider, Assembly[], ConnectionStringSettings>> ProviderSetups
            = new Dictionary<string, Action<IIoCProvider, Assembly[], ConnectionStringSettings>>()
            {
                {    Providers.REST , (p, config, connStr)=>{

                    p.RegisterNamed<RestContextFactory, ISharedContextFactory>(connStr.Name, LifeSpan.SingleInstance);
                    p.RegisterNamed<RestSharpAuthenticationProvider, IRestAuthenticationProvider>(connStr.Name);
                    p.RegisterNamed<RestContext, IRestContext>(connStr.Name);


                    }
                }
            };


        public void RegisterSupportedConnections(Assembly[] targetAssemblies, IConnectionStringSettings connStringManager, IIoCProvider provider)
        {
            provider.Register<RestConnectionStringResolver, IRestConnectionStringResolver>(LifeSpan.SingleInstance);
            provider.Register<RestSharpClient, IRestClient>(LifeSpan.SingleInstance);

            //this is so context objects dataSourceName does blowup if verify is used
            provider.RegisterBehavior<string>(() => "Test container Verify String", LifeSpan.SingleInstance);

            foreach (var conn in connStringManager)
            {
                if (ProviderSetups.ContainsKey(conn.ProviderName))
                {
                    ProviderSetups[conn.ProviderName](provider, targetAssemblies, conn);
                }
            }
        }
    }
}
