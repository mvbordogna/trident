using AutoMapper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trident.Azure.Functions;
using Trident.IoC;

namespace Trident.Azure.IoC
{
    public static class IIoCAzureExtensions
    {
        public static int IFunctionControllerFactory { get; private set; }
        public static int AzureFunctionControllerFactory { get; private set; }

        public static IIoCProvider RegisterAzureFunctionTypeFactory(this IIoCProvider builder, Assembly[] assemblies)
        {

            var controllerTypes = assemblies.SelectMany(x => x.GetTypes())
                .Where(x => typeof(IFunctionController).IsAssignableFrom(x));

            builder.RegisterBehavior<IFunctionControllerFactory>(() =>
           {
               return new AzureFunctionControllerFactory(controllerTypes);
           }
            , LifeSpan.SingleInstance);
            return builder;
        }


        public static IIoCProvider AddAzureFunctionLogging(this IIoCProvider builder)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            builder.Populate(services);
            return builder;
        }

        public static IFunctionsHostBuilder UseLogger(this IFunctionsHostBuilder hostBuilder, Action<ILoggingBuilder, IConfiguration> configure)
        {
            var configuration = hostBuilder.GetContext().Configuration;

            hostBuilder.Services.AddLogging((config) => configure?.Invoke(config, configuration));
            return hostBuilder;
        }

        public class DynamicProfile : Profile
        {
            public DynamicProfile() { }
        }

    }
}
